using Microsoft.JSInterop;

using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCookieAuthentication
{
    public class ClientService
    {
        IJSRuntime js;
        public ClientService(IJSRuntime js)
        {
            this.js = js;
        }

        public async Task login(string id, string pass, bool maintain_login = false)
        {
            var key = AppDomain.CurrentDomain.BaseDirectory.GetType().GUID.ToString("n");
            var pass_sha = CryptoModule.GetSHA256Hash(pass);

            //로그인 실패시
            bool login_success = true;
            if (!login_success)
            {
                throw new Exception("login failed");
            }


            //세션 생성
            var obj = new session_vm()
            {
                session_id = Guid.NewGuid().ToString("N"),
                maintain_login = maintain_login,
            };
            var txt = System.Text.Json.JsonSerializer.Serialize(obj, options: new System.Text.Json.JsonSerializerOptions()
            {
                WriteIndented = false,
            });
            var txt_encrypted = CryptoModule.aes256_encrypt(txt);
            //var txt_decrypted = CryptoModule.aes256_decrypt(txt_encrypted);
            var form = await js.InvokeAsync<IJSObjectReference>("document.getElementById", "frm_login");

            await js.InvokeVoidAsync("setValue", "frm_login_sessionid", txt_encrypted);

            await form.InvokeVoidAsync("submit");
            await form.DisposeAsync();
        }

        public async Task logout()
        {
            var form = await js.InvokeAsync<IJSObjectReference>("document.getElementById", "frm_logout");
            await form.InvokeVoidAsync("submit");
            await form.DisposeAsync();
        }
    }
}
