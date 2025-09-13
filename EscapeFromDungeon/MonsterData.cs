
namespace EscapeFromDungeon
{
    internal static class MonsterData
    {
        public static Dictionary<string, Monster> Dict { get; private set; } = new Dictionary<string, Monster>();

        private static Dictionary<string, IMonsterBehavior> behaviors = new()
        {
            { "Default", new DefaultBehavior() },
            { "Slime", new SlimeBehavior() },
            { "SlimeG", new SlimeGBehavior() },
            { "SlimeS", new SlimeSBehavior() },
            { "Demon", new DemonBehavior() }
        };

        public static void ReadFromCsv(string path)
        {
            var lines = File.ReadAllLines(path).Skip(1).ToArray();//１行目スキップ
            //var lines = Resources.Monster.Split(Const.separator, StringSplitOptions.None).Skip(1).ToArray();//１行目スキップ
            if (lines.Last().Trim() == "") lines = lines.Take(lines.Length - 1).ToArray();//最終行が空行なら削除

            foreach (var item in lines)
            {
                var cells = item.Split(',');

                try
                {
                    var name = cells[0];
                    var hp = int.Parse(cells[1]);
                    var attack = int.Parse(cells[2]);
                    var weak = (Weak)Enum.Parse(typeof(Weak), cells[3]);
                    var image = cells[4];
                    var behaviorType = behaviors.TryGetValue(cells[5], out var behavior) ? behavior: new DefaultBehavior();

                    if (!Dict.ContainsKey(name))
                    {
                        Dict.Add(name, new Monster
                        (
                            name,
                            hp,
                            attack,
                            weak,
                            image,
                            behaviorType
                        ));
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"MonsterData読み込みエラー: {ex.Message} 行: {item}");
                }
            }
        }

    }//class
}// namespace EscapeFromDungeon
