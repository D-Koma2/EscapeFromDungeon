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

        public void ReadFromCsv(string path)
        {
            var lines = File.ReadAllLines(path);

            for (int i = 0; i < lines.Length; i++)
            {
                var cells = lines[i].Split(',');

                for (int j = 0; j < cells.Length; j++)
                {
                    //var effects = new Effect();
                    //if(j >= 1 )
                    //{

                    //}


                    //ItemDatas.Add(new Item { ItemType = (ItemType)Enum.Parse(typeof(ItemType), cells[0]), 
                    //                            Name = cells[1], 
                    //                        Effects = effects)
                    //});
                }
            }
        }
    }
}
