using SqlFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Student> str = new List<Student>();
            str.Add(new Student() { StudentId = 3 });
            Student st = new Student();
            //st.StudentName = "Serdcamsad";
            st.StudentName = "Sercan";
            st.StudentId = 4;
            str.Add(new Student { StudentId = 1, StudentName = "Sercan" });
            SqlRepository<Student> sqlRepository = new SqlRepository<Student>       
               (@"Server=DESKTOP-H4NELET\SQLEXPRESS;Database=Deneme;Trusted_Connection=True;");
            Student st1 = new Student();
            st1.StudentId = 12;
            st1.StudentName = "Mehmet";
            Student st2 = new Student();
            st2.StudentId = 18;
            st2.StudentName = "Halit";
            Student st3 = new Student();
            st3.StudentId = 15;
            st3.StudentName = "İbrahim";
            SqlRepository<Person> sqlRepository1 = new SqlRepository<Person>
               (@"Server=DESKTOP-H4NELET\SQLEXPRESS;Database=Deneme;Trusted_Connection=True;");
            //Person p1 = new Person();
            //p1.PersonName = "Sercan";
            //p1.PersonSurname = "Uludag";
            //sqlRepository1.Insert(p1);
            Person p2 = new Person();
            p2.PersonId = 1;
            p2.PersonName = "Alper";
            p2.PersonSurname = "Aktaş";
            sqlRepository1.Update(p2);
            Student st4 = new Student();
            st4.StudentName = "muhammet";
            sqlRepository.Insert(st4);
            // sqlRepository.Insert(st);
           
            var results =sqlRepository.GetAll();
            var result2 = results.FirstOrDefault(x => x.StudentId == 15);
            Console.WriteLine(result2.StudentName);
            sqlRepository.Update(st);
            sqlRepository.Update(st1);
            sqlRepository.Update(st2);
            sqlRepository.Update(st3);

            sqlRepository.Delete(st1);
        }
      
    }
}
