using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.DTO
{
    public class ShowListUser
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { set; get; }

        // Tickets
        public List<ListRole> ListRolesUser { get; set; } = new List<ListRole>();
        public List<ListTicket> ListTicketUser { set; get; } = new List<ListTicket>();
    }
    public class ListRole
    {
        public int Id { get; set; }
        public string NameRoleEnglish { get; set; }
        public string NameRolePersian { get; set; }
    }
    public class ListTicket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
