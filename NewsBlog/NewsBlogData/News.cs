using System.ComponentModel.DataAnnotations;

namespace NewsBlog.NewsBlogData
{
    public class News
    {
        public int NewsId { get; set; }
        [Key]
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime Published { get; set; }
        public int AuthorId { get; set; }
        public string? ResourcePath { get; set; }
        public virtual User Author { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
