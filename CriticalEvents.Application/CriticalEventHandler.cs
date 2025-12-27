using CriticalEvents.Domain.Entities;
using CriticalEvents.Domain.Services;
using CriticalEvents.Domain.Services.Requests;


namespace CriticalEvents.Application
{
    public class CriticalEventHandler(CrititicalEventsProcessor _eventProcessor)
    {
        public async Task Handle(CriticalEventRequest processEventRequest)
        { 
            var criticalEvent = CriticalEvent.Create(processEventRequest);

            _eventProcessor.ProcessCriticalEvent(criticalEvent);

        }
        
    }
}