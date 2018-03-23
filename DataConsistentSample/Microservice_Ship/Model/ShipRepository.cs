using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntegrationEvents;
using Dapper;
using System.Data.SqlClient;

namespace Microservice_Ship.Model
{
    public class ShipRepository : IShipRepository
    {
        string _connectionString;
        public ShipRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<bool> CreateShip(IOrder order)
        {
            using (var conn = new SqlConnection(_connectionString))
            {

                var selectSql = @"SELECT  [ShipStatus] FROM [dbo].[Events]
 WHERE [OrderID]=@OrderID and EventType=@EventType";

                var list = await conn.QueryAsync<int>(selectSql, param: new { OrderID = order.ID, EventType = "CreateOrder" });
                if (list != null && list.Count() > 0 && list.ToList()[0] == 1)
                {
                    //todo 这里可以再向运单表生成数据,用事务生成运单和更新Events表
                    Console.WriteLine($"运单表中添加数据：{Newtonsoft.Json.JsonConvert.SerializeObject(order)}");
                    Console.WriteLine($"更新Events表中运单传送状态");
                    var sql = @"UPDATE [dbo].[Events]
   SET [ShipStatus] =2      
 WHERE [OrderID]=@OrderID and EventType=@EventType";

                    var result = await conn.ExecuteAsync(sql, param: new { OrderID = order.ID, EventType = "CreateOrder" }) > 0;

                    return result;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
