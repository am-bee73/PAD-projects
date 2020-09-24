namespace Broker.Models
{
    public class Message
    {
        public Message(string nickname, string topic, string messageBody)
        {
            Nickname = nickname;
            Topic = topic;
            MessageBody = messageBody;
        }

        public string Nickname { get; }
        public string Topic { get; }
        public string MessageBody { get; }
    }
}
