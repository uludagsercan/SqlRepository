using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleUI2
{
    [Table("Students",Schema ="dbo")]
    public class Student
    {
        [Key]
        public int StudentId { get; set; }
        public string StudentName { get; set; }
    }
}
