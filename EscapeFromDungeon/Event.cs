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
        ItemGet,
        Heal,
        Trap,
        EnemyEncount
    }

    internal class Event
    {
        public string Id { get; set; }
        public EventType EventType { get; set; }
        public string Word { get; set; }

        public Event(string id, EventType eventType, string word)
        {
            Id = id;
            EventType = eventType;
            Word = word;
        }
    }
}
