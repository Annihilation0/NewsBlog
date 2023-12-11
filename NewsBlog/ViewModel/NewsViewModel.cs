using NewsBlog.NewsBlogData;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsBlog.ViewModel
{
    public class NewsViewModel
    {
        public int NewsId { get; set; }
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
