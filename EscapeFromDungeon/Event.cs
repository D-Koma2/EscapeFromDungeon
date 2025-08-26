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
        EnemyEncount,
        GameStart,
        GameOver,
        GameClear
    }

    internal class Event
    {
        public EventType EventType {  get; set; }
        public string[] Messages {  get; set; }
        public List<Effect> Effects { get; set; }
    }
}
