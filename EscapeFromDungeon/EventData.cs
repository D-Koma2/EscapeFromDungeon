using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromDungeon
{
    internal class EventData
    {
        public List<Event> eventDatas { get; private set; }

        public EventData(string path)
        {
            eventDatas = new List<Event>();
            ReadFromCsv(path);
        }

        public void ReadFromCsv(string path)
        {
            var lines = File.ReadAllLines(path).Skip(1);//１行目スキップ

            foreach (var item in lines)
            {
                var cells = item.Split(',');

                var id = cells[0];
                var eventType = (EventType)Enum.Parse(typeof(EventType), cells[1]);
                var word = cells[2];

                eventDatas.Add(new Event
                (
                    id,
                    eventType,
                    word
                ));
            }
        }
    }
}
