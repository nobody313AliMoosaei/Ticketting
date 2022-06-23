using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.DTO
{
    public class UpdateTicketDTO:AddTicketDTO
    {
        public int UserId { get; set; }
        public int TicketId { get; set; }
    }
}
