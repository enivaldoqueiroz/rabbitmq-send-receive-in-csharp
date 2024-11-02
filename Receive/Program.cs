using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "hello",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

Console.WriteLine(value: " [*] Argumento mensagens.");

EventingBasicConsumer consumer = new EventingBasicConsumer(model: channel);
consumer.Received += (model, basicDeliverEventArgs) =>
{
    byte[] body = basicDeliverEventArgs.Body.ToArray();
    string message = Encoding.UTF8.GetString(body);

    Aluno aluno = JsonSerializer.Deserialize<Aluno>(message);

    Console.WriteLine(value: $" [x] Recebido: {message}");

};

channel.BasicConsume(queue: "hello",
                     autoAck: true,
                     consumer: consumer);
Console.WriteLine(value: " Aperte [ENTER] para sair");
Console.ReadLine();

public class Aluno
{
    public int Id { get; set; }
    public string Nome { get; set; }
}