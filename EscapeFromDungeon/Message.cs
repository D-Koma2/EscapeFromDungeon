using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EscapeFromDungeon
{
    internal class Message
    {
        private const int timerInterval = 16;
        public bool isMessageShowing = false;// メッセージ送り用フィールド
        public bool isMessageCompleted = false;// メッセージが完了したかどうか
        private string fullMessage = "";// フルメッセージ（改行を含む）
        private string currentMessage = "";// 現在表示中のメッセージ
        private int messageIndex = 0;// メッセージの現在のインデックス
        private int messageY = 10;
        private const int MessageLineMargin = 4; // 行間マージン（ピクセル単位で調整可能）                                      
        private bool isMessageTicking = false;// メッセージ送り用
        private System.Windows.Forms.Timer msgTimer;
        private Queue<string> messageQueue = new Queue<string>();
        public event Action OnMessageCompleted;

        public Message()
        {
            msgTimer = new System.Windows.Forms.Timer();
            msgTimer.Interval = timerInterval;
            msgTimer.Tick += MsgTimer_Tick;
        }

        public void MsgTimer_Tick(object sender, EventArgs e)
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
                if (GameManager.gameMode == GameMode.Battle)
                {
                    OnMessageCompleted?.Invoke(); // メッセージ完了通知
                }
            }
        }

        // メッセージ表示開始メソッド
        public void Show(string message)
        {
            // 複数メッセージをキューに分割（$$で区切る）
            var messages = message.Split(new[] { "$$" }, StringSplitOptions.None);
            foreach (var msg in messages) messageQueue.Enqueue(msg);
            msgTimer.Start();
            if (!isMessageShowing)
            {
                ShowNext();
            }
        }

        public async void ShowNext()
        {
            if (messageQueue.Count > 0)
            {
                fullMessage = messageQueue.Dequeue();
                currentMessage = "";
                messageIndex = 0;
                isMessageShowing = true;
                isMessageCompleted = false;
                isMessageTicking = true;
            }
            else
            {
                isMessageShowing = false;
                isMessageCompleted = false;
                currentMessage = "";
                isMessageTicking = false;
                msgTimer.Stop();

                await Task.Delay(1);
            }
        }

        public void Draw(Graphics g)
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
        }

        public async Task ShowAsync(string messageText)
        {
            Show(messageText); // 既存の Show を呼び出す
            while (!isMessageCompleted)
            {
                await Task.Delay(100); // メッセージが完了するまで待機
            }
        }

        public void InputKey()
        {
            if (GameManager.gameMode == GameMode.Explore)
            {
                if (!isMessageCompleted)
                {
                    currentMessage = fullMessage;
                    messageIndex = fullMessage.Length;
                    isMessageCompleted = true;
                }
                else
                {
                    ShowNext();
                }
            }
            else if (GameManager.gameMode == GameMode.Battle)
            {
                currentMessage = fullMessage;
                messageIndex = fullMessage.Length;
                isMessageCompleted = true;
            }
        }

    }//class
}// namespace EscapeFromDungeon
