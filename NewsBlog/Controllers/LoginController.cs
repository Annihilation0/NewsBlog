using Microsoft.AspNetCore.Mvc;
using NewsBlog.Models;
using NewsBlog.NewsBlogData;
using NewsBlog.ViewModel;

namespace NewsBlog.Controllers
{
    public class LoginController : Controller
    {
        private readonly DbContext context;
        private readonly int saltSize = 16;
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
            return PartialView(FillLoginDataUserViewModel(userName, password));
        }
        //Ошибка - пустое поле с паролем
        public IActionResult InvalidLoginEmptyPassword(string userName, string password)
        {
            return PartialView(FillLoginDataUserViewModel(userName, password));
        }
        //Ошибка - пользователь с таким логином не найден
        public IActionResult InvalidLoginUserDoesntExists(string userName, string password)
        {
            return PartialView(FillLoginDataUserViewModel(userName, password));
        }
        //Ошибка - неверный пароль
        public IActionResult InvalidLoginUserInvalidPassword(string userName, string password)
        {
            return PartialView(FillLoginDataUserViewModel(userName, password));
        }
        //Успешный вход
        public IActionResult SuccessfulLoginUser(string userName, string password)
        {
            return PartialView(FillLoginDataUserViewModel(userName, password));
        }
        private LoginDataUserViewModel FillLoginDataUserViewModel(string userName, string password)
        {
            LoginDataUserViewModel viewModel = new LoginDataUserViewModel();
            viewModel.UserName = userName;
            viewModel.Password = password;
            return viewModel;
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
            bool successfulLogin = SuccessfulLogin(userName, password);
            if (!successfulLogin)
            {
                return RedirectToAction("InvalidLoginUserInvalidPassword",
                    new { UserName = userName, Password = password });
            }
            return RedirectToAction("SuccessfulLoginUser",
                    new { UserName = userName, Password = password });
        }
        private bool CheckIfUserExists(DbContext context, string userName)
        {
            var users = context.Users;
            if(users == null) return false;
            var res = users.Select(user => user).Where(user => user.UserName.ToLower().Equals(userName.ToLower())).First();
            if(res == null) return false;
            return true;
        }
        private bool SuccessfulLogin(string userName, string password)
        {      
            User user = context.Users.Select(user => user).Where(user => user.UserName.ToLower().Equals(userName.ToLower())).First();
            if ((user.UserName == userName) && (user.PasswordHash == PasswordHashing.GetHashString(password))) return true;
            else return false;
        }
    }
}
