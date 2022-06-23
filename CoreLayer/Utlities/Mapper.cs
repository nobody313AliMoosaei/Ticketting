using DataLayer.DTO;
using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Utlities
{
    public static class Mapper
    {

        // Base64String = "data:image/png;base64," + Convert.ToBase64String(image.Data, 0, image.Data.Length);
        public static List<ShowListTickets> GetListTicketForShow(List<Ticket> Tickets)
        {
            List<ShowListTickets> allTicket = new List<ShowListTickets>();
            foreach (var item in Tickets)
            {
                if (item.User != null)
                {
                    var ticket = new ShowListTickets()
                    {
                        UserId = item.User.Id,
                        UserName = item.User.UserName,
                        TicketId = item.Id,
                        Title = item.Title,
                        Description = item.Body,
                        TimeInsert = item.InsertTime,
                        DataFile = "data:image/png;base64," + Convert.ToBase64String(item.ByteFile, 0, item.ByteFile.Length)
                    };
                    if (item.Responses != null && item.Responses.Count > 0)
                    {
                        foreach (var i in item.Responses)
                        {
                            var respons = new ResponsTicketForShow()
                            {
                                Id = i.Id,
                                Description = i.Body,
                                Title = i.Title,
                                InsertTime = i.InsertTime
                            };
                            ticket.Responses.Add(respons);
                        }
                    }
                        allTicket.Add(ticket);
                }
                
            }
            return allTicket;
        }
        public static List<ShowListUser> GetUserForShow(List<User> users)
        {
            List<ShowListUser> listuser = new List<ShowListUser>();
            foreach (var item in users)
            {
                var u = new ShowListUser()
                {
                    Id = item.Id,
                    Email = item.Email,
                    FullName = item.FullName,
                    UserName = item.UserName,

                };
                u.ListRolesUser = new List<ListRole>();
                foreach (var i in item.Roles)
                {
                    u.ListRolesUser.Add(new ListRole() { Id = i.Id, NameRoleEnglish = i.NameEnglish, NameRolePersian = i.NamePersian });
                }
                u.ListTicketUser = new List<ListTicket>();
                foreach (var i in item.Tickets)
                {
                    u.ListTicketUser.Add(new ListTicket() { Id = i.Id, Description = i.Body, Title = i.Title });
                }
                listuser.Add(u);

            }
            return listuser;
        }
        public static List<ShowListSupporter> GetSupporterForShow(List<User> Supporters)
        {
            List<ShowListSupporter> Allsupporeter = new List<ShowListSupporter>();
            foreach (var item in Supporters)
            {
                Allsupporeter.Add(new()
                {
                    Id = item.Id,
                    Email = item.Email,
                    Fullname = item.FullName,
                    UserName = item.UserName
                });
            }
            return Allsupporeter;
        }
    }
    public class ShowListSupporter
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
    }
   
}
