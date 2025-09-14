using System;

namespace EscapeFromDungeon.Models
{
    public enum EventType
    {
        Message,
        Hint,
        ItemGet,
        Heal,
        Trap,
        Encount,
        GameClear
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
