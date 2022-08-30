using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace FunctionAppDemo
{
    public static class SayHello
    {
        [FunctionName("SayHello")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            var serverTime = DateTime.Now.ToString();

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully at {serverTime}.";

            var nbRows = QueryDatabase();
            responseMessage += $"Connecting to database and then perform an update query... Updated rows = {nbRows}";

            return new OkObjectResult(responseMessage);
        }

        private static int QueryDatabase()
        {
            var connectionString = Environment.GetEnvironmentVariable("DefaultDb");
            const string sql = "UPDATE [Item] SET [UpdatedAt] = GETDATE()";
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                return connection.Execute(sql);
            }
        }
    }
}
