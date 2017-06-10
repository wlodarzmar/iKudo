using iKudo.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

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

        public bool IsRead => ReadDate.HasValue;

        public string Message => GetDisplayAttributeOfType().Description;

        public string Title => GetDisplayAttributeOfType().Name;

        private DisplayAttribute GetDisplayAttributeOfType()
        {
            return Type.GetType().GetMember(Type.ToString())[0].GetCustomAttribute(typeof(DisplayAttribute), false) as DisplayAttribute;
        }
    }
}
