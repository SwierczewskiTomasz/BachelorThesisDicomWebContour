using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DataAccess
{
    public class ContourContext : DbContext
    {
        public DbSet<ContourEntity> Contours { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=blogging.db");
        }
    }

    public class ContourEntity
    {
        public Guid ContourId { get; set; }
        public string DicomId { get; set; }
        public string Tag { get; set; }
        public bool IsManual { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }

    public class User
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<ContourEntity> ContourEntities { get; set; }
    }
}