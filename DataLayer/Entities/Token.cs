using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
   public  class Token
    {
        [Key]
        public int Id { get; set; }
        public int TokenValue { get; set; }
        public DateTime TimeInsert { get; set; }
        public bool IsExpire { get; set; }


        // Navigation
        public User User { get; set; }
    }
}
