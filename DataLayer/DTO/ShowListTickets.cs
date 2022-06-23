using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.DTO
{
    public class ShowListTickets
    {
        public int TicketId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DataFile { get; set; }
        public DateTime TimeInsert { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public List<ResponsTicketForShow> Responses { get; set; } = new List<ResponsTicketForShow>();
    }
    public class ResponsTicketForShow
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime InsertTime { get; set; }
    }
}
