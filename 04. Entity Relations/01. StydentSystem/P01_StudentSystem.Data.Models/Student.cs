using P01_StudentSystemCommon;
using System.ComponentModel.DataAnnotations;

namespace P01_StudentSystem.Data.Models
{
    public class Student
    {
        public Student()
        {
            StudentsCourses = new HashSet<StudentCourse>();
            Homeworks = new HashSet<Homework>();
        }

        [Key]
        public int StudentId { get; set; }

        [Required]
        [MaxLength(ValidationsConstants.StudentNameMaxLength)]
        public string Name { get; set; } = null!;

        [MaxLength(ValidationsConstants.StudentMaxLengthPhoneNumber)]
        public string? PhoneNumber { get; set; }

        [Required]
        public DateTime RegisteredOn { get; set; } 

        public DateTime? Birthday { get; set; }

        public ICollection<StudentCourse> StudentsCourses { get; set; } = null!;

        public ICollection<Homework> Homeworks { get; set; } = null!;
    }
}