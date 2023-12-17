using NewsBlog.Models;

namespace NewsBlog.ViewModel
{
    public class NewsViewModel
    {
        public int NewsId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Published { get; set; }
        public string? ResourcePath { get; set; }
        public string Author { get; set; } = string.Empty;
        public List<string> Categories { get; set; }   = new List<string>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
