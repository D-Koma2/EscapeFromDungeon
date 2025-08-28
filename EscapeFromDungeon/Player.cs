using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromDungeon
{
    public enum Status
    {
        Normal,
        Poison,
        Stun
    }

    public enum Weak
    {
        None,
        Fire,
        Ice,
        Thunder
    }

    class Character
    {
        private int hp;

        public string Name { get; set; }
        public int Hp
        {
            get => hp;
            set
            {
                if (hp < 0) hp = 0;
                hp = value;
            }
        }
        public int MaxHp { get; set; }
        public int Attack { get; set; }
        public int Defence { get; set; }
        public int Speed { get; set; }

        public Status Status { get; set; }

        public List<Effect> Effects { get; set; }

    }

    internal class Player : Character
    {
        private int limit = 999;

        public enum Direction { Up, Down, Left, Right }

        public Direction Dir { get; set; } = Direction.Up;

        public Image playerImage { get; set; } = Properties.Resources.Up;

        public int Lv { get; private set; } = 1;

        public int Exp { get; set; }

        public int Limit
        {
            get => limit;

            set
            {
                if (limit < 0) limit = 0;
                limit = value;
            }
        }

        public List<Item> Inventry { get; private set; }

        public void GetPlayerImage(Direction dir)
        {
            switch (dir)
            {
                case Player.Direction.Down:
                    Dir = Direction.Down;
                    playerImage = Properties.Resources.Down;
                    break;
                case Player.Direction.Left:
                    Dir = Direction.Left;
                    playerImage = Properties.Resources.Left;
                    break;
                case Player.Direction.Right:
                    Dir = Direction.Right;
                    playerImage = Properties.Resources.Right;
                    break;
                default:
                    Dir = Direction.Up;
                    playerImage = Properties.Resources.Up;
                    break;
            }
        }
    }

    class Enemy : Character
    {
        public Weak Weak { get; set; }
    }
}
