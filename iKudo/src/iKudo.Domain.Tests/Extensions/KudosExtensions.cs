using iKudo.Domain.Enums;
using iKudo.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace iKudo.Domain.Tests.Extensions
{
    public static class KudosExtensions
    {
        public static Kudo WithStatus(this Kudo kudo, KudoStatus status)
        {
            kudo.Status = status;
            return kudo;
        }
    }
}
