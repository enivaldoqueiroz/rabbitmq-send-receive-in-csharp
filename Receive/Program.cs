using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

/// <summary>
/// Este código implementa um consumidor RabbitMQ em C# que recebe e processa mensagens de uma fila chamada "hello".
/// Ele espera até que uma mensagem seja recebida, desserializa o conteúdo JSON para um objeto `Aluno`, 
/// e exibe as informações do objeto no console.
/// A fila é declarada como não durável e configurada para reconhecer mensagens automaticamente após o recebimento.
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

Console.WriteLine(value: " [*] Aguardando mensagens.");

// Configuração do consumidor de eventos
EventingBasicConsumer consumer = new EventingBasicConsumer(model: channel);
consumer.Received += (model, basicDeliverEventArgs) =>
{
    // Recebe e converte o corpo da mensagem
    byte[] body = basicDeliverEventArgs.Body.ToArray();
    string message = Encoding.UTF8.GetString(body);

    // Desserialização da mensagem para um objeto Aluno
    Aluno aluno = JsonSerializer.Deserialize<Aluno>(message);

    Console.WriteLine(value: $" [x] Recebido: {message}");
};

// Configura o consumidor para consumir mensagens da fila "hello"
channel.BasicConsume(queue: "hello",
                     autoAck: true,
                     consumer: consumer);

Console.WriteLine(value: " Aperte [ENTER] para sair");
Console.ReadLine();

/// <summary>
/// Classe que representa um aluno com propriedades Id e Nome.
/// </summary>
public class Aluno
{
    public int Id { get; set; }
    public string Nome { get; set; }
}
