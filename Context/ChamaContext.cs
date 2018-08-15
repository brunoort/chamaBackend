using Chama.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Chama.WebApi.ContextDatabase
{
    public class ChamaContext : DbContext
    {
        #region Constructors
        public ChamaContext() : base() { }
        public ChamaContext(DbContextOptions<ChamaContext> options) : base(options) { } 
        #endregion

        #region Overrides
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<StudentByCourseModel>()
                .HasOne(u => u.Course)
                .WithMany(u => u.StudentsByCourse)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);            

            base.OnModelCreating(builder);

        }

        #endregion

        #region Entities
        public DbSet<CourseModel> Courses { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<StudentByCourseModel> StudentByCourse { get; set; }
        public DbSet<ProcessedCourseModel> ProcessedCourses { get; set; }
        #endregion
    }
}