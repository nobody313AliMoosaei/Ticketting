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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
namespace Api_Center.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class AdminController : ControllerBase
    {
        private readonly ITicketService TicketManager;
        private readonly IUserService UserManager;
        private readonly Context_DB Context;
        public AdminController(ITicketService ticket, IUserService user, Context_DB con)
        {
            TicketManager = ticket;
            UserManager = user;
            Context = con;
        }

        [HttpGet("GetAllTickets")]
        [AllowAnonymous]
        [Authorize]
        public IActionResult GetAllTickets()
        {
            try
            {
                var tickets = TicketManager.Get();
                var mapper = Mapper.GetListTicketForShow(tickets);

                return Ok(mapper);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("GetAllUser")]
        [AllowAnonymous]
        [Authorize]
        public IActionResult GetAllUser()
        {
            try
            {
                var AllUser = Mapper.GetUserForShow(UserManager.Get().ToList());
                return Ok(AllUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [Authorize]
        [HttpGet("GetAllSupporter")]
        public IActionResult GetAllSupporter()
        {
            try
            {
                List<User> Supporeters = new List<User>();
                var Users = Context.Users.Include(t => t.Roles).ToList();
                foreach (var item in Users)
                {
                    var RolesUser = item.Roles.ToList();
                    foreach (var Role in RolesUser)
                    {
                        if (Role.NameEnglish.ToLower().Contains("supporter"))
                        {
                            Supporeters.Add(item);
                            break;
                        }
                    }
                }
                var AllSupporter = Mapper.GetSupporterForShow(Supporeters);
                return Ok(AllSupporter);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [AllowAnonymous]
        [HttpGet("GetUserHasRequest")]
        public IActionResult GetUserHasRequest()
        {
            try
            {
                var user = Context.Users.Where(t => t.CounterRequest > 0).ToList();
                if (user == null)
                    return NotFound("There is no request");
                var AllUser = Mapper.GetUserForShow(user);
                return Ok(AllUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [AllowAnonymous]
        [HttpGet("GetTicketsHasReport")]
        public IActionResult GetTicketHasReport()
        {
            try
            {
                var Tickets = Context.Tickets.
                    Include(t => t.User).Include(t => t.Responses).Where(t => t.CounterReport > 0).ToList();
                if (Tickets == null)
                    return NotFound();
                var AllTicket = Mapper.GetListTicketForShow(Tickets);
                return Ok(AllTicket);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [Authorize]
        [HttpPost("AcceptRequestSupporter")]
        public IActionResult AcceptRequest(int UserId)
        {
            try
            {
                if (UserId == 0)
                    return BadRequest();

                var user = UserManager.Get(UserId);

                if (user == null)
                    return NotFound("User = Null");

                if (user.CounterRequest == 0 || user.CounterRequest < 0)
                    return BadRequest();

                var r = UserManager.AddRole(UserId, "supporter", "پشتیبان");
                if (r)
                {
                    return Created(Url.Action(nameof(GetAllSupporter), "Admin", new { }, Request.Scheme), true);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [Authorize]
        [HttpPost("BlockUser")]
        public IActionResult BlockUser(int UserID)
        {
            try
            {
                var user = UserManager.Get(UserID);
                if (user == null)
                    return NotFound();
                user.IsActive = false;
                Context.SaveChanges();
                return Created(Url.Action(nameof(GetAllUser), "Admin", new { }, Request.Scheme), true);
            }
            catch
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [Authorize]
        [HttpPost("DeleteTicket")]
        public IActionResult DeleteTicket(int UserID, int TicketID)
        {
            try
            {
                var r = TicketManager.Delete(UserID, TicketID);
                if (r)
                    return Created(Url.Action(nameof(GetAllTickets), "Admin", new { }, Request.Scheme), true);
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
