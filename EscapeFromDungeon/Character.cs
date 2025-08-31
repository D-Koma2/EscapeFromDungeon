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
    }

    public enum Weak
    {
        None,
        Fire,
        Ice,
        Thunder,
        Heavy,
        Holy
    }

    class Character
    {
        private int hp;

        public int Hp
        {
            get => hp;
            set
            {
                hp = value;
                if (hp < 0) hp = 0;
                if (hp >= MaxHp) hp = MaxHp;
            }
        }
        public int MaxHp { get; private set; }
        public string Name { get; private set; }
        public int Attack { get; private set; }
        public Status Status { get; set; } = Status.Normal;

        public Character(string name, int hp, int attack)
        {
            Name = name;
            MaxHp = hp;
            Hp = hp;
            Attack = attack;
        }
    }

    internal class Player : Character
    {
        private int limit;
        public List<Item> Inventry { get; set; }

        public Player(string name, int hp, int attack, int limit) : base(name, hp, attack) 
        {
            Inventry = new List<Item>();
            this.Limit = limit;
        }

        public enum Direction { Up, Down, Left, Right }

        public Direction Dir { get; set; } = Direction.Up;

        public Image playerImage { get; set; } = Properties.Resources.Up;

        public int Limit
        {
            get => limit;
            set
            {
                limit = value;
                if (limit < 0) limit = 0;
            }
        }

        public void Heal(int amount)
        {
            Hp += amount;
            if (Hp > MaxHp) Hp = MaxHp;
        }

        public void UseItem(string itemName)
        {
            var usedItem = Inventry.Find(item => item.Name == itemName);
            if (usedItem != null) Inventry.Remove(usedItem);
        }

        public void SetDirectionImage(Direction dir)
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

    class Monster : Character
    {
        public Weak Weak { get; set; } = Weak.None;
        public Monster(string name, int hp, int attack, Weak weak) : base(name, hp, attack) 
        {
            Weak = weak;
        }
    }
}
