using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsBlog.ViewModel;

namespace NewsBlog.Controllers
{
    public class LogoutController : Controller
    {
        private readonly DbContext context;
        public LogoutController(DbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            HttpContext.Session.SetString("userName", "");
            var news = GetAllNews(context);
            return View("../News/AllNews", news);
        }
        private IQueryable<NewsViewModel> GetAllNews(DbContext context)
        {
            var news = context.News
                .Include(news => news.Categories)
                .Include(news => news.Author)
                .Select(news => new NewsViewModel
                {
                    NewsId = news.NewsId,
                    Title = news.Title,
                    Content = news.Content,
                    Published = news.Published,
                    Author = news.Author.FirstName + " " + news.Author.LastName,
                    ResourcePath = news.ResourcePath,
                    Categories = news.Categories.Select(category => category.CategoryName).ToList(),
                    Comments = news.Comments,
                });

            return news;
        }
    }
}
