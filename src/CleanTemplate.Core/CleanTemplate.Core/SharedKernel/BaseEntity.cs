using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CleanTemplate.Core.SharedKernel
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        public List<BaseDomainEvent> Events = new List<BaseDomainEvent>();
    }
}
