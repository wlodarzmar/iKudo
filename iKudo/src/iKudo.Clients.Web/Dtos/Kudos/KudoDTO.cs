using iKudo.Clients.Web.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace iKudo.Dtos
{
    public class KudoDto
    {
        public int Id { get; set; }

        public KudoTypeDto Type { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        public UserDto Receiver { get; set; }

        [Required]
        public string SenderId { get; set; }

        public UserDto Sender { get; set; }

        public string Description { get; set; }

        public bool IsAnonymous { get; set; }

        public int BoardId { get; set; }

        public DateTime CreationDate { get; set; }

        public int Status { get; set; }

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

        public bool IsApprovalEnabled { get; set; }

        public bool CanRemove { get; set; }
    }
}
