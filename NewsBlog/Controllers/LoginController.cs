using Microsoft.AspNetCore.Mvc;
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
        //Ошибка
        public IActionResult InvalidLogin(string userName, string password, string errorMessage)
        {
            return PartialView(FillLoginDataUserViewModel(userName, password, errorMessage));
        }
   
        //Успешный вход
        public IActionResult SuccessfulLogin(string userName, string password)
        {
            var user = GetUserByUsername(userName);
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
                return RedirectToAction("InvalidLogin",
                    new { userName, password, ErrorMessage = "Введите логин"});
            }
            if (password is null)
            {
                return RedirectToAction("InvalidLogin",
                    new { userName, password, ErrorMessage = "Введите пароль" });
            }
            bool userExists = CheckIfUserExists(userName);
            // пользователь не найден
            if (!userExists)
            {
                return RedirectToAction("InvalidLogin",
                    new { userName, password, ErrorMessage = "Пользователь не найден" });
            }
            bool isLoginSuccessful = IsLoginSuccessful(userName, password);
            if (!isLoginSuccessful)
            {
                return RedirectToAction("InvalidLogin",
                    new { userName, password, ErrorMessage = "Неверный пароль" });
            }
            return RedirectToAction("SuccessfulLogin",
                    new {userName, password });
        }
        private LoginDataUserViewModel FillLoginDataUserViewModel(string userName, string password, string error)
        {
            LoginDataUserViewModel viewModel = new LoginDataUserViewModel();
            viewModel.UserName = userName;
            viewModel.Password = password;
            viewModel.ErrorMessage = error;

            var user = GetUserByUsername(userName);
            if (user != null)
            {
                viewModel.FirstName = (user.FirstName != null) ? user.FirstName : string.Empty;
                viewModel.LastName = (user.LastName != null) ? user.LastName : string.Empty;
            }
            return viewModel;
        }
        private bool CheckIfUserExists(string userName)
        {
            var users = context.Users;
            if(users == null) return false;
            var res = users.Select(user => user).Where(user => (user.UserName ?? string.Empty).ToLower().Equals(userName.ToLower())).FirstOrDefault();
            if(res == null) return false;
            return true;
        }
        private User? GetUserByUsername(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return null;
            var users = context.Users;
            var lowerUserName = userName.ToLower();
            var res = users.FirstOrDefault(user => ((user.UserName ?? string.Empty).ToLower() == lowerUserName));
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
