using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsBlog.Models;
using NewsBlog.NewsBlogData;
using NewsBlog.ViewModel;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

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
            var news = getAllNews();
            return View(news);
        }
        public IActionResult BuilderNews(string? errorMessage)
        {
            var categories = context.Categories.Select(categories => new CategoryViewModel
                {
                    CategoryId = categories.CategoryId,
                    CategoryName = categories.CategoryName,
                    ErrorMessage = errorMessage ?? string.Empty
            });
            return View(categories);
        }
        public IActionResult InvalidBuilderNews(string? errorMessage)
        {
            var categories = context.Categories.Select(categories => new CategoryViewModel
            {
                CategoryId = categories.CategoryId,
                CategoryName = categories.CategoryName,
                ErrorMessage = errorMessage ?? string.Empty
            });
            return PartialView(categories);
        }
        public IActionResult ShowAddCategory(string? errorMessage)
        {
            var categories = context.Categories.Select(categories => new CategoryViewModel
            {
                CategoryId = categories.CategoryId,
                CategoryName = categories.CategoryName,
                ErrorMessage = errorMessage ?? string.Empty
            });
            return PartialView(categories);
        }
        public IActionResult MyNews()
        {
            string userName = HttpContext.Session.GetString("userName") ?? string.Empty;
            var news = getUserNews(userName);
            return View(news);
        }
        public IActionResult AddNews(string newsTitle, string newsCategories, string newsText, string path)
        {
            string errorMessage = "";
            if (string.IsNullOrWhiteSpace(newsTitle))
            {
                errorMessage = "Заполните заголовок новости";
                return RedirectToAction("InvalidBuilderNews", new { errorMessage });
            }
            if (string.IsNullOrWhiteSpace(newsCategories))
            {
                errorMessage = "Выберите хотя бы одну категорию";
                return RedirectToAction("InvalidBuilderNews", new { errorMessage });
            }
            if (string.IsNullOrWhiteSpace(newsText))
            {
                errorMessage = "Заполните текст новости";
                return RedirectToAction("InvalidBuilderNews", new { errorMessage });
            }
            string[] newsCategoriesArr = newsCategories.Split(',');
            List<Category> categories = new List<Category>();
            foreach (string categoryId in newsCategoriesArr)
            {
                categories.Add(GetCategoryById(Int32.Parse(categoryId)));
            }
            string userName = HttpContext.Session.GetString("userName") ?? string.Empty;
            var author = GetUserByUsername(userName);
            AddNews(newsTitle, newsText, categories, author, path);

            var news = getUserNews(userName);

            return PartialView("MyNews", news);
        }
        public IActionResult DeleteNews(int newsId)
        {
            var deletedNews = context.News.Select(news => news).Where(news => news.NewsId.Equals(newsId)).FirstOrDefault();
            context.News.Remove(deletedNews);
            context.SaveChanges();

            string userName = HttpContext.Session.GetString("userName") ?? string.Empty;
            var news = getUserNews(userName);
            return PartialView("MyNews", news);
        }
        /*--------------------------------------
        PartialView для отображения результата
        поиска 
        --------------------------------------*/
        public IActionResult SearchNews(string search)
        {
            var searchingNews = SearchNewsByTitle(search);
            return PartialView(searchingNews);
        }
        public IActionResult SearchByCategoryNews(string category)
        {
            var searchingNews = SearchNewsViewModelByCategory(category);
            return PartialView(searchingNews);
        }
        /*--------------------------------------
        PartialView для отображения новости с
        заданным newsId
        --------------------------------------*/
        public IActionResult AddComment(string commentText, int newsId)
        {
            string userName = HttpContext.Session.GetString("userName") ?? string.Empty;
            var user = GetUserByUsername(userName);
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
            var comments = SearchCommentOfNews(newsId);

            if ((user == null)||(news == null)) return PartialView("ReadNews", new { news, comments}); 

            AddComment(user, news, commentText);

            comments = SearchCommentOfNews(newsId);

            return RedirectToAction("ReadNews", new { newsId });
        }
        public IActionResult ReadNews(int newsId)
        {
            var searchingNews = new NewsAndCommentViewModel();

            searchingNews.News = SearchNewsById(newsId).First();
            searchingNews.Comments = SearchCommentOfNews(searchingNews.News.NewsId);

            return PartialView(searchingNews);
        }
        public IActionResult AddCategory(string categoryName)
        {
            AddCategoryName(categoryName);
            var categories = context.Categories
               .Select(category => new CategoryViewModel
               {
                   CategoryName = category.CategoryName,
               });
            return RedirectToAction("ShowAddCategory", new {string.Empty});
        }
        private IQueryable<CommentViewModel> SearchCommentOfNews(int newsId)
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
        private IQueryable<NewsViewModel> SearchNewsById(int newsId)
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
        private IQueryable<NewsViewModel> SearchNewsByTitle(string search)
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
        private IQueryable<NewsViewModel> SearchNewsViewModelByCategory(string category)
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
                   Categories = news.Categories.Select(category => category.CategoryName).ToList(),
                   Comments = news.Comments
               })
               .Where(news => news.Categories.Contains(category));
            return res;
        }
        private IQueryable<News>? SearchNewsByCategory(string category)
        {
            category ??= "";
            var categoryModel = context.Categories
                .Select(c => c).Where(c => c.CategoryName.Equals(category)).FirstOrDefault();
            if (categoryModel == null) return null;
            var res = context.News
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
                   Comments = news.Comments
               })
               .Where(news => news.Categories.Contains(categoryModel));
            return res;
        }
        /*--------------------------------------
        Получение всех новостей 
        --------------------------------------*/
        private IQueryable<NewsViewModel> getAllNews()
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
        /*--------------------------------------
        Получение новостей пользователя
        --------------------------------------*/
        private IQueryable<NewsViewModel> getUserNews(string userName)
        {
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
                .Where(news => (news.Author.UserName ?? string.Empty).ToLower().Equals(userName.ToLower()))
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
        /*--------------------------------------
        Получение пользователя по userName
        --------------------------------------*/
        private User? GetUserByUsername(string userName)
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
        private void AddNews(string title, string content, List<Category> categories, User author, string path)
        {
            var news = context.News;
            if (string.IsNullOrEmpty(path))
                path = "/css/Resources/default.jpg";
            context.News.Add(new News
            {
                Title = title,
                Content = content,
                Published = DateTime.UtcNow,
                Author = author,
                ResourcePath = path,
                Categories = categories
            });

            context.SaveChanges();
        }
        private Category GetCategoryById(int categoryId)
        {
            var categories = context.Categories;
            var res = categories.Select(categories => categories)
                .Where(categories => categories.CategoryId.Equals(categoryId)).FirstOrDefault();
            return res;
        }

        private void AddCategoryName(string categoryName)
        {

            if (context.Categories.Select(c => c)
                .Where(c => c.CategoryName.ToLower().Equals(categoryName.ToLower())).FirstOrDefault() != null) return; 
            context.Categories.Add(new Category
            {             
                CategoryName = categoryName
            });

            context.SaveChanges();
        }
    }
}
