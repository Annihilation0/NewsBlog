using System.ComponentModel.DataAnnotations;

namespace NewsBlog.NewsBlogData
{
    public class Comment
    {
        public int CommentId { get; set; }
        [Key]
        public DateTime Published { get; set; }
        public int AuthorId { get; set; }
        public int NewsId { get; set; }
        public string? Content { get; set; }
        public virtual User Author { get; set; }
        public virtual News News { get; set; }

    }
}
