using Microsoft.Data.SqlClient;
using System.Text;

namespace StartUp
{
    public class Program
    {
        static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(Config.ConnectionString);

            sqlConnection.Open();

            int vilianId = int.Parse(Console.ReadLine());
            //string countryName = Console.ReadLine();
            //int[] minionsId = Console.ReadLine()
            //    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            //    .Select(int.Parse)
            //    .ToArray();


            string result = GetInformationAboutMinionAndTheirVilian(sqlConnection, vilianId);


            Console.WriteLine(result);
        }

        // Problem 02 
        private static async Task<string> GetAllViliansWithTheirMinions(SqlConnection sqlConnection)
        {
            StringBuilder sb = new StringBuilder();

            SqlCommand sqlCommand = new SqlCommand(
                SqlQueries.GetAllVilliansAndCountOfTheirMinions, sqlConnection);

            SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();

            while (reader.Read())
            {
                string vilianName = (string)reader["Name"];
                int minionsCount = (int)reader["MinionsCount"];

                sb.AppendLine($"{vilianName}-{minionsCount}");
            }

            return sb.ToString().Trim();
        }

        // Problem 03
        private static string GetInformationAboutMinionAndTheirVilian(
            SqlConnection sqlConnection, int vilianId)
        {
            StringBuilder sb = new StringBuilder();

            SqlCommand getVilianId = new SqlCommand(
                SqlQueries.GetVilianNameById, sqlConnection);

            getVilianId.Parameters.AddWithValue("@Id", vilianId);

            string vilianName = (string)getVilianId.ExecuteScalar();

            if (string.IsNullOrEmpty(vilianName))
            {
                return $"No villain with ID {vilianId} exists in the database.";
            }

            SqlCommand getMinionsInfo = new SqlCommand(
                SqlQueries.GetMinionsInformation, sqlConnection);

            getMinionsInfo.Parameters.AddWithValue("@Id", vilianId);

            SqlDataReader minionsReader = getMinionsInfo.ExecuteReader();

            sb.AppendLine($"Villain: {vilianName}");

            if (!minionsReader.HasRows)
            {
                sb.AppendLine($"(no minions)");
            }
            else
            {
                while (minionsReader.Read())
                {
                    long rowNum = (long)minionsReader["RowNum"];
                    string minionName = (string)minionsReader["Name"];
                    int minionAge = (int)minionsReader["Age"];

                    sb.AppendLine($"{rowNum}. {minionName} {minionAge}");
                }
            }

            return sb.ToString().Trim();
        }

        //Problem 04
        private static string AddNewMinion(SqlConnection sqlConnection,
            string[] minionInfo, string villainName)
        {
            StringBuilder output = new StringBuilder();

            string minionName = minionInfo[0];
            int minionAge = int.Parse(minionInfo[1]);
            string townName = minionInfo[2];

            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                int townId = GetTownId(sqlConnection, sqlTransaction, output, townName);
                int villainId = GetVillainId(sqlConnection, sqlTransaction, output, villainName);
                int minionId = AddMinionAndGetId(sqlConnection, sqlTransaction, minionName, minionAge, townId);

                string addMinionToVillainQuery = @"INSERT INTO [MinionsVillains]([MinionId], [VillainId])
                                                        VALUES
                                                        (@MinionId, @VillainId)";
                SqlCommand addMinionToVillainCmd =
                    new SqlCommand(addMinionToVillainQuery, sqlConnection, sqlTransaction);
                addMinionToVillainCmd.Parameters.AddWithValue("@MinionId", minionId);
                addMinionToVillainCmd.Parameters.AddWithValue("@VillainId", villainId);

                addMinionToVillainCmd.ExecuteNonQuery();
                output.AppendLine($"Successfully added {minionName} to be minion of {villainName}.");

                sqlTransaction.Commit();
            }
            catch (Exception e)
            {
                sqlTransaction.Rollback();
                return e.ToString();
            }

            return output.ToString().TrimEnd();
        }

        private static int GetTownId(SqlConnection sqlConnection, SqlTransaction sqlTransaction, StringBuilder output, string townName)
        {
            string townIdQuery = @"SELECT [Id]
                                     FROM [Towns]
                                    WHERE [Name] = @TownName";
            SqlCommand townIdCmd = new SqlCommand(townIdQuery, sqlConnection, sqlTransaction);
            townIdCmd.Parameters.AddWithValue("@TownName", townName);

            object townIdObj = townIdCmd.ExecuteScalar();
            if (townIdObj == null)
            {
                string addTownQuery = @"INSERT INTO [Towns]([Name])
                                             VALUES
                                                    (@TownName)";
                SqlCommand addTownCmd = new SqlCommand(addTownQuery, sqlConnection, sqlTransaction);
                addTownCmd.Parameters.AddWithValue("@TownName", townName);

                addTownCmd.ExecuteNonQuery();

                output.AppendLine($"Town {townName} was added to the database.");

                townIdObj = townIdCmd.ExecuteScalar();
            }

            return (int)townIdObj;
        }

        private static int GetVillainId(SqlConnection sqlConnection, SqlTransaction sqlTransaction,
            StringBuilder output, string villainName)
        {
            string villainIdQuery = @"SELECT [Id]
                                          FROM [Villains]
                                         WHERE [Name] = @VillainName";
            SqlCommand villainIdCmd = new SqlCommand(villainIdQuery, sqlConnection, sqlTransaction);
            villainIdCmd.Parameters.AddWithValue("@VillainName", villainName);

            object villainIdObj = villainIdCmd.ExecuteScalar();
            if (villainIdObj == null)
            {
                string evilnessFactorQuery = @"SELECT [Id]
                                                     FROM [EvilnessFactors]
                                                    WHERE [Name] = 'Evil'";
                SqlCommand evilnessFactorCmd =
                    new SqlCommand(evilnessFactorQuery, sqlConnection, sqlTransaction);
                int evilnessFactorId = (int)evilnessFactorCmd.ExecuteScalar();

                string insertVillainQuery = @"INSERT INTO [Villains]([Name], [EvilnessFactorId])
                                                     VALUES
                                                (@VillainName, @EvilnessFactorId)";
                SqlCommand insertVillainCmd =
                    new SqlCommand(insertVillainQuery, sqlConnection, sqlTransaction);
                insertVillainCmd.Parameters.AddWithValue("@VillainName", villainName);
                insertVillainCmd.Parameters.AddWithValue("@EvilnessFactorId", evilnessFactorId);

                insertVillainCmd.ExecuteNonQuery();
                output.AppendLine($"Villain {villainName} was added to the database.");

                villainIdObj = villainIdCmd.ExecuteScalar();
            }

            return (int)villainIdObj;
        }

        private static int AddMinionAndGetId(SqlConnection sqlConnection, SqlTransaction sqlTransaction,
            string minionName, int minionAge, int townId)
        {
            string addMinionQuery = @"INSERT INTO [Minions]([Name], [Age], [TownId])
                                               VALUES 
                                            (@MinionName, @MinionAge, @TownId)";
            SqlCommand addMinionCmd = new SqlCommand(addMinionQuery, sqlConnection, sqlTransaction);
            addMinionCmd.Parameters.AddWithValue("@MinionName", minionName);
            addMinionCmd.Parameters.AddWithValue("@MinionAge", minionAge);
            addMinionCmd.Parameters.AddWithValue("@TownId", townId);

            addMinionCmd.ExecuteNonQuery();

            string addedMinionIdQuery = @"SELECT [Id]
                                       FROM [Minions]
                                      WHERE [Name] = @MinionName AND [Age] = @MinionAge AND [TownId] = @TownId";
            SqlCommand getMinionIdCmd = new SqlCommand(addedMinionIdQuery, sqlConnection, sqlTransaction);
            getMinionIdCmd.Parameters.AddWithValue("@MinionName", minionName);
            getMinionIdCmd.Parameters.AddWithValue("@MinionAge", minionAge);
            getMinionIdCmd.Parameters.AddWithValue("@TownId", townId);

            int minionId = (int)getMinionIdCmd.ExecuteScalar();

            return minionId;
        }

        // Problem 05
        private static string UpdateTownNameWithCapitalLetters(
            SqlConnection sqlConnection, string countryName)
        {
            StringBuilder sb = new StringBuilder();

            SqlCommand selectUpdatedTownNames = new SqlCommand(SqlQueries.GetAllTownsInCountry, sqlConnection);
            selectUpdatedTownNames.Parameters.AddWithValue("@countryName", countryName);

            SqlCommand commandToUpdatedTowns = new SqlCommand(SqlQueries.UpdateTownNames, sqlConnection);
            commandToUpdatedTowns.Parameters.AddWithValue("@countryName", countryName);

            int countOfUpdatedTowns = commandToUpdatedTowns.ExecuteNonQuery();

            if (countOfUpdatedTowns > 0)
            {
                SqlDataReader reader = selectUpdatedTownNames.ExecuteReader();

                List<string> towns = new List<string>();

                sb.AppendLine($"{countOfUpdatedTowns} town names were affected.");
                while (reader.Read())
                {
                    string updatedTowns = (string)reader["Name"];

                    towns.Add(updatedTowns);
                }

                sb.AppendLine($"[{string.Join(", ", towns)}]");
            }
            else
            {
                sb.AppendLine("No town names were affected.");
            }

            return sb.ToString().Trim();
        }

        // Problem 07
        private static string PrintAllNamesFromMinions(SqlConnection sqlConnection)
        {
            StringBuilder sb = new StringBuilder();

            SqlCommand sqlCommand = new SqlCommand(
                SqlQueries.SelectAllNamesFromMinions, sqlConnection);

            SqlDataReader reader = sqlCommand.ExecuteReader();

            List<string> allNames = new List<string>();

            while (reader.Read())
            {
                string name = (string)reader["Name"];

                allNames.Add(name);
            }

            allNames.Add("Bob");
            allNames.Add("Cathleen");
            allNames.Add("Mars");
            allNames.Add("Carry");

            int counter = 0;
            int endIndex = allNames.Count - 1;
            int averageIndex = allNames.Count / 2;

            for (int i = 0; i < allNames.Count - 1; i++)
            {
                if (i == averageIndex)
                {
                    sb.AppendLine($"{allNames[i]}");
                    break;
                }

                sb.AppendLine($"{allNames[i]}");
                sb.AppendLine($"{allNames[endIndex - counter]}");

                counter++;
            }

            return sb.ToString().Trim();
        }

        // Problem 08
        //private static string IncreaseMinionsAgeById(SqlConnection sqlConnection, int[] minionId)
        //{
        //    StringBuilder sb = new StringBuilder();

        //    SqlCommand updateAge = new SqlCommand(SqlQueries.UpdateMinionsAge, sqlConnection);
        //    updateAge.Parameters.AddWithValue("@Id", minionId);

        //    SqlCommand selectMinions = new SqlCommand(SqlQueries.selectMinions, sqlConnection);
        //    SqlDataReader reader = selectMinions.ExecuteReader();

        //    while (reader.Read())
        //    {
        //        string name = (string)reader["Name"];
        //        int age = (int)reader["Age"];

        //        sb.AppendLine($"{name} {age}");

        //    }

        //    return sb.ToString().Trim();
        //}
    }
}