import { User } from "./user";
import { Board } from "./board";

export class Notification {
    id: number;
    type: number;
    typeName: string;
    sender: User;
    receiver: User;
    board: Board;
    creationDate: Date;
    readDate: Date;
    isRead: boolean;

    title: string;
    message: string;
}


 //public int Id { get; set; }

 //       public NotificationTypes Type { get; set; }

 //       public string TypeName => Type.ToString().ToLower();

 //       [Required]
 //       public string SenderId { get; set; }

 //       public UserDto Sender { get; set; }

 //       [Required]
 //       public string ReceiverId { get; set; }

 //       public UserDto Receiver { get; set; }

 //       public DateTime CreationDate { get; set; }

 //       public int? BoardId { get; set; }

 //       public DateTime? ReadDate { get; set; }

 //       public BoardDto Board { get; set; }

 //       public bool IsRead => ReadDate.HasValue;