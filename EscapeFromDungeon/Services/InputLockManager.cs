using EscapeFromDungeon;

namespace EscapeFromDungeon.Services
{
    internal class InputLockManager
    {
        private static InputLockManager? _instance;
        public static InputLockManager Instance => _instance ??= new InputLockManager();

        private static bool _isInputLocked = false;
        private static DateTime _inputUnlockTime;

        // 指定秒間入力をロックするときに呼ぶトリガー
        public static void InputLockStart(float time)
        {
            _isInputLocked = true;
            _inputUnlockTime = DateTime.Now.AddSeconds(time);
        }
        
        // ロックしたいメソッドをこの戻り値でチェック
        public static bool IsInputLocked()
        {
            bool result = _isInputLocked && DateTime.Now < _inputUnlockTime;
            if (_isInputLocked && DateTime.Now >= _inputUnlockTime) _isInputLocked = false;
            return result;
        }
    }
}
