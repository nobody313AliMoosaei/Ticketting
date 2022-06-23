using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        public string HashPassword { get; set; }
        
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public bool IsConfirmEmail { get; set; }
        public bool IsRemove { get; set; }
        public int CounterRequest { get; set; } = 0;

        // navigation
        public ICollection<Role> Roles { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<Token> Tokens { get; set; }


    }
}
