using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iKudo.Dtos
{
    public class BoardDto
    {
        public int? Id { get; set; }

        public string CreatorId { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 3)]
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? ModificationDate { get; set; }

        public bool? IsPrivate { get; set; }

        public IEnumerable<UserBoardDto> UserBoards { get; set; }
    }
}
