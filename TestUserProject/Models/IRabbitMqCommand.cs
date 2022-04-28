namespace TestUserProject.Models
{
    public interface IRabbitMqCommand
    {
        public void SendMessage(string message);
    }
}
