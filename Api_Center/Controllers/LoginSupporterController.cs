using Api_Center.Models;
using CoreLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Center.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginSupporterController : ControllerBase
    {
        private readonly IUserService UserManager;
        private readonly IMethodHelper methodHelper;
        public LoginSupporterController(IUserService userService, IMethodHelper helper)
        {
            UserManager = userService;
            methodHelper = helper;
        }

        [AllowAnonymous]
        [HttpPost("LoginSupporter")]
        public IActionResult Login(DataLayer.DTO.UserAuthenticationDTO UserData)
        {
            var ResultUser = UserManager.Authentication(UserData);
            if (ResultUser == null)
                return BadRequest();
            var RolesUser = ResultUser.Roles.ToList();
            bool flag = false;
            for (int i = 0; i < RolesUser.Count; i++)
            {
                if (RolesUser[i].NameEnglish.ToLower().Contains("supporter")
                    || RolesUser[i].NamePersian.ToLower().Contains("پشتیبان"))
                    flag = true;
            }
            if (flag)
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
    }
}
