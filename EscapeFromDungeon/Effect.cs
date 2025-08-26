using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromDungeon
{
    public enum EffectType
    {
        Heal,
        AttackUp,
        DefenceUp,
        SpeedUp,
        LevelUp
    }
    
    internal class Effect
    {
        // エフェクトタイプ
        public EffectType EffectType {  get; set; }
        // 効果値
        public int Value { get; set; }
        // 効果時間
        public int Duration {  get; set; }
    }
}
