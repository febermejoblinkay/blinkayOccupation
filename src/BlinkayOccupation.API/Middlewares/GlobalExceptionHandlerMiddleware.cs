using BlinkayOccupation.Application.Exceptions;
using System.Net;

namespace BlinkayOccupation.API.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ParkingEventsNotFoundException ex)
            {
                _logger.LogWarning(ex, "Parking events not found.");
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.NotFound);
            }
            catch (ParkingRightsNotFoundException ex)
            {
                _logger.LogWarning(ex, "Parking rights not found.");
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.NotFound);
            }
            catch (InstallationNotFoundException ex)
            {
                _logger.LogWarning(ex, "Installations not found.");
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.NotFound);
            }
            catch (ZoneNotFoundException ex)
            {
                _logger.LogWarning(ex, "Zone not found.");
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.NotFound);
            }
            catch (StayNotFoundException ex)
            {
                _logger.LogWarning(ex, "Stay not found.");
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.NotFound);
            }
            catch (ParkingRightsExistsInStayException ex)
            {
                _logger.LogWarning(ex, "Parking Right Ids already exist in current or other stay.");
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.BadRequest);
            }
            catch (ParkingRightsNoValidEndDateException ex)
            {
                _logger.LogWarning(ex, "The Parking right has no validTo date defined.");
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error, something unexpected happend.");
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.InternalServerError);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                code = exception.GetType().Name,
                message = exception.Message
            };

            return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
        }
    }
}
