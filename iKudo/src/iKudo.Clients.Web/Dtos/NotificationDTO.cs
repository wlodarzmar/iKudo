using iKudo.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace iKudo.Dtos
{
    public class NotificationDTO
    {
        public int Id { get; set; }

        public NotificationTypes Type { get; set; }

        public string SenderId { get; set; }

        public string ReceiverId { get; set; }

        public DateTime CreationDate { get; set; }

        public int? BoardId { get; set; }

        public DateTime? ReadDate { get; set; }

        public BoardDTO Board { get; set; }

        public bool IsRead => ReadDate.HasValue;

        public string Message
        {
            get
            {
                string description = GetDisplayAttributeOfType().Description;

                foreach (Match item in Regex.Matches(description, @"{(.*?)}"))
                {
                    string fullQuelifiedPropName = item.Value.Trim('{', '}');
                    string[] propNameParts = fullQuelifiedPropName.Split('.');

                    object value = GetType().GetProperty(propNameParts[0]).GetValue(this);
                    foreach (var prop in propNameParts.Skip(1))
                    {
                        value = value.GetType().GetProperty(prop).GetValue(value);
                    }

                    description = description.Replace(item.Value, value.ToString());
                }
                return description;
            }
        }

        public string Title => GetDisplayAttributeOfType().Name;

        private DisplayAttribute GetDisplayAttributeOfType()
        {
            return Type.GetType().GetMember(Type.ToString())[0].GetCustomAttribute(typeof(DisplayAttribute), false) as DisplayAttribute;
        }
    }
}
