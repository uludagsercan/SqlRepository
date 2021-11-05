using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleUI2
{
    [Table("Lectures",Schema ="dbo")]
    public class Lecture
    {
        [Key]
        public int LectureId { get; set; }
        public string LectureName { get; set; }
        public int StudentId { get; set; }
    }
}
