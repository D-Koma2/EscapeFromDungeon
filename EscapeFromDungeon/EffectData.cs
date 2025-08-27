using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromDungeon
{
    internal class EffectData
    {
        public Dictionary<EffectType, Effect> Effects { get; private set; } = new Dictionary<EffectType, Effect>();

        public void ReadFromCsv(string path)
        {
            var lines = File.ReadAllLines(path);

            foreach (var item in lines)
            {
                var cells = item.Split(',');

                var key = (EffectType)Enum.Parse(typeof(EffectType), cells[0]);
                var value = cells[1];
                var duration = int.Parse(cells[2]);

                //Effects.Add(new Effect
                //{
                //    Value = value,
                //    Duration = duration,
                //    EffectType = key
                //});
            }
        }

    }//class
}// namespace EscapeFromDungeon
