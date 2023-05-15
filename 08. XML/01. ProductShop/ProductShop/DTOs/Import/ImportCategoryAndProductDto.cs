using System.Xml.Serialization;

namespace ProductShop.DTOs.Import
{
    [XmlType("CategoryProduct")]
    public class ImportCategoryAndProductDto
    {
        public int CategoryId { get; set; } 

        public int ProductId { get; set; }
    }
}