namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore.ValueGeneration;
    using System.Globalization;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            //string input = Console.ReadLine();

            //int input = int.Parse(Console.ReadLine());

            string result = GetMostRecentBooks(db);

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

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Authors
                .Where(b => b.FirstName.EndsWith(input))
                .Select(a => new
                {
                    FullName = a.FirstName + " " + a.LastName
                })
                .OrderBy(b => b.FullName)
                .ToArray();

            foreach (var b in books)
            {
                sb.AppendLine(b.FullName);
            }

            return sb.ToString().Trim();
        }

        // Problem 09

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToArray();

            return string.Join(Environment.NewLine, books);
        }

        // Problem 10

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    AuthorFullName = b.Author.FirstName + " " + b.Author.LastName,
                    BookTitle = b.Title
                })
                .ToArray();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.BookTitle} ({b.AuthorFullName})");
            }

            return sb.ToString().Trim();
        }

        // Problem 11

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books = context.Books
                .Where(b => b.Title.Length > lengthCheck)
                .Count();

            return books;
        }

        // Problem 12

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Authors
                .Select(a => new
                {
                    FullName = a.FirstName + ' ' + a.LastName,
                    Copies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(b => b.Copies)
                .ToArray();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.FullName} - {b.Copies}");
            }

            return sb.ToString().Trim();
        }

        // Problem 13

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var bookCategories = context.Categories
                .Select(bc => new
                {
                    bc.Name,
                    TotalProfit = bc.CategoryBooks.Sum(cb => cb.Book.Copies * cb.Book.Price)
                })
                .OrderByDescending(bc => bc.TotalProfit)
                .ThenBy(bc => bc.Name)
                .ToArray();

            foreach (var b in bookCategories)
            {
                sb.AppendLine($"{b.Name} ${b.TotalProfit:f2}");
            }

            return sb.ToString().Trim();
        }

        // Problem 14

        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categories = context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    CategoryName = c.Name,
                    MostRecentBooks = c.CategoryBooks
                        .OrderByDescending(cb => cb.Book.ReleaseDate)
                        .Take(3)
                        .Select(cb => new
                        {
                            BookTitle = cb.Book.Title,
                            ReleaseYear = cb.Book.ReleaseDate!.Value.Year
                        })
                        .ToArray()
                })
                .ToArray();

            foreach (var c in categories)
            {
                sb.AppendLine($"--{c.CategoryName}");

                foreach (var b in c.MostRecentBooks)
                {
                    sb.AppendLine($"{b.BookTitle} ({b.ReleaseYear})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 15

        public static void IncreasePrices(BookShopContext context)
        {
            var increasedBooksPrice = context.Books
                .Where(b => b.ReleaseDate.HasValue && 
                            b.ReleaseDate.Value.Year < 2010)
                .Select(b => b)
                .ToArray();

            foreach (var b in increasedBooksPrice)
            {
                b.Price += 5;
            }
        }

        //Problem 16

        public static int RemoveBooks(BookShopContext context)
        {
            var deletedBooks = context.Books
                .Where(b => b.Copies < 4200);

            context.Books.RemoveRange(deletedBooks);

            int countOfRemovedBooks = deletedBooks.Count();

            context.SaveChanges();

            return countOfRemovedBooks;
        }
    }
}