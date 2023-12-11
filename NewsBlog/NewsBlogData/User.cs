using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsBlog.NewsBlogData
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        [ForeignKey("Role")]
        public int RoleId { get; set; }
        
        public virtual Role Role { get; set; }
        public virtual ICollection<News> News { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
