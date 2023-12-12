using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsBlog.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public DateTime Published { get; set; }
        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        [ForeignKey("News")]
        public int NewsId { get; set; }
        public string? Content { get; set; }
        public virtual User Author { get; set; }
        public virtual News News { get; set; }

    }
}
