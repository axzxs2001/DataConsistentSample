using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntegrationEvents;
using Dapper;
using System.Data.SqlClient;

namespace Microservice_Order.Model
{
    public class OrderEventRepository : IOrderEventRepository
    {
        string _connectionString;
        public OrderEventRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<bool> UpdateOrderEnvent(IOrderEventEntity orderEvent)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var sql = @"UPDATE [dbo].[Events]
   SET [ShipStatus] =@ShipStatus,[StorageStatus]=@StorageStatus    
 WHERE [OrderID]=@OrderID and EventType=@EventType";

                var result = await conn.ExecuteAsync(sql, param: new { orderEvent.OrderID, orderEvent.EventType,orderEvent.ShipStatus,orderEvent.StorageStatus }) > 0;
                return result;
            }
        }
    }
}
