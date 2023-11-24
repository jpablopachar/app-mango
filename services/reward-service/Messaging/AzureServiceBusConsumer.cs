using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using reward_service.Interfaces;
using reward_service.Message;
using reward_service.Services;

namespace reward_service.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string _serviceBusConnectionString;
        private readonly string _orderCreatedTopic;
        private readonly string _orderCreatedRewardSubscription;
        private readonly IConfiguration _configuration;
        private readonly RewardService _rewardService;
        private readonly ServiceBusProcessor _rewardProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, RewardService rewardService)
        {
            _configuration = configuration;
            _rewardService = rewardService;

            _serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString")!;
            _orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic")!;
            _orderCreatedRewardSubscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Rewards_Subscription")!;

            var client = new ServiceBusClient(_serviceBusConnectionString);

            _rewardProcessor = client.CreateProcessor(_orderCreatedTopic, _orderCreatedRewardSubscription);
        }

        /// <summary>Sets up event handlers for processing new order rewards requests and
        /// errors, and then starts the reward processor asynchronously.</summary>
        public async Task Start()
        {
            _rewardProcessor.ProcessMessageAsync += OnNewOrderRewardsRequestReceived;
            _rewardProcessor.ProcessErrorAsync += ErrorHandler;

            await _rewardProcessor.StartProcessingAsync();
        }

        /// <summary>Stops and disposes the `_rewardProcessor` object.</summary>
        public async Task Stop()
        {
            await _rewardProcessor.StopProcessingAsync();
            await _rewardProcessor.DisposeAsync();
        }

        /// <summary>Handles the processing of a new order rewards request message
        /// by updating the rewards and completing the message.</summary>
        /// <param name="args">Contains information about a message received by a
        /// message processing function.</param>
        private async Task OnNewOrderRewardsRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardsMessage rewardsMessage = JsonConvert.DeserializeObject<RewardsMessage>(body)!;

            try
            {
                await _rewardService.UpdateRewards(rewardsMessage);

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