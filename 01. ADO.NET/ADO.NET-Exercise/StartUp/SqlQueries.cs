namespace StartUp
{
    public static class SqlQueries
    {
        public const string GetAllVilliansAndCountOfTheirMinions =
                    @"SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
                        FROM Villains AS v 
                        JOIN MinionsVillains AS mv ON v.Id = mv.VillainId 
                    GROUP BY v.Id, v.Name 
                      HAVING COUNT(mv.VillainId) > 3 
                    ORDER BY COUNT(mv.VillainId)";

        public const string GetVilianNameById = @"SELECT [Name]
                                          FROM [Villains]
                                         WHERE [Id] = @Id";

        public const string GetMinionsInformation =
                  @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) AS RowNum,
                                         m.Name, 
                                         m.Age
                                    FROM MinionsVillains AS mv
                                    JOIN Minions As m ON mv.MinionId = m.Id
                                   WHERE mv.VillainId = @Id
                                ORDER BY m.Name";

        public const string GetAllTownsInCountry =
            @" SELECT t.Name 
                 FROM Towns as t
                 JOIN Countries AS c ON c.Id = t.CountryCode
                WHERE c.Name = @countryName";

        public const string UpdateTownNames =
            @"UPDATE Towns
                 SET Name = UPPER(Name)
               WHERE CountryCode = (
                                        SELECT c.Id 
                                          FROM Countries AS c 
                                         WHERE c.Name = @countryName
                                    )";

        public const string SelectAllNamesFromMinions = @"SELECT Name FROM Minions";

        public const string UpdateMinionsAge =
            @" UPDATE Minions
                  SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1
                WHERE Id = @Id";

        public const string selectMinions = @"SELECT Name, Age FROM Minions";
    }
}