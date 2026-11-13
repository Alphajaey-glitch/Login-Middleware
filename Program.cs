using LoggingUsingMiddleware.CustomMiddleware;

namespace LoggingUsingMiddleware
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.UseLoginMiddleware();

            app.Run(async httpContext =>
            {

                await httpContext.Response.WriteAsync("No response");

            });

            app.Run();
        }
    }
}
