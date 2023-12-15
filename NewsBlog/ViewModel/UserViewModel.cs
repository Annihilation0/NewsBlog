using NewsBlog.Models;

namespace NewsBlog.ViewModel
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PasswordHash { get; set; }
        public string PasswordSalt { get; set; } = string.Empty;
        public Role? Role { get; set; }
        public List<News> News { get; set; } = Enumerable.Empty<News>().ToList();
        public List<Comment> Comments { get; set; } = Enumerable.Empty<Comment>().ToList();

    }
}
