using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CleanTemplate.Core.SharedKernel
{
    public abstract class BaseDomainEvent
    {
        public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
    }
}
