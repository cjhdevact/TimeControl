'/*****************************************************\
'*                                                     *
'*     TimeControl - Form2.vb                          *
'*                                                     *
'*     Copyright (c) CJH.  All Rights Reserved.        *
'*                                                     *
'*     The Settings form.                              *
'*                                                     *
'\*****************************************************/
Imports System.Runtime.InteropServices
Imports Microsoft.Win32
Public Class Form2
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

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        On Error GoTo errcode
            If TextBox1.Text = "" Then
                MessageBox.Show("隐藏时间不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ElseIf TextBox1.Text = "0" Then
                MessageBox.Show("隐藏时间不能为0！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                If ComboBox1.SelectedIndex = 0 Then '秒
                    Form1.Timer2.Interval = TextBox1.Text * 1000
                    Form1.Timer2.Enabled = True
                    Form1.Hide()
                    Me.Close()
                ElseIf ComboBox1.SelectedIndex = 1 Then '分
                    Form1.Timer2.Interval = TextBox1.Text * 1000 * 60
                    Form1.Timer2.Enabled = True
                    Form1.Hide()
                    Me.Close()
                ElseIf ComboBox1.SelectedIndex = 2 Then '时
                    Form1.Timer2.Interval = TextBox1.Text * 1000 * 60 * 60
                    Form1.Timer2.Enabled = True
                    Form1.Hide()
                    Me.Close()
                End If
            End If
        Exit Sub
errcode:
        MsgBox(Err.Description, MsgBoxStyle.Critical, "错误")
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    Private Sub Form2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Form1.TopMost = True Then
            Me.CheckBox1.Checked = True
        Else
            Me.CheckBox1.Checked = False
        End If
        If Form1.UseMoveV = 1 Then
            Me.CheckBox2.Checked = True
        Else
            Me.CheckBox2.Checked = False
        End If

        '如果预先关联事件， Me.CheckBox1.Checked = Ture / Flase 操作会触发事件，导致操作相反
        AddHandler CheckBox1.CheckedChanged, AddressOf CheckBox1_CheckedChanged
        AddHandler CheckBox2.CheckedChanged, AddressOf CheckBox2_CheckedChanged

        ComboBox1.SelectedIndex = 0
        ComboBox1.SelectedText = "秒"

        ComboBox2.SelectedIndex = Form1.appcolor
        If Form1.appcolor = 0 Then
            ComboBox2.SelectedText = "跟随系统"
        ElseIf Form1.appcolor = 1 Then
            ComboBox2.SelectedText = "浅色"
        ElseIf Form1.appcolor = 1 Then
            ComboBox2.SelectedText = "深色"
        End If
        Label1.Text = "时间小工具 版本：" & My.Application.Info.Version.ToString & vbCrLf & "开发分支：" & Form1.DEVBRANCH & " " & vbCrLf & vbCrLf & "版权所有 © 2023 CJH。保留所有权利。"
        Call formatcolorcurset()
    End Sub
    Private Sub TextBox1_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox1.KeyPress
        Dim strlimit As String
        strlimit = "0123456789"
        Dim keychar As Char = e.KeyChar
        If InStr(strlimit, keychar) <> 0 Or e.KeyChar = Microsoft.VisualBasic.ChrW(8) Then
            'If keychar = "." And InStr(TextBox1.Text, keychar) <> 0 Then
            'e.Handled = True
            'Else
            e.Handled = False
            'End If
        Else
            e.Handled = True
        End If
    End Sub
    Sub formatcolorcurset()
        If Form1.crmd = 0 Then
            EnableDarkModeForWindow(Me.Handle, True)
            Me.BackColor = Color.FromArgb(32, 32, 32)
            Me.ForeColor = Color.White
            Me.Button1.BackColor = Color.FromArgb(32, 32, 32)
            Me.Button1.ForeColor = Color.White
            Me.Button2.BackColor = Color.FromArgb(32, 32, 32)
            Me.Button2.ForeColor = Color.White
            Me.TextBox1.BackColor = Color.FromArgb(32, 32, 32)
            Me.TextBox1.ForeColor = Color.White
            Me.ComboBox1.BackColor = Color.FromArgb(32, 32, 32)
            Me.ComboBox1.ForeColor = Color.White
            Me.ComboBox2.BackColor = Color.FromArgb(32, 32, 32)
            Me.ComboBox2.ForeColor = Color.White
            Me.CheckBox1.BackColor = Color.FromArgb(32, 32, 32)
            Me.CheckBox1.ForeColor = Color.White
            Me.CheckBox2.BackColor = Color.FromArgb(32, 32, 32)
            Me.CheckBox2.ForeColor = Color.White
        Else
            EnableDarkModeForWindow(Me.Handle, False)
            Me.BackColor = Color.White
            Me.ForeColor = Color.Black
            Me.Button1.BackColor = Color.Transparent
            Me.Button1.ForeColor = Color.Black
            Me.Button2.BackColor = Color.Transparent
            Me.Button2.ForeColor = Color.Black
            Me.TextBox1.BackColor = Color.White
            Me.TextBox1.ForeColor = Color.Black
            Me.ComboBox1.BackColor = Color.White
            Me.ComboBox1.ForeColor = Color.Black
            Me.ComboBox2.BackColor = Color.White
            Me.ComboBox2.ForeColor = Color.Black
            Me.CheckBox1.BackColor = Color.White
            Me.CheckBox1.ForeColor = Color.Black
            Me.CheckBox2.BackColor = Color.White
            Me.CheckBox2.ForeColor = Color.Black
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
        If Form1.TopMost = True Then
            CheckBox1.Checked = False
            Form1.TopMost = False
            AddReg("Software\CJH\TimeControl\Settings", "AllowTopMost", 0, RegistryValueKind.DWord, "HKCU")
        Else
            CheckBox1.Checked = True
            Form1.TopMost = True
            AddReg("Software\CJH\TimeControl\Settings", "AllowTopMost", 1, RegistryValueKind.DWord, "HKCU")
        End If
    End Sub

    Private Sub CheckBox2_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
        If Form1.UseMoveV = 1 Then
            CheckBox2.Checked = False
            Form1.UseMoveV = 0
            AddReg("Software\CJH\TimeControl\Settings", "EnableDrag", 0, RegistryValueKind.DWord, "HKCU")
        Else
            CheckBox2.Checked = True
            Form1.UseMoveV = 1
            AddReg("Software\CJH\TimeControl\Settings", "EnableDrag", 1, RegistryValueKind.DWord, "HKCU")
        End If
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ComboBox2.SelectedIndexChanged
        If ComboBox2.SelectedIndex = 0 Then
            Form1.appcolor = 0
            AddReg("Software\CJH\TimeControl\Settings", "ColorMode", 0, Microsoft.Win32.RegistryValueKind.DWord, "HKCU")
            'Get System Color
            Dim regkey As RegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", True)
            Dim sysacr As Integer
            If (Not regkey Is Nothing) Then
                'If (If((regkey.GetValue("") Is Nothing), Nothing, regkey.GetValue("").ToString) <> "AppsUseLightTheme") Then
                'End If
                sysacr = regkey.GetValue("AppsUseLightTheme", -1)
                If sysacr = 0 Then
                    Form1.crmd = 0
                ElseIf sysacr = 1 Then
                    Form1.crmd = 1
                Else
                    Form1.crmd = 1
                End If
            Else
                Form1.crmd = 1
            End If
            regkey.Close()
            Call Me.formatcolorcurset()
            Call Form1.formatcolorcur()
        ElseIf ComboBox2.SelectedIndex = 1 Then
            AddReg("Software\CJH\TimeControl\Settings", "ColorMode", 1, Microsoft.Win32.RegistryValueKind.DWord, "HKCU")
            Form1.crmd = 1 'Light
            Form1.appcolor = 1
            Call Me.formatcolorcurset()
            Call Form1.formatcolorcur()
        ElseIf ComboBox2.SelectedIndex = 2 Then
            AddReg("Software\CJH\TimeControl\Settings", "ColorMode", 2, Microsoft.Win32.RegistryValueKind.DWord, "HKCU")
            Form1.crmd = 0 'Dark
            Form1.appcolor = 2
            Call Me.formatcolorcurset()
            Call Form1.formatcolorcur()
        End If
    End Sub

    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click
        If (MessageBox.Show("确定退出时钟小工具吗？", "时钟小工具", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.Yes) Then
            End
        End If
    End Sub
End Class