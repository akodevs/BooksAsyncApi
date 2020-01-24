using AutoMapper;
using Books.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Books.Api.Filters
{
    public class BookWithCoversResultFilterAttribute : ResultFilterAttribute
    {
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var resultFromAction = context.Result as ObjectResult;
            if (resultFromAction?.Value == null
                || resultFromAction.StatusCode < 200
                || resultFromAction.StatusCode >= 300)
            {
                await next();
                return;
            }
             ;

            // deconstruct the name porperty from the value tuple and access them as local variables
            var (book, bookCovers) = ((Entities.Book,
                IEnumerable<ExternalModels.BookCover>))resultFromAction.Value;

            //var temp = ((Entities.Book, IEnumerable<ExternalModels.BookCover>))resultFromAction.Value;

            //var mapper = context.HttpContext.RequestServices.GetService<IMapper>();
            //resultFromAction.Value = mapper.Map<IEnumerable<Models.Book>>(resultFromAction.Value);

            var mapper = context.HttpContext.RequestServices.GetService<IMapper>();
            var mappedBook = mapper.Map<BookWithCovers>(book); 
            resultFromAction.Value = mapper.Map(bookCovers, mappedBook);

            await next();
        }
    }
}