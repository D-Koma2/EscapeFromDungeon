using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromDungeon
{
    public static class MonsterBehaviorRegistry
    {
        public static IMonsterBehavior GetBehavior(string monsterName)
        {
            return behaviors.TryGetValue(monsterName, out var behavior) ? behavior : new DefaultBehavior();
        }

        private static Dictionary<string, IMonsterBehavior> behaviors = new()
        {
                { Const.fireSlime, new SlimeBehavior() },
                { Const.iceSlime, new SlimeBehavior() },
                { Const.thunderSlime, new SlimeBehavior() },
                { Const.fireSlimeG, new SlimeGBehavior() },
                { Const.iceSlimeG, new SlimeGBehavior() },
                { Const.thunderSlimeG, new SlimeGBehavior() },
                { Const.demon, new DemonBehavior() }
        };
    }

    public class MonsterAction
    {
        public string Message { get; set; }
        public int Damage { get; set; }
        public bool SkipDamage { get; set; }
        public Shake shakeType { get; set; }

        public MonsterAction(string message, int damage, bool skipDamage = false, Shake shake = Shake.normal)
        {
            Message = message;
            Damage = damage;
            SkipDamage = skipDamage;
            shakeType = shake;
        }
    }

    public interface IMonsterBehavior
    {
        MonsterAction DecideAction(int turn, Monster monster, Player player);
    }

    public class DefaultBehavior : IMonsterBehavior
    {
        public MonsterAction DecideAction(int turn, Monster monster, Player player)
        {
            return new MonsterAction($"{monster.Name}の攻撃！{player.Name}は {monster.Attack} のダメージ！", monster.Attack, false);
        }
    }

    public class SlimeBehavior : IMonsterBehavior
    {
        public MonsterAction DecideAction(int turn, Monster monster, Player player)
        {
            if (turn % 4 == 3)
            {
                int damage = monster.Attack * 3;
                return new MonsterAction($"{monster.Name}の強力な攻撃！${player.Name}は {damage} の大ダメージ！", damage, false, Shake.weak);
            }
            else if (turn % 4 == 2)
            {
                return new MonsterAction($"{monster.Name}はプルプルしている！", 0, true);
            }
            else
            {
                return new MonsterAction($"{monster.Name}の攻撃！{player.Name}は {monster.Attack} のダメージ！", monster.Attack, false);
            }
        }
    }

    public class SlimeGBehavior : IMonsterBehavior
    {
        public MonsterAction DecideAction(int turn, Monster monster, Player player)
        {
            if (turn % 4 == 3)
            {
                int damage = monster.Attack * 3;
                return new MonsterAction($"{monster.Name}の強力な攻撃！${player.Name}は {damage} の大ダメージ！", damage, false, Shake.weak);
            }
            else
            {
                return new MonsterAction($"{monster.Name}の攻撃！${player.Name}は {monster.Attack} のダメージ！", monster.Attack, false);
            }
        }
    }

    public class DemonBehavior : IMonsterBehavior
    {
        public MonsterAction DecideAction(int turn, Monster monster, Player player)
        {
            if (turn % 5 == 4)
            {
                int damage = monster.Attack * 3;
                return new MonsterAction($"{monster.Name}の強力な攻撃！${player.Name}は {damage} の大ダメージ！", damage, false, Shake.weak);
            }
            else if (turn == 3)
            {
                return new MonsterAction($"{monster.Name}は力をためている！", 0, true);
            }
            else
            {
                return new MonsterAction($"{monster.Name}の攻撃！${player.Name}は {monster.Attack} のダメージ！", monster.Attack, false);
            }
        }
    }

}
