using iKudo.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using iKudo.Domain.Enums;
using iKudo.Domain.Model;

namespace iKudo.Domain.Logic
{
    public class KudosManager : IManageKudos
    {
        public IEnumerable<KudoType> GetTypes()
        {
            return Enum.GetValues(typeof(KudoType)).Cast<KudoType>();
        }

        public Kudo Insert(Kudo kudo)
        {
            throw new NotImplementedException();
        }
    }
}
