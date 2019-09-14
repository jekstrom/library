using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            return Challenge(new AuthenticationProperties() { RedirectUri = returnUrl });
        }

        [HttpGet("api/[controller]/[action]")]
        public JsonResult GetGithubUserData()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Json(new GithubUser(
                    User.FindFirst(c => c.Type == "urn:github:avatar")?.Value,
                    User.FindFirst(c => c.Type == "urn:github:login")?.Value,
                    User.FindFirst(c => c.Type == ClaimTypes.Name)?.Value,
                    User.FindFirst(c => c.Type == "urn:github:url")?.Value
                ));
            }

            return Json(new { });
        }
    }
}