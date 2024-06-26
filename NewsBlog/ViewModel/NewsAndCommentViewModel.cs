﻿namespace NewsBlog.ViewModel
{
    public class NewsAndCommentViewModel
    {
        public NewsViewModel News { get; set; } = new NewsViewModel();
        public IQueryable<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>().AsQueryable();

        public IQueryable<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>().AsQueryable();
    }


}
