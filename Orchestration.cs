using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace AdamCoulterOz;

public class Orchestration
{
	[FunctionName($"{nameof(Orchestration)}_{nameof(Run)}")]
	public async Task Run(
		[OrchestrationTrigger] IDurableOrchestrationContext context)
	{
		var count = 5;
		try
		{
			var tasks = new List<Task>();
			var exceptions = new List<Exception>();
			for (var i = 0; i < count; i++)
			{
				var instanceId = $"sayhello-{i}";
				tasks.Add(context.CallSubOrchestratorAsync($"{nameof(SubOrchestration)}_{nameof(SubOrchestration.Run)}", instanceId, null));

				// do i want to batch them? then await batches?
				if (tasks.Count == 3 || i == count - 1)
				{
					exceptions.AddRange(await tasks.WhenAllHandler());
					tasks.Clear();
				}
			}
			if (exceptions.Count > 0)
				throw new AggregateException(exceptions);
		}
		catch (Exception)
		{
			await Notifier.Result(success: false, context);
			throw;
		}
		await Notifier.Result(success: true, context);
	}

	[FunctionName($"{nameof(Orchestration)}_{nameof(Start)}")]
	public async Task<HttpResponseMessage> Start([HttpTrigger("get")] HttpRequestMessage req, [DurableClient] IDurableOrchestrationClient starter, ILogger log)
		=> starter.CreateCheckStatusResponse(req, await starter.StartNewAsync($"{nameof(Orchestration)}_{nameof(Run)}"));
}
