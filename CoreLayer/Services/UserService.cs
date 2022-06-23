using CoreLayer.Utlities;
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
    public class UserService : IUserService
    {
        private readonly DataLayer.DataBase.Context_DB Context_Db;
        private readonly CoreLayer.Services.IEmailService EmailManager;
        public UserService(DataLayer.DataBase.Context_DB context, IEmailService email)
        {
            Context_Db = context;
            EmailManager = email;
        }
       
        public User Authentication(UserAuthenticationDTO authentication)
        {
            if (authentication == null ||
                string.IsNullOrEmpty(authentication.UserName) ||
                string.IsNullOrEmpty(authentication.Password))
                return null;

            var user = Get(authentication.UserName);

            // Check if User Exists
            if (user == null)
                return null;

            // Check password
            string PasswordHash = CoreLayer.Utlities.Security.Get_Hash(authentication.Password);
            if (PasswordHash != user.HashPassword)
                return null;

            // is OK
            return user;

        }
        public bool CheckToken(string Email, int Code)
        {
            if (string.IsNullOrEmpty(Email) || Code == 0)
                return false;
            var TokensUser = Get(Email).Tokens.Where(t => t.IsExpire == false).ToList();
            foreach (var item in TokensUser)
            {
                if (item.TokenValue == Code)
                {
                    DateTime time = item.TimeInsert;
                    DateTime now = DateTime.Now;
                    var res = now - time;
                    if ((DateTime.Now - item.TimeInsert).TotalMinutes < 5)
                    {
                        item.IsExpire = true;
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }
        public bool Creat(UserRegisterDTO userRegister)
        {
            if (userRegister == null || string.IsNullOrEmpty(userRegister.Password))
                return false;
            var u1 = Get(userRegister.Email);
            var u2 = Get(userRegister.UserName);

            if (u1 != null || u2 != null)
                return false;
            var RoleUser = new Role()
            {
                NameEnglish = "Supporter",
                NamePersian = "پشتیبان"
            };
            var UserCreate = new User()
            {
                FullName = userRegister.FullName,
                UserName = userRegister.UserName,
                Email = userRegister.Email,
                HashPassword = CoreLayer.Utlities.Security.Get_Hash(userRegister.Password),
                IsActive = true,
                IsAdmin = false,
                IsConfirmEmail = false
            };
            UserCreate.Roles = new List<Role>();
            UserCreate.Roles.Add(RoleUser);
            try
            {
                Context_Db.Users.Add(UserCreate);
                Context_Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Delete(int id)
        {
            var user = Context_Db.Users.SingleOrDefault(t => t.Id == id);

            if (user == null)
            {
                return false;
            }
            try
            {
                user.IsRemove = true;
                Context_Db.Users.Update(user);
                Context_Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Delete(string Value)
        {
            var user = Context_Db.Users
                .SingleOrDefault(t => t.UserName.ToLower() == Value.ToLower()
            || t.Email.ToLower() == Value.ToLower());

            if (user == null)
                return false;
            user.IsRemove = true;
            try
            {
                Context_Db.Users.Update(user);
                Context_Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public User Get(int id)
        {
            return Context_Db.Users
                .Include(t => t.Tickets).Include(t => t.Roles).Include(t => t.Tokens)
                .SingleOrDefault(t => t.Id == id);
        }
        public User Get(string Info)
        {
            return Context_Db.Users
                .Include(t => t.Tickets).Include(t => t.Roles).Include(t => t.Tokens)
                .SingleOrDefault(t => t.UserName.ToLower() == Info.ToLower() ||
            t.Email.ToLower() == Info.ToLower());
        }
        public IEnumerable<User> Get()
        {
            return Context_Db.Users.Include(t => t.Roles).Include(t => t.Tickets).Include(t => t.Tokens).Where(t=>t.IsActive==true).ToList();
        }
        public bool ResetPassword(ResetPasswordDTO ResetPassword)
        {
            if (ResetPassword == null||
                string.IsNullOrEmpty(ResetPassword.Email) || string.IsNullOrEmpty(ResetPassword.Password))
                return false;
            try
            {
                var user = Get(ResetPassword.Email);
                user.HashPassword = Security.Get_Hash(ResetPassword.Password);
                Context_Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool SetToken(int id, int Code)
        {
            try
            {
                var user = Get(id);

                Token Token = new Token()
                {
                    IsExpire = false,
                    TimeInsert = DateTime.Now,
                    TokenValue = Code
                };
                user.Tokens.Add(Token);
                Context_Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool SetToken(string Value, int Code)
        {
            try
            {
                var user = Get(Value);

                Token Token = new Token()
                {
                    IsExpire = false,
                    TimeInsert = DateTime.Now,
                    TokenValue = Code
                };
                user.Tokens.Add(Token);
                Context_Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Update(int id, UserUpdateDTO UserUpdate)
        {
            var user = Context_Db.Users.SingleOrDefault(t => t.Id == id);
            if (user == null)
                return false;

            user.FullName = UserUpdate.FullName;
            user.UserName = UserUpdate.UserName;
            user.Email = UserUpdate.Email;
            user.HashPassword = CoreLayer.Utlities.Security.Get_Hash(UserUpdate.Password);

            try
            {
                Context_Db.Users.Update(user);
                Context_Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool SendEmailForConfirm(int UserId)
        {
            try
            {
                Random Random = new Random();
                int Number = Random.Next(100000, 999999);

                var user = Get(UserId);
                if (user == null)
                    return false;
                Token Code = new Token()
                {
                    IsExpire = false,
                    TimeInsert = DateTime.Now,
                    TokenValue = Number
                };
                user.Tokens.Add(Code);
                Context_Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool EnterCodeForConfirm(int UserId, int Code)
        {
            try
            {
                var user = Get(UserId);
                if (user == null)
                    return false;

                var CodesUser = user.Tokens.ToList();
                if (CodesUser == null)
                    return false;

                foreach (var item in CodesUser)
                {
                    if (!item.IsExpire && item.TokenValue == Code)
                    {
                        var time = DateTime.Now - item.TimeInsert;
                        if (time < TimeSpan.FromMinutes(6))
                        {
                            user.IsConfirmEmail = true;
                            Context_Db.SaveChanges();
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public bool AddRole(int UserId, string Role,string PersianRole)
        {
            try
            {
                var user = Get(UserId);
                if (string.IsNullOrEmpty(Role) && string.IsNullOrEmpty(PersianRole))
                    return false;
                Role roleUser = new Role()
                {
                    NameEnglish = Role,
                    NamePersian = PersianRole
                };
                user.Roles.Add(roleUser);
                Context_Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
