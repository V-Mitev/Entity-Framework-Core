using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;
using P01_StudentSystemCommon;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        {
            
        }

        public StudentSystemContext(DbContextOptions options)
            :base(options)
        {
            
        }

        public DbSet<Student> Students { get; set; } = null!;

        public DbSet<Course> Courses { get; set; } = null!;

        public DbSet<Homework> Homeworks { get; set; } = null!;

        public DbSet<Resource> Resources { get; set; } = null!;

        public DbSet<StudentCourse> StudentsCourses { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DbConfig.ConnectionString);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .Property(s => s.PhoneNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Resource>()
                .Property(r => r.Url)
                .IsUnicode(false);

            modelBuilder.Entity<Homework>()
                .Property(hw => hw.Content)
                .IsUnicode(false);

            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentId, sc.CourseId });

            base.OnModelCreating(modelBuilder);
        }
    }
}