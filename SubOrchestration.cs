using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace AdamCoulterOz;

public class SubOrchestration
{
	[FunctionName($"{nameof(SubOrchestration)}_{nameof(Run)}")]
	public async Task<List<string>> Run(
		[OrchestrationTrigger] IDurableOrchestrationContext context)
	{
		async Task<string> SayHelloAsync(string city)
			=> await context.CallActivityAsync<string>($"{nameof(SubOrchestration)}_{nameof(SayHello)}", city);
		try
		{
			await SayHelloAsync("Toyko");
			await SayHelloAsync("Seattle");
			await SayHelloAsync("London");
			throw new Exception("Boom!");
		}
		catch (Exception)
		{
			await Notifier.Result(success: false, context);
			throw;
		}
		await Notifier.Result(success: true, context);
	}

	[FunctionName($"{nameof(SubOrchestration)}_{nameof(SayHello)}")]
	public void SayHello([ActivityTrigger] string name, ILogger log)
		=> log.LogInformation($"Saying hello to {name}.");

	[FunctionName($"{nameof(SubOrchestration)}_{nameof(Start)}")]
	public async Task<HttpResponseMessage> Start([HttpTrigger("get")] HttpRequestMessage req, [DurableClient] IDurableOrchestrationClient starter, ILogger log)
		=> starter.CreateCheckStatusResponse(req, await starter.StartNewAsync($"{nameof(SubOrchestration)}_{nameof(Run)}"));
}
