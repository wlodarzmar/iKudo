using iKudo.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace iKudo.Dtos
{
    public class NotificationDto
    {
        public int Id { get; set; }

        public NotificationTypes Type { get; set; }

        public string TypeName => Type.ToString().ToLower();

        [Required]
        public string SenderId { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        public DateTime CreationDate { get; set; }

        public int? BoardId { get; set; }

        public DateTime? ReadDate { get; set; }

        public BoardDto Board { get; set; }

        public bool IsRead => ReadDate.HasValue;

        public int? KudoId { get; set; }
    }

    public class NotificationGetDto : NotificationDto
    {
        public UserDto Receiver { get; set; }

        public UserDto Sender { get; set; }
    }
}