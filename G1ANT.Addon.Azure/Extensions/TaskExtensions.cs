using System;
using System.Threading;
using System.Threading.Tasks;

namespace G1ANT.Addon.Azure.Extensions
{
    public static class TaskExtensions
    {
        internal struct VoidTypeStruct { } //dummy struct - there is no non-generic version of TaskCompletionSource<TResult>
        public static Task TimeoutAfter(this Task task, int millisecondsTimeout)
        {
            if (task.IsCompleted || (millisecondsTimeout == Timeout.Infinite))
            {
                return task;
            }

            TaskCompletionSource<VoidTypeStruct> tcs = new TaskCompletionSource<VoidTypeStruct>();

            if (millisecondsTimeout == 0)
            {
                tcs.SetException(new TimeoutException());
                return tcs.Task;
            }

            Timer timer = new Timer(state =>
            {
                var myTcs = (TaskCompletionSource<VoidTypeStruct>)state;
                myTcs.TrySetException(new TimeoutException());
            }, tcs, millisecondsTimeout, Timeout.Infinite);

            task.ContinueWith((antecedent, state) =>
            {
                var tuple = (Tuple<Timer, TaskCompletionSource<VoidTypeStruct>>)state;
                tuple.Item1.Dispose();
                MarshalTaskResults(antecedent, tuple.Item2);
            },
            Tuple.Create(timer, tcs),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);

            return tcs.Task;
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
    }
}
