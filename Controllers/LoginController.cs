using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers
{
    public class LoginController : Controller
    {
        private readonly IRepository<LibraryUser> _repository;

        public LoginController(IRepository<LibraryUser> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            return Challenge(new AuthenticationProperties() { RedirectUri = returnUrl });
        }

        [HttpGet("api/[controller]/[action]")]
        public async Task<JsonResult> GetGithubUserData()
        {
            if (User.Identity.IsAuthenticated)
            {
                string name = User.FindFirst(c => c.Type == ClaimTypes.Name)?.Value;
                string username = User.FindFirst(c => c.Type == "urn:github:login")?.Value;
                var user = await PersistUser(new LibraryUser(0, name, username, new Book[] { }));

                return Json(user);
            }

            return Json(new { });
        }

        private async Task<GithubUser> PersistUser(LibraryUser libraryUser)
        {
            LibraryUser newLibraryUser = await _repository.Get(libraryUser.Username) ?? await _repository.Create(libraryUser);

            // Update role claims on current user
            User.AddIdentity(new ClaimsIdentity(newLibraryUser.Roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList()));

            return new GithubUser(
                User.FindFirst(c => c.Type == "urn:github:avatar")?.Value,
                libraryUser.Username,
                libraryUser.Name,
                User.FindFirst(c => c.Type == "urn:github:url")?.Value,
                canEdit: UserCanEdit(),
                canDelete: UserCanDelete(),
                canCheckOut: UserCanCheckOut(),
                newLibraryUser.BooksCheckedOut
            );
        }

        private bool UserCanEdit()
        {
            return User.Claims.Any(c => c.Type == ClaimTypes.Role && (c.Value == "superadmin" || c.Value == "bookwriter"));
        }

        private bool UserCanDelete ()
        {
            return User.Claims.Any(c => c.Type == ClaimTypes.Role && (c.Value == "superadmin" || c.Value == "bookwriter"));
        }

        private bool UserCanCheckOut()
        {
            return User.Claims.Any(c => c.Type == ClaimTypes.Role && (c.Value == "superadmin" || c.Value == "bookwriter" || c.Value == "bookchecker"));
        }
    }
}