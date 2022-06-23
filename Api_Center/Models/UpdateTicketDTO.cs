using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Center.Models
{
    public class UpdateTicketDTO
    {
        public int TicketId { get; set; }
        public string Title { get; set; }
        public string description { get; set; }
        public IFormFile File { get; set; }
    }
}
