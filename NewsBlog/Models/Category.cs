﻿using System.ComponentModel.DataAnnotations;

namespace NewsBlog.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public virtual ICollection<News> News { get; set; }  = new List<News>();  
    }
}
