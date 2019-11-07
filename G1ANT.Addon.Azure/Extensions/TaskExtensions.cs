using System;
using System.Threading;
using System.Threading.Tasks;

namespace G1ANT.Addon.Azure.Extensions
{
    public static class TaskExtensions
    {
        private struct VoidTypeStruct { } //dummy struct - there is no non-generic version of TaskCompletionSource<TResult>
        public static Task TimeoutAfter(this Task task, int millisecondsTimeout)
        {
            if (task.IsCompleted || millisecondsTimeout == Timeout.Infinite)
            {
                return task;
            }

            var completionSource = new TaskCompletionSource<VoidTypeStruct>();

            if (millisecondsTimeout == 0)
            {
                completionSource.SetException(new TimeoutException());
                return completionSource.Task;
            }

            var timeoutTimer = new Timer(state => SetTimerTimeoutException(state), completionSource, millisecondsTimeout, Timeout.Infinite);

            task.ContinueWith((antecedent, state) => 
                FinishTaskExecution(antecedent, state),
                Tuple.Create(timeoutTimer, completionSource),
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);

            return completionSource.Task;
        }

        internal static void MarshalTaskResults<TResult>(
    Task source, TaskCompletionSource<TResult> proxy)
        {
            switch (source.Status)
            {
                case TaskStatus.Faulted:
                    proxy.TrySetException(source.Exception);
                    break;
                case TaskStatus.Canceled:
                    proxy.TrySetCanceled();
                    break;
                case TaskStatus.RanToCompletion:
                    var castedSource = source as Task<TResult>;
                    proxy.TrySetResult(castedSource == null ? default(TResult) : castedSource.Result);
                    break;
            }
        }

        private static void SetTimerTimeoutException(object state)
        {
            var completionSource = (TaskCompletionSource<VoidTypeStruct>)state;
            completionSource.TrySetException(new TimeoutException());
        }

        private static void FinishTaskExecution(Task antecedent, object state)
        {
            var tuple = (Tuple<Timer, TaskCompletionSource<VoidTypeStruct>>)state;
            tuple.Item1.Dispose();
            MarshalTaskResults(antecedent, tuple.Item2);
        }
    }
}
