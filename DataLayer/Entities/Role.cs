using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string NamePersian { get; set; }
        [MaxLength(100)]
        public string NameEnglish { get; set; }

        // Navigation
        public ICollection<User> Users { get; set; }
    }
}
