Public Class Form1
    '仅适用于双屏扩展显示
    '按 [Win + P] 打开“投影面板”，选择“扩展”

    Private Declare Function ReleaseCapture Lib "user32" () As Integer
    Private Declare Function SendMessageA Lib "user32" (ByVal hwnd As Integer, ByVal wMsg As Integer, ByVal wParam As Integer, lParam As VariantType) As Integer
    Private Declare Function GetWindowLong Lib "user32.dll" Alias "GetWindowLongA" (ByVal hwnd As Int32, ByVal nIndex As Int32) As Int32
    Private Declare Function SetWindowLong Lib "user32.dll" Alias "SetWindowLongA" (ByVal hwnd As Int32, ByVal nIndex As Int32, ByVal dwNewLong As Int32) As Int32
    Private Const GWL_STYLE As Int32 = -16
    Private Const WS_THICKFRAME As Int32 = &H40000

    Dim b As Bitmap = New Bitmap(1366, 768)
    Dim g As Graphics

    Dim Dis As Integer = 0
    Dim AspectRatio As Double
    Dim DoubleScreen As Boolean = False

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If Screen.AllScreens.Count = 1 Then Exit Sub

        Me.TopMost = True
        b.Dispose()
        If DoubleScreen Then
            '双屏截图
            b = New Bitmap(Screen.AllScreens.Last.Bounds.Width + Screen.AllScreens.First.Bounds.Width, Math.Max(+Screen.AllScreens.Last.Bounds.Height, Screen.AllScreens.First.Bounds.Height))
        Else
            '仅扩展屏
            b = New Bitmap(Screen.AllScreens.Last.Bounds.Width, Screen.AllScreens.Last.Bounds.Height)
        End If

        g = Graphics.FromImage(b)


        If DoubleScreen Then
            '双屏截图
            g.CopyFromScreen(New Point(0, 0), New Point(0, 0), New Size(Screen.AllScreens.Last.Bounds.Width + Screen.AllScreens.First.Bounds.Width, Math.Max(+Screen.AllScreens.Last.Bounds.Height, Screen.AllScreens.First.Bounds.Height)))
        Else
            '仅扩展屏
            g.CopyFromScreen(New Point(Screen.AllScreens.First.Bounds.Width, 0), New Point(0, 0), Screen.AllScreens.Last.Bounds.Size)
        End If

        '绘制鼠标位置
        If DoubleScreen Then
            '双屏截图
            g.DrawEllipse(Pens.Red, New RectangleF(MousePosition.X - 25, MousePosition.Y - 25, 50, 50))
            g.FillEllipse(Brushes.Red, New RectangleF(MousePosition.X - 5, MousePosition.Y - 5, 10, 10))
        Else
            '仅扩展屏
            If Screen.FromPoint(MousePosition).Equals(Screen.AllScreens.Last) Then
                g.DrawEllipse(Pens.Red, New RectangleF(MousePosition.X - Screen.AllScreens.First.Bounds.Width - 25, MousePosition.Y - 25, 50, 50))
                g.FillEllipse(Brushes.Red, New RectangleF(MousePosition.X - Screen.AllScreens.First.Bounds.Width - 5, MousePosition.Y - 5, 10, 10))
            End If
        End If

        Me.BackgroundImage = b

        g.Dispose()
        GC.Collect()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim MyWindowStyle As Int32 = GetWindowLong(Me.Handle, GWL_STYLE) Or WS_THICKFRAME
        SetWindowLong(Me.Handle, GWL_STYLE, MyWindowStyle)

        AspectRatio = Screen.AllScreens.Last.Bounds.Width / Screen.AllScreens.Last.Bounds.Height
    End Sub

    Private Sub Form1_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        ReleaseCapture()
        SendMessageA(Me.Handle, &HA1, 2, 0&)
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        DoubleScreen = True

        AspectRatio = (Screen.AllScreens.First.Bounds.Width + Screen.AllScreens.Last.Bounds.Width) / Math.Max(+Screen.AllScreens.Last.Bounds.Height, Screen.AllScreens.First.Bounds.Height)
        Me.Height = Me.Width / AspectRatio + Dis
    End Sub

    Private Sub ToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem2.Click
        DoubleScreen = False

        AspectRatio = Screen.AllScreens.Last.Bounds.Width / Screen.AllScreens.Last.Bounds.Height
        Me.Width = (Me.Height - Dis) * AspectRatio
    End Sub

    Private Sub ToolStripMenuItem3_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem3.Click
        Me.WindowState = FormWindowState.Normal
        AspectRatio = Screen.AllScreens.Last.Bounds.Width / Screen.AllScreens.Last.Bounds.Height
        Me.Width = (Me.Height - Dis) * AspectRatio
    End Sub

    Private Sub ToolStripMenuItem4_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem4.Click
        Me.WindowState = FormWindowState.Maximized
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If Me.WindowState = FormWindowState.Normal Then
            Me.Width = (Me.Height - Dis) * AspectRatio
        End If
    End Sub
End Class
