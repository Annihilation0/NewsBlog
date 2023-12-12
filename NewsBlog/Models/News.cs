using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsBlog.Models
{
    public class News
    {
        [Key]
        public int NewsId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime Published { get; set; }
        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        public string? ResourcePath { get; set; }
        public virtual User Author { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
