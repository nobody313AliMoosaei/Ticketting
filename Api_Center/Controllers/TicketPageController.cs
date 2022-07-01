using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using CoreLayer.Services;
using DataLayer.DTO;
using Api_Center.Models.DTO;
using DataLayer.DataBase;
using System.IO;
using CoreLayer.Utlities;
using System.Security.Claims;

namespace Api_Center.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketPageController : ControllerBase
    {
        #region Dependency Injection
        private readonly ITicketService TicketManager;
        private readonly IUserService UserManager;
        private readonly Context_DB Context;
        private readonly IEmailService EmailManager;
        public TicketPageController(ITicketService ticket
            , IUserService user, Context_DB contex,
            IEmailService Email)
        {
            Context = contex;
            TicketManager = ticket;
            UserManager = user;
            EmailManager = Email;
        }
        #endregion

        [AllowAnonymous]
        [HttpGet("GetAllTickets")]
        [Authorize]
        public IActionResult GetAllTicket()
        {
            try
            {
                var Tickets = TicketManager.Get();
                if (Tickets == null)
                    return NotFound();

                var resualt = Mapper.GetListTicketForShow(Tickets);

                return Ok(resualt);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [Authorize]
        [HttpGet("GetOneTicketById")]
        public IActionResult GetOneTicket(int TicketId)
        {
            try
            {
                var Ticket = TicketManager.Get().SingleOrDefault(t => t.Id == TicketId);
                if (Ticket == null)
                    return NotFound();

                return Ok(new
                {
                    TicketId = Ticket.Id,
                    Title = Ticket.Title,
                    Description = Ticket.Body,
                    File = "data:image/png;base64," + Convert.ToBase64String(Ticket.ByteFile, 0, Ticket.ByteFile.Length),
                    UserId = Ticket.User.Id,
                    UserName = Ticket.User.UserName,
                    Email = Ticket.User.Email
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get the user's own tickets => Action
        [AllowAnonymous]
        [Authorize]
        [HttpGet("GetUserTickets")]
        public IActionResult GetUserTickets()
        {
            try
            {
                var Userid = User.Claims.SingleOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;
                int UserId = 0;
                int.TryParse(Userid, out UserId);
                var claims = HttpContext.User.Identity as ClaimsIdentity;
                var id = claims.Claims.FirstOrDefault(T => T.Type == ClaimTypes.NameIdentifier)?.Value;

                if (UserId == 0)
                {
                    string UserName = User.Identity.Name;
                    if (string.IsNullOrEmpty(UserName))
                        return NotFound();

                    var user = UserManager.Get(UserName);

                    if (user == null)
                        return NotFound();

                    var UserTicket = Mapper.GetListTicketForShow(user.Tickets.ToList());

                    return Ok(UserTicket);
                }
                else
                {
                    var user = UserManager.Get((int)UserId);
                    if (user == null)
                        return NotFound();
                    var UserTicket = Mapper.GetListTicketForShow(user.Tickets.ToList());
                    if (UserTicket == null)
                        return NotFound();

                    return Ok(UserTicket);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // CRUD For Over Ticket
        [AllowAnonymous]
        [Authorize]
        [HttpGet("GetOneTicketFromUser")]
        public IActionResult GetOneTicketFromUser(int TicketId)
        {
            try
            {
                var Userid = User.Claims.SingleOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;
                int UserId = 0;
                int.TryParse(Userid, out UserId);

                if (UserId != 0)
                {
                    var Ticket = TicketManager.Get((int)UserId, TicketId);
                    if (Ticket == null)
                        return NotFound();
                    return Ok(new
                    {
                        TicketId = Ticket.Id,
                        Title = Ticket.Title,
                        Description = Ticket.Body,
                        File = "data:image/png;base64," + Convert.ToBase64String(Ticket.ByteFile, 0, Ticket.ByteFile.Length),

                        UserId = Ticket.User.Id,
                        UserName = Ticket.User.UserName,
                        Email = Ticket.User.Email,
                    });
                }
                else
                {
                    string UserName = User.Identity.Name;
                    if (string.IsNullOrEmpty(UserName))
                        return NotFound();

                    var user = UserManager.Get(UserName);
                    if (user == null)
                        return NotFound();
                    var Ticket = user.Tickets.SingleOrDefault(t => t.Id == TicketId);
                    if (Ticket == null)
                        return NotFound();

                    return Ok(new
                    {
                        TicketId = Ticket.Id,
                        Title = Ticket.Title,
                        Description = Ticket.Body,
                        File = "data:image/png;base64," + Convert.ToBase64String(Ticket.ByteFile, 0, Ticket.ByteFile.Length),

                        UserId = Ticket.User.Id,
                        UserName = Ticket.User.UserName,
                        Email = Ticket.User.Email,
                    });

                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [AllowAnonymous]
        [Authorize]
        [HttpPost("AddTicket")]
        public IActionResult AddTicket(Models.AddTicketDTO TicketForAdd)
        {
            try
            {
                var Userid = User.Claims.SingleOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;
                int UserId = 0;
                int.TryParse(Userid, out UserId);
                if (UserId == 0)
                    return BadRequest("UserId Not Found");

                if (!ModelState.IsValid)
                    return BadRequest("ModelState is Not Valid");

                if (TicketForAdd == null)
                    return BadRequest("Data's Ticket is Null");

                if (UserId == 0)
                    return BadRequest("UserId == 0");

                var user = UserManager.Get(UserId);
                if (user == null)
                    return BadRequest("User Not Found");
                /*
                 if (user.IsConfirmEmail == false)
                 {

                     var resualtSendEmail = UserManager.SendEmailForConfirm(user.Id);
                     if (resualtSendEmail)
                     {
                         string url = Url.Action(nameof(VerifyEmail), "TicketPage", new { UserId = user.Id }, Request.Scheme);
                         return Created(url, "Please Verify Your Email");
                     }

                 }
                 */
                string DataFileString = string.Empty;
                byte[] BinaryDataFile = null;

                if (TicketForAdd.File.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        TicketForAdd.File.CopyTo(ms);
                        BinaryDataFile = ms.ToArray();
                        DataFileString = Convert.ToBase64String(BinaryDataFile);
                    }
                }

                bool resault = TicketManager.Add(UserId, new AddTicketDTO()
                {
                    DataFile = DataFileString,
                    Description = TicketForAdd.Description,
                    FileByte = BinaryDataFile,
                    Title = TicketForAdd.Title
                });
                if (resault)
                    return Created(Url.Action(nameof(GetAllTicket), "TicketPage", new { }, protocol: Request.Scheme), true);

                return BadRequest("Ticket Don't Add");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [Authorize]
        [HttpPut("UpdateTicket")]
        public IActionResult UpdateTicket(Models.UpdateTicketDTO UpdateTicket)
        {
            try
            {
                var Userid = User.Claims.SingleOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;
                int UserId = 0;
                int.TryParse(Userid, out UserId);

                if (UpdateTicket == null || UserId == 0)
                    return BadRequest();


                string DataFileString = string.Empty;
                byte[] binaryFile = null;
                var upTicket = new DataLayer.DTO.UpdateTicketDTO();
                if (UpdateTicket.File != null && UpdateTicket.File.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        UpdateTicket.File.CopyTo(ms);
                        binaryFile = ms.ToArray();
                        DataFileString = Convert.ToBase64String(binaryFile);
                        upTicket.DataFile = DataFileString;
                        upTicket.FileByte = binaryFile;
                    }
                }
                upTicket.Title = UpdateTicket.Title;
                upTicket.Description = UpdateTicket.description;
                upTicket.TicketId = UpdateTicket.TicketId;

                bool ResultUpdateTicket = TicketManager.Update(UserId, UpdateTicket.TicketId, upTicket);

                if (ResultUpdateTicket)
                    return Ok();

                return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [Authorize]
        [HttpDelete("DeleteTicket")]
        public IActionResult DeleteTicket(int TicketId)
        {
            try
            {
                var Userid = User.Claims.SingleOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;
                int UserId = 0;
                int.TryParse(Userid, out UserId);

                if (UserId == 0 || TicketId == 0)
                    return BadRequest();

                bool ResultDeleteTicket = TicketManager.Delete(UserId, TicketId);
                if (ResultDeleteTicket)
                    return Ok();

                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("RequestSupporter")]
        [AllowAnonymous]
        [Authorize]
        public IActionResult RequestForSupporter()
        {
            try
            {
                var Userid = User.Claims.SingleOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;
                int UserId = 0;
                int.TryParse(Userid, out UserId);

                var user = UserManager.Get(UserId);
                if (user == null)
                    return NotFound("user is Null");
                if (user.IsAdmin)
                    return BadRequest("You are Admin!");


                var Admin = Context.Users.Where(t => t.IsAdmin == true).SingleOrDefault();
                if (Admin == null)
                    return BadRequest("Admin Not Exist");

                user.CounterRequest++;
                var resualtEmail = EmailManager.SendEmail(Admin.Email, "Request for backup", $"UserId = {user.Id}<br></br>FullName = {user.FullName}<br></br>Username = {user.UserName}");
                if (resualtEmail != null && resualtEmail.IsCompleted)
                    return Ok("Email Request Sent");
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("VerifyEmail")]
        [Authorize]
        [AllowAnonymous]
        public IActionResult VerifyEmail(int Code)
        {
            try
            {
                var Userid = User.Claims.SingleOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;
                int UserId = 0;
                int.TryParse(Userid, out UserId);
                if (UserId == 0)
                    return BadRequest("UserId Not Found");

                var resualtEnterCode = UserManager.EnterCodeForConfirm(UserId, Code);
                if (resualtEnterCode)
                {
                    var url = Url.Action(nameof(AddTicket), "TicketPage", new { }, Request.Scheme);
                    return Created(url, "You Can Add Ticket");
                }
                return BadRequest("The code is incorrect");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
