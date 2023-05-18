using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AsyncAwait.Task2.CodeReviewChallenge;

public interface IBackgroundTaskScheduler
{
    Task StartAsync(CancellationToken cancellationToken);
    void EnqueueTask(Func<Task> task);
}

public class BackgroundTaskScheduler : IBackgroundTaskScheduler
{
    private readonly ConcurrentQueue<Func<Task>> _taskQueue = new();
    private readonly SemaphoreSlim _signal = new(0);

    public void EnqueueTask(Func<Task> task)
    {
        _taskQueue.Enqueue(task);
        _signal.Release();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await _signal.WaitAsync(cancellationToken);

            List<Task> tasks = new();
            while (_taskQueue.TryDequeue(out Func<Task> task))
            {
                tasks.Add(task.Invoke());
            }

            if (tasks.Count > 0)
            {
                Task.WaitAll(tasks.ToArray(), cancellationToken);
            }
        }
    }
}
