using iKudo.Domain.Interfaces;
using System;

namespace iKudo.Domain.Logic
{
    public class DefaultTimeProvider : ITimeProvider
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
