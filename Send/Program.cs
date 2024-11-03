using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

/// <summary>
/// Este código demonstra a criação de um produtor RabbitMQ em C#. 
/// Ele envia mensagens contendo dados de um objeto `Aluno` para uma fila chamada "hello". 
/// O usuário é solicitado a digitar uma mensagem para iniciar o envio, e o loop continua até que uma entrada vazia seja inserida.
/// Cada mensagem representa um objeto `Aluno`, serializado em JSON, e é enviada para a fila utilizando UTF-8.
/// </summary>
var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Declaração da fila "hello"
channel.QueueDeclare(queue: "hello",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

Console.WriteLine(value: "Digite sua mensagem e aperte <ENTER>");

while (true)
{
    // Leitura da mensagem do usuário
    string message = Console.ReadLine();

    // Interrompe o loop se a mensagem estiver vazia
    if (message == "")
        break;

    // Criação e serialização de um objeto Aluno
    Aluno aluno = new Aluno() { Id = 1, Nome = "Enivaldo" };
    message = JsonSerializer.Serialize(aluno);

    // Conversão da mensagem para bytes
    var body = Encoding.UTF8.GetBytes(message);

    // Envio da mensagem para a fila "hello"
    channel.BasicPublish(exchange: string.Empty,
                         routingKey: "hello",
                         basicProperties: null,
                         body: body);

    Console.WriteLine(value: $"[x] Enviado {message}");
}

/// <summary>
/// Classe que representa um aluno com propriedades Id e Nome.
/// </summary>
public class Aluno
{
    public int Id { get; set; }
    public string Nome { get; set; }
}
