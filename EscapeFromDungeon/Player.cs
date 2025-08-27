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
        public string Name { get; set; }
        public int Hp { get; set; }
        public int Attack { get; set; }
        public int Defence { get; set; }
        public int Speed { get; set; }

        public Status Status { get; set; }

        public List<Effect> Effects { get; set; }
    }

    internal class Player : Character
    {
        public enum Direction { Up, Down, Left, Right }

        public Direction Dir { get; set; } = Direction.Up;

        public int Lv { get; private set; } = 1;

        public int Exp { get; set; }

        public List<Item> Inventry { get; private set; }

        //public Image DirectionImage()
        //{
        //    switch (Dir)
        //    {
        //        case Player.Direction.Down:
        //            return Properties.Resources.PlayerDown;
        //        case Player.Direction.Left:
        //            return Properties.Resources.PlayerLeft;
        //        case Player.Direction.Right:
        //            return Properties.Resources.PlayerRight;
        //        default:
        //            return Properties.Resources.PlayerUp;
        //    }
        //}
    }

    class Enemy : Character
    {
        public Weak Weak { get; set; }
    }
}
