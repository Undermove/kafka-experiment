using System;
using System.Net;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaProducer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9093",
                ClientId = Dns.GetHostName()
            };
            
            using var producer = new ProducerBuilder<Null, string>(config).Build();

            producer.Produce(
                "my-topic", new Message<Null, string> { Value = "hello world" }, report =>
                {
                    if (report.Error != null)
                    {
                        Console.WriteLine($"Error: {report.Error}");
                        return;
                    }
                    
                    Console.WriteLine($"Offset: {report.Offset}");
                });

            Console.ReadLine();
        }
    }
}
