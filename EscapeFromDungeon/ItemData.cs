using EscapeFromDungeon.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromDungeon
{
    internal class ItemData
    {
        public Dictionary<string, Item> Dict { get; private set; } = new Dictionary<string, Item>();

        public ItemData(string path)
        {
            var lines = File.ReadAllLines(path).Skip(1).ToArray();//１行目スキップ
            //var lines = Resources.Item.Split(Const.separator, StringSplitOptions.None).Skip(1).ToArray();//１行目スキップ
            if (lines.Last().Trim() == "") lines = lines.Take(lines.Length - 1).ToArray();//最終行が空行なら削除

            foreach (var item in lines)
            {
                var cells = item.Split(',');

                var name = cells[0];
                var description = cells[1];

                Dict.Add(name, new Item
                (
                    name,
                    description
                ));
            }
        }

    }//class
}// namespace EscapeFromDungeon
