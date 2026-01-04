namespace Lab07.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string lastName { get; set; }
        public string firstMidName { get; set; }
        public DateTime enrollmentDate { get; set; }

        public ICollection<Enrollment>? Enrollments { get; set; }
    }
}
