using iKudo.Clients.Web.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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

        public bool IsPrivate { get; set; }

        public string Image { get; set; }

        public bool HasImage => !string.IsNullOrWhiteSpace(Image);

        [RequiredIf(nameof(HasImage), "True")]
        [FileExtensions(Extensions = "jpg,jpeg,png,gif")]
        public string ImageExtension
        {
            get
            {
                if (HasImage)
                {
                    return $".{Image?.Split(';').FirstOrDefault()?.Split('/').LastOrDefault() ?? string.Empty}";
                }

                return null;
            }
        }
    }
}
