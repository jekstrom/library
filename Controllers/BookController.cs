using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers
{
    public class BookController : Controller
    {
        [HttpGet("api/[controller]")]
        public IActionResult Index()
        {
            // Get all books
            return View();
        }

        [HttpGet("api/[controller]/{id}")]
        public IActionResult Index(string id)
        {
            // Get book by id
            return View();
        }

        [HttpPut("api/[controller]/{id}")]
        public IActionResult Index(string id, Book book)
        {
            //Update book
            return View();
        }

        [HttpPost("api/[controller]")]
        public IActionResult Index(Book book)
        {
            //Create new book
            return View();
        }

        [HttpDelete("api/[controller]/{id}")]
        public IActionResult Delete(string id)
        {
            //Create new book
            return View();
        }
    }
}