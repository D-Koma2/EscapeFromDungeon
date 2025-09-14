using EscapeFromDungeon.Constants;
using EscapeFromDungeon.Models;

namespace EscapeFromDungeon.Behaviors
{
    public static class PlayerAttackRegistry
    {
        private static Dictionary<string, IPlayerAttack> behaviors = new()
        {
            { Const.superWepon, new SuperWeponAttack() },
            { Const.fireWepon, new FireWeponAttack() },
            { Const.iceWepon, new IceWeponAttack() },
            { Const.thunderWepon, new ThunderWeponAttack() },
            { Const.heavyWepon, new HeavyWeponAttack() },
            { Const.holyWepon, new HolyWeponAttack() },
        };

        public static IPlayerAttack GetBehavior(string itemName)
        {
            return behaviors.TryGetValue(itemName, out var behavior) ? behavior : new DefaultAttack();
        }
    }

    public class PlayerAttack
    {
        public string Message { get; set; }

        public int Damage { get; set; }
        public Shake shakeType { get; set; }

        public PlayerAttack(string message, int damage, Shake shake = Shake.normal)
        {
            Message = message;
            Damage = damage;
            shakeType = shake;
        }
    }

    public interface IPlayerAttack
    {
        PlayerAttack DecideAction(Player player, Monster monster, string itemName);
    }

    public class DefaultAttack : IPlayerAttack
    {
        public PlayerAttack DecideAction(Player player, Monster monster, string itemName)
        {
            return new PlayerAttack($"{player.Name}の攻撃！{monster.Name}は {player.Attack} のダメージ！", player.Attack);
        }
    }

    public class SuperWeponAttack : IPlayerAttack
    {
        public PlayerAttack DecideAction(Player player, Monster monster, string itemName)
        {
            int damage = player.Attack * 4;
            return new PlayerAttack($"{player.Name}は{itemName}で強力な攻撃！${monster.Name}は {damage} の大ダメージ！", damage, Shake.weak);
        }
    }

    public class FireWeponAttack : IPlayerAttack
    {
        public PlayerAttack DecideAction(Player player, Monster monster, string itemName)
        {
            int damage = player.Attack * 3;
            return new PlayerAttack($"{player.Name}は{itemName}で炎属性攻撃！${monster.Name}は {damage} の大ダメージ！", damage, Shake.weak);
        }
    }

    public class IceWeponAttack : IPlayerAttack
    {
        public PlayerAttack DecideAction(Player player, Monster monster, string itemName)
        {
            int damage = player.Attack * 3;
            return new PlayerAttack($"{player.Name}は{itemName}で氷属性攻撃！${monster.Name}は {damage} の大ダメージ！", damage, Shake.weak);
        }
    }

    public class ThunderWeponAttack : IPlayerAttack
    {
        public PlayerAttack DecideAction(Player player, Monster monster, string itemName)
        {
            int damage = player.Attack * 3;
            return new PlayerAttack($"{player.Name}は{itemName}で雷属性攻撃！${monster.Name}は {damage} の大ダメージ！", damage, Shake.weak);
        }
    }

    public class HeavyWeponAttack : IPlayerAttack
    {
        public PlayerAttack DecideAction(Player player, Monster monster, string itemName)
        {
            int damage = player.Attack * 3;
            return new PlayerAttack($"{player.Name}は{itemName}で重たい攻撃！${monster.Name}は {damage} の大ダメージ！", damage, Shake.weak);
        }
    }

    public class HolyWeponAttack : IPlayerAttack
    {
        public PlayerAttack DecideAction(Player player, Monster monster, string itemName)
        {
            int damage = player.Attack * 3;
            return new PlayerAttack($"{player.Name}は{itemName}で聖属性攻撃！${monster.Name}は {damage} の大ダメージ！", damage, Shake.weak);
        }
    }
}
