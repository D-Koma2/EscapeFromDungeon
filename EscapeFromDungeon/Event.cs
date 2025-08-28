using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromDungeon
{
    public enum EventType
    {
        Message,//ヒント
        ItemGet,
        Effect,//回復、毒、ダメージなど
        Trap,
        EnemyEncount,
        GameStart,
        GameOver,
        GameClear
    }

    internal class Event
    {
        public EventType EventType { get; set; }
        public string Message { get; set; }
        public List<string> Effects { get; set; }
    }
}
