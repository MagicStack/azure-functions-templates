using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace Company.Function
{
    public static class DurableFunctionsOrchestrationCSharp
    {
        [FunctionName("DurableFunctionsOrchestrationCSharp")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<string>("DurableFunctionsOrchestrationCSharp_Hello", "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>("DurableFunctionsOrchestrationCSharp_Hello", "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>("DurableFunctionsOrchestrationCSharp_Hello", "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName("DurableFunctionsOrchestrationCSharp_Hello")]
        public static string SayHello([ActivityTrigger] string name, TraceWriter log)
        {
            log.Info($"Saying hello to {name}.");
            return $"Hello {name}!";
        }

        [FunctionName("DurableFunctionsOrchestrationCSharp_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            TraceWriter log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("DurableFunctionsOrchestrationCSharp", null);

            log.Info($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}