using Invoices.Common;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ImportDto
{
    [XmlType("Client")]
    public class ImportClientDto
    {
        [Required]
        [XmlElement("Name")]
        [MinLength(ValidationConstants.ClientNameMinLength)]
        [MaxLength(ValidationConstants.ClientNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [XmlElement("NumberVat")]
        [MinLength(ValidationConstants.ClientNumberVatMinValue)]
        [MaxLength(ValidationConstants.ClientNumberVatMaxValue)]
        public string NumberVat { get; set; } = null!;

        [XmlArray("Addresses")]
        public ImportAddressDto[] Addresses { get; set; } = null!;
    }
}
