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
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private IBooksRepository _booksRepository;
        private readonly IMapper _mapper;

        public BooksController(IBooksRepository booksRepository, IMapper mapper)
        {
            _booksRepository = booksRepository ?? throw new ArgumentNullException(nameof(booksRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [BooksResultFilterAtrribute]
        public async Task<IActionResult> GetBooks()
        {
            var bookEntities = await _booksRepository.GetBooksAsync();
            return Ok(bookEntities);
        }


        [HttpGet]
        //[BookResultFilter
        [BookWithCoversResultFilter]
        [Route("{id}", Name = "GetBook")]
        public async Task<IActionResult> GetBook(Guid id)
        {
            var bookEntity = await _booksRepository.GetBookAsync(id);
            if (bookEntity == null)
            {
                return NotFound();
            }

            // Get book cover
            // var bookCover = await _booksRepository.GetBookCoverAsync("dummyCover");
            // Get book covers

            var bookCovers = await _booksRepository.GetBookCoversAsync(id);


            //(Entities.Book book, IEnumerable<ExternalModels.BookCover> bookCovers) propertyBag =
            //    (bookEntity, bookCovers);

            return Ok((bookEntity, bookCovers));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] BookForCreation book)
        {
            var bookEntity = _mapper.Map<Entities.Book>(book);
            _booksRepository.AddBook(bookEntity);

            // you have to add validaiton here
            await _booksRepository.SaveChangesAsync();

            return CreatedAtRoute("GetBook", 
                new  { id = bookEntity.Id },
                bookEntity);
        }
    }
}
