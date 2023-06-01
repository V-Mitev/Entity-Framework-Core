namespace Invoices.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.Data.Models.Enums;
    using Invoices.DataProcessor.ImportDto;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedClients
            = "Successfully imported client {0}.";

        private const string SuccessfullyImportedInvoices
            = "Successfully imported invoice with number {0}.";

        private const string SuccessfullyImportedProducts
            = "Successfully imported product - {0} with {1} clients.";


        public static string ImportClients(InvoicesContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute root = new XmlRootAttribute("Clients");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportClientDto[]), root);

            StringReader xmlReader = new StringReader(xmlString);

            ImportClientDto[] importClientDtos = 
                (ImportClientDto[])xmlSerializer.Deserialize(xmlReader);

            ICollection<Client> validClients = new HashSet<Client>();

            foreach (ImportClientDto clientDto in importClientDtos)
            {
                if (!IsValid(clientDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                ICollection<Address> validAddress = new HashSet<Address>();

                foreach (ImportAddressDto addressDto in clientDto.Addresses)
                {
                    if (!IsValid(addressDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Address address = new Address()
                    {
                        StreetName = addressDto.StreetName,
                        StreetNumber = addressDto.StreetNumber,
                        PostCode = addressDto.PostCode,
                        City = addressDto.City,
                        Country = addressDto.Country,
                    };

                    validAddress.Add(address);
                }

                Client client = new Client()
                {
                    Name = clientDto.Name,
                    NumberVat = clientDto.NumberVat,
                    Addresses = validAddress
                };

                validClients.Add(client);

                sb.AppendLine(string.Format(SuccessfullyImportedClients, client.Name));
            }

            context.Clients.AddRange(validClients);
            context.SaveChanges();

            return sb.ToString().Trim();
        }


        public static string ImportInvoices(InvoicesContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportInvoiceDto[] importInvoiceDtos = 
                JsonConvert.DeserializeObject<ImportInvoiceDto[]>(jsonString);

            ICollection<Invoice> validInvoices = new HashSet<Invoice>();

            foreach (ImportInvoiceDto invoiceDto in importInvoiceDtos)
            {
                if (!IsValid(invoiceDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool isIssueDateValid = DateTime.TryParseExact(
                    invoiceDto.IssueDate, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime validIssueDate);

                if (!isIssueDateValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool isDueDateValid = DateTime.TryParseExact(
                    invoiceDto.DueDate, "yyyy-MM-ddTHH:mm:ss",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime validDueDate);

                if (!isDueDateValid)
                {
                    sb.AppendLine(ErrorMessage); 
                    continue;
                }

                if (validIssueDate > validDueDate)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Invoice invoice = new Invoice()
                {
                    Number = invoiceDto.Number,
                    IssueDate = validIssueDate,
                    DueDate = validDueDate,
                    Amount = invoiceDto.Amount,
                    CurrencyType = (CurrencyType)invoiceDto.CurrencyType,
                    ClientId = invoiceDto.ClientId
                };

                validInvoices.Add(invoice);

                sb.AppendLine(string.Format(SuccessfullyImportedInvoices, invoice.Number));
            }

            context.Invoices.AddRange(validInvoices);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportProducts(InvoicesContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportProductDto[] importProductDtos = 
                JsonConvert.DeserializeObject<ImportProductDto[]>(jsonString);

            ICollection<Product> validProducts = new HashSet<Product>();

            foreach (ImportProductDto productDto in importProductDtos)
            {
                if (!IsValid(productDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Product product = new Product()
                {
                    Name = productDto.Name,
                    Price = decimal.Parse(productDto.Price),
                    CategoryType = (CategoryType)productDto.CategoryType
                };

                foreach (int clientId in productDto.ClientsId.Distinct())
                {
                    Client? client = context.Clients.Find(clientId);

                    if (client == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    ProductClient productClient = new ProductClient()
                    {
                        Client = client
                    };

                    product.ProductsClients.Add(productClient);
                }

                validProducts.Add(product);

                sb.AppendLine(string.Format(SuccessfullyImportedProducts, product.Name, product.ProductsClients.Count));
            }

            context.Products.AddRange(validProducts);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    } 
}
