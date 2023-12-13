using NewsBlog.Models;

namespace NewsBlog.ViewModel
{
    public class NewsViewModel
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Published { get; set; }
        public string? ResourcePath { get; set; }
        public string Author { get; set; }
        public List<string> Categories { get; set; }
        public ICollection<Comment> Comments { get; set; }

    }
}
