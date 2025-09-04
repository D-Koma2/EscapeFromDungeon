using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromDungeon
{
    internal static class Const
    {
        public static readonly string mapCsv = "map.csv";
        public static readonly string eventCsv = "event.csv";
        public static readonly string monsterCsv = "monster.csv";
        public static readonly string itemCsv = "item.csv";

        public static readonly string[] separator = new[] { "\r\n", "\r", "\n" };
        public static readonly string title = "Escape\nFrom\nDungeon";
        public static readonly string gameOver = "Game Over";
        public static readonly string retry = "リトライ？";
        public static readonly string gameClear = "Thank You\nFor\nPlaying!!";

        public static readonly string commndMsg = "コマンド？";
        public static readonly string hpFullMsg = "HPは満タンだ！";

        public static readonly string attackLabelText = "[↑] 攻撃";
        public static readonly string defenceLabelText = "[←] 防御";
        public static readonly string healLabelText = "[→] 回復";
        public static readonly string escapeLabelText = "[↓] 逃げる";
        public static readonly string upMoveText = "[↑] 移動";
        public static readonly string leftMoveText = "[←] 移動";
        public static readonly string rightMoveText = "[→] 移動";
        public static readonly string downMoveText = "[↓] 移動";

        public static readonly string startMenu = "スタート";
        public static readonly string retryMenu = "リトライ？";
        public static readonly string exitMenu = "終了する";

        public const string potion = "ポーション";
        public const string curePoison = "毒消し草";
        public const string torch = "松明";

        public const string demon = "デーモン";
        public const string SlimeG = "強力スライム";
        public const string fireSlime = "炎スライム";
        public const string iceSlime = "氷スライム";
        public const string thunderSlime = "雷スライム";
        public const string fireSlimeG = "強力炎スライム";
        public const string iceSlimeG = "強力氷スライム";
        public const string thunderSlimeG = "強力雷スライム";

        public const string fireWepon = "炎の剣";
        public const string iceWepon = "氷の剣";
        public const string thunderWepon = "雷の剣";
        public const string holyWepon = "聖なる剣";
        public const string heavyWepon = "バールかな？";
        public const string superWepon = "エクスカリバール";

        public const string CommandAtk = "Attack";
        public const string CommandDef = "Defence";
        public const string CommandHeal = "Heal";
        public const string CommandEsc = "Escape";
    }
}
