using CriticalEvents.Domain.Entities;



namespace CriticalEvents.Domain.Interfaces
{
    public interface IAccidentStorage
    {
        public Task StoreAccident(Accident accident);

    }
}
