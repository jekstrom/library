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
                await PersistUser(new LibraryUser(name, username));

                return Json(new GithubUser(
                    User.FindFirst(c => c.Type == "urn:github:avatar")?.Value,
                    username,
                    name,
                    User.FindFirst(c => c.Type == "urn:github:url")?.Value
                ));
            }

            return Json(new { });
        }

        private async Task PersistUser(LibraryUser libraryUser)
        {
            LibraryUser newLibraryUser = await _repository.Get(libraryUser.Username) ?? await _repository.Create(libraryUser);

            // Update role claims on current user
            User.AddIdentity(new ClaimsIdentity(newLibraryUser.Roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList()));
        }
    }
}