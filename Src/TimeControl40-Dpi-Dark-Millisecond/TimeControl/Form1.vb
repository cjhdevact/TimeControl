'/*****************************************************\
'*                                                     *
'*     TimeControl - Form1.vb                          *
'*                                                     *
'*     Copyright (c) CJH.  All Rights Reserved.        *
'*                                                     *
'*     The main time form.                             *
'*                                                     *
'\*****************************************************/
Imports Microsoft.Win32
Imports System.Runtime.InteropServices

Public Class Form1
    <DllImport("dwmapi.dll")> _
    Public Shared Function DwmSetWindowAttribute(ByVal hwnd As IntPtr, ByVal attr As DwmWindowAttribute, ByRef attrValue As Integer, ByVal attrSize As Integer) As Integer
    End Function
    'Public Shared Function EnableDarkModeForWindow(ByVal hWnd As IntPtr, ByVal enable As Boolean) As Boolean
    '    Dim darkMode As Integer
    '    darkMode = enable
    '    Dim hr As Integer
    '    Dim i As Integer
    '    'hr = DwmSetWindowAttribute(hWnd, DwmWindowAttribute.UseImmersiveDarkMode, darkMode, sizeof(i))
    '    Return hr >= 0
    'End Function
    Public Shared Function EnableDarkModeForWindow(ByVal hWnd As IntPtr, ByVal enable As Boolean) As Boolean
        Dim attrValue As Integer = If(enable, 1, 0)
        Return (Form1.DwmSetWindowAttribute(hWnd, DwmWindowAttribute.UseImmersiveDarkMode, attrValue, 4) >= 0)
    End Function

    Public Enum DwmWindowAttribute As UInt32
        NCRenderingEnabled = 1
        NCRenderingPolicy
        TransitionsForceDisabled
        AllowNCPaint
        CaptionButtonBounds
        NonClientRtlLayout
        ForceIconicRepresentation
        Flip3DPolicy
        ExtendedFrameBounds
        HasIconicBitmap
        DisallowPeek
        ExcludedFromPeek
        Cloak
        Cloaked
        FreezeRepresentation
        PassiveUpdateMode
        UseHostBackdropBrush
        UseImmersiveDarkMode = 20
        WindowCornerPreference = 33
        BorderColor
        CaptionColor
        TextColor
        VisibleFrameBorderThickness
        SystemBackdropType
        Last
    End Enum

    Public Const DEVBRANCH = "TCTL_DEV_MILL"

    Public a As New System.Drawing.Point
    Public crmd As Integer ' 1=Dark 1=Light
    Public appcolor As Integer ' 0= With System 1= Dark 2= Light
    Public MovedV As Integer
    Public UseMoveV As Integer
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Label1.Text = Format(Now(), "HH:mm:ss.") & DateTime.Now.Millisecond
        If UseMoveV = 0 Then
            If MovedV = 0 Then
                If Me.Location <> a Then
                    Me.Location = a
                End If
            End If
        End If
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        MovedV = 0
        appcolor = 0
        Dim disi As Graphics = Me.CreateGraphics()
        Timer1.Enabled = True
        'Me.Height = 38
        'Me.Width = 120
        If disi.DpiX <= 96 Then
            Me.Height = 38
            Me.Width = 174
        Else
            Me.Height = 38 * disi.DpiY * 0.01 * 1.15
            Me.Width = 174 * disi.DpiX * 0.01 * 1.15
        End If
        'Me.Height = Label1.Height
        'Me.Width = Label1.Width
        a.X = (System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width - Me.Width) / 2
        a.Y = 5 * disi.DpiY * 0.01
        Me.Location = a
        'ContextMenuStrip1.Font = New Font(ContextMenuStrip1.Font.Name, 8.25F * 96.0F / CreateGraphics().DpiX, ContextMenuStrip1.Font.Style, ContextMenuStrip1.Font.Unit, ContextMenuStrip1.Font.GdiCharSet, ContextMenuStrip1.Font.GdiVerticalFont)
        'Font = New Font(Font.Name, 8.25F * 96.0F / CreateGraphics().DpiX, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont)

        'Get Color Settings
        Try
            AddKey("Software\CJH", "HKCU")
            AddKey("Software\CJH\TimeControl", "HKCU")
            AddKey("Software\CJH\TimeControl\Settings", "HKCU")
        Catch ex As Exception
        End Try
        Dim mykey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\CJH\TimeControl\Settings", True)
        Dim myv As Integer
        If (Not mykey Is Nothing) Then
            'If (If((regkey.GetValue("") Is Nothing), Nothing, regkey.GetValue("").ToString) <> "AppsUseLightTheme") Then
            'End If
            myv = mykey.GetValue("ColorMode", -1)
            If myv = 0 Then
                appcolor = 0
            ElseIf myv = -1 Then
                appcolor = 0
                AddReg("Software\CJH\TimeControl\Settings", "ColorMode", 0, Microsoft.Win32.RegistryValueKind.DWord, "HKCU")
            ElseIf myv = 1 Then
                appcolor = 1
            ElseIf myv = 2 Then
                appcolor = 2
            End If
        Else
            appcolor = 0
            AddReg("Software\CJH\TimeControl\Settings", "ColorMode", 0, Microsoft.Win32.RegistryValueKind.DWord, "HKCU")
        End If


        If appcolor = 0 Then
            'Get System Color
            Dim regkey As RegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", True)
            Dim sysacr As Integer
            Try
                If (Not regkey Is Nothing) Then
                    'If (If((regkey.GetValue("") Is Nothing), Nothing, regkey.GetValue("").ToString) <> "AppsUseLightTheme") Then
                    'End If
                    sysacr = regkey.GetValue("AppsUseLightTheme", -1)
                    If sysacr = 0 Then
                        crmd = 0
                    ElseIf sysacr = 1 Then
                        crmd = 1
                    Else
                        crmd = 1
                    End If
                Else
                    crmd = 1
                End If
            Catch ex As Exception
                crmd = 1
            End Try
            regkey.Close()
        ElseIf appcolor = 1 Then
            crmd = 1 'Light
        ElseIf appcolor = 2 Then
            crmd = 0 'Dark
        End If
        AddHandler SystemEvents.UserPreferenceChanged, AddressOf SystemEvents_UserPreferenceChanged

        If (Not mykey Is Nothing) Then
            Me.UseMoveV = mykey.GetValue("EnableDrag", -1)
            If Me.UseMoveV = -1 Then
                RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "EnableDrag", 1, RegistryValueKind.DWord, "HKCU")
                Me.UseMoveV = 1
            End If
        Else
            RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "EnableDrag", 1, RegistryValueKind.DWord, "HKCU")
            Me.UseMoveV = 1
        End If

        Dim UseTop As Integer
        If (Not mykey Is Nothing) Then
            UseTop = mykey.GetValue("AllowTopMost", -1)
            If UseTop = -1 Then
                RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "AllowTopMost", 1, RegistryValueKind.DWord, "HKCU")
                Me.TopMost = True
            ElseIf UseTop = 0 Then
                Me.TopMost = False
            ElseIf UseTop = 1 Then
                Me.TopMost = True
            End If
        Else
            RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "AllowTopMost", 1, RegistryValueKind.DWord, "HKCU")
            Me.TopMost = True
        End If
        mykey.Close()

        Call formatcolorcur()
        Call Form2.formatcolorcurset()
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

    Sub formatcolorcur()
        If crmd = 0 Then
            Me.BackgroundImage = My.Resources.bkgdark
            Me.Label1.ForeColor = Color.White
            Me.ContextMenuStrip1.BackColor = Color.FromArgb(32, 32, 32)
            Me.ContextMenuStrip1.ForeColor = Color.White
            Me.BackColor = Color.FromArgb(32, 32, 32)
            Me.ForeColor = Color.White
            'Me.h10m.BackColor = Color.Black
            'Me.h10m.ForeColor = Color.White
            'Me.h1m.BackColor = Color.Black
            'Me.h1m.ForeColor = Color.White
            'Me.h30s.BackColor = Color.Black
            'Me.h30s.ForeColor = Color.White
            'Me.h5m.BackColor = Color.Black
            'Me.h5m.ForeColor = Color.White
            EnableDarkModeForWindow(Me.Handle, True)
            Me.TransparencyKey = Color.FromArgb(1, 1, 1)
        Else
            Me.BackColor = Color.White
            Me.ForeColor = Color.Black
            Me.BackgroundImage = My.Resources.bkg
            Me.Label1.ForeColor = Color.Black
            Me.ContextMenuStrip1.BackColor = Color.White
            Me.ContextMenuStrip1.ForeColor = Color.Black
            EnableDarkModeForWindow(Me.Handle, False)
            Me.TransparencyKey = Color.FromArgb(184, 184, 184)
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

    Private Sub SystemEvents_UserPreferenceChanged(ByVal sender As Object, ByVal e As UserPreferenceChangedEventArgs)
        If e.Category = UserPreferenceCategory.General Then
            If appcolor = 0 Then
                'Get System Color
                Dim regkey As RegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", True)
                Dim sysacr As Integer
                If (Not regkey Is Nothing) Then
                    'If (If((regkey.GetValue("") Is Nothing), Nothing, regkey.GetValue("").ToString) <> "AppsUseLightTheme") Then
                    'End If
                    sysacr = regkey.GetValue("AppsUseLightTheme", -1)
                    If sysacr = 0 Then
                        crmd = 0
                    ElseIf sysacr = 1 Then
                        crmd = 1
                    Else
                        crmd = 1
                    End If
                Else
                    crmd = 1
                End If
                regkey.Close()
                Call formatcolorcur()
                Call Form2.formatcolorcurset()
            End If
        End If
    End Sub
End Class
