using iKudo.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace iKudo.Dtos
{
    public class NotificationDto
    {
        public int Id { get; set; }

        public NotificationTypes Type { get; set; }

        [Required]
        public string SenderId { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        public DateTime CreationDate { get; set; }

        public int? BoardId { get; set; }

        public DateTime? ReadDate { get; set; }

        public BoardDto Board { get; set; }

        public bool IsRead => ReadDate.HasValue;

        public string Title { get; set; }

        public string Message { get; set; }
    }
}
