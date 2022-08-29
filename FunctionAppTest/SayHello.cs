using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;
using System.IO;

namespace FunctionAppTest
{
    public static class SayHello
    {
        [FunctionName(nameof(SayHello))]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            var serverTime = DateTime.Now.ToString();

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}. Server time is {serverTime}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
