using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace iKudo.Domain.Model
{
    public class NotificationMessage : Notification
    {
        public NotificationMessage(Notification notification)
        {
            Notification = notification;

            Type = notification.Type;
            Id = notification.Id;
            ReadDate = notification.ReadDate;
            ReceiverId = notification.ReceiverId;
            SenderId = notification.SenderId;
            CreationDate = notification.CreationDate;
            BoardId = notification.BoardId;
        }

        private NotificationMessage()
        {
        }

        private Notification Notification { get; set; }

        public string Message => GenerateMessage();

        public string Title => GetDisplayAttributeOfType().Name;

        private DisplayAttribute GetDisplayAttributeOfType()
        {
            return Notification.Type.GetType().GetMember(Notification.Type.ToString())[0].GetCustomAttribute(typeof(DisplayAttribute), false) as DisplayAttribute;
        }

        private string GenerateMessage()
        {
            string description = GetDisplayAttributeOfType().Description;

            foreach (Match item in Regex.Matches(description, @"{(.*?)}"))
            {
                string fullQuelifiedPropName = item.Value.Trim('{', '}');
                string[] propNameParts = fullQuelifiedPropName.Split('.');

                object value = Notification.GetType().GetProperty(propNameParts[0]).GetValue(Notification);
                foreach (var prop in propNameParts.Skip(1))
                {
                    value = value.GetType().GetProperty(prop).GetValue(value);
                }

                description = description.Replace(item.Value, value.ToString());
            }

            return description;
        }
    }
}
