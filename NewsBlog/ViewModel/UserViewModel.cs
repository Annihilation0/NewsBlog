using NewsBlog.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsBlog.ViewModel
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public Role Role { get; set; }
        public ICollection<News> News { get; set; }
        public ICollection<Comment> Comments { get; set; }

    }
}
