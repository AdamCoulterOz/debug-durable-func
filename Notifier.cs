using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace AdamCoulterOz;

public static class Notifier
{
	public static async Task Result(bool success, IDurableOrchestrationContext context)
		=> await context.CallActivityAsync($"{nameof(Notifier)}_{(success ? nameof(Notifier.Succeeded) : nameof(Notifier.Failed))}", null);

	[FunctionName($"{nameof(Notifier)}_{nameof(Failed)}")]
	public static void Failed([ActivityTrigger] IDurableActivityContext context, ILogger log)
		=> log.LogCritical($"Saying hello has failed critically");

	[FunctionName($"{nameof(Notifier)}_{nameof(Succeeded)}")]
	public static void Succeeded([ActivityTrigger] IDurableActivityContext context, ILogger log)
		=> log.LogInformation($"Saying hello has a great success");
}