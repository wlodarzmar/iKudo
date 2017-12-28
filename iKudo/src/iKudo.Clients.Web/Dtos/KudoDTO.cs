using System;
using System.ComponentModel.DataAnnotations;

namespace iKudo.Dtos
{
    public class KudoDTO
    {
        public int Id { get; set; }

        public KudoTypeDTO Type { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        [Required]
        public string SenderId { get; set; }

        public string Description { get; set; }

        public bool IsAnonymous { get; set; }

        public int BoardId { get; set; }

        public DateTime CreationDate { get; set; }

        public string Image { get; set; }
    }
}
