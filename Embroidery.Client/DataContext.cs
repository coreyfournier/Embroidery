﻿using Embroidery.Client.Models;
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
            string dataSource = "";
            //This is the actual application
            if (Program.IsApplicationExecuting)
            {
                var absolute = System.IO.Path.GetFullPath(Environment.CurrentDirectory + @"\..\..\..\Embroidery.db");
                dataSource = $"Data Source={absolute}";
            }
            else //This is entity frame work migrations
            {
                dataSource = @"Data Source=Embroidery.db";
            }

            options.UseSqlite(dataSource);

            System.Console.WriteLine($"Using DS:{dataSource}");
        }

        public DbSet<File> Files { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Folder> Folders { get; set; }
    }
}
