using System.Threading.Tasks;

namespace MaFileGenerator
{
    public struct TimeoutTaskResult<T>
        where T: Task
    {
        public bool IsTaskCompleted;
        public T CompletedTask;

        public TimeoutTaskResult(bool isTaskCompleted, T completedTask)
        {
            IsTaskCompleted = isTaskCompleted;
            CompletedTask = completedTask;
        }
    }
}
