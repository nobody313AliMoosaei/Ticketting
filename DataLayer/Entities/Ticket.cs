using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
   public  class Ticket
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
        
        public string DataFile { get; set; }
        public int CounterReport { set; get; } = 0;
        
        public byte[] ByteFile { get; set; }
        [Required]
        public DateTime InsertTime { get; set; } = DateTime.Now;

        // Navigation Properties
        public ICollection<Ticket> Responses { get; set; }
        public User User { get; set; }
    }
}
