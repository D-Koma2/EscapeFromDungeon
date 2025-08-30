using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromDungeon
{
    public enum EventType
    {
        Message,
        Hint,
        ItemGet,
        Heal,
        Trap,
        Encount
    }

    internal class Event
    {
        public string Id { get; private set; }
        public EventType EventType { get; private set; }
        public string Word { get; private set; }

        public Event(string id, EventType eventType, string word)
        {
            Id = id;
            EventType = eventType;
            Word = word;
        }
    }
}
