namespace Lab07.Models
{
    public enum Grade
    {
        A, B, C, D, F
    }
    public class Enrollment
    {
        public int enrollmentID { get; set; }
        public int studentId { get; set; }
        public int courseId { get; set; }
        public Grade? grade { get; set; }
        public Course Course { get; set; }
        public Student Student { get; set; }

    }
}
