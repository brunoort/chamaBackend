using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Chama.WebApi.Models
{
    [Table("ProcessedCourses")]
    public class ProcessedCourseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int MaxStudents { get; set; }
        public int CurrentNumberStudents { get; set; }
        public double MaxAge { get; set; }
        public double MinAge { get; set; }
        public double AverageAge { get; set; }
        [NotMapped]
        public virtual ICollection<UserModel> Students { get; set; }
    }
}
