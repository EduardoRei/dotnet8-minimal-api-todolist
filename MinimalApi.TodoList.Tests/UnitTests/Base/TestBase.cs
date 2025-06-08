using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MinimalApi.TodoList.Tests.UnitTests.Base
{
    public static class TestBase
    {
        public static DefaultHttpContext GenerateAuthHttpContext(string userId = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var user = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = user };
            return context;
        }
    }
}