using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Configuration;
using System.Diagnostics;

namespace SheepNumberPlace_Cs
{

    public partial class FormMain : Form
    {
        SudokuGrid[,] SudokuNumberGrid;
        protected internal int WindowSize = 3;
        protected internal int GridCount = 9;
        protected internal int GridSize;
        protected internal int CurrentGridX = -1;
        protected internal int CurrentGridY = -1;
        protected internal bool AnalyzeMode = false;

        CommonModule commonmdl = new CommonModule();

        private int GridStartX;
        private int GridStartY;
        private string GridMsg = "";
        private Coordinate SolveHint = new Coordinate();
        private Font NumberFnt;
        private Font MemoFnt;
        private bool CompleteFlg;
        private double CompleteGrd;
        private bool CheckAnswerFlg;
        private bool HintFlg;
        private bool ChangeFlg;

        private Color[] PaletteColor;
        private Color[] PaletteLineColor;

        private HatchStyle HighlightHatch = HatchStyle.Percent50;
        private HatchStyle HighlightHatchB = HatchStyle.WideUpwardDiagonal;
        //HighlightHatch = HatchStyle.SolidDiamond



        private int CurrentLevel = 1;
        private int CurrentAssignCnt;

        private Coordinate[] enterHistory;
        private Coordinate[] enterHistoryB;
        private int currentHistoryNo;

        private SolvingTechnique UsedTechnique;
        private ToolboxStatus[] ToolboxInfo;

        struct SolvingTechnique
        {
            public bool NakedPairTriple;
            public bool HiddenPairTriple;
            public bool SimpleColors;
            public bool XWing;
            public bool XYWing;
            public bool SwordFish;
            public bool MultiColors;
        }

        struct ToolboxStatus
        {
            public string Name;
            public int Left;
            public int Top;
            public int Height;
            public int Width;
            public int UnitSize;
            public int Margin;
            public int StartX;
            public int HoverNo;
            public bool MouseDown;
            public int SelectedNo;
            public bool Visible;
        }


        public FormMain()
        {
            InitializeComponent();

        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Control ctl2;



            PaletteColor = new Color[] { Color.White, Color.LightBlue, Color.LightGreen, Color.LightPink, Color.LightSalmon, Color.White, Color.White };

            PaletteLineColor = new Color[] { Color.DarkBlue, Color.DarkGreen, Color.DeepPink, Color.OrangeRed };

            ChangeFlg = false;

            Set_Menu_DisplayItem();
            Reset_SudokuGrid();
            Load_Settings();
            AddHandle_ClickMenu();
            foreach (Control ctl in this.Panel_PictureBox.Controls)
            {
                if (ctl.GetType().Name == "PictureBox")
                {
                    if (ctl.Name == "PictureBoxGrid")
                    {
                        AddHandle_PictureBox(ctl);
                    }
                    else if (ctl.Name == "PictureBoxMemo")
                    {
                        AddHandle_PictureBoxMemo(ctl);
                    }
                    else if (ctl.Name == "PictureBoxPalette")
                    {
                        AddHandle_PictureBoxPalette(ctl);
                    }
                    else if (ctl.Name == "PictureBoxHighlight")
                    {
                        AddHandle_PictureBoxHighlight(ctl);
                    }
                }
            }





            //List<int> excludeNo = new List<int>();
            //int rnd;
            //for (int i = 0; i <= 10; i++) {
            //    rnd = commonmdl.Generate_RandomRange(1, 10, excludeNo);
            //    Console.Write(Convert.ToString(rnd) + " >> " + Convert.ToString(excludeNo.Count)  + "\r\n");
            //    excludeNo.Add(rnd);
            //}

            List<int>[] PickedNoListOrder = { new List<int>() };

            Get_Combinatorics(5, 2, ref PickedNoListOrder);

            for (int i = 0; i < PickedNoListOrder.Length; i++)
            {

                for (int j = 0; j < PickedNoListOrder[i].Count; j++)
                {
                    Debug.Write(Convert.ToString(PickedNoListOrder[i][j]) + ",");
                }
                Console.Write("\r\n");
            }

            //int ppp;

            //ppp = (int)(GridCount / 2);
            //Debug.Write(">>>>>>>>>>>>>>>>>>>>>>>>>>" + Convert.ToString(ppp));
            //MessageBox.Show(Convert.ToString(ppp));

            //MessageBox.Show(Convert.ToString(ddd));

            Set_Menu_SizeInfo();

            Set_Menu_LevelItem();
            Set_Menu_Mode();
            Reset_History();
            Display_NewQuestion();

            //Call Switch_KeypadDisplay();
            Switch_ToolButtonEnabled();

            ToolStripProfessionalRenderer proRenderer = new ToolStripProfessionalRenderer();

            proRenderer.RoundedEdges = false;

            this.ToolStrip1.Renderer = proRenderer;

        }


        private void Load_Settings()
        {

            foreach (SettingsProperty myCfg in Properties.Settings.Default.Properties)
            {

                if (myCfg.Name == "WindowSize")
                {
                    WindowSize = (int)Properties.Settings.Default[myCfg.Name];
                }
                if (myCfg.Name == "CurrentLevel")
                {
                    CurrentLevel = (int)Properties.Settings.Default[myCfg.Name];
                }
                if (myCfg.Name == "DisplayKeypad")
                {
                    this.Menu_DisplayKeypad.Checked = (bool)Properties.Settings.Default[myCfg.Name];
                }

            }
        }

        private void Reset_History()
        {
            int i, j, n = 0;

            enterHistory = new Coordinate[1];
            enterHistory[0] = new Coordinate();
            currentHistoryNo = 0;
            ChangeFlg = false;

            if (AnalyzeMode == true)
            {
                enterHistoryB = new Coordinate[1];
                enterHistoryB[0] = new Coordinate();

                for (j = 1; j <= GridCount; j++)
                {
                    for (i = 1; i <= GridCount; i++)
                    {
                        if (SudokuNumberGrid[i, j].FixNo > 0)
                        {
                            Array.Resize(ref enterHistoryB, n + 1);
                            enterHistoryB[n] = new Coordinate();
                            enterHistoryB[n].X = i;
                            enterHistoryB[n].Y = j;
                            enterHistoryB[n].No = SudokuNumberGrid[i, j].FixNo;
                            enterHistoryB[n].NoB = 1;
                            n++;
                        }
                    }
                }
            }

        }




        private void Set_Menu_DisplayItem()
        {
            String[,] ToolbarArray =
                          {
                           {"1", "Memo", "Check Prospect Number", "7", "1", "0"},
                           {"2", "Palette", "Color Palette", "7", "1", "0"},
                           {"3", "Highlight", "Highlight", "7", "1", "1"}
                          };

            ToolboxInfo = new ToolboxStatus[ToolbarArray.GetLength(0) + 1];

            //            MessageBox.Show(Convert.ToString(ToolbarArray[2, 0]));


            for (int i = 0; i <= ToolbarArray.GetLength(0) - 1; i++)
            {
                ToolStripMenuItem menuToolbar = new ToolStripMenuItem();
                menuToolbar.Name = "Menu_Toolbar_" + ToolbarArray[i, 1];
                menuToolbar.Text = ToolbarArray[i, 2];
                menuToolbar.Tag = Convert.ToString(ToolbarArray[i, 0]) + Convert.ToString(ToolbarArray[i, 4]) + Convert.ToString(ToolbarArray[i, 5]);
                menuToolbar.Checked = true;
                ToolboxInfo[Convert.ToInt32(ToolbarArray[i, 0])].Visible = true;
                ToolboxInfo[Convert.ToInt32(ToolbarArray[i, 0])].Name = ToolbarArray[i, 1];
                ToolboxInfo[Convert.ToInt32(ToolbarArray[i, 0])].Margin = Convert.ToInt32(ToolbarArray[i, 3]);

                menuToolbar.Click += new EventHandler(Menu_ToolbarChild_Click);
                this.Menu_Display.DropDownItems.Insert(i + 2, menuToolbar);
            }

            this.Menu_Display.DropDownItems.Add(new ToolStripSeparator());

        }

        private void Reset_SudokuGrid()
        {
            SudokuNumberGrid = new SudokuGrid[GridCount + 1, GridCount + 1];
            for (int i = 0; i <= GridCount; i++)
            {
                for (int j = 0; j <= GridCount; j++)
                {
                    SudokuNumberGrid[j, i] = new SudokuGrid();
                }
            }
            CompleteFlg = false;
            Reset_History();
            ChangeFlg = false;
        }


        private void AddHandle_PictureBox(Control myCtl)
        {
            myCtl.Paint += new PaintEventHandler(CtlPictureBox_Paint);
            myCtl.MouseDown += new MouseEventHandler(CtlPictureBox_MouseDown);
            myCtl.MouseWheel += new MouseEventHandler(CtlPictureBox_MouseWheel);

            //            AddHandler myCtl.Paint, AddressOf CtlPictureBox_Paint
            //            AddHandler myCtl.MouseDown, AddressOf CtlPictureBox_MouseDown
        }

        private void AddHandle_PictureBoxMemo(Control myCtl)
        {

            myCtl.Paint += new PaintEventHandler(CtlPictureBoxMemo_Paint);
            myCtl.MouseDown += new MouseEventHandler(CtlPictureBoxMemo_MouseDown);
            myCtl.MouseUp += new MouseEventHandler(CtlPictureBoxMemo_MouseUp);
            myCtl.MouseMove += new MouseEventHandler(CtlPictureBoxMemo_MouseMove);
            myCtl.MouseLeave += new EventHandler(CtlPictureBoxMemo_MouseLeave);
            //AddHandler myCtl.Paint, AddressOf CtlPictureBoxMemo_Paint
            //AddHandler myCtl.MouseDown, AddressOf CtlPictureBoxMemo_MouseDown
            //AddHandler myCtl.MouseUp, AddressOf CtlPictureBoxMemo_MouseUp
            //AddHandler myCtl.MouseMove, AddressOf CtlPictureBoxMemo_MouseMove
            //AddHandler myCtl.MouseLeave, AddressOf CtlPictureBoxMemo_MouseLeave


        }

        private void AddHandle_PictureBoxPalette(Control myCtl)
        {

            myCtl.Paint += new PaintEventHandler(CtlPictureBoxPalette_Paint);
            myCtl.MouseDown += new MouseEventHandler(CtlPictureBoxPalette_MouseDown);
            myCtl.MouseUp += new MouseEventHandler(CtlPictureBoxPalette_MouseUp);
            myCtl.MouseMove += new MouseEventHandler(CtlPictureBoxPalette_MouseMove);
            myCtl.MouseLeave += new EventHandler(CtlPictureBoxPalette_MouseLeave);
            //AddHandler myCtl.Paint, AddressOf CtlPictureBoxPalette_Paint
            //AddHandler myCtl.MouseDown, AddressOf CtlPictureBoxPalette_MouseDown
            //AddHandler myCtl.MouseUp, AddressOf CtlPictureBoxPalette_MouseUp
            //AddHandler myCtl.MouseMove, AddressOf CtlPictureBoxPalette_MouseMove
            //AddHandler myCtl.MouseLeave, AddressOf CtlPictureBoxPalette_MouseLeave
        }

        private void AddHandle_PictureBoxHighlight(Control myCtl)
        {

            myCtl.Paint += new PaintEventHandler(CtlPictureBoxHighlight_Paint);
            myCtl.MouseDown += new MouseEventHandler(CtlPictureBoxHighlight_MouseDown);
            myCtl.MouseUp += new MouseEventHandler(CtlPictureBoxHighlight_MouseUp);
            myCtl.MouseMove += new MouseEventHandler(CtlPictureBoxHighlight_MouseMove);
            myCtl.MouseLeave += new EventHandler(CtlPictureBoxHighlight_MouseLeave);
            //AddHandler myCtl.Paint, AddressOf CtlPictureBoxHighlight_Paint
            //AddHandler myCtl.MouseDown, AddressOf CtlPictureBoxHighlight_MouseDown
            //AddHandler myCtl.MouseUp, AddressOf CtlPictureBoxHighlight_MouseUp
            //AddHandler myCtl.MouseMove, AddressOf CtlPictureBoxHighlight_MouseMove
            //AddHandler myCtl.MouseLeave, AddressOf CtlPictureBoxHighlight_MouseLeave
        }

        private void AddHandle_ClickMenu()
        {
            Btn_Previous.Click += new EventHandler(Btn_Previous_Click);
            Btn_Next.Click += new EventHandler(Btn_Next_Click);
            Tool_Hint.Click += new EventHandler(Tool_Hint_Click);
            Menu_Hint.Click += new EventHandler(Tool_Hint_Click);
            Tool_CheckAnswer.Click += new EventHandler(Tool_CheckAnswer_Click);
            Menu_CheckAnswer.Click += new EventHandler(Tool_CheckAnswer_Click);

            Tool_DisplayAnswer.Click += new EventHandler(Tool_DisplayAnswer_Click);
            Menu_DisplayAnswer.Click += new EventHandler(Tool_DisplayAnswer_Click);

            Tool_NewQuestion.Click += new EventHandler(Tool_NewQuestion_Click);
            Menu_NewQuestion.Click += new EventHandler(Tool_NewQuestion_Click);

            Tool_File_Load.Click += new EventHandler(Menu_File_Load_Click);
            Menu_File_Load.Click += new EventHandler(Menu_File_Load_Click);
            Tool_File_Save.Click += new EventHandler(Menu_File_Save_Click);
            Menu_File_Save.Click += new EventHandler(Menu_File_Save_Click);

            Tool_Reset.Click += new EventHandler(Tool_Reset_Click);
            Tool_ResetAnswer.Click += new EventHandler(Tool_Reset_Click);
            Menu_Reset.Click += new EventHandler(Tool_Reset_Click);
            Menu_ResetAnswer.Click += new EventHandler(Tool_Reset_Click);

            HintTimer.Tick += new EventHandler(HintTimer_Tick);
        }


        private void CtlPictureBox_Paint(object sender, PaintEventArgs e)
        {
            Pen p1 = new Pen(Color.DarkOliveGreen, 1);
            Pen p2 = new Pen(Color.DarkOliveGreen, 3);
            Color HighlightColor;

            SolidBrush GridBrush;
            bool GridNoProspect;
            bool nonlockFlg;
            bool DuplicateFlg;

            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            StringFormat sfp = new StringFormat();
            sfp.LineAlignment = StringAlignment.Far;
            sfp.Alignment = StringAlignment.Center;

            SolidBrush numberBrush;
            SolidBrush HintNumberBrush = new SolidBrush(Color.Red);

            int n;
            SolidBrush memoBrush = new SolidBrush(Color.Maroon);

            int x1, x2, y1, y2;
            int tNo;

            Image imgRight = Properties.Resources.maru;
            Image imgWrong = Properties.Resources.batsu;
            Image imgRightWrong;
            Image imgFailure = Properties.Resources.failure;
            Rectangle destRect;
            Rectangle srcRect;

            //            GridSize = 40;

            System.Drawing.Imaging.ColorMatrix cm = new System.Drawing.Imaging.ColorMatrix();

            cm.Matrix00 = 1.0F;
            cm.Matrix11 = 1.0F;
            cm.Matrix22 = 1.0F;
            cm.Matrix33 = 0.5F;
            cm.Matrix44 = 1.0F;

            System.Drawing.Imaging.ImageAttributes ia = new System.Drawing.Imaging.ImageAttributes();
            ia.SetColorMatrix(cm);

            Switch_ToolButtonEnabled(true);

            if (CompleteFlg == true && CompleteGrd >= 1)
            {
                Image imgSheep = Properties.Resources.sheep3;
                double imgRate_w;
                double imgRate_h;

                if (imgSheep.Width > imgSheep.Height)
                {
                    imgRate_w = 0.9;
                    imgRate_h = 0.9 * (double)imgSheep.Height / (double)imgSheep.Width;
                }
                else
                {
                    imgRate_h = 0.9;
                    imgRate_w = 0.9 * (double)imgSheep.Width / (double)imgSheep.Height;
                }

                destRect = new Rectangle(GridStartX + (int)((1.0 - imgRate_w) * (GridSize * GridCount) / 2),
                                                      GridStartY + (int)((1.0 - imgRate_h) * (GridSize * GridCount) / 2),
                                                      (int)((GridSize * GridCount) * imgRate_w), (int)((GridSize * GridCount) * imgRate_h));
                srcRect = new Rectangle(0, 0, imgSheep.Width, imgSheep.Height);

                e.Graphics.DrawImage(imgSheep, destRect, srcRect, GraphicsUnit.Pixel);
                imgSheep.Dispose();
            }

            for (int j = 1; j <= GridCount; j++)
            {
                for (int i = 1; i <= GridCount; i++)
                {
                    GridBrush = new SolidBrush(SudokuNumberGrid[i, j].BackColor);
                    GridNoProspect = false;
                    DuplicateFlg = false;
                    if (SudokuNumberGrid[i, j].BackColor != Color.White)
                    {
                        GridBrush = new SolidBrush(SudokuNumberGrid[i, j].BackColor);
                    }
                    else if (AnalyzeMode == true)
                    {

                        if (DuplicateNumber(SudokuNumberGrid, new Coordinate(i, j)) == true)
                        { //重複
                            GridBrush = new SolidBrush(Color.Yellow);
                            DuplicateFlg = true;
                            //GridMsg = "重複しています"
                        }
                        else if (SudokuNumberGrid[i, j].ProspectNo.Count == 0)
                        { //破綻
                            destRect = new Rectangle(GridStartX + GridSize * (i - 1) + 3,
                                                     GridStartY + GridSize * (j - 1) + 3,
                                                     GridSize - 6, GridSize - 6);
                            srcRect = new Rectangle(0, 0, imgFailure.Width, imgFailure.Height);
                            e.Graphics.DrawImage(imgFailure, destRect, srcRect, GraphicsUnit.Pixel);
                        }

                    }


                    if (GridBrush.Color != Color.White)
                    {
                        e.Graphics.FillRectangle(GridBrush, GridStartX + GridSize * (i - 1),
                                                 GridStartY + GridSize * (j - 1),
                                                 GridSize, GridSize);
                    }

                    if (GridNoProspect == true)
                    {
                        x1 = GridStartX + GridSize * (i - 1) + 1;
                        x2 = x1 + GridSize - 2;
                        y1 = GridStartY + GridSize * (j - 1) + 1;
                        y2 = y1 + GridSize - 2;
                        e.Graphics.DrawLine(new Pen(Color.Orange, 1), x1, y1, x2, y2);
                        e.Graphics.DrawLine(new Pen(Color.Orange, 1), x1, y2, x2, y1);
                    }

                    tNo = Get_DimNo_From_ToolbarName("Highlight");

                    if (ToolboxInfo[tNo].SelectedNo > 0)
                    {
                        if (SudokuNumberGrid[i, j].FixNo == ToolboxInfo[tNo].SelectedNo)
                        {
                            if (AnalyzeMode == true)
                            {
                                HighlightColor = Color.LightGreen;
                            }
                            else
                            {
                                HighlightColor = Color.Gold;
                            }
                            if (SudokuNumberGrid[i, j].BackColor == Color.White)
                            {
                                e.Graphics.FillRectangle(new SolidBrush(HighlightColor),
                                                             GridStartX + GridSize * (i - 1),
                                                             GridStartY + GridSize * (j - 1),
                                                             GridSize, GridSize);
                            }
                            else
                            {
                                e.Graphics.FillRectangle(new HatchBrush(HighlightHatchB, SudokuNumberGrid[i, j].BackColor, HighlightColor),
                                                             GridStartX + GridSize * (i - 1),
                                                             GridStartY + GridSize * (j - 1),
                                                             GridSize, GridSize);
                            }

                        }
                        else if ((SudokuNumberGrid[i, j].FixNo == 0 && SudokuNumberGrid[i, j].ProspectNo.IndexOf(ToolboxInfo[tNo].SelectedNo) < 0)
                                || SudokuNumberGrid[i, j].FixNo > 0)
                        {
                            e.Graphics.FillRectangle(new HatchBrush(HighlightHatch, Color.Silver, Color.Transparent),
                                                     GridStartX + GridSize * (i - 1),
                                                     GridStartY + GridSize * (j - 1),
                                                     GridSize, GridSize);
                        }
                    }

                    if (CheckAnswerFlg == true && SudokuNumberGrid[i, j].FixNo > 0 && SudokuNumberGrid[i, j].Locked == false)
                    {
                        if (SudokuNumberGrid[i, j].FixError == false)
                        {
                            imgRightWrong = imgRight;
                        }
                        else
                        {
                            imgRightWrong = imgWrong;
                        }
                        destRect = new Rectangle(GridStartX + GridSize * (i - 1) + 3,
                                                     GridStartY + GridSize * (j - 1) + 3,
                                                     GridSize - 6, GridSize - 6);
                        srcRect = new Rectangle(0, 0, imgRightWrong.Width, imgRightWrong.Height);
                        //                        e.Graphics.DrawImage(imgRightWrong, destRect, srcRect, GraphicsUnit.Pixel, ia);

                        e.Graphics.DrawImage(imgRightWrong, destRect, 0, 0, imgRightWrong.Width, imgRightWrong.Height, GraphicsUnit.Pixel, ia);


                    }

                    //シンメトリー
                    if (AnalyzeMode == true && this.Chk_SymmetricGrid.Checked == true)
                    {
                        if (SudokuNumberGrid[i, j].FixNo == 0 && SudokuNumberGrid[GridCount - i + 1, GridCount - j + 1].FixNo > 0
                           && SudokuNumberGrid[GridCount - i + 1, GridCount - j + 1].Locked == true)
                        {
                            e.Graphics.FillRectangle(new HatchBrush(HatchStyle.OutlinedDiamond, Color.SkyBlue, Color.Transparent),
                                                     GridStartX + GridSize * (i - 1),
                                                     GridStartY + GridSize * (j - 1),
                                                     GridSize, GridSize);
                        }
                    }

                    if (DuplicateFlg == true)
                    {
                        e.Graphics.DrawString("Duplicate", MemoFnt, new SolidBrush(Color.Red),
                                   GridStartX + (int)(GridSize * (i - 0.5)),
                                   GridStartY + GridSize * (j),
                                   sfp);
                    }

                }
            }

            for (int j = 1; j <= GridCount; j++)
            {
                for (int i = 1; i <= GridCount; i++)
                {

                    if (SudokuNumberGrid[i, j].FixNo > 0)
                    {
                        //ヒント時に間違いがある場合、該当マスに×を表示
                        if (CheckAnswerFlg == false && SudokuNumberGrid[i, j].FixError == true && SolveHint.NoB == 0)
                        {
                            x1 = GridStartX + GridSize * (i - 1) + 3;
                            y1 = GridStartY + GridSize * (j - 1) + 3;
                            if (SolveHint.NoB == 0)
                            {
                                destRect = new Rectangle(x1, y1, GridSize - 6, GridSize - 6);
                                e.Graphics.DrawImage(imgWrong, destRect, 0, 0, imgWrong.Width, imgWrong.Height, GraphicsUnit.Pixel, ia);
                            }
                        }

                        if (SudokuNumberGrid[i, j].ForeColor != Color.Black)
                        {
                            numberBrush = new SolidBrush(SudokuNumberGrid[i, j].ForeColor);
                        }
                        else
                        {
                            if (SudokuNumberGrid[i, j].Locked == true)
                            {
                                numberBrush = new SolidBrush(Color.Black);
                            }
                            else
                            {
                                if (AnalyzeMode == true)
                                {
                                    numberBrush = new SolidBrush(Color.Coral);
                                }
                                else
                                {
                                    numberBrush = new SolidBrush(Color.Blue);
                                }
                                nonlockFlg = true;
                            }
                        }
                        e.Graphics.DrawString(Convert.ToString(SudokuNumberGrid[i, j].FixNo), NumberFnt, numberBrush,
                                        (float)(GridStartX + GridSize * (i - 0.5)), (float)(GridStartY + GridSize * (j - 0.5)), sf);
                    }


                    //メモNo表示
                    if (SudokuNumberGrid[i, j].MemoNo.Count > 0)
                    {
                        for (n = 0; n <= SudokuNumberGrid[i, j].MemoNo.Count - 1; n++)
                        {
                            if (SudokuNumberGrid[i, j].MemoNo[n] > 0)
                            {
                                e.Graphics.DrawString(Convert.ToString(Math.Abs(SudokuNumberGrid[i, j].MemoNo[n])), MemoFnt, memoBrush,
                                           (GridStartX + GridSize * (float)(i - 1 + (((Math.Abs(SudokuNumberGrid[i, j].MemoNo[n]) - 1) % 3) * 2 + 1) / 6.0)),
                                           (GridStartY + GridSize * (float)(j - 1 + (Math.Floor((double)((Math.Abs(SudokuNumberGrid[i, j].MemoNo[n]) - 1) / 3)) * 2 + 1) / 6.0) + 2),
                                           sf);
//                                (float)(GridStartX + GridSize * (i - 1 + (((Math.Abs(SudokuNumberGrid[i, j].MemoNo[n]) - 1) % 3) * 2 + 1) / 6)),
//                                (float)(GridStartY + GridSize * (j - 1 + (Math.Floor((decimal)((Math.Abs(SudokuNumberGrid[i, j].MemoNo[n]) - 1) / 3)) * 2 + 1) / 6) + 2),

                            }
                        }
                    }


                    //候補No表示（問題作成モード時）
                    if (AnalyzeMode == true || true)
                    {
                        if (SudokuNumberGrid[i, j].ProspectNo.Count > 0 && SudokuNumberGrid[i, j].FixNo == 0)
                        {
                            if (this.Chk_DisplayProspect.Checked == true || true)
                            {
                                for (n = 0; n < SudokuNumberGrid[i, j].ProspectNo.Count; n++)
                                {
                                    e.Graphics.DrawString(Convert.ToString(SudokuNumberGrid[i, j].ProspectNo[n]), MemoFnt, new SolidBrush(Color.MediumPurple),
                                               (float)(GridStartX + GridSize * (i - 1 + (float)(((SudokuNumberGrid[i, j].ProspectNo[n] - 1) % 3) * 2 + 1) / 6)),
                                               (float)(GridStartY + GridSize * (j - 1 + (Math.Floor((decimal)((SudokuNumberGrid[i, j].ProspectNo[n] - 1) / 3)) * 2 + 1) / 6) + 1),
                                               sf);
                                    //e.Graphics.DrawString(Convert.ToString(SudokuNumberGrid[i, j].ProspectNo[n]), MemoFnt, new SolidBrush(Color.MediumPurple),
                                    //           (float)(GridStartX + GridSize * (i - 1 + (((SudokuNumberGrid[i, j].ProspectNo[n] - 1) % 3) * 2 + 1) / 6)),
                                    //           (float)(GridStartY + GridSize * (j - 1 + (Math.Floor((decimal)((SudokuNumberGrid[i, j].ProspectNo[n] - 1) / 3)) * 2 + 1) / 6) + 1),
                                    //           sf);
                                }
                            }
                        }
                    }
                }
            }

            //ヒント
            if (SolveHint.X > 0 && SolveHint.Y > 0)
            {
                x1 = GridStartX + GridSize * (SolveHint.X - 1) + 3;
                y1 = GridStartY + GridSize * (SolveHint.Y - 1) + 3;
                Image imgSheep2 = Properties.Resources.sheep1;

                destRect = new Rectangle(x1, y1, GridSize - 6, GridSize - 6);
                e.Graphics.DrawImage(imgSheep2, destRect, 0, 0, imgSheep2.Width, imgSheep2.Height, GraphicsUnit.Pixel, ia);

                if (SolveHint.NoB > 0)
                {
                    e.Graphics.DrawString(Convert.ToString(SolveHint.NoB), NumberFnt, HintNumberBrush,
                                   (float)(GridStartX + GridSize * (SolveHint.X - 0.5)), (float)(GridStartY + GridSize * (SolveHint.Y - 0.5)), sf);
                }

            }


            //横線
            for (int i = 1; i <= GridCount; i++)
            {
                //３マス毎に線を太くする　
                if (i % 3 == 0)
                {
                    // p = p2;
                }
                else
                {
                    x1 = GridStartX;
                    x2 = GridStartX + GridSize * GridCount;
                    y1 = GridStartY + GridSize * i;
                    y2 = y1;
                    e.Graphics.DrawLine(p1, x1, y1, x2, y2);
                }
            }

            //縦
            for (int j = 1; j <= GridCount; j++)
            {
                //３マス毎に線を太くする　
                if (j % 3 == 0)
                {
                    // p = p2;
                }
                else
                {
                    y1 = GridStartY;
                    y2 = GridStartY + GridSize * GridCount;
                    x1 = GridStartX + GridSize * j;
                    x2 = x1;
                    e.Graphics.DrawLine(p1, x1, y1, x2, y2);
                }
            }

            //スクエア
            for (int i = 1; i <= GridCount; i++)
            {
                x1 = GridStartX + (Get_XY_From_SquareNo(i, 1).X - 1) * GridSize;
                x2 = x1 + GridSize * Convert.ToInt32(Math.Sqrt(GridCount)) - 1;
                y1 = GridStartY + (Get_XY_From_SquareNo(i, 1).Y - 1) * GridSize;
                y2 = y1 + GridSize * Convert.ToInt32(Math.Sqrt(GridCount)) - 1;
                e.Graphics.DrawRectangle(p2, x1, y1, GridSize * Convert.ToInt32(Math.Sqrt(GridCount)), GridSize * Convert.ToInt32(Math.Sqrt(GridCount)));
            }

            if (CurrentGridY > 0 && CurrentGridY <= GridCount && CurrentGridX > 0 && CurrentGridX <= GridCount)
            {
                //現在のマス目を強調表示
                e.Graphics.DrawRectangle(new Pen(Color.Red, 3), GridStartX + GridSize * (CurrentGridX - 1),
                                         GridStartY + GridSize * (CurrentGridY - 1),
                                         GridSize, GridSize);
            }

            //パズル完成時メッセージ表示
            if (CompleteFlg == true)
            {
                Font CompleteFnt = new Font("MS UI Gothic", NumberFnt.Size, FontStyle.Bold);
                String strComplete = "Congratulations!";

                x1 = (int)(GridStartX + (GridSize * GridCount) / 2 - e.Graphics.MeasureString(strComplete, CompleteFnt).Width / 2);
                x2 = (int)(x1 + e.Graphics.MeasureString(strComplete, CompleteFnt).Width);
                y1 = (int)(GridStartY + (GridSize * Math.Floor(Convert.ToDecimal(GridCount / 2))) - CompleteFnt.Height / 2);
                y2 = y1 + CompleteFnt.Height;

                if (CompleteGrd >= 1)
                {
                    e.Graphics.DrawString(strComplete, CompleteFnt, Brushes.Blue,
                            GridStartX + (GridSize * GridCount) / 2, (float)(GridStartY + (GridSize * Math.Floor(Convert.ToDecimal(GridCount / 2)))), sf);
                }
                else
                {
                    LinearGradientBrush bg = new LinearGradientBrush(new Point(x1, 0), new Point(Convert.ToInt32(x1 + (x2 - x1) * (CompleteGrd - Convert.ToInt32(CompleteGrd)) + 1), 0),
                                        Color.Red, Color.Yellow);
                    e.Graphics.DrawString(strComplete, CompleteFnt, bg,
                            GridStartX + (GridSize * GridCount) / 2, (float)(GridStartY + (GridSize * Math.Floor(Convert.ToDecimal(GridCount / 2)))), sf);
                    bg.Dispose();
                }
            }
            else if (GridMsg.Length > 0)
            {
                Font MsgFnt = new Font("MS UI Gothic", NumberFnt.Size - 4, FontStyle.Bold);
                SolidBrush bm1 = new SolidBrush(Color.Gold);
                SolidBrush bm2 = new SolidBrush(Color.FromArgb(128, Color.Black));


                x1 = (int)(GridStartX + (GridSize * GridCount) / 2 - e.Graphics.MeasureString(GridMsg, MsgFnt).Width / 2);
                x2 = (int)(x1 + e.Graphics.MeasureString(GridMsg, MsgFnt).Width);
                y1 = (int)(GridStartY + (GridSize * Math.Floor(Convert.ToDecimal(GridCount / 2))) - MsgFnt.Height / 2);
                y2 = y1 + MsgFnt.Height;

                e.Graphics.FillRectangle(bm2, new Rectangle(x1 - 3, y1 - 3, x2 - x1 + 6, y2 - y1 + 6));
                e.Graphics.DrawString(GridMsg, MsgFnt, bm1,
                        GridStartX + (GridSize * GridCount) / 2, (float)(GridStartY + (GridSize * Math.Floor(Convert.ToDecimal(GridCount / 2)))), sf);
                bm1.Dispose();
                bm2.Dispose();
            }

            if (currentHistoryNo > 0)
            {
                this.Btn_Previous.Image = Properties.Resources.arrow_l;
                this.Btn_Previous.Image.Tag = "True";
            }
            else
            {
                this.Btn_Previous.Image = Properties.Resources.arrow_ld;
                this.Btn_Previous.Image.Tag = "False";
            }

            //            MessageBox.Show(Convert.ToString(currentHistoryNo) + "----" + enterHistory.Length); 


            if (currentHistoryNo < enterHistory.Length - 1)
            {
                this.Btn_Next.Image = Properties.Resources.arrow_r;
                this.Btn_Next.Image.Tag = "True";
            }
            else
            {
                this.Btn_Next.Image = Properties.Resources.arrow_rd;
                this.Btn_Next.Image.Tag = "False";
            }


            //           MessageBox.Show(Convert.ToString(GridSize)); 


            //            MessageBox.Show(Convert.ToString(GridCount)); 
        }


        private void CtlPictureBox_MouseDown(object sender, MouseEventArgs e)
        {

            PictureBox myPicturebox = (PictureBox)sender;
            PictureBox memoPicturebox = this.PictureBoxMemo;
            PictureBox palettePicturebox = this.PictureBoxPalette;

            int x = myPicturebox.PointToClient(System.Windows.Forms.Cursor.Position).X;
            int y = myPicturebox.PointToClient(System.Windows.Forms.Cursor.Position).Y;
            int cX = 0, cY = 0, myNo;
            bool boolCheckNumber = false;

            if (e.Button == MouseButtons.Right)
            {
                Get_SudokuGridXY_From_Coordinate(x, y, ref cX, ref cY);
                if (cX == CurrentGridX && cY == CurrentGridY)
                {
                    if (SudokuNumberGrid[CurrentGridX, CurrentGridY].Locked == false || AnalyzeMode == true)
                    {
                        myNo = SudokuNumberGrid[CurrentGridX, CurrentGridY].FixNo + 1;
                        if (myNo < 0)
                        {
                            myNo = myNo + (GridCount + 1);
                        }
                        else if (myNo > GridCount)
                        {
                            myNo = myNo - (GridCount + 1);
                        }
                        Input_Number(myNo);
                    }
                }
            }
            else
            {
                Get_SudokuGridXY_From_Coordinate(x, y, ref CurrentGridX, ref CurrentGridY);
            }

            GridMsg = "";
            Reset_Hint();
            Reset_AnswerCheck();

            myPicturebox.Invalidate();
            memoPicturebox.Invalidate();
            palettePicturebox.Invalidate();
        }

        private void CtlPictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            int myPich, myNo;
            int x = this.PictureBoxGrid.PointToClient(System.Windows.Forms.Cursor.Position).X;
            int y = this.PictureBoxGrid.PointToClient(System.Windows.Forms.Cursor.Position).Y;
            int myX = 0, myY = 0;

            Get_SudokuGridXY_From_Coordinate(x, y, ref myX, ref myY);
            if (myX == CurrentGridX && myY == CurrentGridY
                && CurrentGridX > 0 && CurrentGridX <= GridCount
                && CurrentGridY > 0 && CurrentGridY <= GridCount)
            {
                if (SudokuNumberGrid[CurrentGridX, CurrentGridY].Locked == false || AnalyzeMode == true)
                {
                    if (e.Delta > 0)
                    {
                        myPich = -1;
                    }
                    else
                    {
                        myPich = 1;
                    }
                    myNo = SudokuNumberGrid[CurrentGridX, CurrentGridY].FixNo + myPich;
                    if (myNo < 0)
                    {
                        myNo = myNo + (GridCount + 1);
                    }
                    else if (myNo > GridCount)
                    {
                        myNo = myNo - (GridCount + 1);
                    }
                    Input_Number(myNo);
                }
            }

            //            MessageBox.Show(Convert.ToString (e.X));  
        }

        private void CtlPictureBoxMemo_Paint(Object senderb, PaintEventArgs e)
        {

            Pen penLine = new Pen(Color.Gray, 1);
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Near;

            StringFormat sfp = new StringFormat();
            sfp.LineAlignment = StringAlignment.Center;
            sfp.Alignment = StringAlignment.Center;
            Brush currentBrush;
            SolidBrush MemoBrush = new SolidBrush(Color.Navy);
            SolidBrush MemoBrushChecked = new SolidBrush(Color.White);
            Font mFnt;

            PictureBox sender = (PictureBox)senderb;

            SolidBrush prospectBrush = new SolidBrush(Color.Green);
            int x1, x2, y1, y2, tNo;

            tNo = Get_DimNo_From_ToolbarName("Memo");

            //外枠
            e.Graphics.DrawRectangle(new Pen(Color.DimGray, 1), 0, 0, sender.Width - 1, sender.Height - 1);
            e.Graphics.DrawRectangle(new Pen(Color.Silver, 1), 1, 1, sender.Width - 3, sender.Height - 3);

            e.Graphics.DrawString("Check", MemoFnt, MemoBrush,
                      GridStartX + 5, (float)(ToolboxInfo[tNo].UnitSize * 0.25) + ToolboxInfo[tNo].Margin, sf);
            e.Graphics.DrawString("Number", MemoFnt, MemoBrush,
                      GridStartX + 5, (float)(ToolboxInfo[tNo].UnitSize * 0.75) + ToolboxInfo[tNo].Margin, sf);

            for (int i = 1; i <= GridCount + 1; i++)
            {

                x1 = GridStartX + ToolboxInfo[tNo].StartX + (ToolboxInfo[tNo].UnitSize + ToolboxInfo[tNo].Margin) * (i - 1);
                y1 = ToolboxInfo[tNo].Margin;
                x2 = x1 + ToolboxInfo[tNo].UnitSize - 1;
                y2 = y1 + ToolboxInfo[tNo].UnitSize - 1;

                if (i == ToolboxInfo[tNo].HoverNo)
                {
                    Rectangle rect = new Rectangle(x1 - Convert.ToInt32(Math.Floor(Convert.ToDecimal(ToolboxInfo[tNo].Margin / 2))), y1 - Convert.ToInt32(Math.Floor(Convert.ToDecimal(ToolboxInfo[tNo].Margin / 2))), ToolboxInfo[tNo].UnitSize + ToolboxInfo[tNo].Margin, ToolboxInfo[tNo].UnitSize + ToolboxInfo[tNo].Margin);

                    if (ToolboxInfo[tNo].MouseDown == true)
                    {
                        ControlPaint.DrawBorder(e.Graphics, rect, Color.LightGray, ButtonBorderStyle.Inset);
                    }
                    else
                    {
                        ControlPaint.DrawBorder(e.Graphics, rect, Color.LightGray, ButtonBorderStyle.Outset);
                    }
                }
                e.Graphics.DrawRectangle(penLine, x1, y1, ToolboxInfo[tNo].UnitSize, ToolboxInfo[tNo].UnitSize);
                currentBrush = MemoBrush;
                if (CurrentGridX > 0 && CurrentGridX <= GridCount && CurrentGridY > 0 && CurrentGridY <= GridCount)
                {
                    if (SudokuNumberGrid[CurrentGridX, CurrentGridY].MemoNo.IndexOf(i) >= 0)
                    {
                        e.Graphics.FillRectangle(MemoBrush, x1 + 1, y1 + 1, ToolboxInfo[tNo].UnitSize - 1, ToolboxInfo[tNo].UnitSize - 1);
                        currentBrush = MemoBrushChecked;
                        mFnt = new Font(MemoFnt.FontFamily, MemoFnt.Size, FontStyle.Bold);
                    }
                }
                if (i <= GridCount)
                {
                    e.Graphics.DrawString(Convert.ToString(i), MemoFnt, currentBrush,
                        (float)(x1 + ToolboxInfo[tNo].UnitSize * 0.5 + 1), (float)(y1 + ToolboxInfo[tNo].UnitSize * 0.5), sfp);
                }
                else
                {
                    e.Graphics.DrawString("Del", MemoFnt, new SolidBrush(Color.Red),
                        (float)(x1 + ToolboxInfo[tNo].UnitSize * 0.5 + 1), (float)(y1 + ToolboxInfo[tNo].UnitSize * 0.5), sfp);
                }
            }
        }

        private void CtlPictureBoxMemo_MouseDown(object sender, MouseEventArgs e)
        {

            PictureBox memoPicturebox = (PictureBox)sender;
            int x = memoPicturebox.PointToClient(System.Windows.Forms.Cursor.Position).X;
            int y = memoPicturebox.PointToClient(System.Windows.Forms.Cursor.Position).Y;
            int SelectedMemoNo;
            int intScratch;
            int tNo;

            tNo = Get_DimNo_From_ToolbarName("Memo");

            ToolboxInfo[tNo].MouseDown = true;

            if (CurrentGridX > 0 && CurrentGridX <= GridCount && CurrentGridY > 0 && CurrentGridY <= GridCount)
            {
                if (SudokuNumberGrid[CurrentGridX, CurrentGridY].Locked == false)
                {
                    SelectedMemoNo = Get_MemoNo_From_Coordinate(x, y);
                    if (SelectedMemoNo > 0 && SelectedMemoNo <= GridCount)
                    {
                        if (SudokuNumberGrid[CurrentGridX, CurrentGridY].MemoNo.IndexOf(SelectedMemoNo) >= 0)
                        {
                            SudokuNumberGrid[CurrentGridX, CurrentGridY].MemoNo.Remove(SelectedMemoNo);
                        }
                        else
                        {
                            SudokuNumberGrid[CurrentGridX, CurrentGridY].MemoNo.Add(SelectedMemoNo);
                        }
                    }
                    else if (SelectedMemoNo == GridCount + 1 && SudokuNumberGrid[CurrentGridX, CurrentGridY].MemoNo.Count > 0)
                    {
                        SudokuNumberGrid[CurrentGridX, CurrentGridY].MemoNo.Clear();
                        ToolboxInfo[tNo].MouseDown = false;
                    }
                }
            }

            memoPicturebox.Invalidate();
            this.PictureBoxGrid.Invalidate();
        }

        private void CtlPictureBoxMemo_MouseUp(object sender, MouseEventArgs e)
        {

            PictureBox memoPicturebox = (PictureBox)sender;
            int tNo;

            tNo = Get_DimNo_From_ToolbarName("Memo");

            ToolboxInfo[tNo].MouseDown = false;
            memoPicturebox.Invalidate();
        }

        private void CtlPictureBoxMemo_MouseMove(object sender, MouseEventArgs e)
        {
            PictureBox memoPicturebox = (PictureBox)sender;
            int x = memoPicturebox.PointToClient(System.Windows.Forms.Cursor.Position).X;
            int y = memoPicturebox.PointToClient(System.Windows.Forms.Cursor.Position).Y;
            int tNo;

            tNo = Get_DimNo_From_ToolbarName("Memo");

            ToolboxInfo[tNo].HoverNo = Get_MemoNo_From_Coordinate(x, y);

            memoPicturebox.Invalidate();
        }

        private void CtlPictureBoxMemo_MouseLeave(object sender, EventArgs e)
        {

            PictureBox memoPicturebox = (PictureBox)sender;
            int tNo;

            tNo = Get_DimNo_From_ToolbarName("Memo");

            ToolboxInfo[tNo].HoverNo = 0;

            memoPicturebox.Invalidate();
        }


        private int Get_MemoNo_From_Coordinate(int x, int y)
        {

            int x1, x2, tNo;

            tNo = Get_DimNo_From_ToolbarName("Memo");

            if (y >= ToolboxInfo[tNo].Margin && y <= ToolboxInfo[tNo].Margin + ToolboxInfo[tNo].UnitSize)
            {
                for (int i = 1; i <= GridCount + 1; i++)
                {
                    x1 = GridStartX + ToolboxInfo[tNo].StartX + (ToolboxInfo[tNo].UnitSize + ToolboxInfo[tNo].Margin) * (i - 1);
                    x2 = x1 + ToolboxInfo[tNo].UnitSize;
                    if (x >= x1 && x <= x2)
                    {
                        return i;
                    }
                }
            }

            return 0;
        }

        private void CtlPictureBoxPalette_Paint(Object senderb, PaintEventArgs e)
        {

            Pen p;
            Pen p1 = new Pen(Color.Gray, 1);
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Near;

            StringFormat sfp = new StringFormat();
            sfp.LineAlignment = StringAlignment.Center;
            sfp.Alignment = StringAlignment.Center;

            SolidBrush PaletteBrush = new SolidBrush(Color.Navy);
            SolidBrush prospectBrush = new SolidBrush(Color.Green);
            int x1, x2, y1, y2, tNo;

            PictureBox sender = (PictureBox)senderb;

            tNo = Get_DimNo_From_ToolbarName("Palette");

            e.Graphics.DrawRectangle(new Pen(Color.DimGray, 1), 0, 0, sender.Width - 1, sender.Height - 1);
            e.Graphics.DrawRectangle(new Pen(Color.Silver, 1), 1, 1, sender.Width - 3, sender.Height - 3);

            e.Graphics.DrawString("Color Palette", MemoFnt, PaletteBrush,
                      GridStartX + 5, (float)(ToolboxInfo[tNo].UnitSize * 0.5 + ToolboxInfo[tNo].Margin), sf);

            for (int i = 1; i <= 6; i++)
            {
                x1 = GridStartX + (ToolboxInfo[tNo].UnitSize + ToolboxInfo[tNo].Margin) * (i - 1) + ToolboxInfo[tNo].StartX;
                y1 = ToolboxInfo[tNo].Margin;// '\ 2

                Rectangle rect = new Rectangle(x1 - Convert.ToInt32(Math.Floor(Convert.ToDecimal(ToolboxInfo[tNo].Margin / 2))), y1 - Convert.ToInt32(Math.Floor(Convert.ToDecimal(ToolboxInfo[tNo].Margin / 2))), ToolboxInfo[tNo].UnitSize + ToolboxInfo[tNo].Margin, ToolboxInfo[tNo].UnitSize + ToolboxInfo[tNo].Margin);

                if (i == ToolboxInfo[tNo].HoverNo)
                {
                    if (ToolboxInfo[tNo].MouseDown == true)
                    {
                        ControlPaint.DrawBorder(e.Graphics, rect, Color.LightGray, ButtonBorderStyle.Inset);
                    }
                    else
                    {
                        ControlPaint.DrawBorder(e.Graphics, rect, Color.LightGray, ButtonBorderStyle.Outset);
                    }
                }
                e.Graphics.FillRectangle(new SolidBrush(Color.Silver), x1 + 2, y1 + 2, ToolboxInfo[tNo].UnitSize + 1, ToolboxInfo[tNo].UnitSize + 1);
                p = p1;
                if (CurrentGridX > 0 && CurrentGridX <= GridCount && CurrentGridY > 0 && CurrentGridY <= GridCount)
                {
                    if (i <= 3 && SudokuNumberGrid[CurrentGridX, CurrentGridY].BackColor == PaletteColor[i])
                    {
                        p = new Pen(PaletteLineColor[i], 1);
                    }
                }
                e.Graphics.DrawRectangle(p, x1, y1, ToolboxInfo[tNo].UnitSize, ToolboxInfo[tNo].UnitSize);
                e.Graphics.FillRectangle(new SolidBrush(PaletteColor[i]), x1 + 1, y1 + 1, ToolboxInfo[tNo].UnitSize - 1, ToolboxInfo[tNo].UnitSize - 1);
            }

            e.Graphics.DrawString("Del", MemoFnt, new SolidBrush(Color.Red),
                              (float)(GridStartX + ToolboxInfo[tNo].StartX + (6 - 1) * (ToolboxInfo[tNo].UnitSize + ToolboxInfo[tNo].Margin) + 0.5 * ToolboxInfo[tNo].UnitSize),
                              (float)(ToolboxInfo[tNo].UnitSize * 0.5 + ToolboxInfo[tNo].Margin), sfp);

            Image[] imgSheep;
            imgSheep = new Image[5];
            double imgRate_w, imgRate_h;

            imgSheep[1] = Properties.Resources.sheep5;
            imgSheep[2] = Properties.Resources.sheep2;
            imgSheep[3] = Properties.Resources.sheep3;
            imgSheep[4] = Properties.Resources.sheep4;
            imgSheep[0] = Properties.Resources.sheep5;

            if (imgSheep[0].Width > imgSheep[0].Height)
            {
                imgRate_w = 1.0;
                imgRate_h = (double)imgSheep[0].Height / (double)imgSheep[0].Width;
            }
            else
            {
                imgRate_h = 1.0;
                imgRate_w = (double)imgSheep[0].Width / (double)imgSheep[0].Height;
            }

            int mySize = sender.Height;

            x1 = GridStartX + (ToolboxInfo[tNo].UnitSize + ToolboxInfo[tNo].Margin) * (8 - 1) + ToolboxInfo[tNo].StartX;
            y1 = 0;
            Rectangle destRect = new Rectangle(x1 + Convert.ToInt32((1.0 - imgRate_w) * mySize / 2),
                                                  y1 + Convert.ToInt32((1.0 - imgRate_h) * mySize / 2),
                                                  Convert.ToInt32(mySize * imgRate_w), Convert.ToInt32(mySize * imgRate_h));
            Rectangle srcRect = new Rectangle(0, 0, imgSheep[0].Width, imgSheep[0].Height);

            e.Graphics.DrawImage(imgSheep[0], destRect, srcRect, GraphicsUnit.Pixel);
        }


        private void CtlPictureBoxPalette_MouseDown(Object sender, MouseEventArgs e)
        {

            PictureBox palettePicturebox = (PictureBox)sender;
            int x = palettePicturebox.PointToClient(System.Windows.Forms.Cursor.Position).X;
            int y = palettePicturebox.PointToClient(System.Windows.Forms.Cursor.Position).Y;
            int SelectedPaletteNo, tNo;
            DialogResult result;

            tNo = Get_DimNo_From_ToolbarName("Palette");

            ToolboxInfo[tNo].MouseDown = true;

            SelectedPaletteNo = Get_PaletteNo_From_Coordinate(x, y);
            if (SelectedPaletteNo == 6)
            {
                palettePicturebox.Invalidate();
                result = MessageBox.Show("Whiten All Colored Grids", "Clear Color", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    for (int j = 1; j <= GridCount; j++)
                    {
                        for (int i = 1; i <= GridCount; i++)
                        {
                            SudokuNumberGrid[i, j].BackColor = Color.White;
                        }
                    }
                }
                ToolboxInfo[tNo].MouseDown = false;

            }
            else if (SelectedPaletteNo > 0)
            {
                if (CurrentGridX > 0 && CurrentGridX <= GridCount && CurrentGridY > 0 && CurrentGridY <= GridCount)
                {
                    if (SudokuNumberGrid[CurrentGridX, CurrentGridY].BackColor == PaletteColor[SelectedPaletteNo])
                    {
                        SudokuNumberGrid[CurrentGridX, CurrentGridY].BackColor = Color.White;
                    }
                    else
                    {
                        SudokuNumberGrid[CurrentGridX, CurrentGridY].BackColor = PaletteColor[SelectedPaletteNo];
                    }
                }
            }

            palettePicturebox.Invalidate();
            this.PictureBoxGrid.Invalidate();
        }

        private void CtlPictureBoxPalette_MouseUp(Object sender, MouseEventArgs e)
        {
            int tNo;
            tNo = Get_DimNo_From_ToolbarName("Palette");
            ToolboxInfo[tNo].MouseDown = false;
        }

        private void CtlPictureBoxPalette_MouseMove(Object sender, MouseEventArgs e)
        {

            PictureBox palettePicturebox = (PictureBox)sender;
            int x = palettePicturebox.PointToClient(System.Windows.Forms.Cursor.Position).X;
            int y = palettePicturebox.PointToClient(System.Windows.Forms.Cursor.Position).Y;
            int tNo;
            tNo = Get_DimNo_From_ToolbarName("Palette");

            ToolboxInfo[tNo].HoverNo = Get_PaletteNo_From_Coordinate(x, y);
            palettePicturebox.Invalidate();
        }

        private void CtlPictureBoxPalette_MouseLeave(Object sender, EventArgs e)
        {

            PictureBox palettePicturebox = (PictureBox)sender;
            int tNo;
            tNo = Get_DimNo_From_ToolbarName("Palette");

            ToolboxInfo[tNo].HoverNo = 0;
            palettePicturebox.Invalidate();
        }


        private int Get_PaletteNo_From_Coordinate(int x, int y)
        {

            int x1, x2, tNo;
            tNo = Get_DimNo_From_ToolbarName("Palette");


            if (y >= Math.Floor((double)(ToolboxInfo[tNo].Margin / 2)) && y <= Math.Floor((double)(ToolboxInfo[tNo].Margin / 2)) + ToolboxInfo[tNo].UnitSize)
            {
                for (int i = 1; i <= 6; i++)
                {
                    x1 = GridStartX + ToolboxInfo[tNo].StartX + (ToolboxInfo[tNo].UnitSize + ToolboxInfo[tNo].Margin) * (i - 1);// GridCount
                    x2 = x1 + ToolboxInfo[tNo].UnitSize;
                    if (x >= x1 && x <= x2)
                    {
                        return i;
                    }
                }
            }

            return 0;
        }


        private void CtlPictureBoxHighlight_Paint(Object senderb, PaintEventArgs e)
        {

            Pen p = new Pen(Color.Gray, 1);
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Near;

            StringFormat sfp = new StringFormat();
            sfp.LineAlignment = StringAlignment.Center;
            sfp.Alignment = StringAlignment.Center;

            Brush b;
            SolidBrush HighlightBrush = new SolidBrush(Color.Navy);
            SolidBrush HighlightBrushSelected = new SolidBrush(Color.White);
            Color HighlightColor;
            Font mFnt;

            PictureBox sender = (PictureBox)senderb;

            //Dim i As Integer, j As Integer, n As Integer
            int x1, x2, y1, y2, tNo;

            tNo = Get_DimNo_From_ToolbarName("Highlight");

            e.Graphics.DrawRectangle(new Pen(Color.DimGray, 1), 0, 0, sender.Width - 1, sender.Height - 1);
            e.Graphics.DrawRectangle(new Pen(Color.Silver, 1), 1, 1, sender.Width - 3, sender.Height - 3);
            e.Graphics.DrawString("Highlight", MemoFnt, HighlightBrush,
                      GridStartX + 5, (float)(ToolboxInfo[tNo].UnitSize * 0.5) + ToolboxInfo[tNo].Margin, sf);

            for (int i = 1; i <= GridCount; i++)
            {
                x1 = GridStartX + ToolboxInfo[tNo].StartX + (ToolboxInfo[tNo].UnitSize + ToolboxInfo[tNo].Margin) * (i - 1);
                y1 = ToolboxInfo[tNo].Margin;
                Rectangle rect = new Rectangle(x1 - Convert.ToInt32(ToolboxInfo[tNo].Margin / 2),
                                               y1 - Convert.ToInt32(ToolboxInfo[tNo].Margin / 2),
                                                ToolboxInfo[tNo].UnitSize + ToolboxInfo[tNo].Margin,
                                                ToolboxInfo[tNo].UnitSize + ToolboxInfo[tNo].Margin);
                if (i == ToolboxInfo[tNo].HoverNo)
                {
                    if (ToolboxInfo[tNo].MouseDown == true)
                    {
                        ControlPaint.DrawBorder(e.Graphics, rect, Color.LightGray, ButtonBorderStyle.Inset);
                    }
                    else
                    {
                        ControlPaint.DrawBorder(e.Graphics, rect, Color.LightGray, ButtonBorderStyle.Outset);
                    }
                }
                e.Graphics.DrawRectangle(p, x1, y1, ToolboxInfo[tNo].UnitSize, ToolboxInfo[tNo].UnitSize);

                if (i == ToolboxInfo[tNo].SelectedNo)
                {
                    if (AnalyzeMode == true)
                    {
                        HighlightColor = Color.LightGreen;
                    }
                    else
                    {
                        HighlightColor = Color.Gold;
                    }
                    e.Graphics.FillRectangle(new SolidBrush(HighlightColor), x1 + 1, y1 + 1, ToolboxInfo[tNo].UnitSize - 1, ToolboxInfo[tNo].UnitSize - 1);
                    mFnt = new Font(MemoFnt.FontFamily, MemoFnt.Size, FontStyle.Bold);
                }
                else
                {
                    e.Graphics.FillRectangle(new HatchBrush(HighlightHatch, Color.Gainsboro, Color.White),
                             x1 + 1, y1 + 1, ToolboxInfo[tNo].UnitSize - 1, ToolboxInfo[tNo].UnitSize - 1);
                    mFnt = MemoFnt;
                }
                e.Graphics.DrawString(Convert.ToString(i), mFnt, HighlightBrush,
                        (float)(x1 + ToolboxInfo[tNo].UnitSize * 0.5 + 1), (float)(y1 + ToolboxInfo[tNo].UnitSize * 0.5), sfp);
            }
        }


        private void CtlPictureBoxHighlight_MouseDown(Object sender, MouseEventArgs e)
        {

            PictureBox highlightPicturebox = (PictureBox)sender;
            int x = highlightPicturebox.PointToClient(System.Windows.Forms.Cursor.Position).X;
            int y = highlightPicturebox.PointToClient(System.Windows.Forms.Cursor.Position).Y;
            int SelectedNo, tNo;

            tNo = Get_DimNo_From_ToolbarName("Highlight");
            ToolboxInfo[tNo].MouseDown = true;

            SelectedNo = Get_HighlightNo_From_Coordinate(x, y);
            Switch_Highlight(SelectedNo);
        }

        private void Switch_Highlight(int SelectedNo)
        {

            int tNo;
            tNo = Get_DimNo_From_ToolbarName("Highlight");

            if (SelectedNo > 0)
            {
                if (ToolboxInfo[tNo].SelectedNo == SelectedNo)
                {
                    ToolboxInfo[tNo].SelectedNo = 0;
                }
                else
                {
                    ToolboxInfo[tNo].SelectedNo = SelectedNo;
                }
            }

            this.PictureBoxHighlight.Invalidate();
            this.PictureBoxGrid.Invalidate();
        }

        private void CtlPictureBoxHighlight_MouseUp(Object sender, MouseEventArgs e)
        {

            PictureBox highlightPicturebox = (PictureBox)sender;
            int tNo;

            tNo = Get_DimNo_From_ToolbarName("Highlight");

            ToolboxInfo[tNo].MouseDown = false;
            highlightPicturebox.Invalidate();
        }

        private void CtlPictureBoxHighlight_MouseMove(Object sender, MouseEventArgs e)
        {

            PictureBox highlightPicturebox = (PictureBox)sender;
            int x = highlightPicturebox.PointToClient(System.Windows.Forms.Cursor.Position).X;
            int y = highlightPicturebox.PointToClient(System.Windows.Forms.Cursor.Position).Y;
            int tNo;

            tNo = Get_DimNo_From_ToolbarName("Highlight");

            ToolboxInfo[tNo].HoverNo = Get_HighlightNo_From_Coordinate(x, y);

            highlightPicturebox.Invalidate();
        }

        private void CtlPictureBoxHighlight_MouseLeave(Object sender, EventArgs e)
        {

            PictureBox highlightPicturebox = (PictureBox)sender;
            int tNo;

            tNo = Get_DimNo_From_ToolbarName("Highlight");

            ToolboxInfo[tNo].HoverNo = 0;
            highlightPicturebox.Invalidate();
        }

        private int Get_HighlightNo_From_Coordinate(int x, int y)
        {
            int x1, x2, tNo;
            tNo = Get_DimNo_From_ToolbarName("Highlight");

            if (y >= ToolboxInfo[tNo].Margin && y <= ToolboxInfo[tNo].Margin + ToolboxInfo[tNo].UnitSize)
            {
                for (int i = 1; i <= GridCount; i++)
                {
                    x1 = GridStartX + ToolboxInfo[tNo].StartX + (ToolboxInfo[tNo].UnitSize + ToolboxInfo[tNo].Margin) * (i - 1);
                    x2 = x1 + ToolboxInfo[tNo].UnitSize;
                    if (x >= x1 && x <= x2)
                    {
                        return i;
                    }
                }
            }

            return 0;
        }


        private void Switch_ToolButtonEnabled()
        {
            Switch_ToolButtonEnabled(false);
        }

        private void Switch_ToolButtonEnabled(bool skipFlg)
        {
            bool flgFilledNum;

            if (skipFlg == false)
            {
                if (AnalyzeMode == true)
                {
                    this.Tool_Hint.Visible = false;
                    this.Tool_CheckAnswer.Visible = false;
                    this.Menu_Hint.Visible = false;
                    this.Menu_CheckAnswer.Visible = false;

                    this.Tool_DisplayAnswer.Enabled = true;
                    this.Menu_DisplayAnswer.Enabled = true;

                    this.Tool_DisplayAnswer.Image = Properties.Resources.RightAnswerCheck;
                    this.Tool_DisplayAnswer.Text = "Check Right Answer";
                    this.Menu_DisplayAnswer.Text = "Check Right Answer";

                    this.Tool_Reset.Visible = false;
                    this.Menu_Reset.Visible = false;

                    this.Tool_ResetAnswer.Visible = true;
                    this.Menu_ResetAnswer.Visible = true;
                }
                else
                {
                    this.Tool_Hint.Visible = true;
                    this.Tool_CheckAnswer.Visible = true;
                    this.Menu_Hint.Visible = true;
                    this.Menu_CheckAnswer.Visible = true;

                    this.Tool_Reset.Visible = true;
                    this.Menu_Reset.Visible = true;

                    this.Tool_ResetAnswer.Visible = false;
                    this.Menu_ResetAnswer.Visible = false;

                    this.Tool_DisplayAnswer.Image = Properties.Resources.RightAnswer;
                    this.Tool_DisplayAnswer.Text = "Display Answer";
                    this.Menu_DisplayAnswer.Text = "Display Answer";
                }
            }

            if (AnalyzeMode == true)
            {
                this.Tool_ResetAnswer.Enabled = Check_FilledNumber();
                this.Menu_ResetAnswer.Enabled = Check_FilledNumber();
                this.Tool_File_Save.Enabled = true;
                this.Menu_File_Save.Enabled = true;
            }
            else
            {
                if (Check_FixAll() == true)
                {
                    this.Tool_File_Save.Enabled = false;
                    this.Tool_Hint.Enabled = false;
                    this.Tool_CheckAnswer.Enabled = false;
                    this.Tool_Reset.Enabled = false;
                    this.Tool_DisplayAnswer.Enabled = false;
                    this.Menu_File_Save.Enabled = false;
                    this.Menu_Hint.Enabled = false;
                    this.Menu_CheckAnswer.Enabled = false;
                    this.Menu_Reset.Enabled = false;
                    this.Menu_DisplayAnswer.Enabled = false;
                }
                else
                {
                    flgFilledNum = Check_FilledNumber();
                    this.Tool_File_Save.Enabled = true;
                    this.Tool_CheckAnswer.Enabled = flgFilledNum;
                    this.Tool_Reset.Enabled = flgFilledNum;
                    this.Menu_File_Save.Enabled = true;
                    this.Menu_CheckAnswer.Enabled = flgFilledNum;
                    this.Menu_Reset.Enabled = flgFilledNum;
                    if (CompleteFlg == true)
                    {
                        this.Tool_Hint.Enabled = false;
                        this.Menu_Hint.Enabled = false;
                        this.Tool_DisplayAnswer.Enabled = false;
                        this.Menu_DisplayAnswer.Enabled = false;
                    }
                    else
                    {
                        this.Tool_Hint.Enabled = true;
                        this.Menu_Hint.Enabled = true;
                        this.Tool_DisplayAnswer.Enabled = true;
                        this.Menu_DisplayAnswer.Enabled = true;
                    }
                }
            }
        }

        private bool Check_FilledNumber()
        {
            for (int j = 1; j <= GridCount; j++)
            {
                for (int i = 1; i <= GridCount; i++)
                {
                    if (SudokuNumberGrid[i, j].FixNo > 0 && SudokuNumberGrid[i, j].Locked == false)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool Check_FixAll()
        {
            for (int j = 1; j <= GridCount; j++)
            {
                for (int i = 1; i <= GridCount; i++)
                {
                    if (SudokuNumberGrid[i, j].Locked == false)
                    {
                        return false;
                    }
                }
            }
            return true;

        }

        private void Menu_SizeInfo_Click(System.Object senderb, System.EventArgs e)
        {

            foreach (ToolStripMenuItem myMenu in this.Menu_Size.DropDownItems)
            {
                myMenu.Checked = false;
            }

            ToolStripMenuItem sender = (ToolStripMenuItem)senderb;

            sender.Checked = true;

            WindowSize = Convert.ToInt32(Convert.ToString(sender.Tag).Substring(0, 2));

            //            CommonModule cm = new CommonModule();
            if (commonmdl.Exist_Settings("WindowSize") == true)
            {
                Properties.Settings.Default["WindowSize"] = WindowSize;
            }

            GridSize = Convert.ToInt32(Convert.ToString(sender.Tag).Substring(2, 3));

            //            MessageBox.Show(Convert.ToString(GridSize)); 

            NumberFnt = new Font("MS UI Gothic", Convert.ToInt32(Convert.ToString(sender.Tag).Substring(5, 3)), FontStyle.Bold);
            MemoFnt = new Font("MS UI Gothic", Convert.ToInt32(Convert.ToString(sender.Tag).Substring(8, 3)), FontStyle.Regular);

            if (GridCount > 0)
            {
                Set_Grid(this.PictureBoxGrid, this.PictureBoxMemo, this.PictureBoxPalette, this.PictureBoxHighlight);
            }
        }

        private void Set_Menu_SizeInfo()
        {

            string[,] SizeInfoArray =
                          {
                           {"1", "Extra Large", "60", "32", "12"},
                           {"2", "Large", "50", "24", "10"},
                           {"3", "Middle", "40", "20", "9"},
                           {"4", "Small", "35", "16", "8"}
                          };

            for (int i = 0; i <= SizeInfoArray.GetLength(0) - 1; i++)
            {
                ToolStripMenuItem menuSize = new ToolStripMenuItem();
                System.EventArgs e = new System.EventArgs();
                menuSize.Text = SizeInfoArray[i, 1];
                menuSize.Tag = String.Format("{0:D2}", Convert.ToInt32(SizeInfoArray[i, 0])) + String.Format("{0:D3}", Convert.ToInt32(SizeInfoArray[i, 2]))
                                + String.Format("{0:D3}", Convert.ToInt32(SizeInfoArray[i, 3])) + String.Format("{0:D3}", Convert.ToInt32(SizeInfoArray[i, 4]));
                if (Convert.ToInt32(SizeInfoArray[i, 0]) == WindowSize)
                {
                    menuSize.Checked = true;
                    Menu_SizeInfo_Click(menuSize, e);
                }
                else
                {
                    menuSize.Checked = false;
                }

                menuSize.Click += new EventHandler(Menu_SizeInfo_Click);

                this.Menu_Size.DropDownItems.Add(menuSize);
            }
        }


        private void Menu_ToolbarChild_Click(Object sender, EventArgs e)
        {
            ToolStripMenuItem curTool = (ToolStripMenuItem)sender;

            ToolboxInfo[Convert.ToInt32(Convert.ToString(curTool.Tag).Substring(0, 1))].Visible = !curTool.Checked;

            curTool.Checked = !curTool.Checked;

            Set_Grid(this.PictureBoxGrid, this.PictureBoxMemo, this.PictureBoxPalette, this.PictureBoxHighlight);


        }


        private void Set_Grid(PictureBox myPicturebox, PictureBox memoPicturebox, PictureBox palettePicturebox, PictureBox highlightPicturebox)
        {

            int MarginX = 10;
            int MarginY = 10;
            int wdd, myW, myH, h, w, tNo;

            this.BackColor = Color.White;

            //ディスプレイの高さ
            h = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            //ディスプレイの幅
            w = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;

            if (CurrentGridX <= 0 && CurrentGridY <= 0)
            {
                CurrentGridX = GridCount / 2 + GridCount % 2;
                CurrentGridY = GridCount / 2 + GridCount % 2;
            }
            GridStartX = 1; //MarginX
            GridStartY = MarginY;

            if (AnalyzeMode == true)
            {
                this.Panel_ChkBox.Visible = true;
                this.LblLevel.Visible = false;
                this.LblLevel.Tag = "";
                this.Menu_Level.Enabled = false;
            }
            else
            {
                this.Panel_ChkBox.Visible = false;
                this.LblLevel.Visible = true;
                this.Menu_Level.Enabled = true;
            }

            wdd = 0;
            myH = this.MenuStrip1.Height + this.ToolStrip1.Height + 5;


            //            MessageBox.Show(Convert.ToString(GridSize));

            myPicturebox.Size = new Size(GridStartX + GridSize * GridCount + MarginX,
                                         GridStartY + GridSize * GridCount + MarginY / 2);
            myPicturebox.BackColor = Color.Transparent;//   'Color.White
            myPicturebox.Top = myH;

            myH = myH + myPicturebox.Height;

            this.Panel_ChkBox.Left = myPicturebox.Left;
            this.Panel_ChkBox.Top = myH;

            this.LblLevel.Top = myH;
            this.LblLevel.Left = myPicturebox.Left - MarginX;
            this.LblLevel.Width = myPicturebox.Width;
            this.LblLevel.TextAlign = ContentAlignment.TopRight;

            myH = myH + this.LblLevel.Height;

            tNo = Get_DimNo_From_ToolbarName("Memo");
            if (tNo > 0)
            {
                ToolboxInfo[tNo].Left = myPicturebox.Left;
                ToolboxInfo[tNo].UnitSize = Convert.ToInt32(GridSize * 0.6);
                ToolboxInfo[tNo].StartX = Convert.ToInt32(MemoFnt.Size * 7);// '+ 20
                ToolboxInfo[tNo].Width = ToolboxInfo[tNo].StartX + (ToolboxInfo[tNo].UnitSize + ToolboxInfo[tNo].Margin) * (GridCount + 1) + 10;
                ToolboxInfo[tNo].Height = ToolboxInfo[tNo].UnitSize + ToolboxInfo[tNo].Margin * 2;
                if (wdd > ToolboxInfo[tNo].Width)
                {
                    ToolboxInfo[tNo].Width = wdd;
                }
                else
                {
                    wdd = ToolboxInfo[tNo].Width;
                }
                ToolboxInfo[tNo].Top = myH;
                if (ToolboxInfo[tNo].Visible == true)
                {
                    myH = myH + ToolboxInfo[tNo].Height - 1;
                }
                memoPicturebox.Left = ToolboxInfo[tNo].Left;
                memoPicturebox.Top = ToolboxInfo[tNo].Top;
                memoPicturebox.Height = ToolboxInfo[tNo].Height;
                memoPicturebox.BackColor = Color.Transparent;//   'Color.White
                memoPicturebox.Visible = ToolboxInfo[tNo].Visible;
            }

            tNo = Get_DimNo_From_ToolbarName("Palette");
            if (tNo > 0)
            {
                ToolboxInfo[tNo].Left = myPicturebox.Left;
                ToolboxInfo[tNo].UnitSize = Convert.ToInt32(GridSize * 0.6);
                ToolboxInfo[tNo].StartX = Convert.ToInt32(MemoFnt.Size * 7) + 20;
                ToolboxInfo[tNo].Width = ToolboxInfo[tNo].StartX + (ToolboxInfo[tNo].UnitSize + ToolboxInfo[tNo].Margin) * 6 + 10;
                ToolboxInfo[tNo].Height = ToolboxInfo[tNo].UnitSize + ToolboxInfo[tNo].Margin * 2;
                if (wdd > ToolboxInfo[tNo].Width)
                {
                    ToolboxInfo[tNo].Width = wdd;
                }
                else
                {
                    wdd = ToolboxInfo[tNo].Width;
                }
                ToolboxInfo[tNo].Top = myH;
                if (ToolboxInfo[tNo].Visible == true)
                {
                    myH = myH + ToolboxInfo[tNo].Height - 1;
                }

                palettePicturebox.Left = ToolboxInfo[tNo].Left;
                palettePicturebox.Top = ToolboxInfo[tNo].Top;
                palettePicturebox.Height = ToolboxInfo[tNo].Height;
                palettePicturebox.BackColor = Color.Transparent;//   'Color.White
                palettePicturebox.Visible = ToolboxInfo[tNo].Visible;
            }

            tNo = Get_DimNo_From_ToolbarName("Highlight");
            if (tNo > 0)
            {
                ToolboxInfo[tNo].Left = myPicturebox.Left;
                ToolboxInfo[tNo].UnitSize = Convert.ToInt32(GridSize * 0.6);
                ToolboxInfo[tNo].StartX = Convert.ToInt32(MemoFnt.Size * 7);// '+ 20
                ToolboxInfo[tNo].Width = ToolboxInfo[tNo].StartX + (ToolboxInfo[tNo].UnitSize + ToolboxInfo[tNo].Margin) * GridCount + 10;
                ToolboxInfo[tNo].Height = ToolboxInfo[tNo].UnitSize + ToolboxInfo[tNo].Margin * 2;
                if (wdd > ToolboxInfo[tNo].Width)
                {
                    ToolboxInfo[tNo].Width = wdd;
                }
                else
                {
                    wdd = ToolboxInfo[tNo].Width;
                }
                ToolboxInfo[tNo].Top = myH;
                if (ToolboxInfo[tNo].Visible == true)
                {
                    myH = myH + ToolboxInfo[tNo].Height;
                }

                highlightPicturebox.Left = ToolboxInfo[tNo].Left;
                highlightPicturebox.Top = ToolboxInfo[tNo].Top;
                highlightPicturebox.Height = ToolboxInfo[tNo].Height;
                highlightPicturebox.BackColor = Color.Transparent;//   'Color.White
                highlightPicturebox.Visible = ToolboxInfo[tNo].Visible;
            }


            palettePicturebox.Width = wdd;
            memoPicturebox.Width = wdd;
            highlightPicturebox.Width = wdd;

            if (myPicturebox.Width > wdd)
            {
                wdd = myPicturebox.Width;
            }

            myW = myPicturebox.Left + wdd + 30;
            myH = myH + 50;

            if (myW > w - 30)
            {
                myW = w - 30;
            }
            else if (myW < 410)
            {
                myW = 410;
            }
            if (myH > h - 70)
            {
                myH = h - 70;
            }

            myPicturebox.Invalidate();
            palettePicturebox.Invalidate();
            memoPicturebox.Invalidate();
            highlightPicturebox.Invalidate();

            this.Height = myH;
            this.Width = myW;

            this.Left = (w - myW) / 2;
            this.Top = (h - myH) / 2;

            //           FormNumberKey.Create_NumberKeyButton();
        }


        private bool Get_SudokuGridXY_From_Coordinate(int x, int y, ref int GridX, ref int GridY)
        {

            int prevX, prevY;

            prevX = CurrentGridX;
            prevY = CurrentGridY;

            GridX = Convert.ToInt32((x - GridStartX) / GridSize) + 1;
            GridY = Convert.ToInt32((y - GridStartY) / GridSize) + 1;

            if (GridX > 0 && GridX <= GridCount && GridY > 0 && GridY <= GridCount)
            {
                if (prevX != GridX || prevY != GridY)
                {
                    enterHistory[currentHistoryNo].NoB = 0;
                }
                return true;
            }

            return false;

        }


        private int Get_DimNo_From_ToolbarName(String myToolbarName)
        {

            for (int i = 1; i <= ToolboxInfo.Length + 1; i++)
            {
                if (ToolboxInfo[i].Name == myToolbarName)
                {
                    return i;
                }
            }
            return 0;
        }


        private int Get_SquareNo(int x, int y, ref int sNo)
        {

            sNo = ((y - 1) % 3) * 3 + ((x - 1) % 3) + 1;
            return ((y - 1) / 3) * 3 + (((x - 1) / 3) % 3) + 1;

        }

        private Coordinate Get_XY_From_SquareNo(int s, int sNo)
        {

            Coordinate coord = new Coordinate();

            coord.X = ((s - 1) % 3) * 3 + ((sNo - 1) % 3) + 1;
            coord.Y = ((s - 1) / 3) * 3 + ((sNo - 1) / 3) + 1;

            return coord;
        }

        private bool IsSameGroup(Coordinate myCoordinateA, Coordinate myCoordinateB)
        {

            int sNo = 0;

            if (myCoordinateA.X == myCoordinateB.X || myCoordinateA.Y == myCoordinateB.Y
                   || Get_SquareNo(myCoordinateA.X, myCoordinateA.Y, ref sNo) == Get_SquareNo(myCoordinateB.X, myCoordinateB.Y, ref sNo))
            {
                return true;
            }
            return false;

        }

        //'
        //'  各マスの候補Noを適正化
        //'
        private void Adjust_ProspectNo(ref SudokuGrid[,] tmpNumberGrid)
        {

            int i, j;

            for (j = 1; j <= GridCount; j++)
            {
                for (i = 1; i <= GridCount; i++)
                {
                    tmpNumberGrid[i, j].Reset_ProspectNo();
                }
            }

            for (j = 1; j <= GridCount; j++)
            {
                for (i = 1; i <= GridCount; i++)
                {
                    if (tmpNumberGrid[i, j].FixNo > 0)
                    {
                        Remove_ProspectNo(1, new Coordinate(i, j, 0, tmpNumberGrid[i, j].FixNo, 0), ref tmpNumberGrid);
                    }
                }
            }

        }






        private bool DuplicateNumber(SudokuGrid[,] tmpNumberGrid, Coordinate myCoordinate)
        {

            for (int y = 1; y <= GridCount; y++)
            {
                for (int x = 1; x <= GridCount; x++)
                {
                    if ((myCoordinate.X == 0 || myCoordinate.X == x) && (myCoordinate.Y == 0 || myCoordinate.Y == y))
                    {
                        if (tmpNumberGrid[x, y].FixNo > 0)
                        {
                            for (int y2 = 1; y2 <= GridCount; y2++)
                            {
                                for (int x2 = 1; x2 <= GridCount; x2++)
                                {
                                    if (IsSameGroup(new Coordinate(x, y), new Coordinate(x2, y2)) == true && (x != x2 || y != y2))
                                    {
                                        if (tmpNumberGrid[x, y].FixNo == tmpNumberGrid[x2, y2].FixNo)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }


        private void Input_Number(int myKeyNo)
        {

            SudokuNumberGrid[CurrentGridX, CurrentGridY].FixNo = myKeyNo;
            SudokuNumberGrid[CurrentGridX, CurrentGridY].FixError = false;

            if (SudokuNumberGrid[CurrentGridX, CurrentGridY].BackColor == Color.LightSteelBlue)
            {
                SudokuNumberGrid[CurrentGridX, CurrentGridY].BackColor = Color.White;
            }

            if (myKeyNo == 0)
            {
                SudokuNumberGrid[CurrentGridX, CurrentGridY].Locked = false;
            }
            else
            {
                SudokuNumberGrid[CurrentGridX, CurrentGridY].Locked = AnalyzeMode;
            }

            Adjust_ProspectNo(ref SudokuNumberGrid);
            Add_enterHistory();
            Reset_Hint();
            Reset_AnswerCheck();

            if (AnalyzeMode == false)
            {
                //              Display_CompleteWindow();
            }
            else
            {
                DuplicateNumber(SudokuNumberGrid, new Coordinate(CurrentGridX, CurrentGridY));
            }

            //            DuplicateNumber(SudokuNumberGrid, new Coordinate(CurrentGridX, CurrentGridY));


            this.PictureBoxGrid.Invalidate();
        }


        private void Add_enterHistory()
        {

            int hNo;

            hNo = enterHistory.Length - 1;

            if (enterHistory[hNo].X != CurrentGridX || enterHistory[hNo].Y != CurrentGridY
                                    || enterHistory[currentHistoryNo].NoB == 0)
            {
                currentHistoryNo = currentHistoryNo + 1;
                if (currentHistoryNo >= enterHistory.Length)
                {
                    Array.Resize(ref enterHistory, currentHistoryNo + 1);
                }
                ChangeFlg = true;
                hNo++;
            }
            enterHistory[currentHistoryNo] = new Coordinate();
            enterHistory[currentHistoryNo].X = CurrentGridX;
            enterHistory[currentHistoryNo].Y = CurrentGridY;
            enterHistory[currentHistoryNo].No = SudokuNumberGrid[CurrentGridX, CurrentGridY].FixNo;
            enterHistory[currentHistoryNo].NoB = 1;
        }

        private void Set_Menu_Mode()
        {

            String[,] ModeArray =
                {
                    {"false", "Puzzle Mode"},
                    {"true", "Create and Analyze Mode"}
                };

            for (int i = 0; i <= ModeArray.GetLength(0) - 1; i++)
            {
                ToolStripMenuItem menuMode = new ToolStripMenuItem();
                menuMode.Text = ModeArray[i, 1];
                menuMode.Tag = ModeArray[i, 0];

                if (Convert.ToBoolean(ModeArray[i, 0]) == AnalyzeMode)
                {
                    menuMode.Checked = true;
                }
                else
                {
                    menuMode.Checked = false;
                }

                menuMode.Click += new EventHandler(Menu_Mode_Click);

                this.Menu_SelectMode.DropDownItems.Add(menuMode);
            }
        }

        private void Menu_Mode_Click(Object sender, EventArgs e)
        {

            ToolStripMenuItem myMenu = (ToolStripMenuItem)sender;

            //            MessageBox.Show(Convert.ToString(myMenu.Tag));  

            if (AnalyzeMode == Convert.ToBoolean(myMenu.Tag))
            {
                return;
            }
            else
            {
                //                MessageBox.Show("change");  
                if (Check_SavePuzzleData() == true)
                {
                    AnalyzeMode = !AnalyzeMode;
                    Change_Mode();
                }
            }
        }

        private void Change_Mode()
        {
            Change_Mode(true);
        }


        private void Change_Mode(bool ResetFlg)
        {

            //            ToolStripMenuItem myMenu;

            foreach (ToolStripMenuItem myMenu in this.Menu_SelectMode.DropDownItems)
            {
                if (AnalyzeMode == Convert.ToBoolean(myMenu.Tag))
                {
                    myMenu.Checked = true;
                }
                else
                {
                    myMenu.Checked = false;
                }
            }

            //        Object myToolbarMenu; //ToolStripMenuItem


            foreach (ToolStripMenuItem myToolbarMenu in this.Menu_Display.DropDownItems.OfType<ToolStripMenuItem>())
            {
                if ((myToolbarMenu.Name).StartsWith("Menu_Toolbar_") == true)
                {
                    myToolbarMenu.Enabled = Convert.ToBoolean(Convert.ToInt32(Convert.ToString(myToolbarMenu.Tag).Substring(1 + Math.Abs(Convert.ToInt32(AnalyzeMode)), 1)));
                    myToolbarMenu.Checked = myToolbarMenu.Enabled;
                    ToolboxInfo[Convert.ToInt32(Convert.ToString(myToolbarMenu.Tag).Substring(0, 1))].Visible = myToolbarMenu.Checked;
                }
            }

            Switch_ToolButtonEnabled();
            ToolboxInfo[Get_DimNo_From_ToolbarName("Highlight")].SelectedNo = 0;

            if (ResetFlg == true)
            {
                if (AnalyzeMode == true)
                {
                    Reset_SudokuGrid();
                }
                else
                {
                    Display_NewQuestion();
                }
            }

            //          MessageBox.Show(Convert.ToString(AnalyzeMode)); 

            GridMsg = "";
            Reset_Hint();

            Set_Grid(this.PictureBoxGrid, this.PictureBoxMemo, this.PictureBoxPalette, this.PictureBoxHighlight);
        }

        private bool Check_SavePuzzleData()
        {

            DialogResult result;

            //            MessageBox.Show(Convert.ToString(ChangeFlg)); 


            if (ChangeFlg == true)
            {
                result = MessageBox.Show("Do you save current puzzle?", "Save Puzzle", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    //                    Menu_File_Save_Click();
                }
                else if (result == DialogResult.Cancel)
                {
                    return false;
                }
            }

            return true;
        }


        private void Set_Menu_LevelItem()
        {

            String[,] LevelArray =
                          {
                           {"1", "Level 1", "40"},
                           {"2", "Level 2", "30"},
                           {"3", "Level 3", "24"},
                           {"4", "Level 4", "25"},
                           {"5", "Level 5", "25"},
                           {"6", "Level 6", "25"},
                           {"7", "Level 7", "25"}
                          };

            for (int i = 0; i <= LevelArray.GetLength(0) - 1; i++)
            {

                ToolStripMenuItem menuLevel = new ToolStripMenuItem();

                menuLevel.Text = LevelArray[i, 1];
                menuLevel.Tag = Convert.ToInt32(LevelArray[i, 0]) + "-" + Convert.ToInt32(LevelArray[i, 2]).ToString("00");
                menuLevel.Checked = false; //ToolbarInfo(CInt(ToolbarArray(i, 2))).Visible

                if (Convert.ToInt32(LevelArray[i, 0]) == CurrentLevel)
                {
                    menuLevel.Checked = true;
                    CurrentAssignCnt = Convert.ToInt32(LevelArray[i, 2]);
                }
                else
                {
                    menuLevel.Checked = false;
                }

                menuLevel.Click += new EventHandler(Menu_LevelChild_Click);

                this.Menu_Level.DropDownItems.Add(menuLevel);
            }
        }

        private void Menu_LevelChild_Click(Object sender, EventArgs e)
        {

            int newLevel;
            DialogResult result;

            foreach (ToolStripMenuItem myMenu in this.Menu_Level.DropDownItems)
            {
                myMenu.Checked = false;
            }

            ToolStripMenuItem currentMenu = (ToolStripMenuItem)sender;
            currentMenu.Checked = true;

            newLevel = Convert.ToInt32((Convert.ToString(currentMenu.Tag)).Substring(0, 1));
            if (CurrentLevel != newLevel)
            {
                CurrentLevel = newLevel; //CInt((sender.tag).ToString.Substring(0, 1))

                //               CommonModule cm = new CommonModule();
                if (commonmdl.Exist_Settings("CurrentLevel") == true)
                {
                    Properties.Settings.Default["CurrentLevel"] = CurrentLevel;
                }
            }

            CurrentAssignCnt = Convert.ToInt32(Convert.ToString(currentMenu.Tag).Substring(2, 2));
            if (ChangeFlg == true)
            {

                result = MessageBox.Show("Quit current puzzle and start new puzzle Level " + Convert.ToSingle(CurrentLevel) + ". Are you ready?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.No)
                {
                    return;
                }
            }

            CurrentGridX = Convert.ToInt32(Math.Floor(Convert.ToDecimal(GridCount / 2))) + GridCount % 2;
            CurrentGridY = Convert.ToInt32(Math.Floor(Convert.ToDecimal(GridCount / 2))) + GridCount % 2;

            GridMsg = "";
            Reset_Hint();
            Set_Grid(this.PictureBoxGrid, this.PictureBoxMemo, this.PictureBoxPalette, this.PictureBoxHighlight);

            Display_NewQuestion();

        }

        private void Display_NewQuestion()
        {
            int cnt = 0;
            int tNo, myLevel;

            Reset_History();

            Coordinate SolveHint = new Coordinate();

            tNo = Get_DimNo_From_ToolbarName("Highlight");
            ToolboxInfo[tNo].SelectedNo = 0;

            CompleteFlg = false;

            myLevel = CurrentLevel;

            if (myLevel >= 3)
            {
                Read_PzlData(myLevel);
            }
            else
            {
                Create_NewQuestion(CurrentAssignCnt, ref cnt);
            }

            Set_Grid(this.PictureBoxGrid, this.PictureBoxMemo, this.PictureBoxPalette, this.PictureBoxHighlight);

            this.LblLevel.Text = "Level " + Convert.ToString(myLevel);
            this.LblLevel.Tag = Convert.ToString(myLevel);

            ChangeFlg = false;
        }

        private int Get_QuestionCount(int myLv)
        {

            String txtAll = (String)Properties.Resources.ResourceManager.GetObject("NumLgLv" + Convert.ToString(myLv));
            Debug.Write(txtAll);

            String[] txtLines = txtAll.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            //Debug.Write(Convert.ToString(txtLines.Length ));


            double d;
            if (double.TryParse(txtLines[0], out d))
            {
                return (int)d;
            }
            else
            {
                return 0;
            }
        }

        private bool Read_PzlData(int myLv)
        {


            int qCount, myNo = 0, myLevel = 0;
            bool myNg = false;
            SortedList myList = new SortedList();
            Coordinate dHint = new Coordinate();



            qCount = Get_QuestionCount(myLv);
            if (qCount > 0)
            {
                myNo = commonmdl.Generate_RandomRange(1, qCount);

                Load_NumLogicStock(myLv, myNo);

            }

            //MessageBox.Show("myNo" + Convert.ToString(myNo)); 


            //チェック用
            int FixCnt;
            SudokuGrid[,] tmpNumberGrid = new SudokuGrid[GridCount + 1, GridCount + 1];

            for (int j = 1; j <= GridCount; j++)
            {
                for (int i = 1; i <= GridCount; i++)
                {
                    tmpNumberGrid[i, j] = new SudokuGrid();
                    tmpNumberGrid[i, j].Copy(SudokuNumberGrid[i, j]);
                }
            }


            Solve_Sudoku(1, ref myNg, ref tmpNumberGrid, ref myLevel, ref dHint);
            Debug.Write(Convert.ToString(myNg) + ": Level" + Convert.ToString(myLevel));

            //MessageBox.Show("Level" + Convert.ToString(myLv));
            //MessageBox.Show(Convert.ToString(myNg) + ": Level" + Convert.ToString(myLevel));


            FixCnt = 0;
            for (int j = 1; j <= GridCount; j++)
            {
                for (int i = 1; i <= GridCount; i++)
                {
                    if (SudokuNumberGrid[i, j].FixNo > 0)
                    {
                        FixCnt = FixCnt + 1;
                    }
                }
            }

            //MessageBox.Show("FixCnt" + Convert.ToString(FixCnt));

            return true;
        }


        private void Save_NumLogicData(String FilePath)
        {

            int i, j, n, c;
            int intFileType = 1;
            String strLine = "", strTitle = "";
            bool workFlag = false, memoFlag = false;

            try {
                System.IO.StreamWriter wfile = new System.IO.StreamWriter(FilePath, false, System.Text.Encoding.GetEncoding(932));
                for (j = 1; j <= GridCount; j++)
                {
                    strLine = "";
                    for (i = 1; i <= GridCount; i++)
                    {
                        if (SudokuNumberGrid[i, j].Locked == true)
                        {
                            strLine += Convert.ToString(SudokuNumberGrid[i, j].FixNo);
                        }
                        else
                        {
                            strLine += "0";
                            if (SudokuNumberGrid[i, j].FixNo > 0)
                            {
                                workFlag = true;
                            }
                            if (SudokuNumberGrid[i, j].MemoNo.Count > 0 || SudokuNumberGrid[i, j].BackColor != Color.White)
                            {
                                memoFlag = true;
                            }
                        }
                    }
                    wfile.WriteLine(strLine);
                }

                //'解答途中データを保存
                if (workFlag == true)
                {
                    wfile.WriteLine("Work");
                    for (j = 1; j <= GridCount; j++)
                    {
                        strLine = "";
                        for (i = 1; i <= GridCount; i++)
                        {
                            strLine += Convert.ToString(SudokuNumberGrid[i, j].FixNo);
                        }
                        wfile.WriteLine(strLine);
                    }
                }

                //'メモデータを保存
                if (memoFlag == true)
                {
                    wfile.WriteLine("Memo");
                    for (j = 1; j <= GridCount; j++)
                    {
                        for (i = 1; i <= GridCount; i++)
                        {
                            strLine = "";
                            for (n = 0; n < SudokuNumberGrid[i, j].MemoNo.Count; n++)
                            {
                                strLine += "," + Convert.ToString(SudokuNumberGrid[i, j].MemoNo[n]);
                            }
                            c = Get_ColorNo(SudokuNumberGrid[i, j].BackColor);
                            if (strLine.Length > 0 || c > 0)
                            {
                                wfile.WriteLine(Convert.ToString(i) + "," + Convert.ToString(j) + "," + Convert.ToString(c) + strLine);
                            }
                        }

                    }
                }

                double d;
                if (double.TryParse(Convert.ToString(this.LblLevel.Tag), out d))
                {
                    wfile.WriteLine("Level:" + Convert.ToString(this.LblLevel.Tag));
                }

                wfile.Close();
                ChangeFlg = false;

            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return;
            }

        }



        private void Load_NumLogicData(String FilePath)
        {
            int i, j, l, x = 0, y = 0, myLevel = 0;
            Encoding enc = Encoding.GetEncoding(932);
            bool readflag = true, workflag = false, memoflag = false, modeGame = false;
            //String[] txtLineArray;
            String strAsterisk = "";
            String[] txtLines = System.IO.File.ReadAllLines(FilePath, enc);
            double d;

            ChangeFlg = false;
            Reset_SudokuGrid();

            for (j = 0; j < txtLines.Length; j++)
            {
                if (txtLines[j].StartsWith("Level:") == true)
                {
                    if (double.TryParse(txtLines[j].Substring(6, 1), out d))
                    {
                        myLevel = Convert.ToInt32(txtLines[j].Substring(6, 1));
                    }
                }

                if (readflag == false)
                {
                    if (txtLines[j] == "Work" || txtLines[j] == "Memo") {
                        readflag = true;
                        workflag = false;
                        memoflag = false;
                        modeGame = true;
                        y = 0;
                        if (txtLines[j] == "Work") {
                            workflag = true;
                        } else if (txtLines[j] == "Memo")
                        {
                            memoflag = true;
                        }
                    }
                } else
                {
                    if (memoflag == false) {
                        y++;
                        //Debug.WriteLine(txtLines[j]);
                        for (i = 0; i < txtLines[j].Length; i++) {
                            x = i + 1;
                            if (x >= 1 && x <= GridCount && y >= 1 && y <= GridCount) {
                                if (double.TryParse(txtLines[j].Substring(i, 1), out d)) {
                                    //Debug.WriteLine("-----------------------" + Convert.ToInt32(x) + "," + Convert.ToInt32(y) + "------------------------------" + Convert.ToInt32(txtLines[j].Substring(i, 1)));
                                    SudokuNumberGrid[x, y].FixNo = Convert.ToInt32(txtLines[j].Substring(i, 1));
                                    if (SudokuNumberGrid[x, y].FixNo > 0) {
                                        if (workflag == false) {
                                            SudokuNumberGrid[x, y].Locked = true;
                                        }
                                    }
                                }
                            }

                        }
                        if (y >= GridCount)
                        {
                            readflag = false;
                        }

                    } else
                    {
                        String[] txtLineArray = txtLines[j].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (txtLineArray.Length >= 2) {
 //                           Debug.WriteLine("----------------------------------------> "+txtLineArray[0] + " :" + txtLineArray[1]);
                            if (double.TryParse(txtLineArray[0], out d) && double.TryParse(txtLineArray[1], out d))
                            {

                                x = Convert.ToInt32(txtLineArray[0]);
                                y = Convert.ToInt32(txtLineArray[1]);
                                if (x >= 1 && x <= GridCount && y >= 1 && y <= GridCount) {
                                    if (txtLineArray.Length >= 3 && double.TryParse(txtLineArray[2], out d)) {
                                        SudokuNumberGrid[x, y].BackColor = Get_Color_From_No(Convert.ToInt32(txtLineArray[2]));
                                    }
                                    for (i = 3; i < txtLineArray.Length; i++)
                                    {
                                        if (double.TryParse(txtLineArray[i], out d))
                                        {
                                            if ((Convert.ToInt32(txtLineArray[i]) >= 1 && (Convert.ToInt32(txtLineArray[i]) <= GridCount))) {
                                                SudokuNumberGrid[x, y].MemoNo.Add(Convert.ToInt32(txtLineArray[i]));
                                            }
                                        }
                                    }

                                }

                            }
                        }

                    }


                }
            }

            this.LblLevel.Text = "";
            this.LblLevel.Tag = "";

            if (modeGame == true) {
                AnalyzeMode = false;
                Change_Mode(false);
                if (myLevel > 0) {
                    this.LblLevel.Text = "Level " + Convert.ToString(myLevel);
                    this.LblLevel.Tag = Convert.ToString(myLevel);
                }
            }

            Adjust_ProspectNo(ref SudokuNumberGrid);
            Reset_History();

            ToolboxInfo[Get_DimNo_From_ToolbarName("Highlight")].SelectedNo = 0;

            this.PictureBoxGrid.Invalidate();
            this.PictureBoxHighlight.Invalidate();
            this.PictureBoxMemo.Invalidate();



        }



        private void Load_NumLogicStock(int myLv)
        {
            Load_NumLogicStock(myLv, 1);
        }

        private void Load_NumLogicStock(int myLv, int LoadNo)
        {

            int l, x = 0, y = 0;
            System.Text.Encoding enc = System.Text.Encoding.GetEncoding(932);
            bool readflag = false;
            String strAsterisk = "";

            Console.Write("LoadNo=" + Convert.ToString(LoadNo));

            ChangeFlg = false;
            Reset_SudokuGrid();

            String txtAll = (String)Properties.Resources.ResourceManager.GetObject("NumLgLv" + Convert.ToString(myLv));

            String[] txtLines = txtAll.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);


            for (int j = 0; j < txtLines.Length; j++)
            {
                if (readflag == false)
                {
                    if (txtLines[j].IndexOf("No." + String.Format("{0:D4}", Convert.ToInt32(LoadNo))) >= 0)
                    {
                        readflag = true;
                        y = 0;
                    }
                }
                else
                {
                    y = y + 1;
                    for (int i = 0; i < txtLines[j].Length; i++)
                    {
                        x = i + 1;
                        if (x > 0 && x <= GridCount && y > 0 && y <= GridCount)
                        {
                            double d;
                            if (double.TryParse(txtLines[j].Substring(i, 1), out d))
                            {
                                SudokuNumberGrid[x, y].FixNo = Convert.ToInt32(d);
                                if (SudokuNumberGrid[x, y].FixNo > 0)
                                {
                                    SudokuNumberGrid[x, y].Locked = true;
                                }
                            }
                        }
                    }
                    if (y >= GridCount)
                    {
                        readflag = false;
                    }
                }
            }

            if (LoadNo > 0)
            {
                //                Replace_GridPosition(SudokuNumberGrid);
            }

            this.LblLevel.Text = "";
            this.LblLevel.Tag = "";

            Adjust_ProspectNo(ref SudokuNumberGrid);

            Reset_History();
            PictureBoxGrid.Invalidate();

        }


        private Color Get_Color_From_No(int myNo) {

            if (PaletteColor.Length - 1 >= myNo)
            {
                return PaletteColor[myNo];
            } else
            {
                return Color.White;
            }

        }


        private int Get_ColorNo(Color myColor) {

            int i;

            for (i = 0; i < PaletteColor.Length; i++) {
                if (PaletteColor[i] == myColor) {
                    return i;
                }
            }

            return 0;
        }


        private void Create_NewQuestion(int assignCnt, ref int FixCnt)
        {
            int myRnd, x, y, x2, y2, pre_x2, pre_y2;
            int i, j, n = 0, s, ss, answerCnt, errCnt;
            int myLevel = 0, blankGrid, dCnt = 50;
            int[] AssignedNo, fixedCnt;
            bool backFlag, ngFlag = false, loopFlgA, loopFlgB, loopFlgC, skipCpart;
            List<int> ProspectNoBalance = new List<int>();
            List<int> myRndProspectNo = new List<int>();
            Coordinate myCoordinate = new Coordinate();
            List<SudokuGrid[,]> answerNumberGrid = new List<SudokuGrid[,]>() { };
            SudokuGrid[,] myTmpNumberGrid;
            Coordinate[] myNextGrid;
            Coordinate dHint = new Coordinate();

            AssignedNo = new int[] { };

            SudokuNumberGrid = new SudokuGrid[GridCount + 1, GridCount + 1];

            loopFlgA = false;
            loopFlgB = false;
            loopFlgC = false;

            //FS:
            do
            {
                loopFlgA = false;
                errCnt = 0;
                backFlag = false;

                n++;
                if (n > 100)
                {
                    break;
                }
                fixedCnt = new int[GridCount + 1];

                for (j = 1; j <= GridCount; j++)
                {
                    for (i = 1; i <= GridCount; i++)
                    {
                        SudokuNumberGrid[i, j] = new SudokuGrid();
                    }
                }
                AssignedNo = new int[] { };
                myTmpNumberGrid = new SudokuGrid[GridCount + 1, GridCount + 1];

                Debug.Write("FS>>>>>>>>>>>>>>>>>>>>>>>>>>");


                //F_Loop:
                do
                {
                    //Debug.Write("FLoop>>>>>>>>>>>>>>>>>>>>>>>>>>");

                    loopFlgB = false;
                    skipCpart = false;
                    if (errCnt > 100 || AssignedNo.Length > assignCnt)
                    {
                        Debug.Write("1");
                        loopFlgA = true;
                        break;
                    }

                    if (backFlag == false)
                    {
                        //                        MessageBox.Show("aaaaa");

                        if (AssignedNo.Length > 0)
                        {
                            //'直近に割り当てたマスと点対称の位置にあるマスの座標
                            pre_x2 = ((GridCount * GridCount - AssignedNo[AssignedNo.Length - 1] + 1) - 1) % GridCount + 1;
                            pre_y2 = Convert.ToInt32(Math.Floor((double)(((GridCount * GridCount - AssignedNo[AssignedNo.Length - 1] + 1) - 1) / GridCount))) + 1;
                        }
                        else
                        {
                            pre_x2 = 0;
                            pre_y2 = 0;

                        }
                        //'バックトラック法を適用
                        if (AssignedNo.Length > assignCnt - 2 && SudokuNumberGrid[pre_x2, pre_y2].FixNo > 0)
                        //                       if (AssignedNo.Length > assignCnt * 2 && SudokuNumberGrid[pre_x2, pre_y2].FixNo > 0)
                        {
                            for (j = 1; j <= GridCount; j++)
                            {
                                for (i = 1; i <= GridCount; i++)
                                {
                                    myTmpNumberGrid[i, j] = new SudokuGrid();
                                    myTmpNumberGrid[i, j].Copy(SudokuNumberGrid[i, j]);
                                }
                            }
                            ss = Solve_SudokuBackTrack(new Coordinate(), myTmpNumberGrid, ref answerNumberGrid, ref dCnt);
                            if (ss > 0)
                            {
                                myNextGrid = new Coordinate[] { };
                                answerCnt = 0;
                                Get_MinimumAnserPair(answerNumberGrid, SudokuNumberGrid, ref myNextGrid, ref answerCnt);
                                if (myNextGrid.Length == 2)
                                {

                                    for (i = 0; i < myNextGrid.Length; i++)
                                    {
                                        SudokuNumberGrid[myNextGrid[i].X, myNextGrid[i].Y].FixNo = myNextGrid[i].No;
                                        SudokuNumberGrid[myNextGrid[i].X, myNextGrid[i].Y].Locked = true;
                                        Array.Resize(ref AssignedNo, AssignedNo.Length + 1);
                                        AssignedNo[AssignedNo.Length - 1] = (myNextGrid[i].Y - 1) * GridCount + myNextGrid[i].X;
                                        fixedCnt[myNextGrid[i].No]++;
                                    }
                                    blankGrid = Solve_Sudoku(3, ref ngFlag, ref SudokuNumberGrid, ref myLevel, ref dHint);
                                    if (blankGrid == 0)
                                    {
                                        Debug.Write("2");
                                        break;
                                        //GoTo F_Fix
                                    }
                                    else
                                    {
                                        Debug.Write("3");
                                        skipCpart = true;
                                        //GoTo F_Loop
                                    }
                                }
                                else
                                {
                                    Debug.Write("4");
                                    loopFlgA = true;
                                    break;
                                    // GoTo FS
                                }
                            }
                            else
                            {
                                Debug.Write("5");
                                loopFlgA = true;
                                break;
                                //GoTo FS
                            }
                        }
                        else if (Assign_NextTaget(ref AssignedNo, SudokuNumberGrid) == false)
                        {
                            Debug.Write("6");
                            //                            MessageBox.Show("6"); 
                            loopFlgA = true;
                            break;
                            //GoTo FS
                        }
                    }

                    if (skipCpart == false)
                    {
                        x = (AssignedNo[AssignedNo.Length - 1] - 1) % GridCount + 1;
                        y = (AssignedNo[AssignedNo.Length - 1] - 1) / GridCount + 1;
                        x2 = GridCount - x + 1;
                        y2 = GridCount - y + 1;

                        backFlag = false;

                        //F_Loop2:
                        do
                        {
                            //Debug.Write("FLoop2>>>>>>>>>>>>>>>>>>>>>>>>>>");
                            loopFlgC = false;
                            myRnd = Select_FitProspectNo(new Coordinate(x, y), SudokuNumberGrid);
                            SudokuNumberGrid[x, y].FixNo = myRnd;

                            Debug.Write("Assigned." + Convert.ToString(AssignedNo.Length) + " x=" + Convert.ToString(x) + "  y=" + Convert.ToString(y) + " myRnd=" + Convert.ToString(myRnd) + "\r\n");


                            if (myRnd > 0)
                            {
                                SudokuNumberGrid[x, y].Locked = true;
                                //'この時点で、問題として成り立つか（＝最後まで解けるか）をチェック
                                blankGrid = Solve_Sudoku(3, ref ngFlag, ref SudokuNumberGrid, ref myLevel, ref dHint);
                                //'Debug.Print("x=" & x & " y=" & y & " No." & myRnd & "   blankGrid：" & blankGrid & " " & ngFlag)
                                if (ngFlag == true)
                                {
                                    //'破綻がある場合は、番号を変更してやり直し
                                    SudokuNumberGrid[x, y].ExcludeNo.Add(myRnd);
                                    SudokuNumberGrid[x, y].Locked = false;
                                    SudokuNumberGrid[x, y].FixNo = 0;
                                    Adjust_ProspectNo(ref SudokuNumberGrid);
                                    loopFlgC = true;
                                    //Debug.Write("7:Assigned." + Convert.ToString(AssignedNo.Length) + " x=" + Convert.ToString(x) + "  y=" + Convert.ToString(y) + " myRnd="+ Convert.ToString(myRnd)+"\r\n");
                                    //GoTo F_Loop2
                                }
                                else
                                {
                                    fixedCnt[SudokuNumberGrid[x, y].FixNo]++;
                                    if (blankGrid > 0)
                                    {
                                        //'破綻はないがまだ未完成の場合は次のマスの割り当てへ
                                        loopFlgB = true;
                                        //                                       Debug.Write("8\r\n");
                                        break;
                                        //GoTo F_Loop;
                                    }
                                    else
                                    {
                                        //'問題として成り立つ場合
                                        //'点対称にするための調整
                                        if (SudokuNumberGrid[x2, y2].FixNo == 0)
                                        {
                                            SudokuNumberGrid[x2, y2].FixNo = SudokuNumberGrid[x2, y2].ProspectNo[0];
                                            SudokuNumberGrid[x2, y2].Locked = true;
                                            fixedCnt[SudokuNumberGrid[x2, y2].FixNo]++;
                                            Array.Resize(ref AssignedNo, AssignedNo.Length + 1);
                                            AssignedNo[AssignedNo.Length - 1] = (y2 - 1) * GridCount + x2;
                                        }
                                    }
                                }


                            }
                            else
                            {
                                //'該当マスに割り当てられる数値がなくなってしまった場合は、１つ前のマスに戻ってやり直し
                                SudokuNumberGrid[x, y].ExcludeNo.Clear();
                                //'1つ前に割り当てた座標
                                x = (AssignedNo[AssignedNo.Length - 2] - 1) % GridCount + 1;
                                y = (AssignedNo[AssignedNo.Length - 2] - 1) / GridCount + 1;
                                fixedCnt[SudokuNumberGrid[x, y].FixNo]--;
                                SudokuNumberGrid[x, y].ExcludeNo.Add(SudokuNumberGrid[x, y].FixNo);
                                SudokuNumberGrid[x, y].FixNo = 0;
                                Adjust_ProspectNo(ref SudokuNumberGrid);
                                Array.Resize(ref AssignedNo, AssignedNo.Length - 1);
                                if (AssignedNo.Length == 0)
                                {
                                    Debug.Write("9");
                                    break;
                                    //                GoTo FS
                                }
                                errCnt++;
                                backFlag = true;
                                loopFlgB = true;
                                //                             Debug.Write("10\r\n");

                                break;
                                //GoTo F_Loop
                            }


                        } while (loopFlgC == true);

                    }

                } while (loopFlgB == true);

            } while (loopFlgA == true);

            //F_Fix:
            FixCnt = 0;
            for (j = 1; j <= GridCount; j++)
            {
                for (i = 1; i <= GridCount; i++)
                {
                    if (SudokuNumberGrid[i, j].FixNo > 0)
                    {
                        FixCnt++;
                    }
                    else
                    {
                        SudokuNumberGrid[i, j].Locked = false;
                    }
                }
            }

            //'割り当てマスが少ない場合は足す（初級者向け）
            while (FixCnt < assignCnt)
            {
                if (Assign_NextTaget(ref AssignedNo, SudokuNumberGrid) == true)
                {
                    x = (AssignedNo[AssignedNo.Length - 1] - 1) % GridCount + 1;
                    y = (AssignedNo[AssignedNo.Length - 1] - 1) / GridCount + 1;
                    SudokuNumberGrid[x, y].FixNo = SudokuNumberGrid[x, y].ProspectNo[0];
                    SudokuNumberGrid[x, y].Locked = true;
                    FixCnt++;
                    //'点対称位置のマスにも配置
                    x2 = GridCount - x + 1;
                    y2 = GridCount - y + 1;
                    if (SudokuNumberGrid[x2, y2].FixNo == 0)
                    {
                        SudokuNumberGrid[x2, y2].FixNo = SudokuNumberGrid[x2, y2].ProspectNo[0];
                        SudokuNumberGrid[x2, y2].Locked = true;
                        FixCnt++;
                        Array.Resize(ref AssignedNo, AssignedNo.Length + 1);
                        AssignedNo[AssignedNo.Length - 1] = (y2 - 1) * GridCount + x2;
                    }
                }

            }
            //        Loop

            Adjust_ProspectNo(ref SudokuNumberGrid);
            this.PictureBoxGrid.Invalidate();

            Debug.Write("FixCnt=" + Convert.ToString(FixCnt));

        }








        //'
        //'  指定のマスに入りうる数値のうち、その番号を入れることによって他のマスの候補Noが最も減るものを選択
        //'
        private int Select_FitProspectNo(Coordinate myCoordinate, SudokuGrid[,] tmpSudokuNumberGrid)
        {

            int cntProspect, i, rnd, sNo = 0, maxCnt = 0;
            List<Coordinate> myProspect = new List<Coordinate>();
            List<int> FitNo = new List<int>();

            ////++++++++++++++++++++++++++++++++++++++++++test++++++++++++++++++++++++++++++++++++++++++++++++++
            //for (i = 0; i < tmpSudokuNumberGrid[myCoordinate.X, myCoordinate.Y].ProspectNo.Count; i++)
            //{
            //    myCoordinate.No = tmpSudokuNumberGrid[myCoordinate.X, myCoordinate.Y].ProspectNo[i];
            //    if (tmpSudokuNumberGrid[myCoordinate.X, myCoordinate.Y].ExcludeNo.IndexOf(myCoordinate.No) >= 0)
            //    {
            //        //skip  
            //    }
            //    else
            //    {
            //        return myCoordinate.No;
            //    }

            //}
            //        //++++++++++++++++++++++++++++++++++++++++++test++++++++++++++++++++++++++++++++++++++++++++++++++


            myCoordinate.S = Get_SquareNo(myCoordinate.X, myCoordinate.Y, ref sNo);

            for (i = 0; i < tmpSudokuNumberGrid[myCoordinate.X, myCoordinate.Y].ProspectNo.Count; i++)
            {
                myCoordinate.No = tmpSudokuNumberGrid[myCoordinate.X, myCoordinate.Y].ProspectNo[i];
                if (tmpSudokuNumberGrid[myCoordinate.X, myCoordinate.Y].ExcludeNo.IndexOf(myCoordinate.No) >= 0)
                {
                    //skip  
                }
                else
                {
                    cntProspect = Count_ProspectNo_On_Group(myCoordinate, ref myProspect, tmpSudokuNumberGrid);

                    if (cntProspect >= maxCnt)
                    {
                        if (cntProspect > maxCnt)
                        {
                            FitNo.Clear();
                            maxCnt = cntProspect;
                        }
                        FitNo.Add(myCoordinate.No);
                    }
                }

            }

            rnd = commonmdl.Generate_Random_FromList(FitNo, tmpSudokuNumberGrid[myCoordinate.X, myCoordinate.Y].ExcludeNo);

            return rnd;
        }


        //'関数の戻り値　全てのマスに埋める数値が決まる場合=0、空きがある場合（作りかけ）=>埋まらないマスの数  
        //'引数等
        //'　SolveMode=1：全解答モード
        //'                現在の入力状態に間違いがない場合、空きマスを解答で埋める
        //'　　　　　　　　間違いがある場合、間違っているマスのエラーフラグ（FixErr）をオンにする　　　
        //'　SolveMode=2：ヒントモード
        //'　　　　　　　　現在の入力状態に間違いがない場合、引数（NextHint）に次に埋めるマス・数字をヒントとして返す
        //'　　　　　　　　間違いがある場合、間違っているマスのエラーフラグ（FixErr）をオンにする　　　
        //'　SolveMode=3：問題作成時モード
        //'　　　　　　　  現在の入力状態に間違いがないか、全てのマスに埋める数値が決まるかのチェックのみ()
        private int Solve_Sudoku(int SolveMode, ref bool NGFlag, ref SudokuGrid[,] myNumberGrid, ref int myLevel, ref Coordinate NextHint)
        {

            int i, j, n, p, s, sNo = 0, errCnt;
            bool boolChange;
            SudokuGrid[,] tmpNumberGrid;
            int returnInt = 0;
            bool bFlg = false;

            //'　NGFlag：数値の確定マスに破綻がある場合：False　ない場合：True
            NGFlag = false;

            //'ローカル変数tmpSudokuNumberGridに参照元のmyNumberGridの情報をコピー
            tmpNumberGrid = new SudokuGrid[GridCount + 1, GridCount + 1];


            for (j = 1; j <= GridCount; j++)
            {
                for (i = 1; i <= GridCount; i++)
                {
                    tmpNumberGrid[i, j] = new SudokuGrid();
                    //'全解答モードでは数値固定マス以外の情報はクリア（間違い回答の可能性があるため）
                    if (myNumberGrid[i, j].Locked == true || SolveMode > 2)
                    {
                        tmpNumberGrid[i, j].Copy(myNumberGrid[i, j]);
                    }

                }
            }

            UsedTechnique = new SolvingTechnique();

            myLevel = 0;

            if (SolveMode == 1)
            {
                Adjust_ProspectNo(ref tmpNumberGrid);
            }
            NextHint = new Coordinate();

            if (DuplicateNumber(tmpNumberGrid, new Coordinate()) == true)
            {
                NGFlag = true;
            }
            p = 0;
            //F_Loop
            do
            {
                p++;
                boolChange = false;
                bFlg = false;
                for (j = 1; j <= GridCount; j++)
                {
                    for (i = 1; i <= GridCount; i++)
                    {
                        //if (SolveMode == 2)
                        //{
                        //    MessageBox.Show("i=" + Convert.ToString(i) + " j:" + Convert.ToString(j));
                        //    Debug.Write("i=" + Convert.ToString(i) + " j:" + Convert.ToString(j) + "\r\n");

                        //}
                        if (tmpNumberGrid[i, j].FixNo == 0)
                        {
                            //'候補Noが１つしかない→数値確定
                            if (tmpNumberGrid[i, j].ProspectNo.Count == 1)
                            {
                                if (myNumberGrid[i, j].FixNo == 0 && NextHint.No == 0)
                                {
                                    NextHint.X = i;
                                    NextHint.Y = j;
                                    NextHint.No = tmpNumberGrid[i, j].ProspectNo[0];
                                }
                                tmpNumberGrid[i, j].FixNo = tmpNumberGrid[i, j].ProspectNo[0];
                                Remove_ProspectNo(1, new Coordinate(i, j, 0, tmpNumberGrid[i, j].FixNo, 0), ref tmpNumberGrid);
                                boolChange = true;
                            }
                        }
                        else
                        {
                            //'既に数値確定済みのマスがある場合、その縦・横・エリアのいずれかが同じマスの該当番号を候補より除外
                            if (Remove_ProspectNo(1, new Coordinate(i, j, 0, tmpNumberGrid[i, j].FixNo, 0), ref tmpNumberGrid) == true)
                            {
                                boolChange = true;
                                if (SolveMode == 2)
                                {
                                    bFlg = true;
                                    //i = -1;
                                    //j = -1;
                                    break;
                                }
                            }
                        }
                    }
                    if (bFlg == true)
                    {
                        break;
                    }
                }

                while (boolChange == false)
                {
                    for (j = 1; j <= GridCount; j++)
                    {
                        for (i = 1; i <= GridCount; i++)
                        {
                            if (tmpNumberGrid[i, j].FixNo == 0)
                            {
                                for (n = 0; n < tmpNumberGrid[i, j].ProspectNo.Count; n++)
                                {
                                    //'該当番号以外入り得ないケース（縦・横・スクエアに該当番号以外の全ての数値が存在）
                                    if (Check_ProspectNo_Solo(new Coordinate(i, j, Get_SquareNo(i, j, ref sNo), tmpNumberGrid[i, j].ProspectNo[n], 0), ref tmpNumberGrid) == true)
                                    {
                                        if (myNumberGrid[i, j].FixNo == 0 && NextHint.No == 0)
                                        {
                                            NextHint.X = i;
                                            NextHint.Y = j;
                                            NextHint.No = tmpNumberGrid[i, j].ProspectNo[n];
                                        }
                                        tmpNumberGrid[i, j].FixNo = tmpNumberGrid[i, j].ProspectNo[n];
                                        Remove_ProspectNo(1, new Coordinate(i, j, 0, tmpNumberGrid[i, j].FixNo, 0), ref tmpNumberGrid);
                                        boolChange = true;
                                        break;
                                    }

                                }
                            }
                        }
                    }
                    if (boolChange == true)
                    {
                        break;
                    }

                    if (SolveMode == 4 || SolveMode == 3)
                    {
                        //Skip Detail Check
                    }
                    else
                    {
                        //MessageBox.Show("Detail Check");

                        for (s = 1; s <= GridCount; s++)
                        {
                            if (Check_ProspectNo_LimitLine(s, ref tmpNumberGrid) == true)
                            {
                                myLevel = Math.Max(myLevel, 1);
                                boolChange = true;
                                break;
                            }
                        }
                        if (boolChange == true)
                        {
                            break;
                        }
                        //' 指定した列（横列）において、番号ｎが入る候補マスが、特定スクエアに限定されるかをチェック
                        for (j = 1; j <= GridCount; j++)
                        {
                            if (Check_ProspectNo_LimitSquare(0, j, ref tmpNumberGrid) == true)
                            {
                                myLevel = Math.Max(myLevel, 1);
                                boolChange = true;
                                break;
                            }
                        }
                        if (boolChange == true)
                        {
                            break;
                        }
                        //' 指定した列（縦列）において、番号ｎが入る候補マスが、特定スクエアに限定されるかをチェック
                        for (i = 1; i <= GridCount; i++)
                        {
                            if (Check_ProspectNo_LimitSquare(i, 0, ref tmpNumberGrid) == true)
                            {
                                myLevel = Math.Max(myLevel, 1);
                                boolChange = true;
                                break;
                            }
                        }
                        if (boolChange == true)
                        {
                            break;
                        }

                        if (SolveMode == 5)
                        {
                            //Skip Detail Check
                        }
                        else
                        {
                            if (Check_ProspectNo_NakedPairTriple(ref tmpNumberGrid) == true)
                            {
                                UsedTechnique.NakedPairTriple = true;
                                myLevel = Math.Max(myLevel, 2);
                                boolChange = true;
                                break;
                            }
                            if (SolveMode == 6)
                            {
                                //Skip Detail Check
                            }
                            else
                            {
                                if (Check_ProspectNo_HiddenPairTriple(ref tmpNumberGrid) == true)
                                {
                                    UsedTechnique.HiddenPairTriple = true;
                                    myLevel = Math.Max(myLevel, 3);
                                    boolChange = true;
                                    break;
                                }
                                if (Check_ProspectNo_SimpleColors(ref tmpNumberGrid) == true)
                                {
                                    UsedTechnique.SimpleColors = true;
                                    myLevel = Math.Max(myLevel, 3);
                                    boolChange = true;
                                    break;
                                }
                                if (Check_ProspectNo_SwordFish(ref tmpNumberGrid, 2) == true)
                                {
                                    this.PictureBoxGrid.Invalidate();
                                    UsedTechnique.XWing = true;
                                    myLevel = Math.Max(myLevel, 3);
                                    boolChange = true;
                                    break;
                                }
                                if (Check_ProspectNo_XYWing(ref tmpNumberGrid) == true)
                                {
                                    UsedTechnique.XYWing = true;
                                    myLevel = Math.Max(myLevel, 3);
                                    boolChange = true;
                                    break;
                                }
                                if (Check_ProspectNo_SwordFish(ref tmpNumberGrid, 3) == true)
                                {
                                    UsedTechnique.SwordFish = true;
                                    myLevel = Math.Max(myLevel, 4);
                                    boolChange = true;
                                    break;
                                }
                                if (Check_ProspectNo_MultiColors(ref tmpNumberGrid) == true)
                                {
                                    UsedTechnique.MultiColors = true;
                                    myLevel = Math.Max(myLevel, 4);
                                    boolChange = true;
                                    break;
                                }
                            }
                        }
                    }
                    break;
                }

            } while (boolChange == true);

            errCnt = 0;
            for (j = 1; j <= GridCount; j++)
            {
                for (i = 1; i <= GridCount; i++)
                {
                    if (tmpNumberGrid[i, j].FixNo == 0)
                    {
                        returnInt++;
                        if (tmpNumberGrid[i, j].ProspectNo.Count == 0)
                        {  //'候補がなくなってしまった状態（破綻）
                            NGFlag = true;
                        }
                    }
                    else
                    {
                        if (SolveMode <= 2)
                        {
                            if (tmpNumberGrid[i, j].FixNo > 0 && myNumberGrid[i, j].FixNo > 0
                                && tmpNumberGrid[i, j].FixNo != myNumberGrid[i, j].FixNo)
                            {
                                myNumberGrid[i, j].FixError = true;
                                returnInt++;
                                NGFlag = true;
                            }
                        }
                    }
                }
            }

            if (SolveMode == 1)
            {
                if (NGFlag == false)
                {
                    for (j = 1; j <= GridCount; j++)
                    {
                        for (i = 1; i <= GridCount; i++)
                        {
                            myNumberGrid[i, j].Copy(tmpNumberGrid[i, j]);
                        }
                    }
                }
            }
            else if (SolveMode == 2)
            {
                //
            }
            else if (SolveMode >= 4)
            {
                for (j = 1; j <= GridCount; j++)
                {
                    for (i = 1; i <= GridCount; i++)
                    {
                        myNumberGrid[i, j].Copy(tmpNumberGrid[i, j]);
                    }
                }
            }
            else if (SolveMode == 3)
            {
                //'問題作成モード時は候補No情報のみを戻す
                if (NGFlag == false)
                {
                    for (j = 1; j <= GridCount; j++)
                    {
                        for (i = 1; i <= GridCount; i++)
                        {
                            myNumberGrid[i, j].ProspectNo.Clear();
                            myNumberGrid[i, j].ProspectNo.AddRange(tmpNumberGrid[i, j].ProspectNo);
                        }
                    }

                }
            }

            return returnInt;

        }



        private int Solve_SudokuBackTrack(Coordinate myGrid, SudokuGrid[,] myNumberGrid, ref List<SudokuGrid[,]> answerNumberGrid, ref int multianswerCnt)
        {



            int i, j, n, myNo;
            SudokuGrid[,] tmpNumberGrid = new SudokuGrid[GridCount + 1, GridCount + 1];
            Coordinate NextTarget = new Coordinate();
            bool myNG = false;
            int returnInt = 0;
            int dLevel = 0;
            Coordinate dHint = new Coordinate();

            //for (j = 1; j <= GridCount; j++)
            //{
            //    for (i = 1; i <= GridCount; i++)
            //    {
            //        Debug.Write("myNumberGrid: x=" + Convert.ToString(i) + " y=" + Convert.ToString(j) + "  FixNo:" + Convert.ToString(myNumberGrid[i, j].FixNo) + "\r\n");

            //    }
            //}


            //'最初に仮定で数値割当するマスを決定
            if (myGrid.X == 0 || myGrid.Y == 0)
            {
                myGrid = Select_NextTaget_For_BackTrack(myNumberGrid);
            }

            //Debug.Write("X=" + Convert.ToString(myGrid.X) + "  Y=" + Convert.ToString(myGrid.Y) + "\r\n");


            if ((multianswerCnt == 0 && answerNumberGrid.Count > 0) || multianswerCnt == -1)
            {
                return returnInt;
            }


            //'対象マスの候補Noがなくなってしまったら終了
            do
            {
                myNo = myNumberGrid[myGrid.X, myGrid.Y].ProspectNo[0];
                //'仮定で数値割当
                myNumberGrid[myGrid.X, myGrid.Y].FixNo = myNo;

                //'一般解法にて解析
                for (j = 1; j <= GridCount; j++)
                {
                    for (i = 1; i <= GridCount; i++)
                    {
                        tmpNumberGrid[i, j] = new SudokuGrid();
                        tmpNumberGrid[i, j].Copy(myNumberGrid[i, j]);
                    }
                }

                if (Solve_Sudoku(4, ref myNG, ref tmpNumberGrid, ref dLevel, ref dHint) == 0)
                {
                    //'解が見つかった
                    returnInt++;
                    answerNumberGrid.Add(tmpNumberGrid);
                    //                    ReDim Preserve answerNumberGrid(GridCount, GridCount, UBound(answerNumberGrid, 3) + 1)
                    //For j = 1 To GridCount
                    //    For i = 1 To GridCount
                    //        answerNumberGrid(i, j, UBound(answerNumberGrid, 3)) = New SudokuGrid
                    //        answerNumberGrid(i, j, UBound(answerNumberGrid, 3)).Copy(tmpNumberGrid(i, j))
                    //    Next
                    //Next
                    if (answerNumberGrid.Count > multianswerCnt)
                    {
                        multianswerCnt = -1;
                    }
                }
                else
                {
                    //'解が見つからない
                    if (myNG == true)
                    {
                        //'破綻してしまっている（＝仮定で割り当てた番号が間違っている） "  y=" & myGrid.X & "  NO=" & myNo & "   " & multianswerCnt)
                    }
                    else
                    {
                        //'破綻はしていない（まだ、数値が埋まらない）→更なる仮定が必要
                        NextTarget = Select_NextTaget_For_BackTrack(tmpNumberGrid);
                        if (NextTarget.X == 0 || NextTarget.Y == 0)
                        {
                            //'仮定割当のマスがなくなってしまったら、同条件での仮定終了
                        }
                        else
                        {
                            //'再帰
                            returnInt += Solve_SudokuBackTrack(NextTarget, tmpNumberGrid, ref answerNumberGrid, ref multianswerCnt);
                        }
                    }
                }

                if (multianswerCnt == -1)
                {
                    break;
                }

                myNumberGrid[myGrid.X, myGrid.Y].FixNo = 0;
                myNumberGrid[myGrid.X, myGrid.Y].ProspectNo.Remove(myNo);

            } while (myNumberGrid[myGrid.X, myGrid.Y].ProspectNo.Count > 0);


            return returnInt;

        }

        //'
        //'  バックトラック法での解答探索時に、次に対象とするマス及び割当Noを選択する
        //'　　　※現時点で、候補No数が最も少ないマスを選択（候補数が同じ場合はY座標、X座標でソートし）
        //'　　　　割当Noは、対象マスの候補Noの中で最小のもの　　　

        private Coordinate Select_NextTaget_For_BackTrack(SudokuGrid[,] myNumberGrid)
        {

            int x, y, p;
            Coordinate returnCoord = new Coordinate();

            p = 1; //'候補No数の初期値



            do
            {
                for (y = 1; y <= GridCount; y++)
                {
                    for (x = 1; x <= GridCount; x++)
                    {
                        if (myNumberGrid[x, y].FixNo == 0)
                        {
                            if (myNumberGrid[x, y].ProspectNo.Count == p)
                            {
                                returnCoord.X = x;
                                returnCoord.Y = y;
                            }
                        }
                    }

                }
                p++;
            } while (p <= GridCount);

            return returnCoord;

        }

        //'
        //'  該当グリッドの候補Noが同一列・スクエアにおいて単独であるかどうかをチェック
        //'
        private bool Check_ProspectNo_Solo(Coordinate myPlace, ref SudokuGrid[,] tmpNumberGrid)
        {

            bool flgX = true, flgY = true, flgS = true;
            int myS, mySNo = 0;

            for (int i = 1; i <= GridCount; i++)
            {
                if (i != myPlace.X)
                {
                    //同列（横）の別マスに同番号が入る可能性あり＝確定出来ない
                    if (tmpNumberGrid[i, myPlace.Y].ProspectNo.IndexOf(myPlace.No) >= 0)
                    {
                        flgX = false;
                        break;
                    }
                }
            }
            if (flgX == true)
            {
                return true;
            }

            for (int j = 1; j <= GridCount; j++)
            {
                if (j != myPlace.Y)
                {
                    //同列（横）の別マスに同番号が入る可能性あり＝確定出来ない
                    if (tmpNumberGrid[myPlace.X, j].ProspectNo.IndexOf(myPlace.No) >= 0)
                    {
                        flgY = false;
                        break;
                    }
                }
            }
            if (flgY == true)
            {
                return true;
            }

            for (int j = 1; j <= GridCount; j++)
            {
                for (int i = 1; i <= GridCount; i++)
                {
                    if (Get_SquareNo(i, j, ref mySNo) == myPlace.S && (i != myPlace.X || myPlace.Y != j))
                    {
                        //'同スクエアの別マスに同番号が入る可能性あり＝確定出来ない
                        if (tmpNumberGrid[i, j].ProspectNo.IndexOf(myPlace.No) >= 0)
                        {
                            flgS = false;
                            break;
                        }
                    }
                }
            }
            if (flgS == true)
            {
                return true;
            }

            return false;
        }

        //'
        //' 指定したスクエアにおいて、番号ｎが入る候補マスが、特定列（縦・横）に限定されるかをチェック
        //'　　　　　　　　　※限定される場合、他のスクエアの同列にはｎは入らないため、候補から除外
        //'
        private bool Check_ProspectNo_LimitLine(int mySquareNo, ref SudokuGrid[,] tmpNumberGrid)
        {

            int p, sNo;
            int x1, x2, y1, y2;
            List<Coordinate> myProspect = new List<Coordinate>();
            Coordinate[] myLimitNo = { new Coordinate() };
            bool xOnLineFlg, yOnLineFlg, removeFlg = false;

            //'指定したスクエアのＸ座標、Ｙ座標の範囲
            x1 = ((mySquareNo - 1) % 3) * 3 + 1;
            x2 = x1 + 2;
            y1 = Convert.ToInt32(Math.Floor((double)((mySquareNo - 1) / 3))) * 3 + 1;
            y2 = y1 + 2;

            for (int n = 1; n <= GridCount; n++)
            {
                myProspect.Clear();
                for (int j = y1; j <= y2; j++)
                {
                    for (int i = x1; i <= x2; i++)
                    {
                        if (tmpNumberGrid[i, j].FixNo == 0)
                        {
                            if (tmpNumberGrid[i, j].ProspectNo.IndexOf(n) >= 0)
                            {
                                myProspect.Add(new Coordinate(i, j));
                            }
                        }
                    }
                }
                if (myProspect.Count > 0)
                {
                    xOnLineFlg = true;
                    yOnLineFlg = true;
                    for (p = 0; p < myProspect.Count; p++)
                    {
                        if (myProspect[p].X != myProspect[0].X)
                        {
                            xOnLineFlg = false; // '縦列限定なし
                        }
                        if (myProspect[p].Y != myProspect[0].Y)
                        {
                            yOnLineFlg = false; //'横列限定なし
                        }
                    }
                    if (xOnLineFlg == true || yOnLineFlg == true)
                    {
                        Array.Resize(ref myLimitNo, myLimitNo.Length + 1);
                        myLimitNo[myLimitNo.Length - 1] =
                              new Coordinate(xOnLineFlg == true ? myProspect[0].X : 0, yOnLineFlg == true ? myProspect[0].Y : 0, mySquareNo, n, 0);
                    }
                }
            }

            for (int i = 1; i < myLimitNo.Length; i++)
            {
                if (Remove_ProspectNo(2, myLimitNo[i], ref tmpNumberGrid) == true)
                {
                    removeFlg = true;
                }
            }

            return removeFlg;
        }

        //'
        //' 指定した列において、番号ｎが入る候補マスが、特定スクエアに限定されるかをチェック
        //'　　　　　　　　　※限定される場合、同一スクエア内の別列マスにはｎは入らないため、候補から除外
        //'
        private bool Check_ProspectNo_LimitSquare(int x, int y, ref SudokuGrid[,] tmpNumberGrid)
        {

            int i, j, n, p;
            int x1, x2, y1, y2, sNo = 0, myS;
            List<Coordinate> myProspect = new List<Coordinate>();
            Coordinate[] myLimitNo = { new Coordinate() };
            bool SameSquareFlg, removeFlg = false;

            //        Check_ProspectNo_LimitSquare = False

            if (x == 0)
            {
                x1 = 1;
                x2 = GridCount;
            }
            else
            {
                //'x固定（縦列チェック）
                x1 = x;
                x2 = x;
            }

            if (y == 0)
            {
                y1 = 1;
                y2 = GridCount;
            }
            else
            {
                //'y固定（横列チェック）
                y1 = y;
                y2 = y;
            }

            for (n = 1; n <= GridCount; n++)
            {
                myProspect.Clear();
                for (j = y1; j <= y2; j++)
                {
                    for (i = x1; i <= x2; i++)
                    {
                        if (tmpNumberGrid[i, j].FixNo == 0)
                        {
                            if (tmpNumberGrid[i, j].ProspectNo.IndexOf(n) >= 0)
                            {
                                myProspect.Add(new Coordinate(i, j, Get_SquareNo(i, j, ref sNo), n, 0));
                            }
                        }
                    }
                }
                if (myProspect.Count > 0)
                {
                    SameSquareFlg = true;
                    myS = myProspect[0].S;
                    for (p = 0; p < myProspect.Count; p++)
                    {
                        if (myProspect[p].S != myS)
                        {
                            SameSquareFlg = false;
                            break;
                        }
                    }
                    if (SameSquareFlg == true)
                    {
                        Array.Resize(ref myLimitNo, myLimitNo.Length + 1);
                        myLimitNo[myLimitNo.Length - 1] = new Coordinate(x, y, myS, n, 0);
                    }
                }
            }

            for (p = 1; p < myLimitNo.Length; p++)
            {
                if (Remove_ProspectNo(3, myLimitNo[p], ref tmpNumberGrid) == true)
                {
                    removeFlg = true;
                }
            }

            return removeFlg;
        }

        //'
        //' 　同列（縦・横）、同一スクエアにおいて候補Noリスト（No数=ｎ）の同じマスがｎ個存在する場合、
        //'　 　　　　　　同列（縦・横）、同一スクエアの別のマスの候補から、そのNoを除外する
        //'
        private bool Check_ProspectNo_NakedPairTriple(ref SudokuGrid[,] tmpNumberGrid)
        {

            int i, j, n, p;
            int x, y, x1, x2, y1, y2;
            int myNo, myS, mySNo = 0, mySNoB = 0;
            List<int> SameGrid = new List<int>();
            List<Coordinate> myProspect = new List<Coordinate>();
            bool returnFlg = false;

            //        Check_ProspectNo_NakedPairTriple = False

            for (y = 1; y <= GridCount; y++)
            {
                for (x = 1; x <= GridCount; x++)
                {
                    if (tmpNumberGrid[x, y].FixNo == 0)
                    {
                        //'同列（横）に候補リストの組み合わせが同じマスがいくつあるかをチェック
                        SameGrid.Clear();
                        for (i = 1; i <= GridCount; i++)
                        {
                            if (i != x)
                            {
                                if (IsSameProspectNoList(new Coordinate(x, y), new Coordinate(i, y), tmpNumberGrid) == true)
                                {
                                    SameGrid.Add(i);
                                }
                            }
                            else
                            {
                                SameGrid.Add(i);
                            }
                        }
                        if (SameGrid.Count == tmpNumberGrid[x, y].ProspectNo.Count)
                        {
                            for (i = 1; i <= GridCount; i++)
                            {
                                if (SameGrid.IndexOf(i) < 0)
                                {  //'候補リストが同じでないマス（＝除外対象）
                                    for (n = 0; n < tmpNumberGrid[x, y].ProspectNo.Count; n++)
                                    {
                                        myNo = tmpNumberGrid[x, y].ProspectNo[n];
                                        if (tmpNumberGrid[i, y].ProspectNo.IndexOf(myNo) >= 0)
                                        {
                                            tmpNumberGrid[i, y].ProspectNo.Remove(myNo);
                                            returnFlg = true;
                                        }
                                    }
                                }
                            }
                        }

                        if (returnFlg == true)
                        {
                            return true;
                        }

                        //'同列（縦）に候補リストの組み合わせが同じマスがいくつあるかをチェック
                        SameGrid.Clear();
                        for (j = 1; j <= GridCount; j++)
                        {
                            if (j != y)
                            {
                                if (IsSameProspectNoList(new Coordinate(x, y), new Coordinate(x, j), tmpNumberGrid) == true)
                                {
                                    SameGrid.Add(j);
                                }
                            }
                            else
                            {
                                SameGrid.Add(j);
                            }
                        }
                        if (SameGrid.Count == tmpNumberGrid[x, y].ProspectNo.Count)
                        {
                            for (j = 1; j <= GridCount; j++)
                            {
                                if (SameGrid.IndexOf(j) < 0)
                                {  //'候補リストが同じでないマス（＝除外対象）
                                    for (n = 0; n < tmpNumberGrid[x, y].ProspectNo.Count; n++)
                                    {
                                        myNo = tmpNumberGrid[x, y].ProspectNo[n];
                                        if (tmpNumberGrid[x, j].ProspectNo.IndexOf(myNo) >= 0)
                                        {
                                            tmpNumberGrid[x, j].ProspectNo.Remove(myNo);
                                            returnFlg = true;
                                        }
                                    }
                                }
                            }
                        }

                        if (returnFlg == true)
                        {
                            return true;
                        }

                        //'同一スクエアに候補リストの組み合わせが同じマスがいくつあるかをチェック
                        SameGrid.Clear();
                        myS = Get_SquareNo(x, y, ref mySNo);
                        for (j = 1; j <= GridCount; j++)
                        {
                            for (i = 1; i <= GridCount; i++)
                            {
                                if (Get_SquareNo(i, j, ref mySNoB) == myS)
                                {
                                    if (i != x || j != y)
                                    {
                                        if (IsSameProspectNoList(new Coordinate(x, y), new Coordinate(i, j), tmpNumberGrid) == true)
                                        {
                                            SameGrid.Add(mySNoB);
                                        }
                                    }
                                    else
                                    {
                                        SameGrid.Add(mySNoB);
                                    }
                                }
                            }
                        }

                        if (SameGrid.Count == tmpNumberGrid[x, y].ProspectNo.Count)
                        {
                            for (j = 1; j <= GridCount; j++)
                            {
                                for (i = 1; i <= GridCount; i++)
                                {
                                    if (Get_SquareNo(i, j, ref mySNoB) == myS)
                                    {
                                        if (SameGrid.IndexOf(mySNoB) < 0)
                                        {  //'候補リストが同じでないマス（＝除外対象）
                                            for (n = 0; n < tmpNumberGrid[x, y].ProspectNo.Count; n++)
                                            {
                                                myNo = tmpNumberGrid[x, y].ProspectNo[n];
                                                if (tmpNumberGrid[i, j].ProspectNo.IndexOf(myNo) >= 0)
                                                {
                                                    tmpNumberGrid[i, j].ProspectNo.Remove(myNo);
                                                    returnFlg = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return returnFlg;
        }

        //
        //' 　同列（縦・横）、同一スクエアにおいて、ｎ種類の数字が入りうるマスがｎ個しか存在しない場合、
        //'　 　　　　　　それらのマスには他の数字は入らないので候補から除外する
        //'
        private bool Check_ProspectNo_HiddenPairTriple(ref SudokuGrid[,] tmpNumberGrid)
        {

            int i, j, k, n, Gr, p, x, y, sNo = 0;
            int curNo, ss;
            String strNo;
            List<Coordinate> myProspect = new List<Coordinate>();
            List<int>[] PickedNoListOrder = { new List<int>() };
            List<int> PickedNoList = new List<int>();
            bool fitFlg = false, removeFlg;

            //        Check_ProspectNo_HiddenPairTriple = False


            F_Top:
            //'行・列・スクエア
            for (Gr = 1; Gr <= 3; Gr++)
            {
                //'チェックするのはPairもしくはTriple
                for (n = 2; n <= 3; n++)
                {
                    for (y = 1; y <= GridCount; y++)
                    {
                        for (x = 1; x <= GridCount; x++)
                        {
                            if (tmpNumberGrid[x, y].FixNo == 0 && tmpNumberGrid[x, y].ProspectNo.Count >= n)
                            {
                                Get_Combinatorics(tmpNumberGrid[x, y].ProspectNo.Count, n, ref PickedNoListOrder);
                                for (i = 0; i < PickedNoListOrder.Length; i++)
                                {
                                    PickedNoList.Clear();
                                    for (j = 0; j < PickedNoListOrder[i].Count; j++)
                                    {
                                        PickedNoList.Add(tmpNumberGrid[x, y].ProspectNo[PickedNoListOrder[i][j]]);
                                    }
                                    if (Gr == 1)
                                    {
                                        fitFlg = Check_ProspectNoList_On_Group(new Coordinate(0, y), PickedNoList, ref myProspect, tmpNumberGrid);
                                    }
                                    else if (Gr == 2)
                                    {
                                        fitFlg = Check_ProspectNoList_On_Group(new Coordinate(x, 0), PickedNoList, ref myProspect, tmpNumberGrid);
                                    }
                                    else if (Gr == 3)
                                    {
                                        fitFlg = Check_ProspectNoList_On_Group(new Coordinate(0, 0, Get_SquareNo(x, y, ref sNo), 0, 0), PickedNoList, ref myProspect, tmpNumberGrid);
                                    }

                                    if (fitFlg == true)
                                    {
                                        strNo = "";
                                        for (ss = 0; ss < PickedNoList.Count; ss++)
                                        {
                                            strNo = strNo + Convert.ToString(PickedNoList[ss]) + "-";
                                        }

                                        for (p = 0; p < myProspect.Count; p++)
                                        {
                                            for (k = tmpNumberGrid[myProspect[p].X, myProspect[p].Y].ProspectNo.Count - 1; k >= 0; k--)
                                            {
                                                curNo = tmpNumberGrid[myProspect[p].X, myProspect[p].Y].ProspectNo[k];
                                                if (PickedNoList.IndexOf(curNo) < 0)
                                                {
                                                    tmpNumberGrid[myProspect[p].X, myProspect[p].Y].ProspectNo.Remove(curNo);
                                                    return true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;

        }

        //'
        //'  同一グループ（横・縦列、スクエア）上で指定したｎ種類の数字が入りうるマスがｎ個であるかをチェック
        //'
        private bool Check_ProspectNoList_On_Group(Coordinate myTarget, List<int> myNumList, ref List<Coordinate> myPlace, SudokuGrid[,] tmpNumberGrid)
        {

            int i, j, n, nn, sNo = 0;
            List<Coordinate>[] myPlaceNum = { };
            String strNum = "";

            //        Check_ProspectNoList_On_Group = False

            myPlace = new List<Coordinate>();
            nn = myNumList.Count;

            Array.Resize(ref myPlaceNum, nn);

            for (n = 0; n < nn; n++)
            {
                strNum = strNum + Convert.ToString(myNumList[n]) + ",";
                myPlaceNum[n] = new List<Coordinate>();
                for (j = 1; j <= GridCount; j++)
                {
                    for (i = 1; i <= GridCount; i++)
                    {
                        if (i == myTarget.X || j == myTarget.Y || Get_SquareNo(i, j, ref sNo) == myTarget.S)
                        {
                            if (tmpNumberGrid[i, j].ProspectNo.IndexOf(myNumList[n]) >= 0)
                            {
                                myPlaceNum[n].Add(new Coordinate(i, j));
                            }
                        }
                    }
                }
                myPlace = Merge_CoordinateList(myPlace, myPlaceNum[n]);
            }

            if (myPlace.Count == nn)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private bool Check_ProspectNo_SimpleColors(ref SudokuGrid[,] tmpNumberGrid)
        {

            int i, j, n, p, linkNo, excludeColor;
            List<Coordinate> crossCoordinate = new List<Coordinate>();
            List<Coordinate>[] myStrongLinkChain = { };
            bool returnFlg = false;

            for (n = 1; n <= GridCount; n++)
            {
                //'強リンクのチェーンを作成
                Create_StrongLinkChain(n, ref myStrongLinkChain, tmpNumberGrid);
                for (linkNo = 0; linkNo < myStrongLinkChain.Length; linkNo++)
                {
                    excludeColor = 0;
                    for (i = 0; i < myStrongLinkChain[linkNo].Count - 1; i++)
                    {
                        for (j = i + 1; j < myStrongLinkChain[linkNo].Count; j++)
                        {
                            if (myStrongLinkChain[linkNo][i].ColorNo != myStrongLinkChain[linkNo][j].ColorNo)
                            {
                                //'チェーンで２色分け（裏表）されたマスの、
                                //'双方各色の影響範囲（横・縦列、スクエア）が重なるマスの座標を取得
                                crossCoordinate = Get_MutualGrid(myStrongLinkChain[linkNo][i], myStrongLinkChain[linkNo][j]);
                                for (p = 0; p < crossCoordinate.Count; p++)
                                {
                                    if (tmpNumberGrid[crossCoordinate[p].X, crossCoordinate[p].Y].FixNo == 0
                                        && tmpNumberGrid[crossCoordinate[p].X, crossCoordinate[p].Y].ProspectNo.IndexOf(n) >= 0)
                                    {
                                        tmpNumberGrid[crossCoordinate[p].X, crossCoordinate[p].Y].ProspectNo.Remove(n);
                                        returnFlg = true;
                                    }
                                }
                            }
                            else
                            {
                                //'チェーンで２色分け（裏表）されたマスの、どちらかの色が同じグループ上に複数存在
                                //'してしまっている場合、その色のマスは候補とはなり得ない→もう片方の色のマス数値確定　
                                if (IsSameGroup(myStrongLinkChain[linkNo][i], myStrongLinkChain[linkNo][j]) == true)
                                {
                                    excludeColor = myStrongLinkChain[linkNo][i].ColorNo;
                                }
                            }
                        }
                    }
                    if (excludeColor > 0)
                    {
                        for (i = 0; i < myStrongLinkChain[linkNo].Count; i++)
                        {
                            if (myStrongLinkChain[linkNo][i].ColorNo == excludeColor)
                            {
                                if (tmpNumberGrid[myStrongLinkChain[linkNo][i].X, myStrongLinkChain[linkNo][i].Y].FixNo == 0
                                    && tmpNumberGrid[myStrongLinkChain[linkNo][i].X, myStrongLinkChain[linkNo][i].Y].ProspectNo.IndexOf(n) >= 0)
                                {
                                    tmpNumberGrid[myStrongLinkChain[linkNo][i].X, myStrongLinkChain[linkNo][i].Y].ProspectNo.Remove(n);
                                    returnFlg = true;
                                }
                            }
                        }
                    }
                }
            }

            return returnFlg;
        }

        private bool Check_ProspectNo_MultiColors(ref SudokuGrid[,] tmpNumberGrid)
        {

            int i, j, ca, cb, n, p;
            int linkANo, linkBNo;
            bool[,] sameGrpFlg = new bool[2, 2];
            List<Coordinate> crossCoordinate = new List<Coordinate>();
            List<Coordinate>[] myStrongLinkChain = { };
            bool returnFlg = false;


            for (n = 1; n <= GridCount; n++)
            {
                //'強リンクのチェーンを作成
                Create_StrongLinkChain(n, ref myStrongLinkChain, tmpNumberGrid);
                //'MultiColorsが適用されるのは同ナンバーのリンクチェーンが複数存在する場合のみ
                if (myStrongLinkChain.Length >= 2)
                {
                    for (linkANo = 0; linkANo < myStrongLinkChain.Length; linkANo++)
                    {
                        for (linkBNo = linkANo + 1; linkBNo < myStrongLinkChain.Length; linkBNo++)
                        {
                            //'フラグを初期化
                            for (ca = 0; ca <= 1; ca++)
                            {
                                for (cb = 0; cb <= 1; cb++)
                                {
                                    sameGrpFlg[ca, cb] = false;
                                }
                            }
                            for (i = 0; i < myStrongLinkChain[linkANo].Count; i++)
                            {
                                for (j = 0; j < myStrongLinkChain[linkBNo].Count; j++)
                                {
                                    //'チェーンＡのマスとチェーンＢのマスが同一グループ上に存在
                                    if (IsSameGroup(myStrongLinkChain[linkANo][i], myStrongLinkChain[linkBNo][j]) == true)
                                    {
                                        //'フラグオン
                                        sameGrpFlg[myStrongLinkChain[linkANo][i].ColorNo, myStrongLinkChain[linkBNo][j].ColorNo] = true;
                                    }
                                }
                            }
                            //'チェーンＡの色a1のマスが、チェーンＢの色b1、b2のそれぞれと同じグループに存在する
                            //'　→色b1、b2のマスが両方とも採用されることはない→a1は候補から除外→a2確定　　　
                            for (ca = 0; ca <= 1; ca++)
                            {
                                if (sameGrpFlg[ca, 0] == true && sameGrpFlg[ca, 1] == true)
                                {
                                    for (i = 0; i < myStrongLinkChain[linkANo].Count; i++)
                                    {
                                        if (myStrongLinkChain[linkANo][i].ColorNo == ca)
                                        {
                                            if (tmpNumberGrid[myStrongLinkChain[linkANo][i].X, myStrongLinkChain[linkANo][i].Y].FixNo == 0
                                                && tmpNumberGrid[myStrongLinkChain[linkANo][i].X, myStrongLinkChain[linkANo][i].Y].ProspectNo.IndexOf(n) >= 0)
                                            {
                                                tmpNumberGrid[myStrongLinkChain[linkANo][i].X, myStrongLinkChain[linkANo][i].Y].ProspectNo.Remove(n);
                                                returnFlg = true;
                                            }
                                        }
                                    }
                                }
                            }

                            if (returnFlg == true)
                            {
                                return true;
                            }

                            for (cb = 0; cb <= 1; ca++)
                            {
                                if (sameGrpFlg[0, cb] == true && sameGrpFlg[1, cb] == true)
                                {
                                    for (j = 0; j < myStrongLinkChain[linkBNo].Count; j++)
                                    {
                                        if (myStrongLinkChain[linkBNo][j].ColorNo == cb)
                                        {
                                            if (tmpNumberGrid[myStrongLinkChain[linkBNo][j].X, myStrongLinkChain[linkBNo][j].Y].FixNo == 0
                                                && tmpNumberGrid[myStrongLinkChain[linkBNo][j].X, myStrongLinkChain[linkBNo][j].Y].ProspectNo.IndexOf(n) >= 0)
                                            {
                                                tmpNumberGrid[myStrongLinkChain[linkBNo][j].X, myStrongLinkChain[linkBNo][j].Y].ProspectNo.Remove(n);
                                                returnFlg = true;
                                            }
                                        }
                                    }
                                }
                            }

                            if (returnFlg == true)
                            {
                                return true;
                            }

                            //'チェーンＡの色a1のマスとチェーンＢの色b1のマスが同じグループに存在する
                            //'　→色a1と色b1のマスの少なくともどちらかは候補から除外される
                            //'　→色a2と色b2の少なくともどちらかは数値確定　　　
                            //'　→色a2と色b2の影響範囲（横・縦列、スクエア）が重なるマスは候補から除外　　　
                            for (ca = 0; ca <= 2; ca++)
                            {
                                for (cb = 0; cb <= 1; ca++)
                                {
                                    if (sameGrpFlg[ca, cb] == true)
                                    {
                                        for (i = 0; i < myStrongLinkChain[linkANo].Count; i++)
                                        {
                                            for (j = 0; j < myStrongLinkChain[linkBNo].Count; j++)
                                            {
                                                if (myStrongLinkChain[linkANo][i].ColorNo != ca && myStrongLinkChain[linkBNo][j].ColorNo != cb)
                                                {
                                                    crossCoordinate = Get_MutualGrid(myStrongLinkChain[linkANo][i], myStrongLinkChain[linkBNo][j]);
                                                    for (p = 0; p < crossCoordinate.Count; p++)
                                                    {
                                                        if (tmpNumberGrid[crossCoordinate[p].X, crossCoordinate[p].Y].FixNo == 0
                                                            && tmpNumberGrid[crossCoordinate[p].X, crossCoordinate[p].Y].ProspectNo.IndexOf(n) >= 0)
                                                        {
                                                            tmpNumberGrid[crossCoordinate[p].X, crossCoordinate[p].Y].ProspectNo.Remove(n);
                                                            returnFlg = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (returnFlg == true)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return returnFlg;

        }


        private bool Check_ProspectNo_XYWing(ref SudokuGrid[,] tmpNumberGrid)
        {

            //'xyを候補とするマスと同じグループ内（横・縦列、スクエア）に
            //'xzを候補とするマス１とyzを候補とするマス２がある場合（マス１とマス２は同じグループでなくて良い）
            //'マス１、マス２それぞれの影響範囲（横・縦列、スクエア）が重なるマスにはzは入らない

            int i, j, ii, jj, n, p = 0, pp, p1, p2;
            int x, y, z, xIndex, yIndex, zIndex, myNo;
            List<Coordinate> crossCoordinate = new List<Coordinate>();
            Coordinate[] CoordinateXZ, CoordinateYZ;
            bool returnFlg = false;

            //            Check_ProspectNo_XYWing = False

            for (j = 1; j <= GridCount; j++)
            {
                for (i = 1; i <= GridCount; i++)
                {
                    if (tmpNumberGrid[i, j].ProspectNo.Count == 2)
                    {
                        CoordinateXZ = new Coordinate[] { };
                        CoordinateYZ = new Coordinate[] { };
                        //'x,yをセット
                        x = tmpNumberGrid[i, j].ProspectNo[0];
                        y = tmpNumberGrid[i, j].ProspectNo[1];
                        for (jj = 1; jj <= GridCount; jj++)
                        {
                            for (ii = 1; ii <= GridCount; ii++)
                            {
                                if ((i != ii || j != jj) && tmpNumberGrid[ii, jj].ProspectNo.Count == 2
                                    && IsSameGroup(new Coordinate(i, j), new Coordinate(ii, jj)) == true)
                                {
                                    xIndex = tmpNumberGrid[ii, jj].ProspectNo.IndexOf(x);
                                    if (xIndex >= 0)
                                    {
                                        zIndex = xIndex * -1 + 1;
                                        Array.Resize(ref CoordinateXZ, CoordinateXZ.Length + 1);
                                        CoordinateXZ[CoordinateXZ.Length - 1] = new Coordinate(ii, jj, 0, tmpNumberGrid[ii, jj].ProspectNo[zIndex], 0);
                                    }
                                    yIndex = tmpNumberGrid[ii, jj].ProspectNo.IndexOf(y);
                                    if (yIndex >= 0)
                                    {
                                        zIndex = yIndex * -1 + 1;
                                        Array.Resize(ref CoordinateYZ, CoordinateYZ.Length + 1);
                                        CoordinateYZ[CoordinateYZ.Length - 1] = new Coordinate(ii, jj, 0, tmpNumberGrid[ii, jj].ProspectNo[zIndex], 0);
                                    }
                                }
                            }
                        }
                        for (p1 = 0; p1 < CoordinateXZ.Length; p1++)
                        {
                            for (p2 = 0; p2 < CoordinateYZ.Length; p2++)
                            {
                                if (CoordinateXZ[p1].No == CoordinateYZ[p2].No)
                                {
                                    z = CoordinateXZ[p1].No;
                                    //'MsgBox(CoordinateXZ(p1).X & "," & CoordinateXZ(p1).Y & "  " & CoordinateYZ(p2).X & "," & CoordinateYZ(p2).Y)
                                    //'影響範囲（横・縦列、スクエア）が重なるマスの座標を取得
                                    crossCoordinate = Get_MutualGrid(CoordinateXZ[p1], CoordinateYZ[p2]);
                                    for (p = 0; p < crossCoordinate.Count; p++)
                                    {
                                        if (tmpNumberGrid[crossCoordinate[p].X, crossCoordinate[p].Y].ProspectNo.IndexOf(z) >= 0)
                                        {
                                            tmpNumberGrid[crossCoordinate[p].X, crossCoordinate[p].Y].ProspectNo.Remove(z);
                                            returnFlg = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return returnFlg;

        }

        private bool Check_ProspectNo_SwordFish(ref SudokuGrid[,] tmpNumberGrid, int LineCntLev)
        { // As Integer = 3) As Boolean

            int i, j, n, p, c, x, y, LineCnt;
            List<int> SameGrid = new List<int>();
            List<int>[] myPlaceX;
            List<int>[] myPlaceY;
            int[] myPlaceX_Y;
            int[] myPlaceY_X;
            List<int> myPlaceUnion = new List<int>();
            List<Coordinate> myProspect = new List<Coordinate>();
            List<Coordinate> myProspectB = new List<Coordinate>();
            List<int> targetLine = new List<int>();
            List<int>[] PickedNoList;
            bool chkFlg, returnFlg = false;

            //            Check_ProspectNo_SwordFish = False
            for (n = 1; n <= GridCount; n++)
            {
                c = 0;
                myPlaceX = new List<int>[] { };
                myPlaceX_Y = new int[] { };
                for (y = 1; y <= GridCount; y++)
                {   //'横列チェック
                    targetLine.Clear();
                    myProspect = new List<Coordinate>();
                    LineCnt = Count_ProspectNo_On_Group(new Coordinate(0, y, 0, n, 0), ref myProspect, tmpNumberGrid);
                    if (LineCnt >= 2 && LineCnt <= LineCntLev)
                    {
                        Array.Resize(ref myPlaceX, c + 1);
                        myPlaceX[c] = new List<int>();
                        for (i = 0; i < myProspect.Count; i++)
                        {
                            myPlaceX[c].Add(myProspect[i].X);
                        }
                        Array.Resize(ref myPlaceX_Y, c + 1);
                        myPlaceX_Y[c] = y;
                        c = c + 1;
                    }
                }

                if (myPlaceX.Length >= 2)
                {
                    //'対象Noが候補となっているマス数が指定（LineLev）以下である列（列数=p）のうち、
                    //'任意のLineLev列の候補となっているＸ座標数＝LineLevに限定される時、他列の同Ｘ座標マス
                    //'の候補から対象Noを除外出来る
                    for (LineCnt = 2; LineCnt <= LineCntLev; LineCnt++)
                    {
                        PickedNoList = new List<int>[] { };
                        Get_Combinatorics(myPlaceX.Length, LineCnt, ref PickedNoList);
                        for (p = 0; p < PickedNoList.Length; p++)
                        {
                            myPlaceUnion = Get_ProspectNo_Union(myPlaceX, PickedNoList[p]); //  '候補Noの和集合（Ｘ座標）を取得
                            if (myPlaceUnion.Count == LineCnt)
                            {
                                for (y = 1; y <= GridCount; y++)
                                {
                                    chkFlg = false;
                                    for (c = 0; c < PickedNoList[p].Count; c++)
                                    {
                                        if (myPlaceX_Y[PickedNoList[p][c]] == y)
                                        { // '対象行（除外対象外）
                                            chkFlg = true;
                                            break;
                                        }
                                    }
                                    if (chkFlg == false)
                                    {
                                        for (i = 0; i < myPlaceUnion.Count; i++)
                                        {
                                            if (tmpNumberGrid[myPlaceUnion[i], y].FixNo == 0
                                                && tmpNumberGrid[myPlaceUnion[i], y].ProspectNo.IndexOf(n) >= 0)
                                            {
                                                tmpNumberGrid[myPlaceUnion[i], y].ProspectNo.Remove(n);
                                                returnFlg = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (returnFlg == true)
                        {
                            return true;
                        }
                    }
                }

                c = 0;
                myPlaceY = new List<int>[] { };
                myPlaceY_X = new int[] { };
                for (x = 1; x <= GridCount; x++)
                {   //'縦列チェック
                    targetLine.Clear();
                    myProspect = new List<Coordinate>();
                    LineCnt = Count_ProspectNo_On_Group(new Coordinate(x, 0, 0, n, 0), ref myProspect, tmpNumberGrid);
                    if (LineCnt >= 2 && LineCnt <= LineCntLev)
                    {
                        Array.Resize(ref myPlaceY, c + 1);
                        myPlaceY[c] = new List<int>();
                        for (i = 0; i < myProspect.Count; i++)
                        {
                            myPlaceY[c].Add(myProspect[i].Y);
                        }
                        Array.Resize(ref myPlaceY_X, c + 1);
                        myPlaceY_X[c] = x;
                        c = c + 1;
                    }
                }

                if (myPlaceY.Length >= 2)
                {
                    //'対象Noが候補となっているマス数が指定（LineLev）以下である列（列数=p）のうち、
                    //'任意のLineLev列の候補となっているＹ座標数＝LineLevに限定される時、他列の同Ｙ座標マス
                    //'の候補から対象Noを除外出来る
                    for (LineCnt = 2; LineCnt <= LineCntLev; LineCnt++)
                    {
                        PickedNoList = new List<int>[] { };
                        Get_Combinatorics(myPlaceY.Length, LineCnt, ref PickedNoList);

                        for (p = 0; p < PickedNoList.Length; p++)
                        {
                            myPlaceUnion = Get_ProspectNo_Union(myPlaceY, PickedNoList[p]); //  '候補Noの和集合（Y座標）を取得
                            if (myPlaceUnion.Count == LineCnt)
                            {
                                for (x = 1; x <= GridCount; x++)
                                {
                                    chkFlg = false;
                                    for (c = 0; c < PickedNoList[p].Count; c++)
                                    {
                                        if (myPlaceY_X[PickedNoList[p][c]] == x)
                                        { // '対象行（除外対象外）
                                            chkFlg = true;
                                            break;
                                        }
                                    }
                                    if (chkFlg == false)
                                    {
                                        for (i = 0; i < myPlaceUnion.Count; i++)
                                        {
                                            if (tmpNumberGrid[x, myPlaceUnion[i]].FixNo == 0
                                                && tmpNumberGrid[x, myPlaceUnion[i]].ProspectNo.IndexOf(n) >= 0)
                                            {
                                                tmpNumberGrid[x, myPlaceUnion[i]].ProspectNo.Remove(n);
                                                returnFlg = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (returnFlg == true)
                        {
                            return true;
                        }
                    }
                }
            }

            return returnFlg;

        }


        private bool Remove_ProspectNo(int myRemoveMode, Coordinate myCoordinate, ref SudokuGrid[,] tmpNumberGrid)
        {

            int i, j, myS, mySNo = 0;
            List<Coordinate> myRemoveGrid = new List<Coordinate>();
            bool returnFlg = false;

            myS = myCoordinate.S;


            //     myS = Get_SquareNo(myCoordinate.X, myCoordinate.Y, ref mySNo);


            myRemoveGrid.Clear();

            //'対象Noを候補から外すマスをリスト化

            //'1.確定マスの同列・同スクエアのマス
            if (myRemoveMode == 1)
            {
                if (myCoordinate.X > 0 && myCoordinate.Y > 0)
                {
                    myS = Get_SquareNo(myCoordinate.X, myCoordinate.Y, ref mySNo);
                    for (j = 1; j <= GridCount; j++)
                    {
                        for (i = 1; i <= GridCount; i++)
                        {
                            if (i == myCoordinate.X && j == myCoordinate.Y)
                            {
                                //'確定マスの候補Noは当然確定数値のみ
                                tmpNumberGrid[myCoordinate.X, myCoordinate.Y].ProspectNo.Clear();
                                tmpNumberGrid[myCoordinate.X, myCoordinate.Y].ProspectNo.Add(myCoordinate.No);
                            }
                            else if (i == myCoordinate.X || j == myCoordinate.Y || Get_SquareNo(i, j, ref mySNo) == myS)
                            {
                                myRemoveGrid.Add(new Coordinate(i, j));
                            }
                        }
                    }
                }
            }
            else if (myRemoveMode == 2)
            {        //'2.指定した列の候補確定スクエア以外のマス
                for (j = 1; j <= GridCount; j++)
                {
                    for (i = 1; i <= GridCount; i++)
                    {
                        if ((i == myCoordinate.X || j == myCoordinate.Y) && Get_SquareNo(i, j, ref mySNo) != myS)
                        {
                            myRemoveGrid.Add(new Coordinate(i, j));
                        }
                    }
                }
            }
            else if (myRemoveMode == 3)
            {        //'3.指定したスクエアの候補確定列以外のマス
                for (j = 1; j <= GridCount; j++)
                {
                    for (i = 1; i <= GridCount; i++)
                    {
                        if (Get_SquareNo(i, j, ref mySNo) == myS && i != myCoordinate.X && j != myCoordinate.Y)
                        {
                            myRemoveGrid.Add(new Coordinate(i, j));
                        }
                    }
                }
            }

            for (i = 0; i < myRemoveGrid.Count; i++)
            {
                if (tmpNumberGrid[myRemoveGrid[i].X, myRemoveGrid[i].Y].FixNo == 0)
                {
                    if (tmpNumberGrid[myRemoveGrid[i].X, myRemoveGrid[i].Y].ProspectNo.IndexOf(myCoordinate.No) >= 0)
                    {
                        tmpNumberGrid[myRemoveGrid[i].X, myRemoveGrid[i].Y].ProspectNo.Remove(myCoordinate.No);
                        returnFlg = true;
                    }
                }
            }

            return returnFlg;

        }


        //'
        //'　対象マス【座標:(TargetX,TargetY)】の候補リストが、参照元【座標:(SourceX,SourceY)】と同じか否かをチェック  
        //'
        private bool IsSameProspectNoList(Coordinate mySource, Coordinate myTarget, SudokuGrid[,] tmpNumberGrid)
        {

            int myNo;

            //        IsSameProspectNoList = False

            if (tmpNumberGrid[myTarget.X, myTarget.Y].FixNo > 0 || tmpNumberGrid[myTarget.X, myTarget.Y].ProspectNo.Count == 0)
            {
                return false;
            }

            //'参照元と対象マスの候補Noリストが完全に一致しなくても、
            //'参照元(1,3,8)、対象マス(3,8)などのように対象マスの候補Noが参照元に全て含まれていればＯＫ
            for (int i = 0; i < tmpNumberGrid[myTarget.X, myTarget.Y].ProspectNo.Count; i++)
            {
                myNo = tmpNumberGrid[myTarget.X, myTarget.Y].ProspectNo[i];
                if (tmpNumberGrid[mySource.X, mySource.Y].ProspectNo.IndexOf(myNo) < 0)
                {
                    return false;
                }
            }

            return true;

        }


        private List<Coordinate> Merge_CoordinateList(List<Coordinate> listA, List<Coordinate> listB)
        {

            int i, j;
            bool sameFlg;
            List<Coordinate> listAB = new List<Coordinate>();

            for (i = 0; i < listA.Count; i++)
            {
                listAB.Add(listA[i]);
            }

            for (i = 0; i < listB.Count; i++)
            {
                sameFlg = false;
                for (j = 0; j < listAB.Count; j++)
                {
                    if (listB[i].X == listAB[j].X && listB[i].Y == listAB[j].Y)
                    {
                        sameFlg = true;
                        break;
                    }
                }
                if (sameFlg == false)
                {
                    listAB.Add(listB[i]);
                }
            }

            return listAB;

        }


        //'
        //'  n個の中からm個を選ぶ組み合わせを取得し、配列（myCombinatorics）に収納
        //'
        private void Get_Combinatorics(int n, int m, ref List<int>[] myCombinatorics)
        {

            int i, j, nCm, c;
            int[] v = { };
            bool endFlg;

            Array.Resize(ref v, m + 1);

            //'nCm=全組み合わせ数
            nCm = commonmdl.Combinatorics(n, m);
            Array.Resize(ref myCombinatorics, nCm);

            for (i = 0; i < nCm; i++)
            {
                if (i == 0)
                {
                    //配列の先頭に入れる組み合わせ（最小のものからm個）
                    for (c = 0; c < m; c++)
                    {
                        v[c] = c;
                    }
                }
                else
                {
                    c = m - 1;
                    endFlg = false;
                    while (!endFlg)
                    {
                        if (v[c] < n - (m - c))
                        {
                            v[c] = v[c] + 1;
                            for (j = c + 1; j <= m; j++)
                            {
                                v[j] = v[j - 1] + 1;
                            }
                            endFlg = true;
                        }
                        else
                        {
                            c = c - 1;
                        }
                    }
                }

                myCombinatorics[i] = new List<int>();
                for (c = 0; c < m; c++)
                {
                    myCombinatorics[i].Add(v[c]);
                }
            }
        }

        //'
        //'　指定した複数列の候補Noが位置する座標の和集合を取得
        //'
        private List<int> Get_ProspectNo_Union(List<int>[] myPlace, List<int> myPickedNoList)
        {

            int i, j;
            List<int> myLineNo = new List<int>();
            List<int> returnList = new List<int>();

            for (i = 0; i < myPickedNoList.Count; i++)
            {
                for (j = 0; j < myPlace[myPickedNoList[i]].Count; j++)
                {
                    if (myLineNo.IndexOf(myPlace[myPickedNoList[i]][j]) < 0)
                    {
                        myLineNo.Add(myPlace[myPickedNoList[i]][j]);
                    }
                }
            }

            returnList.AddRange(myLineNo);
            return returnList;
        }

        private void BK_Get_Combinatorics(int n, int m, ref List<int>[] myCombinatorics)
        {

            int i, j, nCm, c;
            int[] v = { };
            bool endFlg;

            Array.Resize(ref v, m + 1);

            //'nCm=全組み合わせ数
            nCm = commonmdl.Combinatorics(n, m);
            Array.Resize(ref myCombinatorics, nCm);

            for (i = 0; i < nCm; i++)
            {
                if (i == 0)
                {
                    //配列の先頭に入れる組み合わせ（最小のものからm個）
                    for (c = 1; c <= m; c++)
                    {
                        v[c] = c;
                    }
                }
                else
                {
                    c = m;
                    endFlg = false;
                    while (!endFlg)
                    {
                        if (v[c] < n - (m - c))
                        {
                            v[c] = v[c] + 1;
                            for (j = c + 1; j <= m; j++)
                            {
                                v[j] = v[j - 1] + 1;
                            }
                            endFlg = true;
                        }
                        else
                        {
                            c = c - 1;
                        }
                    }
                }

                myCombinatorics[i] = new List<int>();
                for (c = 1; c <= m; c++)
                {
                    myCombinatorics[i].Add(v[c]);
                }
            }
        }

        //'
        //'  指定したナンバー（myNo）の強リンク連鎖情報を取得しコレクション配列（myStrongLink）に追加
        //'
        private void Create_StrongLinkChain(int myNo, ref List<Coordinate>[] myStrongLink, SudokuGrid[,] tmpNumberGrid)
        {

            int i, j, p = 0;

            myStrongLink = new List<Coordinate>[] { };

            //            myStrongLink(0) = New List(Of Coordinate)
            for (j = 1; j <= GridCount; j++)
            {
                for (i = 1; i <= GridCount; i++)
                {
                    p = p + addStrongLink(new Coordinate(i, j, 0, myNo, 0), ref myStrongLink, tmpNumberGrid);
                }
            }
        }


        private int addStrongLink(Coordinate myTarget, ref List<Coordinate>[] myStrongLink, SudokuGrid[,] tmpNumberGrid)
        {

            int i, j, p = 0, sNo = 0, cntProspect = 0, LinkNo;
            List<Coordinate> myCoordinate = new List<Coordinate>();
            List<int> targetLine = new List<int>();
            int cnt = 0, cNo;

            if (tmpNumberGrid[myTarget.X, myTarget.Y].ProspectNo.IndexOf(myTarget.No) >= 0)
            {
                for (i = 1; i <= 3; i++)
                {
                    if (i == 1)
                    {        //'該当マスの属する列（横列）中の該当Noを候補に持つマス数
                        cntProspect = Count_ProspectNo_On_Group(new Coordinate(0, myTarget.Y, 0, myTarget.No, 0), ref myCoordinate, tmpNumberGrid);
                    }
                    else if (i == 2)
                    { //'該当マスの属する列（縦列）中の該当Noを候補に持つマス数
                        cntProspect = Count_ProspectNo_On_Group(new Coordinate(myTarget.X, 0, 0, myTarget.No, 0), ref myCoordinate, tmpNumberGrid);
                    }
                    else if (i == 3)
                    { //'該当マスの属するグループ中の該当Noを候補に持つマス数
                        cntProspect = Count_ProspectNo_On_Group(new Coordinate(0, 0, Get_SquareNo(myTarget.X, myTarget.Y, ref sNo), myTarget.No, 0), ref myCoordinate, tmpNumberGrid);
                    }
                    if (cntProspect == 2)
                    { //'強リンク＝対象Noを候補に持つマスがグループ内で２つ
                        if (Exist_Coordinate(myStrongLink, myCoordinate[0]) == true && Exist_Coordinate(myStrongLink, myCoordinate[1]) == true)
                        {
                            //'両方とも登録済みの時は何もしない
                        }
                        else
                        {
                            if (Exist_Coordinate(myStrongLink, myCoordinate[0]) == false)
                            {
                                p = 0;
                            }
                            else if (Exist_Coordinate(myStrongLink, myCoordinate[1]) == false)
                            {
                                p = 1;
                            }
                            cNo = myCoordinate[p].ColorNo;
                            LinkNo = Get_LinkNo(myStrongLink, myCoordinate[(-p + 1)], ref cNo);
                            //                            LinkNo = Get_LinkNo(myStrongLink, myCoordinate[(-p + 1)], ref myCoordinate[p]);
                            myCoordinate[p].ColorNo = cNo;
                            if (myStrongLink.Length == LinkNo)
                            {
                                Array.Resize(ref myStrongLink, LinkNo + 1);
                                myStrongLink[LinkNo] = new List<Coordinate>();
                            }
                            myStrongLink[LinkNo].Add(myCoordinate[p]);
                            //'次の接続先を探査
                            cnt = cnt + 1 + addStrongLink(myCoordinate[p], ref myStrongLink, tmpNumberGrid);
                        }
                    }
                }
            }

            return cnt;
        }

        //'
        //'  同一グループ（横・縦列、スクエア）上に該当ナンバーが候補となっているマスがいくつあるかをチェック
        //'
        private int Count_ProspectNo_On_Group(Coordinate myTarget, ref List<Coordinate> myPlace, SudokuGrid[,] tmpNumberGrid)
        {

            int i, j, sNo = 0;
            int cnt = 0;

            myPlace = new List<Coordinate>();

            for (j = 1; j <= GridCount; j++)
            {
                for (i = 1; i <= GridCount; i++)
                {
                    if (i == myTarget.X || j == myTarget.Y || Get_SquareNo(i, j, ref sNo) == myTarget.S)
                    {
                        if (tmpNumberGrid[i, j].ProspectNo.IndexOf(myTarget.No) >= 0)
                        {
                            cnt++;
                            myPlace.Add(new Coordinate(i, j, 0, myTarget.No, 0));
                        }
                    }
                }
            }

            return cnt;

        }

        //'
        //'　既存のリンクに指定の座標がつながるかをチェック   
        //'
        //        private int Get_LinkNo(List<Coordinate>[] myLink, Coordinate targetCoordinate, ref Coordinate pairCoordinate) {
        private int Get_LinkNo(List<Coordinate>[] myLink, Coordinate targetCoordinate, ref int pairColorNo)
        {

            int i, j;

            for (i = 0; i < myLink.Length; i++)
            {
                for (j = 0; j < myLink[i].Count; j++)
                {
                    if (targetCoordinate.X == myLink[i][j].X && targetCoordinate.Y == myLink[i][j].Y)
                    {
                        pairColorNo = myLink[i][j].ColorNo * -1 + 1; //  'リンク元が0ならば1、1ならば0
                                                                     //                        pairCoordinate.ColorNo = (myLink[i][j].ColorNo - 1) * -1 + 2; //  'リンク元が1ならば2、2ならば1
                        return i;
                    }
                }
            }
            //'既存のリンクに含まれない場合、新しいリンクNoを取得
            pairColorNo = 0;
            //            pairCoordinate.ColorNo = 1;
            return myLink.Length;
        }


        //'
        //'　リンク情報リスト(myLink)に指定の座標が既に存在しているかをチェック   
        //''
        private bool Exist_Coordinate(List<Coordinate>[] myLink, Coordinate targetCoordinate)
        {

            int i, j;

            for (i = 0; i < myLink.Length; i++)
            {
                for (j = 0; j < myLink[i].Count; j++)
                {
                    if (myLink[i][j].X == targetCoordinate.X && myLink[i][j].Y == targetCoordinate.Y)
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        //'
        //'  指定した２つのマスのそれぞれと同じグループ（横・縦列、スクエア）に属するマスの情報を取得し配列に収納
        //'
        private List<Coordinate> Get_MutualGrid(Coordinate myCoordinateA, Coordinate myCoordinateB)
        {

            Coordinate tmpCoordinate;
            int i, j, p;
            List<Coordinate> returnList = new List<Coordinate>();

            for (j = 1; j <= GridCount; j++)
            {
                for (i = 1; i <= GridCount; i++)
                {
                    if ((i != myCoordinateA.X || j != myCoordinateA.Y) && (i != myCoordinateB.X || j != myCoordinateB.Y))
                    {
                        tmpCoordinate = new Coordinate(i, j);
                        if (IsSameGroup(tmpCoordinate, myCoordinateA) == true && IsSameGroup(tmpCoordinate, myCoordinateB) == true)
                        {
                            returnList.Add(tmpCoordinate);
                        }
                    }
                }
            }

            return returnList;
        }

        private void Get_MinimumAnserPair(List<SudokuGrid[,]> answerNumberGrid, SudokuGrid[,] tmpNumberGrid, ref Coordinate[] myPlace, ref int minCntAnswer)
        {

            int i, j, n, s, p;
            int x1, x2, y1, y2;
            int cntAroundGrid, ans1, ans2, minCntAround;
            int[,] ansCnt;
            SudokuGrid[,] currentAns;

            Array.Resize(ref myPlace, 2);

            minCntAnswer = answerNumberGrid.Count;
            minCntAround = 8;

            for (y1 = 1; y1 <= (int)(GridCount / 2) + 1; y1++)
            {
                for (x1 = 1; x1 <= GridCount; x1++)
                {
                    x2 = GridCount - x1 + 1;
                    y2 = GridCount - y1 + 1;
                    cntAroundGrid = Count_AroundGrid(x1, y1, tmpNumberGrid);
                    if (tmpNumberGrid[x1, y1].FixNo == 0 && tmpNumberGrid[x2, y2].FixNo == 0 && (x1 != x2 || y1 != y2))
                    {
                        ansCnt = new int[GridCount + 1, GridCount + 1];
                        for (s = 0; s < answerNumberGrid.Count; s++)
                        {
                            currentAns = answerNumberGrid[s];
                            ansCnt[currentAns[x1, y1].FixNo, currentAns[x1, y1].FixNo]++;
                        }
                        for (ans2 = 1; ans2 <= GridCount; ans2++)
                        {
                            for (ans1 = 1; ans1 <= GridCount; ans1++)
                            {
                                if (ansCnt[ans1, ans2] > 0)
                                {
                                    if (ansCnt[ans1, ans2] < minCntAnswer || (ansCnt[ans1, ans2] == minCntAnswer && cntAroundGrid <= minCntAround))
                                    {
                                        minCntAnswer = ansCnt[ans1, ans2];
                                        myPlace[0] = new Coordinate(x1, y1, 0, ans1, 0);
                                        myPlace[1] = new Coordinate(x2, y2, 0, ans2, 0);
                                        if (cntAroundGrid < minCntAround)
                                        {
                                            minCntAround = cntAroundGrid;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        private int Count_AroundGrid(int x, int y, SudokuGrid[,] tmpNumberGrid)
        {

            int i, j;
            int returnInt = 0;

            if (x == 1 || x == GridCount)
            {
                returnInt++;
            }
            if (y == 1 || y == GridCount)
            {
                returnInt++;
            }

            for (j = y - 1; j <= y + 1; j++)
            {
                for (i = x - 1; i <= x + 1; j++)
                {
                    if (j >= 1 && j <= GridCount && i >= 1 && i <= GridCount && (x != i || j != y))
                    {
                        if (tmpNumberGrid[i, j].FixNo > 0)
                        {
                            returnInt++;
                        }
                    }
                }
            }

            return returnInt;

        }


        //'
        //'  問題作成時：次に数値を割当てるマスを決める
        //'　　　1.１つ前に割当てたNoの点対称位置のマスを優先
        //'　　　2.点対称マスが既に割当て済の場合は、現時点で候補No数が最も多いマスを選択（候補数が同じ場合は乱数使用）
        //'

        private bool Assign_NextTaget(ref int[] AssignedNo, SudokuGrid[,] myNumberGrid)
        {

            int x1, x2, y1, y2, p, i;
            int preNo, symmetryNo, newNo = 0;
            List<int> myNoList = new List<int>();

            if (AssignedNo.Length > 0)
            {
                preNo = AssignedNo[AssignedNo.Length - 1]; // '前回割り当てのマスNo
                symmetryNo = GridCount * GridCount - preNo + 1; //  '前回割り当てのマスと点対称位置にあるマス

                //Debug.Write(Convert.ToString(preNo) + "--" + Convert.ToString(symmetryNo) + "\r\n");
                newNo = symmetryNo;
                for (i = 0; i < AssignedNo.Length; i++)
                {
                    if (AssignedNo[i] == symmetryNo)
                    {
                        newNo = 0;
                        break;
                    }
                }
            }

            if (newNo == 0)
            {
                p = GridCount * 2; // '候補No数の初期値
                do
                {
                    for (y1 = 1; y1 <= GridCount / 2 + 1; y1++)
                    {
                        for (x1 = 1; x1 <= ((y1 == 5) ? GridCount / 2 + 1 : GridCount); x1++)
                        {
                            x2 = GridCount - x1 + 1;
                            y2 = GridCount - y1 + 1;
                            if (myNumberGrid[x1, y1].FixNo == 0 && myNumberGrid[x2, y2].FixNo == 0)
                            {
                                if (myNumberGrid[x1, y1].ProspectNo.Count + myNumberGrid[x2, y2].ProspectNo.Count == p)
                                {
                                    myNoList.Add((y1 - 1) * GridCount + x1);
                                    myNoList.Add((y2 - 1) * GridCount + x2);
                                }
                            }
                        }
                    }
                    if (myNoList.Count > 0)
                    {
                        newNo = commonmdl.Generate_Random_FromList(myNoList);
                        break;
                    }
                    p--;

                } while (p >= 2);

            }

            if (newNo > 0)
            {
                Array.Resize(ref AssignedNo, AssignedNo.Length + 1);
                AssignedNo[AssignedNo.Length - 1] = newNo;
                return true;
            }

            return false;

        }


        private void Reset_Hint()
        {

            int i, j;

            //'ヒント表示でエラー発見時
            if (SolveHint.No == 99)
            {
                for (j = 1; j <= GridCount; j++)
                {
                    for (i = 1; i <= GridCount; i++)
                    {
                        SudokuNumberGrid[i, j].FixError = false;
                    }
                }
            }
            SolveHint = new Coordinate();
            HintTimer.Stop();
            HintFlg = false;

            this.PictureBoxGrid.Invalidate();
            this.Tool_Hint.Checked = false;

        }


        private void Reset_AnswerCheck()
        {
            int i, j;

            CheckAnswerFlg = false;
            for (j = 1; j <= GridCount; j++)
            {
                for (i = 1; i <= GridCount; i++)
                {
                    SudokuNumberGrid[i, j].FixError = false;
                }
            }
            this.Tool_CheckAnswer.Checked = false;

        }


        private void Btn_Previous_Click(object sender, EventArgs e)
        {

            if (Convert.ToString(this.Btn_Previous.Image.Tag) == "False")
            {
                return;
            }
            SudokuNumberGrid[2, 3].FixNo = Get_PreviousNo(enterHistory[currentHistoryNo]);

            SudokuNumberGrid[enterHistory[currentHistoryNo].X, enterHistory[currentHistoryNo].Y].FixNo = Get_PreviousNo(enterHistory[currentHistoryNo]);
            if (SudokuNumberGrid[enterHistory[currentHistoryNo].X, enterHistory[currentHistoryNo].Y].FixNo > 0)
            {
                SudokuNumberGrid[enterHistory[currentHistoryNo].X, enterHistory[currentHistoryNo].Y].Locked = AnalyzeMode;
            }
            else
            {
                SudokuNumberGrid[enterHistory[currentHistoryNo].X, enterHistory[currentHistoryNo].Y].Locked = false;

            }

            if (currentHistoryNo > 0)
            {
                currentHistoryNo--;
            }
            if (AnalyzeMode == true)
            {
                Tool_Reset_Click(sender, e);
            }
            Reset_Hint();
            Reset_AnswerCheck();
            Adjust_ProspectNo(ref SudokuNumberGrid);
            this.PictureBoxGrid.Invalidate();
        }

        private void Btn_Next_Click(object sender, EventArgs e)
        {


            if (Convert.ToString(this.Btn_Next.Image.Tag) == "False")
            {
                return;
            }

            if (currentHistoryNo < enterHistory.Length - 1)
            {
                currentHistoryNo++;
                SudokuNumberGrid[enterHistory[currentHistoryNo].X, enterHistory[currentHistoryNo].Y].FixNo = enterHistory[currentHistoryNo].No;
                if (SudokuNumberGrid[enterHistory[currentHistoryNo].X, enterHistory[currentHistoryNo].Y].FixNo > 0)
                {
                    SudokuNumberGrid[enterHistory[currentHistoryNo].X, enterHistory[currentHistoryNo].Y].Locked = AnalyzeMode;
                }
                else
                {
                    SudokuNumberGrid[enterHistory[currentHistoryNo].X, enterHistory[currentHistoryNo].Y].Locked = false;
                }

                if (AnalyzeMode == true)
                {
                    Tool_Reset_Click(sender, e);
                }
                Reset_Hint();
                Reset_AnswerCheck();
                Adjust_ProspectNo(ref SudokuNumberGrid);
                this.PictureBoxGrid.Invalidate();
            }
        }

        private void Tool_Hint_Click(object sender, EventArgs e)
        {

            Coordinate hint = new Coordinate();
            bool myNg = false;
            int intRest, n, nn, i, j, myLv = 0;
            SudokuGrid[,] myNumberGrid = new SudokuGrid[GridCount + 1, GridCount + 1];
            SudokuGrid[,] tmpSudokuNumberGrid = new SudokuGrid[GridCount + 1, GridCount + 1];

            if (HintFlg == true)
            {
                Reset_Hint();
            }
            else
            {
                HintFlg = true;
                this.Tool_Hint.Checked = true;
                for (j = 1; j <= GridCount; j++)
                {
                    for (i = 1; i <= GridCount; i++)
                    {
                        myNumberGrid[i, j] = new SudokuGrid();
                        myNumberGrid[i, j].Copy(SudokuNumberGrid[i, j]);
                    }
                }

                CheckAnswerFlg = false;

                intRest = Solve_Sudoku(2, ref myNg, ref SudokuNumberGrid, ref myLv, ref SolveHint);
                if (intRest == 0)
                {
                    SolveHint.NoB = SolveHint.No;
                }
                else
                {
                    Chk_Hint_BackTrack();
                }

                this.PictureBoxGrid.Invalidate();

                HintTimer.Tag = 0;
                HintTimer.Start();

            }
        }

        private void Chk_Hint_BackTrack()
        {
            int nn, pMin, i, j, dCnt = 1;
            SudokuGrid[,] myNumberGrid = new SudokuGrid[GridCount + 1, GridCount + 1];
            SudokuGrid[,] tmpSudokuNumberGrid = new SudokuGrid[GridCount + 1, GridCount + 1];
            List<SudokuGrid[,]> answerNumberGrid = new List<SudokuGrid[,]>() { };
            bool errFlag = false;

            for (j = 1; j <= GridCount; j++)
            {
                for (i = 1; i <= GridCount; i++)
                {
                    myNumberGrid[i, j] = new SudokuGrid();
                    if (SudokuNumberGrid[i, j].Locked == true)
                    {
                        myNumberGrid[i, j].Copy(SudokuNumberGrid[i, j]);
                    }
                }
            }

            nn = Solve_SudokuBackTrack(new Coordinate(), myNumberGrid, ref answerNumberGrid, ref dCnt);
            if (nn == 1)
            {
                for (j = 1; j <= GridCount; j++)
                {
                    for (i = 1; i <= GridCount; i++)
                    {
                        if (SudokuNumberGrid[i, j].FixNo > 0 && SudokuNumberGrid[i, j].FixNo != answerNumberGrid[0][i, j].FixNo)
                        {
                            SudokuNumberGrid[i, j].FixError = true;
                            SolveHint.X = 0;
                            SolveHint.Y = 0;
                            SolveHint.No = 99;
                            SolveHint.NoB = 0;
                            errFlag = true;
                        }
                    }
                }
            }
            else
            {
                SolveHint.X = 0;
                SolveHint.Y = 0;
                SolveHint.No = 99;
                SolveHint.NoB = 0;
                errFlag = true;
            }

            if (errFlag == false)
            {
                if (SolveHint.X > 0 && SolveHint.Y > 0)
                {
                    SolveHint.NoB = SolveHint.No;
                }
                else
                {
                    pMin = GridCount;
                    for (j = 1; j <= GridCount; j++)
                    {
                        for (i = 1; i <= GridCount; i++)
                        {
                            if (SudokuNumberGrid[i, j].FixNo == 0 && SudokuNumberGrid[i, j].ProspectNo.Count < pMin)
                            {
                                SolveHint.X = i;
                                SolveHint.Y = j;
                                SolveHint.No = answerNumberGrid[0][i, j].FixNo;
                                SolveHint.NoB = SolveHint.No;
                                pMin = SudokuNumberGrid[i, j].ProspectNo.Count;
                            }
                        }
                    }
                }
            }

        }

        private int Get_PreviousNo(Coordinate myCoordinate)
        {

            int i;
            int returnInt = 0;

            for (i = currentHistoryNo; i >= 1; i--)
            {
                if (enterHistory[i].X == myCoordinate.X && enterHistory[i].Y == myCoordinate.Y
                    && enterHistory[i].No != SudokuNumberGrid[myCoordinate.X, myCoordinate.Y].FixNo)
                {
                    return enterHistory[i].No;
                }
            }
            if (AnalyzeMode == true)
            {
                for (i = 1; i < enterHistoryB.Length; i++)
                {
                    if (enterHistoryB[i].X == myCoordinate.X && enterHistoryB[i].Y == myCoordinate.Y
                        && enterHistoryB[i].No != SudokuNumberGrid[myCoordinate.X, myCoordinate.Y].FixNo)
                    {
                        return enterHistoryB[i].No;
                    }
                }
            }

            return 0;

        }

        private void Tool_Reset_Click(Object sender, EventArgs e)
        { // Handles Tool_Reset.Click, Tool_ResetAnswer.Click, Menu_Reset.Click, Menu_ResetAnswer.Click

            int i, j, ret, tNo;
            bool boolInput = false;
            DialogResult result;

            for (j = 1; j <= GridCount; j++)
            {
                for (i = 1; i <= GridCount; i++)
                {
                    if (SudokuNumberGrid[i, j].FixNo > 0 && SudokuNumberGrid[i, j].Locked == false)
                    {
                        boolInput = true;
                        break;
                    }
                }
            }

            if (boolInput == true)
            {
                if (AnalyzeMode == false)
                {
                    result = MessageBox.Show("Clear all input numbers. Are you OK ? ", "Clear Input Numbers", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                }
                else
                {
                    result = DialogResult.Yes;
                }

                if (result == DialogResult.Yes)
                {
                    for (j = 1; j <= GridCount; j++)
                    {
                        for (i = 1; i <= GridCount; i++)
                        {
                            SudokuNumberGrid[i, j].BackColor = Color.White;

                            if (SudokuNumberGrid[i, j].Locked = false || SudokuNumberGrid[i, j].ForeColor != Color.Black)
                            {
                                SudokuNumberGrid[i, j].Locked = false;
                                SudokuNumberGrid[i, j].FixNo = 0;
                                SudokuNumberGrid[i, j].ExcludeNo.Clear();
                                SudokuNumberGrid[i, j].MemoNo.Clear();
                                SudokuNumberGrid[i, j].ForeColor = Color.Black;
                            }
                        }
                    }

                    Adjust_ProspectNo(ref SudokuNumberGrid);
                    CompleteFlg = false;

                    if (AnalyzeMode == false)
                    {
                        Reset_History();
                    }
                    GridMsg = "";

                    Reset_Hint();

                    tNo = Get_DimNo_From_ToolbarName("Highlight");

                    ToolboxInfo[tNo].SelectedNo = 0;

                    this.PictureBoxGrid.Invalidate();
                    this.PictureBoxHighlight.Invalidate();
                    this.PictureBoxMemo.Invalidate();

                }
            }
        }


        private void Tool_CheckAnswer_Click(Object sender, EventArgs e)
        {
            // Handles Tool_CheckAnswer.Click, Menu_CheckAnswer.Click

            int i, j, restGrid, restGridL, n, nn, dCnt = 5;
            bool myNg = false, myNgL = false;
            int myLevel = 0, myMode;
            SudokuGrid[,] tmpSudokuNumberGrid = new SudokuGrid[GridCount + 1, GridCount + 1];
            SudokuGrid[,] tmpSudokuNumberGridL = new SudokuGrid[GridCount + 1, GridCount + 1];
            List<SudokuGrid[,]> answerNumberGrid = new List<SudokuGrid[,]>() { };
            Coordinate myCoordinate;
            Coordinate dHint = new Coordinate();

            if (CheckAnswerFlg == true)
            {
                Reset_AnswerCheck();
            }
            else
            {
                this.Tool_CheckAnswer.Checked = true;

                for (j = 1; j <= GridCount; j++)
                {
                    for (i = 1; i <= GridCount; i++)
                    {
                        tmpSudokuNumberGrid[i, j] = new SudokuGrid();
                        tmpSudokuNumberGrid[i, j].Copy(SudokuNumberGrid[i, j]);
                    }
                }

                myMode = 1;

                restGrid = Solve_Sudoku(myMode, ref myNg, ref tmpSudokuNumberGrid, ref myLevel, ref dHint);

                //                MessageBox.Show("restGrid =" + Convert.ToString(restGrid)); 

                n = 0;
                if (restGrid > 0)
                {
                    for (j = 1; j <= GridCount; j++)
                    {
                        for (i = 1; i <= GridCount; i++)
                        {
                            tmpSudokuNumberGridL[i, j] = new SudokuGrid();
                            if (SudokuNumberGrid[i, j].Locked == true)
                            {
                                tmpSudokuNumberGridL[i, j].Copy(SudokuNumberGrid[i, j]);
                                n++;
                            }

                        }
                    }
                    restGridL = Solve_Sudoku(myMode, ref myNgL, ref tmpSudokuNumberGridL, ref myLevel, ref dHint);
                    //                    MessageBox.Show("restGridL =" + Convert.ToString(restGridL));
                    if (restGridL == 0)
                    {
                        //'                nn = 0
                    }
                    else
                    {
                        nn = Solve_SudokuBackTrack(new Coordinate(), tmpSudokuNumberGridL, ref answerNumberGrid, ref dCnt);
                        if (nn > 1)
                        {
                            return;
                            //Exit Sub
                        }
                        else
                        {
                            for (j = 1; j <= GridCount; j++)
                            {
                                for (i = 1; i <= GridCount; i++)
                                {
                                    if (SudokuNumberGrid[i, j].FixNo > 0 && SudokuNumberGrid[i, j].FixNo != answerNumberGrid[0][i, j].FixNo)
                                    {
                                        tmpSudokuNumberGrid[i, j].FixError = true;
                                    }
                                    else
                                    {
                                        //Debug.Print(" x=" & i & "   y=" & j & "    no=" & tmpSudokuNumberGrid(i, j).FixNo)
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    nn = 1;
                }

                for (j = 1; j <= GridCount; j++)
                {
                    for (i = 1; i <= GridCount; i++)
                    {
                        SudokuNumberGrid[i, j].FixError = tmpSudokuNumberGrid[i, j].FixError;
                    }
                }

                CheckAnswerFlg = true;
            }

            this.PictureBoxGrid.Invalidate();

        }


        private void Tool_DisplayAnswer_Click(Object sender, EventArgs e)
        //            Handles Tool_DisplayAnswer.Click, Menu_DisplayAnswer.Click
        {
            int i, j, n, nn, restGrid, cntMulti;
            int myLevel = 0, myMode;
            bool myNg = false;
            SudokuGrid[,] tmpSudokuNumberGrid = new SudokuGrid[GridCount + 1, GridCount + 1];
            List<SudokuGrid[,]> answerNumberGrid = new List<SudokuGrid[,]>() { };
            DialogResult result;
            Coordinate dHint = new Coordinate();

            if (Check_FixAll() == true) {
                return;
            }

            if (AnalyzeMode == true)
            {
                myMode = 4;
            }
            else
            {
                myMode = 1;

                result = MessageBox.Show("Quit current puzzle and display right answer. Are you OK ?", "Display right answer", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.No)
                {
                    return;
                }
            }

            for (j = 1; j <= GridCount; j++)
            {
                for (i = 1; i <= GridCount; i++)
                {
                    tmpSudokuNumberGrid[i, j] = new SudokuGrid();
                    if (SudokuNumberGrid[i, j].Locked == true)
                    {
                        tmpSudokuNumberGrid[i, j].Copy(SudokuNumberGrid[i, j]);
                    }
                }
            }
            restGrid = Solve_Sudoku(myMode, ref myNg, ref tmpSudokuNumberGrid, ref myLevel, ref dHint);
            for (j = 1; j <= GridCount; j++)
            {
                for (i = 1; i <= GridCount; i++)
                {
                    Debug.Write("myNumberGrid: x=" + Convert.ToString(i) + " y=" + Convert.ToString(j) + "  FixNo:" + tmpSudokuNumberGrid[i, j].FixNo + "\r\n");
                    if (AnalyzeMode == false)
                    {
                        if (SudokuNumberGrid[i, j].FixNo != tmpSudokuNumberGrid[i, j].FixNo && tmpSudokuNumberGrid[i, j].FixNo > 0)
                        {
                            tmpSudokuNumberGrid[i, j].ForeColor = Color.Salmon;
                        }
                        else if (SudokuNumberGrid[i, j].FixNo > 0 && SudokuNumberGrid[i, j].Locked == false)
                        {
                            tmpSudokuNumberGrid[i, j].ForeColor = Color.Blue;
                        }
                    }

                    if (tmpSudokuNumberGrid[i, j].FixNo > 0)
                    {
                        SudokuNumberGrid[i, j].Copy(tmpSudokuNumberGrid[i, j]);
                        SudokuNumberGrid[i, j].BackColor = Color.White;
                        if (AnalyzeMode == false)
                        {
                            SudokuNumberGrid[i, j].Locked = true;
                        }
                    }

                }
            }

            //MessageBox.Show("display" + Convert.ToString(restGrid) + "  mode:" + Convert.ToString(myMode));


            if (restGrid > 0)
            {
                if (myNg == false)
                {

                    cntMulti = 29;
                    nn = Solve_SudokuBackTrack(new Coordinate(), tmpSudokuNumberGrid, ref answerNumberGrid, ref cntMulti);
                    if (nn > 0)
                    {
                        if (cntMulti > 0)
                        {
                            for (j = 1; j <= GridCount; j++)
                            {
                                for (i = 1; i <= GridCount; i++)
                                {
                                    if (AnalyzeMode == false && SudokuNumberGrid[i, j].FixNo != answerNumberGrid[0][i, j].FixNo)
                                    {
                                        //'Debug.Print("InputNO:  x=" & i & "   y=" & j & "   No=" & SudokuNumberGrid[i, j].FixNo)
                                        answerNumberGrid[0][i, j].ForeColor = Color.Salmon;
                                    }//                                End If
                                    SudokuNumberGrid[i, j].Copy(answerNumberGrid[0][i, j]);
                                    if (AnalyzeMode == false)
                                    {
                                        SudokuNumberGrid[i, j].Locked = true;
                                    }
                                }
                            }
                            if (nn > 1)
                            {
                                GridMsg = "There are " + Convert.ToString(nn) + " solutions.";
                                for (n = 1; n < nn; n++)
                                {
                                    for (j = 1; j <= GridCount; j++)
                                    {
                                        for (i = 1; i <= GridCount; i++)
                                        {
                                            if (answerNumberGrid[n][i, j].FixNo != answerNumberGrid[0][i, j].FixNo)
                                            {
                                                SudokuNumberGrid[i, j].BackColor = Color.LightSteelBlue;
                                            }
                                        }
                                    }
                                }
                                                                            
                            }
                        }
                        else
                        {
                            //                        'MsgBox("解は" & nn & "個以上存在します")
                            GridMsg = "There are more than " + Convert.ToString(nn) + " solutions.";
                        }

                    }
                    else
                    {
                        GridMsg = "There is no solution.";
                    }
                }
                else
                {
                    GridMsg = "There is no solution.";
                }

            } else {        //        Else
                nn = 1;
            }

            if (AnalyzeMode == false)
            {
                Reset_History();
            }
            
            this.PictureBoxGrid.Invalidate();

        }

        private void Tool_NewQuestion_Click(Object sender, EventArgs e) {
            //Handles Tool_NewQuestion.Click, Menu_NewQuestion.Click

            DialogResult result;

            if (AnalyzeMode == true)
            {
                if (Check_SavePuzzleData() == false) {
                    return;
                }
                Reset_SudokuGrid();
            } else
            {
                if (ChangeFlg == true) {
                    result = MessageBox.Show("Quit current puzzle and start new puzzle. Are you ready ?", "Start new question", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }
                Display_NewQuestion();

            }

            CurrentGridX = GridCount / 2 + GridCount % 2;
            CurrentGridY = GridCount / 2 + GridCount % 2;
            GridMsg = "";
            Reset_Hint();

            Set_Grid(this.PictureBoxGrid, this.PictureBoxMemo, this.PictureBoxPalette, this.PictureBoxHighlight);

        }

        private void Menu_File_Load_Click(Object sender, EventArgs e) {
            //Handles Menu_File_Load.Click, Tool_File_Load.Click

            String FilePath;

            if (Check_SavePuzzleData() == false) {
                return;
            }

            GridMsg = "";

            Reset_Hint();
            Reset_AnswerCheck();

            FilePath = commonmdl.Get_FilePath_OpenSave(this.OpenFileDialog1, "pzn");

            if (FilePath.Length == 0)
            {
                return;
            }

            Load_NumLogicData(FilePath);

            System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(FilePath));

        }

        private void Menu_File_Save_Click(Object sender, EventArgs e) {
            //Handles Menu_File_Save.Click, Tool_File_Save.Click

            String FilePath;

            if (Check_FixAll() == true && AnalyzeMode == false) {
                return;
            }

            FilePath = commonmdl.Get_FilePath_OpenSave(this.SaveFileDialog1, "pzn");

            if (FilePath.Length > 0)
            {
                Save_NumLogicData(FilePath);
                System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(FilePath));

            }

        }


        private void HintTimer_Tick(Object sender, EventArgs e) {
            //Handles HintTimer.Tick

            if (SolveHint.NoB == 0) {
                SolveHint.NoB = SolveHint.No;
            } else
            {
                SolveHint.NoB = 0;
            }

            this.PictureBoxGrid.Invalidate();

            HintTimer.Tag = Convert.ToString(Convert.ToInt32(HintTimer.Tag) + 1);

            if (Convert.ToInt32(HintTimer.Tag) >= 4) {
                HintTimer.Stop();
            }

        }


    }
}
