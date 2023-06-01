namespace Invoices.DataProcessor
{
    using Invoices.Data;
    using Invoices.Data.Models.Enums;
    using Invoices.DataProcessor.ExportDto;
    using Newtonsoft.Json;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportClientsWithTheirInvoices(InvoicesContext context, DateTime date)
        {
            StringBuilder sb = new StringBuilder();

            StringWriter writer = new StringWriter(sb);

            XmlRootAttribute root = new XmlRootAttribute("Clients");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportClientDto[]), root);

            XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
            xmlns.Add(string.Empty, string.Empty);

            ExportClientDto[] exportClientDtos = context.Clients
                .Where(c => c.Invoices.Any(i => i.IssueDate.Date > date))
                .ToArray()
                .Select(c => new ExportClientDto()
                {
                    InvoicesCount = c.Invoices.Count,
                    ClientName = c.Name,
                    VatNumber = c.NumberVat,
                    Invoices = c.Invoices
                    .OrderBy(i => i.IssueDate)
                    .ThenByDescending(i => i.DueDate)
                    .Select(i => new ExportInvoiceDto()
                    {
                        InvoiceNumber = i.Number,
                        InvoiceAmount = i.Amount,
                        DueDate = i.DueDate.ToString("d", CultureInfo.InvariantCulture),
                        Currency = i.CurrencyType.ToString()
                    })
                    .ToArray()
                })
                .OrderByDescending(c => c.InvoicesCount)
                .ThenBy(c => c.ClientName)
                .ToArray();

            xmlSerializer.Serialize(writer, exportClientDtos, xmlns);

            return sb.ToString().Trim();
        }

        public static string ExportProductsWithMostClients(InvoicesContext context, int nameLength)
        {
            var products = context.Products
                .Where(p => p.ProductsClients.Any(c => c.Client.Name.Length >= nameLength))
                .ToArray()
                .Select(p => new
                {
                    Name = p.Name,
                    Price = p.Price,
                    Category = p.CategoryType.ToString(),
                    Clients = p.ProductsClients
                    .Where(pc => pc.Client.Name.Length >= nameLength)
                    .Select(c => new
                    {
                        Name = c.Client.Name,
                        NumberVat = c.Client.NumberVat
                    })
                    .OrderBy(c => c.Name)
                    .ToArray()
                })
                .OrderByDescending(p => p.Clients.Count())
                .ThenBy(p => p.Name)
                .Take(5)
                .ToArray();

            string result = JsonConvert.SerializeObject(products, Formatting.Indented);

            return result;
        }
    }
}