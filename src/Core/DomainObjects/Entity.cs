using System;
using System.Collections.Generic;
using System.Text;
using Core.Messages;

namespace Core.DomainObjects
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        private List<Event> _notifications;
        public IReadOnlyCollection<Event> Notifications => _notifications?.AsReadOnly();

        protected Entity()
        {
            Id = Guid.NewGuid();
        }

        public void AddEvent(Event @event)
        {
            _notifications ??= new List<Event>();
            _notifications.Add(@event);
        }

        public void RemoveEvent(Event @event)
        {
            _notifications?.Remove(@event);
        }

        public void ClearEvents()
        {
            _notifications?.Clear();
        }
    }
}
