using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsBlog.Models;
using NewsBlog.ViewModel;
using System;
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

        /*--------------------------------------
        Отображение всех новостей из контекста базы данных
        --------------------------------------*/
        public IActionResult Index()
        {
            var news = getAllNews(this.dbContext);
            return View(news);
        }

        public IActionResult SearchNews(string search)
        {
            var searchingNews = SearchNewsByTitle(this.dbContext, search);
            return PartialView(searchingNews);

        }

        private IQueryable<NewsViewModel> SearchNewsByTitle(DbContext dbContext, string search)
        {
             var res = dbContext.News
                .Include(news => news.Categories)
                .Include(news => news.Author)
                .Select(news => new NewsViewModel
                {
                    Title = news.Title,
                    Content = news.Content,
                    Published = news.Published,
                    Author = news.Author.FirstName + " " + news.Author.LastName,
                    ResourcePath = news.ResourcePath,
                    Categories = string.Join(" ", news.Categories.Select(category => category.CategoryName)),
                    Comments = string.Join(" ", news.Comments.Select(comment => comment.Content)),
                }).Where(news => news.Title.Contains(search));

            return res;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /*--------------------------------------
        Получение всех новостей из контекста базы данных
        --------------------------------------*/
        private IQueryable<NewsViewModel> getAllNews(DbContext context)
        {
            var news = context.News
                .Include(news => news.Categories)
                .Include(news => news.Author)
                .Select(news => new NewsViewModel
            {
                Title = news.Title,
                Content = news.Content,
                Published = news.Published,
                Author = news.Author.FirstName + " " + news.Author.LastName,
                ResourcePath = news.ResourcePath,
                Categories = string.Join(" ", news.Categories.Select(category => category.CategoryName)),
                Comments = string.Join(" ", news.Comments.Select(comment => comment.Content)),
                });

            return news;
        }
    }
}
