using Polly;
using Polly.Timeout;

namespace Commerce.Inventory.Service.Clients;

public static class Extensions
{
    public static IServiceCollection AddClients(this IServiceCollection services)
    {
        var randomJiterrer = new Random();
        services.AddHttpClient<ProductClient>(client =>
         {
             client.BaseAddress = new Uri("https://localhost:2500");
         })
         // Defines a behavior during http request exceptions.
         .AddTransientHttpErrorPolicy(builder =>
         // Wait for specified error thrown, then run specified process. (Specifies the type of exception that this policy can handle.)
         builder.Or<TimeoutRejectedException>()
         // Request repeated 5 times if there is a problem in remote server, each attempt delays amount of specified times.
         .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(randomJiterrer.Next(0, 1000)),
         onRetry: (outcome, timespan, retryAttempt) =>
         {
             // Create an instance for logger to log ProductClient requests.
             var serviceProvider = services.BuildServiceProvider();
             serviceProvider.GetService<ILogger<ProductClient>>()?.LogWarning($"Delay for {timespan} seconds, then making retry {retryAttempt}");
         }))
         .AddTransientHttpErrorPolicy(builder =>
         // After the specified retry attempt, circuit breaker activated and during the specified duration of time, request rejected to remote server
         // It gives a time to remote server to become active again.
         builder.Or<TimeoutRejectedException>()
         .CircuitBreakerAsync(3,
         TimeSpan.FromSeconds(15),
         onBreak: (outcome, timespan) =>
         {
             var serviceProvider = services.BuildServiceProvider();
             serviceProvider.GetService<ILogger<ProductClient>>()?.LogWarning($"Openin the circuit for {timespan.TotalSeconds} seconds.");
         },
         onReset: () =>
         {
             var serviceProvider = services.BuildServiceProvider();
             serviceProvider.GetService<ILogger<ProductClient>>()?.LogWarning($"Closing the circuit...");
         }
         ))
         .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(3));

        return services;
    }
}