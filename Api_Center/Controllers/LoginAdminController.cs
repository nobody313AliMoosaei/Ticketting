using Api_Center.Models;
using CoreLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class LoginAdminController : ControllerBase
    {
        private readonly IUserService UserManager;
        private readonly IMethodHelper methodHelper;
        public LoginAdminController(IUserService userService, IMethodHelper helper)
        {
            UserManager = userService;
            methodHelper = helper;
        }

        [AllowAnonymous]
        [HttpPost("LoginAdmin")]
        public IActionResult Login(DataLayer.DTO.UserAuthenticationDTO DataUser)
        {try
            {
                if (DataUser == null)
                    return NoContent();
                var ResultUser = UserManager.Authentication(DataUser);
                if (ResultUser == null)
                    return BadRequest();
                if (ResultUser.IsAdmin)
                {
                    var Token = methodHelper.CreatToken(ResultUser.Id, ResultUser.UserName);
                    return Ok(new
                    {
                        Id = ResultUser.Id,
                        FullName = ResultUser.FullName,
                        UserName = ResultUser.UserName,
                        Email = ResultUser.Email,
                        Token = Token
                    });
                }
                return NotFound();
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
