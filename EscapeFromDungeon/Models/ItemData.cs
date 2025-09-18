using EscapeFromDungeon.Constants;
using EscapeFromDungeon.Properties;

namespace EscapeFromDungeon.Models
{
    internal static class ItemData
    {
        public static Dictionary<string, Item> Dict { get; private set; } = new Dictionary<string, Item>();

        public static void ReadData(string path)
        {
            //var lines = File.ReadAllLines(path).Skip(1).ToArray();//１行目スキップ
            var lines = Resources.Item.Split(Const.separator, StringSplitOptions.None).Skip(1).ToArray();//１行目スキップ
            if (lines.Last().Trim() == "") lines = lines.Take(lines.Length - 1).ToArray();//最終行が空行なら削除

            foreach (var item in lines)
            {
                var cells = item.Split(',');

                try
                {
                    var name = cells[0];
                    var description = cells[1];

                    if (!Dict.ContainsKey(name))
                    {
                        Dict.Add(name, new Item
                        (
                            name,
                            description
                        ));
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ItemData読み込みエラー: {ex.Message} 行: {item}");
                }
            }
        }

    }//class
}// namespace EscapeFromDungeon
