using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsBlog.Models;
using NewsBlog.NewsBlogData;
using NewsBlog.ViewModel;

namespace NewsBlog.Controllers
{
    public class NewsController : Controller
    {
        private readonly DbContext context;
        public NewsController(DbContext context)
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
        public IActionResult AllNews()
        {
            var news = getAllNews(this.context);
            return View(news);
        }
        /*--------------------------------------
        PartialView для отображения результата
        поиска 
        --------------------------------------*/
        public IActionResult SearchNews(string search)
        {
            var searchingNews = SearchNewsByTitle(this.context, search);
            return PartialView(searchingNews);

        }
        public IActionResult SearchByCategoryNews(string category)
        {
            var searchingNews = SearchByCategoryNews(this.context, category);
            return PartialView(searchingNews);

        }
        /*--------------------------------------
        PartialView для отображения новости с
        заданным newsId
        --------------------------------------*/
        public IActionResult AddComment(string commentText, int newsId)
        {
            string userName = HttpContext.Session.GetString("userName") ?? string.Empty;
            var user = GetUserByUsername(this.context, userName);
            var news = context.News
               .Include(news => news.Categories)
               .Include(news => news.Author)
               .Select(news => new News
               {
                   NewsId = news.NewsId,
                   Title = news.Title,
                   Content = news.Content,
                   Published = news.Published,
                   Author = news.Author,
                   ResourcePath = news.ResourcePath,
                   Categories = news.Categories,
                   Comments = news.Comments,
               })
               .Where(news => news.NewsId.Equals(newsId)).First();
            var comments = SearchCommentOfNews(context, newsId);

            if ((user == null)||(news == null)) return PartialView("ReadNews", new { news, comments}); 

            AddComment(user, news, commentText);

            comments = SearchCommentOfNews(context, newsId);

            return RedirectToAction("ReadNews", new { newsId });
        }
        public IActionResult ReadNews(int newsId)
        {
            var searchingNews = new NewsAndCommentViewModel();

            searchingNews.News = SearchNewsById(this.context, newsId).First();
            searchingNews.Comments = SearchCommentOfNews(this.context, searchingNews.News.NewsId);

            return PartialView(searchingNews);
        }
        private IQueryable<CommentViewModel> SearchCommentOfNews(DbContext context, int newsId)
        {
            var res = context.Comments
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
        private IQueryable<NewsViewModel> SearchNewsById(DbContext context, int newsId)
        {
            var res = context.News
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
               })
               .Where(news => news.NewsId.Equals(newsId));
            return res;
        }
        /*--------------------------------------
        Получение всех новостей из контекста базы данных,
        заголовок которых содержит в себе строку search
        --------------------------------------*/
        private IQueryable<NewsViewModel> SearchNewsByTitle(DbContext context, string search)
        {
            search ??= "";
            var res = context.News
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
        private IQueryable<NewsViewModel> SearchByCategoryNews(DbContext context, string category)
        {
            category ??= "";
            var res = context.News
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
        /*--------------------------------------
        Получение пользователя по userName
        --------------------------------------*/
        private User? GetUserByUsername(DbContext context, string userName)
        {
            var users = context.Users;
            var res = users.Select(user => user).Where(user => (user.UserName ?? string.Empty).ToLower().Equals(userName.ToLower())).FirstOrDefault();
            return res;
        }
        /*--------------------------------------
        Добавление нового комментария
        --------------------------------------*/
        private void AddComment(User author, News news, string commentText)
        {
            var comments = context.Comments;

            context.Comments.Add(new Comment
            {
                Published = DateTime.UtcNow,
                Author = author,
                News = context.News.Select(n => n).Where(n => n.NewsId.Equals(news.NewsId)).First(),
                Content = commentText
            });

            context.SaveChanges();
        }
    }
}
