using Invoices.Common;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Invoices.DataProcessor.ImportDto
{
    public class ImportInvoiceDto
    {
        [JsonProperty("Number")]
        [Range(ValidationConstants.InvoiceNumberMinValue, 
               ValidationConstants.InvoiceNumberMaxValue)]
        public int Number { get; set; }

        [Required]
        [JsonProperty("IssueDate")]
        public string IssueDate { get; set; } = null!;

        [Required]
        [JsonProperty("DueDate")]
        public string DueDate { get; set; } = null!;

        [JsonProperty("Amount")]
        public decimal Amount { get; set; }

        [JsonProperty("CurrencyType")]
        [Range(ValidationConstants.InvoiceCurrencyTypeMinValue, ValidationConstants.InvoiceCurrencyTypeMaxValue)]
        public int CurrencyType { get; set; }

        [JsonProperty("ClientId")]
        public int ClientId { get; set; }
    }
}