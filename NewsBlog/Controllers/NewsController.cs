using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsBlog.Models;
using NewsBlog.ViewModel;
using System.Data.SqlTypes;
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
        Отображение всех новостей из контекста
        базы данных
        --------------------------------------*/
        public IActionResult Index()
        {
            
            return View();
        }
        public IActionResult AllNews()
        {
            var news = getAllNews(this.dbContext);
            return View(news);
        }
        /*--------------------------------------
        PartialView для отображения результата
        поиска 
        --------------------------------------*/
        public IActionResult SearchNews(string search)
        {
            var searchingNews = SearchNewsByTitle(this.dbContext, search);
            return PartialView(searchingNews);

        }
        public IActionResult SearchByCategoryNews(string category)
        {
            var searchingNews = SearchByCategoryNews(this.dbContext, category);
            return PartialView(searchingNews);

        }
        /*--------------------------------------
        PartialView для отображения новости с
        заданным newsId
        --------------------------------------*/
        public IActionResult ReadNews(int newsId)
        {
           //var searchingNews = SearchNewsById(this.Context, newsId);
           // return PartialView(searchingNews);

            var searchingNews = new NewsAndCommentViewModel();

            searchingNews.News = SearchNewsById(this.dbContext, newsId).First();
            searchingNews.Comments = SearchCommentOfNews(this.dbContext, searchingNews.News.NewsId);

            return PartialView(searchingNews);
        }
        private IQueryable<CommentViewModel> SearchCommentOfNews(DbContext dbContext, int newsId)
        {
            var res = dbContext.Comments
               .Include(comment => comment.Author)
               .Include(comment => comment.News)
               .Select(comment => new CommentViewModel
               {
                   CommentId = comment.CommentId,
                   Published = comment.Published,
                   Content = comment.Content,
                   Author = comment.Author.FirstName + " " + comment.Author.LastName,
                   News = comment.News
               })
               .Where(comment => comment.News.NewsId.Equals(newsId));

            return res;
        }
        /*--------------------------------------
        Получение новости из контекста базы данных
        по идентификатору newsId
        --------------------------------------*/
        private IQueryable<NewsViewModel> SearchNewsById(DbContext dbContext, int newsId)
        {
            var res = dbContext.News
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
                   //Categories = string.Join(" ", news.Categories.Select(category => category.CategoryName)),
                   Categories = news.Categories.Select(category => category.CategoryName).ToList(),
                   Comments = news.Comments,
               })
               .Where(news => news.NewsId.Equals(newsId));
            return res;
        }
        /*--------------------------------------
        Получение всех новостей из контекста базы данных,
        заголовок которых содержит в себе строку search
        --------------------------------------*/
        private IQueryable<NewsViewModel> SearchNewsByTitle(DbContext dbContext, string search)
        {
            search ??= "";
            var res = dbContext.News
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
                   //Categories = string.Join(" ", news.Categories.Select(category => category.CategoryName)),
                   Categories = news.Categories.Select(category => category.CategoryName).ToList(),
                   Comments = news.Comments,
               })
               .Where(news => news.Title.ToLower().Contains(search.ToLower()));
            return res;
        }
        private bool ContainsCaseInsensitive(string source, string substring)
        {
            return source?.IndexOf(substring, StringComparison.OrdinalIgnoreCase) > -1;
        }
        private IQueryable<NewsViewModel> SearchByCategoryNews(DbContext dbContext, string category)
        {
            category ??= "";
            var res = dbContext.News
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
                   //Categories = string.Join(" ", news.Categories.Select(category => category.CategoryName)),
                   Categories = news.Categories.Select(category => category.CategoryName).ToList(),
                   Comments = news.Comments
               })
               .Where(news => news.Categories.Contains(category));
            return res;
        }
        /*--------------------------------------
        Получение всех новостей из контекста
        базы данных
        --------------------------------------*/
        private IQueryable<NewsViewModel> getAllNews(DbContext context)
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
                    //Categories = string.Join(" ", news.Categories.Select(category => category.CategoryName)),
                    Categories = news.Categories.Select(category => category.CategoryName).ToList(),
                    Comments = news.Comments,
                });

            return news;
        }


  
    }
}
