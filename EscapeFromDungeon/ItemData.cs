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
        public List<Item> ItemDatas { get; private set; } = new List<Item>();

        public void ReadFromCsv(string path)
        {
            var lines = File.ReadAllLines(path);

            foreach (var item in lines)
            {
                var cells = item.Split(',');

                var itemType = (ItemType)Enum.Parse(typeof(ItemType), cells[0]);
                var name = cells[1];
                var description = cells[2];

                var effects = new List<string>();
                for (int j = 3; j < cells.Length; j++) effects.Add(cells[j]);

                ItemDatas.Add(new Item
                {
                    ItemType = itemType,
                    Name = name,
                    Description = description,
                    Effects = effects
                });
            }
        }

    }//class
}// namespace EscapeFromDungeon
