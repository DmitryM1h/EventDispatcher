using System.Collections.Concurrent;
using System.ComponentModel;
using WebApiPatterns.Application.Dtos;

namespace WebApiPatterns.Application
{
    public class CriticalEventHandler
    {
        ILogger<CriticalEventHandler> _logger;
        public CriticalEventHandler(ILogger<CriticalEventHandler> logger)
        {
            _logger = logger;

            typesHandlers[CriticalEventType.type1] = CreateIncidentOne;
            typesHandlers[CriticalEventType.type2] = CreateIncidentTwo;
            typesHandlers[CriticalEventType.type3] = CreateIncidentThree;


        }

        private static readonly ConcurrentDictionary<CriticalEventType, Action<CriticalEvent>> typesHandlers = new();

        public void ProcessCriticalEvent(CriticalEventRequest request)
        {
            CriticalEvent criticalEvent = new(request.id, request.Description, request.Type, DateTime.Now);
            _logger.LogInformation("В обработку получено критическое событие, id = " + criticalEvent.id.ToString() + " типа " + criticalEvent.Type);

            Action<CriticalEvent> handler = typesHandlers[criticalEvent.Type];

            handler(criticalEvent);
        }

        public void CreateIncidentOne(CriticalEvent criticalEvent)
        {
            var accident = new Accident(Guid.NewGuid(), AccidentType.Type1, criticalEvent);
            _logger.LogInformation("Создан инцидент типа 1 на основе события с id " + criticalEvent.id.ToString());

        }
        public void CreateIncidentTwo(CriticalEvent criticalEvent)
        {

            var sourceEventDate = DateTime.Now;
            int secondsToWait = 20;

            void LocalHandler(CriticalEvent ce)
            {
                if (DateTime.Now - sourceEventDate < TimeSpan.FromSeconds(secondsToWait))
                {
                    _logger.LogInformation("20 секунд еще не прошло, создаем!!");
                    var accident = new Accident(Guid.NewGuid(), AccidentType.Type2, criticalEvent, ce);
                    _logger.LogInformation("Создан инцидент типа 2 на основе событий с id = " + accident.CriticalEventFirst.id.ToString() + " " + accident.CriticalEventSecond!.id.ToString());
                }

                typesHandlers[CriticalEventType.type1] -= LocalHandler;
                typesHandlers[CriticalEventType.type1] += CreateIncidentOne;
            }

            typesHandlers[CriticalEventType.type1] += LocalHandler;
            typesHandlers[CriticalEventType.type1] -= CreateIncidentOne;


        }

        public void CreateIncidentThree(CriticalEvent criticalEvent)
        {

            var sourceEventDate = DateTime.Now;

            int secondsToWait = 30;

            void LocalHandler(CriticalEvent ce)
            {
                if (DateTime.Now - sourceEventDate < TimeSpan.FromSeconds(secondsToWait))
                {
                    _logger.LogInformation("30 секунд еще не прошло, создаем!!");
                    var accident = new Accident(Guid.NewGuid(), AccidentType.Type3, criticalEvent, ce);
                    _logger.LogInformation("Создан инцидент типа 3 на основе событий с id = " + accident.CriticalEventFirst.id.ToString() + " " + accident.CriticalEventSecond!.id.ToString());
                }

                typesHandlers[CriticalEventType.type2] -= LocalHandler;
                typesHandlers[CriticalEventType.type2] += CreateIncidentTwo;
            }

            typesHandlers[CriticalEventType.type2] += LocalHandler;
            typesHandlers[CriticalEventType.type2] -= CreateIncidentTwo;
        }



    }
}