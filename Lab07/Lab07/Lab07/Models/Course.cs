using System.ComponentModel.DataAnnotations.Schema;
namespace Lab07.Models
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int courseID { get; set; }
        public string title { get; set; }
        public int credits { get; set; }
        public ICollection<Enrollment>? Enrollments { get; set; }
    }
}
