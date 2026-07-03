using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Application.Common.Interfaces;
using BasicJira.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BasicJira.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    // DbContextOptions, connection string ve EF Core ile ilgili ayarları içerir.
    // Bu sayede AppDbContext kendi içinde connection string tutmaz. 

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)

    {
    }

    // EF Core, bu DbSetleri tablo olarak algılar ve veritabanında karşılık gelen tabloları oluşturur.

    public DbSet<Project> Projects { get; set; }

    public DbSet<AppUser> Users { get; set; }

    public DbSet<TaskItem> TaskItems { get; set; }

    public DbSet<TaskComment> TaskComments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Prevents SQL Server multiple cascade path error.
        // A TaskComment is connected to both TaskItem and AppUser.
        // If both relationships use Cascade Delete, SQL Server sees multiple delete paths.
        modelBuilder.Entity<TaskComment>()
            .HasOne(x => x.User)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevents deleting a user from automatically deleting assigned tasks.
        // In real systems, users are usually deactivated instead of deleted.
        modelBuilder.Entity<TaskItem>()
            .HasOne(x => x.AssignedUser)
            .WithMany(x => x.AssignedTasks)
            .HasForeignKey(x => x.AssignedUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}



/*
 genel mantık şu ;

application katmanı -> IAppDbContext interface'i ile DbContext'e bağımlı değil. 
IAppDbContext interface'i, uygulama katmanının ihtiyaç duyduğu DbSetleri ve SaveChangesAsync metodunu tanımlar.
Bu sayede uygulama katmanı, veri erişim detaylarından bağımsız hale gelir. Talep eder. 

infrastructure katmanı -> AppDbContext sınıfı, IAppDbContext interface'ini uygular ve DbContext'ten türetilir.
AppDbContext, veritabanı ile ilgili tüm detayları içerir ve uygulama katmanına veri erişim hizmeti sağlar.

EF Core -> AppDbContext sınıfını kullanarak veritabanı işlemlerini gerçekleştirir. Dbsetlerden tabloları çıkarır. 
 
 */

