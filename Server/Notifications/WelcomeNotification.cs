using Infrastructure.Abstractions;

namespace Server.Notifications
{
    internal class WelcomeNotification : INotification 
    {
        // Порядок полей важен и должен соответствовать порядку параметров конструктора сообщения

        public string FullName { get; private set; }

        public int Age { get; private set; }

        public WelcomeNotification(string fullName, int age)
        {
            FullName = fullName;
            Age = age;
        }
    }
}
