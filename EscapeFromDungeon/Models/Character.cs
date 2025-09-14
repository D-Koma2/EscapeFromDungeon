using EscapeFromDungeon.Behaviors;

namespace EscapeFromDungeon.Models
{
    public enum Status
    {
        Normal,
        Poison,
    }

    [Flags]
    public enum Weak
    {
        None = 0,
        Fire = 1 << 0, // 1
        Ice = 1 << 1, // 2
        Thunder = 1 << 2, // 4
        Heavy = 1 << 3, // 8
        Holy = 1 << 4  // 16
    }

    public class Character
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

    public class Player : Character
    {
        public List<Item> Inventry { get; set; }

        public Player(string name, int hp, int attack, int limit) : base(name, hp, attack) 
        {
            Inventry = new List<Item>();
            Limit = limit;
        }

        public enum Direction { Up, Down, Left, Right }

        public Direction Dir { get; set; } = Direction.Up;

        public Image playerImage { get; private set; } = Properties.Resources.Up;

        private int limit;
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
                case Direction.Down:
                    Dir = Direction.Down;
                    playerImage = Properties.Resources.Down;
                    break;
                case Direction.Left:
                    Dir = Direction.Left;
                    playerImage = Properties.Resources.Left;
                    break;
                case Direction.Right:
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

    public class Monster : Character
    {
        public Weak Weak { get; set; } = Weak.None;

        public string ImageName {  get; set; }

        public IMonsterBehavior behavior { get; set; }
        public Monster(string name, int hp, int attack, Weak weak, string image, IMonsterBehavior behavior) : base(name, hp, attack) 
        {
            Weak = weak;
            ImageName = image;
            this.behavior = behavior;
        }
    }
}
