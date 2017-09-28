using iKudo.Domain.Interfaces;
using System;

namespace iKudo.Domain.Logic
{
    public class DefaultTimeProvider : IProvideTime
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
