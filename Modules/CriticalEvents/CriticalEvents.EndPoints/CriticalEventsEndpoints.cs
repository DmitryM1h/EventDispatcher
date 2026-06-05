using CriticalEvents.Application;
using CriticalEvents.Domain.Services.Requests;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CriticalEvents.EndPoints;

public static class CriticalEventsEndpoints
{
    public static void AddCriticalEventsEndPoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("CriticalEvent", async ([FromBody] CriticalEventRequest criticalEvent, [FromServices] CriticalEventHandler eventsHandler) =>
        {
            await eventsHandler.Handle(criticalEvent);

            return Results.Accepted();
        })
        .Produces<string>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)      // Если пользователь не найден
        .WithTags("Notifications", "SSE")             // Теги для Swagger
        .WithName("GetPatientNotifications")          // Имя эндпоинта
        .WithDescription("Stream real-time notifications for a patient");
    }
}
