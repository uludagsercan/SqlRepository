using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleUI
{
    [Table("Customers",Schema ="dbo")]
    
    public class Deneme
    {
       [ForeignKey("deneme")]
        public int MyProperty { get; set; }
        public int deneme2 { get; set; }
        public int deneme3 { get; set; }
    }
   
}
