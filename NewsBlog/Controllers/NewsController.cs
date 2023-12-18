using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsBlog.Models;
using NewsBlog.ViewModel;


namespace NewsBlog.Controllers
{
    public class NewsController : Controller
    {
        private readonly DbContext context;
        private const string BlankTitleError = "Заполните заголовок новости";
        private const string NoCategoryError = "Выберите хотя бы одну категорию";
        private const string BlankTextError = "Заполните текст новости";
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
            var news = GetAllNews();
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
        public IActionResult GetDataNews(int newsId)
        {
            var news = GetNewsById(newsId).FirstOrDefault();
            var categories = context.Categories.Select(categories => new CategoryViewModel
            {
                CategoryId = categories.CategoryId,
                CategoryName = categories.CategoryName,
                ErrorMessage = string.Empty
            });

            var newsAndCategories = new NewsAndCommentViewModel();
            newsAndCategories.News = news ?? new NewsViewModel();
            newsAndCategories.Categories = categories;
            return PartialView(newsAndCategories);
        }
        public IActionResult UpdateNews(string newsTitle, string newsCategories, string newsText, string path, int newsId)
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

            UpdateNews(newsTitle, newsText, categories, author, path, newsId);
            var news = GetUserNews(userName);
            return PartialView("MyNews", news);
  
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
            var news = GetUserNews(userName);
            return View(news);
        }
        public IActionResult AddNews(string newsTitle, string newsCategories, string newsText, string path)
        {           
            string errorMessage = ValidateNewsInput(newsTitle, newsCategories, newsText);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return RedirectToAction(nameof(InvalidBuilderNews), new { errorMessage });
            }

            List<Category> categories = ParseCategories(newsCategories);
            string userName = HttpContext.Session.GetString("userName") ?? string.Empty;
            var author = GetUserByUsername(userName);
            AddNews(newsTitle, newsText, categories, author, path);

            var news = GetUserNews(userName);

            return PartialView("MyNews", news);
        }
        private string ValidateNewsInput(string title, string categories, string text)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return BlankTitleError;
            }
            if (string.IsNullOrWhiteSpace(categories))
            {
                return NoCategoryError;
            }
            if (string.IsNullOrWhiteSpace(text))
            {
                return BlankTextError;
            }

            return string.Empty;
        }
        private List<Category> ParseCategories(string categories)
        {
            var categoryIds = categories.Split(',');
            var categoryList = new List<Category>();
            foreach (var categoryId in categoryIds)
            {
                if (int.TryParse(categoryId, out int id))
                {
                    categoryList.Add(GetCategoryById(id));
                }
            }
            return categoryList;
        }
        public IActionResult DeleteNews(int newsId)
        {
            var deletedNews = context.News.Select(news => news).Where(news => news.NewsId.Equals(newsId)).FirstOrDefault();
            context.News.Remove(deletedNews);
            context.SaveChanges();

            string userName = HttpContext.Session.GetString("userName") ?? string.Empty;
            var news = GetUserNews(userName);
            return PartialView("MyNews", news);
        }
        /*--------------------------------------
        PartialView для отображения результата
        поиска 
        --------------------------------------*/
        public IActionResult SearchNews(string search)
        {
            var searchingNews = GetNewsByTitle(search);
            return PartialView(searchingNews);
        }
        public IActionResult SearchByCategoryNews(string category)
        {
            var searchingNews = GetNewsByCategory(category);
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
               .Where(news => news.NewsId.Equals(newsId)).FirstOrDefault();
            var comments = GetCommentOfNews(newsId);

            if ((user == null) || (news == null)) return PartialView("ReadNews", new { news, comments });

            AddComment(user, news, commentText);

            comments = GetCommentOfNews(newsId);

            return RedirectToAction("ReadNews", new { newsId });
        }
        public IActionResult ReadNews(int newsId)
        {
            var searchingNews = new NewsAndCommentViewModel();

            searchingNews.News = GetNewsById(newsId).First();
            searchingNews.Comments = GetCommentOfNews(searchingNews.News.NewsId);

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
            return RedirectToAction("ShowAddCategory", new { string.Empty });
        }
        private IQueryable<CommentViewModel> GetCommentOfNews(int newsId)
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
        private IQueryable<NewsViewModel> GetNewsById(int newsId)
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
        private IQueryable<NewsViewModel> GetNewsByTitle(string search)
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
        private IQueryable<NewsViewModel> GetNewsByCategory(string category)
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
        /*--------------------------------------
        Получение всех новостей 
        --------------------------------------*/
        private IQueryable<NewsViewModel> GetAllNews()
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
        private IQueryable<NewsViewModel> GetUserNews(string userName)
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
        private void UpdateNews(string title, string content, List<Category> categories, User author, string path, int newsId)
        {
            if (string.IsNullOrEmpty(path))
                path = "/css/Resources/default.jpg";
            var newsToUpdate = context.News.Include(n => n.Categories).FirstOrDefault(news => news.NewsId.Equals(newsId));
            if (newsToUpdate != null)
            {
                newsToUpdate.Title = title;
                newsToUpdate.Content = content;
                newsToUpdate.Published = DateTime.UtcNow;
                newsToUpdate.ResourcePath = path;

                // Очищаем существующие категории
                newsToUpdate.Categories.Clear();

                // Добавляем новые категории к новости
                foreach (var category in categories)
                {
                    var categoryToAdd = context.Categories.FirstOrDefault(c => c.CategoryId == category.CategoryId);
                    if (categoryToAdd != null)
                    {
                        newsToUpdate.Categories.Add(categoryToAdd);
                    }
                }

                // Сохраняем изменения в контексте
                context.SaveChanges();
            }



        }
        private Category? GetCategoryById(int categoryId)
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
