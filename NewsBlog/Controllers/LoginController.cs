using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsBlog.Models;
using NewsBlog.NewsBlogData;
using NewsBlog.ViewModel;

namespace NewsBlog.Controllers
{
    public class LoginController : Controller
    {
        private readonly DbContext context;
        public LoginController(DbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        //Ошибка - пустое поле с логином
        public IActionResult InvalidLoginEmptyUsername(string userName, string password)
        {
            return PartialView(FillLoginDataUserViewModel(userName, password, "Введите логин"));
        }
        //Ошибка - пустое поле с паролем
        public IActionResult InvalidLoginEmptyPassword(string userName, string password)
        {
            return PartialView(FillLoginDataUserViewModel(userName, password, "Введите пароль"));
        }
        //Ошибка - пользователь с таким логином не найден
        public IActionResult InvalidLoginUserDoesntExists(string userName, string password)
        {
            return PartialView(FillLoginDataUserViewModel(userName, password, "Пользователь не найден"));
        }
        //Ошибка - неверный пароль
        public IActionResult InvalidLoginUserInvalidPassword(string userName, string password)
        {
            return PartialView(FillLoginDataUserViewModel(userName, password, "Неверный пароль"));
        }
        //Успешный вход
        public IActionResult SuccessfulLoginUser(string userName, string password)
        {
            var user = GetUserByUsername(context, userName);
            string firstName = user.FirstName ?? string.Empty;
            string lastName = user.LastName ?? string.Empty;

            HttpContext.Session.SetString("userName", userName);
            HttpContext.Session.SetString("firstName", firstName);
            HttpContext.Session.SetString("lastName", lastName);

            return PartialView(FillLoginDataUserViewModel(userName, password, ""));
        }
        public IActionResult Login(string userName, string password)
        {
            if (userName is null)
            {
                return RedirectToAction("InvalidLoginEmptyUsername",
                    new { UserName = userName, Password = password });
            }
            if (password is null)
            {
                return RedirectToAction("InvalidLoginEmptyPassword",
                    new { UserName = userName, Password = password });
            }
            bool userExists = CheckIfUserExists(this.context, userName);
            // пользователь не найден
            if (!userExists)
            {
                return RedirectToAction("InvalidLoginUserDoesntExists",
                    new { UserName = userName, Password = password });
            }
            bool isLoginSuccessful = IsLoginSuccessful(userName, password);
            if (!isLoginSuccessful)
            {
                return RedirectToAction("InvalidLoginUserInvalidPassword",
                    new { UserName = userName, Password = password });
            }
            return RedirectToAction("SuccessfulLoginUser",
                    new { UserName = userName, Password = password });
        }
        private LoginDataUserViewModel FillLoginDataUserViewModel(string userName, string password, string error)
        {
            LoginDataUserViewModel viewModel = new LoginDataUserViewModel();
            viewModel.UserName = userName;
            viewModel.Password = password;
            viewModel.ErrorMessage = error;

            var user = GetUserByUsername(context, userName);
            if (user != null)
            {
                viewModel.FirstName = (user.FirstName != null) ? user.FirstName : string.Empty;
                viewModel.LastName = (user.LastName != null) ? user.LastName : string.Empty;
            }
            return viewModel;
        }
        private bool CheckIfUserExists(DbContext context, string userName)
        {
            var users = context.Users;
            if(users == null) return false;
            var res = users.Select(user => user).Where(user => (user.UserName ?? string.Empty).ToLower().Equals(userName.ToLower())).First();
            if(res == null) return false;
            return true;
        }
        private User? GetUserByUsername(DbContext context, string userName)
        {
            var users = context.Users;
            var res = users.Select(user => user).Where(user => (user.UserName ?? string.Empty).ToLower().Equals(userName.ToLower())).FirstOrDefault();
            return res;
        }
        private bool IsLoginSuccessful(string userName, string password)
        {      
            User user = context.Users.Select(user => user).Where(user => (user.UserName ?? string.Empty).ToLower().Equals(userName.ToLower())).First();
            if ((user.UserName == userName) && (user.PasswordHash == PasswordHashing.GetHashString(password))) return true;
            else return false;
        }
    }
}
