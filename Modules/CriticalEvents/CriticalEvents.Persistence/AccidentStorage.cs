using CriticalEvents.Domain.Entities;
using CriticalEvents.Domain.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace CriticalEvents.Persistence
{
    public class AccidentStorage : IAccidentStorage
    {
        const string connectionString = "Server=localhost;Database=MyTestBase;Trusted_Connection=True;TrustServerCertificate=True;";

        public async Task StoreAccident(Accident accident)
        {

            var criticalEvents = new List<CriticalEvent>();

            if (accident.CriticalEventFirst is not null)
                criticalEvents.Add(accident.CriticalEventFirst);

            if (accident.CriticalEventSecond is not null)
                criticalEvents.Add(accident.CriticalEventSecond);

            var eventsToInsert = criticalEvents.Select(ev => new
            {
                Id = ev.Id,
                CriticalEventType = ev.Type,
                AccidentId = accident.id,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            using SqlConnection db = new SqlConnection(connectionString);

            await db.OpenAsync();

            using var transaction = db.BeginTransaction();
            {

                await db.ExecuteAsync(@"INSERT INTO Accidents (Id, AccidentType) VALUES
                (@id, @type)", new { @id = accident!.id, @type = accident.Type }, transaction);

                await db.ExecuteAsync(
                               @"INSERT INTO CriticalEvents (Id, CriticalEventType, AccidentId, CreatedAt) 
                              VALUES (@Id, @CriticalEventType, @AccidentId, @CreatedAt)",
                               eventsToInsert,
                               transaction: transaction);
            }
            await transaction.CommitAsync();            
        }
    }
}
