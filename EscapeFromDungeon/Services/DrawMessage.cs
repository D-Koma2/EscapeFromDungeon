using EscapeFromDungeon.Core;

namespace EscapeFromDungeon.Services
{
    internal static class DrawMessage
    {
        private const int _timerInterval = 16;
        private static bool _isMessageCompleted = false;// メッセージが完了したかどうか
        private static string _fullMessage = "";// フルメッセージ（改行を含む）
        private static string _currentMessage = "";// 現在表示中のメッセージ
        private static int _messageIndex = 0;// メッセージの現在のインデックス
        private static int _messageY = 10;
        private const int _MessageLineMargin = 4; // 行間マージン（ピクセル単位で調整可能）                                      
        private static bool _isMessageTicking = false;// メッセージ送り用
        private static System.Windows.Forms.Timer _msgTimer = new System.Windows.Forms.Timer();
        private static Queue<string> _messageQueue = new Queue<string>();

        private static bool _isMessageShowing = false;// メッセージ送り用フィールド
        public static bool IsMessageShowing { get => _isMessageShowing; private set => _isMessageShowing = value; }

        public static event Action? OnMessageCompleted;

        public static void timerSetup()
        {
            _msgTimer.Interval = _timerInterval;
            _msgTimer.Tick += MsgTimerTick;
        }

        public static void MsgTimerTick(object? sender, EventArgs e)
        {
            if (!_isMessageTicking) return;

            if (_messageIndex < _fullMessage.Length)
            {
                _currentMessage += _fullMessage[_messageIndex];
                _messageIndex++;
            }
            else
            {
                _isMessageTicking = false;
                _isMessageCompleted = true;

                // 次のメッセージがあれば表示
                if (_messageQueue.Count > 0)
                {
                    ShowNextAsync();
                }
                else
                {
                    IsMessageShowing = false;
                    _msgTimer.Stop();
                }

                OnMessageCompleted?.Invoke();
            }
        }

        private static async void ShowNextAsync()
        {
            if (_messageQueue.Count > 0)
            {
                _fullMessage = _messageQueue.Dequeue();
                _currentMessage = "";
                _messageIndex = 0;
                IsMessageShowing = true;
                _isMessageCompleted = false;
                _isMessageTicking = true;

                await Task.Delay(500);
            }
            else
            {
                IsMessageShowing = false;
                _isMessageCompleted = false;
                _currentMessage = "";
                _isMessageTicking = false;
                _msgTimer.Stop();

                await Task.Delay(500);
            }
        }

        public static void Draw(Graphics g)
        {
            if (!string.IsNullOrEmpty(_currentMessage))
            {
                using (Font font = new Font("MS UI Gothic", 12))
                {
                    string[] lines = _currentMessage.Split('$');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        int y = _messageY + i * (font.Height + _MessageLineMargin);
                        g.DrawString(lines[i], font, Brushes.White, 10, y);
                    }
                }
            }
            //Debug.WriteLine($"CurrentMessage: {currentMessage}");
        }

        // メッセージ表示開始メソッド
        public static void Show(string message)
        {
            // 複数メッセージをキューに分割（$$で区切る）
            var messages = message.Split(new[] { "$$" }, StringSplitOptions.None);
            foreach (var msg in messages) _messageQueue.Enqueue(msg);

            if (!IsMessageShowing)
            {
                ShowNextAsync();
                _msgTimer.Start();
            }
        }

        public static async Task ShowAsync(string message)
        {
            Show(message);

            while (!_isMessageCompleted)
            {
                await Task.Delay(200); // メッセージが完了するまで待機
            }
        }

        public static void InputKey()
        {
            if (GameStateManager.Instance.CurrentMode == GameMode.Explore)
            {
                if (!_isMessageCompleted)
                {
                    _currentMessage = _fullMessage;
                    _messageIndex = _fullMessage.Length;
                    _isMessageCompleted = true;
                }
                else ShowNextAsync();
            }
        }

        public static void Init()
        {
            IsMessageShowing = false;
            _isMessageCompleted = false;
            _fullMessage = "";
            _currentMessage = "";
            _messageIndex = 0;
            _messageQueue.Clear();
            _isMessageTicking = false;
            _msgTimer.Stop();
        }

    }//class
}// namespace EscapeFromDungeon
