using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Shared.Middleware
{
    public class HeaderClaimsMiddleware
    {
        private readonly RequestDelegate _next;

        public HeaderClaimsMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            var claims = new List<Claim>();

            if (context.Request.Headers.TryGetValue("X-User-Id", out var id))
                claims.Add(new Claim("id", id!));

            if (context.Request.Headers.TryGetValue("X-User-Email", out var email))
                claims.Add(new Claim(ClaimTypes.Email, email!));

            if (context.Request.Headers.TryGetValue("X-User-Role", out var role))
                claims.Add(new Claim(ClaimTypes.Role, role!));

            if (claims.Any())
                context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Ocelot"));

            await _next(context);
        }
    }
}
