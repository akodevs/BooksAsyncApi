using Books.Api.ExternalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books.Api.Services
{
    public interface IBooksRepository
    {

        //IEnumerable<Entities.Book> GetBooks();

        //Entities.Book GetBook(Guid id);

        Task<IEnumerable<Entities.Book>> GetBooksAsync();

        Task<IEnumerable<Entities.Book>> GetBooksAsync(IEnumerable<Guid> bookIds);
        
        // do book cover make sense without book?
        // We are not adding another repo for book cover since our repo is persistent, ignorant
        // and can call to multiple data sources
        Task<BookCover> GetBookCoverAsync(string coverId);

        Task<Entities.Book> GetBookAsync(Guid id);

        void AddBook(Entities.Book bookToAdd);

        Task<bool> SaveChangesAsync();
    }
}
