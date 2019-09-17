using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Repositories;
using library.PostDTOs;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers
{
    // TODO: Authorization attribute filter
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
            if (UserIsAuthorized("bookreader"))
            {
                IReadOnlyCollection<Book> books = await _repository.Get();
                if (books.Any())
                {
                    return Json(books);
                }
                HttpContext.Response.StatusCode = 404;
                return Json(new Book[] { });
            }
            HttpContext.Response.StatusCode = 403;
            return Json(null);
        }

        [HttpGet("api/[controller]/{id}")]
        public async Task<JsonResult> Index(int id)
        {
            if (UserIsAuthorized("bookreader"))
            {
                Book book = await _repository.Get(id);
                if (book is object)
                {
                    return Json(book);
                }
                HttpContext.Response.StatusCode = 404;
                return Json(null);
            }
            HttpContext.Response.StatusCode = 403;
            return Json(null);
        }

        [HttpPut("api/[controller]/{id}")]
        public async Task<JsonResult> Index(int id, [FromBody]BookDTO bookDto)
        {
            if (UserIsAuthorized("bookwriter"))
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
            HttpContext.Response.StatusCode = 403;
            return Json(null);
        }

        [HttpPost("api/[controller]")]
        public async Task<JsonResult> Index([FromBody]BookDTO bookDto)
        {
            if (UserIsAuthorized("bookwriter"))
            {
                var book = new Book(0, bookDto.Title, bookDto.Author, bookDto.ISBN, bookDto.Description);
                book.SetCreatedBy(User?.Identity?.Name ?? "Anonymous");

                Book newBook = await _repository.Create(book);
                HttpContext.Response.StatusCode = 201;
                return Json(newBook);
            }
            HttpContext.Response.StatusCode = 403;
            return Json(null);
        }

        [HttpPost("api/[controller]/{id}/[action]")]
        public async Task<JsonResult> Checkout(int id)
        {
            if (UserIsAuthorized("bookchecker"))
            {
                // TODO: Move logic to domain layer
                var existingBook = await _repository.Get(id);
                if (!existingBook.CheckedOut)
                {
                    existingBook.CheckOut();
                }
                else
                {
                    HttpContext.Response.StatusCode = 400;
                    return Json(null);
                }
                Book checkedOutBook = await _repository.Update(id, existingBook);
                return Json(checkedOutBook);
            }
            HttpContext.Response.StatusCode = 403;
            return Json(null);
        }

        [HttpDelete("api/[controller]/{id}")]
        public async Task<JsonResult> Delete(int id)
        {
            if (UserIsAuthorized("bookwriter"))
            {
                bool deleted = await _repository.Delete(id);
                return Json(new { deleted });
            }
            HttpContext.Response.StatusCode = 403;
            return Json(null);
        }

        private bool UserIsAuthorized(string action)
        {
            return User.Claims.Any(c => c.Type == ClaimTypes.Role && (c.Value == "superadmin" || c.Value == action));
        }
    }
}