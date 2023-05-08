/*
* Study the code of this application to calculate the sum of integers from 0 to N, and then
* change the application code so that the following requirements are met:
* 1. The calculation must be performed asynchronously.
* 2. N is set by the user from the console. The user has the right to make a new boundary in the calculation process,
* which should lead to the restart of the calculation.
* 3. When restarting the calculation, the application should continue working without any failures.
*/

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait.Task1.CancellationTokens;

internal class Program
{
    /// <summary>
    /// The Main method should not be changed at all.
    /// </summary>
    /// <param name="args"></param>
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Mentoring program L2. Async/await.V1. Task 1");
        Console.WriteLine("Calculating the sum of integers from 0 to N.");
        Console.WriteLine("Use 'q' key to exit...");
        Console.WriteLine();

        AskN();

        var input = Console.ReadLine();
        while (input?.Trim().ToUpper() != "Q")
        {
            CancellationTokenSource cancellationTokenSource = new();
            Task calculateTask = null;
            if (int.TryParse(input, out int n))
            {
                calculateTask = CalculateSumAsync(n, cancellationTokenSource.Token);
            }
            else
            {
                Console.WriteLine($"Invalid integer: '{input}'. Please try again.");
                AskN();
            }

            input = Console.ReadLine();
            cancellationTokenSource.Cancel();
            if (calculateTask != null)
            {
                await calculateTask;
            }
        }

        Console.WriteLine("Press any key to continue");
        Console.ReadLine();
    }

    private static async Task CalculateSumAsync(int n, CancellationToken token)
    {
        Console.WriteLine($"The task for {n} started... Enter N to cancel the request:");
        try
        {
            var sum = await Calculator.CalculateAsync(n, token);
            Console.WriteLine($"Sum for {n} = {sum}.");
            Console.WriteLine();
            AskN();
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"Sum for {n} cancelled...");
        }
    }

    private static void AskN() => Console.WriteLine("Enter N: ");
}
