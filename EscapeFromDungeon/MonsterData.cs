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
        public List<Character> monsterDatas { get; private set; }

        public void ReadFromCsv(string path)
        {
            var lines = File.ReadAllLines(path).Skip(1);//１行目スキップ
        }
    }
}
