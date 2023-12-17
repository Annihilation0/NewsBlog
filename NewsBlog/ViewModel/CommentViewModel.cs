using NewsBlog.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsBlog.ViewModel
{
    public class CommentViewModel
    {
        public int CommentId { get; set; }
        public DateTime Published { get; set; }
        public string? Content { get; set; }
        public string Author { get; set; } = string.Empty;
        public News News { get; set; } = new News();
    }
   
}
