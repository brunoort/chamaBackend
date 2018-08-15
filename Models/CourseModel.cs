using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Chama.WebApi.Models
{
    [Table("Courses")]
    public class CourseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int MaxStudents { get; set; }

        [NotMapped]
        public virtual ICollection<UserModel> Students { get; set; }
        [NotMapped]
        public virtual ICollection<StudentByCourseModel> StudentsByCourse { get; set; }

        public int TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public UserModel Teacher { get; set; }
    }
}
