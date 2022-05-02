using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace SignalRConnectionSupport
{
    public static class SignalR
	{
		[FunctionName("negotiate")]
		public static IActionResult Negotiate(
			[HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
			[SignalRConnectionInfo(HubName = "spinozahub")] SignalRConnectionInfo connectionInfo, ILogger log)

		{
			try
			{
				log.LogInformation("the Negotiate beggins");
				if (!req.HttpContext.Response.Headers.ContainsKey("Access-Control-Allow-Credentials"))
				{
					req.HttpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
				}
				if (req.Headers.ContainsKey("Origin") && !req.HttpContext.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
				{
					req.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", req.Headers["Origin"][0]);
				}
				if (req.Headers.ContainsKey("Access-Control-Request-Headers"))
				{
					req.HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", req.Headers["access-control-request-headers"][0]);
				}
				log.LogInformation("negotiate API succeeded.");
				if (connectionInfo == null)
				{
					log.LogError("No cooncetion info");
					return new NotFoundObjectResult("Azure SignalR not found.");
				}
				return new OkObjectResult(connectionInfo);
			}
			catch
			{
				return new BadRequestResult();
			}
		}
	}
}
