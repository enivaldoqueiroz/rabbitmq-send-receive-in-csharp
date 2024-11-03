# Projeto de Exemplo com RabbitMQ

Este projeto é uma aplicação de exemplo em C# que utiliza o RabbitMQ para enviar e receber mensagens. Ele demonstra o funcionamento de um **produtor** e um **consumidor** de mensagens, utilizando uma fila chamada `hello` para transmitir dados de um objeto `Aluno`.

## Requisitos

- [.NET 8](https://dotnet.microsoft.com/download/dotnet) ou superior
- [RabbitMQ](https://www.rabbitmq.com/download.html) instalado e em execução na máquina local, com o host configurado como `localhost`

## Instalação e Configuração do RabbitMQ

1. Baixe e instale o RabbitMQ em sua máquina local. Siga as instruções de instalação do site oficial: [RabbitMQ Installation Guide](https://www.rabbitmq.com/download.html).
2. Após a instalação, inicie o RabbitMQ com o comando:
   ```bash  rabbitmq-server ```

3. Verifique se o RabbitMQ está em execução no endereço padrão `localhost` e na porta `15672` - http://localhost:15672/.

## Estrutura do Projeto

O projeto contém dois principais componentes:

- **Produtor**: Envia mensagens para a fila `hello`, contendo informações de um objeto `Aluno` (Id e Nome) em formato JSON.
- **Consumidor**: Recebe mensagens da fila `hello`, desserializa os dados de `Aluno`, e exibe as informações do objeto no console.

## Configuração e Uso

### 1. Inicialize o Produtor

O **produtor** lê a entrada do usuário para enviar mensagens de texto, que são transformadas em objetos `Aluno` e serializadas em JSON antes de serem enviadas para a fila RabbitMQ.

#### Código do Produtor

```csharp
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

Console.WriteLine("Digite sua mensagem e aperte <ENTER>");

while (true)
{
    string message = Console.ReadLine();
    if (message == "") break;

    Aluno aluno = new Aluno() { Id = 1, Nome = "Enivaldo" };
    message = JsonSerializer.Serialize(aluno);

    var body = Encoding.UTF8.GetBytes(message);
    channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);

    Console.WriteLine($"[x] Enviado {message}");
}

public class Aluno
{
    public int Id { get; set; }
    public string Nome { get; set; }
}
```

Para rodar o produtor, execute o código e insira uma mensagem. Pressione Enter para enviar. Um objeto `Aluno` será enviado à fila `hello`.

### 2. Inicialize o Consumidor

O **consumidor** recebe as mensagens da fila `hello`, converte o conteúdo JSON de volta para um objeto `Aluno` e exibe as informações no console.

#### Código do Consumidor

```csharp
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

Console.WriteLine(" [*] Aguardando mensagens.");

EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    Aluno aluno = JsonSerializer.Deserialize<Aluno>(message);
    Console.WriteLine($" [x] Recebido {message}");
};

channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);
Console.WriteLine("Aperte [ENTER] para sair");
Console.ReadLine();

public class Aluno
{
    public int Id { get; set; }
    public string Nome { get; set; }
}
```

Para rodar o consumidor, execute o código. O consumidor ficará "ouvindo" a fila `hello` e processará cada mensagem recebida.

## Executando o Projeto

1. Inicie o RabbitMQ.
2. Abra um terminal para rodar o **produtor** e outro para o **consumidor**.
3. No terminal do **produtor**, insira uma mensagem e pressione Enter para enviar.
4. No terminal do **consumidor**, a mensagem será exibida quando recebida.
