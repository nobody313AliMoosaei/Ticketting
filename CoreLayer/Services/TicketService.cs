using DataLayer.DTO;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Services
{
    public class TicketService : ITicketService
    {
        private readonly DataLayer.DataBase.Context_DB Context;
        private readonly IUserService Usermanager;
        public TicketService(DataLayer.DataBase.Context_DB con
            , IUserService userservice)
        {
            Context = con;
            Usermanager = userservice;
        }

        public bool Add(int UserId, AddTicketDTO AddTicket)
        {
            if (UserId == 0 || AddTicket == null
                || string.IsNullOrEmpty(AddTicket.Title) || string.IsNullOrEmpty(AddTicket.Description))
                return false;

            var user = Usermanager.Get(UserId);
            Ticket Ticket = new Ticket();
            if (AddTicket.FileByte != null && !string.IsNullOrEmpty(AddTicket.DataFile))
            {
                Ticket NewTicket = new Ticket()
                {
                    Title = AddTicket.Title,
                    Body = AddTicket.Description,
                    ByteFile = AddTicket.FileByte,
                    DataFile = AddTicket.DataFile,
                    InsertTime = DateTime.Now
                };
                user.Tickets.Add(NewTicket);
                Context.SaveChanges();
                return true;
            }
            else if (AddTicket.FileByte != null && string.IsNullOrEmpty(AddTicket.DataFile))
            {
                Ticket NewTicket = new Ticket()
                {
                    Title = AddTicket.Title,
                    Body = AddTicket.Description,
                    ByteFile = AddTicket.FileByte,
                    InsertTime = DateTime.Now
                };
                user.Tickets.Add(NewTicket);
                Context.SaveChanges();
                return true;
            }
            else if (AddTicket.FileByte == null && !string.IsNullOrEmpty(AddTicket.DataFile))
            {
                Ticket NewTicket = new Ticket()
                {
                    Title = AddTicket.Title,
                    Body = AddTicket.Description,
                    DataFile = AddTicket.DataFile,
                    InsertTime = DateTime.Now
                };
                user.Tickets.Add(NewTicket);
                Context.SaveChanges();
                return true;
            }
            else
            {
                Ticket NewTicket = new Ticket()
                {
                    Title = AddTicket.Title,
                    Body = AddTicket.Description,
                    InsertTime = DateTime.Now
                };
                user.Tickets.Add(NewTicket);
                Context.SaveChanges();
                return true;
            }
        }



        public bool Delete(Ticket Ticket)
        {
            try
            {
                var user = Usermanager.Get(Ticket.User.Id);
                user.Tickets.Remove(Ticket);
                Context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(int UserId, int TicketId)
        {
            try
            {
                var user = Usermanager.Get(UserId);
                if (user == null)
                    return false;

                var Ticket = user.Tickets.SingleOrDefault(t => t.Id == TicketId);
                if (Ticket == null)
                    return false;

                user.Tickets.Remove(Ticket);
                Context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(int UserId, string TicketTitle)
        {
            try
            {
                var user = Usermanager.Get(UserId);
                if (user == null)
                    return false;

                var Ticket = user.Tickets.SingleOrDefault(t => t.Title == TicketTitle);
                if (Ticket == null)
                    return false;

                user.Tickets.Remove(Ticket);
                Context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(User user, int TicketId)
        {
            try
            {
                if (user == null)
                    return false;
                user = Usermanager.Get(user.Id);
                if (user == null)
                    return false;

                var Ticket = user.Tickets.SingleOrDefault(t => t.Id == TicketId);
                if (Ticket == null)
                    return false;

                user.Tickets.Remove(Ticket);
                Context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteAllTicketUser(int UserId)
        {
            if (UserId == 0)
                return false;

            var user = Usermanager.Get(UserId);
            if (user == null)
                return false;

            foreach (var item in user.Tickets.ToList())
            {
                if (!Delete(UserId, item.Id))
                    return false;
            }
            return true;
        }

        public List<Ticket> Get()
        {
            try
            {
                return Context.Tickets.Include(t => t.User).Include(t => t.Responses).ToList();
            }
            catch
            {
                return null;
            }
        }

        public List<Ticket> Get(string Title)
        {
            var AllTickets = Get();
            List<Ticket> Tickets = new List<Ticket>();
            foreach (var item in AllTickets)
            {
                if (item.Title.Contains(Title))
                {
                    Tickets.Add(item);
                }
            }
            return Tickets;
        }

        public List<Ticket> Get(int UserId)
        {
            try
            {
                var t = Context.Tickets.Include(t => t.Responses).Include(t => t.User).ToList();
                List<Ticket> TicketsUser = new List<Ticket>();
                foreach (var item in t)
                {
                    if (item.User.Id == UserId)
                        TicketsUser.Add(item);
                }
                return TicketsUser;
            }
            catch
            {
                return null;
            }
        }

        public Ticket Get(int UserId, int TicketId)
        {
            try
            {
                return Usermanager.Get(UserId).Tickets.SingleOrDefault(t => t.Id == TicketId);
            }
            catch
            {
                return null;
            }
        }

        public bool Update(int UserId, int TicketId, AddTicketDTO UpdateTicket)
        {
            try
            {
                var user = Usermanager.Get(UserId);
                if (user == null)
                    return false;

                var Ticket = user.Tickets.SingleOrDefault(t => t.Id == TicketId);
                if (Ticket == null)
                    return false;
                if (string.IsNullOrEmpty(UpdateTicket.Title) && string.IsNullOrEmpty(UpdateTicket.Description))
                    return false;

                Ticket.Body = UpdateTicket.Description;
                Ticket.Title = UpdateTicket.Title;

                if (UpdateTicket.FileByte != null)
                    Ticket.ByteFile = UpdateTicket.FileByte;

                if (!string.IsNullOrEmpty(UpdateTicket.DataFile))
                    Ticket.DataFile = UpdateTicket.DataFile;
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
