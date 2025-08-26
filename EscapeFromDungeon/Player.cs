using System;
using System.Collections.Generic;
using System.Linq;
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

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
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
        public int Lv { get; set; }

        public int Exp {  get; set; }

        public Direction Dir { get; set; }

        public List<Item> Inventry { get; set; }
    }

    class Enemy : Character
    {
        public Weak Weak { get; set; }
    }
}
