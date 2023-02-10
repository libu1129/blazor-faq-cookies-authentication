using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorCookieAuthentication
{
    [Route("/[controller]")]
    [ApiController]
    public class CookieController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> Login([FromForm] string name)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, name),
                new Claim(ClaimTypes.Name, name),
            }, "auth");
            ClaimsPrincipal claims = new ClaimsPrincipal(claimsIdentity);


            var authProperties = new AuthenticationProperties
            {
                //IsPersistent = keep_login, //쿠키를 브라우저를 닫아도 일정기간동안엔 유지되도록 함 (자동로그인 기능)
                //RedirectUri = this.Request.Host.Value,
                //ExpiresUtc = DateTime.UtcNow.AddDays(30), //하루마다 재로그인 제대로 작동하나 테스트
            };

            await HttpContext.SignInAsync(claims, authProperties);
            return Redirect("/");
        }
    }
}