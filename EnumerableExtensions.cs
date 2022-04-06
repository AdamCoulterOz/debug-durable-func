public static class EnumerableExtensions
{
	public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
	{
		source.All(x =>
		{
			action.Invoke(x);
			return true;
		});
	}

    public static async Task<List<Exception>> WhenAllHandler(this IEnumerable<Task> tasks)
    {
        var whenAllTask = Task.WhenAll(tasks);
        var allExceptions = new List<Exception>();
        try
        {
            await whenAllTask;
        }
        catch (Exception)
        {
            whenAllTask.Exception?.InnerExceptions.ForEach(e => allExceptions.Add(e));
        }
        return allExceptions;
    }
}