namespace WebApiPatterns.Application.Dtos
{
    public record CriticalEventRequest(Guid id, string Description, CriticalEventType Type);
    

    public record CriticalEvent(Guid id, string Description, CriticalEventType Type, DateTime Date);
    
    public enum CriticalEventType
    {
        type1,
        type2,
        type3
    }
   
}
