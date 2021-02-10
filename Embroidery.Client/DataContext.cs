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
            => options.UseSqlite(@"Data Source=C:\Users\Corey\source\repos\Embroidery\Embroidery.Client\Embroidery.db");

        public DbSet<File> Files { get; set; }

        public DbSet<Tag> Tags { get; set; }
    }
}
