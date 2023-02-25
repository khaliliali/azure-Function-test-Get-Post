using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

namespace sqlFuncApp
{
    public static class AddProduct
    {
        [FunctionName("AddProduct")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string reqBody = await new StreamReader(req.Body).ReadToEndAsync();
            Product product = JsonConvert.DeserializeObject<Product>(reqBody);

            SqlConnection _connection = GetConnection();

            _connection.Open();

            string statement = "INSERT INTO Products(ProductID, ProductName, Quantity) VALUES (@param1, @param2,@param3)";

            using (SqlCommand cmd = new SqlCommand(statement, _connection))
            {
                cmd.Parameters.Add("@param1", System.Data.SqlDbType.Int).Value = product.ProductID;
                cmd.Parameters.Add("@param2", System.Data.SqlDbType.VarChar,1000).Value = product.ProductName;
                cmd.Parameters.Add("@param3", System.Data.SqlDbType.Int).Value = product.Quantity;

                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            return new OkObjectResult("Product Successfully Added! ");
        }

        private static SqlConnection GetConnection()
        {
            string connectionString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_SQLConnetionString");
            return new SqlConnection(connectionString);
        }
    }

}

