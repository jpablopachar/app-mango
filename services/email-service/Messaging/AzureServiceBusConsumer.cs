using System.Text;
using Azure.Messaging.ServiceBus;
using email_service.Dtos;
using email_service.Services;
using Newtonsoft.Json;

namespace email_service.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string _serviceBusConnectionString;
        private readonly string _emailCartQueue;
        private readonly string _registerUserQueue;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly ServiceBusProcessor _emailCartProcessor;
        private readonly ServiceBusProcessor _registerUserProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;

            _serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString")!;
            _emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue")!;
            _registerUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue")!;

            var client = new ServiceBusClient(_serviceBusConnectionString);

            _emailCartProcessor = client.CreateProcessor(_emailCartQueue);
            _registerUserProcessor = client.CreateProcessor(_registerUserQueue);
        }

        /// <summary>Starts two processors, one for processing email cart requests and
        /// another for processing user registration requests.</summary>
        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;

            await _emailCartProcessor.StartProcessingAsync();

            _registerUserProcessor.ProcessMessageAsync += OnUserRegisterRequestReceived;
            _registerUserProcessor.ProcessErrorAsync += ErrorHandler;

            await _registerUserProcessor.StartProcessingAsync();
        }

        /// <summary>Stops and disposes two processors asynchronously.</summary>
        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();

            await _registerUserProcessor.StopProcessingAsync();
            await _registerUserProcessor.DisposeAsync();
        }

        /// <summary>Handles the processing of an email cart request by deserializing the
        /// request body, calling the `_emailService.EmailCartAndLogAsync` method, and
        /// completing the message.</summary>
        /// <param name="args">Contains information about a message
        /// received by a message processor.</param>
        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(body)!;

            try
            {
                await _emailService.EmailCartAndLogAsync(cartDto);
                await args.CompleteMessageAsync(message);
            }
            catch (Exception) { throw; }
        }

        /// <summary>Handles a user registration request by extracting the email from the
        /// message body, registering the email and logging the registration
        /// asynchronously, and then completing the message.</summary>
        /// <param name="args">Contains information about a message received by a message processor.</param>
        private async Task OnUserRegisterRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            string email = JsonConvert.DeserializeObject<string>(body)!;

            try
            {
                await _emailService.RegisterUserEmailAndLogAsync(email);
                await args.CompleteMessageAsync(message);
            }
            catch (Exception) { throw; }
        }

        /// <summary>Handles and logs exceptions that occur during a process.</summary>
        /// <param name="args">Contains information about an error that occurred during a process.</param>
        /// <returns>Task object with the CompletedTask property.</returns>
        private static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());

            return Task.CompletedTask;
        }
    }
}