using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromDungeon
{
    public enum ItemType
    {
        UseItem,
        Weapon,
        Armor,
        keyItem
    }

    internal class Item
    {
        public ItemType ItemType { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public List<string> Effects {  get; set; }
    }
}
