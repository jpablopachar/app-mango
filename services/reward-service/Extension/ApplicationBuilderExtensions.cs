using reward_service.Interfaces;

namespace reward_service.Extension
{
    public static class ApplicationBuilderExtensions
    {
        private static IAzureServiceBusConsumer? ServiceBusConsumer { get; set; }

        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();

            var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostApplicationLife!.ApplicationStarted.Register(OnStart);
            hostApplicationLife.ApplicationStopping.Register(OnStop);

            return app;
        }

        private static void OnStop() => ServiceBusConsumer!.Stop();

        private static void OnStart() => ServiceBusConsumer!.Start();
    }
}