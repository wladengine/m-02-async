using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AsyncAwait.Task2.CodeReviewChallenge.Headers;
using CloudServices.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace AsyncAwait.Task2.CodeReviewChallenge.Middleware;

public class StatisticMiddleware
{
    private readonly RequestDelegate _next;

    private readonly IStatisticService _statisticService;

    public StatisticMiddleware(RequestDelegate next, IStatisticService statisticService)
    {
        _next = next;
        _statisticService = statisticService ?? throw new ArgumentNullException(nameof(statisticService));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string path = context.Request.Path;

        Task<long> visitsCountTask = _statisticService.GetVisitsCountAsync(path);
        _statisticService.RegisterVisitAsync(path);

        long count = await visitsCountTask + 1;
        context.Response.Headers.Add(
            CustomHttpHeaders.TotalPageVisits,
            count.ToString());

        await _next(context);
    }
    
}
