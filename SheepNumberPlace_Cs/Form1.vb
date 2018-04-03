Imports System.Reflection
Imports System.Drawing
Imports System.Drawing.Drawing2D

Public Class Form1


    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        PictureBox1.Invalidate()

        '        Call Create_HatchStyleList()
    End Sub

    Private Sub Check_InstalledFont()

        'PictureBox1のGraphicsオブジェクトを取得
        Dim g As Graphics = PictureBox1.CreateGraphics()
        'Dim g As Graphics = Me.CreateGraphics()

        'InstalledFontCollectionオブジェクトの取得
        Dim ifc As New System.Drawing.Text.InstalledFontCollection
        'インストールされているすべてのフォントファミリアを取得
        Dim ffs As FontFamily() = ifc.Families

        'Dim fnt1 As New Font("Century", 12)
        'g.DrawString("aaaa", fnt1, Brushes.Red, 30, 30)

        Dim y As Integer = 0
        Dim ff As FontFamily
        Dim i As Integer
        For Each ff In ffs
            i = i + 1
            If i < 10 Then


                'ここではスタイルにRegularが使用できるフォントのみを表示
                If ff.IsStyleAvailable(FontStyle.Regular) Then
                    'Fontオブジェクトを作成
                    Dim fnt As New Font(ff, 12)
                    'フォント名をそのフォントで描画する
                    g.DrawString(fnt.Name, fnt, Brushes.Black, 0, y)

                    Debug.Print(fnt.Name & ":" & y)

                    '次の表示位置を計算
                    y += CInt(fnt.GetHeight(g))
                    'リソースを開放する
                    fnt.Dispose()
                End If
            End If
        Next ff
        ''リソースを開放する
        g.Dispose()



    End Sub


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        Dim g As Graphics = PictureBox1.CreateGraphics()

        MsgBox(System.Drawing.SystemFonts.DefaultFont.Name)

        'Dim ss As FontFamily = Font.Name("")

        'PictureBox1で使用できるFontFamily配列を取得
        Dim ffs As FontFamily() = FontFamily.GetFamilies(g)

        '結果をListBoxに表示する
        Dim ff As FontFamily
        For Each ff In ffs
            Debug.Print(ff.Name)
        Next ff


    End Sub

    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click

    End Sub

    Private Sub PictureBox1_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles PictureBox1.Paint

        'PictureBox1のGraphicsオブジェクトを取得
        ' Dim g As Graphics = e.Graphics 'Graphics = PictureBox1.CreateGraphics()
        'Dim g As Graphics = Me.CreateGraphics()

        'InstalledFontCollectionオブジェクトの取得
        Dim ifc As New System.Drawing.Text.InstalledFontCollection
        'インストールされているすべてのフォントファミリアを取得
        Dim ffs As FontFamily() = ifc.Families

        'Dim fnt1 As New Font("Century", 12)
        'g.DrawString("aaaa", fnt1, Brushes.Red, 30, 30)



        Dim y As Integer = 0
        Dim ff As FontFamily
        Dim i As Integer
        For Each ff In ffs
            i = i + 1
            If i < 1000 Then

                'ここではスタイルにRegularが使用できるフォントのみを表示
                If ff.IsStyleAvailable(FontStyle.Bold) Then
                    'Fontオブジェクトを作成
                    Dim fnt As New Font(ff, 12, FontStyle.Bold)
                    'フォント名をそのフォントで描画する
                    e.Graphics.DrawString(fnt.Name, fnt, Brushes.Black, 0, y)


                    '次の表示位置を計算
                    y += CInt(fnt.GetHeight(e.Graphics))
                    'リソースを開放する
                    fnt.Dispose()
                End If
            End If
        Next ff


        Me.PictureBox1.Height = y + 100
        ''リソースを開放する
        'e.Graphics.Dispose()
    End Sub
End Class