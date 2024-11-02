using RabbitMQ.Client;
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

Console.WriteLine(value: "Digite sua mansagem e aperte <ENTER>");

while (true)
{
    string message = Console.ReadLine();

    if (message == "")
        break;

    Aluno aluno = new Aluno() { Id = 1, Nome = "Enivaldo" };
    message = JsonSerializer.Serialize(aluno);

    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(exchange: string.Empty,
                         routingKey: "hello",
                         basicProperties: null,
                         body: body);

    Console.WriteLine(value: $"[x] Enviado {message}");
}

public class Aluno
{
    public int Id { get; set; }
    public string Nome { get; set; }
}