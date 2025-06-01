using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.TodoList.Models;

namespace MinimalApi.TodoList.Endpoints
{
    public static class IdentityEndpoints
    {
        public static void MapIdentityEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/auth").WithTags("Auth");

            group.MapIdentityApi<User>(); // todos os endpoints de registro/login/etc.

            group.MapPost("/logout", async (
                SignInManager<User> signInManager,
                [FromBody] object _) =>
            {
                await signInManager.SignOutAsync();
                return Results.Ok();
            });
        }
    }
}
