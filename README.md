# Blazor faq cookies authentication

This sample explains that how to authenticate faq cookies for Blazor serverside.

## Prerequisites

Visual Studio 2022

## How to run this application?


CookieController.cs 구현

_Host.cshtml
    자바스트립트 넣기
    
App.razor
    <CascadingAuthenticationState> 로 전체 감싸기
    <AuthorizeView> 부분 입력

Startup.cs
    //서비스 등록
    services.AddAuthentication("Cookies").AddCookie();
    services.AddHttpContextAccessor();
    
    //순서에 맞게 입력
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseAuthentication();

    app.UseRouting();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapBlazorHub();
        endpoints.MapFallbackToPage("/_Host");
    });
