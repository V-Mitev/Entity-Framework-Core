using P01_StudentSystemCommon;
using System.ComponentModel.DataAnnotations;

namespace P01_StudentSystem.Data.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(ValidationsConstants.CourseNameMaxLength)]
        public string Name { get; set; } = null!;

        [MaxLength(ValidationsConstants.CourseDescriptionMaxLength)]
        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal Price { get; set; }

        public ICollection<StudentCourse> StudentsCourses { get; set; } = null!;

        public ICollection<Resource> Resources { get; set; } = null!;

        public ICollection<Homework> Homeworks { get; set; } = null!;
    }
}