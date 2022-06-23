using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.DTO;
using DataLayer.Entities;
namespace CoreLayer.Services
{
    public interface IUserService
    {
        User Authentication(UserAuthenticationDTO authentication);
        User Get(int id);
        User Get(string Info);
        IEnumerable<User> Get();
        bool Creat(UserRegisterDTO userRegister);
        bool Update(int id , UserUpdateDTO UserUpdate);
        bool Delete(int id);
        bool Delete(string Value);
        bool SetToken(int id,int Code);
        bool SetToken(string Value, int Code);
        bool CheckToken(string Email, int Code);
        bool ResetPassword(ResetPasswordDTO ResetPassword);
        public bool SendEmailForConfirm(int UserId);
        public bool EnterCodeForConfirm(int UserId, int Code);
        public bool AddRole(int UserId, string Role,string PersianRole);
    }
}
