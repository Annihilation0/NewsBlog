using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsBlog.Models
{
    public class News
    {
        [Key]
        public int NewsId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Published { get; set; }
        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        public string? ResourcePath { get; set; }
        public virtual User Author { get; set; } = new User();
        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
