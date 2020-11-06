using System;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using RabbitMQ.Client;

namespace RabbitProducer
{
	class Program
	{
		public static void Main()
		{
			var summary = BenchmarkRunner.Run<RabbitProducerClassBenchmark>();
		}
	}

	public class RabbitProducerClass : IDisposable
	{
		private readonly IModel _channel;
		private readonly IConnection _connection;
		
		public RabbitProducerClass()
		{
			var factory = new ConnectionFactory {HostName = "localhost"};
			_connection = factory.CreateConnection();
			_channel = _connection.CreateModel();
			_channel.QueueDeclare(queue: "hello",
				durable: false,
				exclusive: false,
				autoDelete: false,
				arguments: null);
		}

		public void Dispose()
		{
			_channel.Dispose();
			_connection.Dispose();
		}
		
		public void Produce()
		{
			string message = "Hello World!";
			var body = Encoding.UTF8.GetBytes(message);

			_channel.BasicPublish(exchange: "",
				routingKey: "hello",
				basicProperties: null,
				body: body);
			//Console.WriteLine(" [x] Sent {0}", message);
		}
	}

	public class RabbitProducerClassBenchmark
	{
		private readonly RabbitProducerClass _rabbitProducer;

		public RabbitProducerClassBenchmark()
		{
			_rabbitProducer = new RabbitProducerClass();
		}

		[Benchmark]
		public void ProducingBenchmark() => _rabbitProducer.Produce();
	}
}