using Lab07.Models;

namespace Lab07.Data
{
    public class DbInitializer
    {
        public static void Initialize(SchoolContext context)
        {
            context.Database.EnsureCreated();
            // Look for any students.
            if (context.Students.Any())
            {
                return; // DB has been seeded
            }
            var students = new Student[]
            {
 new
Student{firstMidName="Carson",lastName="Alexander",enrollmentDate=DateTime.Parse("2022-09-01")},
 new
Student{firstMidName="Meredith",lastName="Alonso",enrollmentDate=DateTime.Parse("2022-09-01")},
 new
Student{firstMidName="Arturo",lastName="Anand",enrollmentDate=DateTime.Parse("2023-09-01")},
 new
Student{firstMidName="Gytis",lastName="Barzdukas",enrollmentDate=DateTime.Parse("2022-09-01")},
 new Student{firstMidName="Yan",lastName="Li",enrollmentDate=DateTime.Parse("2022-09-01")}, new
Student{firstMidName="Peggy",lastName="Justice",enrollmentDate=DateTime.Parse("2021-09-01")},
 new
Student{firstMidName="Laura",lastName="Norman",enrollmentDate=DateTime.Parse("2023-09-01")},
 new
Student{firstMidName="Nino",lastName="Olivetto",enrollmentDate=DateTime.Parse("2022-09-01")}

            };
            foreach (Student s in students)
            {
                context.Students.Add(s);
            }
            context.SaveChanges();
            var courses = new Course[]
            {
 new Course{courseID=1050,title="Chemistry",credits=3},
 new Course{courseID=4022,title="Microeconomics",credits=3},
 new Course{courseID=4041,title="Macroeconomics",credits=3},
 new Course{courseID=1045,title="Calculus",credits=4},
 new Course{courseID=3141,title="Trigonometry",credits=4},
 new Course{courseID=2021,title="Composition",credits=3},
 new Course{courseID=2042,title="Literature",credits=4}
            }; foreach (Course c in courses)
            {
                context.Courses.Add(c);
            }
            context.SaveChanges();
            var enrollments = new Enrollment[]
            {
 new Enrollment{studentId=1,courseId=1050,grade=Grade.A},
 new Enrollment{studentId=1,courseId=4022,grade=Grade.C},
 new Enrollment{studentId=1,courseId=4041,grade=Grade.B},
 new Enrollment{studentId=2,courseId=1045,grade=Grade.B},
 new Enrollment{studentId=2,courseId=3141,grade=Grade.F},
 new Enrollment{studentId=2,courseId=2021,grade=Grade.F},
 new Enrollment{studentId=3,courseId=1050},
 new Enrollment{studentId=4,courseId=1050},
 new Enrollment{studentId=4,courseId=4022,grade=Grade.F},
 new Enrollment{studentId=5,courseId=4041,grade=Grade.C},
 new Enrollment{studentId=6,courseId=1045},
 new Enrollment{studentId=7,courseId=3141,grade=Grade.A},
            };
            foreach (Enrollment e in enrollments)
            {
                context.Enrollments.Add(e);
            }
            context.SaveChanges();
        }
    }
}
        }
}
