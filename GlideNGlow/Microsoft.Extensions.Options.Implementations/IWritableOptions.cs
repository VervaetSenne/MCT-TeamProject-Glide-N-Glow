namespace Microsoft.Extensions.Options.Implementations
{
	public interface IWritableOptions<out T> : IOptionsMonitor<T> where T : class, new()
	{
		void Update(Action<T> applyChanges, bool doesWrite = true);
	}
}
