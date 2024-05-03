'/*****************************************************\
'*                                                     *
'*     TimeControl Classic - Form1.vb                  *
'*                                                     *
'*     Copyright (c) CJH.  All Rights Reserved.        *
'*                                                     *
'*     The main time form.                             *
'*                                                     *
'\*****************************************************/
Public Class Form1
    Public a As New System.Drawing.Point

    Public MovedV As Integer
    Public UseMoveV As Integer
    Public Const DEVBRANCH = "TCTL_FIX"
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Label1.Text = Format(Now(), "HH:mm:ss")
        If MovedV <> 1 Then
            If UseMoveV = 0 Then
                If Me.Location <> a Then
                    Me.Location = a
                End If
            End If
        End If
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        UseMoveV = 1
        MovedV = 0
        Dim disi As Graphics = Me.CreateGraphics()
        Timer1.Enabled = True
        'Me.Height = 38
        'Me.Width = 120
        If disi.DpiX <= 96 Then
            Me.Height = 38
            Me.Width = 120
        Else
            Me.Height = 38 * disi.DpiY * 0.01 * 1.15
            Me.Width = 120 * disi.DpiX * 0.01 * 1.15
        End If
        'Me.Height = Label1.Height
        'Me.Width = Label1.Width
        a.X = (System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width - Me.Width) / 2
        a.Y = 5 * disi.DpiY * 0.01
        Me.Location = a
        'ContextMenuStrip1.Font = New Font(ContextMenuStrip1.Font.Name, 8.25F * 96.0F / CreateGraphics().DpiX, ContextMenuStrip1.Font.Style, ContextMenuStrip1.Font.Unit, ContextMenuStrip1.Font.GdiCharSet, ContextMenuStrip1.Font.GdiVerticalFont)
        'Font = New Font(Font.Name, 8.25F * 96.0F / CreateGraphics().DpiX, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont)
        Me.ContextMenuStrip1.BackColor = Color.White
        Me.ContextMenuStrip1.ForeColor = Color.Black
    End Sub
    'API移动窗体
    Declare Function SendMessage Lib "user32" Alias "SendMessageA" (ByVal hwnd As IntPtr, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Boolean
    Declare Function ReleaseCapture Lib "user32" Alias "ReleaseCapture" () As Boolean
    Const WM_SYSCOMMAND = &H112
    Const SC_MOVE = &HF010&
    Const HTCAPTION = 2
    Private Sub Form1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown
        If UseMoveV = 1 Then
            ReleaseCapture()
            SendMessage(Me.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0)
            MovedV = 1
        End If
    End Sub
    Private Sub Label1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Label1.MouseDown
        If UseMoveV = 1 Then
            ReleaseCapture()
            SendMessage(Me.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0)
            MovedV = 1
        End If
    End Sub
    Private Sub h30s_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles h30s.Click
        Timer2.Interval = 30000
        Me.Hide()
        Timer2.Enabled = True
    End Sub

    Private Sub h1m_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles h1m.Click
        Timer2.Interval = 60000
        Me.Hide()
        Timer2.Enabled = True
    End Sub

    Private Sub h5m_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles h5m.Click
        Timer2.Interval = 300000
        Me.Hide()
        Timer2.Enabled = True
    End Sub

    Private Sub h10m_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles h10m.Click
        Timer2.Interval = 600000
        Me.Hide()
        Timer2.Enabled = True
    End Sub

    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        Me.Show()
        Timer2.Enabled = False
    End Sub

    Private Sub ext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ext.Click
        'If MessageBox.Show("确定要关闭时钟吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = MsgBoxResult.Yes Then
        'End
        'End If
        Form2.ShowDialog()
    End Sub

    Private Sub Me_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles Me.FormClosing
        'e.Cancel = True
        Select Case (e.CloseReason)
            '应用程序要求关闭窗口
            Case CloseReason.ApplicationExitCall
                e.Cancel = False '不拦截，响应操作
                '自身窗口上的关闭按钮
            Case CloseReason.FormOwnerClosing
                e.Cancel = True '拦截，不响应操作
                'MDI窗体关闭事件
            Case CloseReason.MdiFormClosing
                e.Cancel = True '拦截，不响应操作
                '不明原因的关闭
            Case CloseReason.None
                e.Cancel = False
                '任务管理器关闭进程
            Case CloseReason.TaskManagerClosing
                e.Cancel = False  '不拦截，响应操作
                '用户通过UI关闭窗口或者通过Alt+F4关闭窗口
            Case CloseReason.UserClosing
                e.Cancel = True '拦截，不响应操作
                '操作系统准备关机()
            Case (CloseReason.WindowsShutDown)
                e.Cancel = False '不拦截，响应操作
        End Select

    End Sub

    'Private Sub Me_FormClosed(ByVal sender As Object, ByVal e As FormClosedEventArgs) Handles Me.FormClosed
    'Me.Dispose()
    'End Sub
End Class
