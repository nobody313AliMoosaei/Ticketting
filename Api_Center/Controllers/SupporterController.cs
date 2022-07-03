using Api_Center.Models.DTO;
using CoreLayer.Services;
using CoreLayer.Utlities;
using DataLayer.DataBase;
using DataLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;

namespace Api_Center.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class SupporterController : ControllerBase
    {
        #region Dependency Injection

        private readonly Context_DB Context;
        private readonly IUserService UserManager;
        private readonly IEmailService Emailmanager;

        public SupporterController(Context_DB con, IUserService user, IEmailService email)
        {
            Context = con;
            UserManager = user;
            Emailmanager = email;
        }

        #endregion

        [HttpGet("GetAllTickets")]
        [Authorize]
        [AllowAnonymous]
        public IActionResult GetAllTicket()
        {
            try
            {
                var AllTickets = Context.Tickets.Include(t => t.User)
                    .Where(t => t.User.IsActive == true && t.User.IsRemove == false).Include(t => t.Responses).ToList();
                var Tickets = Mapper.GetListTicketForShow(AllTickets);
                return Ok(Tickets);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [Authorize]
        [HttpPost("AnswerToTicket")]
        public IActionResult AnswerToTicket(AnswerToTicketDTO Respons)
        {
            try
            {
                if (Respons == null)
                    return BadRequest("Blank");

                var user = UserManager.Get(Respons.UserId);
                if (user == null)
                    return BadRequest("User Is Not Found");

                var TicketUser = Context.Tickets.Include(t => t.Responses)
                    .Include(t => t.User).Where(t => t.User.Id == Respons.UserId)
                    .SingleOrDefault(t => t.Id == Respons.TicketId);
                if (TicketUser == null)
                    return BadRequest("Ticket User Is Not Found");

                Ticket ResponsTicket = new Ticket()
                {
                    Title = Respons.Title,
                    Body = Respons.Description,
                    InsertTime = DateTime.Now
                };
                if (Respons.File != null && Respons.File.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        Respons.File.CopyTo(ms);
                        var FileByte = ms.ToArray();
                        string DataFile = Convert.ToBase64String(FileByte);

                        ResponsTicket.ByteFile = FileByte;
                        ResponsTicket.DataFile = DataFile;
                    }
                }

                TicketUser.Responses.Add(ResponsTicket);
                Context.SaveChanges();
                string urlString = Url.Action(nameof(GetAllTicket), "Supporter", new { }, Request.Scheme);
                return Created(urlString, true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ReportTicket")]
        [AllowAnonymous]
        [Authorize]
        public IActionResult ReportTicket(int UserId, int TicketId)
        {
            try
            {
                var Admin = Context.Users.Where(t => t.IsAdmin == true).SingleOrDefault();
                var user = Context.Users.Include(t=>t.Tickets).SingleOrDefault(t=>t.Id==UserId);
                if (user == null)
                    return BadRequest("User Is Null");
                if (Admin == null)
                    return BadRequest("Admin Not Exist");
                var ticket = user.Tickets.SingleOrDefault(t => t.Id == TicketId);
                if (ticket == null)
                    return NotFound("NotFoound Ticket");
                ticket.CounterReport++;

                var resualtSendEmail = Emailmanager.SendEmail(Admin.Email, $"Report Ticket{ticket.Id}", $"please follow up <br></br>UserId={ticket.User.Id} <br></br>TicketId = {ticket.Id}");
                if (resualtSendEmail != null && resualtSendEmail.IsCompleted)
                    return Ok("Email sent");
                else
                    return BadRequest("Email could not be sent");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
