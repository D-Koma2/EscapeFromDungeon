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
        private const string testMassage = "ダンジョンへようこそ！！\nダンジョンへようこそ！！\n\nダンジョンへようこそ！！\nダンジョンへようこそ！！";
        public bool isMessageShowing = false;// メッセージ送り用フィールド
        public bool isMessageCompleted = false;// メッセージが完了したかどうか

        private const int timerInterval = 16; // 約60FPS

        private string fullMessage = "";// フルメッセージ（改行を含む）
        private string currentMessage = "";// 現在表示中のメッセージ
        private int messageIndex = 0;// メッセージの現在のインデックス
        private int messageY = 10;
        private const int MessageLineMargin = 4; // 行間マージン（ピクセル単位で調整可能）                                      
        private bool isMessageTicking = false;// メッセージ送り用
        private System.Windows.Forms.Timer msgTimer;

        private Queue<string> messageQueue = new Queue<string>();

        public Message()
        {
            msgTimer = new System.Windows.Forms.Timer();
            msgTimer.Interval = timerInterval;
            msgTimer.Tick += MsgTimer_Tick;
            msgTimer.Start();
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
            }
        }

        // メッセージ表示開始メソッド
        public void Show(string message)
        {
            // 複数メッセージをキューに分割（例：\n\nで区切る）
            var messages = message.Split(new[] { "\n\n" }, StringSplitOptions.None);
            foreach (var msg in messages) messageQueue.Enqueue(msg);
            ShowNext();
        }

        public void ShowNext()
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
            }
        }

        public void Draw(Graphics g)
        {
            if (!string.IsNullOrEmpty(currentMessage))
            {
                using (Font font = new Font("MS UI Gothic", 16))
                {
                    string[] lines = currentMessage.Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        int y = messageY + i * (font.Height + MessageLineMargin);
                        g.DrawString(lines[i], font, Brushes.White, 10, y);
                    }
                }
            }
        }

        public void InputKey(object sender, KeyEventArgs e)
        {
            // メッセージ中はescape以外のキー操作を無効化
            if (isMessageShowing)
            {
                // メッセージ表示中はスペースで全文表示 or 次のメッセージ
                if (e.KeyCode == Keys.Space)
                {
                    if (!isMessageCompleted)
                    {
                        // 全文一気に表示
                        currentMessage = fullMessage;
                        messageIndex = fullMessage.Length;
                        isMessageCompleted = true;
                    }
                    else ShowNext();
                }
                return;
            }
        }

    }//class
}// namespace EscapeFromDungeon
