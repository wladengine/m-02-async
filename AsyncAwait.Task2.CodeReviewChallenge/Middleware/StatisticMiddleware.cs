using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using CloudServices.Interfaces;
using AsyncAwait.Task2.CodeReviewChallenge.Headers;

namespace AsyncAwait.Task2.CodeReviewChallenge.Middleware;

public class StatisticMiddleware
{
    private readonly RequestDelegate _next;

    private readonly IStatisticService _statisticService;
    private readonly IBackgroundTaskScheduler _taskScheduler;

    public StatisticMiddleware(RequestDelegate next, IStatisticService statisticService, IBackgroundTaskScheduler taskScheduler)
    {
        _next = next;
        _statisticService = statisticService ?? throw new ArgumentNullException(nameof(statisticService));
        _taskScheduler = taskScheduler;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string path = context.Request.Path;

        long count = await _statisticService.GetVisitsCountAsync(path) + 1;

        _taskScheduler.EnqueueTask(() => _statisticService.RegisterVisitAsync(path));

        context.Response.Headers.Add(
            CustomHttpHeaders.TotalPageVisits,
            count.ToString());

        await _next(context);
    }
}
