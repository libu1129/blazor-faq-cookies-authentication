using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorCookieAuthentication
{
    [Route("/[controller]")]
    [ApiController]
    public class CookieController : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromForm] string session_id)
        {
            var txt = CryptoModule.aes256_decrypt(session_id);
            var session = System.Text.Json.JsonSerializer.Deserialize<session_vm>(txt);

            //세션 유효성 체크(ex:database)
            if (string.IsNullOrWhiteSpace(session_id))
            {
                //유효하지 않은 세션이면 자동 사인아웃
                //session_id 를 jwt 방식으로 만들어서 최초 생성 ip 기준 체크를 하는 등 보안 강화 가능
                try { await HttpContext.SignOutAsync(); } catch (Exception ex) { }
                return Redirect("/");
            }

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, session_id),
                new Claim(ClaimTypes.Name, session_id),
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

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }

    public class session_vm
    {
        public string session_id { get; set; }
        public bool maintain_login { get; set; }

    }
}