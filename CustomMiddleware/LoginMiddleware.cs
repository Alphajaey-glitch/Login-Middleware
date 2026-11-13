using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;
using LoggingUsingMiddleware.RegularExpessions;

namespace LoggingUsingMiddleware.CustomMiddleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class LoginMiddleware
    {
        private readonly RequestDelegate _next;

        public LoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string? email = null;
            string? password = null;

            if(httpContext.Request.Method == "POST" && httpContext.Request.Path == "/")
            {
                StreamReader reader = new StreamReader(httpContext.Request.Body);
                string body = await reader.ReadToEndAsync();

                Dictionary<string, StringValues> queryDictionary = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(body);

                if (queryDictionary.ContainsKey("email"))
                {
                    email = queryDictionary["email"].ToString();
                }

                if (queryDictionary.ContainsKey("password"))
                {
                    password = queryDictionary["password"].ToString();
                }
                
                if (string.IsNullOrWhiteSpace(email))
                {
                    if (httpContext.Response.StatusCode == 200)
                    {
                        httpContext.Response.StatusCode = 400;
                    }
                    await httpContext.Response.WriteAsync("Invalid input for email\n");
                    return;
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    if (httpContext.Response.StatusCode == 200)
                    {
                        httpContext.Response.StatusCode = 400;
                    }
                    await httpContext.Response.WriteAsync("Invalid input for password\n");
                    return;
                }
              
                if(EmailRegex.IsMatchingEmail(email) && PasswordRegex.IsMatchingPassword(password))
                {
                    await httpContext.Response.WriteAsync("Successful login");
                    return;
                }
                else
                {
                    httpContext.Response.StatusCode = 400;
                    await httpContext.Response.WriteAsync("Invalid login\n");
                    return;
                }
                
            }
            if(httpContext.Request.Method == "GET" && httpContext.Request.Path == "/")
            {
                httpContext.Response.StatusCode = 200;
                
            }
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseLoginMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoginMiddleware>();
        }
    }
}
