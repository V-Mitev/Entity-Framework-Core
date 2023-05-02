using P01_StudentSystem.Data.Models.Enums;
using P01_StudentSystemCommon;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Data.Models
{
    public class Resource
    {
        [Key]
        public int ResourceId { get; set; }

        [Required]
        [MaxLength(ValidationsConstants.ResourceNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(ValidationsConstants.UrlMaxLength)]
        public string? Url { get; set; }

        public ResourceType ResourceType { get; set; }

        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }

        public virtual Course Course { get; set; } = null!;
    }
}