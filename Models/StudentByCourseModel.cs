using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Chama.WebApi.Models
{
    [Table("StudentsByCourse")]
    public class StudentByCourseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public CourseModel Course { get; set; }
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public UserModel Student { get; set; }
    }
}
