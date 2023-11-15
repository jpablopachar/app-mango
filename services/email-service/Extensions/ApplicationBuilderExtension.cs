using email_service.Messaging;

namespace email_service.Extensions
{
    public static class ApplicationBuilderExtension
    {
        private static IAzureServiceBusConsumer? AzureServiceBusConsumer { get; set; }

        /// <summary>Register an Azure Service Bus consumer and handle application start
        /// and stop events.</summary>
        /// <param name="app">Configure the application's request pipeline. It
        /// provides methods for adding middleware components to the pipeline
        /// and configuring them.</param>
        /// <returns>`IApplicationBuilder`.</returns>
        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            AzureServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();

            var hostApplicationLifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostApplicationLifetime?.ApplicationStarted.Register(OnStart);
            hostApplicationLifetime?.ApplicationStopped.Register(OnStop);

            return app;
        }

        private static void OnStop()
        {
            AzureServiceBusConsumer!.Stop();
        }

        private static void OnStart()
        {
            AzureServiceBusConsumer!.Start();
        }
    }
}