using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using Microservice_BackgroundTask.Model;

namespace Microservice_Order.Model
{
    public class EventRepository : IEventRepository
    {
        string _connectionString;
        public EventRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
         

        public async Task<IEnumerable<EventEntity>> GetEvent()
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
  WHERE EventType=@EventType";
                var result = await conn.QueryAsync<EventEntity>(sql, param: new {EventType = "CreateOrder" });
                return result;
            }
        }
    }
}
