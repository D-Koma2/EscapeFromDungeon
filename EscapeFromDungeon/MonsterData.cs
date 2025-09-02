using EscapeFromDungeon.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromDungeon
{
    internal class MonsterData
    {
        public Dictionary<string,Monster> Dict { get; private set; } = new Dictionary<string, Monster>();
        public MonsterData(string path)
        {
            var lines = File.ReadAllLines(path).Skip(1).ToArray();//１行目スキップ
            //var lines = Resources.Monster.Split(Const.separator, StringSplitOptions.None).Skip(1).ToArray();//１行目スキップ
            if (lines.Last().Trim() == "") lines = lines.Take(lines.Length - 1).ToArray();//最終行が空行なら削除

            foreach (var item in lines)
            {
                var cells = item.Split(',');

                var name = cells[0];
                var hp = int.Parse(cells[1]);
                var attack = int.Parse(cells[2]);
                var weak = (Weak)Enum.Parse(typeof(Weak), cells[3]);
                var image = cells[4]; 

                Dict.Add(name,new Monster
                (
                    name,
                    hp,
                    attack,
                    weak,
                    image
                ));
            }
        }

    }//class
}// namespace EscapeFromDungeon
