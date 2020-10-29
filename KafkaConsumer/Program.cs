using System;
using System.Threading;
using Confluent.Kafka;

namespace KafkaConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "127.0.0.1:9092",
                GroupId = "foo",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            CancellationTokenSource cts = new CancellationTokenSource();
            Thread consumerThread = new Thread(o =>
            {
                using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            
                consumer.Subscribe("my-topic");

                while (!cts.IsCancellationRequested)
                {
                    var consumeResult = consumer.Consume(cts.Token);
                
                    Console.WriteLine($"Consumed message: {consumeResult.Message}");
                }
            
                consumer.Close();
            });

            consumerThread.Start();
            Console.ReadLine();
            cts.Cancel();
            consumerThread.Abort();
        }
    }
}
