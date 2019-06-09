
using CleanTemplate.Core.Intefaces;
using CleanTemplate.Core.SharedKernel;

namespace CleanArchitecture.Tests
{
    public class NoOpDomainEventDispatcher : IDomainEventDispatcher
    {
        public void Dispatch(BaseDomainEvent domainEvent) { }
    }
}
