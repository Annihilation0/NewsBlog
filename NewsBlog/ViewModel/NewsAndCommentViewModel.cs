namespace NewsBlog.ViewModel
{
    public class NewsAndCommentViewModel
    {
        public NewsViewModel News { get; set; }
        public IQueryable<CommentViewModel> Comments { get; set; }
    }


}
