using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Repositories;
using library.PostDTOs;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers
{
    public class BookController : Controller
    {
        private readonly IRepository<Book> _repository;
        public BookController(IRepository<Book> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet("api/[controller]")]
        public async Task<JsonResult> Index()
        {
            IReadOnlyCollection<Book> books = await _repository.Get();
            if (books.Any())
            {
                return Json(books);
            }
            HttpContext.Response.StatusCode = 404;
            return Json(new Book[] { });
        }

        [HttpGet("api/[controller]/{id}")]
        public async Task<JsonResult> Index(int id)
        {
            Book book = await _repository.Get(id);
            if (book is object)
            {
                return Json(book);
            }
            HttpContext.Response.StatusCode = 404;
            return Json(null);
        }

        [HttpPut("api/[controller]/{id}")]
        public async Task<JsonResult> Index(int id, [FromBody]BookDTO bookDto)
        {
            var book = new Book(0, bookDto.Title, bookDto.Author, bookDto.ISBN, bookDto.Description);
            Book updatedBook = await _repository.Update(id, book);
            if (book is object)
            {
                return Json(updatedBook);
            }
            HttpContext.Response.StatusCode = 404;
            return Json(null);
        }

        [HttpPost("api/[controller]")]
        public async Task<JsonResult> Index([FromBody]BookDTO bookDto)
        {
            var book = new Book(0, bookDto.Title, bookDto.Author, bookDto.ISBN, bookDto.Description);
            book.SetCreatedBy(User?.Identity?.Name ?? "Anonymous");
            
            Book newBook = await _repository.Create(book);
            HttpContext.Response.StatusCode = 201;
            return Json(newBook);
        }

        [HttpDelete("api/[controller]/{id}")]
        public async Task<JsonResult> Delete(int id)
        {
            bool deleted = await  _repository.Delete(id);
            return Json(new { deleted });
        }
    }
}