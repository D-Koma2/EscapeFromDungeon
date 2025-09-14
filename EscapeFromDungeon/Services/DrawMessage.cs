using EscapeFromDungeon.Core;

namespace EscapeFromDungeon.Services
{
    internal static class DrawMessage
    {
        private const int timerInterval = 16;
        public static bool isMessageShowing = false;// メッセージ送り用フィールド
        private static bool isMessageCompleted = false;// メッセージが完了したかどうか
        private static string fullMessage = "";// フルメッセージ（改行を含む）
        private static string currentMessage = "";// 現在表示中のメッセージ
        private static int messageIndex = 0;// メッセージの現在のインデックス
        private static int messageY = 10;
        private const int MessageLineMargin = 4; // 行間マージン（ピクセル単位で調整可能）                                      
        private static bool isMessageTicking = false;// メッセージ送り用
        private static System.Windows.Forms.Timer msgTimer = new System.Windows.Forms.Timer();
        private static Queue<string> messageQueue = new Queue<string>();

        public static event Action? OnMessageCompleted;

        public static void Setup()
        {
            msgTimer.Interval = timerInterval;
            msgTimer.Tick += MsgTimerTick;
        }
        public static void MsgTimerTick(object? sender, EventArgs e)
        {
            if (!isMessageTicking) return;

            if (messageIndex < fullMessage.Length)
            {
                currentMessage += fullMessage[messageIndex];
                messageIndex++;
            }
            else
            {
                isMessageTicking = false;
                isMessageCompleted = true;

                // 次のメッセージがあれば表示
                if (messageQueue.Count > 0)
                {
                    ShowNextAsync();
                }
                else
                {
                    isMessageShowing = false;
                    msgTimer.Stop();
                }

                OnMessageCompleted?.Invoke();
            }
        }

        private static async void ShowNextAsync()
        {
            if (messageQueue.Count > 0)
            {
                fullMessage = messageQueue.Dequeue();
                currentMessage = "";
                messageIndex = 0;
                isMessageShowing = true;
                isMessageCompleted = false;
                isMessageTicking = true;

                await Task.Delay(500);
            }
            else
            {
                isMessageShowing = false;
                isMessageCompleted = false;
                currentMessage = "";
                isMessageTicking = false;
                msgTimer.Stop();

                await Task.Delay(500);
            }
        }

        public static void Draw(Graphics g)
        {
            if (!string.IsNullOrEmpty(currentMessage))
            {
                using (Font font = new Font("MS UI Gothic", 12))
                {
                    string[] lines = currentMessage.Split('$');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        int y = messageY + i * (font.Height + MessageLineMargin);
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
            foreach (var msg in messages) messageQueue.Enqueue(msg);

            if (!isMessageShowing)
            {
                ShowNextAsync();
                msgTimer.Start();
            }
        }

        public static async Task ShowAsync(string message)
        {
            Show(message);

            while (!isMessageCompleted)
            {
                await Task.Delay(200); // メッセージが完了するまで待機
            }
        }

        public static void InputKey()
        {
            if (GameManager.gameMode == GameMode.Explore)
            {
                if (!isMessageCompleted)
                {
                    currentMessage = fullMessage;
                    messageIndex = fullMessage.Length;
                    isMessageCompleted = true;
                }
                else ShowNextAsync();
            }
        }

        public static void Init()
        {
            isMessageShowing = false;
            isMessageCompleted = false;
            fullMessage = "";
            currentMessage = "";
            messageIndex = 0;
            messageQueue.Clear();
            isMessageTicking = false;
            msgTimer.Stop();
        }

    }//class
}// namespace EscapeFromDungeon
