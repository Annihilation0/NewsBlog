﻿using Microsoft.EntityFrameworkCore;
using NewsBlog.NewsBlogData;



/// <summary>
/// Summary description for Class1
/// </summary>
public class BlogDbContext : DbContext
{

    public BlogDbContext(DbContextOptions options) : base(options)
    {
    }


    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Здесь можно настроить сложные связи, индексы и т.д.
        // Например, для связи многие ко многим между News и Categories
        modelBuilder.Entity<News>()
            .HasMany(n => n.Categories)
            .WithMany(c => c.News)
            .UsingEntity(j => j.ToTable("News_Categories"));

        // Связь для Комментариев
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.News)
            .WithMany(n => n.Comments)
            .HasForeignKey(c => c.NewsId);

        base.OnModelCreating(modelBuilder);
    }
    
}