using System.Xml.Serialization;

namespace ProductShop.DTOs.Export
{
    [XmlType("User")]
    public class ExportUserSoldProducts
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlArray("soldProducts")]
        public ExportSoldProducts[] SoldProducts { get; set; }
    }
}