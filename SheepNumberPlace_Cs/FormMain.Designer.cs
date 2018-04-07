namespace SheepNumberPlace_Cs
{
    partial class FormMain
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.OpenFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.Menu_Help = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Operation = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_VersionInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.LblLevel = new System.Windows.Forms.Label();
            this.ImageList1 = new System.Windows.Forms.ImageList(this.components);
            this.GradientTimer = new System.Windows.Forms.Timer(this.components);
            this.Menu_SelectMode = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Level = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_ResetAnswer = new System.Windows.Forms.ToolStripMenuItem();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.MenuStrip1 = new System.Windows.Forms.MenuStrip();
            this.Menu_File = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_File_Load = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_File_Save = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_NewQuestion = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Quit = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Display = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Size = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Menu_DisplayKeypad = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Assistant = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Hint = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_CheckAnswer = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Reset = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_DisplayAnswer = new System.Windows.Forms.ToolStripMenuItem();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SaveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.HintTimer = new System.Windows.Forms.Timer(this.components);
            this.ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStrip1 = new System.Windows.Forms.ToolStrip();
            this.Btn_Previous = new System.Windows.Forms.ToolStripButton();
            this.Btn_Next = new System.Windows.Forms.ToolStripButton();
            this.Tool_File_Load = new System.Windows.Forms.ToolStripButton();
            this.Tool_File_Save = new System.Windows.Forms.ToolStripButton();
            this.Tool_NewQuestion = new System.Windows.Forms.ToolStripButton();
            this.Tool_Hint = new System.Windows.Forms.ToolStripButton();
            this.Tool_CheckAnswer = new System.Windows.Forms.ToolStripButton();
            this.Tool_Reset = new System.Windows.Forms.ToolStripButton();
            this.Tool_DisplayAnswer = new System.Windows.Forms.ToolStripButton();
            this.Tool_ResetAnswer = new System.Windows.Forms.ToolStripButton();
            this.Panel_PictureBox = new System.Windows.Forms.Panel();
            this.PictureBoxPalette = new System.Windows.Forms.PictureBox();
            this.PictureBoxGrid = new System.Windows.Forms.PictureBox();
            this.PictureBoxMemo = new System.Windows.Forms.PictureBox();
            this.PictureBoxHighlight = new System.Windows.Forms.PictureBox();
            this.Panel_ChkBox = new System.Windows.Forms.Panel();
            this.Chk_SymmetricGrid = new System.Windows.Forms.CheckBox();
            this.Chk_DisplayProspect = new System.Windows.Forms.CheckBox();
            this.MenuStrip1.SuspendLayout();
            this.ToolStrip1.SuspendLayout();
            this.Panel_PictureBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxPalette)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxMemo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxHighlight)).BeginInit();
            this.Panel_ChkBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // OpenFileDialog1
            // 
            this.OpenFileDialog1.FileName = "OpenFileDialog1";
            // 
            // Menu_Help
            // 
            this.Menu_Help.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_Operation,
            this.Menu_VersionInfo});
            this.Menu_Help.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Menu_Help.Name = "Menu_Help";
            this.Menu_Help.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.Menu_Help.Size = new System.Drawing.Size(63, 31);
            this.Menu_Help.Text = "Help(&H)";
            // 
            // Menu_Operation
            // 
            this.Menu_Operation.Name = "Menu_Operation";
            this.Menu_Operation.Size = new System.Drawing.Size(188, 26);
            this.Menu_Operation.Text = "Document（Web）";
            // 
            // Menu_VersionInfo
            // 
            this.Menu_VersionInfo.Name = "Menu_VersionInfo";
            this.Menu_VersionInfo.Size = new System.Drawing.Size(188, 26);
            this.Menu_VersionInfo.Text = "Version";
            // 
            // LblLevel
            // 
            this.LblLevel.BackColor = System.Drawing.Color.Transparent;
            this.LblLevel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LblLevel.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.LblLevel.Location = new System.Drawing.Point(499, 691);
            this.LblLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblLevel.Name = "LblLevel";
            this.LblLevel.Size = new System.Drawing.Size(205, 25);
            this.LblLevel.TabIndex = 108;
            this.LblLevel.Text = "難易度";
            // 
            // ImageList1
            // 
            this.ImageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.ImageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.ImageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // GradientTimer
            // 
            this.GradientTimer.Interval = 300;
            // 
            // Menu_SelectMode
            // 
            this.Menu_SelectMode.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Menu_SelectMode.Name = "Menu_SelectMode";
            this.Menu_SelectMode.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.Menu_SelectMode.ShowShortcutKeys = false;
            this.Menu_SelectMode.Size = new System.Drawing.Size(70, 31);
            this.Menu_SelectMode.Text = "Mode(&M)";
            // 
            // Menu_Level
            // 
            this.Menu_Level.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Menu_Level.Name = "Menu_Level";
            this.Menu_Level.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.Menu_Level.ShortcutKeyDisplayString = "";
            this.Menu_Level.ShowShortcutKeys = false;
            this.Menu_Level.Size = new System.Drawing.Size(94, 31);
            this.Menu_Level.Text = "Set Level(&L)";
            // 
            // Menu_ResetAnswer
            // 
            this.Menu_ResetAnswer.Name = "Menu_ResetAnswer";
            this.Menu_ResetAnswer.Size = new System.Drawing.Size(255, 26);
            this.Menu_ResetAnswer.Text = "Reset Display Right Answer";
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(-163, -195);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(-163, -195);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // MenuStrip1
            // 
            this.MenuStrip1.AutoSize = false;
            this.MenuStrip1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.MenuStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.MenuStrip1.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.MenuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 0);
            this.MenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.MenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_File,
            this.Menu_Display,
            this.Menu_Assistant,
            this.Menu_Level,
            this.Menu_SelectMode,
            this.Menu_Help});
            this.MenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip1.Name = "MenuStrip1";
            this.MenuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.MenuStrip1.Size = new System.Drawing.Size(760, 35);
            this.MenuStrip1.TabIndex = 107;
            this.MenuStrip1.Text = "MenuStrip1";
            // 
            // Menu_File
            // 
            this.Menu_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_File_Load,
            this.Menu_File_Save,
            this.Menu_NewQuestion,
            this.Menu_Quit});
            this.Menu_File.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Menu_File.Name = "Menu_File";
            this.Menu_File.Padding = new System.Windows.Forms.Padding(0);
            this.Menu_File.ShortcutKeyDisplayString = "";
            this.Menu_File.ShowShortcutKeys = false;
            this.Menu_File.Size = new System.Drawing.Size(51, 31);
            this.Menu_File.Text = "File(&F)";
            // 
            // Menu_File_Load
            // 
            this.Menu_File_Load.Name = "Menu_File_Load";
            this.Menu_File_Load.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.Menu_File_Load.Size = new System.Drawing.Size(233, 26);
            this.Menu_File_Load.Text = "Load Game Data";
            // 
            // Menu_File_Save
            // 
            this.Menu_File_Save.Name = "Menu_File_Save";
            this.Menu_File_Save.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.Menu_File_Save.Size = new System.Drawing.Size(233, 26);
            this.Menu_File_Save.Text = "Save Game Data";
            // 
            // Menu_NewQuestion
            // 
            this.Menu_NewQuestion.Name = "Menu_NewQuestion";
            this.Menu_NewQuestion.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.Menu_NewQuestion.Size = new System.Drawing.Size(233, 26);
            this.Menu_NewQuestion.Text = "New Question";
            // 
            // Menu_Quit
            // 
            this.Menu_Quit.Name = "Menu_Quit";
            this.Menu_Quit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.Menu_Quit.Size = new System.Drawing.Size(233, 26);
            this.Menu_Quit.Text = "Quit Game";
            // 
            // Menu_Display
            // 
            this.Menu_Display.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_Size,
            this.ToolStripSeparator1,
            this.Menu_DisplayKeypad});
            this.Menu_Display.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Menu_Display.Name = "Menu_Display";
            this.Menu_Display.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.Menu_Display.ShowShortcutKeys = false;
            this.Menu_Display.Size = new System.Drawing.Size(78, 31);
            this.Menu_Display.Text = "Display(&V)";
            // 
            // Menu_Size
            // 
            this.Menu_Size.Name = "Menu_Size";
            this.Menu_Size.Size = new System.Drawing.Size(179, 26);
            this.Menu_Size.Text = "Size";
            // 
            // ToolStripSeparator1
            // 
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(176, 6);
            // 
            // Menu_DisplayKeypad
            // 
            this.Menu_DisplayKeypad.Name = "Menu_DisplayKeypad";
            this.Menu_DisplayKeypad.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.Menu_DisplayKeypad.Size = new System.Drawing.Size(179, 26);
            this.Menu_DisplayKeypad.Text = "Key Pad";
            // 
            // Menu_Assistant
            // 
            this.Menu_Assistant.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_Hint,
            this.Menu_CheckAnswer,
            this.Menu_Reset,
            this.Menu_DisplayAnswer,
            this.Menu_ResetAnswer});
            this.Menu_Assistant.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Menu_Assistant.Name = "Menu_Assistant";
            this.Menu_Assistant.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.Menu_Assistant.ShortcutKeyDisplayString = "";
            this.Menu_Assistant.ShowShortcutKeys = false;
            this.Menu_Assistant.Size = new System.Drawing.Size(92, 31);
            this.Menu_Assistant.Text = "Assistant(&A)";
            // 
            // Menu_Hint
            // 
            this.Menu_Hint.Name = "Menu_Hint";
            this.Menu_Hint.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.Menu_Hint.Size = new System.Drawing.Size(255, 26);
            this.Menu_Hint.Text = "Hint";
            // 
            // Menu_CheckAnswer
            // 
            this.Menu_CheckAnswer.Name = "Menu_CheckAnswer";
            this.Menu_CheckAnswer.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.Menu_CheckAnswer.Size = new System.Drawing.Size(255, 26);
            this.Menu_CheckAnswer.Text = "Check Answer";
            // 
            // Menu_Reset
            // 
            this.Menu_Reset.Name = "Menu_Reset";
            this.Menu_Reset.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.Menu_Reset.Size = new System.Drawing.Size(255, 26);
            this.Menu_Reset.Text = "Reset";
            // 
            // Menu_DisplayAnswer
            // 
            this.Menu_DisplayAnswer.Name = "Menu_DisplayAnswer";
            this.Menu_DisplayAnswer.Size = new System.Drawing.Size(255, 26);
            this.Menu_DisplayAnswer.Text = "Display Right Answer";
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.BackColor = System.Drawing.Color.Red;
            this.TopToolStripPanel.Location = new System.Drawing.Point(-149, -182);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(133, 125);
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(-163, -195);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // SaveFileDialog1
            // 
            this.SaveFileDialog1.Tag = "Save";
            // 
            // HintTimer
            // 
            this.HintTimer.Interval = 800;
            // 
            // ToolStripSeparator2
            // 
            this.ToolStripSeparator2.Name = "ToolStripSeparator2";
            this.ToolStripSeparator2.Size = new System.Drawing.Size(6, 35);
            // 
            // ToolStripSeparator3
            // 
            this.ToolStripSeparator3.Name = "ToolStripSeparator3";
            this.ToolStripSeparator3.Size = new System.Drawing.Size(6, 35);
            // 
            // ToolStrip1
            // 
            this.ToolStrip1.BackColor = System.Drawing.Color.White;
            this.ToolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ToolStrip1.GripMargin = new System.Windows.Forms.Padding(0);
            this.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ToolStrip1.ImageScalingSize = new System.Drawing.Size(0, 0);
            this.ToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Btn_Previous,
            this.Btn_Next,
            this.ToolStripSeparator2,
            this.Tool_File_Load,
            this.Tool_File_Save,
            this.Tool_NewQuestion,
            this.ToolStripSeparator3,
            this.Tool_Hint,
            this.Tool_CheckAnswer,
            this.Tool_Reset,
            this.Tool_DisplayAnswer,
            this.Tool_ResetAnswer});
            this.ToolStrip1.Location = new System.Drawing.Point(0, 35);
            this.ToolStrip1.Name = "ToolStrip1";
            this.ToolStrip1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 4);
            this.ToolStrip1.Size = new System.Drawing.Size(760, 41);
            this.ToolStrip1.TabIndex = 109;
            this.ToolStrip1.Text = "ToolStrip1";
            // 
            // Btn_Previous
            // 
            this.Btn_Previous.AutoSize = false;
            this.Btn_Previous.BackColor = System.Drawing.Color.White;
            this.Btn_Previous.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Btn_Previous.Image = global::SheepNumberPlace_Cs.Properties.Resources.arrow_l;
            this.Btn_Previous.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.Btn_Previous.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Btn_Previous.Margin = new System.Windows.Forms.Padding(0);
            this.Btn_Previous.Name = "Btn_Previous";
            this.Btn_Previous.Size = new System.Drawing.Size(40, 35);
            this.Btn_Previous.Text = "ToolStripButton1";
            this.Btn_Previous.ToolTipText = "Undo";
            // 
            // Btn_Next
            // 
            this.Btn_Next.AutoSize = false;
            this.Btn_Next.BackColor = System.Drawing.Color.White;
            this.Btn_Next.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Btn_Next.Image = global::SheepNumberPlace_Cs.Properties.Resources.arrow_r;
            this.Btn_Next.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.Btn_Next.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Btn_Next.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.Btn_Next.Name = "Btn_Next";
            this.Btn_Next.Size = new System.Drawing.Size(40, 35);
            this.Btn_Next.Text = "ToolStripButton2";
            this.Btn_Next.ToolTipText = "Redo";
            // 
            // Tool_File_Load
            // 
            this.Tool_File_Load.AutoSize = false;
            this.Tool_File_Load.BackColor = System.Drawing.Color.White;
            this.Tool_File_Load.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Tool_File_Load.Image = global::SheepNumberPlace_Cs.Properties.Resources.openfile;
            this.Tool_File_Load.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.Tool_File_Load.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Tool_File_Load.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.Tool_File_Load.Name = "Tool_File_Load";
            this.Tool_File_Load.Size = new System.Drawing.Size(40, 35);
            this.Tool_File_Load.Text = "ToolStripButton2";
            this.Tool_File_Load.ToolTipText = "Load Game Data";
            // 
            // Tool_File_Save
            // 
            this.Tool_File_Save.AutoSize = false;
            this.Tool_File_Save.BackColor = System.Drawing.Color.White;
            this.Tool_File_Save.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Tool_File_Save.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Tool_File_Save.Image = global::SheepNumberPlace_Cs.Properties.Resources.savefile;
            this.Tool_File_Save.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.Tool_File_Save.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Tool_File_Save.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.Tool_File_Save.Name = "Tool_File_Save";
            this.Tool_File_Save.Size = new System.Drawing.Size(40, 35);
            this.Tool_File_Save.ToolTipText = "Save Game Data";
            // 
            // Tool_NewQuestion
            // 
            this.Tool_NewQuestion.AutoSize = false;
            this.Tool_NewQuestion.BackColor = System.Drawing.Color.White;
            this.Tool_NewQuestion.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Tool_NewQuestion.Image = global::SheepNumberPlace_Cs.Properties.Resources.NewQ;
            this.Tool_NewQuestion.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.Tool_NewQuestion.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Tool_NewQuestion.Margin = new System.Windows.Forms.Padding(0);
            this.Tool_NewQuestion.Name = "Tool_NewQuestion";
            this.Tool_NewQuestion.Size = new System.Drawing.Size(40, 35);
            this.Tool_NewQuestion.Text = "New Question";
            this.Tool_NewQuestion.ToolTipText = "New Question";
            // 
            // Tool_Hint
            // 
            this.Tool_Hint.AutoSize = false;
            this.Tool_Hint.BackColor = System.Drawing.Color.White;
            this.Tool_Hint.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Tool_Hint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Tool_Hint.Image = global::SheepNumberPlace_Cs.Properties.Resources.hint;
            this.Tool_Hint.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.Tool_Hint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Tool_Hint.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.Tool_Hint.Name = "Tool_Hint";
            this.Tool_Hint.Size = new System.Drawing.Size(40, 35);
            this.Tool_Hint.Text = "hint";
            this.Tool_Hint.ToolTipText = "hint";
            this.Tool_Hint.Click += new System.EventHandler(this.Tool_Hint_Click);
            // 
            // Tool_CheckAnswer
            // 
            this.Tool_CheckAnswer.AutoSize = false;
            this.Tool_CheckAnswer.BackColor = System.Drawing.Color.White;
            this.Tool_CheckAnswer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Tool_CheckAnswer.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.Tool_CheckAnswer.Image = global::SheepNumberPlace_Cs.Properties.Resources.AnswerCheck;
            this.Tool_CheckAnswer.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.Tool_CheckAnswer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Tool_CheckAnswer.Margin = new System.Windows.Forms.Padding(0);
            this.Tool_CheckAnswer.Name = "Tool_CheckAnswer";
            this.Tool_CheckAnswer.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Tool_CheckAnswer.Size = new System.Drawing.Size(40, 35);
            this.Tool_CheckAnswer.Text = "Check Answer";
            this.Tool_CheckAnswer.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // Tool_Reset
            // 
            this.Tool_Reset.AutoSize = false;
            this.Tool_Reset.BackColor = System.Drawing.Color.White;
            this.Tool_Reset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Tool_Reset.Image = global::SheepNumberPlace_Cs.Properties.Resources.Reset;
            this.Tool_Reset.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.Tool_Reset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Tool_Reset.Margin = new System.Windows.Forms.Padding(0);
            this.Tool_Reset.Name = "Tool_Reset";
            this.Tool_Reset.Size = new System.Drawing.Size(40, 35);
            this.Tool_Reset.Text = "Reset";
            // 
            // Tool_DisplayAnswer
            // 
            this.Tool_DisplayAnswer.AutoSize = false;
            this.Tool_DisplayAnswer.BackColor = System.Drawing.Color.White;
            this.Tool_DisplayAnswer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Tool_DisplayAnswer.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.Tool_DisplayAnswer.Image = global::SheepNumberPlace_Cs.Properties.Resources.RightAnswer;
            this.Tool_DisplayAnswer.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.Tool_DisplayAnswer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Tool_DisplayAnswer.Margin = new System.Windows.Forms.Padding(0);
            this.Tool_DisplayAnswer.Name = "Tool_DisplayAnswer";
            this.Tool_DisplayAnswer.Size = new System.Drawing.Size(40, 35);
            this.Tool_DisplayAnswer.Text = "Display Answer";
            this.Tool_DisplayAnswer.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.Tool_DisplayAnswer.ToolTipText = "Display Answer";
            // 
            // Tool_ResetAnswer
            // 
            this.Tool_ResetAnswer.AutoSize = false;
            this.Tool_ResetAnswer.BackColor = System.Drawing.Color.White;
            this.Tool_ResetAnswer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Tool_ResetAnswer.Image = global::SheepNumberPlace_Cs.Properties.Resources.ResetAnswer;
            this.Tool_ResetAnswer.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.Tool_ResetAnswer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Tool_ResetAnswer.Margin = new System.Windows.Forms.Padding(0);
            this.Tool_ResetAnswer.Name = "Tool_ResetAnswer";
            this.Tool_ResetAnswer.Size = new System.Drawing.Size(40, 35);
            this.Tool_ResetAnswer.Text = "Reset Right Answer Check";
            this.Tool_ResetAnswer.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // Panel_PictureBox
            // 
            this.Panel_PictureBox.AutoSize = true;
            this.Panel_PictureBox.BackColor = System.Drawing.Color.Transparent;
            this.Panel_PictureBox.Controls.Add(this.PictureBoxPalette);
            this.Panel_PictureBox.Controls.Add(this.PictureBoxGrid);
            this.Panel_PictureBox.Controls.Add(this.PictureBoxMemo);
            this.Panel_PictureBox.Controls.Add(this.PictureBoxHighlight);
            this.Panel_PictureBox.Location = new System.Drawing.Point(0, 0);
            this.Panel_PictureBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Panel_PictureBox.Name = "Panel_PictureBox";
            this.Panel_PictureBox.Size = new System.Drawing.Size(337, 618);
            this.Panel_PictureBox.TabIndex = 120;
            // 
            // PictureBoxPalette
            // 
            this.PictureBoxPalette.BackColor = System.Drawing.Color.Transparent;
            this.PictureBoxPalette.Location = new System.Drawing.Point(16, 259);
            this.PictureBoxPalette.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.PictureBoxPalette.Name = "PictureBoxPalette";
            this.PictureBoxPalette.Size = new System.Drawing.Size(73, 40);
            this.PictureBoxPalette.TabIndex = 92;
            this.PictureBoxPalette.TabStop = false;
            // 
            // PictureBoxGrid
            // 
            this.PictureBoxGrid.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.PictureBoxGrid.Location = new System.Drawing.Point(16, 89);
            this.PictureBoxGrid.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.PictureBoxGrid.Name = "PictureBoxGrid";
            this.PictureBoxGrid.Size = new System.Drawing.Size(73, 68);
            this.PictureBoxGrid.TabIndex = 0;
            this.PictureBoxGrid.TabStop = false;
            // 
            // PictureBoxMemo
            // 
            this.PictureBoxMemo.BackColor = System.Drawing.Color.Transparent;
            this.PictureBoxMemo.Location = new System.Drawing.Point(16, 211);
            this.PictureBoxMemo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.PictureBoxMemo.Name = "PictureBoxMemo";
            this.PictureBoxMemo.Size = new System.Drawing.Size(73, 40);
            this.PictureBoxMemo.TabIndex = 81;
            this.PictureBoxMemo.TabStop = false;
            // 
            // PictureBoxHighlight
            // 
            this.PictureBoxHighlight.BackColor = System.Drawing.Color.Transparent;
            this.PictureBoxHighlight.Location = new System.Drawing.Point(16, 164);
            this.PictureBoxHighlight.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.PictureBoxHighlight.Name = "PictureBoxHighlight";
            this.PictureBoxHighlight.Size = new System.Drawing.Size(73, 40);
            this.PictureBoxHighlight.TabIndex = 93;
            this.PictureBoxHighlight.TabStop = false;
            // 
            // Panel_ChkBox
            // 
            this.Panel_ChkBox.Controls.Add(this.Chk_SymmetricGrid);
            this.Panel_ChkBox.Controls.Add(this.Chk_DisplayProspect);
            this.Panel_ChkBox.Location = new System.Drawing.Point(16, 639);
            this.Panel_ChkBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Panel_ChkBox.Name = "Panel_ChkBox";
            this.Panel_ChkBox.Size = new System.Drawing.Size(460, 24);
            this.Panel_ChkBox.TabIndex = 121;
            // 
            // Chk_SymmetricGrid
            // 
            this.Chk_SymmetricGrid.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Chk_SymmetricGrid.Location = new System.Drawing.Point(167, 0);
            this.Chk_SymmetricGrid.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Chk_SymmetricGrid.Name = "Chk_SymmetricGrid";
            this.Chk_SymmetricGrid.Size = new System.Drawing.Size(209, 22);
            this.Chk_SymmetricGrid.TabIndex = 99;
            this.Chk_SymmetricGrid.TabStop = false;
            this.Chk_SymmetricGrid.Text = "Highlight symmetric Grid(S)";
            this.Chk_SymmetricGrid.UseVisualStyleBackColor = true;
            // 
            // Chk_DisplayProspect
            // 
            this.Chk_DisplayProspect.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Chk_DisplayProspect.Location = new System.Drawing.Point(4, 0);
            this.Chk_DisplayProspect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Chk_DisplayProspect.Name = "Chk_DisplayProspect";
            this.Chk_DisplayProspect.Size = new System.Drawing.Size(155, 22);
            this.Chk_DisplayProspect.TabIndex = 98;
            this.Chk_DisplayProspect.TabStop = false;
            this.Chk_DisplayProspect.Text = "Display prospect No(P)";
            this.Chk_DisplayProspect.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 722);
            this.Controls.Add(this.Panel_ChkBox);
            this.Controls.Add(this.ToolStrip1);
            this.Controls.Add(this.LblLevel);
            this.Controls.Add(this.RightToolStripPanel);
            this.Controls.Add(this.LeftToolStripPanel);
            this.Controls.Add(this.MenuStrip1);
            this.Controls.Add(this.TopToolStripPanel);
            this.Controls.Add(this.BottomToolStripPanel);
            this.Controls.Add(this.Panel_PictureBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FormMain";
            this.Text = "Sheep Number Place";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.MenuStrip1.ResumeLayout(false);
            this.MenuStrip1.PerformLayout();
            this.ToolStrip1.ResumeLayout(false);
            this.ToolStrip1.PerformLayout();
            this.Panel_PictureBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxPalette)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxMemo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxHighlight)).EndInit();
            this.Panel_ChkBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.OpenFileDialog OpenFileDialog1;
        internal System.Windows.Forms.ToolStripMenuItem Menu_Help;
        internal System.Windows.Forms.ToolStripMenuItem Menu_Operation;
        internal System.Windows.Forms.ToolStripMenuItem Menu_VersionInfo;
        internal System.Windows.Forms.Label LblLevel;
        internal System.Windows.Forms.ImageList ImageList1;
        internal System.Windows.Forms.Timer GradientTimer;
        internal System.Windows.Forms.ToolStripMenuItem Menu_SelectMode;
        internal System.Windows.Forms.ToolStripMenuItem Menu_Level;
        internal System.Windows.Forms.ToolStripMenuItem Menu_ResetAnswer;
        internal System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        internal System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        internal System.Windows.Forms.MenuStrip MenuStrip1;
        internal System.Windows.Forms.ToolStripMenuItem Menu_File;
        internal System.Windows.Forms.ToolStripMenuItem Menu_File_Load;
        internal System.Windows.Forms.ToolStripMenuItem Menu_File_Save;
        internal System.Windows.Forms.ToolStripMenuItem Menu_NewQuestion;
        internal System.Windows.Forms.ToolStripMenuItem Menu_Quit;
        internal System.Windows.Forms.ToolStripMenuItem Menu_Display;
        internal System.Windows.Forms.ToolStripMenuItem Menu_Size;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator1;
        internal System.Windows.Forms.ToolStripMenuItem Menu_DisplayKeypad;
        internal System.Windows.Forms.ToolStripMenuItem Menu_Assistant;
        internal System.Windows.Forms.ToolStripMenuItem Menu_Hint;
        internal System.Windows.Forms.ToolStripMenuItem Menu_CheckAnswer;
        internal System.Windows.Forms.ToolStripMenuItem Menu_Reset;
        internal System.Windows.Forms.ToolStripMenuItem Menu_DisplayAnswer;
        internal System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        internal System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        internal System.Windows.Forms.ToolTip ToolTip1;
        internal System.Windows.Forms.SaveFileDialog SaveFileDialog1;
        internal System.Windows.Forms.Timer HintTimer;
        internal System.Windows.Forms.ToolStripButton Btn_Previous;
        internal System.Windows.Forms.ToolStripButton Btn_Next;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator2;
        internal System.Windows.Forms.ToolStripButton Tool_File_Load;
        internal System.Windows.Forms.ToolStripButton Tool_File_Save;
        internal System.Windows.Forms.ToolStripButton Tool_NewQuestion;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator3;
        internal System.Windows.Forms.ToolStripButton Tool_Hint;
        internal System.Windows.Forms.ToolStripButton Tool_CheckAnswer;
        internal System.Windows.Forms.ToolStripButton Tool_Reset;
        internal System.Windows.Forms.ToolStripButton Tool_DisplayAnswer;
        internal System.Windows.Forms.ToolStripButton Tool_ResetAnswer;
        internal System.Windows.Forms.ToolStrip ToolStrip1;
        internal System.Windows.Forms.Panel Panel_PictureBox;
        internal System.Windows.Forms.PictureBox PictureBoxPalette;
        internal System.Windows.Forms.PictureBox PictureBoxGrid;
        internal System.Windows.Forms.PictureBox PictureBoxMemo;
        internal System.Windows.Forms.PictureBox PictureBoxHighlight;
        internal System.Windows.Forms.Panel Panel_ChkBox;
        internal System.Windows.Forms.CheckBox Chk_SymmetricGrid;
        internal System.Windows.Forms.CheckBox Chk_DisplayProspect;
    }
}

