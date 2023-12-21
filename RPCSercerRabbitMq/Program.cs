using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RPCSercerRabbitMq.Model;
using Newtonsoft.Json;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "rpc_queue",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
var consumer = new EventingBasicConsumer(channel);
channel.BasicConsume(queue: "rpc_queue",
                     autoAck: false,
                     consumer: consumer);
Console.WriteLine(" [x] Awaiting RPC requests");

consumer.Received += (model, ea) =>
{
    string response = string.Empty;

    var body = ea.Body.ToArray();
    var props = ea.BasicProperties;
    var replyProps = channel.CreateBasicProperties();
    replyProps.CorrelationId = props.CorrelationId;
    ResponseModel? responseModel = new ResponseModel();

    try
    {
        var message = Encoding.UTF8.GetString(body);
        RequestModel? req = JsonConvert.DeserializeObject<RequestModel>(message);
        if (req.RequestTime == new DateTime(1, 1, 1, 0, 0, 0))
            req.RequestTime = DateTime.Now;

       

        Console.WriteLine($"\n\n\n{DateTime.Now}\nNew request\nMachine Name : {req.MachineName}\nReqest Time (meta data) : {req.RequestTime}");
        Console.WriteLine("CorrelationId id " + replyProps.CorrelationId);
        Console.WriteLine("request id " + req.ID);

        string _filePath = "Something is here";
        responseModel.MachineName = req.MachineName;
        responseModel.filePath = _filePath;




        //int n = int.Parse(message);
        //Console.WriteLine($" [.] Fib({message})");
        //response = Fib(n).ToString();
    }
    catch (Exception e)
    {
        Console.WriteLine($" [.] {e.Message}");
        responseModel = new ResponseModel { HasError = true };
        responseModel = new ResponseModel { ErrorDescription = "Error: "+e };
        response = string.Empty;
    }
    finally
    {
        response = JsonConvert.SerializeObject(responseModel);
        var responseBytes = Encoding.UTF8.GetBytes(response);
        channel.BasicPublish(exchange: string.Empty,
                             routingKey: props.ReplyTo,
                             basicProperties: replyProps,
                             body: responseBytes);
        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
    }
};

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();


//static int Fib(int n)
//{
//    if (n is 0 or 1)
//    {
//        return n;
//    }

//    return Fib(n - 1) + Fib(n - 2);
//}