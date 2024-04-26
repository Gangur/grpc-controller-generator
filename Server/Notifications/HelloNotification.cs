using Infrastructure.Abstractions;

namespace Server.Notifications
{
    public class HelloNotification : INotification
    {
        // Порядок полей важен и должен соответствовать порядку параметров конструктора сообщения

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public HelloNotification(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
