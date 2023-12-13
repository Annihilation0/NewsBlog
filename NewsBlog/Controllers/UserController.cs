using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsBlog.Models;
using NewsBlog.NewsBlogData;
using NewsBlog.ViewModel;
using System;
using System.Diagnostics;

namespace NewsBlog.Controllers
{
    public class UserController : Controller
    {
        private readonly DbContext context;
        private readonly int saltSize = 16;
        public UserController(DbContext context)
        {
            this.context = context;
        }
        /*--------------------------------------
        Отображение всех новостей из контекста
        базы данных
        --------------------------------------*/
        public IActionResult Index()
        {
            return View();
        }
        private RegistrationDataUserViewModel FillRegistrationDataUserViewModel(string userName, string firstName, string lastName, string password)
        {
            RegistrationDataUserViewModel viewModel = new RegistrationDataUserViewModel();
            viewModel.UserName = userName;
            viewModel.FirstName = firstName;
            viewModel.LastName = lastName;
            viewModel.Password = password;
            return viewModel;
        }
        public IActionResult ValidRegistrationUser(string userName, string firstName, string lastName, string password)
        {          
            return PartialView(FillRegistrationDataUserViewModel(userName, firstName, lastName, password));
        }
        public IActionResult InvalidRegistrationUserAlreadyExists(string userName, string firstName, string lastName, string password)
        {
            return PartialView(FillRegistrationDataUserViewModel(userName, firstName, lastName, password));
        }
        public IActionResult InvalidRegistrationEmptyUsername(string userName, string firstName, string lastName, string password)
        {
            return PartialView(FillRegistrationDataUserViewModel(userName, firstName, lastName, password));
        }
        public IActionResult RegistrationUser(string userName, string firstName, string lastName, string password)
        {
            if (userName is null) {
                return RedirectToAction("InvalidRegistrationEmptyUsername",
                    new { UserName = userName, FirstName = firstName, LastName = lastName, Password = password });
            }
            var userExists = CheckIfUserExists(this.context, userName, firstName, lastName, password);
            // пользователь с таким username уже существует
            if (userExists)
            {
                return RedirectToAction("InvalidRegistrationUserAlreadyExists",
                    new { UserName = userName, FirstName = firstName, LastName = lastName, Password = password });
            }
            else
            {
                //запрос к бд для добавления нового пользователя + хэш пароля
                this.context.Users.Add(new User
                {
                    UserName = userName,
                    FirstName = firstName,
                    LastName = lastName,
                    PasswordHash = PasswordHashing.GetHashString(password),
                    PasswordSalt = PasswordSaltGenerator.GenerateSalt(saltSize),
                    Role = context.Roles.Where(role => role.RoleName == "User").First(),
                });
                this.context.SaveChanges();
                return RedirectToAction("ValidRegistrationUser",
                    new { UserName = userName, FirstName = firstName, LastName = lastName, Password = password});            
            }

        }
        private bool CheckIfUserExists(DbContext context, string userName, string firstName, string lastName, string password)
        {
            bool userExists = userNameIsExist(context, userName);
            return userExists;
        }
        private bool userNameIsExist(DbContext context, string userName) {
            var users = context.Users;
            return users.Select(user => user.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)).First();
        }
        public IActionResult AllUser()
        {
            var users = getAllUsers(this.context);
            return View();
        }
        private IQueryable<UserViewModel> getAllUsers(DbContext context)
        {
            var users = context.Users
                .Include(users => users.Role)
                .Include(users => users.News)
                .Include(users => users.Comments)
                .Select(users => new UserViewModel
                {
                    UserId = users.UserId,
                    UserName = users.UserName,
                    FirstName = users.FirstName,
                    LastName = users.LastName,
                    PasswordHash = users.PasswordHash,
                    PasswordSalt = users.PasswordSalt,
                    Role = users.Role,
                    News = users.News.ToList(),
                    Comments = users.Comments.ToList(),
                });

            return users;
        }


    }
}
