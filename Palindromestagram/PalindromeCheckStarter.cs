using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Palindromestagram
{
    public static class PalindromeCheckStarter
    {
        [FunctionName("PalindromeCheckStarter")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Start")] HttpRequestMessage req, [DurableClient] IDurableClient durableClient ,ILogger log)
        {

            object eventData = await req.Content.ReadAsAsync<List<string>>();

            // We are starting the orchestration and storing the instance ID in a variable for later use.
            // We are also passing in the list of palidromes submitted by the user
            var instanceId = await durableClient.StartNewAsync("PalindromeCheckOrchestrator", Guid.NewGuid().ToString(), eventData);

            //We are logging the start of our orchestration with its instance Id just in case we need to reference this orch to perform actions later. 
            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");


            //We are creating a response for the use that contains information that would allow the user to interact with the orchestration while its running
            //Examples include query the status, raise events, terminate the orchestration. 
            //This is helpful if we wanted to implement a feature that allowed a user to cancel their upload. 
            return durableClient.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
