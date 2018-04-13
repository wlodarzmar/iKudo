using iKudo.Domain.Model;
using System.Collections.Generic;

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

        public static void Fill(this KudoDbContext context, ICollection<User> source)
        {
            context.AddRange(source);
            context.SaveChanges();
        }
    }
}
