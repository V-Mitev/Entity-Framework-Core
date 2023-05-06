namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System.Globalization;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            string input = Console.ReadLine();

            //int input = int.Parse(Console.ReadLine());

            string result = GetBooksReleasedBefore(db, input);

            Console.WriteLine(result);
        }

        // Problem 02

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            string[] bookTitles = context.Books
                .ToArray()
                .OrderBy(b => b.Title)
                .Where(b => b.AgeRestriction.ToString().ToLower() ==  command.ToLower())
                .Select(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitles);
        }

        // Problem 03

        public static string GetGoldenBooks(BookShopContext context)
        {
            var goldenBooks = context.Books
                .Where(b => b.EditionType == EditionType.Gold &&
                            b.Copies < 5000)
                .OrderBy(b=> b.BookId)
                .Select(b=>b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, goldenBooks);
        }

        // Problem 04

        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .OrderByDescending(b => b.Price)
                .ToArray();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title} - ${b.Price:f2}");
            }

            return sb.ToString().Trim();
        }

        // Problem 05

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(b => b.ReleaseDate.HasValue &&
                            b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, books);
        }

        // Problem 06

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            string[] categories = input.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(c => c.ToLower()).ToArray();

            var books = context.Books
                .Where(b => b.BookCategories.Any(bc => categories.Contains(bc.Category.Name.ToLower())))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, books);
        }

        // Problem 07

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder sb = new StringBuilder();

            var parsedDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .Where(b => b.ReleaseDate < parsedDate)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    b.Title,
                    EditionType = b.EditionType.ToString(),
                    b.Price
                })
                .ToArray();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title} - {b.EditionType} - ${b.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 08

        //public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        //{

        //}
    }
}