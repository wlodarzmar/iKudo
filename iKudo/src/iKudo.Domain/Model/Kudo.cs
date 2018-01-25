using iKudo.Domain.Enums;
using System;
using System.Text.RegularExpressions;

namespace iKudo.Domain.Model
{
    public class Kudo
    {
        public int Id { get; set; }

        public KudoType Type { get; set; }

        public string ReceiverId { get; set; }

        public string SenderId { get; set; }

        public string Description { get; set; }

        public bool IsAnonymous { get; set; }

        public int BoardId { get; set; }

        public virtual Board Board { get; set; }

        public DateTime CreationDate { get; set; }

        public bool IsPrivate { get; set; }

        public string Image { get; set; }

        public byte[] ImageArray
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Image))
                {
                    var base64Data = Regex.Match(Image, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                    return Convert.FromBase64String(base64Data);
                }

                return null;
            }
        }

        /// <summary>
        /// Image extension i.e ".jpg"
        /// </summary>
        public string ImageExtension { get; set; }
    }
}
