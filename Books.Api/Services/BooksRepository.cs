using Books.Api.Contexts;
using Books.Api.Entities;
using Books.Api.ExternalModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Books.Api.Services
{
    // it is important to add Idispoable in the repository, so that when I disposable is disposed so as our repositry,
    // we don't want to hold on to that and cause potentially cause leaks
    public class BooksRepository : IBooksRepository, IDisposable
    {
        private BooksContext _context;
        private IHttpClientFactory _httpClientFactory;

        public BooksRepository(BooksContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpClientFactory = httpClientFactory ??
                throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<Book> GetBookAsync(Guid id)
        {
            return await _context.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            return await _context.Books
                .Include(b => b.Author)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksAsync(IEnumerable<Guid> bookIds)
        {
            return await _context.Books 
                .Where(b => bookIds.Contains(b.Id))
                .Include(b => b.Author)
                .ToListAsync();
        }

        // calling from another API
        public async Task<BookCover> GetBookCoverAsync(string coverId)
        {
            var httpClient = _httpClientFactory.CreateClient();

            // pass through a dummy name
            var response = await httpClient
                   .GetAsync($"http://localhost:52644/api/bookcovers/{coverId}");

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<BookCover>(
                    await response.Content.ReadAsStringAsync());
            }

            return null; 
        }

        public void AddBook(Book bookToAdd)
        {
            if(bookToAdd == null)
            {
                throw new ArgumentNullException(nameof(bookToAdd));
            }

             _context.Add(bookToAdd);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(_context != null)
                    {
                        _context.Dispose();
                        _context = null;
                    }
                } 
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true); 

            // Just telling the garbage collector that this repository has already been cleaned up
            GC.SuppressFinalize(this);
        } 
        #endregion
    }
}
