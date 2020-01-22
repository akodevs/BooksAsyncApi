using Books.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books.Api.Contexts
{
    public class BooksContext : DbContext
    {
        public DbSet<Book> Books { get; set; }

        //constructor
        public BooksContext(DbContextOptions<BooksContext> options) : base(options)
        {

        }

        // to seed a db it's efficied to overwrite a method and call into HasData on the entitiy
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().HasData(
                new Author()
                {
                    Id = Guid.Parse("4b8d85c8-8a55-44e0-92f9-75b955032e7f"),
                    FirstName = "George",
                    LastName = "RR Martin"
                },
                new Author()
                {
                    Id = Guid.Parse("f660471c-b932-4afb-bbe1-0400e38dc1a6"),
                    FirstName = "Stepehen",
                    LastName = "Fry"
                }
            ); 
            
            modelBuilder.Entity<Book>().HasData(
                 new Book()
                 {
                     Id = Guid.Parse("c060810d-2625-42e9-b458-757c83cecc1a"),
                     AuthorId = Guid.Parse("f660471c-b932-4afb-bbe1-0400e38dc1a6"),
                     Title = "The Winds of Winter",
                     Description = "This is the book!"
                 },
                 new Book()
                 {
                     Id = Guid.Parse("8d342cc5-1995-4386-8408-08dbc5e81bcb"),
                     AuthorId = Guid.Parse("4b8d85c8-8a55-44e0-92f9-75b955032e7f"),
                     Title = "A Game of Possible",
                     Description = "This is the best book ever!"
                 }
             );

            base.OnModelCreating(modelBuilder);
        }
    }
}
