using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromDungeon
{
    internal static class Const
    {
        public static readonly string[] separator = new[] { "\r\n", "\r", "\n" };
        public static readonly string title = "Escape From Dungeon";
        public static readonly string gameOver = "Game Over";

        public const string potion = "ポーション";
        public const string curePoison = "毒消し草";
        public const string torch = "松明";

        public const string demon = "デーモン";
        public const string SlimeG = "強そうなスライム";
        public const string fireSlime = "炎スライム";
        public const string iceSlime = "氷スライム";
        public const string thunderSlime = "雷スライム";
        public const string fireSlimeG = "強そうな炎スライム";
        public const string iceSlimeG = "強そうな氷スライム";
        public const string thunderSlimeG = "強そうな雷スライム";

        public const string fireWepon = "炎の剣";
        public const string iceWepon = "氷の剣";
        public const string thunderWepon = "雷の剣";
        public const string holyWepon = "聖なる剣";
        public const string hitWepon = "おおきづち";

        public const string commndMsg = "コマンド？";
        public const string hpFullMsg = "HPは満タンだ！";

        public const string CommandAtk = "Attack";
        public const string CommandDef = "Defence";
        public const string CommandHeal = "Heal";
        public const string CommandEsc = "Escape";
    }
}
