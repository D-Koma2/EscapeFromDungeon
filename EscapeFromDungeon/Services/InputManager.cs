using EscapeFromDungeon.Constants;
using EscapeFromDungeon.Core;
using EscapeFromDungeon.Models;

namespace EscapeFromDungeon.Services
{
    internal class InputManager
    {
        private static InputManager? _instance;
        public static InputManager Instance => _instance ??= new InputManager();

        public static Func<string, Keys, Task>? MoveKeyPressed;
        public static Action<string>? ItemKeyPressed;
        public static Action<Keys>? Move;

        public static void KeyInput(Keys keyCode)
        {
            if (GameStateManager.Instance.CurrentMode is GameMode.Explore)
            {
                if (keyCode is (Keys.Up or Keys.Down or Keys.Left or Keys.Right))
                {
                    // メッセージ表示中はメッセージ処理優先
                    if (DrawMessage.IsMessageShowing)
                    {
                        DrawMessage.InputKey();
                        return;
                    }

                    Move?.Invoke(keyCode);
                }
                else if (keyCode is Keys.P)
                {
                    ItemKeyPressed?.Invoke(Const.potion);
                }
                else if (keyCode is Keys.O)
                {
                    ItemKeyPressed?.Invoke(Const.curePoison);
                }
                else if (keyCode is Keys.I)
                {
                    ItemKeyPressed?.Invoke(Const.torch);
                }
                //else if (keyCode is Keys.V)//デバッグ用
                //{
                //    Map.ToggleVisionEnable();
                //}
            }
            else if (GameStateManager.Instance.CurrentMode is GameMode.Battle)
            {
                if (keyCode is Keys.Up)
                {
                    MoveKeyPressed?.Invoke(Const.CommandAtk, Keys.Up);
                }
                else if (keyCode is Keys.Down)
                {
                    MoveKeyPressed?.Invoke(Const.CommandEsc, Keys.Down);
                }
                else if (keyCode is Keys.Left)
                {
                    MoveKeyPressed?.Invoke(Const.CommandDef, Keys.Left);
                }
                else if (keyCode is Keys.Right)
                {
                    MoveKeyPressed?.Invoke(Const.CommandHeal, Keys.Right);
                }
            }
        }
    }
}
