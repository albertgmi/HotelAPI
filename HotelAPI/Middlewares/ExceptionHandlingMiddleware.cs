using HotelAPI.Exceptions;

namespace HotelAPI.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (NotFoundException notFoundException)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(notFoundException.Message);
            }
            catch (NotInRangeException notInRangeException)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(notInRangeException.Message);
            }
            catch (BadDateException badDateException)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(badDateException.Message);
            }
            catch (RoomNotAvailableException roomNotAvailableException)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(roomNotAvailableException.Message);
            }
            catch (BadRequestException badRequestException)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(badRequestException.Message);
            }
            catch (ForbidException forbidException)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync(forbidException.Message);
            }
            catch (Exception)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Something went wrong");
            }
        }
    }
}
