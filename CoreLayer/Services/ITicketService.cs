using DataLayer.DTO;
using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Services
{
    public interface ITicketService
    {
        // Read
        List<Ticket> Get();
        List<Ticket> Get(string Title);
        List<Ticket> Get(int UserId);
        Ticket Get(int UserId,int TicketId);


        // Creat
        bool Add(int UserId, AddTicketDTO AddTicket);

        //Update
        bool Update(int UserId,int TicketId, AddTicketDTO UpdateTicket);
        
        // Delete
        bool Delete(Ticket Ticket);
        bool DeleteAllTicketUser(int UserId);
        bool Delete(int UserId, int TicketId);
        bool Delete(int UserId, string TicketTitle);
        bool Delete(User user, int TicketId);
    }
}
