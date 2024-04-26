namespace Server.Messages
{
    public class HelloMessage : HelloResponse // В Response приходится убирать sealed
    {
        public HelloMessage(string firstName, string lastName)
        {
            Message = $"Hello {firstName} {lastName}!";
        }
    }
}
