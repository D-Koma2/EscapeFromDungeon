using EscapeFromDungeon.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        public Status Status { get; private set; } = Status.Normal;

        public Action? FlashByDamage;

        public Character(string name, int hp, int attack)
        {
            Name = name;
            MaxHp = hp;
            Hp = hp;
            Attack = attack;
        }

        public void TakeDamage(int damage)
        {
            Hp -= damage;
            if (Hp < 0) Hp = 0;
            FlashByDamage?.Invoke();
        }

        public void HealStatus() => Status = Status.Normal;
        public void TakePoison() => Status = Status.Poison;
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

        public void GetItem(string name, string dsc)
        {
            Inventry.Add(new Item(name, dsc));
        }

        public void UseItem(string itemName)
        {
            var usedItem = Inventry.Find(item => item.Name == itemName);
            if (usedItem != null) Inventry.Remove(usedItem);
        }

        public int GetItemCount(string itemName)
        {
            return Inventry.Count(item => item.Name == itemName);
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

        public void Init(int hp, int limit)
        {
            Hp = MaxHp;
            Hp = hp;
            Limit = limit;
            HealStatus();
            Inventry.Clear();
            Dir = Direction.Up;
            SetDirectionImage(Dir);
        }
    }

    class Monster : Character
    {
        public Weak Weak { get; set; } = Weak.None;

        public string ImageName {  get; set; }
        public Monster(string name, int hp, int attack, Weak weak, string image) : base(name, hp, attack) 
        {
            Weak = weak;
            ImageName = image;
        }
    }
}
