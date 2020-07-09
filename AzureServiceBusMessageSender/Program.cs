using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace AzureServiceBusMessageSender
{
    internal class Program
    {
        private const string ServiceBusConnectionString = "<service_bus_connection_string_here>";
        private const string TopicName = "<topic_name_here";
        private static ITopicClient _topicClient;

        public static async Task Main(string[] args)
        {
            _topicClient = new TopicClient(ServiceBusConnectionString, TopicName);

            Console.WriteLine("======================================================");
            Console.WriteLine("Press ENTER key to exit after sending all the messages.");
            Console.WriteLine("======================================================");

            // Read the message body we want to send
            var folderPath = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "App_Data"));
            var filePath = folderPath.FullName + "\\generator-message.json";

            await SendMessagesAsync(filePath);

            Console.ReadKey();

            await _topicClient.CloseAsync();
        }

        private static async Task SendMessagesAsync(string messageBodyFile)
        {
            try
            {

                // Create a new message to send to the topic.
                var message = new Message(Encoding.UTF8.GetBytes(messageBodyFile))
                {
                    ContentType = "application/json",
                    ReplyTo = TopicName
                };

                // Write the body of the message to the console.
                Console.WriteLine("Sending message...");

                // Send the message to the topic.
                await _topicClient.SendAsync(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
    }
}
