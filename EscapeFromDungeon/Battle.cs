using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromDungeon
{
    internal class Battle
    {
        private Monster monster;
        private Player player;
        private Message message;
        private List<Character> turnOrder;

        public Battle(Monster monster, Player player, Message message)
        {
            turnOrder = new List<Character> { player, monster };
            this.message = message;
        }

        public Character BattleLoop()
        {
            while (!turnOrder.Exists(x => x.Hp == 0))
            {
                message.Show($"{turnOrder[0].Name}のターンです。");
                break;
            }

            return turnOrder.Find(x => x.Hp > 0);
        }
    }
}
