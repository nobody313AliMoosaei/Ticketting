using DataLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Api_Center.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagmentController : ControllerBase
    {
        private readonly CoreLayer.Services.IUserService UserManager;
        public UserManagmentController(CoreLayer.Services.IUserService usermanag)
        {
            UserManager = usermanag;
        }

        [HttpGet]
        public IEnumerable<User> Get()
        {
            return UserManager.Get();
        }

        [HttpGet("{id}")]
        public User Get(int id)
        {
            if (id == 0)
                return null;

          return UserManager.Get(id);
        }

        [HttpPost]
        public bool Post([FromBody] DataLayer.DTO.UserRegisterDTO UserRegister)
        {
            return UserManager.Creat(UserRegister);
        }

        [HttpPut]
        public bool Put(int id, [FromBody] DataLayer.DTO.UserUpdateDTO userUpdate)
        {
            if (id == 0)
                return false;

            return UserManager.Update(id, userUpdate);
        }

        [HttpDelete("{id}")]
        public bool Delete(int id)
        {
            if (id == 0)
                return false;
            return UserManager.Delete(id);
        }
        [HttpDelete("{Value}")]
        public bool Delete(string Value)
        {
            if (string.IsNullOrEmpty(Value))
                return false;

            return UserManager.Delete(Value);
        }
    }
}
