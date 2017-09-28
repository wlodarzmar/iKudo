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

        [Required]
        public string SenderId { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        public DateTime CreationDate { get; set; }

        public int? BoardId { get; set; }

        public DateTime? ReadDate { get; set; }

        public BoardDTO Board { get; set; }

        public bool IsRead => ReadDate.HasValue;

        public string Title { get; set; }

        public string Message { get; set; }
    }
}
