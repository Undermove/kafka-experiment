using System;
using System.Net;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Confluent.Kafka;

namespace KafkaProducer
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var summary = BenchmarkRunner.Run<KafkaProducerClassBenchmark>();
		}
	}
	
	public class KafkaProducerClass
	{
		public void Produce()
		{
			var config = new ProducerConfig
			{
				BootstrapServers = "localhost:9092",
				SecurityProtocol = SecurityProtocol.Plaintext,
				ClientId = Dns.GetHostName()
			};

			using var producer = new ProducerBuilder<Null, string>(config).Build();


			producer.Produce(
				"my-topic", new Message<Null, string> {Value = "hello world"}, report =>
				{
					if (report.Error.Code != ErrorCode.NoError)
					{
						Console.WriteLine($"Error: {report.Error}");
						return;
					}

					Console.WriteLine($"Offset: {report.Offset}");
				});

			producer.Flush();
		}
	}
		
	public class KafkaProducerClassBenchmark
	{
		private readonly KafkaProducerClass _kafkaProducer;
			
		public KafkaProducerClassBenchmark()
		{
			_kafkaProducer = new KafkaProducerClass();
		}

		[Benchmark]
		public void ProducingBenchmark() => _kafkaProducer.Produce();
	}
}