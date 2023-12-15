using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsBlog.Models;
using NewsBlog.NewsBlogData;
using NewsBlog.ViewModel;

namespace NewsBlog.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly DbContext context;
        private readonly int saltSize = 16;
        public RegistrationController(DbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        private LoginDataUserViewModel FillLoginDataUserViewModel(string userName, string firstName, string lastName, string password, string errorMessage)
        {
            LoginDataUserViewModel viewModel = new LoginDataUserViewModel();
            viewModel.UserName = userName;
            viewModel.FirstName = firstName;
            viewModel.LastName = lastName;
            viewModel.Password = password;
            viewModel.ErrorMessage = errorMessage;
            return viewModel;
        }
        public IActionResult SuccessfulRegistrationUser(string userName, string firstName, string lastName, string password)
        {          
            return PartialView(FillLoginDataUserViewModel(userName, firstName, lastName, password,""));
        }
        public IActionResult InvalidRegistrationUserAlreadyExists(string userName, string firstName, string lastName, string password)
        {
            return PartialView(FillLoginDataUserViewModel(userName, firstName, lastName, password, "Пользователь с таким логином уже сущуствует"));
        }
        public IActionResult InvalidRegistrationEmptyUsername(string userName, string firstName, string lastName, string password)
        {
            return PartialView(FillLoginDataUserViewModel(userName, firstName, lastName, password, "Введите логин"));
        }
        public IActionResult InvalidRegistrationEmptyPassword(string userName, string firstName, string lastName, string password)
        {
            return PartialView(FillLoginDataUserViewModel(userName, firstName, lastName, password, "Введите пароль"));
        }
        public IActionResult RegistrationUser(string userName, string firstName, string lastName, string password)
        {
            if (userName is null) {
                return RedirectToAction("InvalidRegistrationEmptyUsername",
                    new { UserName = userName, FirstName = firstName, LastName = lastName, Password = password });
            }
            if (password is null)
            {
                return RedirectToAction("InvalidRegistrationEmptyPassword",
                    new { UserName = userName, FirstName = firstName, LastName = lastName, Password = password });
            }
            var userExists = CheckIfUserExists(this.context, userName);
            // Пользователь уже существует
            if (userExists)
            {
                return RedirectToAction("InvalidRegistrationUserAlreadyExists",
                    new { UserName = userName, FirstName = firstName, LastName = lastName, Password = password });
            }
            else
            {
                // Добавление нового пользователя
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
                //ViewData["userName"]= HttpContext.Session.GetString("userName");
                return RedirectToAction("SuccessfulRegistrationUser",
                    new { UserName = userName, FirstName = firstName, LastName = lastName, Password = password});            
            }
        }
        private bool CheckIfUserExists(DbContext context, string userName)
        {
            var users = context.Users;
            if (users == null) return false;
            var res = users.Select(user => user).Where(user => (user.UserName ?? string.Empty).ToLower().Equals(userName.ToLower())).FirstOrDefault();
            if (res == null) return false;
            return true;
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
