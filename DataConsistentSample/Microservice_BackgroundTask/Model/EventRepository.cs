using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using Microservice_BackgroundTask.Model;
using IntegrationEvents;

namespace Microservice_BackgroundTask.Model
{
    public class EventRepository : IEventRepository
    {
        string _connectionString;
        public EventRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public async Task<IEnumerable<OrderEventEntity>> GetEvent()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT [ID]
      ,[EventType]
      ,[OrderID]
      ,[CreateTime]
      ,[ShipStatus]
      ,[StorageStatus]
      ,[EntityJson]
  FROM [dbo].[Events]
  WHERE EventType=@EventType and (StorageStatus=1 or ShipStatus=1)";
                var result = await conn.QueryAsync<OrderEventEntity>(sql, param: new { EventType = "CreateOrder" });
                return result;
            }
        }

        public async Task<bool> UpdateOrderEnvent(IOrderEventEntity orderEvent)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var sql = $@"UPDATE [dbo].[Events]
   SET { (orderEvent.ShipStatus != 0 ? "[ShipStatus]=" + orderEvent.ShipStatus + "," : "")}{(orderEvent.StorageStatus != 0 ? "[StorageStatus]=" + orderEvent.StorageStatus + "," : "")  }";

                sql = sql.TrimEnd(',') + " WHERE [OrderID]=@OrderID and EventType=@EventType";

                Console.WriteLine("----------------------------");
                Console.WriteLine(sql);
                Console.WriteLine("----------------------------");
                var result = await conn.ExecuteAsync(sql, param: new { orderEvent.OrderID, orderEvent.EventType }) > 0;
                return result;
            }
        }
    }
}
