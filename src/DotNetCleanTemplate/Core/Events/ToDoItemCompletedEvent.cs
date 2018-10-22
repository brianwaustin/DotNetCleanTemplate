using DotNetCleanTemplate.Core.Entity;
using DotNetCleanTemplate.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCleanTemplate.Core.Events
{
    public class ToDoItemCompletedEvent : BaseDomainEvent
    {
        public ToDoItem CompletedItem { get; set; }

        public ToDoItemCompletedEvent(ToDoItem completedItem)
        {
            CompletedItem = completedItem;
        }
    }
}
