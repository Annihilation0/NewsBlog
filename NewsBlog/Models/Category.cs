namespace NewsBlog.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public virtual ICollection<News> News { get; set; }
    }
}
