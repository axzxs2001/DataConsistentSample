using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntegrationEvents;
using Dapper;
using System.Data.SqlClient;

namespace Microservice_Order.Model
{
    public class OrderRepository : IOrderRepository
    {
        string _connectionString;
        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<bool> CreateOrder(IOrder order)
        {
            using (var conn = new SqlConnection(_connectionString))
            {

                conn.Open();
                var tran = conn.BeginTransaction();
                try
                {
                    var sql1 = @"INSERT INTO [dbo].[Orders]
           ([ID]
           ,[OrderTime]
           ,[OrderUserID])
     VALUES
           (@ID
           ,@OrderTime
           ,@OrderUserID)";
                    var sql2 = @"INSERT INTO [dbo].[Events]
           ([EventType]
           ,[OrderID]
           ,[EntityJson]
           )
     VALUES
           (@EventType
           ,@OrderID
           ,@EntityJson
           )";
                    var result1 = await conn.ExecuteAsync(sql1, param: order, transaction: tran) > 0;
                    var result2 = await conn.ExecuteAsync(sql2, param: new { EventType = "CreateOrder", OrderID = order.ID, EntityJson = Newtonsoft.Json.JsonConvert.SerializeObject(order) }, transaction: tran) > 0;
                    if (result1 && result2)
                    {
                        tran.Commit();
                    }
                    else
                    {
                        tran.Rollback();
                    }
                    return result1 && result2;
                }
                catch (Exception exc)
                {
                    tran.Rollback();
                    throw exc;
                }

            }
        }
    }
}
