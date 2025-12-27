using CriticalEvents.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CriticalEvents.Domain.Interfaces
{
    public interface IAccidentStorage
    {
        public Task StoreEvent(Accident @event);

    }
}
