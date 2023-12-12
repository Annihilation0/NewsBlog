using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsBlog.Models;
using NewsBlog.ViewModel;
using System.Diagnostics;

namespace NewsBlog.Controllers
{
    public class NewsController : Controller
    {
        private readonly DbContext dbContext;
        public NewsController(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var news = this.dbContext.News.Include(n => n.Categories).Select(n => new NewsViewModel
            {
                Title = n.Title,
                Content = n.Content,
                Published = n.Published,
                Author = n.Author,
                ResourcePath = n.ResourcePath,
                Categories = n.Categories,
                Comments = n.Comments

            });
            return View(news);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
