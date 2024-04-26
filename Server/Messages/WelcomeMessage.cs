namespace Server.Messages
{
    public class WelcomeMessage : HelloResponse // В Response приходится убирать sealed
    {

        public WelcomeMessage(string fullName, int age)
        {
            Message = $"Hello {fullName} ({age} years)!";
        }
    }
}
