using EscapeFromDungeon.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromDungeon
{
    internal class Battle
    {
        public Monster Monster { get; set; }
        public Player Player { get; set; }
        public Message message { get; set; }

        private bool isDefending;

        public Action<bool> SetButtonEnabled;
        public Action<bool> SetMonsterVisible;


        public Battle(Player player, Message message) 
        {
            this.Player = player;
            this.message = message;
        }

        public async Task<Character> BattleLoopAsync()
        {
            if (Monster.Hp <= 0)
            {
                GameManager.gameMode = GameMode.BattleEnd;
                await message.ShowAsync($"{Monster.Name}を倒した！");
                SetMonsterVisible.Invoke(false);
                return Monster;
            }
            if (Player.Hp <= 0)
            {
                GameManager.gameMode = GameMode.BattleEnd;
                SetMonsterVisible.Invoke(false);
                return Player;
            }

            // 戦闘が続いているときだけボタンを表示
            if (GameManager.gameMode == GameMode.Battle)
            {
                //await message.ShowAsync($"コマンド？");
                SetButtonEnabled?.Invoke(true);
            }

            return null;
        }

        public async Task PlayerTurn(string command)
        {
            isDefending = false;

            switch (command)
            {
                case "Attack":
                    Monster.Hp -= Player.Attack;
                    await message.ShowAsync($"{Player.Name}の攻撃！{Monster.Name}に {Player.Attack} ダメージ！");
                    break;
                case "Defence":
                    isDefending = true;
                    await message.ShowAsync($"{Player.Name}は防御の体勢を取った！");
                    break;
                case "Heal":
                    //player.Heal();
                    await message.ShowAsync($"{Player.Name}は回復した！");
                    break;
                case "Escape":
                    GameManager.gameMode = GameMode.Escaped;
                    SetMonsterVisible.Invoke(false);
                    SetButtonEnabled.Invoke(false);
                    return;
            }

            if (Monster.Hp > 0)
            {
                await EnemyTurn();
            }
            else
            {
                await BattleLoopAsync();
            }
        }

        private async Task EnemyTurn()
        {
            if (isDefending)
            {
                await message.ShowAsync($"{Monster.Name}の攻撃！{Player.Name}は防御した！");
            }
            else
            {
                int damage = Monster.Attack;
                Player.Hp -= damage;
                await message.ShowAsync($"{Monster.Name}の攻撃！{Player.Name}に {damage} ダメージ！");
            }

            await BattleLoopAsync();
        }

    }//class Battle
}//namespace EscapeFromDungeon
