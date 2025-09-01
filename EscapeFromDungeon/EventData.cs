using EscapeFromDungeon.Properties;
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
        public Dictionary<string,Event> Dict { get; private set; } = new Dictionary<string, Event>();

        public EventData(string path)
        {
            //var lines = File.ReadAllLines(path).Skip(1);//１行目スキップ
            var lines = Resources.Event.Split(Const.separator, StringSplitOptions.None).Skip(1).ToArray();//１行目スキップ
            if (lines.Last().Trim() == "") lines = lines.Take(lines.Length - 1).ToArray();//最終行が空行なら削除

            foreach (var item in lines)
            {
                var cells = item.Split(',');

                var id = cells[0];
                var eventType = (EventType)Enum.Parse(typeof(EventType), cells[1]);
                var word = cells[2];

                Dict.Add(id, new Event
                (
                    id,
                    eventType,
                    word
                ));
            }
        }
    }
}
