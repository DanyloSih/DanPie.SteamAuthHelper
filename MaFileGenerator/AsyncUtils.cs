using System;
using System.Threading.Tasks;

namespace MaFileGenerator
{
    public static class AsyncUtils
    {
        public static async Task<TimeoutTaskResult<T>> LaunchAsyncOperationWithTimeout<T>(T longRunningTask, int timeout)
            where T: Task
        {
            var timeoutTask = Task.Delay(timeout);

            var completedTask = await Task.WhenAny(longRunningTask, timeoutTask);

            if (completedTask == timeoutTask)
            {
                Console.WriteLine("Operation timed out!");
                return new TimeoutTaskResult<T>(false, null);
            }
            else
            {
                await longRunningTask;
                return new TimeoutTaskResult<T>(true, longRunningTask);
            }
        }
    }
}
