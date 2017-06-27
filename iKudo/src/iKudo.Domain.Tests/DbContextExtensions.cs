using iKudo.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
