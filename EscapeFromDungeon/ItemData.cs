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
        public List<Item> ItemDatas { get; private set; }

        public ItemData(string path)
        {
            ItemDatas = new List<Item>();
            ReadFromCsv(path);
        }

        public void ReadFromCsv(string path)
        {
            var lines = File.ReadAllLines(path).Skip(1);//１行目スキップ

            foreach (var item in lines)
            {
                var cells = item.Split(',');

                var name = cells[0];
                var description = cells[1];

                ItemDatas.Add(new Item
                (
                    name,
                    description
                ));
            }
        }

    }//class
}// namespace EscapeFromDungeon
