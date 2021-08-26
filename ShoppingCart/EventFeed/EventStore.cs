using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ShoppingCart.EventFeed
{
    public interface IEventStore
    {
        IEnumerable<Event> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber);
        void Raise(string eventName, object content);
    }

    public class EventStore : IEventStore
    {
        private static long _currentSequenceNumber;
        private static readonly IList<Event> Database = new List<Event>();

        public IEnumerable<Event> GetEvents(
            long firstEventSequenceNumber,
            long lastEventSequenceNumber)
            => Database
                .Where(e =>
                    e.SequenceNumber >= firstEventSequenceNumber &&
                    e.SequenceNumber <= lastEventSequenceNumber)
                .OrderBy(e => e.SequenceNumber);

        public void Raise(string eventName, object content)
        {
            var seqNumber = Interlocked.Increment(ref _currentSequenceNumber);
            Database.Add(
                new Event(
                    seqNumber,
                    DateTimeOffset.UtcNow,
                    eventName,
                    content));
        }
    }
}