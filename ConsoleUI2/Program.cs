using SqlFramework;
using System;
using System.Linq.Expressions;

namespace ConsoleUI2
{
    class Program
    {
        static void Main(string[] args)
        {
           
            Student s1 = new Student();
            s1.StudentName = "Cihan";
            SqlRepository<Student> repo = new SqlRepository<Student>
                (@"Server=DESKTOP-H4NELET\SQLEXPRESS;Database=Deneme;Trusted_Connection=True;");
            // repo.Insert(s1);
            SqlRepository<Lecture> repository = new SqlRepository<Lecture>(@"Server=DESKTOP-H4NELET\SQLEXPRESS;Database=Deneme;Trusted_Connection=True;");
            //var listOfStudent=repo.GetAll();
            //foreach (var item in listOfStudent)
            //{
            //    Console.WriteLine(item.StudentName);
            //}
            //   Expression<Func<Student, int>> expression = f => f.StudentId;

            // repo.Get(x=> x.StudentId==3);
            //var listStudent = repo.GetAll(x => x.StudentId == 4);
            var listOfStudent = repository.GetAll(x => x.StudentId == 3);
            foreach (var item in listOfStudent)
            {
                Console.WriteLine(item.LectureName);
            }
        }
    }
}
