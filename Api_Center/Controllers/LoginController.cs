using DataLayer.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using CoreLayer.Services;
using Api_Center.Models;

namespace Api_Center.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        #region Dependency injection
        private readonly IUserService UserManager;
        private readonly IMethodHelper MethodHelper;
        private readonly IEmailService EmailService;

        public LoginController(CoreLayer.Services.IUserService usermanager, IConfiguration con,IMethodHelper h,
            IEmailService email)
        {
            UserManager = usermanager;
            EmailService = email;
            MethodHelper = h;
        }
        #endregion

        [AllowAnonymous]
        [HttpPost("LoginUsers")]
        public IActionResult Login([FromBody]UserAuthenticationDTO UserLoginData)
        {
            var user = UserManager.Authentication(UserLoginData);

            if (user == null)
                return BadRequest("User not Found ...");
            if (user.IsActive == false)
                return BadRequest("Is Active == False");

            var Token = MethodHelper.CreatToken(user.Id, user.FullName);

           
            return Ok(new
            {
                Id = user.Id,
                Email = user.Email,
                FullNem = user.FullName,
                UserName = user.UserName,
                Token = Token
            });

        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserRegisterDTO UserRegister)
        {
            var user = UserManager.Creat(UserRegister);
            if (user == false)
                return BadRequest("There is a user in the database or there is a problem with the parameters");

            return Ok();

        }

        [AllowAnonymous]
        [HttpPost("ForgetpasswordByUsername")]
        public IActionResult ForgetPassword(string Value)
        {
            var user = UserManager.Get(Value);
            if (user == null)
                return NotFound();

            int code;
            Random random = new Random();
            code = random.Next(100000, 999999);
            UserManager.SetToken(Value, code);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Hi {user.FullName}");
            sb.AppendLine($"Please enter this code in web site");
            sb.AppendLine($"Code = {code}");
            sb.AppendLine("good luck");

            var result = EmailService.SendEmail(user.Email, "Code Ticketting", sb.ToString());
            if (result != null && result.IsCompleted)
            {
                // Creat Link
                string LinkEnterCode = Url.Action(nameof(EnterCode), "Login", new { EmailUser = user.Email },protocol: Request.Scheme);
                return Created(LinkEnterCode,true);
            }

            return BadRequest();

        }

        [AllowAnonymous]
        [HttpPost("EnterCode")]
        public IActionResult EnterCode(string EmailUser, string Code)
        {
            int MainCode = 0;
            int.TryParse(Code, out MainCode);

            if (MainCode == 0)
                return BadRequest();
            if (UserManager.CheckToken(EmailUser, MainCode))
            {
                // Url ResetPassword
                string UrlResetPassword = Url.Action(nameof(ResetPassword), "Login", new { }, Request.Scheme);
                return Created(UrlResetPassword,true);
            }
            return NotFound();
        }

        [AllowAnonymous]
        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(ResetPasswordDTO ResetPassword)
        {
            bool ResultResetPassword = UserManager.ResetPassword(ResetPassword);
            if (ResultResetPassword)
                return Ok();
            return BadRequest();
        }


        /*
        [HttpGet("GoogleLogin")]
        public IActionResult GoogleLogin(string name)
        {
            var Properties = new AuthenticationProperties()
            {
                RedirectUri = Url.Action(nameof(ResponsGoogle))
            };
            return Challenge(Properties, GoogleDefaults.AuthenticationScheme);

        }
        [HttpGet("GoogleRespons")]
        public async Task<IActionResult> ResponsGoogle()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            var Claims = result.Principal.Identities.FirstOrDefault().Claims;

            return Ok();
        }
        */
       
    }
}
