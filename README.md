# Blazor faq cookies authentication

This sample explains that how to authenticate faq cookies for Blazor serverside.

## Prerequisites

Visual Studio 2022

## How to run this application?


CookieController.cs ����

Startup.cs
    //���� ���
    services.AddAuthentication("Cookies").AddCookie();
    services.AddHttpContextAccessor();
    
    //������ �°� �Է�
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

App.razor
    <CascadingAuthenticationState> �� ��ü ���α�

    <form id="frm_logout"> �κ� �Է�