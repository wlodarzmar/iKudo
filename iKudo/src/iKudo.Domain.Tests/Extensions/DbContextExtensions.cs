using iKudo.Domain.Model;
using System.Collections.Generic;
using System.Linq;

namespace iKudo.Domain.Tests
{
    public static class DbContextExtensions
    {
        public static void Fill(this KudoDbContext context, ICollection<JoinRequest> source)
        {
            context.AddRange(source);
            context.SaveChanges();
        }

        public static void Fill(this KudoDbContext context, ICollection<Board> source)
        {
            context.AddRange(source);
            context.SaveChanges();
        }

        public static void Fill(this KudoDbContext context, params Board[] boards)
        {
            Fill(context, boards.ToList());
        }

        public static void Fill(this KudoDbContext context, params Notification[] notifications)
        {

            Fill(context, notifications.ToList());
        }

        public static void Fill(this KudoDbContext context, ICollection<Notification> source)
        {
            context.AddRange(source);
            context.SaveChanges();
        }

        public static void Fill(this KudoDbContext context, ICollection<Kudo> source)
        {
            context.AddRange(source);
            context.SaveChanges();
        }

        public static void Fill(this KudoDbContext context, params Kudo[] kudos)
        {
            Fill(context, kudos.ToList());
        }

        public static void Fill(this KudoDbContext context, ICollection<User> source)
        {
            context.AddRange(source);
            context.SaveChanges();
        }

        public static void Fill(this KudoDbContext context, params User[] users)
        {
            Fill(context, users.ToList());
        }

        public static void Fill(this KudoDbContext context, ICollection<BoardInvitation> source)
        {
            context.AddRange(source);
            context.SaveChanges();
        }

        public static void Fill(this KudoDbContext context, params BoardInvitation[] invitations)
        {
            Fill(context, invitations.ToList());
        }
    }
}
