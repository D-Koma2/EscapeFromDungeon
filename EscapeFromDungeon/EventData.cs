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

        public void ReadFromCsv(string path)
        {
            var lines = File.ReadAllLines(path).Skip(1);//１行目スキップ

            foreach (var item in lines)
            {
                var cells = item.Split(',');

                var eventType = (EventType)Enum.Parse(typeof(EventType), cells[0]);
                var message = cells[1];
                var description = cells[2];

                var effects = new List<string>();
                for (int j = 3; j < cells.Length; j++) effects.Add(cells[j]);

                eventDatas.Add(new Event
                {
                    EventType = eventType,
                    Message = message,
                    Effects = effects
                });
            }
        }
    }
}
