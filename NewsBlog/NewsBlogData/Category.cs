using System.ComponentModel.DataAnnotations;

namespace NewsBlog.NewsBlogData
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Key]
        public string CategoryName { get; set; }
        public virtual ICollection<News> News { get; set; }
    }
}
