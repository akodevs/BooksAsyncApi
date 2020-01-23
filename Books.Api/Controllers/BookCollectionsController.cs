using AutoMapper;
using Books.Api.Filters;
using Books.Api.Models;
using Books.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books.Api.Controllers
{
    [Route("api/bookcollections")]
    [ApiController]
    [BooksResultFilterAtrribute]
    public class BookCollectionsController : ControllerBase
    {

        private IBooksRepository _booksRepository;
        private readonly IMapper _mapper;

        public BookCollectionsController(IBooksRepository booksRepository, IMapper mapper)
        { 
            _booksRepository = booksRepository ?? throw new ArgumentNullException(nameof(booksRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // api/bookcollections/(id1,id2)
        // booksIds is an array of guid, there's no implicit binding so we need custom model binder
        [HttpGet("({bookIds})", Name = "GetBookCollection" )]
        public async Task<IActionResult> GetBookCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> bookIds)
        {
            var bookEntities = await _booksRepository.GetBooksAsync(bookIds);

            if (bookIds.Count() != bookEntities.Count())
            {
                return NotFound();
            }

            return Ok(bookEntities);
        }

        [HttpPost]
        public  async Task<IActionResult> CreateBookCollection([FromBody] IEnumerable<BookForCreation> bookCollection)
        {
            // Map from entities
            var bookEntities = _mapper.Map<IEnumerable<Entities.Book>>(bookCollection);

            // loop and save
            foreach (var bookEntity in bookEntities)
            {
                _booksRepository.AddBook(bookEntity);
            };

            // save changes
            await _booksRepository.SaveChangesAsync();

            // form and return one collection
            var booksToReturn = await _booksRepository.GetBooksAsync(
                bookEntities.Select(b => b.Id)
                .ToList());

            var bookIds = string.Join(",", booksToReturn.Select(a => a.Id));

            return CreatedAtRoute("GetBookCollection",
                new { bookIds },
                booksToReturn);
        }
    }
}
