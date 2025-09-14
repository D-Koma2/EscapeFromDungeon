using EscapeFromDungeon.Constants;
using EscapeFromDungeon.Properties;

namespace EscapeFromDungeon.Models
{
    internal static class EventData
    {
        public static Dictionary<string, Event> Dict { get; private set; } = new Dictionary<string, Event>();

        public static void ReadFromCsv(string path)
        {
            //var lines = File.ReadAllLines(path).Skip(1).ToArray();//１行目スキップ
            var lines = Resources.Event.Split(Const.separator, StringSplitOptions.None).Skip(1).ToArray();//１行目スキップ
            if (lines.Last().Trim() == "") lines = lines.Take(lines.Length - 1).ToArray();//最終行が空行なら削除

            foreach (var item in lines)
            {
                var cells = item.Split(',');

                try
                {
                    var id = cells[0];
                    var eventType = (EventType)Enum.Parse(typeof(EventType), cells[1]);
                    var word = cells[2];

                    if (!Dict.ContainsKey(id))
                    {
                        Dict.Add(id, new Event
                        (
                            id,
                            eventType,
                            word
                        ));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"EventData読み込みエラー: {ex.Message} 行: {item}");
                }
            }
        }
    }
}
