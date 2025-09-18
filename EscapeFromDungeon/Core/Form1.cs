using EscapeFromDungeon.Constants;
using EscapeFromDungeon.Models;
using EscapeFromDungeon.Properties;
using EscapeFromDungeon.Services;

namespace EscapeFromDungeon.Core
{
    public partial class Form1 : Form
    {
        public static readonly int tilesOfMapWidth = 13;// 必ず奇数(Playerの表示位置が中心でなくなる)
        public static readonly int tilesOfMapHeight = 13;// 必ず奇数(Playerの表示位置が中心でなくなる)

        private readonly Color _lblBaseCol = Color.DarkGray;
        private readonly Color _lblSelectCol = Color.DarkOrange;

        private PictureBox _mapImage = default!, _overlayImg = default!, _playerImg = default!, _monsterImg = default!;
        private FadeForm _fade = default!;
        private GameManager _gameManager;

        private System.Windows.Forms.Timer _timer = default!;//画面表示更新用
        private const int _timerInterval = 32;

        private static Dictionary<Label, string> _itemMap = default!;
        private static Dictionary<Label, CmdAndKeySet> _lblMap = default!;

        private DateTime _lastInputTime = DateTime.MinValue;
        private readonly TimeSpan _inputCooldown = TimeSpan.FromMilliseconds(300);

        private DateTime _lastInputTimeExplor = DateTime.MinValue;
        private readonly TimeSpan _inputCooldownExplor = TimeSpan.FromMilliseconds(125);

        private bool _isWaiting = false;

        public class CmdAndKeySet
        {
            public string Cmd { get; private set; }
            public Keys Keys { get; private set; }
            public CmdAndKeySet(string cmd, Keys keys) { Cmd = cmd; Keys = keys; }
        }

        public Form1()
        {
            InitializeComponent();

            InitDictionary();// ラベルの名前を使うので InitializeComponent()の後

            _gameManager = new GameManager();//最初に生成する事!
            SetupHandleEvents();//GameManager生成の後に呼ぶ

            InitPictureBoxes();
            InitDraw();
            TimerSetUp();
            FadeSetup();
            //DispPoint();
        }

        private void InitDictionary()
        {
            _itemMap = new Dictionary<Label, string>
            {
                { lblUsePosion, Const.potion },
                { lblUseCurePoison, Const.curePoison },
                { lblUseTorch, Const.torch }
            };

            _lblMap = new Dictionary<Label, CmdAndKeySet>
            {
                { lblAttack, new CmdAndKeySet(Const.CommandAtk, Keys.Up) },
                { lblEscape, new CmdAndKeySet(Const.CommandEsc, Keys.Down) },
                { lblDefence, new CmdAndKeySet(Const.CommandDef, Keys.Left) },
                { lblHeal, new CmdAndKeySet(Const.CommandHeal, Keys.Right) },
            };
        }

        public void InitGame()
        {
            GameStateManager.Instance.ChangeMode(GameMode.Title);
            Map.InitMap();
            _gameManager.Init();
            DrawMessage.Init();
            Map.AddViewRadius(Map._maxViewRadius);
            Map.SetIsVisionEnable(true);
            SetMonsterImgVisible(false);
            SetLabelVisible(true);
            InitDraw();
            //DispPoint();
        }

        private void InitDraw()
        {
            ChangeLblText();
            Map.Draw(_mapImage);
            Map.DrawBrightness();
            SetMapPos();
        }

        private void SetupHandleEvents()
        {
            InputManager.MoveKeyPressed = HandleBattleOrExploreAsync;
            InputManager.ItemKeyPressed = UseItem;

            _gameManager.ChangeLblText = ChangeLblText;
            _gameManager.SetMonsterImg = SetMonsterImage;
            _gameManager.SetLabelBaseCol = SetLabelBaseCol;
            _gameManager.SetMapPos = SetMapPos;

            _gameManager.CallDrop = DropEnemyAsync;
            _gameManager.Battle.CallDrop = DropEnemyAsync;
            _gameManager.Battle.CallShaker = ShakeAsync;
            _gameManager.Battle.CallShrink = ShrinkEnemyAsync;

            _gameManager.Battle.SetLabelVisible = SetLabelVisible;
            _gameManager.Battle.SetMonsterVisible = SetMonsterImgVisible;
            _gameManager.Battle.ChangeLblText = ChangeLblText;

            _gameManager.Player.FlashByDamage = FlashByDamage;
        }

        private void FadeSetup()
        {
            _fade = new FadeForm(this, FadeForm.FadeDir.FadeOut); // Form1 を渡す
            _gameManager.StartFade = _fade.StartFade;

            // Form1 が移動したら FadeForm も追従
            this.LocationChanged += (_, __) => _fade.FollowOwner();
            _fade.InitStart = () => InitGame();
            _fade.Show();
        }

        private void TimerSetUp()
        {
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = _timerInterval;
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        // 画面更新タイマーイベント
        private void Timer_Tick(object? sender, EventArgs e)
        {
            Map.ClearBrightness(_overlayImg);
            Map.DrawBrightness();
            _playerImg.Image = _gameManager.Player.playerImage;
            _gameManager.PlayerVisible(_playerImg);
            _overlayImg.Invalidate();
            StateBox.Invalidate();
            MsgBox.Invalidate();
            VisiblelblUse();
            LimitBox.Invalidate();
            //DispPoint();
        }

        public void ChangeLblText()
        {
            if (GameStateManager.Instance.CurrentMode is GameMode.Battle)
            {
                lblAttack.Text = Const.attackLabelText;
                lblDefence.Text = Const.defenceLabelText;
                lblHeal.Text = Const.healLabelText;
                lblEscape.Text = Const.escapeLabelText;
            }
            else
            {
                lblAttack.Text = Const.upMoveText;
                lblDefence.Text = Const.leftMoveText;
                lblHeal.Text = Const.rightMoveText;
                lblEscape.Text = Const.downMoveText;
            }
        }
        private void InitPictureBoxes()
        {
            //マップ画像の枠　入れ子の親
            mapDrawBox.Size = new Size(Map.tileSize * tilesOfMapWidth, Map.tileSize * tilesOfMapHeight);

            // マップ画像　入れ子の子
            _mapImage = new PictureBox
            {
                Size = new Size(Map.Width * Map.tileSize, Map.Height * Map.tileSize),
                Location = Point.Empty,
                Image = Map.MapCanvas,
                SizeMode = PictureBoxSizeMode.Normal
            };

            mapDrawBox.Controls.Add(_mapImage);

            // オーバーレイ画像
            _overlayImg = new PictureBox
            {
                Size = mapDrawBox.Size,
                Location = Point.Empty,
                BackColor = Color.Transparent,
                Image = Map.OverrayCanvas,
                Parent = _mapImage // mapImageの上に重ねる
            };

            _overlayImg.BringToFront(); // mapImageの上に表示

            // プレイヤー画像
            _playerImg = new PictureBox
            {
                Size = new(Map.tileSize, Map.tileSize),
                Location = new Point(Map.tileSize * (tilesOfMapWidth / 2), Map.tileSize * (tilesOfMapHeight / 2)),
                BackColor = Color.Transparent,
                Image = _gameManager.Player.playerImage,
                Parent = _overlayImg
            };

            _overlayImg.Controls.Add(_playerImg);
            _playerImg.BringToFront(); // overlayの上に表示

            // モンスター画像
            _monsterImg = new PictureBox
            {
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            this.Controls.Add(_monsterImg);

            _monsterImg.BringToFront();

            MsgBox.Location = new Point(10, 440);
        }//InitPictureBoxes

        public void SetMonsterImage(System.Drawing.Image img)
        {
            _monsterImg.Image = img;
            _monsterImg.Location = new Point(96, 96);
            _monsterImg.Size = new Size(Map.tileSize * 8, Map.tileSize * 8);
        }

        // マップとプレイヤーの位置調整
        public void SetMapPos()
        {
            int moveX = Map.tileSize * (Map.PlayerPos.X - tilesOfMapWidth / 2);
            int moveY = Map.tileSize * (Map.PlayerPos.Y - tilesOfMapHeight / 2);

            _mapImage.Location = new Point(-moveX, -moveY);
            _overlayImg.Location = new Point(moveX, moveY);
            //playerImg.Location = new Point(Map.tileSize * (tilesOfMapWidth / 2), Map.tileSize * (tilesOfMapHeight / 2));
        }

        private void MainFormKeyDown(object sender, KeyEventArgs e)
        {
            if (DateTime.Now - _lastInputTimeExplor < _inputCooldownExplor) return;
            _lastInputTimeExplor = DateTime.Now;

            SetLblCol(e.KeyCode, _lblSelectCol);
            InputManager.KeyInput(e.KeyCode);
        }

        private void FormKeyUp(object sender, KeyEventArgs e)
        {
            SetLblCol(e.KeyCode, _lblBaseCol);
        }

        private void SetLblCol(Keys keyCode, Color color)
        {
            switch (keyCode)
            {
                case Keys.Left:
                    lblDefence.BackColor = color;
                    break;
                case Keys.Right:
                    lblHeal.BackColor = color;
                    break;
                case Keys.Up:
                    lblAttack.BackColor = color;
                    break;
                case Keys.Down:
                    lblEscape.BackColor = color;
                    break;
                default:
                    break;
            }
        }

        private void StateBoxPaint(object sender, PaintEventArgs e)
        {
            DrawInfo.DrawStatus(e.Graphics, _gameManager.Player);
        }

        private void MsgBoxPaint(object sender, PaintEventArgs e)
        {
            DrawMessage.Draw(e.Graphics);
        }

        private void LimitBoxPaint(object sender, PaintEventArgs e)
        {
            DrawInfo.DrawLimitBar(e.Graphics, _gameManager.Player.Limit, _gameManager.limitMax);
        }

        private async void MovelblClickAsync(object sender, EventArgs e)
        {
            if (sender is Label lbl && _lblMap.TryGetValue(lbl, out CmdAndKeySet? lblName))
            {
                await HandleBattleOrExploreAsync(lblName.Cmd, lblName.Keys);
            }
        }

        // 上下左右ラベルのマウスクリック・対応キー入力時の両方から呼ばれる
        private async Task HandleBattleOrExploreAsync(string command, Keys exploreKey)
        {
            // 探索 <-> バトル移行時に入力をロックする(GameManagerの EncounterEventAsync BattleCheckAsync でロック開始)
            if (InputLockManager.IsInputLocked()) return;

            // キーの連続入力をロック
            if (_isWaiting) return;
            if (DateTime.Now - _lastInputTime < _inputCooldown) return;
            _lastInputTime = DateTime.Now;
            _isWaiting = true;

            SetLblCol(exploreKey, _lblSelectCol);
            await Task.Delay(100);

            if (GameStateManager.Instance.CurrentMode is GameMode.Battle)
            {
                SetLabelVisible(false);
                await _gameManager.Battle.PlayerTurnAsync(command);
                await _gameManager.BattleCheckAsync();
            }
            else if (GameStateManager.Instance.CurrentMode is GameMode.Explore)
            {
                InputManager.KeyInput(exploreKey);
            }

            SetLabelBaseCol();

            _isWaiting = false;
        }

        public async Task ShakeAsync(Target shakeTarget, Shake shakeType, int durationMs = 500, int intervalMs = 30)
        {
            PictureBox target = default!;
            if (shakeTarget is Target.player) target = MsgBox;
            else if (shakeTarget is Target.enemy) target = _monsterImg;

            var originalLocation = target.Location;
            var rand = new Random();
            int shakeCount = durationMs / intervalMs;

            for (int i = 0; i < shakeCount; i++)
            {
                int offsetX, offsetY;
                if (shakeType is Shake.weak)
                {
                    // 弱点攻撃時は大きく揺らす
                    offsetX = rand.Next(-12, 13);
                    offsetY = rand.Next(-12, 13);
                }
                else
                {
                    // 通常の揺れ
                    offsetX = rand.Next(-6, 7);
                    offsetY = rand.Next(-6, 7);
                }

                target.Location = new Point(originalLocation.X + offsetX, originalLocation.Y + offsetY);

                await Task.Delay(intervalMs);
            }

            target.Location = originalLocation;
        }

        public async Task DropEnemyAsync(int dropDistance = 600, int step = 10, int intervalMs = 16, bool isIntro = false)
        {
            Point originalLocation = _monsterImg.Location;

            if (isIntro)
            {
                // バトル開始時：上から落ちてくる
                int startY = originalLocation.Y - dropDistance;
                _monsterImg.Location = new Point(originalLocation.X, startY);
                _monsterImg.Visible = true;

                int steps = dropDistance / step;
                for (int i = 0; i < steps; i++)
                {
                    _monsterImg.Location = new Point(originalLocation.X, _monsterImg.Location.Y + step);
                    await Task.Delay(intervalMs);
                }

                _monsterImg.Location = originalLocation;
            }
            else
            {
                // 敵撃破時：下に落ちて消える
                int steps = dropDistance / step;
                for (int i = 0; i < steps; i++)
                {
                    _monsterImg.Location = new Point(originalLocation.X, _monsterImg.Location.Y + step);
                    await Task.Delay(intervalMs);
                }

                _monsterImg.Visible = false;
                _monsterImg.Location = originalLocation;
            }
        }

        public async Task ShrinkEnemyAsync(int steps = 20, int intervalMs = 30)
        {
            var originalSize = _monsterImg.Size;
            var originalLocation = _monsterImg.Location;

            for (int i = 0; i < steps; i++)
            {
                // 割合を計算（徐々に小さく）
                float scale = 1f - (i + 1f) / steps;

                int newWidth = (int)(originalSize.Width * scale);
                int newHeight = (int)(originalSize.Height * scale);

                // 中心を保つように位置を調整
                int offsetX = (originalSize.Width - newWidth) / 2;
                int offsetY = (originalSize.Height - newHeight) / 2;

                _monsterImg.Size = new Size(newWidth, newHeight);
                _monsterImg.Location = new Point(originalLocation.X + offsetX, originalLocation.Y + offsetY);

                await Task.Delay(intervalMs);
            }

            // 完了後に非表示＆サイズ・位置を初期化
            _monsterImg.Visible = false;
            _monsterImg.Size = originalSize;
            _monsterImg.Location = originalLocation;
        }

        private void SetLabelVisible(bool isVisible)
        {
            lblAttack.Visible = isVisible;
            lblDefence.Visible = isVisible;
            lblHeal.Visible = isVisible;
            lblEscape.Visible = isVisible;
            SetLabelBaseCol();
        }

        private void SetLabelBaseCol()
        {
            lblDefence.BackColor = _lblBaseCol;
            lblHeal.BackColor = _lblBaseCol;
            lblAttack.BackColor = _lblBaseCol;
            lblEscape.BackColor = _lblBaseCol;
        }

        public void LabelHealVisible()
        {
            if (GameStateManager.Instance.CurrentMode is not GameMode.Battle)
            {
                int potionCount = _gameManager.Player.GetItemCount(Const.potion);
                lblHeal.Visible = potionCount > 0;
            }
        }

        private void SetMonsterImgVisible(bool visible) => _monsterImg.Visible = visible;

        private void VisiblelblUse()
        {
            int potionCount = _gameManager.Player.GetItemCount(Const.potion);
            int curePoisonCount = _gameManager.Player.GetItemCount(Const.curePoison);
            int torchCount = _gameManager.Player.GetItemCount(Const.torch);
            lblUsePosion.Visible = GameStateManager.Instance.CurrentMode == GameMode.Explore && potionCount > 0;
            lblUseCurePoison.Visible = GameStateManager.Instance.CurrentMode == GameMode.Explore && curePoisonCount > 0;
            lblUseTorch.Visible = GameStateManager.Instance.CurrentMode == GameMode.Explore && torchCount > 0;
        }

        private void ItemLabelClick(object sender, EventArgs e)
        {
            if (sender is Label lbl && _itemMap.TryGetValue(lbl, out string? itemName))
            {
                UseItem(itemName);
            }
        }

        private async void UseItem(string itemName)
        {
            switch (itemName)
            {
                case Const.potion:
                    int potionCount = _gameManager.Player.GetItemCount(Const.potion);
                    if (potionCount == 0) return;
                    SetUseLabelCol(itemName, _lblSelectCol);
                    if (_gameManager.Player.Hp == _gameManager.Player.MaxHp)
                    {
                        await DrawMessage.ShowAsync(Const.hpFullMsg);
                        await Task.Delay(200);
                        SetUseLabelCol(itemName, _lblBaseCol);
                        return;
                    }
                    _gameManager.Player.Heal(30);
                    break;

                case Const.curePoison:
                    int curePoisonCount = _gameManager.Player.GetItemCount(Const.curePoison);
                    if (curePoisonCount == 0) return;
                    SetUseLabelCol(itemName, _lblSelectCol);
                    _gameManager.Player.HealStatus();
                    break;

                case Const.torch:
                    int torchCount = _gameManager.Player.GetItemCount(Const.torch);
                    if (torchCount == 0) return;
                    SetUseLabelCol(itemName, _lblSelectCol);
                    Map.AddViewRadius(4);
                    break;
            }

            await DrawMessage.ShowAsync($"{itemName}を使った！");
            GameManager.sePlayer.PlayOnce(Resources.maou_se_magical15);
            _gameManager.Player.UseItem(itemName);
            await Task.Delay(200);
            SetUseLabelCol(itemName, _lblBaseCol);
        }

        private void SetUseLabelCol(string item, Color color)
        {
            switch (item)
            {
                case Const.potion:
                    lblUsePosion.BackColor = color;
                    break;
                case Const.curePoison:
                    lblUseCurePoison.BackColor = color;
                    break;
                case Const.torch:
                    lblUseTorch.BackColor = color;
                    break;
                default:
                    break;
            }
        }

        public async void FlashByDamage()
        {
            int red = 0;
            Color color = Color.FromArgb(255, red, 0, 0);

            for (int i = 0; i < 255; i += 5)
            {
                red++;
                this.BackColor = Color.FromArgb(255, red, 0, 0);
            }

            await Task.Delay(100);

            for (int i = 255; i > 0; i -= 5)
            {
                red--;
                this.BackColor = Color.FromArgb(255, red, 0, 0);
            }
        }

        // デバッグ用
        private void DispPoint()
        {
            label1.Text = $"XY:({Map.PlayerPos.X},{Map.PlayerPos.Y}) mi:({_mapImage.Location.X},{_mapImage.Location.Y}) " +
                $"oi:({_overlayImg.Location.X},{_overlayImg.Location.Y}) mode:({GameStateManager.Instance.CurrentMode})";
        }

    }//class
}//namespace
