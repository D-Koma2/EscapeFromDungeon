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
        public List<Monster> monsterDatas { get; private set; }
        public MonsterData(string path)
        {
            monsterDatas = new List<Monster>();
            ReadFromCsv(path);
        }

        public void ReadFromCsv(string path)
        {
            var lines = File.ReadAllLines(path).Skip(1);//１行目スキップ

            foreach (var item in lines)
            {
                var cells = item.Split(',');

                var name = cells[0];
                var hp = int.Parse(cells[1]);
                var attack = int.Parse(cells[2]);
                var weak = (Weak)Enum.Parse(typeof(Weak), cells[3]);

                monsterDatas.Add(new Monster
                (
                    name,
                    hp,
                    attack,
                    weak
                ));
            }
        }

    }//class
}// namespace EscapeFromDungeon
