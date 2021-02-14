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
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {

            //This is the actual application
            if (System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.ToLower() == $"{nameof(Embroidery)}.{nameof(Client)}".ToLower())
            {
                var absolute = System.IO.Path.GetFullPath(Environment.CurrentDirectory + @"\..\..\..\Embroidery.db");
                options.UseSqlite($"Data Source={absolute}");
            }
            else //This is entity frame work migrations
                options.UseSqlite(@"Data Source=Embroidery.db");

        }

        public DbSet<File> Files { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Folder> Folders { get; set; }
    }
}
