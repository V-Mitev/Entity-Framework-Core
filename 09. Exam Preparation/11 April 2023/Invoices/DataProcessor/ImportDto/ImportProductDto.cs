using Invoices.Common;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Invoices.DataProcessor.ImportDto
{
    public class ImportProductDto
    {

        [JsonProperty("Name")]
        [MinLength(ValidationConstants.ProductNameMinLength)]
        [MaxLength(ValidationConstants.ProductNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [JsonProperty("Price")]
        [Range(typeof(decimal), ValidationConstants.ProductPriceMinValue, ValidationConstants.ProductPriceMaxValue)]
        public string Price { get; set; } = null!;

        [JsonProperty("CategoryType")]
        [Range(ValidationConstants.ProductCategoryTypeMinValue, ValidationConstants.ProductCategoryTypeMaxValue)]
        public int CategoryType { get; set; }

        [JsonProperty("Clients")]
        public int[] ClientsId { get; set; }
    }
}