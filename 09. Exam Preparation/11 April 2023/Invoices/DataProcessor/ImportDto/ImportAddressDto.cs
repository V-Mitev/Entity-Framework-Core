using Invoices.Common;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ImportDto
{
    [XmlType("Address")]
    public class ImportAddressDto
    {
        [Required]
        [XmlElement("StreetName")]
        [MinLength(ValidationConstants.AddressStreetNameMinLength)]
        [MaxLength(ValidationConstants.AddressStreetNameMaxLength)]
        public string StreetName { get; set; } = null!;

        [XmlElement("StreetNumber")]
        public int StreetNumber { get; set; }

        [Required]
        [XmlElement("PostCode")]
        public string PostCode { get; set; } = null!;

        [Required]
        [XmlElement("City")]
        [MinLength(ValidationConstants.AddressCityMinLength)]
        [MaxLength(ValidationConstants.AddressCityMaxLength)]
        public string City { get; set; } = null!;

        [Required]
        [XmlElement("Country")]
        [MinLength(ValidationConstants.AddressCountryMinLength)]
        [MaxLength(ValidationConstants.AddressCountryMaxLength)]
        public string Country { get; set; } = null!;
    }
}