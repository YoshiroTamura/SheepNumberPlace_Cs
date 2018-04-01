
Imports System.Drawing
Imports System.Drawing.Drawing2D
'Imports System.Configuration

Public Class FormMain

    'Private Const EM_SETRECT = &HB3             'テキストボックスのフォーマット領域の矩形を設定する

    Protected Friend SudokuNumberGrid(,) As SudokuGrid
    Protected Friend WindowSize As Integer = 3
    Protected Friend GridCount As Integer = 9
    Protected Friend GridSize ' As Integer = 40
    Protected Friend CurrentGridX As Integer = -1
    Protected Friend CurrentGridY As Integer = -1
    Protected Friend AnalyzeMode As Boolean = False

    Private GridStartX As Integer
    Private GridStartY As Integer
    Private GridMsg As String = ""
    Private SolveHint As New Coordinate
    Private NumberFnt As Font
    Private MemoFnt As Font
    Private CompleteFlg As Boolean
    Private CompleteGrd As Double
    Private CheckAnswerFlg As Boolean
    Private HintFlg As Boolean
    Private ChangeFlg As Boolean

    Private PaletteColor() As Color
    Private PaletteLineColor() As Color

    Private HighlightHatch As HatchStyle, HighlightHatchB As HatchStyle

    Private CurrentLevel As Integer = 1
    Private CurrentAssignCnt As Integer

    Private enterHistory() As Coordinate
    Private enterHistoryB() As Coordinate
    Private currentHistoryNo As Integer

    Private UsedTechnique As SolvingTechnique
    Private ToolboxInfo() As ToolboxStatus

    Private Structure SolvingTechnique

        Dim NakedPairTriple As Boolean
        Dim HiddenPairTriple As Boolean
        Dim SimpleColors As Boolean
        Dim XWing As Boolean
        Dim XYWing As Boolean
        Dim SwordFish As Boolean
        Dim MultiColors As Boolean

    End Structure

    Private Structure ToolboxStatus
        Dim Name As String
        Dim Left As Integer
        Dim Top As Integer
        Dim Height As Integer
        Dim Width As Integer
        Dim UnitSize As Integer
        Dim Margin As Integer
        Dim StartX As Integer
        Dim HoverNo As Integer
        Dim MouseDown As Boolean
        Dim SelectedNo As Integer
        Dim Visible As Boolean
    End Structure

    Private Sub FormMain_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Click, LblLevel.Click

        If CurrentGridX >= 0 Then
            CurrentGridX = -1
            CurrentGridY = -1
            Me.PictureBoxGrid.Invalidate()
            Me.PictureBoxMemo.Invalidate()
            Me.PictureBoxPalette.Invalidate()
        End If

        GridMsg = ""
        Call Reset_Hint()
        Call Reset_AnswerCheck()

    End Sub

    Private Sub FormMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim ctl As New Control
        Dim ctl2 As New Control

        ReDim PaletteColor(6)
        ReDim PaletteLineColor(4)

        PaletteColor(0) = Color.White
        PaletteColor(1) = Color.LightBlue
        PaletteColor(2) = Color.LightGreen
        PaletteColor(3) = Color.LightPink
        PaletteColor(4) = Color.LightSalmon
        PaletteColor(5) = Color.White
        PaletteColor(6) = Color.White

        PaletteLineColor(1) = Color.DarkBlue
        PaletteLineColor(2) = Color.DarkGreen
        PaletteLineColor(3) = Color.DeepPink
        PaletteLineColor(4) = Color.OrangeRed



        ChangeFlg = False

        Call Set_Menu_DisplayItem()
        Call Reset_SudokuGrid()

        Call Load_Settings()


        For Each ctl In Me.Panel_PictureBox.Controls
            'Debug.Print("2:" & ctl.Name)

            If TypeName(ctl) = "PictureBox" Then
                If ctl.Name = "PictureBoxGrid" Then
                    AddHandle_PictureBox(ctl)
                ElseIf ctl.Name = "PictureBoxMemo" Then
                    AddHandle_PictureBoxMemo(ctl)
                ElseIf ctl.Name = "PictureBoxPalette" Then
                    AddHandle_PictureBoxPalette(ctl)
                ElseIf ctl.Name = "PictureBoxHighlight" Then
                    AddHandle_PictureBoxHighlight(ctl)
                End If
            End If

        Next


        Call Set_Menu_SizeInfo()
        Call Set_Menu_LevelItem()
        Call Set_Menu_Mode()
        Call Reset_History()
        Call Display_NewQuestion()

        Call Switch_KeypadDisplay()
        Call Switch_ToolButtonEnabled()

        Dim proRenderer As New ToolStripProfessionalRenderer()

        proRenderer.RoundedEdges = False

        Me.ToolStrip1.Renderer = proRenderer

    End Sub

    Private Sub Load_Settings()

        Dim myCfg As System.Configuration.SettingsProperty

        For Each myCfg In My.Settings.Properties
            If myCfg.Name = "WindowSize" Then
                WindowSize = My.Settings(myCfg.Name)
            End If
            If myCfg.Name = "CurrentLevel" Then
                CurrentLevel = My.Settings("CurrentLevel")
            End If
            If myCfg.Name = "DisplayKeypad" Then
                Me.Menu_DisplayKeypad.Checked = My.Settings("DisplayKeypad")

            End If
            'Debug.Print(myCfg.Name & ":" & My.Settings(myCfg.Name))
        Next



    End Sub

    Private Sub Display_CompleteWindow()

        CompleteFlg = Check_Complete()
        If CompleteFlg = True Then
            '            FormComplete.Show()
            Call Reset_History()
            CompleteGrd = 0
            Me.GradientTimer.Tag = 1
            Me.GradientTimer.Interval = 10
            Me.GradientTimer.Start()
        End If

    End Sub

    Private Function Check_Complete() As Boolean


        Dim i As Integer
        Dim j As Integer
        Dim myNg As Boolean

        Check_Complete = False

        For j = 1 To GridCount
            For i = 1 To GridCount
                If SudokuNumberGrid(i, j).FixNo = 0 Then
                    Exit Function
                End If
            Next
        Next
        Call Solve_Sudoku(3, myNg, SudokuNumberGrid)
        If myNg = False Then
            Check_Complete = True
            For j = 1 To GridCount
                For i = 1 To GridCount
                    If SudokuNumberGrid(i, j).Locked = False Then
                        '                        SudokuNumberGrid(i, j).Locked = True
                        SudokuNumberGrid(i, j).ForeColor = Color.Turquoise
                        'SudokuNumberGrid(i, j).ForeColor = Color.DeepPink
                    End If
                    SudokuNumberGrid(i, j).BackColor = Color.Honeydew
                    'SudokuNumberGrid(i, j).BackColor = Color.Transparent
                Next
            Next

        End If


    End Function

    Private Sub Reset_History()

        Dim i As Integer
        Dim j As Integer
        Dim n As Integer

        ReDim enterHistory(0)
        enterHistory(0) = New Coordinate
        currentHistoryNo = 0
        'Me.Chk_Change.Checked = False
        ChangeFlg = False


        If AnalyzeMode = True Then
            ReDim enterHistoryB(0)
            enterHistoryB(0) = New Coordinate
            n = 0
            For j = 1 To GridCount
                For i = 1 To GridCount
                    If SudokuNumberGrid(i, j).FixNo > 0 Then
                        n = n + 1
                        ReDim Preserve enterHistoryB(n)
                        enterHistoryB(n) = New Coordinate
                        enterHistoryB(n).X = i
                        enterHistoryB(n).Y = j
                        enterHistoryB(n).No = SudokuNumberGrid(i, j).FixNo
                        enterHistoryB(n).NoB = 1
                    End If
                Next
            Next
        End If

    End Sub

    Private Sub AddHandle_PictureBox(ByVal myCtl As Control)

        AddHandler myCtl.Paint, AddressOf CtlPictureBox_Paint
        AddHandler myCtl.MouseDown, AddressOf CtlPictureBox_MouseDown

    End Sub

    Private Sub AddHandle_PictureBoxMemo(ByVal myCtl As Control)

        AddHandler myCtl.Paint, AddressOf CtlPictureBoxMemo_Paint
        AddHandler myCtl.MouseDown, AddressOf CtlPictureBoxMemo_MouseDown
        AddHandler myCtl.MouseUp, AddressOf CtlPictureBoxMemo_MouseUp
        AddHandler myCtl.MouseMove, AddressOf CtlPictureBoxMemo_MouseMove
        AddHandler myCtl.MouseLeave, AddressOf CtlPictureBoxMemo_MouseLeave

    End Sub

    Private Sub AddHandle_PictureBoxPalette(ByVal myCtl As Control)

        AddHandler myCtl.Paint, AddressOf CtlPictureBoxPalette_Paint
        AddHandler myCtl.MouseDown, AddressOf CtlPictureBoxPalette_MouseDown
        AddHandler myCtl.MouseUp, AddressOf CtlPictureBoxPalette_MouseUp
        AddHandler myCtl.MouseMove, AddressOf CtlPictureBoxPalette_MouseMove
        AddHandler myCtl.MouseLeave, AddressOf CtlPictureBoxPalette_MouseLeave

    End Sub

    Private Sub AddHandle_PictureBoxHighlight(ByVal myCtl As Control)

        AddHandler myCtl.Paint, AddressOf CtlPictureBoxHighlight_Paint
        AddHandler myCtl.MouseDown, AddressOf CtlPictureBoxHighlight_MouseDown
        AddHandler myCtl.MouseUp, AddressOf CtlPictureBoxHighlight_MouseUp
        AddHandler myCtl.MouseMove, AddressOf CtlPictureBoxHighlight_MouseMove
        AddHandler myCtl.MouseLeave, AddressOf CtlPictureBoxHighlight_MouseLeave

    End Sub

    Private Sub CtlPictureBox_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs)

        Dim p1 As New Pen(Color.DarkOliveGreen, 1)
        Dim p2 As New Pen(Color.DarkOliveGreen, 3)
        Dim HighlightColor As Color

        Dim GridBrush As SolidBrush
        Dim GridNoProspect As Boolean
        Dim nonlockFlg As Boolean
        Dim DuplicateFlg As Boolean

        Dim sf As New StringFormat
        sf.LineAlignment = StringAlignment.Center
        sf.Alignment = StringAlignment.Center

        Dim sfp As New StringFormat()
        sfp.LineAlignment = StringAlignment.Far
        sfp.Alignment = StringAlignment.Center

        Dim numberBrush As SolidBrush
        Dim HintNumberBrush As New SolidBrush(Color.Red)

        Dim i As Integer, j As Integer, n As Integer
        Dim memoBrush As New SolidBrush(Color.Maroon)

        Dim x1 As Integer, x2 As Integer
        Dim y1 As Integer, y2 As Integer

        Dim tNo As Integer


        Dim imgRight As Image = My.Resources.maru
        Dim imgWrong As Image = My.Resources.batsu    'Image.FromFile("batsu.jpg")
        Dim imgRightWrong As Image
        Dim imgFailure As Image = My.Resources.failure

        Dim cm As New System.Drawing.Imaging.ColorMatrix()

        cm.Matrix00 = 1.0F
        cm.Matrix11 = 1.0F
        cm.Matrix22 = 1.0F
        cm.Matrix33 = 0.5F
        cm.Matrix44 = 1.0F

        Dim ia As New System.Drawing.Imaging.ImageAttributes()
        'ColorMatrixを設定する
        ia.SetColorMatrix(cm)

        Call Switch_ToolButtonEnabled(True)

        If CompleteFlg = True And CompleteGrd >= 1 Then
            Dim imgSheep As Image
            Dim imgRate_w As Double
            Dim imgRate_h As Double
            imgSheep = My.Resources.sheep3

            If imgSheep.Width > imgSheep.Height Then
                imgRate_w = 0.9
                imgRate_h = 0.9 * imgSheep.Height / imgSheep.Width
            Else
                imgRate_h = 0.9
                imgRate_w = 0.9 * imgSheep.Width / imgSheep.Height
            End If

            e.Graphics.DrawImage(imgSheep, _
                                        New Rectangle(GridStartX + (1.0 - imgRate_w) * (GridSize * GridCount) / 2, _
                                                      GridStartY + (1.0 - imgRate_h) * (GridSize * GridCount) / 2, _
                                                      (GridSize * GridCount) * imgRate_w, (GridSize * GridCount) * imgRate_h), _
                                        0, 0, imgSheep.Width, imgSheep.Height, GraphicsUnit.Pixel)
            imgSheep.Dispose()
        End If


        For j = 1 To GridCount
            For i = 1 To GridCount
                'If CheckAnswerFlg = True And SudokuNumberGrid(i, j).FixNo > 0 And SudokuNumberGrid(i, j).Locked = False Then
                '    If SudokuNumberGrid(i, j).FixError = False Then
                '        imgRightWrong = imgRight
                '    Else
                '        imgRightWrong = imgWrong
                '    End If

                '    e.Graphics.DrawImage(imgRightWrong, _
                '                    New Rectangle(GridStartX + GridSize * (i - 1) + 3, _
                '                                      GridStartY + GridSize * (j - 1) + 3, _
                '                                      GridSize - 6, GridSize - 6), _
                '                    0, 0, imgRightWrong.Width, imgRightWrong.Height, GraphicsUnit.Pixel, ia)
                'End If

                GridBrush = New SolidBrush(SudokuNumberGrid(i, j).BackColor)
                GridNoProspect = False
                DuplicateFlg = False
                If SudokuNumberGrid(i, j).BackColor <> Color.White Then
                    GridBrush = New SolidBrush(SudokuNumberGrid(i, j).BackColor)
                ElseIf AnalyzeMode = True Then
                    If DuplicateNumber(SudokuNumberGrid, New Coordinate(i, j)) = True Then '重複
                        GridBrush = New SolidBrush(Color.Yellow)
                        DuplicateFlg = True
                        'GridMsg = "重複しています"
                    ElseIf SudokuNumberGrid(i, j).ProspectNo.Count = 0 Then   '破綻
                        e.Graphics.DrawImage(imgFailure, _
                                        New Rectangle(GridStartX + GridSize * (i - 1) + 3, _
                                                          GridStartY + GridSize * (j - 1) + 3, _
                                                          GridSize - 6, GridSize - 6), _
                                        0, 0, imgFailure.Width, imgFailure.Height, GraphicsUnit.Pixel)


                    End If
                End If
                If GridBrush.Color <> Color.White Then
                    e.Graphics.FillRectangle(GridBrush, GridStartX + GridSize * (i - 1), _
                                                 GridStartY + GridSize * (j - 1), _
                                                 GridSize, GridSize)

                End If
                If GridNoProspect = True Then
                    x1 = GridStartX + GridSize * (i - 1) + 1
                    x2 = x1 + GridSize - 2
                    y1 = GridStartY + GridSize * (j - 1) + 1
                    y2 = y1 + GridSize - 2
                    e.Graphics.DrawLine(New Pen(Color.Orange, 1), x1, y1, x2, y2)
                    e.Graphics.DrawLine(New Pen(Color.Orange, 1), x1, y2, x2, y1)

                End If


                tNo = Get_DimNo_From_ToolbarName("Highlight")

                If ToolboxInfo(tNo).SelectedNo > 0 Then
                    If SudokuNumberGrid(i, j).FixNo = ToolboxInfo(tNo).SelectedNo Then
                        If AnalyzeMode = True Then
                            HighlightColor = Color.LightGreen
                        Else
                            HighlightColor = Color.Gold
                        End If
                        If SudokuNumberGrid(i, j).BackColor = Color.White Then
                            e.Graphics.FillRectangle(New SolidBrush(HighlightColor), _
                                                         GridStartX + GridSize * (i - 1), _
                                                         GridStartY + GridSize * (j - 1), _
                                                         GridSize, GridSize)
                        Else
                            e.Graphics.FillRectangle(New HatchBrush(HighlightHatchB, SudokuNumberGrid(i, j).BackColor, HighlightColor), _
                                                         GridStartX + GridSize * (i - 1), _
                                                         GridStartY + GridSize * (j - 1), _
                                                         GridSize, GridSize)
                        End If
                    ElseIf (SudokuNumberGrid(i, j).FixNo = 0 _
                            And SudokuNumberGrid(i, j).ProspectNo.IndexOf(ToolboxInfo(tNo).SelectedNo) < 0) _
                            Or SudokuNumberGrid(i, j).FixNo > 0 Then
                        e.Graphics.FillRectangle(New HatchBrush(HighlightHatch, Color.Silver, Color.Transparent), _
                                                     GridStartX + GridSize * (i - 1), _
                                                     GridStartY + GridSize * (j - 1), _
                                                     GridSize, GridSize)
                    End If
                End If

                If CheckAnswerFlg = True And SudokuNumberGrid(i, j).FixNo > 0 And SudokuNumberGrid(i, j).Locked = False Then
                    If SudokuNumberGrid(i, j).FixError = False Then
                        imgRightWrong = imgRight
                    Else
                        imgRightWrong = imgWrong
                    End If

                    e.Graphics.DrawImage(imgRightWrong, _
                                    New Rectangle(GridStartX + GridSize * (i - 1) + 3, _
                                                      GridStartY + GridSize * (j - 1) + 3, _
                                                      GridSize - 6, GridSize - 6), _
                                    0, 0, imgRightWrong.Width, imgRightWrong.Height, GraphicsUnit.Pixel, ia)
                End If


                'シンメトリー
                If AnalyzeMode = True And Me.Chk_SymmetricGrid.Checked = True Then
                    If SudokuNumberGrid(i, j).FixNo = 0 _
                       And SudokuNumberGrid(GridCount - i + 1, GridCount - j + 1).FixNo > 0 _
                       And SudokuNumberGrid(GridCount - i + 1, GridCount - j + 1).Locked = True Then
                        '                       And GridHatch = False Then

                        e.Graphics.FillRectangle(New HatchBrush(HatchStyle.OutlinedDiamond, Color.SkyBlue, Color.Transparent), _
                                                     GridStartX + GridSize * (i - 1), _
                                                     GridStartY + GridSize * (j - 1), _
                                                     GridSize, GridSize)
                    End If
                End If

                If DuplicateFlg = True Then
                    e.Graphics.DrawString("Duplicate", MemoFnt, New SolidBrush(Color.Red), _
                                   GridStartX + GridSize * (i - 0.5), _
                                   GridStartY + GridSize * (j), _
                                   sfp)
                End If

            Next
        Next

        For j = 1 To GridCount
            For i = 1 To GridCount
                '--------------------------------------------------------------------------------------------------------------
                'If SudokuNumberGrid(i, j).Locked = True Then
                '    e.Graphics.FillRectangle(New SolidBrush(Color.AliceBlue), GridStartX + GridSize * (i - 1), _
                '                                 GridStartY + GridSize * (j - 1), _
                '                                 GridSize, GridSize)

                'End If
                '--------------------------------------------------------------------------------------------------------------
                If SudokuNumberGrid(i, j).FixNo > 0 Then
                    'ヒント時に間違いがある場合、該当マスに×を表示
                    If CheckAnswerFlg = False And SudokuNumberGrid(i, j).FixError = True And SolveHint.NoB = 0 Then
                        x1 = GridStartX + GridSize * (i - 1) + 3
                        y1 = GridStartY + GridSize * (j - 1) + 3
                        If SolveHint.NoB = 0 Then
                            e.Graphics.DrawImage(imgWrong, _
                                            New Rectangle(x1, y1, GridSize - 6, GridSize - 6), _
                                            0, 0, imgWrong.Width, imgWrong.Height, GraphicsUnit.Pixel, ia)
                        End If

                    End If

                    If SudokuNumberGrid(i, j).ForeColor <> Color.Black Then
                        numberBrush = New SolidBrush(SudokuNumberGrid(i, j).ForeColor)
                    Else
                        If SudokuNumberGrid(i, j).Locked = True Then
                            numberBrush = New SolidBrush(Color.Black)
                        Else
                            If AnalyzeMode = True Then
                                numberBrush = New SolidBrush(Color.Coral)
                            Else
                                numberBrush = New SolidBrush(Color.Blue)
                            End If
                            nonlockFlg = True
                        End If
                    End If
                    e.Graphics.DrawString(SudokuNumberGrid(i, j).FixNo, NumberFnt, numberBrush, _
                                   GridStartX + GridSize * (i - 0.5), GridStartY + GridSize * (j - 0.5), sf)
                End If
                'メモNo表示
                If SudokuNumberGrid(i, j).MemoNo.Count > 0 Then

                    For n = 0 To SudokuNumberGrid(i, j).MemoNo.Count - 1
                        If SudokuNumberGrid(i, j).MemoNo(n) > 0 Then
                            e.Graphics.DrawString(Math.Abs(SudokuNumberGrid(i, j).MemoNo(n)), MemoFnt, memoBrush, _
                                           GridStartX + GridSize * (i - 1 + (((Math.Abs(SudokuNumberGrid(i, j).MemoNo(n)) - 1) Mod 3) * 2 + 1) / 6), _
                                           GridStartY + GridSize * (j - 1 + (((Math.Abs(SudokuNumberGrid(i, j).MemoNo(n)) - 1) \ 3) * 2 + 1) / 6) + 2, _
                                           sf)
                        End If
                    Next
                End If

                '候補No表示（問題作成モード時）
                If AnalyzeMode = True Then
                    If SudokuNumberGrid(i, j).ProspectNo.Count > 0 And SudokuNumberGrid(i, j).FixNo = 0 Then
                        If Me.Chk_DisplayProspect.Checked = True Then
                            For n = 0 To SudokuNumberGrid(i, j).ProspectNo.Count - 1
                                e.Graphics.DrawString(SudokuNumberGrid(i, j).ProspectNo(n), MemoFnt, New SolidBrush(Color.MediumPurple), _
                                               GridStartX + GridSize * (i - 1 + (((SudokuNumberGrid(i, j).ProspectNo(n) - 1) Mod 3) * 2 + 1) / 6), _
                                               GridStartY + GridSize * (j - 1 + (((SudokuNumberGrid(i, j).ProspectNo(n) - 1) \ 3) * 2 + 1) / 6) + 1, _
                                               sf)
                            Next
                        End If
                    End If
                End If



            Next i
        Next j

        'ヒント
        If SolveHint.X > 0 And SolveHint.Y > 0 Then
            x1 = GridStartX + GridSize * (SolveHint.X - 1) + 3
            y1 = GridStartY + GridSize * (SolveHint.Y - 1) + 3

            Dim imgSheep2 As Image
            imgSheep2 = My.Resources.sheep1

            e.Graphics.DrawImage(imgSheep2, _
                            New Rectangle(x1, y1, GridSize - 6, GridSize - 6), _
                            0, 0, imgSheep2.Width, imgSheep2.Height, GraphicsUnit.Pixel, ia)



            If SolveHint.NoB > 0 Then


                e.Graphics.DrawString(SolveHint.NoB, NumberFnt, HintNumberBrush, _
                               GridStartX + GridSize * (SolveHint.X - 0.5), GridStartY + GridSize * (SolveHint.Y - 0.5), sf)


            End If
        End If

        '横線
        For i = 0 To GridCount
            '３マス毎に線を太くする　
            If i Mod 3 = 0 Then
                '                p = p2
            Else
                x1 = GridStartX
                x2 = GridStartX + GridSize * GridCount
                y1 = GridStartY + GridSize * i
                y2 = y1
                e.Graphics.DrawLine(p1, x1, y1, x2, y2)
            End If
        Next

        '縦線
        For j = 0 To GridCount
            If j Mod 3 = 0 Then
                '                p = p2
            Else
                y1 = GridStartY
                y2 = GridStartY + GridSize * GridCount
                x1 = GridStartX + GridSize * j
                x2 = x1
                e.Graphics.DrawLine(p1, x1, y1, x2, y2)
            End If
        Next

        'スクエア
        For i = 1 To GridCount
            x1 = GridStartX + (Get_XY_From_SquareNo(i, 1).X - 1) * GridSize
            x2 = x1 + GridSize * CInt(Math.Sqrt(GridCount)) - 1
            y1 = GridStartY + (Get_XY_From_SquareNo(i, 1).Y - 1) * GridSize
            y2 = y1 + GridSize * CInt(Math.Sqrt(GridCount)) - 1
            e.Graphics.DrawRectangle(p2, x1, y1, GridSize * CInt(Math.Sqrt(GridCount)), GridSize * CInt(Math.Sqrt(GridCount)))
        Next

        If CurrentGridY > 0 And CurrentGridY <= GridCount _
            And CurrentGridX > 0 And CurrentGridX <= GridCount Then
            '現在のマス目を強調表示
            e.Graphics.DrawRectangle(New Pen(Color.Red, 3), GridStartX + GridSize * (CurrentGridX - 1), _
                                         GridStartY + GridSize * (CurrentGridY - 1), _
                                         GridSize, GridSize)
        End If

        'パズル完成時メッセージ表示
        If CompleteFlg = True Then
            Dim CompleteFnt As New Font("MS UI Gothic", NumberFnt.Size, FontStyle.Bold)
            '     Dim CompleteFnt As New Font("Century", NumberFnt.Size, FontStyle.Bold)
            Dim strComplete As String = "Congratulations!"

            x1 = GridStartX + (GridSize * GridCount) / 2 - e.Graphics.MeasureString(strComplete, CompleteFnt).Width / 2
            x2 = x1 + e.Graphics.MeasureString(strComplete, CompleteFnt).Width
            y1 = GridStartY + (GridSize * (GridCount \ 2)) - CompleteFnt.Height / 2
            y2 = y1 + CompleteFnt.Height

            If CompleteGrd >= 1 Then
                e.Graphics.DrawString(strComplete, CompleteFnt, Brushes.Blue, _
                            GridStartX + (GridSize * GridCount) / 2, GridStartY + (GridSize * (GridCount \ 2)), sf)
            Else

                Dim bg As New LinearGradientBrush(New Point(x1, 0), New Point(x1 + (x2 - x1) * (CompleteGrd - Int(CompleteGrd)) + 1, 0), _
                                        Color.Red, Color.Yellow) ', LinearGradientMode.Vertical)
                e.Graphics.DrawString(strComplete, CompleteFnt, bg, _
                            GridStartX + (GridSize * GridCount) / 2, GridStartY + (GridSize * (GridCount \ 2)), sf)
                bg.Dispose()
            End If



            'メッセージ表示
        ElseIf GridMsg.Length > 0 Then
            Dim MsgFnt As New Font("MS UI Gothic", NumberFnt.Size - 4, FontStyle.Bold)
            '            Dim MsgFnt As New Font("MS UI Gothic", NumberFnt.Size - 4, FontStyle.Bold)
            '            Dim bm As New SolidBrush(Color.Red)
            Dim bm1 As New SolidBrush(Color.Gold)
            Dim bm2 As New SolidBrush(Color.FromArgb(128, Color.Black))


            x1 = GridStartX + (GridSize * GridCount) / 2 - e.Graphics.MeasureString(GridMsg, MsgFnt).Width / 2
            x2 = x1 + e.Graphics.MeasureString(GridMsg, MsgFnt).Width
            y1 = GridStartY + (GridSize * (GridCount \ 2)) - MsgFnt.Height / 2
            y2 = y1 + MsgFnt.Height

            e.Graphics.FillRectangle(bm2, New Rectangle(x1 - 3, y1 - 3, x2 - x1 + 6, y2 - y1 + 6))
            e.Graphics.DrawString(GridMsg, MsgFnt, bm1, _
                        GridStartX + (GridSize * GridCount) / 2, GridStartY + (GridSize * (GridCount \ 2)), sf)
            bm1.Dispose()
            bm2.Dispose()

        End If


        If currentHistoryNo > 0 Then
            Me.Btn_Previous.Image = My.Resources.arrow_l
            Me.Btn_Previous.Image.Tag = "True"
        Else
            Me.Btn_Previous.Image = My.Resources.arrow_ld
            Me.Btn_Previous.Image.Tag = "False"
        End If

        If currentHistoryNo < UBound(enterHistory) Then
            Me.Btn_Next.Image = My.Resources.arrow_r
            Me.Btn_Next.Image.Tag = "True"
        Else
            Me.Btn_Next.Image = My.Resources.arrow_rd
            Me.Btn_Next.Image.Tag = "False"
        End If

    End Sub

    Private Sub CtlPictureBox_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)

        Dim myPicturebox As PictureBox = sender
        Dim memoPicturebox As PictureBox = Me.PictureBoxMemo
        Dim palettePicturebox As PictureBox = Me.PictureBoxPalette

        Dim x As Integer = myPicturebox.PointToClient(System.Windows.Forms.Cursor.Position).X
        Dim y As Integer = myPicturebox.PointToClient(System.Windows.Forms.Cursor.Position).Y
        Dim cX As Integer, cY As Integer
        Dim boolCheckNumber As Boolean = False
        Dim myNo As Integer

        If e.Button = Windows.Forms.MouseButtons.Right Then
            Call Get_SudokuGridXY_From_Coordinate(x, y, cX, cY)
            '            If CurrentGridX > 0 And CurrentGridX <= GridCount And CurrentGridY > 0 And CurrentGridY <= GridCount Then
            If cX = CurrentGridX And cY = CurrentGridY Then
                If SudokuNumberGrid(CurrentGridX, CurrentGridY).Locked = False Or AnalyzeMode = True Then
                    myNo = SudokuNumberGrid(CurrentGridX, CurrentGridY).FixNo + 1
                    If myNo < 0 Then
                        myNo = myNo + (GridCount + 1)
                    ElseIf myNo > GridCount Then
                        myNo = myNo - (GridCount + 1)
                    End If
                    Call Input_Number(myNo)
                End If
            End If
        Else
            Call Get_SudokuGridXY_From_Coordinate(x, y, CurrentGridX, CurrentGridY)
        End If

        GridMsg = ""
        Call Reset_Hint()
        Call Reset_AnswerCheck()

        myPicturebox.Invalidate()
        memoPicturebox.Invalidate()
        palettePicturebox.Invalidate()

    End Sub

    Private Sub CtlPictureBoxMemo_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs)

        Dim penLine As New Pen(Color.Gray, 1)
        Dim sf As New StringFormat()
        sf.LineAlignment = StringAlignment.Center
        sf.Alignment = StringAlignment.Near

        Dim sfp As New StringFormat()
        sfp.LineAlignment = StringAlignment.Center
        sfp.Alignment = StringAlignment.Center
        Dim currentBrush As Brush
        Dim MemoBrush As New SolidBrush(Color.Navy)
        Dim MemoBrushChecked As New SolidBrush(Color.White)
        Dim mFnt As Font

        Dim i As Integer, j As Integer, n As Integer
        Dim prospectBrush As New SolidBrush(Color.Green)
        Dim x1 As Integer, x2 As Integer
        Dim y1 As Integer, y2 As Integer

        Dim tNo As Integer

        tNo = Get_DimNo_From_ToolbarName("Memo")

        '外枠
        e.Graphics.DrawRectangle(New Pen(Color.DimGray, 1), 0, 0, sender.Width - 1, sender.Height - 1)
        e.Graphics.DrawRectangle(New Pen(Color.Silver, 1), 1, 1, sender.Width - 3, sender.Height - 3)

        e.Graphics.DrawString("Check", MemoFnt, MemoBrush, _
                      GridStartX + 5, ToolboxInfo(tNo).UnitSize * 0.25 + ToolboxInfo(tNo).Margin, sf)
        e.Graphics.DrawString("Number", MemoFnt, MemoBrush, _
                      GridStartX + 5, ToolboxInfo(tNo).UnitSize * 0.75 + ToolboxInfo(tNo).Margin, sf)

        For i = 1 To GridCount + 1

            x1 = GridStartX + ToolboxInfo(tNo).StartX + (ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin) * (i - 1)
            y1 = ToolboxInfo(tNo).Margin
            x2 = x1 + ToolboxInfo(tNo).UnitSize - 1
            y2 = y1 + ToolboxInfo(tNo).UnitSize - 1

            If i = ToolboxInfo(tNo).HoverNo Then
                If ToolboxInfo(tNo).MouseDown = True Then
                    ControlPaint.DrawBorder(e.Graphics, New Rectangle(x1 - ToolboxInfo(tNo).Margin \ 2, y1 - ToolboxInfo(tNo).Margin \ 2, ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin, ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin), Color.LightGray, ButtonBorderStyle.Inset)
                Else
                    ControlPaint.DrawBorder(e.Graphics, New Rectangle(x1 - ToolboxInfo(tNo).Margin \ 2, y1 - ToolboxInfo(tNo).Margin \ 2, ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin, ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin), Color.LightGray, ButtonBorderStyle.Outset)
                End If
            End If
            e.Graphics.DrawRectangle(penLine, x1, y1, ToolboxInfo(tNo).UnitSize, ToolboxInfo(tNo).UnitSize)
            currentBrush = MemoBrush
            If CurrentGridX > 0 And CurrentGridX <= GridCount _
               And CurrentGridY > 0 And CurrentGridY <= GridCount Then
                If SudokuNumberGrid(CurrentGridX, CurrentGridY).MemoNo.IndexOf(i) >= 0 Then
                    e.Graphics.FillRectangle(MemoBrush, _
                                                 x1 + 1, _
                                                 y1 + 1, _
                                                 ToolboxInfo(tNo).UnitSize - 1, ToolboxInfo(tNo).UnitSize - 1)
                    currentBrush = MemoBrushChecked
                    mFnt = New Font(MemoFnt.FontFamily, MemoFnt.Size, FontStyle.Bold)
                End If
            End If
            If i <= GridCount Then
                e.Graphics.DrawString(i, MemoFnt, currentBrush, _
                         x1 + ToolboxInfo(tNo).UnitSize * 0.5 + 1, _
                         y1 + ToolboxInfo(tNo).UnitSize * 0.5, sfp)
            Else
                e.Graphics.DrawString("Del", MemoFnt, New SolidBrush(Color.Red), _
                         x1 + ToolboxInfo(tNo).UnitSize * 0.5 + 1, _
                         y1 + ToolboxInfo(tNo).UnitSize * 0.5, sfp)
            End If



        Next

    End Sub

    Private Sub CtlPictureBoxMemo_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)

        Dim memoPicturebox As PictureBox = sender
        Dim x As Integer = memoPicturebox.PointToClient(System.Windows.Forms.Cursor.Position).X
        Dim y As Integer = memoPicturebox.PointToClient(System.Windows.Forms.Cursor.Position).Y
        Dim SelectedMemoNo As Integer
        Dim intScratch As Integer
        Dim tNo As Integer

        tNo = Get_DimNo_From_ToolbarName("Memo")

        ToolboxInfo(tNo).MouseDown = True

        If CurrentGridX > 0 And CurrentGridX <= GridCount _
           And CurrentGridY > 0 And CurrentGridY <= GridCount Then
            If SudokuNumberGrid(CurrentGridX, CurrentGridY).Locked = False Then
                SelectedMemoNo = Get_MemoNo_From_Coordinate(x, y)
                If SelectedMemoNo > 0 And SelectedMemoNo <= GridCount Then
                    If SudokuNumberGrid(CurrentGridX, CurrentGridY).MemoNo.IndexOf(SelectedMemoNo) >= 0 Then
                        SudokuNumberGrid(CurrentGridX, CurrentGridY).MemoNo.Remove(SelectedMemoNo)
                    Else
                        SudokuNumberGrid(CurrentGridX, CurrentGridY).MemoNo.Add(SelectedMemoNo)
                    End If
                ElseIf SelectedMemoNo = GridCount + 1 And SudokuNumberGrid(CurrentGridX, CurrentGridY).MemoNo.Count > 0 Then
                    'If SudokuNumberGrid(CurrentGridX, CurrentGridY).FixNo = 0 Then
                    '    intScratch = MsgBox("候補Noチェックを全てクリアします。", MsgBoxStyle.YesNo, "候補Noチェッククリア")
                    '    If intScratch = vbYes Then
                    '        SudokuNumberGrid(CurrentGridX, CurrentGridY).MemoNo.Clear()
                    '    End If
                    'Else
                    SudokuNumberGrid(CurrentGridX, CurrentGridY).MemoNo.Clear()
                    'End If
                    ToolboxInfo(tNo).MouseDown = False
                End If

            End If

        End If

        memoPicturebox.Invalidate()
        Me.PictureBoxGrid.Invalidate()

    End Sub

    Private Sub CtlPictureBoxMemo_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)

        Dim memoPicturebox As PictureBox = sender
        Dim tNo As Integer

        tNo = Get_DimNo_From_ToolbarName("Memo")

        ToolboxInfo(tNo).MouseDown = False
        memoPicturebox.Invalidate()

    End Sub

    Private Sub CtlPictureBoxMemo_MouseMove(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim memoPicturebox As PictureBox = sender
        Dim x As Integer = memoPicturebox.PointToClient(System.Windows.Forms.Cursor.Position).X
        Dim y As Integer = memoPicturebox.PointToClient(System.Windows.Forms.Cursor.Position).Y
        Dim tNo As Integer

        tNo = Get_DimNo_From_ToolbarName("Memo")

        ToolboxInfo(tNo).HoverNo = Get_MemoNo_From_Coordinate(x, y)

        memoPicturebox.Invalidate()

    End Sub

    Private Sub CtlPictureBoxMemo_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim memoPicturebox As PictureBox = sender
        Dim tNo As Integer

        tNo = Get_DimNo_From_ToolbarName("Memo")

        ToolboxInfo(tNo).HoverNo = 0

        memoPicturebox.Invalidate()

    End Sub

    Private Function Get_MemoNo_From_Coordinate(ByVal x As Integer, ByVal y As Integer) As Integer

        Dim x1 As Integer, x2 As Integer
        Dim tNo As Integer

        tNo = Get_DimNo_From_ToolbarName("Memo")

        Get_MemoNo_From_Coordinate = 0

        If y >= ToolboxInfo(tNo).Margin And y <= ToolboxInfo(tNo).Margin + ToolboxInfo(tNo).UnitSize Then
            For i = 1 To GridCount + 1
                x1 = GridStartX + ToolboxInfo(tNo).StartX + (ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin) * (i - 1)
                x2 = x1 + ToolboxInfo(tNo).UnitSize
                If x >= x1 And x <= x2 Then
                    Get_MemoNo_From_Coordinate = i
                    Exit Function
                End If
            Next
        End If

    End Function

    Private Sub CtlPictureBoxPalette_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs)

        Dim p
        Dim p1 As New Pen(Color.Gray, 1)
        Dim sf As New StringFormat()
        sf.LineAlignment = StringAlignment.Center
        sf.Alignment = StringAlignment.Near

        Dim sfp As New StringFormat()
        sfp.LineAlignment = StringAlignment.Center
        sfp.Alignment = StringAlignment.Center

        Dim PaletteBrush As New SolidBrush(Color.Navy)

        Dim i As Integer, j As Integer, n As Integer
        Dim prospectBrush As New SolidBrush(Color.Green)
        Dim x1 As Integer, x2 As Integer
        Dim y1 As Integer, y2 As Integer
        Dim tNo As Integer

        tNo = Get_DimNo_From_ToolbarName("Palette")


        e.Graphics.DrawRectangle(New Pen(Color.DimGray, 1), 0, 0, sender.Width - 1, sender.Height - 1)
        e.Graphics.DrawRectangle(New Pen(Color.Silver, 1), 1, 1, sender.Width - 3, sender.Height - 3)


        e.Graphics.DrawString("Color Palette", MemoFnt, PaletteBrush, _
                      GridStartX + 5, ToolboxInfo(tNo).UnitSize * 0.5 + ToolboxInfo(tNo).Margin, sf)

        For i = 1 To 6 'GridCount
            x1 = GridStartX + (ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin) * (i - 1) + ToolboxInfo(tNo).StartX
            y1 = ToolboxInfo(tNo).Margin '\ 2

            If i = ToolboxInfo(tNo).HoverNo Then
                If ToolboxInfo(tNo).MouseDown = True Then
                    ControlPaint.DrawBorder(e.Graphics, New Rectangle(x1 - (ToolboxInfo(tNo).Margin \ 2), y1 - (ToolboxInfo(tNo).Margin \ 2), ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin, ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin), Color.LightGray, ButtonBorderStyle.Inset)
                Else
                    ControlPaint.DrawBorder(e.Graphics, New Rectangle(x1 - (ToolboxInfo(tNo).Margin \ 2), y1 - (ToolboxInfo(tNo).Margin \ 2), ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin, ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin), Color.LightGray, ButtonBorderStyle.Outset)
                End If

            End If
            e.Graphics.FillRectangle(New SolidBrush(Color.Silver), x1 + 2, y1 + 2, ToolboxInfo(tNo).UnitSize + 1, ToolboxInfo(tNo).UnitSize + 1)
            p = p1
            If CurrentGridX > 0 And CurrentGridX <= GridCount _
               And CurrentGridY > 0 And CurrentGridY <= GridCount Then
                If i <= 4 And SudokuNumberGrid(CurrentGridX, CurrentGridY).BackColor = PaletteColor(i) Then
                    Dim p2 As New Pen(PaletteLineColor(i), 1)
                    p = p2
                End If
            End If
            e.Graphics.DrawRectangle(p, x1, y1, ToolboxInfo(tNo).UnitSize, ToolboxInfo(tNo).UnitSize)
            e.Graphics.FillRectangle(New SolidBrush(PaletteColor(i)), x1 + 1, y1 + 1, ToolboxInfo(tNo).UnitSize - 1, ToolboxInfo(tNo).UnitSize - 1)
        Next

        e.Graphics.DrawString("Del", MemoFnt, New SolidBrush(Color.Red), _
                              GridStartX + ToolboxInfo(tNo).StartX + (6 - 1) * (ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin) + 0.5 * ToolboxInfo(tNo).UnitSize, _
                              ToolboxInfo(tNo).UnitSize * 0.5 + ToolboxInfo(tNo).Margin, sfp)

        Dim imgSheep(4) As Image
        Dim imgRate_w As Double
        Dim imgRate_h As Double

        imgSheep(1) = My.Resources.sheep5
        imgSheep(2) = My.Resources.sheep2
        imgSheep(3) = My.Resources.sheep3
        imgSheep(4) = My.Resources.sheep4

        imgSheep(0) = imgSheep(1)

        If imgSheep(0).Width > imgSheep(0).Height Then
            imgRate_w = 1.0
            imgRate_h = imgSheep(0).Height / imgSheep(0).Width
        Else
            imgRate_h = 1.0
            imgRate_w = imgSheep(0).Width / imgSheep(0).Height
        End If

        Dim mySize As Integer

        mySize = sender.Height

        x1 = GridStartX + (ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin) * (8 - 1) + ToolboxInfo(tNo).StartX
        y1 = 0
        e.Graphics.DrawImage(imgSheep(0), _
                                    New Rectangle(x1 + (1.0 - imgRate_w) * mySize / 2, _
                                                  y1 + (1.0 - imgRate_h) * mySize / 2, _
                                                  mySize * imgRate_w, mySize * imgRate_h), _
                                    0, 0, imgSheep(0).Width, imgSheep(0).Height, GraphicsUnit.Pixel)



    End Sub

    Private Sub CtlPictureBoxPalette_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)

        Dim PalettePicturebox As PictureBox = sender
        Dim x As Integer = palettePicturebox.PointToClient(System.Windows.Forms.Cursor.Position).X
        Dim y As Integer = palettePicturebox.PointToClient(System.Windows.Forms.Cursor.Position).Y
        Dim intRet As Integer
        Dim SelectedPaletteNo As Integer
        Dim i As Integer, j As Integer
        Dim tNo As Integer

        tNo = Get_DimNo_From_ToolbarName("Palette")

        ToolboxInfo(tNo).MouseDown = True

        SelectedPaletteNo = Get_PaletteNo_From_Coordinate(x, y)
        If SelectedPaletteNo = 6 Then
            PalettePicturebox.Invalidate()
            intRet = MsgBox("Whiten All Colored Grids", MsgBoxStyle.YesNo, "Clear Color")
            If intRet = vbYes Then
                For j = 1 To GridCount
                    For i = 1 To GridCount
                        SudokuNumberGrid(i, j).BackColor = Color.White
                    Next
                Next
            End If
            ToolboxInfo(tNo).MouseDown = False

        ElseIf SelectedPaletteNo > 0 Then
            If CurrentGridX > 0 And CurrentGridX <= GridCount _
               And CurrentGridY > 0 And CurrentGridY <= GridCount Then
                If SudokuNumberGrid(CurrentGridX, CurrentGridY).BackColor = PaletteColor(SelectedPaletteNo) Then
                    SudokuNumberGrid(CurrentGridX, CurrentGridY).BackColor = Color.White
                Else
                    SudokuNumberGrid(CurrentGridX, CurrentGridY).BackColor = PaletteColor(SelectedPaletteNo)
                End If
            End If
        End If



        PalettePicturebox.Invalidate()
        Me.PictureBoxGrid.Invalidate()

    End Sub

    Private Sub CtlPictureBoxPalette_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)

        Dim tNo As Integer

        tNo = Get_DimNo_From_ToolbarName("Palette")
        ToolboxInfo(tNo).MouseDown = False

    End Sub


    Private Sub CtlPictureBoxPalette_MouseMove(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim palettePicturebox As PictureBox = sender
        Dim x As Integer = palettePicturebox.PointToClient(System.Windows.Forms.Cursor.Position).X
        Dim y As Integer = palettePicturebox.PointToClient(System.Windows.Forms.Cursor.Position).Y
        Dim tNo As Integer

        tNo = Get_DimNo_From_ToolbarName("Palette")

        ToolboxInfo(tNo).HoverNo = Get_PaletteNo_From_Coordinate(x, y)
        palettePicturebox.Invalidate()


    End Sub


    Private Sub CtlPictureBoxPalette_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim palettePicturebox As PictureBox = sender
        Dim tNo As Integer

        tNo = Get_DimNo_From_ToolbarName("Palette")

        ToolboxInfo(tNo).HoverNo = 0
        palettePicturebox.Invalidate()


    End Sub

    Private Function Get_PaletteNo_From_Coordinate(ByVal x As Integer, ByVal y As Integer) As Integer

        Dim x1 As Integer, x2 As Integer
        Dim tNo As Integer

        tNo = Get_DimNo_From_ToolbarName("Palette")

        Get_PaletteNo_From_Coordinate = 0


        If y >= ToolboxInfo(tNo).Margin \ 2 And y <= ToolboxInfo(tNo).Margin \ 2 + ToolboxInfo(tNo).UnitSize Then
            For i = 1 To 6
                x1 = GridStartX + ToolboxInfo(tNo).StartX + (ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin) * (i - 1)  ' GridCount
                x2 = x1 + ToolboxInfo(tNo).UnitSize
                If x >= x1 And x <= x2 Then
                    Get_PaletteNo_From_Coordinate = i
                    Exit Function
                End If
            Next
        End If



    End Function

    Private Sub CtlPictureBoxHighlight_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs)

        Dim p As New Pen(Color.Gray, 1)

        Dim sf As New StringFormat()
        sf.LineAlignment = StringAlignment.Center
        sf.Alignment = StringAlignment.Near

        Dim sfp As New StringFormat()
        sfp.LineAlignment = StringAlignment.Center
        sfp.Alignment = StringAlignment.Center

        Dim b As Brush
        Dim HighlightBrush As New SolidBrush(Color.Navy)
        Dim HighlightBrushSelected As New SolidBrush(Color.White)
        Dim HighlightColor As Color
        Dim mFnt As Font

        Dim i As Integer, j As Integer, n As Integer
        Dim x1 As Integer, x2 As Integer
        Dim y1 As Integer, y2 As Integer
        Dim tNo As Integer

        tNo = Get_DimNo_From_ToolbarName("Highlight")


        e.Graphics.DrawRectangle(New Pen(Color.DimGray, 1), 0, 0, sender.Width - 1, sender.Height - 1)
        e.Graphics.DrawRectangle(New Pen(Color.Silver, 1), 1, 1, sender.Width - 3, sender.Height - 3)

        e.Graphics.DrawString("Highlight", MemoFnt, HighlightBrush, _
                      GridStartX + 5, ToolboxInfo(tNo).UnitSize * 0.5 + ToolboxInfo(tNo).Margin, sf)

        For i = 1 To GridCount
            x1 = GridStartX + ToolboxInfo(tNo).StartX + (ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin) * (i - 1)
            y1 = ToolboxInfo(tNo).Margin

            If i = ToolboxInfo(tNo).HoverNo Then
                If ToolboxInfo(tNo).MouseDown = True Then
                    ControlPaint.DrawBorder(e.Graphics, New Rectangle(x1 - ToolboxInfo(tNo).Margin \ 2, y1 - ToolboxInfo(tNo).Margin \ 2, ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin, ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin), Color.LightGray, ButtonBorderStyle.Inset)
                Else
                    ControlPaint.DrawBorder(e.Graphics, New Rectangle(x1 - ToolboxInfo(tNo).Margin \ 2, y1 - ToolboxInfo(tNo).Margin \ 2, ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin, ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin), Color.LightGray, ButtonBorderStyle.Outset)
                End If
            End If
            e.Graphics.DrawRectangle(p, x1, y1, ToolboxInfo(tNo).UnitSize, ToolboxInfo(tNo).UnitSize)


            'If ToolboxInfo(tNo).SelectedNo.IndexOf(i) >= 0 Then
            If i = ToolboxInfo(tNo).SelectedNo Then
                If AnalyzeMode = True Then
                    HighlightColor = Color.LightGreen
                Else
                    HighlightColor = Color.Gold
                End If
                e.Graphics.FillRectangle(New SolidBrush(HighlightColor), x1 + 1, y1 + 1, ToolboxInfo(tNo).UnitSize - 1, ToolboxInfo(tNo).UnitSize - 1)
                mFnt = New Font(MemoFnt.FontFamily, MemoFnt.Size, FontStyle.Bold)
            Else
                e.Graphics.FillRectangle(New HatchBrush(HighlightHatch, Color.Gainsboro, Color.White), _
                             x1 + 1, y1 + 1, ToolboxInfo(tNo).UnitSize - 1, ToolboxInfo(tNo).UnitSize - 1)
                mFnt = MemoFnt
            End If
            e.Graphics.DrawString(i, mFnt, HighlightBrush, x1 + ToolboxInfo(tNo).UnitSize * 0.5 + 1, y1 + ToolboxInfo(tNo).UnitSize * 0.5, sfp)


        Next

    End Sub

    Private Sub CtlPictureBoxHighlight_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)

        Dim highlightPicturebox As PictureBox = sender
        Dim x As Integer = highlightPicturebox.PointToClient(System.Windows.Forms.Cursor.Position).X
        Dim y As Integer = highlightPicturebox.PointToClient(System.Windows.Forms.Cursor.Position).Y
        Dim SelectedNo As Integer
        Dim tNo As Integer

        tNo = Get_DimNo_From_ToolbarName("Highlight")

        ToolboxInfo(tNo).MouseDown = True

        SelectedNo = Get_HighlightNo_From_Coordinate(x, y)
        Call Switch_Highlight(SelectedNo)


    End Sub

    Private Sub Switch_Highlight(ByVal SelectedNo As Integer)

        Dim tNo As Integer

        tNo = Get_DimNo_From_ToolbarName("Highlight")

        If SelectedNo > 0 Then
            If ToolboxInfo(tNo).SelectedNo = SelectedNo Then
                ToolboxInfo(tNo).SelectedNo = 0
            Else
                ToolboxInfo(tNo).SelectedNo = SelectedNo
            End If
        End If

        Me.PictureBoxHighlight.Invalidate()
        Me.PictureBoxGrid.Invalidate()

    End Sub


    Private Sub CtlPictureBoxHighlight_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)

        Dim highlightPicturebox As PictureBox = sender
        Dim tNo As Integer

        tNo = Get_DimNo_From_ToolbarName("Highlight")

        ToolboxInfo(tNo).MouseDown = False
        highlightPicturebox.Invalidate()

    End Sub

    Private Sub CtlPictureBoxHighlight_MouseMove(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim highlightPicturebox As PictureBox = sender
        Dim x As Integer = highlightPicturebox.PointToClient(System.Windows.Forms.Cursor.Position).X
        Dim y As Integer = highlightPicturebox.PointToClient(System.Windows.Forms.Cursor.Position).Y
        Dim tNo As Integer

        tNo = Get_DimNo_From_ToolbarName("Highlight")

        ToolboxInfo(tNo).HoverNo = Get_HighlightNo_From_Coordinate(x, y)

        highlightPicturebox.Invalidate()


    End Sub

    Private Sub CtlPictureBoxHighlight_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim highlightPicturebox As PictureBox = sender
        Dim tNo As Integer

        tNo = Get_DimNo_From_ToolbarName("Highlight")

        ToolboxInfo(tNo).HoverNo = 0
        highlightPicturebox.Invalidate()


    End Sub


    Private Function Get_HighlightNo_From_Coordinate(ByVal x As Integer, ByVal y As Integer) As Integer

        Dim x1 As Integer, x2 As Integer
        Dim tNo As Integer

        tNo = Get_DimNo_From_ToolbarName("Highlight")

        Get_HighlightNo_From_Coordinate = 0



        If y >= ToolboxInfo(tNo).Margin And y <= ToolboxInfo(tNo).Margin + ToolboxInfo(tNo).UnitSize Then
            For i = 1 To GridCount
                'x1 = GridStartX + GridCount * GridSize - (ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin) * (GridCount - i + 1) + ToolboxInfo(tNo).Margin
                x1 = GridStartX + ToolboxInfo(tNo).StartX + (ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin) * (i - 1)
                x2 = x1 + ToolboxInfo(tNo).UnitSize
                If x >= x1 And x <= x2 Then
                    Get_HighlightNo_From_Coordinate = i
                    Exit Function
                End If
            Next
        End If




    End Function

    Private Sub Reset_SudokuGrid()

        Dim i As Integer
        Dim j As Integer


        ReDim SudokuNumberGrid(0 To GridCount, 0 To GridCount)

        For i = 0 To GridCount
            For j = 0 To GridCount
                SudokuNumberGrid(j, i) = New SudokuGrid
            Next
        Next
        CompleteFlg = False

        Call Reset_History()
        'Me.Chk_Change.Checked = False
        ChangeFlg = False

    End Sub

    Private Function Get_DimNo_From_ToolbarName(ByVal myToolbarName As String) As Integer

        Dim i As Integer

        Get_DimNo_From_ToolbarName = 0

        For i = 1 To UBound(ToolboxInfo)
            If ToolboxInfo(i).Name = myToolbarName Then
                Get_DimNo_From_ToolbarName = i
                Exit Function
            End If
        Next


    End Function


    Private Sub Set_Grid(ByVal myPicturebox As PictureBox, ByVal memoPicturebox As PictureBox, _
                            ByVal palettePicturebox As PictureBox, ByVal highlightPicturebox As PictureBox)

        Dim MarginX As Integer = 10
        Dim MarginY As Integer = 10
        Dim wd(3) As Integer
        Dim wdd As Integer

        Dim myW As Integer
        Dim myH As Integer
        Dim h, w As Integer
        Dim tNo As Integer

        Me.BackColor = Color.White

        'ディスプレイの高さ
        h = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height
        'ディスプレイの幅
        w = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width

        'CurrentGridX = -1
        'CurrentGridY = -1
        If CurrentGridX <= 0 And CurrentGridY <= 0 Then
            CurrentGridX = GridCount \ 2 + GridCount Mod 2
            CurrentGridY = GridCount \ 2 + GridCount Mod 2
        End If

        GridStartX = 1 ' MarginX
        GridStartY = MarginY

        If AnalyzeMode = True Then
            Me.Panel_ChkBox.Visible = True
            Me.LblLevel.Visible = False
            Me.LblLevel.Tag = ""
            Me.Menu_Level.Enabled = False
        Else
            Me.Panel_ChkBox.Visible = False
            Me.LblLevel.Visible = True
            Me.Menu_Level.Enabled = True
        End If

        wdd = 0
        myH = Me.MenuStrip1.Height + Me.ToolStrip1.Height + 5


        myPicturebox.Size = New Size(GridStartX + GridSize * GridCount + MarginX, _
                     GridStartY + GridSize * GridCount + MarginY \ 2)
        myPicturebox.BackColor = Color.Transparent   'Color.White
        myPicturebox.Top = myH

        myH = myH + myPicturebox.Height

        Me.Panel_ChkBox.Left = myPicturebox.Left
        Me.Panel_ChkBox.Top = myH

        Me.LblLevel.Top = myH
        Me.LblLevel.Left = myPicturebox.Left - MarginX
        Me.LblLevel.Width = myPicturebox.Width
        Me.LblLevel.TextAlign = ContentAlignment.TopRight

        myH = myH + Me.LblLevel.Height

        tNo = Get_DimNo_From_ToolbarName("Memo")
        If tNo > 0 Then
            ToolboxInfo(tNo).Left = myPicturebox.Left
            ToolboxInfo(tNo).UnitSize = GridSize * 0.6
            ToolboxInfo(tNo).StartX = MemoFnt.Size * 7 '+ 20
            ToolboxInfo(tNo).Width = ToolboxInfo(tNo).StartX + (ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin) * (GridCount + 1) + 10
            ToolboxInfo(tNo).Height = ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin * 2
            If wdd > ToolboxInfo(tNo).Width Then
                ToolboxInfo(tNo).Width = wdd
            Else
                wdd = ToolboxInfo(tNo).Width
            End If
            ToolboxInfo(tNo).Top = myH
            If ToolboxInfo(tNo).Visible = True Then
                myH = myH + ToolboxInfo(tNo).Height - 1
            End If
            memoPicturebox.Left = ToolboxInfo(tNo).Left
            memoPicturebox.Top = ToolboxInfo(tNo).Top
            memoPicturebox.Height = ToolboxInfo(tNo).Height
            memoPicturebox.BackColor = Color.Transparent   'Color.White
            memoPicturebox.Visible = ToolboxInfo(tNo).Visible

        End If



        tNo = Get_DimNo_From_ToolbarName("Palette")
        If tNo > 0 Then
            ToolboxInfo(tNo).Left = myPicturebox.Left
            ToolboxInfo(tNo).UnitSize = GridSize * 0.6
            ToolboxInfo(tNo).StartX = MemoFnt.Size * 7 + 20
            ToolboxInfo(tNo).Width = ToolboxInfo(tNo).StartX + (ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin) * 6 + 10
            ToolboxInfo(tNo).Height = ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin * 2
            If wdd > ToolboxInfo(tNo).Width Then
                ToolboxInfo(tNo).Width = wdd
            Else
                wdd = ToolboxInfo(tNo).Width
            End If
            ToolboxInfo(tNo).Top = myH
            If ToolboxInfo(tNo).Visible = True Then
                myH = myH + ToolboxInfo(tNo).Height - 1
            End If

            palettePicturebox.Left = ToolboxInfo(tNo).Left
            palettePicturebox.Top = ToolboxInfo(tNo).Top
            palettePicturebox.Height = ToolboxInfo(tNo).Height
            palettePicturebox.BackColor = Color.Transparent   'Color.White
            palettePicturebox.Visible = ToolboxInfo(tNo).Visible

        End If

        tNo = Get_DimNo_From_ToolbarName("Highlight")
        If tNo > 0 Then
            ToolboxInfo(tNo).Left = myPicturebox.Left
            ToolboxInfo(tNo).UnitSize = GridSize * 0.6
            ToolboxInfo(tNo).StartX = MemoFnt.Size * 7 '+ 20
            ToolboxInfo(tNo).Width = ToolboxInfo(tNo).StartX + (ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin) * GridCount + 10
            ToolboxInfo(tNo).Height = ToolboxInfo(tNo).UnitSize + ToolboxInfo(tNo).Margin * 2
            If wdd > ToolboxInfo(tNo).Width Then
                ToolboxInfo(tNo).Width = wdd
            Else
                wdd = ToolboxInfo(tNo).Width
            End If
            ToolboxInfo(tNo).Top = myH
            If ToolboxInfo(tNo).Visible = True Then
                myH = myH + ToolboxInfo(tNo).Height
            End If
            'ToolboxInfo(tNo).SelectedNo = New List(Of Integer)


            highlightPicturebox.Left = ToolboxInfo(tNo).Left
            highlightPicturebox.Top = ToolboxInfo(tNo).Top
            highlightPicturebox.Height = ToolboxInfo(tNo).Height
            highlightPicturebox.BackColor = Color.Transparent   'Color.White
            highlightPicturebox.Visible = ToolboxInfo(tNo).Visible
        End If
        'HighlightSize = GridSize * 0.7
        'ToolboxInfo(tNo).StartX = MemoFnt.Size * 5 + 10

        HighlightHatch = HatchStyle.Percent50
        'HighlightHatch = HatchStyle.SolidDiamond
        HighlightHatchB = HatchStyle.WideUpwardDiagonal


        palettePicturebox.Width = wdd
        memoPicturebox.Width = wdd
        highlightPicturebox.Width = wdd

        If myPicturebox.Width > wdd Then
            wdd = myPicturebox.Width
        End If


        myW = myPicturebox.Left + wdd + 30
        myH = myH + 50

        If myW > w - 30 Then
            myW = w - 30
        ElseIf myW < 410 Then
            myW = 410
        End If
        If myH > h - 70 Then
            myH = h - 70
        End If

        '        Debug.Print(Me.Menu_Help.Bounds.Left & ":" & Me.Menu_Help.Bounds.Width)

        myPicturebox.Invalidate()
        palettePicturebox.Invalidate()
        memoPicturebox.Invalidate()
        highlightPicturebox.Invalidate()

        Me.Height = myH
        Me.Width = myW


        Me.Left = (w - myW) \ 2
        Me.Top = (h - myH) \ 2

        Call FormNumberKey.Create_NumberKeyButton()


        'Debug.Print("Tool_DisplayAnswer：" & Tool_DisplayAnswer.Bounds.Left + Tool_DisplayAnswer.Width)

    End Sub

    Private Function Get_SudokuGridXY_From_Coordinate(ByVal x As Integer, ByVal y As Integer, _
                                ByRef GridX As Integer, ByRef GridY As Integer) As Boolean

        Dim prevX As Integer, prevY As Integer

        prevX = CurrentGridX
        prevY = CurrentGridY

        GridX = 0
        GridY = 0

        GridX = Int((x - GridStartX) / GridSize) + 1
        GridY = Int((y - GridStartY) / GridSize) + 1

        If GridX > 0 And GridX <= GridCount _
           And GridY > 0 And GridY <= GridCount Then
            Get_SudokuGridXY_From_Coordinate = True
            If prevX <> GridX Or prevY <> GridY Then
                'MsgBox(CurrentGridX & "--" & GridX & "  " & CurrentGridY & "--" & GridY)
                enterHistory(currentHistoryNo).NoB = 0
            End If

        End If


    End Function

    Private Sub Save_NumLogicData(ByVal FilePath As String)

        Dim i As Integer
        Dim j As Integer
        Dim n As Integer
        Dim c As Integer
        Dim strLine As String
        Dim intFileType As Integer = 1
        Dim strTitle As String = ""
        Dim workFlag As Boolean
        Dim memoFlag As Boolean

        On Error GoTo saveError

        workFlag = False
        memoFlag = False

        Dim wfile As New System.IO.StreamWriter(FilePath, False, System.Text.Encoding.GetEncoding(932))

        For j = 1 To GridCount
            strLine = ""
            For i = 1 To GridCount
                If SudokuNumberGrid(i, j).Locked = True Then
                    '                   If SudokuNumberGrid(i, j).Locked = True And SudokuNumberGrid(i, j).ForeColor = Color.Black Then
                    strLine = strLine & SudokuNumberGrid(i, j).FixNo
                Else
                    strLine = strLine & "0"
                    If SudokuNumberGrid(i, j).FixNo > 0 Then
                        workFlag = True
                    End If
                    If SudokuNumberGrid(i, j).MemoNo.Count > 0 Or SudokuNumberGrid(i, j).BackColor <> Color.White Then
                        memoFlag = True
                    End If
                End If
            Next
            wfile.WriteLine(strLine)
        Next

        '解答途中データを保存
        If workFlag = True Then
            wfile.WriteLine("Work")
            For j = 1 To GridCount
                strLine = ""
                For i = 1 To GridCount
                    strLine = strLine & SudokuNumberGrid(i, j).FixNo
                Next
                wfile.WriteLine(strLine)
            Next
        End If

        'メモデータを保存
        If memoFlag = True Then
            wfile.WriteLine("Memo")
            For j = 1 To GridCount
                For i = 1 To GridCount
                    strLine = ""
                    For n = 0 To SudokuNumberGrid(i, j).MemoNo.Count - 1
                        strLine = strLine & "," & SudokuNumberGrid(i, j).MemoNo(n)
                    Next
                    c = Get_ColorNo(SudokuNumberGrid(i, j).BackColor)
                    If strLine.Length > 0 Or c > 0 Then
                        wfile.WriteLine(i & "," & j & "," & c & strLine)
                    End If
                Next

            Next
        End If

        If IsNumeric(Me.LblLevel.Tag) = True Then
            wfile.WriteLine("Level:" & Me.LblLevel.Tag)
        End If


        wfile.Close()
        'Me.Chk_Change.Checked = False
        ChangeFlg = False


        Exit Sub

saveError:
        MsgBox(Err.Number & ":" & Err.Description)

    End Sub

    Private Sub Load_NumLogicData(ByVal FilePath As String)

        Dim i As Integer, j As Integer, l As Integer
        Dim x As Integer, y As Integer
        Dim enc As System.Text.Encoding = System.Text.Encoding.GetEncoding(932)
        Dim readflag As Boolean
        Dim workflag As Boolean
        Dim memoflag As Boolean
        Dim modeGame As Boolean
        Dim myLevel As Integer
        Dim txtLineArray() As String
        Dim strAsterisk As String = ""



        readflag = True
        workflag = False
        memoflag = False
        modeGame = False
        myLevel = 0

        'Me.Chk_Change.Checked = False
        ChangeFlg = False
        Reset_SudokuGrid()

        Dim txtLines As String() = System.IO.File.ReadAllLines(FilePath, enc)

        For j = 0 To txtLines.Length - 1
            If txtLines(j) Like "Level:*" Then
                If IsNumeric(txtLines(j).Substring(6, 1)) = True Then
                    myLevel = CInt(txtLines(j).Substring(6, 1))
                End If
            End If

            If readflag = False Then
                If txtLines(j) = "Work" Or txtLines(j) = "Memo" Then
                    '                    Debug.Print("txtLines(j)" & txtLines(j))
                    readflag = True
                    workflag = False
                    memoflag = False
                    y = 0
                    If txtLines(j) = "Work" Then
                        workflag = True
                        modeGame = True
                    ElseIf txtLines(j) = "Memo" Then
                        memoflag = True
                        modeGame = True
                    End If
                End If
            Else
                If memoflag = False Then
                    y = y + 1
                    For i = 0 To txtLines(j).Length - 1
                        x = i + 1
                        '                    y = j + 1
                        If x > 0 And x <= GridCount And y > 0 And y <= GridCount Then
                            If IsNumeric(txtLines(j).Substring(i, 1)) = True Then
                                SudokuNumberGrid(x, y).FixNo = CInt(txtLines(j).Substring(i, 1))
                                If SudokuNumberGrid(x, y).FixNo > 0 Then
                                    If workflag = False Then
                                        SudokuNumberGrid(x, y).Locked = True
                                    End If
                                End If
                            End If
                        End If
                    Next
                    If y >= GridCount Then
                        readflag = False
                    End If
                Else
                    txtLineArray = txtLines(j).Split(",")
                    If txtLineArray.Length >= 2 Then
                        If IsNumeric(txtLineArray(0)) = True And txtLineArray(1) = True Then
                            x = CInt(txtLineArray(0))
                            y = CInt(txtLineArray(1))
                            If x > 0 And x <= GridCount And y > 0 And y <= GridCount Then
                                If IsNumeric(txtLineArray(2)) = True Then
                                    SudokuNumberGrid(x, y).BackColor = Get_Color_From_No(txtLineArray(2))
                                End If
                                For i = 3 To txtLineArray.Length - 1
                                    If IsNumeric(txtLineArray(i)) = True Then
                                        If txtLineArray(i) > 0 And txtLineArray(i) <= GridCount Then
                                            SudokuNumberGrid(x, y).MemoNo.Add(CInt(txtLineArray(i)))
                                        End If
                                    End If
                                Next
                            End If
                        End If
                    End If
                End If
            End If
        Next

        Me.LblLevel.Text = ""
        Me.LblLevel.Tag = ""

        If modeGame = True Then
            AnalyzeMode = False
            Call Change_Mode(False)
            If myLevel > 0 Then
                Me.LblLevel.Text = "Level " & myLevel 'strAsterisk.PadLeft(myLevel, "★")
                Me.LblLevel.Tag = myLevel
            End If
        End If

        Call Adjust_ProspectNo(SudokuNumberGrid)
        Call Reset_History()

        ToolboxInfo(Get_DimNo_From_ToolbarName("Highlight")).SelectedNo = 0

        Me.PictureBoxGrid.Invalidate()
        Me.PictureBoxHighlight.Invalidate()
        Me.PictureBoxMemo.Invalidate()

    End Sub

    Private Sub Load_NumLogicStock(ByVal myLv As Integer, Optional ByVal LoadNo As Integer = 1)

        Dim i As Integer, j As Integer, l As Integer
        Dim x As Integer, y As Integer
        Dim enc As System.Text.Encoding = System.Text.Encoding.GetEncoding(932)
        Dim readflag As Boolean
        Dim strAsterisk As String = ""

        readflag = False

        'Me.Chk_Change.Checked = False
        ChangeFlg = False
        Reset_SudokuGrid()

        Dim txtAll As String = My.Resources.ResourceManager.GetObject("NumLgLv" & myLv)

        Dim txtLines As String() = Split(txtAll, vbCrLf)

        For j = 0 To txtLines.Length - 1
            If readflag = False Then
                If txtLines(j) Like "*No." & Format(LoadNo, "0000") & "*" Then
                    'Debug.Print("txtLines(j)" & txtLines(j))
                    readflag = True
                    y = 0
                End If
            Else
                y = y + 1
                For i = 0 To txtLines(j).Length - 1
                    x = i + 1
                    '                    y = j + 1
                    If x > 0 And x <= GridCount And y > 0 And y <= GridCount Then
                        If IsNumeric(txtLines(j).Substring(i, 1)) = True Then
                            SudokuNumberGrid(x, y).FixNo = CInt(txtLines(j).Substring(i, 1))
                            If SudokuNumberGrid(x, y).FixNo > 0 Then
                                SudokuNumberGrid(x, y).Locked = True
                            End If
                        End If
                    End If
                Next
                If y >= GridCount Then
                    readflag = False
                End If
            End If
        Next

        If LoadNo > 0 Then
            Call Replace_GridPosition(SudokuNumberGrid)
        End If

        Me.LblLevel.Text = ""
        Me.LblLevel.Tag = ""

        Call Adjust_ProspectNo(SudokuNumberGrid)

        Call Reset_History()
        Me.PictureBoxGrid.Invalidate()

        'Debug.Print("Lv:" & myLv & "  LoadNo:" & LoadNo)

    End Sub

    Private Sub Create_NewQuestion(ByVal assignCnt As Integer, ByRef FixCnt As Integer)
        '                                                    ,  Optional ByVal Flg_BackTrack As Boolean = False)

        Dim myRnd As Integer
        Dim AssignedNo() As Integer
        Dim ProspectNoBalance As New List(Of Integer)
        Dim x As Integer
        Dim y As Integer
        Dim x2 As Integer
        Dim y2 As Integer
        Dim pre_x2 As Integer
        Dim pre_y2 As Integer
        Dim i As Integer, j As Integer
        Dim n As Integer, s As Integer, ss As Integer
        Dim answerCnt As Integer
        Dim errCnt As Integer
        Dim backFlag As Boolean
        Dim fixedCnt() As Integer
        Dim blankGrid As Integer
        Dim ngFlag As Boolean
        Dim myLevel As Integer
        Dim myCoordinate As New Coordinate
        Dim myRndProspectNo As New List(Of Integer)
        Dim answerNumberGrid(,,) As SudokuGrid
        Dim myTmpNumberGrid(,) As SudokuGrid
        Dim myNextGrid() As Coordinate

        n = 0
FS:
        errCnt = 0
        backFlag = False

        n = n + 1
        'Debug.Print("n=" & n)
        If n > 100 Then
            Exit Sub
        End If

        ReDim fixedCnt(0 To GridCount)
        For i = 1 To GridCount
            fixedCnt(i) = 0
        Next

        For j = 1 To GridCount
            For i = 1 To GridCount
                SudokuNumberGrid(i, j) = New SudokuGrid
            Next
        Next

        ReDim AssignedNo(0)
        ReDim myTmpNumberGrid(GridCount, GridCount)


        'p = 0

F_Loop:
        'If errCnt > 100 Then
        If errCnt > 100 Or UBound(AssignedNo) >= assignCnt Then
            'MsgBox("errCnt > 100")
            GoTo FS
        End If

        If backFlag = False Then
            If UBound(AssignedNo) > 0 Then
                '直近に割り当てたマスと点対称の位置にあるマスの座標
                pre_x2 = ((GridCount * GridCount - AssignedNo(UBound(AssignedNo)) + 1) - 1) Mod GridCount + 1
                pre_y2 = ((GridCount * GridCount - AssignedNo(UBound(AssignedNo)) + 1) - 1) \ GridCount + 1
            Else
                pre_x2 = 0
                pre_y2 = 0
            End If
            'バックトラック法を適用
            If UBound(AssignedNo) >= assignCnt - 2 And SudokuNumberGrid(pre_x2, pre_y2).FixNo > 0 Then 'And Flg_BackTrack = True Then
                For j = 1 To GridCount
                    For i = 1 To GridCount
                        myTmpNumberGrid(i, j) = New SudokuGrid
                        myTmpNumberGrid(i, j).Copy(SudokuNumberGrid(i, j))
                    Next
                Next
                ss = Solve_SudokuBackTrack(New Coordinate, myTmpNumberGrid, answerNumberGrid, 1, 50)
                If ss > 0 Then
                    Call Get_MinimumAnserPair(answerNumberGrid, SudokuNumberGrid, myNextGrid, answerCnt)
                    If UBound(myNextGrid) = 2 Then
                        For i = 1 To 2
                            SudokuNumberGrid(myNextGrid(i).X, myNextGrid(i).Y).FixNo = myNextGrid(i).No
                            'SudokuNumberGrid(myNextGrid(i).X, myNextGrid(i).Y).ForeColor = Color.Orange
                            SudokuNumberGrid(myNextGrid(i).X, myNextGrid(i).Y).Locked = True
                            ReDim Preserve AssignedNo(UBound(AssignedNo) + 1)
                            AssignedNo(UBound(AssignedNo)) = (myNextGrid(i).Y - 1) * GridCount + myNextGrid(i).X
                            fixedCnt(myNextGrid(i).No) = fixedCnt(myNextGrid(i).No) + 1
                        Next
                        'Debug.Print("【選択】x=" & myNextGrid(1).X & " y=" & myNextGrid(1).Y & " Answer=" & myNextGrid(1).No _
                        '        & "   " & "x2=" & myNextGrid(2).X & " y2=" & myNextGrid(2).Y & " Answer=" & myNextGrid(2).No & " Count:" & answerCnt)
                        blankGrid = Solve_Sudoku(3, ngFlag, SudokuNumberGrid, myLevel)
                        If blankGrid = 0 Then
                            GoTo F_Fix
                        Else
                            GoTo F_Loop
                        End If
                    Else
                        GoTo FS
                    End If
                Else
                    GoTo FS
                End If
            ElseIf Assign_NextTaget(AssignedNo, SudokuNumberGrid) = False Then
                GoTo FS
            End If

        End If


        'Debug.Print("p=" & UBound(AssignedNo) & "/" & assignCnt)


        x = (AssignedNo(UBound(AssignedNo)) - 1) Mod GridCount + 1
        y = (AssignedNo(UBound(AssignedNo)) - 1) \ GridCount + 1
        x2 = GridCount - x + 1
        y2 = GridCount - y + 1

        backFlag = False


F_Loop2:

        ''割り当てる数値が偏りすぎないよう調整
        'ProspectNoBalance.Clear()

        'For i = 0 To SudokuNumberGrid(x, y).ProspectNo.Count - 1

        '    For j = 1 To GridCount - fixedCnt(SudokuNumberGrid(x, y).ProspectNo(i))
        '        ProspectNoBalance.Add(SudokuNumberGrid(x, y).ProspectNo(i))
        '    Next
        'Next
        'myRnd = Generate_Random(ProspectNoBalance, SudokuNumberGrid(x, y).ExcludeNo)

        myRnd = Select_FitProspectNo(New Coordinate(x, y, 0), SudokuNumberGrid)


        SudokuNumberGrid(x, y).FixNo = myRnd

        If myRnd > 0 Then
            SudokuNumberGrid(x, y).Locked = True
            'この時点で、問題として成り立つか（＝最後まで解けるか）をチェック
            blankGrid = Solve_Sudoku(3, ngFlag, SudokuNumberGrid, myLevel)
            'Debug.Print("x=" & x & " y=" & y & " No." & myRnd & "   blankGrid：" & blankGrid & " " & ngFlag)

            If ngFlag = True Then
                '破綻がある場合は、番号を変更してやり直し
                SudokuNumberGrid(x, y).ExcludeNo.Add(myRnd)
                SudokuNumberGrid(x, y).Locked = False
                SudokuNumberGrid(x, y).FixNo = 0
                Call Adjust_ProspectNo(SudokuNumberGrid)
                GoTo F_Loop2
            Else
                fixedCnt(SudokuNumberGrid(x, y).FixNo) = fixedCnt(SudokuNumberGrid(x, y).FixNo) + 1
                If blankGrid > 0 Then
                    '破綻はないがまだ未完成の場合は次のマスの割り当てへ
                    GoTo F_Loop
                Else
                    '問題として成り立つ場合
                    '点対称にするための調整
                    If SudokuNumberGrid(x2, y2).FixNo = 0 Then
                        SudokuNumberGrid(x2, y2).FixNo = SudokuNumberGrid(x2, y2).ProspectNo(0)
                        'SudokuNumberGrid(x2, y2).ForeColor = Color.DeepPink
                        SudokuNumberGrid(x2, y2).Locked = True
                        fixedCnt(SudokuNumberGrid(x2, y2).FixNo) = fixedCnt(SudokuNumberGrid(x2, y2).FixNo) + 1
                        ReDim Preserve AssignedNo(UBound(AssignedNo) + 1)
                        AssignedNo(UBound(AssignedNo)) = (y2 - 1) * GridCount + x2
                    End If
                End If
            End If

        Else
            '該当マスに割り当てられる数値がなくなってしまった場合は、１つ前のマスに戻ってやり直し
            SudokuNumberGrid(x, y).ExcludeNo.Clear()
            '1つ前に割り当てた座標
            x = (AssignedNo(UBound(AssignedNo) - 1) - 1) Mod GridCount + 1
            y = (AssignedNo(UBound(AssignedNo) - 1) - 1) \ GridCount + 1
            fixedCnt(SudokuNumberGrid(x, y).FixNo) = fixedCnt(SudokuNumberGrid(x, y).FixNo) - 1
            SudokuNumberGrid(x, y).ExcludeNo.Add(SudokuNumberGrid(x, y).FixNo)
            SudokuNumberGrid(x, y).FixNo = 0
            'SudokuNumberGrid(x, y).Locked = False

            Call Adjust_ProspectNo(SudokuNumberGrid)
            ReDim Preserve AssignedNo(UBound(AssignedNo) - 1)
            If UBound(AssignedNo) = 0 Then
                GoTo FS
            End If
            errCnt = errCnt + 1
            backFlag = True
            GoTo F_Loop
        End If

F_Fix:
        'n = 0
        'For i = 1 To GridCount
        '    'Debug.Print(i & ">>>" & fixedCnt(i))
        '    n = n + fixedCnt(i)
        'Next
        FixCnt = 0
        For j = 1 To GridCount
            For i = 1 To GridCount
                If SudokuNumberGrid(i, j).FixNo > 0 Then
                    FixCnt = FixCnt + 1
                Else
                    SudokuNumberGrid(i, j).Locked = False
                End If
            Next
        Next



        '割り当てマスが少ない場合は足す（初級者向け）
        Do While FixCnt < assignCnt
            If Assign_NextTaget(AssignedNo, SudokuNumberGrid) = True Then
                x = (AssignedNo(UBound(AssignedNo)) - 1) Mod GridCount + 1
                y = (AssignedNo(UBound(AssignedNo)) - 1) \ GridCount + 1
                SudokuNumberGrid(x, y).FixNo = SudokuNumberGrid(x, y).ProspectNo(0)
                SudokuNumberGrid(x, y).Locked = True
                'SudokuNumberGrid(x, y).ForeColor = Color.DeepPink
                FixCnt = FixCnt + 1
                '点対称位置のマスにも配置
                x2 = GridCount - x + 1
                y2 = GridCount - y + 1
                If SudokuNumberGrid(x2, y2).FixNo = 0 Then
                    SudokuNumberGrid(x2, y2).FixNo = SudokuNumberGrid(x2, y2).ProspectNo(0)
                    SudokuNumberGrid(x2, y2).Locked = True
                    'SudokuNumberGrid(x2, y2).ForeColor = Color.DeepPink
                    FixCnt = FixCnt + 1
                    ReDim Preserve AssignedNo(UBound(AssignedNo) + 1)
                    AssignedNo(UBound(AssignedNo)) = (y2 - 1) * GridCount + x2
                End If

            End If
        Loop

        Call Adjust_ProspectNo(SudokuNumberGrid)
        Me.PictureBoxGrid.Invalidate()

        'Debug.Print("sum>>>" & FixCnt & "   Level=" & myLevel)

    End Sub

    '
    '  指定のマスに入りうる数値のうち、その番号を入れることによって他のマスの候補Noが最も減るものを選択
    '
    Private Function Select_FitProspectNo(ByVal myCoordinate As Coordinate, ByVal tmpSudokuNumberGrid(,) As SudokuGrid) As Integer

        Dim cntProspect As Integer
        Dim myProspect As New List(Of Coordinate)
        Dim i As Integer, j As Integer
        Dim sNo As Integer
        Dim maxCnt As Integer = 0
        Dim FitNo As New List(Of Integer)

        Select_FitProspectNo = 0

        myCoordinate.S = Get_SquareNo(myCoordinate.X, myCoordinate.Y, sNo)

        For i = 0 To tmpSudokuNumberGrid(myCoordinate.X, myCoordinate.Y).ProspectNo.Count - 1
            myCoordinate.No = tmpSudokuNumberGrid(myCoordinate.X, myCoordinate.Y).ProspectNo(i)
            cntProspect = Count_ProspectNo_On_Group(myCoordinate, myProspect, tmpSudokuNumberGrid)

            If cntProspect >= maxCnt Then
                If cntProspect > maxCnt Then
                    FitNo.Clear()
                    maxCnt = cntProspect
                End If
                FitNo.Add(myCoordinate.No)
            End If
        Next

        Select_FitProspectNo = Generate_Random(FitNo, tmpSudokuNumberGrid(myCoordinate.X, myCoordinate.Y).ExcludeNo)


    End Function

    Private Function Solve_Sudoku(ByVal SolveMode As Integer, ByRef NGFlag As Boolean, _
                                     ByRef myNumberGrid(,) As SudokuGrid, _
                                     Optional ByRef myLevel As Integer = 0, _
                                     Optional ByRef NextHint As Coordinate = Nothing) As Integer
        Dim i As Integer, j As Integer, n As Integer
        Dim p As Integer, sNo As Integer
        Dim errCnt As Integer
        Dim boolChange As Boolean
        Dim tmpNumberGrid(,) As SudokuGrid

        '関数の戻り値　全てのマスに埋める数値が決まる場合=0、空きがある場合（作りかけ）=>埋まらないマスの数  
        '引数等
        '　SolveMode=1：全解答モード
        '                現在の入力状態に間違いがない場合、空きマスを解答で埋める
        '　　　　　　　　間違いがある場合、間違っているマスのエラーフラグ（FixErr）をオンにする　　　
        '　SolveMode=2：ヒントモード
        '　　　　　　　　現在の入力状態に間違いがない場合、引数（NextHint）に次に埋めるマス・数字をヒントとして返す
        '　　　　　　　　間違いがある場合、間違っているマスのエラーフラグ（FixErr）をオンにする　　　
        '　SolveMode=3：問題作成時モード
        '　　　　　　　  現在の入力状態に間違いがないか、全てのマスに埋める数値が決まるかのチェックのみ()

        '　NGFlag：数値の確定マスに破綻がある場合：False　ない場合：True
        NGFlag = False
F_Start:
        'ローカル変数tmpSudokuNumberGridに参照元のmyNumberGridの情報をコピー
        ReDim tmpNumberGrid(GridCount, GridCount)
        For j = 1 To GridCount
            For i = 1 To GridCount
                tmpNumberGrid(i, j) = New SudokuGrid
                '全解答モードでは数値固定マス以外の情報はクリア（間違い回答の可能性があるため）
                If myNumberGrid(i, j).Locked = True Or SolveMode > 2 Then
                    tmpNumberGrid(i, j).Copy(myNumberGrid(i, j))
                End If
            Next
        Next

        UsedTechnique = New SolvingTechnique
        myLevel = 0

        If SolveMode = 1 Then
            Adjust_ProspectNo(tmpNumberGrid)
        End If

        NextHint = New Coordinate

        If DuplicateNumber(tmpNumberGrid, New Coordinate) = True Then
            NGFlag = True
        End If

        p = 0
F_Loop:

        p = p + 1
        boolChange = False
        For j = 1 To GridCount
            For i = 1 To GridCount
                If tmpNumberGrid(i, j).FixNo = 0 Then
                    '候補Noが１つしかない→数値確定
                    If tmpNumberGrid(i, j).ProspectNo.Count = 1 Then
                        If myNumberGrid(i, j).FixNo = 0 And NextHint.No = 0 Then
                            NextHint.X = i
                            NextHint.Y = j
                            NextHint.No = tmpNumberGrid(i, j).ProspectNo(0)
                        End If
                        tmpNumberGrid(i, j).FixNo = tmpNumberGrid(i, j).ProspectNo(0)
                        Call Remove_ProspectNo(1, New Coordinate(i, j, 0, tmpNumberGrid(i, j).FixNo), tmpNumberGrid)
                        boolChange = True
                    End If
                Else
                    '既に数値確定済みのマスがある場合、その縦・横・エリアのいずれかが同じマスの該当番号を候補より除外
                    If Remove_ProspectNo(1, New Coordinate(i, j, 0, tmpNumberGrid(i, j).FixNo), tmpNumberGrid) = True Then
                        boolChange = True
                        If SolveMode = 2 Then
                            GoTo F_Loop
                        End If
                    End If
                End If
            Next
        Next

        If boolChange = True Then
            GoTo F_Loop
        End If

        For j = 1 To GridCount
            For i = 1 To GridCount
                If tmpNumberGrid(i, j).FixNo = 0 Then
                    For n = 0 To tmpNumberGrid(i, j).ProspectNo.Count - 1
                        '該当番号以外入り得ないケース（縦・横・スクエアに該当番号以外の全ての数値が存在）
                        If Check_ProspectNo_Solo(New Coordinate(i, j, Get_SquareNo(i, j, sNo), tmpNumberGrid(i, j).ProspectNo(n)), tmpNumberGrid) = True Then
                            If myNumberGrid(i, j).FixNo = 0 And NextHint.No = 0 Then
                                NextHint.X = i
                                NextHint.Y = j
                                NextHint.No = tmpNumberGrid(i, j).ProspectNo(n)
                                'Debug.Print(i & "," & j & "  -------------- " & tmpNumberGrid(i, j).ProspectNo(0))
                            End If
                            tmpNumberGrid(i, j).FixNo = tmpNumberGrid(i, j).ProspectNo(n)
                            Call Remove_ProspectNo(1, New Coordinate(i, j, 0, tmpNumberGrid(i, j).FixNo), tmpNumberGrid)
                            boolChange = True
                            Exit For
                        End If
                    Next
                End If
            Next
        Next

        If boolChange = True Then
            GoTo F_Loop
        End If

        If SolveMode = 4 Or SolveMode = 3 Then
            GoTo Skip_DetailCheck
        End If

        For s = 1 To GridCount
            If Check_ProspectNo_LimitLine(s, tmpNumberGrid) Then
                If myLevel < 1 Then
                    myLevel = 1
                End If
                'Debug.Print("Check_ProspectNo_LimitLine")
                boolChange = True
                GoTo F_Loop
            End If
        Next


        ' 指定した列（横列）において、番号ｎが入る候補マスが、特定スクエアに限定されるかをチェック
        For j = 1 To GridCount
            If Check_ProspectNo_LimitSquare(0, j, tmpNumberGrid) = True Then
                'Debug.Print("Check_ProspectNo_LimitSquareY")
                If myLevel < 1 Then
                    myLevel = 1
                End If
                boolChange = True
                GoTo F_Loop
            End If
        Next

        ' 指定した列（縦列）において、番号ｎが入る候補マスが、特定スクエアに限定されるかをチェック
        For i = 1 To GridCount
            If Check_ProspectNo_LimitSquare(i, 0, tmpNumberGrid) = True Then
                'Debug.Print("Check_ProspectNo_LimitSquareX")
                If myLevel < 1 Then
                    myLevel = 1
                End If
                boolChange = True
                GoTo F_Loop
            End If
        Next

        If boolChange = True Then
            GoTo F_Loop
        End If

        If SolveMode = 5 Then
            GoTo Skip_DetailCheck
        End If

        If Check_ProspectNo_NakedPairTriple(tmpNumberGrid) = True Then
            'Debug.Print("Check_ProspectNo_NakedPairTriple")
            UsedTechnique.NakedPairTriple = True
            If myLevel < 2 Then
                myLevel = 2
            End If
            boolChange = True
            GoTo F_Loop
        End If

        If SolveMode = 6 Then
            GoTo Skip_DetailCheck
        End If

        If Check_ProspectNo_HiddenPairTriple(tmpNumberGrid) = True Then
            UsedTechnique.HiddenPairTriple = True
            If myLevel < 3 Then
                myLevel = 3
            End If
            boolChange = True
            GoTo F_Loop
        End If

        If Check_ProspectNo_SimpleColors(tmpNumberGrid) = True Then
            UsedTechnique.SimpleColors = True
            If myLevel < 3 Then
                myLevel = 3
            End If
            boolChange = True
            GoTo F_Loop
        End If

        If Check_ProspectNo_SwordFish(tmpNumberGrid, 2) = True Then
            Me.PictureBoxGrid.Invalidate()
            UsedTechnique.XWing = True
            If myLevel < 3 Then
                myLevel = 3
            End If
            boolChange = True
            GoTo F_Loop
        End If

        If Check_ProspectNo_XYWing(tmpNumberGrid) = True Then
            UsedTechnique.XYWing = True
            If myLevel < 3 Then
                myLevel = 3
            End If
            boolChange = True
            GoTo F_Loop
        End If

        If Check_ProspectNo_SwordFish(tmpNumberGrid) = True Then
            UsedTechnique.SwordFish = True
            If myLevel < 4 Then
                myLevel = 4
            End If
            boolChange = True
            GoTo F_Loop
        End If

        If Check_ProspectNo_MultiColors(tmpNumberGrid) = True Then
            UsedTechnique.MultiColors = True
            If myLevel < 4 Then
                myLevel = 4
            End If
            boolChange = True
            GoTo F_Loop
        End If

Skip_DetailCheck:

        Solve_Sudoku = 0
        errCnt = 0
        For j = 1 To GridCount
            For i = 1 To GridCount
                If tmpNumberGrid(i, j).FixNo = 0 Then
                    Solve_Sudoku = Solve_Sudoku + 1
                    If tmpNumberGrid(i, j).ProspectNo.Count = 0 Then  '候補がなくなってしまった状態（破綻）
                        NGFlag = True
                    End If
                Else
                    If SolveMode <= 2 Then
                        If tmpNumberGrid(i, j).FixNo > 0 And myNumberGrid(i, j).FixNo > 0 _
                               And tmpNumberGrid(i, j).FixNo <> myNumberGrid(i, j).FixNo Then
                            myNumberGrid(i, j).FixError = True
                            Solve_Sudoku = Solve_Sudoku + 1
                            NGFlag = True
                            'MsgBox(i & "," & j)
                        End If
                    End If
                End If
            Next
        Next

        If SolveMode = 1 Then
            If NGFlag = False Then 'Solve_SudokuNew = 0 Then
                For j = 1 To GridCount
                    For i = 1 To GridCount
                        myNumberGrid(i, j).Copy(tmpNumberGrid(i, j))
                    Next
                Next
            End If
        ElseIf SolveMode = 2 Then
            'If NGFlag = True Then
            '    SolveMode = 1
            '    GoTo F_Start
            'End If

        ElseIf SolveMode >= 4 Then
            '            If NGFlag = False Then 'Solve_SudokuNew = 0 Then
            For j = 1 To GridCount
                For i = 1 To GridCount
                    myNumberGrid(i, j).Copy(tmpNumberGrid(i, j))
                Next
            Next
            '        End If
        ElseIf SolveMode = 3 Then
            '問題作成モード時は候補No情報のみを戻す
            If NGFlag = False Then
                For j = 1 To GridCount
                    For i = 1 To GridCount
                        myNumberGrid(i, j).ProspectNo.Clear()
                        myNumberGrid(i, j).ProspectNo.AddRange(tmpNumberGrid(i, j).ProspectNo)
                    Next
                Next
            End If
        End If

    End Function

    Private Function Solve_SudokuBackTrack(ByVal myGrid As Coordinate, _
                                   ByVal myNumberGrid(,) As SudokuGrid, _
                                   ByRef answerNumberGrid(,,) As SudokuGrid, _
                                   Optional ByVal nestLev As Integer = 1, _
                                   Optional ByRef multianswerCnt As Integer = 0 _
                                   ) As Integer

        Dim i As Integer, j As Integer, n As Integer
        Dim myNo As Integer
        Dim tmpNumberGrid(,) As SudokuGrid
        Dim NextTarget = New Coordinate
        Dim myNG As Boolean

        Solve_SudokuBackTrack = 0

        'If nestLevMax < nestLev Then
        '    nestLevMax = nestLev
        'End If

        'ローカル変数初期化処理
        ReDim tmpNumberGrid(GridCount, GridCount)

        '最初に仮定で数値割当するマスを決定
        If myGrid.X = 0 Or myGrid.Y = 0 Then
            myGrid = Select_NextTaget_For_BackTrack(myNumberGrid)
            '            ReDim answerNumberGrid(0)
            ReDim answerNumberGrid(GridCount, GridCount, 0) ' = New List(Of SudokuGrid(,))
        Else
            'Debug.Print("UBound(answerNumberGrid, 3):" & UBound(answerNumberGrid, 3))
        End If

        If (multianswerCnt = 0 And UBound(answerNumberGrid, 3) > 0) Or multianswerCnt = -1 Then
            Exit Function
        End If

        '対象マスの候補Noがなくなってしまったら終了
        Do While myNumberGrid(myGrid.X, myGrid.Y).ProspectNo.Count > 0
            'Debug.Print("候補数" & myNumberGrid(myGrid.X, myGrid.Y).ProspectNo.Count)
            myNo = myNumberGrid(myGrid.X, myGrid.Y).ProspectNo(0)
            '仮定で数値割当
            myNumberGrid(myGrid.X, myGrid.Y).FixNo = myNo
            'Debug.Print("　　　x=" & myGrid.X & "  y=" & myGrid.Y & "  仮定No." & myNo)
            '一般解法にて解析
            For j = 1 To GridCount
                For i = 1 To GridCount
                    tmpNumberGrid(i, j) = New SudokuGrid
                    tmpNumberGrid(i, j).Copy(myNumberGrid(i, j))
                Next
            Next
            If Solve_Sudoku(4, myNG, tmpNumberGrid) = 0 Then
                '解が見つかった
                Solve_SudokuBackTrack = Solve_SudokuBackTrack + 1
                ReDim Preserve answerNumberGrid(GridCount, GridCount, UBound(answerNumberGrid, 3) + 1)
                'Debug.Print("answerNumberGrid:" & UBound(answerNumberGrid, 3))
                For j = 1 To GridCount
                    For i = 1 To GridCount
                        answerNumberGrid(i, j, UBound(answerNumberGrid, 3)) = New SudokuGrid
                        answerNumberGrid(i, j, UBound(answerNumberGrid, 3)).Copy(tmpNumberGrid(i, j))
                    Next
                Next

                If UBound(answerNumberGrid, 3) > multianswerCnt Then
                    multianswerCnt = -1
                End If
            Else '解が見つからない
                If myNG = True Then '破綻してしまっている（＝仮定で割り当てた番号が間違っている） 
                    'Debug.Print("破綻   x=" & myGrid.X & "  y=" & myGrid.X & "  NO=" & myNo & "   " & multianswerCnt)
                Else        '破綻はしていない（まだ、数値が埋まらない）→更なる仮定が必要
                    NextTarget = Select_NextTaget_For_BackTrack(tmpNumberGrid)
                    '                    Debug.Print("NextTarget.X=" & NextTarget.X & "  y=" & NextTarget.Y & "  ProspectNo.Count=" & myNumberGrid(NextTarget.X, NextTarget.Y).ProspectNo.Count)
                    If NextTarget.X = 0 Or NextTarget.Y = 0 Then  '仮定割当のマスがなくなってしまったら、同条件での仮定終了
                        'myNumberGrid(myGrid.X, myGrid.Y).ProspectNo.Remove(myNo)
                    Else
                        '再帰
                        Solve_SudokuBackTrack = Solve_SudokuBackTrack + Solve_SudokuBackTrack(NextTarget, tmpNumberGrid, answerNumberGrid, nestLev + 1, multianswerCnt)
                    End If
                End If
            End If
            If multianswerCnt = -1 Then
                Exit Do
            End If

            myNumberGrid(myGrid.X, myGrid.Y).FixNo = 0
            myNumberGrid(myGrid.X, myGrid.Y).ProspectNo.Remove(myNo)
        Loop


    End Function

    '
    '  バックトラック法での解答探索時に、次に対象とするマス及び割当Noを選択する
    '　　　※現時点で、候補No数が最も少ないマスを選択（候補数が同じ場合はY座標、X座標でソートし）
    '　　　　割当Noは、対象マスの候補Noの中で最小のもの　　　

    Private Function Select_NextTaget_For_BackTrack(ByVal myNumberGrid(,) As SudokuGrid) As Coordinate

        Dim i As Integer, j As Integer, n As Integer
        Dim x As Integer, y As Integer
        Dim p As Integer

        Select_NextTaget_For_BackTrack = New Coordinate

        p = 1 '候補No数の初期値
        Do While p <= GridCount
            For y = 1 To GridCount
                For x = 1 To GridCount
                    If myNumberGrid(x, y).FixNo = 0 Then
                        If myNumberGrid(x, y).ProspectNo.Count = p Then
                            Select_NextTaget_For_BackTrack.X = x
                            Select_NextTaget_For_BackTrack.Y = y
                            '                            Select_NextTaget_For_BackTrack.No = myNumberGrid(x, y).ProspectNo(0)
                            Exit Function
                        End If
                    End If
                Next
            Next
            p = p + 1
        Loop


    End Function

    '
    '  指定した２つのマスが同じグループ（横・縦列、スクエア）に属しているかをチェック
    '
    Private Function IsSameGroup(ByVal myCoordinateA As Coordinate, ByVal myCoordinateB As Coordinate) As Boolean

        Dim sNo As Integer

        IsSameGroup = False

        If myCoordinateA.X = myCoordinateB.X Or myCoordinateA.Y = myCoordinateB.Y _
           Or Get_SquareNo(myCoordinateA.X, myCoordinateA.Y, sNo) _
                  = Get_SquareNo(myCoordinateB.X, myCoordinateB.Y, sNo) Then
            IsSameGroup = True
        End If

    End Function

    '
    '  指定した２つのマスの指定Noにおける候補のリンク状態をチェック
    '
    Private Function Get_LinkLevel(ByVal myCoordinateA As Coordinate, ByVal myCoordinateB As Coordinate, _
                                                 ByVal tmpNumberGrid(,) As SudokuGrid) As Integer

        Dim myNo As Integer

        Get_LinkLevel = 0

        If IsSameGroup(myCoordinateA, myCoordinateB) = True Then
            myNo = myCoordinateA.No
            If tmpNumberGrid(myCoordinateA.X, myCoordinateA.Y).ProspectNo.IndexOf(myNo) >= 0 _
                  And tmpNumberGrid(myCoordinateB.X, myCoordinateB.Y).ProspectNo.IndexOf(myNo) >= 0 Then
                If tmpNumberGrid(myCoordinateA.X, myCoordinateA.Y).ProspectNo.Count = 2 _
                      And tmpNumberGrid(myCoordinateB.X, myCoordinateB.Y).ProspectNo.Count = 2 Then
                    Get_LinkLevel = 2   '強リンク
                Else
                    Get_LinkLevel = 1   '弱リンク
                End If
            End If
        End If

    End Function

    '
    '  指定した２つのマスのそれぞれと同じグループ（横・縦列、スクエア）に属するマスの情報を取得し配列に収納
    '

    Private Function Get_MutualGrid(ByVal myCoordinateA As Coordinate, ByVal myCoordinateB As Coordinate) _
                                          As List(Of Coordinate)

        Dim tmpCoordinate As Coordinate
        Dim i As Integer
        Dim j As Integer
        Dim p As Integer

        Get_MutualGrid = New List(Of Coordinate)

        For j = 1 To GridCount
            For i = 1 To GridCount
                If (i <> myCoordinateA.X Or j <> myCoordinateA.Y) _
                     And (i <> myCoordinateB.X Or j <> myCoordinateB.Y) Then
                    tmpCoordinate = New Coordinate(i, j)
                    If IsSameGroup(tmpCoordinate, myCoordinateA) And IsSameGroup(tmpCoordinate, myCoordinateB) Then
                        Get_MutualGrid.Add(tmpCoordinate)
                    End If
                End If
            Next
        Next



    End Function

    '
    '  各マスの候補Noを適正化
    '
    Private Sub Adjust_ProspectNo(ByRef tmpNumberGrid(,) As SudokuGrid)

        Dim i As Integer, j As Integer

        For j = 1 To GridCount
            For i = 1 To GridCount
                Call tmpNumberGrid(i, j).Reset_ProspectNo()
            Next
        Next

        For j = 1 To GridCount
            For i = 1 To GridCount
                If tmpNumberGrid(i, j).FixNo > 0 Then
                    Call Remove_ProspectNo(1, New Coordinate(i, j, 0, tmpNumberGrid(i, j).FixNo), tmpNumberGrid)
                End If
            Next
        Next

        'If AnalyzeMode = True Then
        '    For j = 1 To GridCount
        '        For i = 1 To GridCount
        '            If tmpNumberGrid(i, j).ProspectNo.Count = 0 Then
        '                tmpNumberGrid(i, j).BackColor = Color.LightPink
        '            Else
        '                tmpNumberGrid(i, j).BackColor = Color.White
        '            End If
        '        Next
        '    Next
        'End If


    End Sub

    Private Function DuplicateNumber(ByVal tmpNumberGrid(,) As SudokuGrid, _
                                      ByVal myCoordinate As Coordinate) As Boolean

        Dim x As Integer, y As Integer
        Dim x2 As Integer, y2 As Integer

        DuplicateNumber = False

        For y = 1 To GridCount
            For x = 1 To GridCount
                If (myCoordinate.X = 0 Or myCoordinate.X = x) And (myCoordinate.Y = 0 Or myCoordinate.Y = y) Then
                    If tmpNumberGrid(x, y).FixNo > 0 Then
                        For y2 = 1 To GridCount
                            For x2 = 1 To GridCount
                                If IsSameGroup(New Coordinate(x, y, 0), New Coordinate(x2, y2, 0)) = True _
                                   And (x <> x2 Or y <> y2) Then
                                    If tmpNumberGrid(x, y).FixNo = tmpNumberGrid(x2, y2).FixNo Then
                                        DuplicateNumber = True
                                        Exit Function
                                    End If
                                End If
                            Next
                        Next
                    End If
                End If
            Next
        Next


    End Function

    '
    '  該当グリッドの候補Noが同一列・スクエアにおいて単独であるかどうかをチェック
    '
    Private Function Check_ProspectNo_Solo(ByVal myPlace As Coordinate, _
                                              ByRef tmpNumberGrid(,) As SudokuGrid) As Boolean
        ' x As Integer, ByVal y As Integer, ByVal myNo As Integer

        Dim i As Integer
        Dim j As Integer
        Dim flgX As Boolean
        Dim flgY As Boolean
        Dim flgS As Boolean
        Dim myS As Integer, mySNo As Integer

        Check_ProspectNo_Solo = False

        '同列（横）に該当番号が候補として残っているかをチェック→なければ確定
        For i = 1 To GridCount
            If i <> myPlace.X Then
                '同列（横）の別マスに同番号が入る可能性あり＝確定出来ない
                If tmpNumberGrid(i, myPlace.Y).ProspectNo.IndexOf(myPlace.No) >= 0 Then
                    GoTo SkipX
                End If
            End If
        Next
        GoTo SoloOK

SkipX:
        '同列（縦）をチェック
        For j = 1 To GridCount
            If j <> myPlace.Y Then
                '同列（縦）の別マスに同番号が入る可能性あり＝確定出来ない
                If tmpNumberGrid(myPlace.X, j).ProspectNo.IndexOf(myPlace.No) >= 0 Then
                    GoTo SkipY
                End If
            End If
        Next
        GoTo SoloOK

SkipY:
        '同一スクエア（３×３の正方形）をチェック
        For j = 1 To GridCount
            For i = 1 To GridCount
                If Get_SquareNo(i, j, mySNo) = myPlace.S And (i <> myPlace.X Or myPlace.Y <> j) Then

                    '同スクエアの別マスに同番号が入る可能性あり＝確定出来ない
                    If tmpNumberGrid(i, j).ProspectNo.IndexOf(myPlace.No) >= 0 Then
                        GoTo SkipS
                    End If
                End If
            Next
        Next
        GoTo SoloOK

SkipS:
        Exit Function

SoloOK:
        '横、縦、スクエアのうち、いずれか１つでも確定出来ればＯＫ
        Check_ProspectNo_Solo = True

    End Function

    '
    ' 指定したスクエアにおいて、番号ｎが入る候補マスが、特定列（縦・横）に限定されるかをチェック
    '　　　　　　　　　※限定される場合、他のスクエアの同列にはｎは入らないため、候補から除外
    '
    Private Function Check_ProspectNo_LimitLine(ByVal mySquareNo As Integer, ByRef tmpNumberGrid(,) As SudokuGrid) As Boolean

        Dim i As Integer
        Dim j As Integer
        Dim n As Integer
        Dim p As Integer, sNo As Integer
        Dim x1 As Integer, x2 As Integer
        Dim y1 As Integer, y2 As Integer
        Dim myProspect As New List(Of Coordinate)
        Dim myLimitNo() As Coordinate
        Dim xOnLineFlg As Boolean, yOnLineFlg As Boolean

        Check_ProspectNo_LimitLine = False

        ReDim myLimitNo(0)
        myLimitNo(0) = New Coordinate

        '指定したスクエアのＸ座標、Ｙ座標の範囲
        x1 = ((mySquareNo - 1) Mod 3) * 3 + 1
        x2 = x1 + 2
        y1 = ((mySquareNo - 1) \ 3) * 3 + 1
        y2 = y1 + 2

        For n = 1 To GridCount
            myProspect.Clear()
            For j = y1 To y2
                For i = x1 To x2
                    If tmpNumberGrid(i, j).FixNo = 0 Then
                        If tmpNumberGrid(i, j).ProspectNo.IndexOf(n) >= 0 Then
                            myProspect.Add(New Coordinate(i, j))
                        End If
                    End If
                Next
            Next
            If myProspect.Count > 0 Then
                xOnLineFlg = True
                yOnLineFlg = True
                For p = 0 To myProspect.Count - 1
                    If myProspect(p).X <> myProspect(0).X Then
                        xOnLineFlg = False '縦列限定なし　
                    End If
                    If myProspect(p).Y <> myProspect(0).Y Then
                        yOnLineFlg = False '横列限定なし
                    End If
                Next
                If xOnLineFlg = True Or yOnLineFlg = True Then
                    ReDim Preserve myLimitNo(UBound(myLimitNo) + 1)
                    myLimitNo(UBound(myLimitNo)) = _
                              New Coordinate(IIf(xOnLineFlg = True, myProspect(0).X, 0), _
                                             IIf(yOnLineFlg = True, myProspect(0).Y, 0), _
                                             mySquareNo, n)
                End If
            End If
        Next

        For i = 1 To UBound(myLimitNo)
            'Debug.Print("Check_ProspectNo_LimitLine:" & myLimitNo(i).X & "," & myLimitNo(i).Y & "  S=" & myLimitNo(i).S & "  No." & myLimitNo(i).No)

            If Remove_ProspectNo(2, myLimitNo(i), tmpNumberGrid) = True Then
                Check_ProspectNo_LimitLine = True
            End If
        Next
        'Me.PictureBoxGrid.Invalidate()


    End Function

    '
    ' 指定した列において、番号ｎが入る候補マスが、特定スクエアに限定されるかをチェック
    '　　　　　　　　　※限定される場合、同一スクエア内の別列マスにはｎは入らないため、候補から除外
    '

    Private Function Check_ProspectNo_LimitSquare(ByVal x As Integer, ByVal y As Integer, _
                                                     ByRef tmpNumberGrid(,) As SudokuGrid) As Boolean
        Dim i As Integer
        Dim j As Integer
        Dim n As Integer
        Dim p As Integer
        Dim x1 As Integer, x2 As Integer
        Dim y1 As Integer, y2 As Integer
        Dim sNo As Integer, myS As Integer
        Dim myProspect As New List(Of Coordinate)
        Dim myLimitNo() As Coordinate
        Dim SameSquareFlg As Boolean

        Check_ProspectNo_LimitSquare = False

        ReDim myLimitNo(0)
        myLimitNo(0) = New Coordinate


        If x = 0 Then
            x1 = 1
            x2 = GridCount
        Else
            'x固定（縦列チェック）
            x1 = x
            x2 = x
        End If

        If y = 0 Then
            y1 = 1
            y2 = GridCount
        Else
            'y固定（横列チェック）
            y1 = y
            y2 = y
        End If

        For n = 1 To GridCount
            myProspect.Clear()
            For j = y1 To y2
                For i = x1 To x2
                    If tmpNumberGrid(i, j).FixNo = 0 Then
                        If tmpNumberGrid(i, j).ProspectNo.IndexOf(n) >= 0 Then
                            myProspect.Add(New Coordinate(i, j, Get_SquareNo(i, j, sNo), n))
                        End If
                    End If
                Next
            Next
            If myProspect.Count > 0 Then
                SameSquareFlg = True
                myS = myProspect(0).S
                For p = 0 To myProspect.Count - 1
                    If myProspect(p).S <> myS Then
                        SameSquareFlg = False
                        Exit For
                    End If
                Next
                If SameSquareFlg = True Then
                    '                  Debug.Print("s=" & mySquareNo & " No." & n & "  " & xOnLineFlg & "," & yOnLineFlg)
                    ReDim Preserve myLimitNo(UBound(myLimitNo) + 1)
                    myLimitNo(UBound(myLimitNo)) = New Coordinate(x, y, myS, n)
                End If
            End If
        Next

        For p = 1 To UBound(myLimitNo)
            'Debug.Print("Check_ProspectNo_LimitSquare  s=" & myLimitNo(p).S & "  x=" & myLimitNo(p).X & "  y=" & myLimitNo(p).Y & "  No." & myLimitNo(p).No)
            If Remove_ProspectNo(3, myLimitNo(p), tmpNumberGrid) = True Then
                Check_ProspectNo_LimitSquare = True
            End If
        Next

    End Function

    Private Function Get_SquareNo(ByVal x As Integer, ByVal y As Integer, ByRef sNo As Integer) As Integer

        Get_SquareNo = ((y - 1) \ 3) * 3 + (((x - 1) \ 3) Mod 3) + 1
        sNo = ((y - 1) Mod 3) * 3 + ((x - 1) Mod 3) + 1

    End Function

    Private Function Get_XY_From_SquareNo(ByVal s As Integer, ByVal sNo As Integer) As Coordinate

        Get_XY_From_SquareNo = New Coordinate

        Get_XY_From_SquareNo.X = ((s - 1) Mod 3) * 3 + ((sNo - 1) Mod 3) + 1
        Get_XY_From_SquareNo.Y = ((s - 1) \ 3) * 3 + ((sNo - 1) \ 3) + 1

    End Function

    Private Function Set_RndProspectNo(ByVal x As Integer, ByVal y As Integer) As List(Of Integer)

        Dim i As Integer

        Set_RndProspectNo = New List(Of Integer)

        Set_RndProspectNo = SudokuNumberGrid(x, y).ProspectNo
        For i = 0 To SudokuNumberGrid(x, y).ExcludeNo.Count - 1
            If Set_RndProspectNo.IndexOf(SudokuNumberGrid(x, y).ExcludeNo(i)) >= 0 Then
                Set_RndProspectNo.Remove(SudokuNumberGrid(x, y).ExcludeNo(i))
            End If
        Next

    End Function

    '
    ' 　同列（縦・横）、同一スクエアにおいて候補Noリスト（No数=ｎ）の同じマスがｎ個存在する場合、
    '　 　　　　　　同列（縦・横）、同一スクエアの別のマスの候補から、そのNoを除外する
    '
    Private Function Check_ProspectNo_NakedPairTriple(ByRef tmpNumberGrid(,) As SudokuGrid) As Boolean

        Dim i As Integer, j As Integer
        Dim n As Integer
        Dim p As Integer
        Dim x As Integer, y As Integer
        Dim x1 As Integer, x2 As Integer
        Dim y1 As Integer, y2 As Integer
        Dim myNo As Integer
        Dim myS As Integer, mySNo As Integer, mySNoB As Integer
        Dim SameGrid As New List(Of Integer)
        Dim myProspect As New List(Of Coordinate)

        Check_ProspectNo_NakedPairTriple = False

        For y = 1 To GridCount
            For x = 1 To GridCount
                If tmpNumberGrid(x, y).FixNo = 0 Then
                    '同列（横）に候補リストの組み合わせが同じマスがいくつあるかをチェック
                    SameGrid.Clear()
                    For i = 1 To GridCount
                        If i <> x Then
                            '                            If IsSameProspectNoListNew(x, y, i, y, tmpNumberGrid) = True Then
                            If IsSameProspectNoList(New Coordinate(x, y, 0), New Coordinate(i, y, 0), tmpNumberGrid) = True Then
                                SameGrid.Add(CInt(i))
                            End If
                        Else
                            SameGrid.Add(CInt(i))
                        End If
                    Next
                    If SameGrid.Count = tmpNumberGrid(x, y).ProspectNo.Count Then
                        For i = 1 To GridCount
                            If SameGrid.IndexOf(i) < 0 Then  '候補リストが同じでないマス（＝除外対象）
                                For n = 0 To tmpNumberGrid(x, y).ProspectNo.Count - 1
                                    myNo = tmpNumberGrid(x, y).ProspectNo(n)
                                    If tmpNumberGrid(i, y).ProspectNo.IndexOf(myNo) >= 0 Then
                                        tmpNumberGrid(i, y).ProspectNo.Remove(myNo)
                                        'Debug.Print("Check_ProspectNo_NakedPairTriple 同列（横） : " & x & "," & y _
                                        '            & "  x=" & i _
                                        '            & "  y=" & y _
                                        '            & "  RemoveNo=" & myNo)
                                        Check_ProspectNo_NakedPairTriple = True
                                    End If
                                Next
                            End If
                        Next
                    End If
                    If Check_ProspectNo_NakedPairTriple = True Then
                        Exit Function
                    End If

                    '同列（縦）に候補リストの組み合わせが同じマスがいくつあるかをチェック
                    SameGrid.Clear()
                    For j = 1 To GridCount
                        If j <> y Then
                            If IsSameProspectNoList(New Coordinate(x, y, 0), New Coordinate(x, j, 0), tmpNumberGrid) = True Then
                                '                                If IsSameProspectNoListNew(x, y, x, j, tmpNumberGrid) = True Then
                                SameGrid.Add(CInt(j))
                            End If
                        Else
                            SameGrid.Add(CInt(j))
                        End If
                    Next
                    If SameGrid.Count = tmpNumberGrid(x, y).ProspectNo.Count Then
                        For j = 1 To GridCount
                            If SameGrid.IndexOf(j) < 0 Then  '候補リストが同じでないマス（＝除外対象）
                                For n = 0 To tmpNumberGrid(x, y).ProspectNo.Count - 1
                                    'Debug.Print("ProspectNo.Count=" & tmpNumberGrid(x, y).ProspectNo.Count)
                                    'Debug.Print("ProspectNo." & n & "=" & tmpNumberGrid(x, y).ProspectNo(n))
                                    myNo = tmpNumberGrid(x, y).ProspectNo(n)
                                    If tmpNumberGrid(x, j).ProspectNo.IndexOf(myNo) >= 0 Then
                                        tmpNumberGrid(x, j).ProspectNo.Remove(myNo)
                                        'Debug.Print("Check_ProspectNo_NakedPairTriple 同列（縦） : " & x & "," & y _
                                        '            & "  x=" & x _
                                        '            & "  y=" & j _
                                        '            & "  RemoveNo=" & myNo)
                                        Check_ProspectNo_NakedPairTriple = True
                                    End If
                                Next
                            End If
                        Next
                    End If
                    If Check_ProspectNo_NakedPairTriple = True Then
                        Exit Function
                    End If

                    '同一スクエアに候補リストの組み合わせが同じマスがいくつあるかをチェック
                    SameGrid.Clear()
                    myS = Get_SquareNo(x, y, mySNo)
                    For j = 1 To GridCount
                        For i = 1 To GridCount
                            If Get_SquareNo(i, j, mySNoB) = myS Then
                                If (i <> x Or j <> y) Then
                                    '                                    If IsSameProspectNoListNew(x, y, i, j, tmpNumberGrid) = True Then
                                    If IsSameProspectNoList(New Coordinate(x, y, 0), New Coordinate(i, j, 0), tmpNumberGrid) = True Then
                                        SameGrid.Add(CInt(mySNoB))
                                    End If
                                Else
                                    SameGrid.Add(CInt(mySNoB))
                                End If
                            End If
                        Next
                    Next
                    If SameGrid.Count = tmpNumberGrid(x, y).ProspectNo.Count Then
                        For j = 1 To GridCount
                            For i = 1 To GridCount
                                If Get_SquareNo(i, j, mySNoB) = myS Then
                                    If SameGrid.IndexOf(mySNoB) < 0 Then  '候補リストが同じでないマス（＝除外対象）
                                        For n = 0 To tmpNumberGrid(x, y).ProspectNo.Count - 1
                                            myNo = tmpNumberGrid(x, y).ProspectNo(n)
                                            If tmpNumberGrid(i, j).ProspectNo.IndexOf(myNo) >= 0 Then
                                                'Debug.Print("Check_ProspectNo_NakedPairTriple 同スクエア: " & x & "," & y _
                                                '            & "  x=" & i _
                                                '            & "  y=" & j _
                                                '            & "  RemoveNo=" & myNo & " -----" & SameGrid.Count)
                                                tmpNumberGrid(i, j).ProspectNo.Remove(myNo)
                                                Check_ProspectNo_NakedPairTriple = True
                                            End If
                                        Next
                                    End If
                                End If
                            Next
                        Next
                    End If
                    If Check_ProspectNo_NakedPairTriple = True Then
                        Exit Function
                    End If
                End If
            Next
        Next

    End Function
    '
    ' 　同列（縦・横）、同一スクエアにおいて、ｎ種類の数字が入りうるマスがｎ個しか存在しない場合、
    '　 　　　　　　それらのマスには他の数字は入らないので候補から除外する
    '
    Private Function Check_ProspectNo_HiddenPairTriple(ByRef tmpNumberGrid(,) As SudokuGrid) As Boolean

        Dim i As Integer, j As Integer, k As Integer
        Dim n As Integer, Gr As Integer, fitFlg As Integer
        Dim p As Integer
        Dim x As Integer, y As Integer
        Dim sNo As Integer
        Dim myProspect As New List(Of Coordinate)
        Dim PickedNoListOrder() As List(Of Integer)
        Dim PickedNoList As List(Of Integer)
        Dim removeFlg As Boolean
        Dim curNo As Integer
        Dim strNo As String, ss As Integer

        Check_ProspectNo_HiddenPairTriple = False

        PickedNoList = New List(Of Integer)


F_Top:
        '行・列・スクエア
        For Gr = 1 To 3
            'チェックするのはPairもしくはTriple
            For n = 2 To 3
                For y = 1 To GridCount
                    For x = 1 To GridCount
                        If tmpNumberGrid(x, y).FixNo = 0 And tmpNumberGrid(x, y).ProspectNo.Count >= n Then
                            Call Get_Combinatorics(tmpNumberGrid(x, y).ProspectNo.Count, n, PickedNoListOrder)
                            For i = 1 To UBound(PickedNoListOrder)
                                PickedNoList.Clear()
                                For j = 0 To PickedNoListOrder(i).Count - 1
                                    'Debug.Print(tmpNumberGrid(x, y).ProspectNo.Count & ":::" & PickedNoListOrder(i)(j) - 1)
                                    'Debug.Print("i=" & i & " j=" & j & "  " & tmpNumberGrid(x, y).ProspectNo(PickedNoListOrder(i)(j) - 1))
                                    PickedNoList.Add(tmpNumberGrid(x, y).ProspectNo(PickedNoListOrder(i)(j) - 1))
                                Next

                                If Gr = 1 Then
                                    fitFlg = Check_ProspectNoList_On_Group(New Coordinate(0, y, 0, 0), PickedNoList, myProspect, tmpNumberGrid)
                                ElseIf Gr = 2 Then
                                    fitFlg = Check_ProspectNoList_On_Group(New Coordinate(x, 0, 0, 0), PickedNoList, myProspect, tmpNumberGrid)
                                ElseIf Gr = 3 Then
                                    fitFlg = Check_ProspectNoList_On_Group(New Coordinate(0, 0, Get_SquareNo(x, y, sNo), 0), PickedNoList, myProspect, tmpNumberGrid)
                                End If
                                If fitFlg Then
                                    strNo = ""
                                    For ss = 0 To PickedNoList.Count - 1
                                        strNo = strNo & PickedNoList(ss) & "-"
                                    Next


                                    For p = 0 To myProspect.Count - 1
                                        '                                       For k = 0 To tmpNumberGrid(myProspect(p).X, myProspect(p).Y).ProspectNo.Count - 1
                                        For k = tmpNumberGrid(myProspect(p).X, myProspect(p).Y).ProspectNo.Count - 1 To 0 Step -1
                                            curNo = tmpNumberGrid(myProspect(p).X, myProspect(p).Y).ProspectNo(k)
                                            If PickedNoList.IndexOf(curNo) < 0 Then
                                                tmpNumberGrid(myProspect(p).X, myProspect(p).Y).ProspectNo.Remove(curNo)
                                                Check_ProspectNo_HiddenPairTriple = True
                                                'Debug.Print("Check_ProspectNo_HiddenPairTriple " & strGr(Gr) & " : " & strNo & "  " & x & "," & y _
                                                '            & "  Gr=" & Gr _
                                                '            & "  x=" & myProspect(p).X _
                                                '            & "  y=" & myProspect(p).Y _
                                                '            & "  RemoveNo=" & curNo)
                                                Exit Function
                                                '                                                GoTo F_Top
                                            End If
                                        Next
                                    Next
                                End If
                            Next
                        End If
                    Next
                Next
            Next
        Next

    End Function

    '
    '  同一グループ（横・縦列、スクエア）上で指定したｎ種類の数字が入りうるマスがｎ個であるかをチェック
    '
    Private Function Check_ProspectNoList_On_Group(ByVal myTarget As Coordinate, ByVal myNumList As List(Of Integer), _
                                               ByRef myPlace As List(Of Coordinate), _
                                               ByVal tmpNumberGrid(,) As SudokuGrid) As Boolean
        Dim i As Integer, j As Integer
        Dim n As Integer, nn As Integer
        Dim sNo As Integer
        Dim myPlaceNum() As List(Of Coordinate)
        Dim strNum As String

        strNum = ""


        Check_ProspectNoList_On_Group = False

        myPlace = New List(Of Coordinate)
        nn = myNumList.Count
        ReDim myPlaceNum(nn)

        For n = 0 To nn - 1
            strNum = strNum & myNumList(n) & ","
            myPlaceNum(n) = New List(Of Coordinate)
            For j = 1 To GridCount
                For i = 1 To GridCount
                    If i = myTarget.X Or j = myTarget.Y Or Get_SquareNo(i, j, sNo) = myTarget.S Then
                        If tmpNumberGrid(i, j).ProspectNo.IndexOf(myNumList(n)) >= 0 Then
                            myPlaceNum(n).Add(New Coordinate(i, j, 0, 0))
                        End If
                    End If
                Next
            Next
            myPlace = Merge_CoordinateList(myPlace, myPlaceNum(n))
        Next

        If myPlace.Count = nn Then
            Check_ProspectNoList_On_Group = True
        End If

        'Debug.Print(strNum)

    End Function

    Private Function Merge_CoordinateList(ByVal listA As List(Of Coordinate), ByVal listB As List(Of Coordinate)) As List(Of Coordinate)

        Dim i As Integer
        Dim j As Integer
        Dim sameFlg As Boolean
        Dim listAB As New List(Of Coordinate)

        '       Merge_CoordinateList = New List(Of Coordinate)

        For i = 0 To listA.Count - 1
            listAB.Add(listA(i))

        Next
        '        Merge_CoordinateList.AddRange(listA)

        For i = 0 To listB.Count - 1
            sameFlg = False
            For j = 0 To listAB.Count - 1
                If listB(i).X = listAB(j).X And listB(i).Y = listAB(j).Y Then
                    sameFlg = True
                    Exit For
                End If
            Next
            If sameFlg = False Then
                listAB.Add(listB(i))
            End If
        Next

        Merge_CoordinateList = listAB

    End Function

    '
    '　対象マス【座標:(TargetX,TargetY)】の候補リストが、参照元【座標:(SourceX,SourceY)】と同じか否かをチェック  
    '
    Private Function IsSameProspectNoList(ByVal mySource As Coordinate, ByVal myTarget As Coordinate, _
                                          ByVal tmpNumberGrid(,) As SudokuGrid) As Boolean
        Dim i As Integer
        Dim myNo As Integer

        IsSameProspectNoList = False

        If tmpNumberGrid(myTarget.X, myTarget.Y).FixNo > 0 _
              Or tmpNumberGrid(myTarget.X, myTarget.Y).ProspectNo.Count = 0 Then
            Exit Function
        End If

        '参照元と対象マスの候補Noリストが完全に一致しなくても、
        '参照元(1,3,8)、対象マス(3,8)などのように対象マスの候補Noが参照元に全て含まれていればＯＫ
        For i = 0 To tmpNumberGrid(myTarget.X, myTarget.Y).ProspectNo.Count - 1
            myNo = tmpNumberGrid(myTarget.X, myTarget.Y).ProspectNo(i)
            If tmpNumberGrid(mySource.X, mySource.Y).ProspectNo.IndexOf(myNo) < 0 Then
                Exit Function
            End If
        Next

        IsSameProspectNoList = True


    End Function

    Private Function Check_ProspectNo_XYWing(ByRef tmpNumberGrid(,) As SudokuGrid) As Boolean

        'xyを候補とするマスと同じグループ内（横・縦列、スクエア）に
        'xzを候補とするマス１とyzを候補とするマス２がある場合（マス１とマス２は同じグループでなくて良い）
        'マス１、マス２それぞれの影響範囲（横・縦列、スクエア）が重なるマスにはzは入らない

        Dim i As Integer, j As Integer
        Dim ii As Integer, jj As Integer
        Dim n As Integer
        Dim p As Integer, pp As Integer
        Dim p1 As Integer, p2 As Integer
        Dim x As Integer, y As Integer, z As Integer
        Dim xIndex As Integer, yIndex As Integer, zIndex As Integer
        Dim myNo As Integer
        '        Dim myCoordinate() As Coordinate
        Dim crossCoordinate As New List(Of Coordinate)
        Dim CoordinateXZ() As Coordinate
        Dim CoordinateYZ() As Coordinate

        '        ReDim myCoordinate(0)

        Check_ProspectNo_XYWing = False

        p = 0
        For j = 1 To GridCount
            For i = 1 To GridCount
                'Debug.Print("x=" & i & " y=" & j & "   " & tmpNumberGrid(i, j).ProspectNo.Count)
                If tmpNumberGrid(i, j).ProspectNo.Count = 2 Then
                    ReDim CoordinateXZ(0)
                    ReDim CoordinateYZ(0)
                    'x,yをセット
                    x = tmpNumberGrid(i, j).ProspectNo(0)
                    y = tmpNumberGrid(i, j).ProspectNo(1)
                    'Debug.Print("Prospect2  x=" & i & " y=" & j & "   " & x & "," & y)
                    For jj = 1 To GridCount
                        For ii = 1 To GridCount
                            If (i <> ii Or j <> jj) And tmpNumberGrid(ii, jj).ProspectNo.Count = 2 _
                                      And IsSameGroup(New Coordinate(i, j), New Coordinate(ii, jj)) = True Then
                                xIndex = tmpNumberGrid(ii, jj).ProspectNo.IndexOf(x)
                                'Debug.Print("Prospect2a  x=" & ii & " y=" & jj)

                                If xIndex >= 0 Then
                                    zIndex = IIf(xIndex = 0, 1, 0)
                                    ReDim Preserve CoordinateXZ(UBound(CoordinateXZ) + 1)
                                    CoordinateXZ(UBound(CoordinateXZ)) _
                                       = New Coordinate(ii, jj, , tmpNumberGrid(ii, jj).ProspectNo(zIndex))
                                    'Debug.Print("x=" & CoordinateXZ(UBound(CoordinateXZ)).X _
                                    '            & "  y=" & CoordinateXZ(UBound(CoordinateXZ)).Y _
                                    '            & "  no=" & CoordinateXZ(UBound(CoordinateXZ)).No)
                                End If
                                yIndex = tmpNumberGrid(ii, jj).ProspectNo.IndexOf(y)
                                If yIndex >= 0 Then
                                    zIndex = IIf(yIndex = 0, 1, 0)
                                    ReDim Preserve CoordinateYZ(UBound(CoordinateYZ) + 1)
                                    CoordinateYZ(UBound(CoordinateYZ)) _
                                       = New Coordinate(ii, jj, , tmpNumberGrid(ii, jj).ProspectNo(zIndex))
                                    'Debug.Print("x=" & CoordinateYZ(UBound(CoordinateYZ)).X _
                                    '            & "  y=" & CoordinateYZ(UBound(CoordinateYZ)).Y _
                                    '            & "  no=" & CoordinateYZ(UBound(CoordinateYZ)).No)
                                End If
                            End If
                        Next
                    Next
                    For p1 = 1 To UBound(CoordinateXZ)
                        For p2 = 1 To UBound(CoordinateYZ)
                            If CoordinateXZ(p1).No = CoordinateYZ(p2).No Then
                                z = CoordinateXZ(p1).No
                                'MsgBox(CoordinateXZ(p1).X & "," & CoordinateXZ(p1).Y & "  " & CoordinateYZ(p2).X & "," & CoordinateYZ(p2).Y)
                                '影響範囲（横・縦列、スクエア）が重なるマスの座標を取得
                                '                               ReDim myCoordinate(0)
                                crossCoordinate = Get_MutualGrid(CoordinateXZ(p1), CoordinateYZ(p2))
                                '                                Call BkGet_MutualGrid(CoordinateXZ(p1), CoordinateYZ(p2), myCoordinate)
                                '                                For p = 1 To UBound(myCoordinate)
                                For p = 0 To crossCoordinate.Count - 1
                                    If tmpNumberGrid(crossCoordinate(p).X, crossCoordinate(p).Y).ProspectNo.IndexOf(z) >= 0 Then
                                        tmpNumberGrid(crossCoordinate(p).X, crossCoordinate(p).Y).ProspectNo.Remove(z)
                                        'Debug.Print("Check_ProspectNo_XYWing  XY=" & i & "," & j _
                                        '            & "  XZ=" & CoordinateXZ(p1).X & "," & CoordinateXZ(p1).Y _
                                        '            & "  YZ=" & CoordinateYZ(p2).X & "," & CoordinateYZ(p2).Y _
                                        '            & "  x=" & crossCoordinate(p).X _
                                        '            & "  y=" & crossCoordinate(p).Y _
                                        '            & "  no=" & z)

                                        Check_ProspectNo_XYWing = True
                                    End If

                                Next

                            End If
                        Next
                    Next

                    CoordinateXZ = Nothing
                    CoordinateYZ = Nothing


                End If
            Next
        Next

        'Debug.Print("----Check_ProspectNo_XYWing=" & Check_ProspectNo_XYWing)

    End Function

    Private Function Check_ProspectNo_SwordFish(ByRef tmpNumberGrid(,) As SudokuGrid, _
                                                  Optional ByVal LineCntLev As Integer = 3) As Boolean

        Dim i As Integer, j As Integer
        Dim n As Integer
        Dim p As Integer, c As Integer
        Dim x As Integer, y As Integer
        Dim LineCnt As Integer
        Dim SameGrid As New List(Of Integer)
        Dim myPlaceX() As List(Of Integer)
        Dim myPlaceY() As List(Of Integer)
        Dim myPlaceX_Y() As Integer
        Dim myPlaceY_X() As Integer
        Dim myPlaceUnion As New List(Of Integer)
        Dim myProspect As New List(Of Coordinate)
        Dim myProspectB As New List(Of Coordinate)
        Dim targetLine As New List(Of Integer)
        Dim PickedNoList() As List(Of Integer)

        Check_ProspectNo_SwordFish = False

        For n = 1 To GridCount
            c = 0
            ReDim myPlaceX(0)
            ReDim myPlaceX_Y(0)
            myPlaceX(0) = New List(Of Integer)
            For y = 1 To GridCount   '横列チェック
                targetLine.Clear()
                LineCnt = Count_ProspectNo_On_Group(New Coordinate(0, y, 0, n), myProspect, tmpNumberGrid)
                If LineCnt >= 2 And LineCnt <= LineCntLev Then
                    c = c + 1
                    ReDim Preserve myPlaceX(c)
                    myPlaceX(c) = New List(Of Integer)
                    For i = 0 To myProspect.Count - 1
                        myPlaceX(c).Add(myProspect(i).X)
                    Next
                    ReDim Preserve myPlaceX_Y(c)
                    myPlaceX_Y(c) = y
                End If
            Next

            If UBound(myPlaceX) >= 2 Then
                '対象Noが候補となっているマス数が指定（LineLev）以下である列（列数=p）のうち、
                '任意のLineLev列の候補となっているＸ座標数＝LineLevに限定される時、他列の同Ｘ座標マス
                'の候補から対象Noを除外出来る
                For LineCnt = 2 To LineCntLev
                    Call Get_Combinatorics(UBound(myPlaceX), LineCnt, PickedNoList)
                    For p = 1 To UBound(PickedNoList)
                        myPlaceUnion = Get_ProspectNo_Union(myPlaceX, PickedNoList(p))  '候補Noの和集合（Ｘ座標）を取得
                        If myPlaceUnion.Count = LineCnt Then
                            For y = 1 To GridCount
                                For c = 0 To PickedNoList(p).Count - 1
                                    If myPlaceX_Y(PickedNoList(p)(c)) = y Then '対象行（除外対象外）
                                        GoTo skip_y
                                    End If
                                Next
                                For i = 0 To myPlaceUnion.Count - 1
                                    If tmpNumberGrid(myPlaceUnion(i), y).FixNo = 0 _
                                       And tmpNumberGrid(myPlaceUnion(i), y).ProspectNo.IndexOf(n) >= 0 Then
                                        tmpNumberGrid(myPlaceUnion(i), y).ProspectNo.Remove(n)
                                        'Debug.Print("Check_ProspectNoSwordFish  No." & n & "  x=" & myPlaceUnion(i) & "  y=" & y)
                                        Check_ProspectNo_SwordFish = True
                                    End If
                                Next
Skip_y:
                            Next
                        End If
                    Next
                    If Check_ProspectNo_SwordFish = True Then
                        Exit Function
                    End If
                Next
            End If

            c = 0
            ReDim myPlaceY(0)
            ReDim myPlaceY_X(0)
            myPlaceY(0) = New List(Of Integer)
            For x = 1 To GridCount   '縦列チェック
                targetLine.Clear()
                LineCnt = Count_ProspectNo_On_Group(New Coordinate(x, 0, 0, n), myProspect, tmpNumberGrid)

                If LineCnt >= 2 And LineCnt <= LineCntLev Then
                    c = c + 1
                    ReDim Preserve myPlaceY(c)
                    myPlaceY(c) = New List(Of Integer)
                    For i = 0 To myProspect.Count - 1
                        myPlaceY(c).Add(myProspect(i).Y)
                    Next
                    ReDim Preserve myPlaceY_X(c)
                    myPlaceY_X(c) = x
                End If
            Next
            If UBound(myPlaceY) >= 2 Then
                '対象Noが候補となっているマス数が指定（LineLev）以下である列（列数=p）のうち、
                '任意のLineLev列の候補となっているＹ座標数＝LineLevに限定される時、他列の同Ｙ座標マス
                'の候補から対象Noを除外出来る
                For LineCnt = 2 To LineCntLev
                    Call Get_Combinatorics(UBound(myPlaceY), LineCnt, PickedNoList)
                    For p = 1 To UBound(PickedNoList)
                        myPlaceUnion = Get_ProspectNo_Union(myPlaceY, PickedNoList(p))  '候補Noの和集合（y座標）を取得
                        If myPlaceUnion.Count = LineCnt Then
                            For x = 1 To GridCount
                                For c = 0 To PickedNoList(p).Count - 1
                                    If myPlaceY_X(PickedNoList(p)(c)) = x Then '対象行（除外対象外）
                                        GoTo skip_x
                                    End If
                                Next
                                For i = 0 To myPlaceUnion.Count - 1
                                    If tmpNumberGrid(x, myPlaceUnion(i)).FixNo = 0 _
                                       And tmpNumberGrid(x, myPlaceUnion(i)).ProspectNo.IndexOf(n) >= 0 Then
                                        tmpNumberGrid(x, myPlaceUnion(i)).ProspectNo.Remove(n)
                                        'Debug.Print("Check_ProspectNoSwordFish  No." & n & "  x=" & x & "  y=" & myPlaceUnion(i))
                                        Check_ProspectNo_SwordFish = True
                                    End If
                                Next
Skip_x:
                            Next
                        End If
                    Next
                    If Check_ProspectNo_SwordFish = True Then
                        Exit Function
                    End If
                Next
            End If

        Next

        'Debug.Print("----Check_ProspectNo_SwordFish=" & Check_ProspectNo_SwordFish)


    End Function

    '
    '　指定した複数列の候補Noが位置する座標の和集合を取得
    '
    Private Function Get_ProspectNo_Union(ByVal myPlace() As List(Of Integer), _
                                          ByVal myPickedNoList As List(Of Integer)) As List(Of Integer)

        Dim i As Integer, j As Integer
        Dim p As Integer
        Dim myLineNo As New List(Of Integer)

        Get_ProspectNo_Union = New List(Of Integer)
        For i = 0 To myPickedNoList.Count - 1
            For j = 0 To myPlace(myPickedNoList(i)).Count - 1
                If myLineNo.IndexOf(myPlace(myPickedNoList(i))(j)) < 0 Then
                    myLineNo.Add(myPlace(myPickedNoList(i))(j))
                End If
            Next
        Next
        Get_ProspectNo_Union.AddRange(myLineNo)

    End Function

    '
    '  n個の中からm個を選ぶ組み合わせを取得し、配列（myCombinatorics）に収納
    '
    Private Sub Get_Combinatorics(ByVal n As Integer, ByVal m As Integer, _
                                          ByRef myCombinatorics() As List(Of Integer))
        Dim i As Integer
        Dim j As Integer
        Dim nCm As Integer
        Dim v() As Integer, c As Integer


        ReDim v(m)

        'nCm=全組み合わせ数
        nCm = Combinatorics(n, m)
        ReDim myCombinatorics(nCm)

        For i = 1 To nCm
            If i = 1 Then
                '配列の先頭に入れる組み合わせ（最小のものからＰ個）
                For c = 1 To m
                    v(c) = c
                Next
            Else
                c = m
                Do
                    If v(c) < n - (m - c) Then
                        v(c) = v(c) + 1
                        For j = c + 1 To m
                            v(j) = v(j - 1) + 1
                        Next

                        Exit Do
                    Else
                        c = c - 1
                    End If
                Loop
            End If

            myCombinatorics(i) = New List(Of Integer)
            For c = 1 To m
                myCombinatorics(i).Add(v(c))
            Next
        Next

    End Sub

    Private Function Check_ProspectNo_SimpleColors(ByRef tmpNumberGrid(,) As SudokuGrid) As Boolean

        Dim i As Integer, j As Integer
        Dim n As Integer, p As Integer
        Dim linkNo As Integer
        Dim excludeColor As Integer
        Dim crossCoordinate As New List(Of Coordinate)
        Dim myStrongLinkChain() As List(Of Coordinate)



        For n = 1 To GridCount
            '強リンクのチェーンを作成
            Call Create_StrongLinkChain(n, myStrongLinkChain, tmpNumberGrid)
            For linkNo = 1 To UBound(myStrongLinkChain)
                excludeColor = 0
                For i = 0 To myStrongLinkChain(linkNo).Count - 2
                    For j = i + 1 To myStrongLinkChain(linkNo).Count - 1

                        'Debug.Print("No." & n & "   " & myStrongLinkChain(linkNo)(i).X & "," & myStrongLinkChain(linkNo)(i).Y _
                        '             & "   " & myStrongLinkChain(linkNo)(j).X & "," & myStrongLinkChain(linkNo)(j).Y)


                        If myStrongLinkChain(linkNo)(i).ColorNo <> myStrongLinkChain(linkNo)(j).ColorNo Then
                            'チェーンで２色分け（裏表）されたマスの、
                            '双方各色の影響範囲（横・縦列、スクエア）が重なるマスの座標を取得
                            crossCoordinate = Get_MutualGrid(myStrongLinkChain(linkNo)(i), myStrongLinkChain(linkNo)(j))
                            For p = 0 To crossCoordinate.Count - 1
                                If tmpNumberGrid(crossCoordinate(p).X, crossCoordinate(p).Y).FixNo = 0 _
                                   And tmpNumberGrid(crossCoordinate(p).X, crossCoordinate(p).Y).ProspectNo.IndexOf(n) >= 0 Then
                                    'Debug.Print("SimpleColors crossCoordinate : No=" & n _
                                    '            & "  x=" & crossCoordinate(p).X _
                                    '            & "  y=" & crossCoordinate(p).Y & " ..." & tmpNumberGrid(crossCoordinate(p).X, crossCoordinate(p).Y).ProspectNo.IndexOf(n) & " FixNo=" & tmpNumberGrid(crossCoordinate(p).X, crossCoordinate(p).Y).FixNo)
                                    tmpNumberGrid(crossCoordinate(p).X, crossCoordinate(p).Y).ProspectNo.Remove(n)
                                    Check_ProspectNo_SimpleColors = True
                                End If
                            Next
                        Else
                            'チェーンで２色分け（裏表）されたマスの、どちらかの色が同じグループ上に複数存在
                            'してしまっている場合、その色のマスは候補とはなり得ない→もう片方の色のマス数値確定　
                            If IsSameGroup(myStrongLinkChain(linkNo)(i), myStrongLinkChain(linkNo)(j)) = True Then
                                excludeColor = myStrongLinkChain(linkNo)(i).ColorNo
                            End If
                        End If
                    Next
                Next
                If excludeColor > 0 Then
                    For i = 0 To myStrongLinkChain(linkNo).Count - 1
                        If myStrongLinkChain(linkNo)(i).ColorNo = excludeColor Then
                            If tmpNumberGrid(myStrongLinkChain(linkNo)(i).X, myStrongLinkChain(linkNo)(i).Y).FixNo = 0 _
                               And tmpNumberGrid(myStrongLinkChain(linkNo)(i).X, myStrongLinkChain(linkNo)(i).Y).ProspectNo.IndexOf(n) >= 0 Then
                                'Debug.Print("Simple Colors Exclude : No=" & n _
                                '            & "  x=" & myStrongLinkChain(linkNo)(i).X _
                                '            & "  y=" & myStrongLinkChain(linkNo)(i).Y)
                                tmpNumberGrid(myStrongLinkChain(linkNo)(i).X, myStrongLinkChain(linkNo)(i).Y).ProspectNo.Remove(n)
                                Check_ProspectNo_SimpleColors = True
                            End If
                        End If
                    Next
                End If
            Next
        Next

        'Debug.Print("----Check_ProspectNo_SimpleColors=" & Check_ProspectNo_SimpleColors)


    End Function

    Private Function Check_ProspectNo_MultiColors(ByRef tmpNumberGrid(,) As SudokuGrid) As Boolean

        Dim i As Integer, j As Integer
        Dim ca As Integer, cb As Integer
        Dim n As Integer, p As Integer
        Dim linkANo As Integer
        Dim linkBNo As Integer
        Dim sameGrpFlg(2, 2) As Boolean
        Dim crossCoordinate As New List(Of Coordinate)
        Dim myStrongLinkChain() As List(Of Coordinate)

        ReDim myStrongLinkChain(0)
        myStrongLinkChain(0) = New List(Of Coordinate)


        For n = 1 To GridCount
            '強リンクのチェーンを作成
            Call Create_StrongLinkChain(n, myStrongLinkChain, tmpNumberGrid)
            'MultiColorsが適用されるのは同ナンバーのリンクチェーンが複数存在する場合のみ
            If UBound(myStrongLinkChain) >= 2 Then
                For linkANo = 1 To UBound(myStrongLinkChain) - 1
                    For linkBNo = linkANo + 1 To UBound(myStrongLinkChain)
                        'フラグを初期化
                        For ca = 1 To 2
                            For cb = 1 To 2
                                sameGrpFlg(ca, cb) = False
                            Next
                        Next
                        For i = 0 To myStrongLinkChain(linkANo).Count - 1
                            For j = 0 To myStrongLinkChain(linkBNo).Count - 1

                                'チェーンＡのマスとチェーンＢのマスが同一グループ上に存在
                                If IsSameGroup(myStrongLinkChain(linkANo)(i), myStrongLinkChain(linkBNo)(j)) = True Then
                                    'フラグオン
                                    sameGrpFlg(myStrongLinkChain(linkANo)(i).ColorNo, myStrongLinkChain(linkBNo)(j).ColorNo) = True
                                End If
                            Next
                        Next
                        'チェーンＡの色a1のマスが、チェーンＢの色b1、b2のそれぞれと同じグループに存在する
                        '　→色b1、b2のマスが両方とも採用されることはない→a1は候補から除外→a2確定　　　
                        For ca = 1 To 2
                            If sameGrpFlg(ca, 1) = True And sameGrpFlg(ca, 2) = True Then
                                For i = 0 To myStrongLinkChain(linkANo).Count - 1
                                    If myStrongLinkChain(linkANo)(i).ColorNo = ca Then
                                        If tmpNumberGrid(myStrongLinkChain(linkANo)(i).X, myStrongLinkChain(linkANo)(i).Y).FixNo = 0 _
                                           And tmpNumberGrid(myStrongLinkChain(linkANo)(i).X, myStrongLinkChain(linkANo)(i).Y).ProspectNo.IndexOf(n) >= 0 Then
                                            tmpNumberGrid(myStrongLinkChain(linkANo)(i).X, myStrongLinkChain(linkANo)(i).Y).ProspectNo.Remove(n)
                                            'Debug.Print("MultiColors Exclude : No=" & n _
                                            '            & "  x=" & myStrongLinkChain(linkANo)(i).X _
                                            '            & "  y=" & myStrongLinkChain(linkANo)(i).Y)
                                            Check_ProspectNo_MultiColors = True
                                        End If
                                    End If
                                Next
                            End If
                        Next
                        If Check_ProspectNo_MultiColors = True Then
                            Exit Function
                        End If

                        For cb = 1 To 2
                            If sameGrpFlg(1, cb) = True And sameGrpFlg(2, cb) = True Then
                                For j = 0 To myStrongLinkChain(linkBNo).Count - 1
                                    If myStrongLinkChain(linkBNo)(j).ColorNo = cb Then
                                        If tmpNumberGrid(myStrongLinkChain(linkBNo)(j).X, myStrongLinkChain(linkBNo)(j).Y).FixNo = 0 _
                                           And tmpNumberGrid(myStrongLinkChain(linkBNo)(j).X, myStrongLinkChain(linkBNo)(j).Y).ProspectNo.IndexOf(n) >= 0 Then
                                            tmpNumberGrid(myStrongLinkChain(linkBNo)(j).X, myStrongLinkChain(linkBNo)(j).Y).ProspectNo.Remove(n)
                                            'Debug.Print("MultiColors Exclude : No=" & n _
                                            '            & "  x=" & myStrongLinkChain(linkBNo)(j).X _
                                            '            & "  y=" & myStrongLinkChain(linkBNo)(j).Y)
                                            Check_ProspectNo_MultiColors = True
                                        End If
                                    End If
                                Next
                            End If
                        Next
                        If Check_ProspectNo_MultiColors = True Then
                            Exit Function
                        End If


                        'チェーンＡの色a1のマスとチェーンＢの色b1のマスが同じグループに存在する
                        '　→色a1と色b1のマスの少なくともどちらかは候補から除外される
                        '　→色a2と色b2の少なくともどちらかは数値確定　　　
                        '　→色a2と色b2の影響範囲（横・縦列、スクエア）が重なるマスは候補から除外　　　
                        For ca = 1 To 2
                            For cb = 1 To 2
                                If sameGrpFlg(ca, cb) = True Then
                                    For i = 0 To myStrongLinkChain(linkANo).Count - 1
                                        For j = 0 To myStrongLinkChain(linkBNo).Count - 1
                                            If myStrongLinkChain(linkANo)(i).ColorNo <> ca And myStrongLinkChain(linkBNo)(j).ColorNo <> cb Then
                                                crossCoordinate = Get_MutualGrid(myStrongLinkChain(linkANo)(i), myStrongLinkChain(linkBNo)(j))
                                                For p = 0 To crossCoordinate.Count - 1
                                                    If tmpNumberGrid(crossCoordinate(p).X, crossCoordinate(p).Y).FixNo = 0 _
                                                       And tmpNumberGrid(crossCoordinate(p).X, crossCoordinate(p).Y).ProspectNo.IndexOf(n) >= 0 Then
                                                        'Debug.Print("MultiColors crossCoordinate : No=" & n _
                                                        '            & "  x=" & crossCoordinate(p).X _
                                                        '            & "  y=" & crossCoordinate(p).Y & " ...." & tmpNumberGrid(crossCoordinate(p).X, crossCoordinate(p).Y).ProspectNo.IndexOf(n))
                                                        tmpNumberGrid(crossCoordinate(p).X, crossCoordinate(p).Y).ProspectNo.Remove(n)
                                                        Check_ProspectNo_MultiColors = True
                                                    End If
                                                Next
                                            End If
                                        Next
                                    Next
                                End If
                            Next
                        Next
                        If Check_ProspectNo_MultiColors = True Then
                            Exit Function
                        End If
                    Next
                Next

            End If

        Next

        'Debug.Print("----Check_ProspectNo_MultiColors=" & Check_ProspectNo_MultiColors)



    End Function

    '
    '  指定したナンバー（myNo）の強リンク連鎖情報を取得しコレクション配列（myStrongLink）に追加
    '
    Private Sub Create_StrongLinkChain(ByVal myNo As Integer, ByRef myStrongLink() As List(Of Coordinate), _
                                                                 ByVal tmpNumberGrid(,) As SudokuGrid)
        Dim i As Integer, j As Integer
        Dim p As Integer

        p = 0
        ReDim myStrongLink(0)
        myStrongLink(0) = New List(Of Coordinate)
        For j = 1 To GridCount
            For i = 1 To GridCount
                p = p + addStrongLink(New Coordinate(i, j, 0, myNo), myStrongLink, tmpNumberGrid)
            Next
        Next

    End Sub

    Private Function addStrongLink(ByVal myTarget As Coordinate, _
                                   ByRef myStrongLink() As List(Of Coordinate), _
                                   ByVal tmpNumberGrid(,) As SudokuGrid) As Integer
        Dim i As Integer, j As Integer, p As Integer
        Dim sNo As Integer
        Dim cntProspect As Integer
        Dim myCoordinate As New List(Of Coordinate)
        Dim targetLine As New List(Of Integer)
        Dim LinkNo As Integer

        If tmpNumberGrid(myTarget.X, myTarget.Y).ProspectNo.IndexOf(myTarget.No) >= 0 Then
            For i = 1 To 3
                If i = 1 Then     '該当マスの属する列（横列）中の該当Noを候補に持つマス数
                    cntProspect = Count_ProspectNo_On_Group(New Coordinate(0, myTarget.Y, 0, myTarget.No), myCoordinate, tmpNumberGrid)
                ElseIf i = 2 Then '該当マスの属する列（縦列）中の該当Noを候補に持つマス数
                    cntProspect = Count_ProspectNo_On_Group(New Coordinate(myTarget.X, 0, 0, myTarget.No), myCoordinate, tmpNumberGrid)
                ElseIf i = 3 Then '該当マスの属するグループ中の該当Noを候補に持つマス数
                    cntProspect = Count_ProspectNo_On_Group(New Coordinate(0, 0, Get_SquareNo(myTarget.X, myTarget.Y, sNo), myTarget.No), myCoordinate, tmpNumberGrid)
                End If
                If cntProspect = 2 Then  '強リンク＝対象Noを候補に持つマスがグループ内で２つ
                    If Exist_Coordinate(myStrongLink, myCoordinate(0)) = True _
                       And Exist_Coordinate(myStrongLink, myCoordinate(1)) = True Then '両方とも登録済みの時は何もしない

                    Else
                        If Exist_Coordinate(myStrongLink, myCoordinate(0)) = False Then
                            p = 0
                        ElseIf Exist_Coordinate(myStrongLink, myCoordinate(1)) = False Then
                            p = 1
                        End If
                        LinkNo = Get_LinkNo(myStrongLink, myCoordinate(IIf(p = 0, 1, 0)), myCoordinate(p).ColorNo)
                        If UBound(myStrongLink) < LinkNo Then
                            ReDim Preserve myStrongLink(LinkNo)
                            myStrongLink(LinkNo) = New List(Of Coordinate)
                        End If

                        myStrongLink(LinkNo).Add(myCoordinate(p))
                        '次の接続先を探査
                        addStrongLink = addStrongLink + 1 + addStrongLink(myCoordinate(p), myStrongLink, tmpNumberGrid)

                    End If
                End If
            Next
        End If

    End Function


    '
    '　リンク情報リスト(myLink)に指定の座標が既に存在しているかをチェック   
    ''
    Private Function Exist_Coordinate(ByVal myLink() As List(Of Coordinate), _
                                           ByVal targetCoordinate As Coordinate) As Boolean

        Dim i As Integer
        Dim j As Integer

        Exist_Coordinate = False

        For i = 1 To UBound(myLink)
            For j = 0 To myLink(i).Count - 1
                If myLink(i)(j).X = targetCoordinate.X And myLink(i)(j).Y = targetCoordinate.Y Then
                    Exist_Coordinate = True
                    Exit Function
                End If
            Next
        Next

    End Function

    '
    '　既存のリンクに指定の座標がつながるかをチェック   
    '
    Private Function Get_LinkNo(ByVal myLink() As List(Of Coordinate), _
                                ByVal targetCoordinate As Coordinate, _
                                ByRef pairColorNo As Integer) As Integer
        Dim i As Integer
        Dim j As Integer

        For i = 1 To UBound(myLink)
            For j = 0 To myLink(i).Count - 1
                If targetCoordinate.X = myLink(i)(j).X And targetCoordinate.Y = myLink(i)(j).Y Then
                    Get_LinkNo = i
                    pairColorNo = (myLink(i)(j).ColorNo - 1) * -1 + 2  'リンク元が1ならば2、2ならば1
                    Exit Function
                End If
            Next
        Next
        Get_LinkNo = UBound(myLink) + 1 '既存のリンクに含まれない場合、新しいリンクNoを取得
        pairColorNo = 1


    End Function

    '
    '  同一グループ（横・縦列、スクエア）上に該当ナンバーが候補となっているマスがいくつあるかをチェック
    '
    Private Function Count_ProspectNo_On_Group(ByVal myTarget As Coordinate, _
                                               ByRef myPlace As List(Of Coordinate), _
                                               ByVal tmpNumberGrid(,) As SudokuGrid) As Integer
        Dim i As Integer, j As Integer
        Dim sNo As Integer

        Count_ProspectNo_On_Group = 0

        myPlace = New List(Of Coordinate)


        For j = 1 To GridCount
            For i = 1 To GridCount
                If i = myTarget.X Or j = myTarget.Y Or Get_SquareNo(i, j, sNo) = myTarget.S Then
                    If tmpNumberGrid(i, j).ProspectNo.IndexOf(myTarget.No) >= 0 Then
                        Count_ProspectNo_On_Group = Count_ProspectNo_On_Group + 1
                        myPlace.Add(New Coordinate(i, j, 0, myTarget.No))
                    End If
                End If
            Next
        Next

    End Function

    Private Function Remove_ProspectNo(ByVal myRemoveMode As Integer, ByVal myCoordinate As Coordinate, _
                                                              ByRef tmpNumberGrid(,) As SudokuGrid) As Boolean
        Dim i As Integer
        Dim j As Integer
        Dim myS As Integer, mySNo As Integer
        Dim myRemoveGrid As New List(Of Coordinate)

        Remove_ProspectNo = False

        myS = myCoordinate.S

        myRemoveGrid.Clear()

        '対象Noを候補から外すマスをリスト化

        '1.確定マスの同列・同スクエアのマス
        If myRemoveMode = 1 Then
            If myCoordinate.X > 0 And myCoordinate.Y > 0 Then
                myS = Get_SquareNo(myCoordinate.X, myCoordinate.Y, mySNo)
                For j = 1 To GridCount
                    For i = 1 To GridCount
                        If i = myCoordinate.X And j = myCoordinate.Y Then
                            '確定マスの候補Noは当然確定数値のみ
                            tmpNumberGrid(myCoordinate.X, myCoordinate.Y).ProspectNo.Clear()
                            tmpNumberGrid(myCoordinate.X, myCoordinate.Y).ProspectNo.Add(myCoordinate.No)
                        ElseIf i = myCoordinate.X Or j = myCoordinate.Y Or Get_SquareNo(i, j, mySNo) = myS Then
                            myRemoveGrid.Add(New Coordinate(i, j))
                        End If
                    Next
                Next
            End If
        ElseIf myRemoveMode = 2 Then
            '2.指定した列の候補確定スクエア以外のマス
            For j = 1 To GridCount
                For i = 1 To GridCount
                    If (i = myCoordinate.X Or j = myCoordinate.Y) And Get_SquareNo(i, j, mySNo) <> myS Then
                        myRemoveGrid.Add(New Coordinate(i, j))
                    End If
                Next
            Next
        ElseIf myRemoveMode = 3 Then
            '3.指定したスクエアの候補確定列以外のマス
            For j = 1 To GridCount
                For i = 1 To GridCount
                    If Get_SquareNo(i, j, mySNo) = myS And i <> myCoordinate.X And j <> myCoordinate.Y Then
                        myRemoveGrid.Add(New Coordinate(i, j))
                    End If
                Next
            Next
        End If

        For i = 0 To myRemoveGrid.Count - 1
            If tmpNumberGrid(myRemoveGrid(i).X, myRemoveGrid(i).Y).FixNo = 0 Then
                If tmpNumberGrid(myRemoveGrid(i).X, myRemoveGrid(i).Y).ProspectNo.IndexOf(myCoordinate.No) >= 0 Then
                    tmpNumberGrid(myRemoveGrid(i).X, myRemoveGrid(i).Y).ProspectNo.Remove(myCoordinate.No)
                    'Debug.Print("Remove   No." & myCoordinate.No & "  x=" & myRemoveGrid(i).X & "  y=" & myRemoveGrid(i).Y)
                    Remove_ProspectNo = True
                End If
            End If
        Next

        myRemoveGrid.Clear()

    End Function

    Private Sub Reset_Hint()

        Dim i As Integer
        Dim j As Integer

        'ヒント表示でエラー発見時
        If SolveHint.No = 99 Then
            For j = 1 To GridCount
                For i = 1 To GridCount
                    SudokuNumberGrid(i, j).FixError = False
                Next
            Next
        End If
        SolveHint = New Coordinate
        HintTimer.Stop()
        HintFlg = False
        Me.PictureBoxGrid.Invalidate()

        Me.Tool_Hint.Checked = False


    End Sub

    Private Function Count_AroundGrid(ByVal x As Integer, ByVal y As Integer, _
                                      ByVal tmpNumberGrid(,) As SudokuGrid) As Integer

        Dim i As Integer
        Dim j As Integer

        Count_AroundGrid = 0

        If x = 1 Or x = GridCount Then
            Count_AroundGrid = Count_AroundGrid + 1
        End If
        If y = 1 Or y = GridCount Then
            Count_AroundGrid = Count_AroundGrid + 1
        End If

        For j = y - 1 To y + 1
            For i = x - 1 To x + 1
                If j > 0 And j <= GridCount And i > 0 And i <= GridCount _
                   And (x <> i Or j <> y) Then
                    If tmpNumberGrid(i, j).FixNo > 0 Then
                        Count_AroundGrid = Count_AroundGrid + 1
                    End If
                End If
            Next
        Next

    End Function

    Private Sub Rotate_Grid(ByVal cntRotate As Integer, ByRef myNumberGrid(,) As SudokuGrid)

        Dim i As Integer, j As Integer
        Dim x As Integer, y As Integer
        Dim n As Integer
        Dim tmpNumberGrid(,) As SudokuGrid

        ReDim tmpNumberGrid(GridCount, GridCount)

        For n = 1 To cntRotate
            For j = 1 To GridCount
                For i = 1 To GridCount
                    x = (j - (GridCount \ 2 + 1)) * Math.Round(Math.Sin(-1 / 4 * Math.PI) * Math.Sqrt(2)) + GridCount \ 2 + 1
                    y = (i - (GridCount \ 2 + 1)) * Math.Round(Math.Cos(-1 / 4 * Math.PI) * Math.Sqrt(2)) + GridCount \ 2 + 1
                    tmpNumberGrid(x, y) = New SudokuGrid
                    tmpNumberGrid(x, y).Copy(myNumberGrid(i, j))
                Next
            Next
            For j = 1 To GridCount
                For i = 1 To GridCount
                    myNumberGrid(i, j).Copy(tmpNumberGrid(i, j))
                Next
            Next
        Next


    End Sub

    Private Sub Menu_File_Load_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_File_Load.Click, Tool_File_Load.Click

        Dim FilePath As String

        If Check_SavePuzzleData() = False Then
            Exit Sub
        End If

        GridMsg = ""
        Call Reset_Hint()
        Call Reset_AnswerCheck()


        FilePath = Get_FilePath_OpenSave(Me.OpenFileDialog1, "pzn")

        If Len(FilePath) = 0 Then
            Exit Sub
        End If

        Call Load_NumLogicData(FilePath)
        '        System.IO.Directory.SetCurrentDirectory(System.IO.Directory.GetParent(FilePath).FullName())
        System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(FilePath))


    End Sub

    Private Sub Menu_File_Save_Click() Handles Menu_File_Save.Click, Tool_File_Save.Click

        Dim FilePath As String

        If Check_FixAll() = True And AnalyzeMode = False Then
            Exit Sub
        End If


        FilePath = Get_FilePath_OpenSave(Me.SaveFileDialog1, "pzn")

        If Len(FilePath) = 0 Then
            Exit Sub
        End If

        Call Save_NumLogicData(FilePath)
        System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(FilePath))

    End Sub

    Private Sub Menu_SizeInfo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Dim myMenu As ToolStripMenuItem


        For Each myMenu In Me.Menu_Size.DropDownItems
            myMenu.Checked = False
        Next
        sender.Checked = True

        WindowSize = CInt(sender.tag.ToString.Substring(0, 2))
        If Exist_Settings("WindowSize") = True Then
            My.Settings("WindowSize") = WindowSize
        End If


        GridSize = CInt(sender.tag.ToString.Substring(2, 3))
        NumberFnt = New Font("MS UI Gothic", CInt(sender.tag.ToString.Substring(5, 3)), FontStyle.Bold)
        'NumberFnt = New Font("ＭＳ Ｐ明朝", CInt(sender.tag.ToString.Substring(5, 3)), FontStyle.Bold)
        MemoFnt = New Font("MS UI Gothic", CInt(sender.tag.ToString.Substring(8, 3)), FontStyle.Regular)

        If GridCount > 0 Then
            Set_Grid(Me.PictureBoxGrid, Me.PictureBoxMemo, Me.PictureBoxPalette, Me.PictureBoxHighlight)

        End If

    End Sub

    Private Sub Set_Menu_SizeInfo()

        '配列の用意
        Dim SizeInfoArray As String(,) = _
                          { _
                           {"1", "Extra Large", "60", "32", "12"}, _
                           {"2", "Large", "50", "24", "10"}, _
                           {"3", "Middle", "40", "20", "9"}, _
                           {"4", "Small", "35", "16", "8"} _
                          }
        '                           {"5", "極小", "25", "12", "5"} _


        For i As Integer = 0 To SizeInfoArray.GetLength(0) - 1

            Dim menuSize As New ToolStripMenuItem
            Dim e As New System.EventArgs

            menuSize.Text = SizeInfoArray(i, 1)
            menuSize.Tag = Format(CInt(SizeInfoArray(i, 0)), "00") & Format(CInt(SizeInfoArray(i, 2)), "000") & Format(CInt(SizeInfoArray(i, 3)), "000") & Format(CInt(SizeInfoArray(i, 4)), "000")


            If CInt(SizeInfoArray(i, 0)) = WindowSize Then
                menuSize.Checked = True
                Call Menu_SizeInfo_Click(menuSize, e)
            Else
                menuSize.Checked = False
            End If

            AddHandler menuSize.Click, AddressOf Menu_SizeInfo_Click

            Me.Menu_Size.DropDownItems.Add(menuSize)

        Next


    End Sub


    Private Sub Menu_ToolbarChild_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

        ToolboxInfo(CInt(sender.Tag.ToString.Substring(0, 1))).Visible = sender.Checked Xor True
        sender.Checked = sender.Checked Xor True

        Set_Grid(Me.PictureBoxGrid, Me.PictureBoxMemo, Me.PictureBoxPalette, Me.PictureBoxHighlight)


    End Sub

    Private Sub Set_Menu_DisplayItem()

        '配列の用意
        Dim ToolbarArray As String(,) = _
                          { _
                           {1, "Memo", "Check Prospect Number", 7, 1, 0}, _
                           {2, "Palette", "Color Palette", 7, 1, 0}, _
                           {3, "Highlight", "Highlight", 7, 1, 1} _
                          }

        ReDim ToolboxInfo(ToolbarArray.GetLength(0))

        For i As Integer = 0 To ToolbarArray.GetLength(0) - 1

            Dim menuToolbar As New ToolStripMenuItem

            menuToolbar.Name = "Menu_Toolbar_" & ToolbarArray(i, 1)
            menuToolbar.Text = ToolbarArray(i, 2)
            menuToolbar.Tag = CInt(ToolbarArray(i, 0)) & CInt(ToolbarArray(i, 4)) & CInt(ToolbarArray(i, 5))

            menuToolbar.Checked = True 'ToolbarInfo(CInt(ToolbarArray(i, 2))).Visible
            ToolboxInfo(CInt(ToolbarArray(i, 0))).Visible = True

            'menuToolbar.Checked = True 'ToolbarInfo(CInt(ToolbarArray(i, 2))).Visible
            'ToolboxInfo(CInt(ToolbarArray(i, 0))).Visible = True
            ToolboxInfo(CInt(ToolbarArray(i, 0))).Name = ToolbarArray(i, 1)
            ToolboxInfo(CInt(ToolbarArray(i, 0))).Margin = ToolbarArray(i, 3)

            AddHandler menuToolbar.Click, AddressOf Menu_ToolbarChild_Click

            '            Me.Menu_Display.DropDownItems.Add(menuToolbar)

            Me.Menu_Display.DropDownItems.Insert(i + 2, menuToolbar)

        Next


        Me.Menu_Display.DropDownItems.Add(New ToolStripSeparator)

        'Dim menuNumberKeyPad As New ToolStripMenuItem

        'menuNumberKeyPad.Text = "キーパッド"
        'menuNumberKeyPad.Checked = KeypadVisible

        ''        AddHandler menuToolbar.Click, AddressOf Menu_ToolbarChild_Click

        'Me.Menu_Display.DropDownItems.Add(menuNumberKeyPad)




    End Sub

    Private Sub Menu_DisplayKeypad_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_DisplayKeypad.Click

        Me.Menu_DisplayKeypad.Checked = Me.Menu_DisplayKeypad.Checked Xor True
        Call Switch_KeypadDisplay()
        If Exist_Settings("DisplayKeypad") = True Then
            My.Settings("DisplayKeypad") = Me.Menu_DisplayKeypad.Checked
        End If

    End Sub

    Private Sub Switch_ToolButtonEnabled(Optional ByVal skipFlg As Boolean = False)

        Dim flgFilledNum As Boolean

        If skipFlg = True Then
            GoTo SkipF
        End If

        If AnalyzeMode = True Then
            Me.Tool_Hint.Visible = False
            Me.Tool_CheckAnswer.Visible = False
            Me.Menu_Hint.Visible = False
            Me.Menu_CheckAnswer.Visible = False
            Me.Tool_DisplayAnswer.Enabled = True
            Me.Menu_DisplayAnswer.Enabled = True
            Me.Tool_DisplayAnswer.Image = My.Resources.RightAnswerCheck
            Me.Tool_DisplayAnswer.Text = "Check Right Answer"
            Me.Menu_DisplayAnswer.Text = "Check Right Answer"
            Me.Tool_Reset.Visible = False
            Me.Menu_Reset.Visible = False
            Me.Tool_ResetAnswer.Visible = True
            Me.Menu_ResetAnswer.Visible = True
        Else
            Me.Tool_Hint.Visible = True
            Me.Tool_CheckAnswer.Visible = True
            Me.Tool_Reset.Visible = True
            Me.Tool_ResetAnswer.Visible = False
            Me.Menu_Hint.Visible = True
            Me.Menu_CheckAnswer.Visible = True
            Me.Menu_Reset.Visible = True
            Me.Menu_ResetAnswer.Visible = False
            Me.Tool_DisplayAnswer.Image = My.Resources.RightAnswer
            Me.Tool_DisplayAnswer.Text = "Display Answer"
            Me.Menu_DisplayAnswer.Text = "Display Answer"
        End If

SkipF:
        If AnalyzeMode = True Then
            Me.Tool_ResetAnswer.Enabled = Check_FilledNumber()
            Me.Menu_ResetAnswer.Enabled = Check_FilledNumber()
            Me.Tool_File_Save.Enabled = True
            Me.Menu_File_Save.Enabled = True
        Else
            If Check_FixAll() = True Then
                Me.Tool_File_Save.Enabled = False
                Me.Tool_Hint.Enabled = False
                Me.Tool_CheckAnswer.Enabled = False
                Me.Tool_Reset.Enabled = False
                Me.Tool_DisplayAnswer.Enabled = False
                Me.Menu_File_Save.Enabled = False
                Me.Menu_Hint.Enabled = False
                Me.Menu_CheckAnswer.Enabled = False
                Me.Menu_Reset.Enabled = False
                Me.Menu_DisplayAnswer.Enabled = False
            Else

                flgFilledNum = Check_FilledNumber()
                Me.Tool_File_Save.Enabled = True
                Me.Tool_CheckAnswer.Enabled = flgFilledNum
                Me.Tool_Reset.Enabled = flgFilledNum
                Me.Menu_File_Save.Enabled = True
                Me.Menu_CheckAnswer.Enabled = flgFilledNum
                Me.Menu_Reset.Enabled = flgFilledNum
                If CompleteFlg = True Then
                    Me.Tool_Hint.Enabled = False
                    Me.Menu_Hint.Enabled = False
                    Me.Tool_DisplayAnswer.Enabled = False
                    Me.Menu_DisplayAnswer.Enabled = False
                Else
                    Me.Tool_Hint.Enabled = True
                    Me.Menu_Hint.Enabled = True
                    Me.Tool_DisplayAnswer.Enabled = True
                    Me.Menu_DisplayAnswer.Enabled = True
                End If

            End If
        End If


        'Dim btn As ToolStripItem

        'For Each btn In Me.ToolStrip1.Items
        '    If btn.GetType.Name = "ToolStripButton" Then

        '        Debug.Print(btn.Name & ":" & btn.Bounds.X & " " & btn.BackColor.ToString)
        '    End If
        'Next

    End Sub

    Private Sub Switch_KeypadDisplay()

        If Me.Menu_DisplayKeypad.Checked = True Then
            FormNumberKey.Show()
        Else
            FormNumberKey.Close()
        End If

    End Sub

    Private Sub Menu_LevelChild_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Dim myMenu As ToolStripMenuItem
        Dim newLevel As Integer
        Dim ret As Integer

        For Each myMenu In Me.Menu_Level.DropDownItems
            myMenu.Checked = False
        Next
        sender.Checked = True

        newLevel = CInt((sender.tag).ToString.Substring(0, 1))
        If CurrentLevel <> newLevel Then
            CurrentLevel = newLevel 'CInt((sender.tag).ToString.Substring(0, 1))
            If Exist_Settings("CurrentLevel") = True Then
                My.Settings("CurrentLevel") = CurrentLevel
            End If

            CurrentAssignCnt = CInt((sender.tag).ToString.Substring(2, 2))
            If ChangeFlg = True Then
                ret = MsgBox("Quit current puzzle and start new puzzle Level " & CurrentLevel & ". Are you ready?", MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2)
                If ret = vbNo Then
                    Exit Sub
                End If
            End If
            Call Display_NewQuestion()

            CurrentGridX = GridCount \ 2 + GridCount Mod 2
            CurrentGridY = GridCount \ 2 + GridCount Mod 2

            GridMsg = ""
            Call Reset_Hint()
            Set_Grid(Me.PictureBoxGrid, Me.PictureBoxMemo, Me.PictureBoxPalette, Me.PictureBoxHighlight)

        End If


    End Sub

    Private Sub Set_Menu_LevelItem()

        '配列の用意
        Dim LevelArray As String(,) = _
                          { _
                           {1, "Level 1", "40"}, _
                           {2, "Level 2", "30"}, _
                           {3, "Level 3", "24"}, _
                           {4, "Level 4", "25"}, _
                           {5, "Level 5", "25"}, _
                           {6, "Level 6", "25"}, _
                           {7, "Level 7", "25"} _
                          }

        'レベル表示メニューを一旦全て削除
        'Me.Menu_Level.DropDownItems.Clear()

        For i As Integer = 0 To LevelArray.GetLength(0) - 1

            Dim menuLevel As New ToolStripMenuItem

            menuLevel.Text = LevelArray(i, 1)
            menuLevel.Tag = CInt(LevelArray(i, 0)) & "-" & Format(CInt(LevelArray(i, 2)), "00")
            menuLevel.Checked = False 'ToolbarInfo(CInt(ToolbarArray(i, 2))).Visible

            If CInt(LevelArray(i, 0)) = CurrentLevel Then
                menuLevel.Checked = True
                CurrentAssignCnt = CInt(LevelArray(i, 2))
            Else
                menuLevel.Checked = False
            End If

            AddHandler menuLevel.Click, AddressOf Menu_LevelChild_Click

            Me.Menu_Level.DropDownItems.Add(menuLevel)

        Next

    End Sub

    Private Sub Set_Menu_Mode()

        '配列の用意
        Dim ModeArray As String(,) = _
                          { _
                           {"False", "Puzzle Mode"}, _
                           {"True", "Create and Analyze Mode"} _
                          }

        For i As Integer = 0 To ModeArray.GetLength(0) - 1


            Dim menuMode As New ToolStripMenuItem

            menuMode.Text = ModeArray(i, 1)
            menuMode.Tag = ModeArray(i, 0)

            If CBool(ModeArray(i, 0)) = AnalyzeMode Then
                menuMode.Checked = True
            Else
                menuMode.Checked = False
            End If

            AddHandler menuMode.Click, AddressOf Menu_Mode_Click

            Me.Menu_SelectMode.DropDownItems.Add(menuMode)

        Next

    End Sub


    Private Sub Menu_Mode_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

        If AnalyzeMode = CBool(sender.tag) Then
            Exit Sub
        Else
            If Check_SavePuzzleData() = True Then
                AnalyzeMode = True Xor AnalyzeMode
                Call Change_Mode()
            End If
        End If

        'For Each myMenu In Me.Menu_SelectMode.DropDownItems
        '    myMenu.Checked = False
        'Next
        'sender.Checked = True

        'Dim myToolbarMenu As Object 'ToolStripMenuItem

        'For Each myToolbarMenu In Me.Menu_Display.DropDownItems
        '    If myToolbarMenu.Name Like "Menu_Toolbar_*" Then
        '        myToolbarMenu.Enabled = CBool(myToolbarMenu.Tag.ToString.Substring(1 + Math.Abs(CInt(AnalyzeMode)), 1))
        '        myToolbarMenu.Checked = myToolbarMenu.Enabled
        '        ToolboxInfo(myToolbarMenu.Tag.ToString.Substring(0, 1)).Visible = myToolbarMenu.Checked
        '        'Debug.Print("Name:" & myToolbarMenu.Name & CInt(AnalyzeMode) & "  " & myToolbarMenu.Tag)
        '    End If
        'Next

        'If AnalyzeMode = True Then
        '    Call Reset_SudokuGrid()
        'Else
        '    Call Display_NewQuestion()

        'End If

        'Set_Grid(Me.PictureBoxGrid, Me.PictureBoxMemo, Me.PictureBoxPalette, Me.PictureBoxHighlight)


    End Sub

    Private Sub Change_Mode(Optional ByVal ResetFlg As Boolean = True)

        Dim myMenu As ToolStripMenuItem


        For Each myMenu In Me.Menu_SelectMode.DropDownItems
            If AnalyzeMode = CBool(myMenu.Tag) Then
                myMenu.Checked = True
            Else
                myMenu.Checked = False
            End If
        Next

        Dim myToolbarMenu As Object 'ToolStripMenuItem

        For Each myToolbarMenu In Me.Menu_Display.DropDownItems
            If myToolbarMenu.Name Like "Menu_Toolbar_*" Then
                myToolbarMenu.Enabled = CBool(myToolbarMenu.Tag.ToString.Substring(1 + Math.Abs(CInt(AnalyzeMode)), 1))
                myToolbarMenu.Checked = myToolbarMenu.Enabled
                ToolboxInfo(myToolbarMenu.Tag.ToString.Substring(0, 1)).Visible = myToolbarMenu.Checked
            End If
        Next

        Call Switch_ToolButtonEnabled()
        ToolboxInfo(Get_DimNo_From_ToolbarName("Highlight")).SelectedNo = 0

        'MsgBox("ChangeMode")



        '     Call Check_SavePuzzleData()

        If ResetFlg = True Then
            If AnalyzeMode = True Then
                Call Reset_SudokuGrid()
            Else
                Call Display_NewQuestion()
            End If
        End If

        GridMsg = ""
        Call Reset_Hint()



        Set_Grid(Me.PictureBoxGrid, Me.PictureBoxMemo, Me.PictureBoxPalette, Me.PictureBoxHighlight)

    End Sub

    Private Sub Menu_Operation_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_Operation.Click

        System.Diagnostics.Process.Start("http://www.eclip.jp/sheepnumplace/operation.html")

    End Sub

    Private Sub Menu_VersionInfo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_VersionInfo.Click

        FormVersionInfo.Show()

    End Sub

    Private Function Get_PreviousNo(ByVal myCoordinate As Coordinate) As Integer

        Dim i As Integer

        Get_PreviousNo = 0

        For i = currentHistoryNo To 1 Step -1
            'Debug.Print("enterHistory(i).No" & enterHistory(i).No)
            If enterHistory(i).X = myCoordinate.X And enterHistory(i).Y = myCoordinate.Y _
               And enterHistory(i).No <> SudokuNumberGrid(myCoordinate.X, myCoordinate.Y).FixNo Then
                Get_PreviousNo = enterHistory(i).No
                Exit Function
            End If
        Next
        If AnalyzeMode = True Then
            For i = 1 To UBound(enterHistoryB)
                If enterHistoryB(i).X = myCoordinate.X And enterHistoryB(i).Y = myCoordinate.Y _
                   And enterHistoryB(i).No <> SudokuNumberGrid(myCoordinate.X, myCoordinate.Y).FixNo Then
                    Get_PreviousNo = enterHistoryB(i).No
                    Exit Function
                End If
            Next
        End If



    End Function


    '
    '  問題作成時：次に数値を割当てるマスを決める
    '　　　1.１つ前に割当てたNoの点対称位置のマスを優先
    '　　　2.点対称マスが既に割当て済の場合は、現時点で候補No数が最も多いマスを選択（候補数が同じ場合は乱数使用）
    '

    Private Function Assign_NextTaget(ByRef AssignedNo() As Integer, ByVal myNumberGrid(,) As SudokuGrid) As Boolean

        Dim i As Integer, j As Integer, n As Integer
        Dim x1 As Integer, y1 As Integer
        Dim x2 As Integer, y2 As Integer
        Dim p As Integer, sNo As Integer
        Dim preNo As Integer, symmetryNo As Integer
        Dim newNo As Integer
        Dim myNoList As New List(Of Integer)
        Dim cntP As Integer, cntMax As Integer
        Dim myPlace As New List(Of Coordinate)

        Assign_NextTaget = False

        newNo = 0
        If UBound(AssignedNo) > 0 Then
            preNo = AssignedNo(UBound(AssignedNo)) '前回割り当てのマスNo
            symmetryNo = GridCount * GridCount - preNo + 1  '前回割り当てのマスと点対称位置にあるマス
            newNo = symmetryNo
            For i = 1 To UBound(AssignedNo)
                If AssignedNo(i) = symmetryNo Then  '既に割当て済
                    newNo = 0
                    Exit For
                End If
            Next
        End If

        If newNo = 0 Then
            p = GridCount * 2 '候補No数の初期値
            Do While p >= 2
                For y1 = 1 To GridCount \ 2 + 1
                    For x1 = 1 To IIf(y1 = 5, GridCount \ 2 + 1, GridCount)
                        x2 = GridCount - x1 + 1
                        y2 = GridCount - y1 + 1
                        If myNumberGrid(x1, y1).FixNo = 0 And myNumberGrid(x2, y2).FixNo = 0 Then
                            If myNumberGrid(x1, y1).ProspectNo.Count + myNumberGrid(x2, y2).ProspectNo.Count = p Then
                                myNoList.Add((y1 - 1) * GridCount + x1)
                                myNoList.Add((y2 - 1) * GridCount + x2)
                            End If
                        End If
                    Next
                Next
                'Debug.Print("myNoList.Count:" & myNoList.Count)
                If myNoList.Count > 0 Then
                    newNo = Generate_Random(myNoList)
                    Exit Do
                End If
                p = p - 1
            Loop
        End If

        If newNo > 0 Then
            ReDim Preserve AssignedNo(UBound(AssignedNo) + 1)
            AssignedNo(UBound(AssignedNo)) = newNo
            Assign_NextTaget = True
        End If

    End Function

    Private Sub Replace_GridPosition(ByRef myNumberGrid(,) As SudokuGrid)

        Dim x As Integer, y As Integer
        Dim i As Integer, j As Integer
        Dim tmpNumberGrid(,) As SudokuGrid
        Dim n As Integer, s As Integer, p As Integer
        Dim newX As List(Of Integer)
        Dim newY As List(Of Integer)
        Dim newXs As List(Of Integer)
        Dim newYs As List(Of Integer)
        Dim newNo As List(Of Integer)
        Dim cntRotate As Integer
        Dim strTxt As String

        ReDim tmpNumberGrid(GridCount, GridCount)

        newX = New List(Of Integer)
        newY = New List(Of Integer)
        newXs = New List(Of Integer)
        newYs = New List(Of Integer)
        newNo = New List(Of Integer)

        s = Int(Math.Sqrt(GridCount))

        newXs.Add(5)
        newYs.Add(5)
        For i = 1 To GridCount \ 2
            '            Debug.Print(((i - 1) \ s) * s + 1 & "---" & ((i - 1) \ s) * s + s)
            x = Generate_RandomRange(((i - 1) \ s) * s + 1, ((i - 1) \ s) * s + s, newXs)
            y = Generate_RandomRange(((i - 1) \ s) * s + 1, ((i - 1) \ s) * s + s, newYs)
            newX.Add(x)
            newY.Add(y)
            newXs.Add(x)
            newYs.Add(y)
        Next
        newX.Add(5)
        newY.Add(5)
        For i = newXs.Count - 1 To 1 Step -1
            newX.Add(GridCount - newXs(i) + 1)
            newY.Add(GridCount - newYs(i) + 1)
        Next

        strTxt = ""
        For i = 0 To newX.Count - 1
            strTxt = strTxt & newX(i) & ","
        Next
        'Debug.Print("X:" & strTxt)

        strTxt = ""
        For i = 0 To newY.Count - 1
            strTxt = strTxt & newY(i) & ","
        Next
        'Debug.Print("Y:" & strTxt)

        For i = 1 To GridCount
            newNo.Add(Generate_RandomRange(1, GridCount, newNo))
        Next

        strTxt = ""
        For i = 0 To newNo.Count - 1
            strTxt = strTxt & newNo(i) & "-"
        Next
        'Debug.Print(strTxt)


        For j = 0 To newY.Count - 1
            For i = 0 To newX.Count - 1
                tmpNumberGrid(newX(i), newY(j)) = New SudokuGrid
                tmpNumberGrid(newX(i), newY(j)).Copy(myNumberGrid(i + 1, j + 1))
            Next
        Next

        For j = 1 To GridCount
            For i = 1 To GridCount
                myNumberGrid(i, j).Copy(tmpNumberGrid(i, j))
                If myNumberGrid(i, j).FixNo > 0 Then
                    myNumberGrid(i, j).FixNo = newNo(myNumberGrid(i, j).FixNo - 1)
                End If
            Next
        Next
        Call Adjust_ProspectNo(myNumberGrid)

        cntRotate = Generate_RandomRange(0, 3)
        Call Rotate_Grid(cntRotate, myNumberGrid)



    End Sub

    Private Sub FormMain_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles Me.KeyPress

        Dim myKeyNo As Integer

        If CurrentGridX > 0 And CurrentGridX <= GridCount _
           And CurrentGridY > 0 And CurrentGridY <= GridCount Then
            If SudokuNumberGrid(CurrentGridX, CurrentGridY).Locked = False Or AnalyzeMode = True Then
                If IsNumeric(e.KeyChar) = True Then
                    myKeyNo = CInt(e.KeyChar.ToString)
                    If myKeyNo > 0 And myKeyNo <= GridCount Then
                        Call Input_Number(myKeyNo)
                    End If
                End If
            End If
        End If

    End Sub


    Private Sub FormMain_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown

        Dim dx As Integer = 0, dy As Integer = 0


        If CurrentGridX > 0 And CurrentGridX <= GridCount _
           And CurrentGridY > 0 And CurrentGridY <= GridCount Then
            Select Case e.KeyCode
                'Case Keys.Alt
                '    Debug.Print("alt")
                Case Keys.Up
                    If CurrentGridY > 1 Then
                        dy = -1
                    End If
                Case Keys.Left
                    If CurrentGridX > 1 Then
                        dx = -1
                    End If
                Case Keys.Down
                    If CurrentGridY < GridCount Then
                        dy = 1
                    End If
                Case Keys.Right
                    If CurrentGridX < GridCount Then
                        dx = 1
                    End If
                Case Keys.Back, Keys.Escape, Keys.Delete
                    If SudokuNumberGrid(CurrentGridX, CurrentGridY).Locked = False Or AnalyzeMode = True Then
                        Call Input_Number(0)
                    End If
                Case Keys.F1 To Keys.F9
                    Call Switch_Highlight(Mid(e.KeyCode.ToString, 2, 1))
                Case Keys.P
                    If Me.Chk_DisplayProspect.Visible = True Then
                        Me.Chk_DisplayProspect.Checked = Me.Chk_DisplayProspect.Checked Xor True
                    End If
                Case Keys.S
                    If Me.Chk_SymmetricGrid.Visible = True Then
                        Me.Chk_SymmetricGrid.Checked = Me.Chk_SymmetricGrid.Checked Xor True
                    End If
                Case Keys.Tab
                    If FormNumberKey.Visible = True Then
                        FormNumberKey.Select()
                    End If
            End Select
        Else
            If e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down Or e.KeyCode = Keys.Left Or e.KeyCode = Keys.Right Then
                dx = GridCount \ 2 + GridCount Mod 2 - CurrentGridX
                dy = GridCount \ 2 + GridCount Mod 2 - CurrentGridY
            End If
        End If

        If Math.Abs(dx) > 0 Or Math.Abs(dy) > 0 Then
            CurrentGridX = CurrentGridX + dx
            CurrentGridY = CurrentGridY + dy
            GridMsg = ""
            Call Reset_Hint()
            Call Reset_AnswerCheck()
            Me.PictureBoxGrid.Invalidate()
            Me.PictureBoxMemo.Invalidate()
        End If



    End Sub

    Private Sub FormMain_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseWheel

        Dim myPich As Integer
        Dim myNo As Integer
        Dim x As Integer = Me.PictureBoxGrid.PointToClient(System.Windows.Forms.Cursor.Position).X
        Dim y As Integer = Me.PictureBoxGrid.PointToClient(System.Windows.Forms.Cursor.Position).Y
        Dim myX As Integer, myY As Integer

        Call Get_SudokuGridXY_From_Coordinate(x, y, myX, myY)

        If myX = CurrentGridX And myY = CurrentGridY _
           And CurrentGridX > 0 And CurrentGridX <= GridCount _
           And CurrentGridY > 0 And CurrentGridY <= GridCount Then
            If SudokuNumberGrid(CurrentGridX, CurrentGridY).Locked = False Or AnalyzeMode = True Then
                If e.Delta > 0 Then
                    myPich = -1
                Else
                    myPich = 1
                End If
                myNo = SudokuNumberGrid(CurrentGridX, CurrentGridY).FixNo + myPich
                If myNo < 0 Then
                    myNo = myNo + (GridCount + 1)
                ElseIf myNo > GridCount Then
                    myNo = myNo - (GridCount + 1)
                End If
                Call Input_Number(myNo)
            End If
        End If

    End Sub

    Protected Friend Sub Input_Number(ByVal myKeyNo As Integer)

        SudokuNumberGrid(CurrentGridX, CurrentGridY).FixNo = myKeyNo
        SudokuNumberGrid(CurrentGridX, CurrentGridY).FixError = False

        If SudokuNumberGrid(CurrentGridX, CurrentGridY).BackColor = Color.LightSteelBlue Then
            SudokuNumberGrid(CurrentGridX, CurrentGridY).BackColor = Color.White
        End If


        If myKeyNo = 0 Then
            SudokuNumberGrid(CurrentGridX, CurrentGridY).Locked = False
        Else
            SudokuNumberGrid(CurrentGridX, CurrentGridY).Locked = AnalyzeMode
        End If
        Call Adjust_ProspectNo(SudokuNumberGrid)
        Call Add_enterHistory()
        Call Reset_Hint()
        Call Reset_AnswerCheck()

        If AnalyzeMode = False Then
            Call Display_CompleteWindow()
        Else
            Call DuplicateNumber(SudokuNumberGrid, New Coordinate(CurrentGridX, CurrentGridY))
        End If

        Me.PictureBoxGrid.Invalidate()

    End Sub

    Private Sub Reset_AnswerCheck()

        Dim i As Integer
        Dim j As Integer

        CheckAnswerFlg = False
        For j = 1 To GridCount
            For i = 1 To GridCount
                SudokuNumberGrid(i, j).FixError = False
            Next
        Next
        '        Me.Tool_CheckAnswer.BackColor = Color.White
        Me.Tool_CheckAnswer.Checked = False


    End Sub

    Private Sub Add_enterHistory()

        If enterHistory(currentHistoryNo).X <> CurrentGridX Or enterHistory(currentHistoryNo).Y <> CurrentGridY _
                         Or enterHistory(currentHistoryNo).NoB = 0 Then
            currentHistoryNo = currentHistoryNo + 1
            ReDim Preserve enterHistory(currentHistoryNo)
            'Me.Chk_Change.Checked = True
            ChangeFlg = True
        End If
        enterHistory(currentHistoryNo) = New Coordinate
        enterHistory(currentHistoryNo).X = CurrentGridX
        enterHistory(currentHistoryNo).Y = CurrentGridY
        enterHistory(currentHistoryNo).No = SudokuNumberGrid(CurrentGridX, CurrentGridY).FixNo
        enterHistory(currentHistoryNo).NoB = 1

    End Sub

    Private Sub HintTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HintTimer.Tick

        If SolveHint.NoB = 0 Then
            SolveHint.NoB = SolveHint.No
        Else
            SolveHint.NoB = 0
        End If

        Me.PictureBoxGrid.Invalidate()

        HintTimer.Tag = CInt(HintTimer.Tag.ToString) + 1
        If HintTimer.Tag >= 4 Then
            HintTimer.Stop()
        End If


    End Sub

    Private Sub GradientTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GradientTimer.Tick

        Me.GradientTimer.Tag = CInt(Me.GradientTimer.Tag) + 1

        CompleteGrd = CompleteGrd + 0.02
        If CompleteGrd > 1 Then
            CompleteGrd = 1
        End If

        Me.PictureBoxGrid.Invalidate()

        If CompleteGrd >= 1 Then

            For j = 1 To GridCount
                For i = 1 To GridCount
                    SudokuNumberGrid(i, j).BackColor = Color.Transparent
                    If SudokuNumberGrid(i, j).ForeColor <> Color.Black Then
                        SudokuNumberGrid(i, j).ForeColor = Color.DeepPink
                    End If
                Next
            Next


            Me.GradientTimer.Stop()
        End If

    End Sub

    Private Sub Initialize_SudokuNumberGrid(ByRef tmpNumberGrid(,) As SudokuGrid)

        Dim i As Integer
        Dim j As Integer

        ReDim tmpNumberGrid(GridCount, GridCount)

        For j = 1 To GridCount
            For i = 1 To GridCount
                tmpNumberGrid(i, j) = New SudokuGrid
            Next
        Next

    End Sub

    Private Function Get_Color_From_No(ByVal myNo As Integer) As Color

        Get_Color_From_No = Color.White
        If PaletteColor.Length - 1 >= myNo Then
            Get_Color_From_No = PaletteColor(myNo)
        End If

    End Function

    Private Function Get_ColorNo(ByVal myColor As Color) As Integer

        Dim i As Integer

        Get_ColorNo = 0

        For i = 0 To PaletteColor.Length - 1
            If PaletteColor(i) = myColor Then
                Get_ColorNo = i
                Exit Function
            End If
        Next

    End Function


    Private Sub Get_MinimumAnser(ByVal myPlace As Coordinate, ByVal answerNumberGrid(,,) As SudokuGrid, _
                                    ByRef MinAnswer() As Integer, ByRef CntAnswer As Integer)
        Dim i As Integer
        Dim j As Integer
        Dim n As Integer, s As Integer, p As Integer
        Dim x1 As Integer, x2 As Integer
        Dim y1 As Integer, y2 As Integer
        Dim minCnt As Integer
        Dim ansCnt(,) As Integer

        ReDim MinAnswer(2)
        ReDim ansCnt(GridCount, GridCount)
        x1 = myPlace.X
        y1 = myPlace.Y
        x2 = GridCount - x1 + 1
        y2 = GridCount - y1 + 1


        For s = 1 To UBound(answerNumberGrid, 3)
            ansCnt(answerNumberGrid(x1, y1, s).FixNo, answerNumberGrid(x2, y2, s).FixNo) _
                  = ansCnt(answerNumberGrid(x1, y1, s).FixNo, answerNumberGrid(x2, y2, s).FixNo) + 1
        Next

        minCnt = UBound(answerNumberGrid, 3)
        For j = 1 To GridCount
            For i = 1 To GridCount
                If ansCnt(i, j) > 0 And ansCnt(i, j) < minCnt Then
                    minCnt = ansCnt(i, j)
                    MinAnswer(1) = i
                    MinAnswer(2) = j
                End If
            Next
        Next
        CntAnswer = minCnt



    End Sub


    Private Sub Get_MinimumAnserPair(ByVal answerNumberGrid(,,) As SudokuGrid, ByVal tmpNumberGrid(,) As SudokuGrid, _
                    ByRef myPlace() As Coordinate, ByRef minCntAnswer As Integer)
        Dim i As Integer
        Dim j As Integer
        Dim n As Integer, s As Integer, p As Integer
        Dim x1 As Integer, x2 As Integer
        Dim y1 As Integer, y2 As Integer
        Dim cntAroundGrid As Integer
        Dim ans1 As Integer, ans2 As Integer
        'Dim minCntAnswer As Integer
        Dim minCntAround As Integer
        Dim ansCnt(,) As Integer

        ReDim myPlace(2)

        minCntAnswer = UBound(answerNumberGrid, 3)
        minCntAround = 8

        For y1 = 1 To GridCount \ 2 + 1
            For x1 = 1 To GridCount
                x2 = GridCount - x1 + 1
                y2 = GridCount - y1 + 1
                cntAroundGrid = Count_AroundGrid(x1, y1, tmpNumberGrid)
                If tmpNumberGrid(x1, y1).FixNo = 0 _
                   And tmpNumberGrid(x2, y2).FixNo = 0 And (x1 <> x2 Or y1 <> y2) Then
                    ReDim ansCnt(GridCount, GridCount)
                    For s = 1 To UBound(answerNumberGrid, 3)
                        ansCnt(answerNumberGrid(x1, y1, s).FixNo, answerNumberGrid(x2, y2, s).FixNo) _
                              = ansCnt(answerNumberGrid(x1, y1, s).FixNo, answerNumberGrid(x2, y2, s).FixNo) + 1
                    Next
                    For ans2 = 1 To GridCount
                        For ans1 = 1 To GridCount
                            If ansCnt(ans1, ans2) > 0 Then
                                If ansCnt(ans1, ans2) < minCntAnswer _
                                   Or (ansCnt(ans1, ans2) = minCntAnswer And cntAroundGrid <= minCntAround) Then
                                    minCntAnswer = ansCnt(ans1, ans2)
                                    myPlace(1) = New Coordinate(x1, y1, 0, ans1)
                                    myPlace(2) = New Coordinate(x2, y2, 0, ans2)
                                    If cntAroundGrid < minCntAround Then
                                        minCntAround = cntAroundGrid
                                    End If
                                End If
                            End If
                        Next
                    Next
                End If
            Next
        Next

    End Sub

    Private Function Check_SavePuzzleData() As Boolean

        Dim ret As Integer

        Check_SavePuzzleData = False

        If ChangeFlg = True Then

            ret = MsgBox("Do you save current puzzle?", MsgBoxStyle.YesNoCancel)
            If ret = vbCancel Then
                Exit Function
            ElseIf ret = vbYes Then
                Call Menu_File_Save_Click()
            End If
        End If

        Check_SavePuzzleData = True

    End Function


    Private Sub Display_NewQuestion()

        Dim cnt As Integer
        Dim strAsterisk As String = ""
        Dim tNo As Integer
        Dim ret As Integer
        Dim myLevel As Integer

        Call Reset_History()

        SolveHint = New Coordinate

        tNo = Get_DimNo_From_ToolbarName("Highlight")
        ToolboxInfo(tNo).SelectedNo = 0

        CompleteFlg = False

        myLevel = CurrentLevel

        '-------------------------------------------------------------------------------------------
        'Dim i As Integer, p As Integer
        'Dim x As Integer, y As Integer

        'For i = 1 To 100
        '    p = 0
        '    Call Create_NewQuestion(CurrentAssignCnt, cnt)
        '    For y = 1 To GridCount
        '        For x = 1 To GridCount
        '            If SudokuNumberGrid(x, y).FixNo = 0 And SudokuNumberGrid(x, y).Locked = True Then
        '                p = p + 1
        '            End If
        '        Next
        '    Next
        '    Debug.Print("p=" & p)
        '    Set_Grid(Me.PictureBoxGrid, Me.PictureBoxMemo, Me.PictureBoxPalette, Me.PictureBoxHighlight)
        '    System.Windows.Forms.Application.DoEvents()
        'Next
        'Exit Sub
        '-------------------------------------------------------------------------------------------

        If myLevel >= 3 Then
            Call Read_PzlData(myLevel)
        Else
            Call Create_NewQuestion(CurrentAssignCnt, cnt)
        End If
        Set_Grid(Me.PictureBoxGrid, Me.PictureBoxMemo, Me.PictureBoxPalette, Me.PictureBoxHighlight)

        Me.LblLevel.Text = "Level " & myLevel 'strAsterisk.PadLeft(myLevel, "")
        Me.LblLevel.Tag = myLevel

        'Me.Chk_Change.Checked = False
        ChangeFlg = False

    End Sub

    Private Function Get_QuestionCount(ByVal myLv As Integer) As Integer

        '        Dim txtLines As String() = System.IO.File.ReadAllLines(FilePath, enc)
        Dim txtAll As String = My.Resources.ResourceManager.GetObject("NumLgLv" & myLv)
        Dim txtLines As String() = Split(txtAll, vbCrLf)

        Get_QuestionCount = 0

        If IsNumeric(txtLines(0)) = True Then
            Get_QuestionCount = CInt(txtLines(0))
        End If

    End Function

    Private Function Read_PzlData(ByVal myLv As Integer) As Boolean

        Dim qCount As Integer
        Dim myNo As Integer
        Dim myNg As Boolean
        Dim myLevel As Integer
        Dim myList As New SortedList

        Read_PzlData = False

F_Loop:

        qCount = Get_QuestionCount(myLv)
        If qCount > 0 Then
            myNo = Generate_RandomRange(1, qCount)
            Call Load_NumLogicStock(myLv, myNo)
        End If

        'チェック用
        Dim FixCnt As Integer
        Dim i As Integer, j As Integer
        Dim tmpNumberGrid(,) As SudokuGrid

        ReDim tmpNumberGrid(GridCount, GridCount)

        For j = 1 To GridCount
            For i = 1 To GridCount
                tmpNumberGrid(i, j) = New SudokuGrid
                tmpNumberGrid(i, j).Copy(SudokuNumberGrid(i, j))
            Next
        Next

        Call Solve_Sudoku(1, myNg, tmpNumberGrid, myLevel)
        FixCnt = 0
        For j = 1 To GridCount
            For i = 1 To GridCount
                If SudokuNumberGrid(i, j).FixNo > 0 Then
                    FixCnt = FixCnt + 1
                End If
            Next
        Next

        Read_PzlData = True

    End Function

    Private Sub Chk_DisplayProspect_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Chk_DisplayProspect.CheckedChanged
        Me.PictureBoxGrid.Invalidate()
    End Sub

    Private Sub Chk_SymmetricGrid_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Chk_SymmetricGrid.CheckedChanged
        Me.PictureBoxGrid.Invalidate()
    End Sub


    Private Function Check_FixAll() As Boolean

        Dim i As Integer
        Dim j As Integer

        Check_FixAll = False

        For j = 1 To GridCount
            For i = 1 To GridCount
                If SudokuNumberGrid(i, j).Locked = False Then
                    Exit Function
                End If
            Next
        Next

        Check_FixAll = True


    End Function

    Private Function Check_FilledNumber() As Boolean

        Dim i As Integer
        Dim j As Integer

        Check_FilledNumber = False

        For j = 1 To GridCount
            For i = 1 To GridCount
                If SudokuNumberGrid(i, j).FixNo > 0 And SudokuNumberGrid(i, j).Locked = False Then
                    Check_FilledNumber = True
                    Exit Function
                End If
            Next
        Next

    End Function


    Private Sub Btn_Previous_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Previous.Click

        If Me.Btn_Previous.Image.Tag = "False" Then
            Exit Sub
        End If

        SudokuNumberGrid(enterHistory(currentHistoryNo).X, enterHistory(currentHistoryNo).Y).FixNo _
              = Get_PreviousNo(enterHistory(currentHistoryNo))
        If SudokuNumberGrid(enterHistory(currentHistoryNo).X, enterHistory(currentHistoryNo).Y).FixNo > 0 Then
            SudokuNumberGrid(enterHistory(currentHistoryNo).X, enterHistory(currentHistoryNo).Y).Locked = AnalyzeMode
        Else
            SudokuNumberGrid(enterHistory(currentHistoryNo).X, enterHistory(currentHistoryNo).Y).Locked = False
        End If
        If currentHistoryNo > 0 Then
            currentHistoryNo = currentHistoryNo - 1
        End If
        If AnalyzeMode = True Then
            Call Tool_Reset_Click(sender, e)
        End If
        Call Reset_Hint()
        Call Reset_AnswerCheck()
        Call Adjust_ProspectNo(SudokuNumberGrid)
        Me.PictureBoxGrid.Invalidate()

    End Sub

    Private Sub Btn_Next_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Next.Click


        If Me.Btn_Next.Image.Tag = "False" Then
            Exit Sub
        End If

        If currentHistoryNo < UBound(enterHistory) Then
            currentHistoryNo = currentHistoryNo + 1
            SudokuNumberGrid(enterHistory(currentHistoryNo).X, enterHistory(currentHistoryNo).Y).FixNo _
               = enterHistory(currentHistoryNo).No
            If SudokuNumberGrid(enterHistory(currentHistoryNo).X, enterHistory(currentHistoryNo).Y).FixNo > 0 Then
                SudokuNumberGrid(enterHistory(currentHistoryNo).X, enterHistory(currentHistoryNo).Y).Locked = AnalyzeMode
            Else
                SudokuNumberGrid(enterHistory(currentHistoryNo).X, enterHistory(currentHistoryNo).Y).Locked = False
            End If

            If AnalyzeMode = True Then
                Call Tool_Reset_Click(sender, e)
            End If
            Call Reset_Hint()
            Call Reset_AnswerCheck()
            Call Adjust_ProspectNo(SudokuNumberGrid)
            Me.PictureBoxGrid.Invalidate()
        End If

    End Sub


    Private Sub Tool_Hint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tool_Hint.Click, Menu_Hint.Click

        Dim hint As New Coordinate
        Dim myNg As Boolean
        Dim myNumberGrid(,) As SudokuGrid
        Dim intRest As Integer
        Dim n As Integer, nn As Integer
        Dim tmpSudokuNumberGrid(,) As SudokuGrid
        Dim i As Integer, j As Integer

        ReDim tmpSudokuNumberGrid(GridCount, GridCount)

        ReDim myNumberGrid(GridCount, GridCount)

        If HintFlg = True Then
            Reset_Hint()
        Else
            HintFlg = True
            Me.Tool_Hint.Checked = True

            For j = 1 To GridCount
                For i = 1 To GridCount
                    myNumberGrid(i, j) = New SudokuGrid
                    myNumberGrid(i, j).Copy(SudokuNumberGrid(i, j))
                Next
            Next

            CheckAnswerFlg = False

            '        If Solve_Sudoku(2, myNg, SudokuNumberGrid, 0, SolveHint) = 0 Then '間違いなし
            intRest = Solve_Sudoku(2, myNg, SudokuNumberGrid, 0, SolveHint)
            If intRest = 0 Then '間違いなし
                SolveHint.NoB = SolveHint.No
            Else 'If myNg = False Then
                Call Chk_Hint_BackTrack()
            End If

            Me.PictureBoxGrid.Invalidate()

            HintTimer.Tag = 0
            HintTimer.Start()

        End If

    End Sub

    Private Sub myRenderer_RenderGradation(ByVal sender As Object, ByVal e As ToolStripRenderEventArgs)
        Dim br As New Drawing2D.LinearGradientBrush(New Point(0, 0), New Point(0, e.AffectedBounds.Bottom), _
                                                    Color.LightGray, Color.Gainsboro)

        e.Graphics.FillRectangle(br, e.AffectedBounds)
        br.Dispose()
    End Sub

    Private Sub Tool_CheckAnswer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tool_CheckAnswer.Click, Menu_CheckAnswer.Click

        Dim i As Integer
        Dim j As Integer
        Dim restGrid As Integer
        Dim restGridL As Integer
        Dim n As Integer, nn As Integer
        Dim myNg As Boolean, myNgL As Boolean
        Dim myLevel As Integer
        Dim myMode As Integer
        Dim tmpSudokuNumberGrid(,) As SudokuGrid
        Dim tmpSudokuNumberGridL(,) As SudokuGrid
        Dim answerNumberGrid(,,) As SudokuGrid
        Dim myCoordinate As Coordinate

        ReDim tmpSudokuNumberGrid(GridCount, GridCount)
        ReDim tmpSudokuNumberGridL(GridCount, GridCount)


        If CheckAnswerFlg = True Then
            Reset_AnswerCheck()
        Else
            '            Me.Tool_CheckAnswer.BackColor = Color.Red
            Me.Tool_CheckAnswer.Checked = True


            For j = 1 To GridCount
                For i = 1 To GridCount
                    tmpSudokuNumberGrid(i, j) = New SudokuGrid
                    tmpSudokuNumberGrid(i, j).Copy(SudokuNumberGrid(i, j))
                Next
            Next

            myMode = 1

            restGrid = Solve_Sudoku(myMode, myNg, tmpSudokuNumberGrid, myLevel)

            n = 0
            If restGrid > 0 Then
                For j = 1 To GridCount
                    For i = 1 To GridCount
                        tmpSudokuNumberGridL(i, j) = New SudokuGrid
                        If SudokuNumberGrid(i, j).Locked = True Then
                            tmpSudokuNumberGridL(i, j).Copy(SudokuNumberGrid(i, j))
                            n = n + 1
                        End If
                    Next
                Next
                restGridL = Solve_Sudoku(myMode, myNgL, tmpSudokuNumberGridL, myLevel)
                If restGridL = 0 Then
                    '                nn = 0
                Else
                    nn = Solve_SudokuBackTrack(New Coordinate, tmpSudokuNumberGridL, answerNumberGrid, 1, 5)
                    '               MsgBox(nn & "   n=" & n)
                    If nn > 1 Then
                        Exit Sub
                    Else
                        For j = 1 To GridCount
                            For i = 1 To GridCount
                                If SudokuNumberGrid(i, j).FixNo > 0 _
                                   And SudokuNumberGrid(i, j).FixNo <> answerNumberGrid(i, j, 1).FixNo Then
                                    tmpSudokuNumberGrid(i, j).FixError = True
                                    '                                MsgBox("NG x=" & i & "   y=" & j)
                                Else
                                    '                               Debug.Print(" x=" & i & "   y=" & j & "    no=" & tmpSudokuNumberGrid(i, j).FixNo)
                                End If
                            Next
                        Next
                    End If

                End If
                'End If
            Else
                nn = 1
            End If

            For j = 1 To GridCount
                For i = 1 To GridCount
                    SudokuNumberGrid(i, j).FixError = tmpSudokuNumberGrid(i, j).FixError
                Next
            Next

            '            Me.PictureBoxGrid.Invalidate()

            CheckAnswerFlg = True

            'Me.Tool_CheckAnswer.BackColor = Color.Black

        End If

        Me.PictureBoxGrid.Invalidate()


    End Sub

    Private Sub Tool_DisplayAnswer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tool_DisplayAnswer.Click, Menu_DisplayAnswer.Click

        Dim i As Integer
        Dim j As Integer
        Dim restGrid As Integer
        Dim n As Integer, nn As Integer
        Dim cntMulti As Integer
        Dim myNg As Boolean
        Dim myLevel As Integer
        Dim myMode As Integer
        Dim tmpSudokuNumberGrid(,) As SudokuGrid
        Dim answerNumberGrid(,,) As SudokuGrid
        Dim ret As Integer

        ReDim tmpSudokuNumberGrid(GridCount, GridCount)


        If Check_FixAll() = True Then
            Exit Sub
        End If


        If AnalyzeMode = True Then
            myMode = 4
        Else
            myMode = 1
            ret = MsgBox("Quit current puzzle and display right answer. Are you OK ?", MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2)
            If ret = vbNo Then
                Exit Sub
            End If
        End If

        'p = 0
        For j = 1 To GridCount
            For i = 1 To GridCount
                tmpSudokuNumberGrid(i, j) = New SudokuGrid
                If SudokuNumberGrid(i, j).Locked = True Then
                    tmpSudokuNumberGrid(i, j).Copy(SudokuNumberGrid(i, j))
                End If
            Next
        Next



        restGrid = Solve_Sudoku(myMode, myNg, tmpSudokuNumberGrid, myLevel)
        For j = 1 To GridCount
            For i = 1 To GridCount
                If AnalyzeMode = False Then
                    If SudokuNumberGrid(i, j).FixNo <> tmpSudokuNumberGrid(i, j).FixNo And tmpSudokuNumberGrid(i, j).FixNo > 0 Then
                        tmpSudokuNumberGrid(i, j).ForeColor = Color.Salmon
                        'tmpSudokuNumberGrid(i, j).BackColor = Color.White
                    ElseIf SudokuNumberGrid(i, j).FixNo > 0 And SudokuNumberGrid(i, j).Locked = False Then
                        tmpSudokuNumberGrid(i, j).ForeColor = Color.Blue


                    End If

                End If
                If tmpSudokuNumberGrid(i, j).FixNo > 0 Then
                    SudokuNumberGrid(i, j).Copy(tmpSudokuNumberGrid(i, j))
                    SudokuNumberGrid(i, j).BackColor = Color.White
                    If AnalyzeMode = False Then
                        SudokuNumberGrid(i, j).Locked = True
                    End If

                End If
            Next
        Next

        'Dim RstFlg As Boolean

FFF:
        If restGrid > 0 Then
            If myNg = False Then

                cntMulti = 29
                nn = Solve_SudokuBackTrack(New Coordinate, tmpSudokuNumberGrid, answerNumberGrid, 1, cntMulti)
                If nn > 0 Then
                    If cntMulti > 0 Then
                        For j = 1 To GridCount
                            For i = 1 To GridCount
                                If AnalyzeMode = False And SudokuNumberGrid(i, j).FixNo <> answerNumberGrid(i, j, 1).FixNo Then
                                    'Debug.Print("InputNO:  x=" & i & "   y=" & j & "   No=" & SudokuNumberGrid(i, j).FixNo)
                                    answerNumberGrid(i, j, 1).ForeColor = Color.Salmon
                                End If
                                SudokuNumberGrid(i, j).Copy(answerNumberGrid(i, j, 1))
                                If AnalyzeMode = False Then
                                    SudokuNumberGrid(i, j).Locked = True
                                End If
                            Next
                        Next
                        If nn > 1 Then
                            '                        MsgBox("解は" & nn & "個" & "  バックトラックネストレベル=" & nestLevMax & "   " & cntMulti)
                            GridMsg = "There are " & nn & " solutions."
                            For n = 2 To nn
                                For j = 1 To GridCount
                                    For i = 1 To GridCount
                                        If answerNumberGrid(i, j, n).FixNo <> answerNumberGrid(i, j, 1).FixNo Then
                                            SudokuNumberGrid(i, j).BackColor = Color.LightSteelBlue
                                        End If
                                    Next
                                Next
                            Next
                        End If
                    Else
                        'MsgBox("解は" & nn & "個以上存在します")
                        GridMsg = "There are more than " & nn & " solutions."

                    End If
                Else
                    GridMsg = "There is no solution."
                End If
            Else
                GridMsg = "There is no solution."
            End If
        Else
            nn = 1
        End If

        If AnalyzeMode = False Then
            Call Reset_History()
        End If

        Me.PictureBoxGrid.Invalidate()


    End Sub

    Private Sub Tool_NewQuestion_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tool_NewQuestion.Click, Menu_NewQuestion.Click

        Dim ret As Integer


        If AnalyzeMode = True Then

            If Check_SavePuzzleData() = False Then
                Exit Sub
            End If
            Call Reset_SudokuGrid()
        Else
            '            If Me.Chk_Change.Checked = True Then
            If ChangeFlg = True Then
                ret = MsgBox("Quit current puzzle and start new puzzle. Are you ready ?", MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2)
                If ret = vbNo Then
                    Exit Sub
                End If
            End If
            Call Display_NewQuestion()
        End If

        CurrentGridX = GridCount \ 2 + GridCount Mod 2
        CurrentGridY = GridCount \ 2 + GridCount Mod 2


        GridMsg = ""
        Call Reset_Hint()

        Set_Grid(Me.PictureBoxGrid, Me.PictureBoxMemo, Me.PictureBoxPalette, Me.PictureBoxHighlight)


    End Sub

    Private Sub Tool_Reset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tool_Reset.Click, Tool_ResetAnswer.Click, Menu_Reset.Click, Menu_ResetAnswer.Click

        Dim i As Integer, j As Integer
        Dim boolInput As Boolean
        Dim ret As Integer
        Dim tNo As Integer

        For j = 1 To GridCount
            For i = 1 To GridCount
                If SudokuNumberGrid(i, j).FixNo > 0 And SudokuNumberGrid(i, j).Locked = False Then
                    boolInput = True
                    j = GridCount
                    Exit For
                End If
            Next
        Next

        'If Check_FixAll() = True Then
        '    boolInput = True
        'End If

        'If Check_Complete() Then
        '    boolInput = True
        'End If


        If boolInput = True Then
            If AnalyzeMode = False Then
                ret = MsgBox("Clear all input numbers. Are you OK ?", MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2)
            Else
                ret = vbYes
            End If
            If ret = vbYes Then
                For j = 1 To GridCount
                    For i = 1 To GridCount
                        SudokuNumberGrid(i, j).BackColor = Color.White
                        If SudokuNumberGrid(i, j).Locked = False Or SudokuNumberGrid(i, j).ForeColor <> Color.Black Then
                            '    SudokuNumberGrid(i, j).ForeColor = Color.DeepPink
                            'End If
                            SudokuNumberGrid(i, j).Locked = False



                            'If SudokuNumberGrid(i, j).Locked = False Then
                            SudokuNumberGrid(i, j).FixNo = 0
                            SudokuNumberGrid(i, j).ExcludeNo.Clear()
                            SudokuNumberGrid(i, j).MemoNo.Clear()
                            SudokuNumberGrid(i, j).ForeColor = Color.Black
                        End If
                    Next
                Next
                Adjust_ProspectNo(SudokuNumberGrid)
                CompleteFlg = False

                If AnalyzeMode = False Then
                    Call Reset_History()
                End If
                GridMsg = ""
                Call Reset_Hint()

                tNo = Get_DimNo_From_ToolbarName("Highlight")
                ToolboxInfo(tNo).SelectedNo = 0

                Me.PictureBoxGrid.Invalidate()
                Me.PictureBoxHighlight.Invalidate()
                Me.PictureBoxMemo.Invalidate()

            End If
        End If


    End Sub

    Protected Overrides Function ProcessDialogKey( _
        ByVal keyData As Keys) As Boolean
        If (keyData And Keys.KeyCode) = Keys.Left Or (keyData And Keys.KeyCode) = Keys.Right _
            Or (keyData And Keys.KeyCode) = Keys.Up Or (keyData And Keys.KeyCode) = Keys.Down Then
            Return False
        ElseIf (keyData And Keys.KeyCode) = Keys.Tab Then
            Return False
        End If

        Return MyBase.ProcessDialogKey(keyData)

    End Function

    Private Sub Chk_Hint_BackTrack()

        Dim myNumberGrid(,) As SudokuGrid
        Dim nn As Integer
        Dim tmpSudokuNumberGrid(,) As SudokuGrid
        Dim answerNumberGrid(,,) As SudokuGrid
        Dim errFlag As Boolean
        Dim pMin As Integer
        Dim i As Integer, j As Integer

        ReDim tmpSudokuNumberGrid(GridCount, GridCount)
        ReDim myNumberGrid(GridCount, GridCount)

        For j = 1 To GridCount
            For i = 1 To GridCount
                myNumberGrid(i, j) = New SudokuGrid
                If SudokuNumberGrid(i, j).Locked = True Then
                    myNumberGrid(i, j).Copy(SudokuNumberGrid(i, j))
                End If
            Next
        Next

        nn = Solve_SudokuBackTrack(New Coordinate, myNumberGrid, answerNumberGrid, 1, 1)
        If nn = 1 Then
            For j = 1 To GridCount
                For i = 1 To GridCount
                    If SudokuNumberGrid(i, j).FixNo > 0 And SudokuNumberGrid(i, j).FixNo <> answerNumberGrid(i, j, 1).FixNo Then
                        SudokuNumberGrid(i, j).FixError = True
                        SolveHint.X = 0
                        SolveHint.Y = 0
                        SolveHint.No = 99
                        SolveHint.NoB = 0
                        errFlag = True
                    End If
                Next
            Next
        Else
            SolveHint.X = 0
            SolveHint.Y = 0
            SolveHint.No = 99
            SolveHint.NoB = 0
            errFlag = True
        End If

        If errFlag = False Then
            If SolveHint.X > 0 And SolveHint.Y > 0 Then
                SolveHint.NoB = SolveHint.No
            Else
                pMin = GridCount
                For j = 1 To GridCount
                    For i = 1 To GridCount
                        If SudokuNumberGrid(i, j).FixNo = 0 And SudokuNumberGrid(i, j).ProspectNo.Count < pMin Then

                            SolveHint.X = i
                            SolveHint.Y = j
                            SolveHint.No = answerNumberGrid(i, j, 1).FixNo
                            SolveHint.NoB = SolveHint.No
                            pMin = SudokuNumberGrid(i, j).ProspectNo.Count
                        End If
                    Next
                Next
            End If
        End If

    End Sub


    Private Sub Menu_Quit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Menu_Quit.Click

        Dim ret As Integer

        If AnalyzeMode = True Then
            If Check_SavePuzzleData() = False Then
                Exit Sub
            End If
        Else
            If ChangeFlg = True Then
                ret = MsgBox("Quit this software. Are you OK ?", MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2)
                If ret = vbNo Then
                    Exit Sub
                End If
            End If
        End If
        Application.Exit()

    End Sub

    Private Sub Btn_CheckLv()

        Dim x1 As Integer, x2 As Integer
        Dim y1 As Integer, y2 As Integer
        Dim myProspect As New List(Of Coordinate)
        Dim xOnLineFlg As Boolean, yOnLineFlg As Boolean
        Dim mySquareNo As Integer
        Dim myS As Integer
        Dim i As Integer, j As Integer, mySNo As Integer
        Dim x As Integer, y As Integer
        Dim myXY As Coordinate

        Dim myRnd As Integer
        Dim AssignedNo As New List(Of Integer)
        Dim n As Integer, s As Integer, p As Integer
        Dim fixFlag As Boolean
        Dim myCoordinate As New Coordinate
        Dim myRndProspectNo As New List(Of Integer)
        Dim myAssignedNo() As Integer
        Dim myCrossDir As String = "\PuzzleData"
        Dim myDirName As String
        Dim myPuzzleDirName As String
        Dim myDrvName As String
        Dim myFileNameL As String
        Dim myFileNameS As String
        Dim currentNo As Integer = 0
        Dim strPuzzleFile As String
        Dim strFileName As String
        Dim fNo As Integer
        Dim tmpNumberGrid(,) As SudokuGrid
        Dim myNo(2) As List(Of Integer)
        Dim myNoBase(2) As List(Of Integer)
        Dim myNoExclude(2) As List(Of Integer)
        Dim strTxt As String
        Dim di As Double
        Dim dj As Double
        Dim strLine As String
        Dim intFileType As Integer = 1
        Dim strTitle As String = ""
        Dim cnt As Integer
        Dim boolExist As Boolean
        Dim FilePath As String
        Dim pznFileName As String
        Dim myNg As Boolean
        Dim myLevel As Integer, myLevelb As Integer
        Dim Lv1 As Integer, Lv2 As Integer, Lv3 As Integer
        Dim myList As New SortedList
        Dim qCount As Integer
        Dim myLv As Integer, myNum As Integer

        ReDim tmpNumberGrid(GridCount, GridCount)

        For myLv = 3 To 7

            qCount = Get_QuestionCount(myLv)

            For i = 1 To 1000
                myNum = Generate_RandomRange(1, qCount)
                Call Load_NumLogicStock(myLv, myNum)


                '            Load_NumLogicStock(myLv, i)

                p = 0
                For y = 1 To GridCount
                    For x = 1 To GridCount
                        tmpNumberGrid(x, y) = New SudokuGrid
                        tmpNumberGrid(x, y).Copy(SudokuNumberGrid(x, y))
                        If SudokuNumberGrid(x, y).FixNo > 0 Then
                            p = p + 1
                        End If
                    Next
                Next

                If Solve_Sudoku(1, myNg, tmpNumberGrid, myLevel) = 0 Then
                    Debug.Print("Lv:" & myLv & " " & Format(i, "000") & "   No." & Format(myNum, "000") & "  p=" & p & "  Level:" & myLevel)
                Else
                    MsgBox("error")
                    Exit Sub
                End If

            Next

        Next
    End Sub

    Private Sub testSolve()

        Dim tmpSudokuNumberGrid(,) As SudokuGrid
        Dim i As Integer
        Dim j As Integer
        Dim p As Integer
        Dim myNg As Boolean
        Dim myLevel As Integer
        Dim myMode As Integer

        ReDim tmpSudokuNumberGrid(GridCount, GridCount)

        p = 0
        For j = 1 To GridCount
            For i = 1 To GridCount
                tmpSudokuNumberGrid(i, j) = New SudokuGrid
                tmpSudokuNumberGrid(i, j).Copy(SudokuNumberGrid(i, j))
                If tmpSudokuNumberGrid(i, j).FixNo > 0 Then
                    p = p + 1
                End If
            Next
        Next

        myMode = 5

        Call Solve_Sudoku(myMode, myNg, tmpSudokuNumberGrid, myLevel)

        '        If Solve_Sudoku(myMode, myNg, tmpSudokuNumberGrid, myLevel) = 0 Then
        For j = 1 To GridCount
            For i = 1 To GridCount
                SudokuNumberGrid(i, j).Copy(tmpSudokuNumberGrid(i, j))
            Next
        Next
        Me.PictureBoxGrid.Invalidate()

        For j = 1 To GridCount
            For i = 1 To GridCount
                If SudokuNumberGrid(i, j).FixNo = 0 Then
                    For p = 0 To SudokuNumberGrid(i, j).ProspectNo.Count - 1
                        SudokuNumberGrid(i, j).MemoNo.Add(SudokuNumberGrid(i, j).ProspectNo(p))
                    Next

                End If
            Next
        Next




        '        Call Btn_CheckLv()

    End Sub

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Call testSolve()
        '        Call Btn_CheckLv()
    End Sub
End Class

