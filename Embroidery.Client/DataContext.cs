using Embroidery.Client.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Embroidery.Client
{
    public class DataContext : DbContext
    {
        public const string DbName = "Embroidery.db";
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string dataSource = "";

            //This is the actual application
            if (Program.IsApplicationExecuting)
            {
                var absolute = System.IO.Path.Combine(Program.UserApplicationFolder, DbName);
                dataSource = $"Data Source={absolute}";
            }
            else //This is entity frame work migrations
            {
                dataSource = @$"Data Source={DbName}";
            }

            options.UseSqlite(dataSource);

            System.Console.WriteLine($"Using DS:{dataSource}");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.View.GroupedFile>()
                .ToTable(nameof(Models.View.GroupedFile), t => t.ExcludeFromMigrations())
                .HasNoKey();

            modelBuilder.Entity<Models.View.SimpleFile>()
                .ToTable(nameof(Models.View.SimpleFile), t => t.ExcludeFromMigrations())
                .HasNoKey();
        }

        public DbSet<File> Files { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Folder> Folders { get; set; }

        public DbSet<Models.View.GroupedFile> GroupedFiles  {get;set;}

        public DbSet<Models.View.SimpleFile> SimpleFiles { get; set; }

        public DbSet<Setting> Settings { get; set; }
    }
}
