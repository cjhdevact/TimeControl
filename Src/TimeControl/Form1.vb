'****************************************************************************
'    TimeControl
'    Copyright (C) 2022-2025 CJH.
'
'    This program is free software: you can redistribute it and/or modify
'    it under the terms of the GNU General Public License as published by
'    the Free Software Foundation, either version 3 of the License, or
'    (at your option) any later version.
'
'    This program is distributed in the hope that it will be useful,
'    but WITHOUT ANY WARRANTY; without even the implied warranty of
'    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    GNU General Public License for more details.
'
'    You should have received a copy of the GNU General Public License
'    along with this program.  If not, see <http://www.gnu.org/licenses/>.
'****************************************************************************
'/*****************************************************\
'*                                                     *
'*     TimeControl - Form1.vb                          *
'*                                                     *
'*     Copyright (c) CJH.                              *
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

    Public Const DEVBRANCH = "TCTL_DEV_MAIN"

    Public a As New System.Drawing.Point
    Public crmd As Integer ' 0=Dark 1=Light
    Public appcolor As Integer ' 0= With System 1= Dark 2= Light
    Public MovedV As Integer
    Public UseMoveV As Integer
    Public SaveLoc As Integer
    Public UnSupportDarkSys As Integer ' 系统 0=支持 1=不支持 深色模式
    Public TimeF As String 'Time Format
    Public CaW As Integer '计算的宽度
    Public CaH As Integer '计算的高度
    Public IsBootV As Integer '启动程序执行代码的标志
    Public MySize As Integer '程序是否使用自定义大小
    Public TimeTheme As Integer '程序主题
    Public CustOpacity As Integer '自定义透明度
    Public DisbFuState As Integer '禁用功能
    Public UnSaveData As Integer '不保存设置
    Public UnReadData As Integer '不读取设置
    Public ShowModeTips As Integer '不显示横幅
    Public NeedStillTopMost As Integer '是否强制顶置

    '在Alt+Tab中隐藏
    Const WS_EX_COMPOSITED = &H2000000 '0x02000000
    'Const WS_EX_NOACTIVATE = &H8000000 '0x08000000
    Const WS_EX_TOOLWINDOW = &H80 '0x00000080
    'Const WS_EX_TRANSPARENT = &H20 '0x00000020
    Protected Overrides ReadOnly Property CreateParams As CreateParams
        Get
            Dim cp As CreateParams = MyBase.CreateParams
            cp.ExStyle = cp.ExStyle Or WS_EX_TOOLWINDOW Or WS_EX_COMPOSITED
            Return cp
        End Get
    End Property

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Try
            If Form2.CheckBox5.Checked = True Then
                If TimeF = "HH:mm:ss" Then
                    Label1.Text = Format(Now(), TimeF) & "." & DateTime.Now.Millisecond
                Else
                    Label1.Text = Format(Now(), TimeF)
                End If
            Else
                Label1.Text = Format(Now(), TimeF)
            End If
        Catch ex As Exception
            TimeF = "HH:mm:ss"
            MsgBox(ex.Message & vbCrLf & "时间格式化失败，已重置为默认格式。", MsgBoxStyle.Critical, "错误")
        End Try
        If UseMoveV = 0 Then
            If MovedV = 0 Then
                If Me.Location <> a Then
                    Me.Location = a
                End If
            End If
        End If
        If Me.TopMost = True Then
            If Me.Visible = True Then
                If NeedStillTopMost = 1 Then
                    SetWindowPos(Me.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS)
                End If
            End If
        End If
    End Sub
    <DllImport("user32.dll")>
    Private Shared Function SetWindowPos(ByVal hWnd As IntPtr, ByVal hWndInsertAfter As IntPtr, ByVal X As Integer, ByVal Y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal uFlags As UInteger) As Boolean
    End Function

    Const HWND_TOPMOST = -1
    Const SWP_NOSIZE As UInteger = &H1
    Const SWP_NOMOVE As UInteger = &H2
    Const TOPMOST_FLAGS As UInteger = SWP_NOMOVE Or SWP_NOSIZE

    Public Sub SetTimeFormSize(ByVal MeH As Integer, ByVal MeW As Integer)
        Dim disi As Graphics = Me.CreateGraphics()
        'If disi.DpiX <= 96 Then
        '    Me.Height = MeH
        '    Me.Width = MeW
        'Else
        '    Me.Height = MeH * disi.DpiY * 0.01 * 1.15
        '    Me.Width = MeW * disi.DpiX * 0.01 * 1.15
        'End If
        'If disi.DpiX <= 96 Then
        '    Me.Height = MeH + 3
        '    Me.Width = MeW + 5
        'Else
        '    Me.Height = MeH * disi.DpiY * 0.01
        '    Me.Width = MeW * disi.DpiX * 0.01
        'End If
        Me.Height = MeH + disi.DpiY * 0.01 * 12
        Me.Width = MeW + disi.DpiX * 0.01 * 12
        If Me.WindowState = FormWindowState.Normal Then
            If TimeTheme = 0 Then
                If Me.Width >= 250 Then
                    If crmd = 0 Then
                        Me.BackgroundImage = My.Resources.bkgdark400
                    Else
                        Me.BackgroundImage = My.Resources.bkg400
                    End If
                ElseIf Me.Width <= 70 Then
                    If crmd = 0 Then
                        Me.BackgroundImage = My.Resources.bkgdark50
                    Else
                        Me.BackgroundImage = My.Resources.bkg50
                    End If
                Else
                    If crmd = 0 Then
                        Me.BackgroundImage = My.Resources.bkgdark
                    Else
                        Me.BackgroundImage = My.Resources.bkg
                    End If
                End If
            End If
        End If
    End Sub
    Public Sub GetTimeFormSize(ByVal MeH As Integer, ByVal MeW As Integer)
        Dim disi As Graphics = Me.CreateGraphics()
        'If disi.DpiX <= 96 Then
        '    CaH = MeH
        '    CaW = MeW
        'Else
        '    CaH = MeH * disi.DpiY * 0.01 * 1.15
        '    CaW = MeW * disi.DpiX * 0.01 * 1.15
        'End If
        CaH = MeH + disi.DpiY * 0.01 * 8
        CaW = MeW + disi.DpiX * 0.01 * 8
    End Sub
    Public Sub disbfu()
        'Form2.Label20.Visible = True
        Form2.ComboBox2.Enabled = False
        Form2.ComboBox3.Enabled = False
        Form2.ComboBox4.Enabled = False
        Form2.Button10.Enabled = False
        Form2.Button11.Enabled = False
        Form2.TextBox5.Enabled = False
        Form2.TextBox2.Enabled = False
        Form2.Button4.Enabled = False
        Form2.Button7.Enabled = False
        Form2.Button8.Enabled = False
        Form2.CheckBox3.Enabled = False
        Form2.CheckBox4.Enabled = False
        Form2.TextBox3.Enabled = False
        Form2.TextBox4.Enabled = False
        Form2.Button9.Enabled = False
        Form2.Button5.Visible = False
        Form2.TrackBar1.Enabled = False
        Form2.LinkLabel3.Visible = False
        DisbFuState = 1
        Form2.Button5.Visible = False
        Form2.Label20.Text = "部分功能由于被管理员禁用而无法使用。"
    End Sub
    Public Sub loaddef(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '////////////////////////////////////////////////////////////////////////////////////
        '//
        '//  系统颜色读取注册表读取（UnReadData=1）
        '//
        '////////////////////////////////////////////////////////////////////////////////////
        Try
            'Get System Color
            Dim regkey As RegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", True)
            Dim sysacr As Integer
            Try
                If (Not regkey Is Nothing) Then
                    'If (If((regkey.GetValue("") Is Nothing), Nothing, regkey.GetValue("").ToString) <> "AppsUseLightTheme") Then
                    'End If
                    sysacr = regkey.GetValue("AppsUseLightTheme", -1)
                Else
                    UnSupportDarkSys = 1
                End If
            Catch ex As Exception
                UnSupportDarkSys = 1
            End Try
            If appcolor = 0 Then
                If (Not regkey Is Nothing) Then
                    'If (If((regkey.GetValue("") Is Nothing), Nothing, regkey.GetValue("").ToString) <> "AppsUseLightTheme") Then
                    'End If
                    If sysacr = 0 Then
                        UnSupportDarkSys = 0
                        crmd = 0
                    ElseIf sysacr = 1 Then
                        UnSupportDarkSys = 0
                        crmd = 1
                    Else
                        UnSupportDarkSys = 1
                        crmd = 1
                    End If
                Else
                    UnSupportDarkSys = 1
                    crmd = 1
                End If
            ElseIf appcolor = 1 Then
                If (Not regkey Is Nothing) Then
                    If sysacr = 0 Then
                        UnSupportDarkSys = 0
                    ElseIf sysacr = 1 Then
                        UnSupportDarkSys = 0
                    Else
                        UnSupportDarkSys = 1
                    End If
                Else
                    UnSupportDarkSys = 1
                End If
                crmd = 1 'Light
            ElseIf appcolor = 2 Then
                If (Not regkey Is Nothing) Then
                    If sysacr = 0 Then
                        UnSupportDarkSys = 0
                    ElseIf sysacr = 1 Then
                        UnSupportDarkSys = 0
                    Else
                        UnSupportDarkSys = 1
                    End If
                Else
                    UnSupportDarkSys = 1
                End If
                crmd = 0 'Dark
            End If

            regkey.Close()
        Catch ex As Exception
        End Try


        If UnSupportDarkSys = 1 Then
            If appcolor = 0 Then
                appcolor = 1
                If UnSaveData = 0 Then
                    AddReg("Software\CJH\TimeControl\Settings", "ColorMode", 1, Microsoft.Win32.RegistryValueKind.DWord, "HKCU")
                End If
                Form2.ComboBox2.SelectedIndex = 0
            ElseIf appcolor = 1 Then
                Form2.ComboBox2.SelectedIndex = 0
            ElseIf appcolor = 2 Then
                Form2.ComboBox2.SelectedIndex = 1
            End If

            Form2.ComboBox2.Items.Clear()
            Form2.ComboBox2.Items.AddRange(New Object() {"浅色", "深色"})
        End If

        AddHandler SystemEvents.UserPreferenceChanged, AddressOf SystemEvents_UserPreferenceChanged


        '******************************************************************************************

        Form2.Button5.Visible = False
        appcolor = 0
        Form2.LinkLabel3.Visible = False
        Me.UseMoveV = 1
        Me.SaveLoc = 0
        Me.MySize = 0
        Me.TopMost = True
        Form2.TextBox2.Text = "HH:mm:ss"
        Form2.ComboBox3.SelectedIndex = 0
        Me.Location = New Size((System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width - Me.Width) / 2, 5)
        AddHandler Form2.ComboBox3.SelectedIndexChanged, AddressOf Form2.ComboBox3_SelectedIndexChanged
        Call Form2.ComboBox3_SelectedIndexChanged(sender, e)
        If Form2.ComboBox3.SelectedIndex = 6 Then
            Call Form2.Button4_Click(sender, e)
        End If
        Form2.TextBox3.Text = Me.Width
        Form2.TextBox4.Text = Me.Height
        TimeTheme = 0
        If Width >= 250 Then
            If crmd = 0 Then
                BackgroundImage = My.Resources.bkgdark400
            Else
                BackgroundImage = My.Resources.bkg400
            End If
        ElseIf Width <= 70 Then
            If crmd = 0 Then
                BackgroundImage = My.Resources.bkgdark50
            Else
                BackgroundImage = My.Resources.bkg50
            End If
        Else
            If crmd = 0 Then
                BackgroundImage = My.Resources.bkgdark
            Else
                BackgroundImage = My.Resources.bkg
            End If
        End If
        If crmd = 0 Then
            TransparencyKey = Color.FromArgb(1, 1, 1)
        Else
            TransparencyKey = Color.FromArgb(184, 184, 184)
        End If
        Form2.TextBox5.Text = ""
        CustOpacity = 100
        If TimeTheme = 2 Or 3 Then
            Opacity = CustOpacity * 0.01
        End If

        Form2.ColorDialog1.Color = Label1.ForeColor
        Form2.FontDialog1.Font = Label1.Font
        Form2.Label17.Text = CustOpacity & "%"
        Form2.TrackBar1.Value = CustOpacity
    End Sub
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        NeedStillTopMost = 1
        TimeF = "HH:mm:ss"
        '（在Alt+Tab隐藏窗体：启动时设置为Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None，但切换其他模式再切换回来又会显示）
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None
        MovedV = 0
        appcolor = 0
        Dim disi As Graphics = Me.CreateGraphics()
        Timer1.Enabled = True
        Me.Location = New Point(((System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width - Me.Width) / 2), 5 * disi.DpiY * 0.01)
        IsBootV = 1
        'Me.Height = 38
        'Me.Width = 120

        'Me.Height = Label1.Height
        'Me.Width = Label1.Width
        'SetTimeFormSize(38, 120)
        'ContextMenuStrip1.Font = New Font(ContextMenuStrip1.Font.Name, 8.25F * 96.0F / CreateGraphics().DpiX, ContextMenuStrip1.Font.Style, ContextMenuStrip1.Font.Unit, ContextMenuStrip1.Font.GdiCharSet, ContextMenuStrip1.Font.GdiVerticalFont)
        'Font = New Font(Font.Name, 8.25F * 96.0F / CreateGraphics().DpiX, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont)

        'TimeF = "HH:mm:ss"
        'SetTimeFormSize(38, 120)

        'Get Color Settings

        '////////////////////////////////////////////////////////////////////////////////////
        '//
        '//  时钟禁用功能策略注册表读取
        '//
        '////////////////////////////////////////////////////////////////////////////////////
        DisbFuState = 0
        UnSaveData = 0
        UnReadData = 0
        ShowModeTips = 1

        Dim cdisbfu As Integer = 0
        Dim cdisbfut As Integer = 0
        Dim unsavefut As Integer = 0
        Dim unloadfut As Integer = 0
        Try

            Dim plkeycr As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Policies\CJH\TimeControl", True)

            Dim disfucrt As Integer = -1
            If (Not plkeycr Is Nothing) Then
                disfucrt = plkeycr.GetValue("DisableFeaturesTip", -1)
                If disfucrt = 1 Then
                    Form2.Label20.Visible = False
                    ShowModeTips = 0
                    cdisbfut = 1
                ElseIf disfucrt = 0 Then
                    Form2.Label20.Visible = True
                    ShowModeTips = 1
                    cdisbfut = 0
                End If
            End If

            Dim disfucr As Integer
            If (Not plkeycr Is Nothing) Then
                disfucr = plkeycr.GetValue("DisableFeatures", -1)
                If disfucr = 1 Then
                    Call disbfu()
                    cdisbfu = 1
                End If
            End If


            Dim unsavecfgcr As Integer
            If (Not plkeycr Is Nothing) Then
                unsavecfgcr = plkeycr.GetValue("NoSaveProfile", -1)
                If unsavecfgcr = 1 Then
                    UnSaveData = 1
                    If DisbFuState = 1 Then
                        Form2.Label20.Text = "由于策略设置，你的更改将不会被保存。部分功能由于被管理员禁用而无法使用。"
                    Else
                        Form2.Label20.Text = "由于策略设置，你的更改将不会被保存。"
                    End If
                    Form2.LinkLabel3.Visible = False
                    Form2.Button5.Visible = False
                    unsavefut = 1
                End If
            End If

            Dim unreadcfgcr As Integer
            If (Not plkeycr Is Nothing) Then
                unreadcfgcr = plkeycr.GetValue("NoProfile", -1)
                If unreadcfgcr = 1 Then
                    'If DisbFuState = 0 And UnSaveData = 0 Then
                    '    Form2.Label20.Visible = False
                    '    ShowModeTips = 0
                    'End If
                    unloadfut = 1
                    UnReadData = 1
                    UnSaveData = 1
                    Call loaddef(sender, e)
                    If DisbFuState = 1 Then
                        Form2.Label20.Text = "当前处于无配置模式，你的更改将不会被保存。部分功能由于被管理员禁用而无法使用。"
                    Else
                        Form2.Label20.Text = "当前处于无配置模式，你的更改将不会被保存。"
                    End If
                End If
            End If

            If (Not plkeycr Is Nothing) Then
                plkeycr.Close()
            End If
        Catch ex As Exception
        End Try

        Try
            Dim plkey As RegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\Policies\CJH\TimeControl", True)
            Dim disfu As Integer

            If cdisbfut = 0 Then
                Dim disfut As Integer = -1
                If (Not plkey Is Nothing) Then
                    disfut = plkey.GetValue("DisableFeaturesTip", -1)
                    If disfut = 1 Then
                        Form2.Label20.Visible = False
                        ShowModeTips = 0
                    ElseIf disfut = 0 Then
                        Form2.Label20.Visible = True
                        ShowModeTips = 1
                    End If
                End If
            End If

            If cdisbfu = 0 Then
                If (Not plkey Is Nothing) Then
                    disfu = plkey.GetValue("DisableFeatures", -1)
                    If disfu = 1 Then
                        Call disbfu()
                    End If
                End If
            End If

            If unsavefut = 0 Then
                Dim unsavecfg As Integer
                If (Not plkey Is Nothing) Then
                    unsavecfg = plkey.GetValue("NoSaveProfile", -1)
                    If unsavecfg = 1 Then
                        UnSaveData = 1
                        If DisbFuState = 1 Then
                            Form2.Label20.Text = "由于策略设置，你的更改将不会被保存。部分功能由于被管理员禁用而无法使用。"
                        Else
                            Form2.Label20.Text = "由于策略设置，你的更改将不会被保存。"
                        End If
                        Form2.LinkLabel3.Visible = False
                        Form2.Button5.Visible = False
                    End If
                End If
            End If

            If unloadfut = 0 Then
                Dim unreadcfg As Integer

                If (Not plkey Is Nothing) Then
                    'If DisbFuState = 0 And UnSaveData = 0 Then
                    '    Form2.Label20.Visible = False
                    '    ShowModeTips = 0
                    'End If
                    unreadcfg = plkey.GetValue("NoProfile", -1)
                    If unreadcfg = 1 Then
                        UnReadData = 1
                        UnSaveData = 1
                        Call loaddef(sender, e)
                        If DisbFuState = 1 Then
                            Form2.Label20.Text = "当前处于无配置模式，你的更改将不会被保存。部分功能由于被管理员禁用而无法使用。"
                        Else
                            Form2.Label20.Text = "当前处于无配置模式，你的更改将不会被保存。"
                        End If
                    End If
                End If
            End If

            If (Not plkey Is Nothing) Then
                plkey.Close()
            End If

        Catch ex As Exception
        End Try

        Try
            If Command().ToLower = "/safemode" Then
                If DisbFuState = 1 Then
                    Form2.Label20.Text = "当前处于安全模式，你的更改将不会被保存。部分功能由于被管理员禁用而无法使用。"
                Else
                    Form2.Label20.Text = "当前处于安全模式，你的更改将不会被保存。"
                End If
                UnReadData = 1
                UnSaveData = 1
                Form2.Label20.Visible = True
                Form2.LinkLabel3.Visible = True
                Form2.Button5.Visible = True
                Call loaddef(sender, e)
            End If

            If Command().ToLower = "/noprofile" Then
                If DisbFuState = 1 Then
                    Form2.Label20.Text = "当前处于无配置模式，你的更改将不会被保存。部分功能由于被管理员禁用而无法使用。"
                Else
                    Form2.Label20.Text = "当前处于无配置模式，你的更改将不会被保存。"
                End If
                UnReadData = 1
                UnSaveData = 1
                Call loaddef(sender, e)
            End If

            If Command().ToLower = "/nosaveprofile" Then
                If DisbFuState = 1 Then
                    Form2.Label20.Text = "当前你的更改将不会被保存。部分功能由于被管理员禁用而无法使用。"
                Else
                    Form2.Label20.Text = "当前你的更改将不会被保存。"
                End If
                UnReadData = 0
                UnSaveData = 1
                Form2.LinkLabel3.Visible = False
                Form2.Button5.Visible = False
            End If
        Catch ex As Exception
        End Try

        '******************************************************************************************

        If UnReadData = 0 Then
            If UnSaveData = 0 And UnReadData = 0 And DisbFuState = 0 Then
                Form2.Label20.Visible = False
                ShowModeTips = 0
            End If
            Try
                AddKey("Software\CJH", "HKCU")
                AddKey("Software\CJH\TimeControl", "HKCU")
                AddKey("Software\CJH\TimeControl\Settings", "HKCU")
            Catch ex As Exception
            End Try
            Dim mykey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\CJH\TimeControl\Settings", True)
            Dim myv As Integer
            Try


                '////////////////////////////////////////////////////////////////////////////////////
                '//
                '//  时钟颜色主题注册表读取
                '//
                '////////////////////////////////////////////////////////////////////////////////////
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
                '////////////////////////////////////////////////////////////////////////////////////
                '//
                '//  系统颜色读取注册表读取
                '//
                '////////////////////////////////////////////////////////////////////////////////////
                Try
                    'Get System Color
                    Dim regkey As RegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", True)
                    Dim sysacr As Integer
                    Try
                        If (Not regkey Is Nothing) Then
                            'If (If((regkey.GetValue("") Is Nothing), Nothing, regkey.GetValue("").ToString) <> "AppsUseLightTheme") Then
                            'End If
                            sysacr = regkey.GetValue("AppsUseLightTheme", -1)
                        Else
                            UnSupportDarkSys = 1
                        End If
                    Catch ex As Exception
                        UnSupportDarkSys = 1
                    End Try
                    If appcolor = 0 Then
                        If (Not regkey Is Nothing) Then
                            'If (If((regkey.GetValue("") Is Nothing), Nothing, regkey.GetValue("").ToString) <> "AppsUseLightTheme") Then
                            'End If
                            If sysacr = 0 Then
                                UnSupportDarkSys = 0
                                crmd = 0
                            ElseIf sysacr = 1 Then
                                UnSupportDarkSys = 0
                                crmd = 1
                            Else
                                UnSupportDarkSys = 1
                                crmd = 1
                            End If
                        Else
                            UnSupportDarkSys = 1
                            crmd = 1
                        End If
                    ElseIf appcolor = 1 Then
                        If (Not regkey Is Nothing) Then
                            If sysacr = 0 Then
                                UnSupportDarkSys = 0
                            ElseIf sysacr = 1 Then
                                UnSupportDarkSys = 0
                            Else
                                UnSupportDarkSys = 1
                            End If
                        Else
                            UnSupportDarkSys = 1
                        End If
                        crmd = 1 'Light
                    ElseIf appcolor = 2 Then
                        If (Not regkey Is Nothing) Then
                            If sysacr = 0 Then
                                UnSupportDarkSys = 0
                            ElseIf sysacr = 1 Then
                                UnSupportDarkSys = 0
                            Else
                                UnSupportDarkSys = 1
                            End If
                        Else
                            UnSupportDarkSys = 1
                        End If
                        crmd = 0 'Dark
                    End If

                    regkey.Close()
                Catch ex As Exception
                End Try


                If UnSupportDarkSys = 1 Then
                    If appcolor = 0 Then
                        appcolor = 1

                        AddReg("Software\CJH\TimeControl\Settings", "ColorMode", 1, Microsoft.Win32.RegistryValueKind.DWord, "HKCU")


                        Form2.ComboBox2.SelectedIndex = 0
                    ElseIf appcolor = 1 Then
                        Form2.ComboBox2.SelectedIndex = 0
                    ElseIf appcolor = 2 Then
                        Form2.ComboBox2.SelectedIndex = 1
                    End If

                    Form2.ComboBox2.Items.Clear()
                    Form2.ComboBox2.Items.AddRange(New Object() {"浅色", "深色"})
                End If

                AddHandler SystemEvents.UserPreferenceChanged, AddressOf SystemEvents_UserPreferenceChanged

                '////////////////////////////////////////////////////////////////////////////////////
                '//
                '//  时钟是否启用拖放注册表读取
                '//
                '////////////////////////////////////////////////////////////////////////////////////

                If (Not mykey Is Nothing) Then
                    Me.UseMoveV = mykey.GetValue("EnableDrag", -1)
                    If Me.UseMoveV = -1 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "EnableDrag", 1, RegistryValueKind.DWord, "HKCU")
                        Me.UseMoveV = 1
                    ElseIf Me.UseMoveV > 1 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "EnableDrag", 1, RegistryValueKind.DWord, "HKCU")
                        Me.UseMoveV = 1
                    End If
                Else
                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "EnableDrag", 1, RegistryValueKind.DWord, "HKCU")
                    Me.UseMoveV = 1
                End If

                '////////////////////////////////////////////////////////////////////////////////////
                '//
                '//  时钟是否保存位置注册表读取
                '//
                '////////////////////////////////////////////////////////////////////////////////////

                If (Not mykey Is Nothing) Then
                    Me.SaveLoc = mykey.GetValue("SaveLocations", -1)
                    If Me.SaveLoc = -1 Then

                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "SaveLocations", 0, RegistryValueKind.DWord, "HKCU")


                        Me.SaveLoc = 0
                    ElseIf Me.SaveLoc > 1 Then

                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "SaveLocations", 0, RegistryValueKind.DWord, "HKCU")

                        Me.SaveLoc = 0
                    End If
                Else

                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "SaveLocations", 0, RegistryValueKind.DWord, "HKCU")

                    Me.SaveLoc = 0
                End If

                '////////////////////////////////////////////////////////////////////////////////////
                '//
                '//  时钟是否使用自定义大小注册表读取
                '//
                '////////////////////////////////////////////////////////////////////////////////////

                If (Not mykey Is Nothing) Then
                    Me.MySize = mykey.GetValue("UseCustomSize", -1)
                    If Me.MySize = -1 Then

                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "UseCustomSize", 0, RegistryValueKind.DWord, "HKCU")


                        Me.MySize = 0
                    ElseIf MySize > 1 Then

                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "UseCustomSize", 0, RegistryValueKind.DWord, "HKCU")

                        Me.MySize = 0
                    End If
                Else

                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "UseCustomSize", 0, RegistryValueKind.DWord, "HKCU")
                    Me.MySize = 0
                End If

                '////////////////////////////////////////////////////////////////////////////////////
                '//
                '//  时钟顶置注册表读取
                '//
                '////////////////////////////////////////////////////////////////////////////////////

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
                    ElseIf UseTop > 1 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "AllowTopMost", 1, RegistryValueKind.DWord, "HKCU")
                        Me.TopMost = True
                    End If
                Else
                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "AllowTopMost", 1, RegistryValueKind.DWord, "HKCU")
                    Me.TopMost = True
                End If

                '////////////////////////////////////////////////////////////////////////////////////
                '//
                '//  时钟字体样式注册表读取
                '//
                '////////////////////////////////////////////////////////////////////////////////////
                Dim tfnt As String
                If (Not mykey Is Nothing) Then
                    tfnt = mykey.GetValue("TimeFont", Chr(10))
                    If tfnt = Chr(10) Then
                        tfnt = Me.Label1.Font.Name
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFont", Me.Label1.Font.Name, RegistryValueKind.String, "HKCU")
                    End If
                Else
                    tfnt = Me.Label1.Font.Name
                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFont", Me.Label1.Font.Name, RegistryValueKind.String, "HKCU")
                End If

                Dim tfntpx As Single
                If (Not mykey Is Nothing) Then
                    tfntpx = mykey.GetValue("TimeFontPx", -1)
                    If tfntpx = -1 Then
                        tfntpx = Me.Label1.Font.Size
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontPx", Me.Label1.Font.Size, RegistryValueKind.DWord, "HKCU")
                    End If
                Else
                    tfntpx = Me.Label1.Font.Size
                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontPx", Me.Label1.Font.Size, RegistryValueKind.DWord, "HKCU")
                End If

                Dim tfntit As Integer
                If (Not mykey Is Nothing) Then
                    tfntit = mykey.GetValue("TimeFontItalic", -1)
                    If tfntit = -1 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontItalic", Me.Label1.Font.Italic, RegistryValueKind.DWord, "HKCU")
                        tfntit = Me.Label1.Font.Italic
                    ElseIf tfntit > 1 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontItalic", Me.Label1.Font.Italic, RegistryValueKind.DWord, "HKCU")
                        tfntit = Me.Label1.Font.Italic
                    End If
                Else
                    tfntit = Me.Label1.Font.Italic
                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontItalic", Me.Label1.Font.Italic, RegistryValueKind.DWord, "HKCU")
                End If

                Dim tfntbd As Integer
                If (Not mykey Is Nothing) Then
                    tfntbd = mykey.GetValue("TimeFontBold", -1)
                    If tfntbd = -1 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontBold", Me.Label1.Font.Bold, RegistryValueKind.DWord, "HKCU")
                        tfntbd = Me.Label1.Font.Bold
                    ElseIf tfntbd > 1 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontBold", Me.Label1.Font.Bold, RegistryValueKind.DWord, "HKCU")
                        tfntbd = Me.Label1.Font.Bold
                    End If
                Else
                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontBold", Me.Label1.Font.Bold, RegistryValueKind.DWord, "HKCU")
                    tfntbd = Me.Label1.Font.Bold
                End If

                Dim tfntul As Integer
                If (Not mykey Is Nothing) Then
                    tfntul = mykey.GetValue("TimeFontUnderLine", -1)
                    If tfntul = -1 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontUnderLine", Me.Label1.Font.Underline, RegistryValueKind.DWord, "HKCU")
                        tfntul = Me.Label1.Font.Underline
                    ElseIf tfntul > 1 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontUnderLine", Me.Label1.Font.Underline, RegistryValueKind.DWord, "HKCU")
                        tfntul = Me.Label1.Font.Underline
                    End If
                Else
                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontUnderLine", Me.Label1.Font.Underline, RegistryValueKind.DWord, "HKCU")
                    tfntul = Me.Label1.Font.Underline
                End If

                Dim tfntst As Integer
                If (Not mykey Is Nothing) Then
                    tfntst = mykey.GetValue("TimeFontStrikeout", -1)
                    If tfntst = -1 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontStrikeout", Me.Label1.Font.Strikeout, RegistryValueKind.DWord, "HKCU")
                        tfntul = Me.Label1.Font.Underline
                    ElseIf tfntst > 1 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontStrikeout", Me.Label1.Font.Strikeout, RegistryValueKind.DWord, "HKCU")
                        tfntul = Me.Label1.Font.Underline
                    End If
                Else
                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontStrikeout", Me.Label1.Font.Strikeout, RegistryValueKind.DWord, "HKCU")
                    tfntul = Me.Label1.Font.Underline
                End If

                Dim fntype As Object

                If tfntit = 1 And tfntbd = 1 And tfntul = 1 And tfntst = 1 Then
                    fntype = CType((System.Drawing.FontStyle.Underline Or System.Drawing.FontStyle.Strikeout Or System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle)

                ElseIf tfntit = 1 And tfntbd = 1 And tfntul = 1 Then
                    '123
                    fntype = CType((System.Drawing.FontStyle.Underline Or System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle)
                ElseIf tfntit = 1 And tfntbd = 1 And tfntst = 1 Then
                    '124
                    fntype = CType((System.Drawing.FontStyle.Strikeout Or System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle)
                ElseIf tfntbd = 1 And tfntul = 1 And tfntst = 1 Then
                    '234
                    fntype = CType((System.Drawing.FontStyle.Underline Or System.Drawing.FontStyle.Strikeout Or System.Drawing.FontStyle.Bold), System.Drawing.FontStyle)
                ElseIf tfntit = 1 And tfntul = 1 And tfntst = 1 Then
                    '134
                    fntype = CType((System.Drawing.FontStyle.Underline Or System.Drawing.FontStyle.Strikeout Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle)

                ElseIf tfntit = 1 And tfntbd = 1 Then
                    '12
                    fntype = CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle)
                ElseIf tfntit = 1 And tfntul = 1 Then
                    '13
                    fntype = CType((System.Drawing.FontStyle.Underline Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle)
                ElseIf tfntbd = 1 And tfntul = 1 Then
                    '23
                    fntype = CType((System.Drawing.FontStyle.Underline Or System.Drawing.FontStyle.Bold), System.Drawing.FontStyle)
                ElseIf tfntit = 1 And tfntst = 1 Then
                    '14
                    fntype = CType((System.Drawing.FontStyle.Strikeout Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle)
                ElseIf tfntbd = 1 And tfntst = 1 Then
                    '24
                    fntype = CType((System.Drawing.FontStyle.Strikeout Or System.Drawing.FontStyle.Bold), System.Drawing.FontStyle)
                ElseIf tfntul = 1 And tfntst = 1 Then
                    '34
                    fntype = CType((System.Drawing.FontStyle.Underline Or System.Drawing.FontStyle.Strikeout), System.Drawing.FontStyle)

                ElseIf tfntbd = 1 Then
                    '2
                    fntype = System.Drawing.FontStyle.Bold
                ElseIf tfntit = 1 Then
                    '1
                    fntype = System.Drawing.FontStyle.Italic
                ElseIf tfntst = 1 Then
                    '4
                    fntype = System.Drawing.FontStyle.Strikeout
                ElseIf tfntul = 1 Then
                    '3
                    fntype = System.Drawing.FontStyle.Underline
                Else
                    fntype = System.Drawing.FontStyle.Regular
                End If
                'CType((System.Drawing.FontStyle.Underline Or System.Drawing.FontStyle.Strikeout)
                Label1.Font = New System.Drawing.Font(tfnt, tfntpx!, fntype, System.Drawing.GraphicsUnit.Point)
                Form2.FontDialog1.Font = Label1.Font


                '////////////////////////////////////////////////////////////////////////////////////
                '//
                '//  时钟字体颜色注册表读取
                '//
                '////////////////////////////////////////////////////////////////////////////////////
                Dim tfntcr As Integer
                If (Not mykey Is Nothing) Then
                    tfntcr = mykey.GetValue("TimeFontR", -1)
                    If tfntcr = -1 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontR", Me.Label1.ForeColor.R, RegistryValueKind.DWord, "HKCU")
                        tfntcr = Me.Label1.ForeColor.R
                        If tfntcr > 255 Then
                            RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontR", Me.Label1.ForeColor.R, RegistryValueKind.DWord, "HKCU")
                            tfntcr = Me.Label1.ForeColor.R
                        End If
                    End If
                Else
                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontR", Me.Label1.ForeColor.R, RegistryValueKind.DWord, "HKCU")
                    tfntcr = Me.Label1.ForeColor.R
                End If

                Dim tfntcg As Integer
                If (Not mykey Is Nothing) Then
                    tfntcg = mykey.GetValue("TimeFontG", -1)
                    If tfntcg = -1 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontG", Me.Label1.ForeColor.G, RegistryValueKind.DWord, "HKCU")
                        tfntcg = Me.Label1.ForeColor.G
                        If tfntcg > 255 Then
                            RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontG", Me.Label1.ForeColor.G, RegistryValueKind.DWord, "HKCU")
                            tfntcg = Me.Label1.ForeColor.G
                        End If
                    End If
                Else
                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontG", Me.Label1.ForeColor.G, RegistryValueKind.DWord, "HKCU")
                    tfntcg = Me.Label1.ForeColor.G
                End If

                Dim tfntcb As Integer
                If (Not mykey Is Nothing) Then
                    tfntcb = mykey.GetValue("TimeFontB", -1)
                    If tfntcb = -1 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontB", Me.Label1.ForeColor.B, RegistryValueKind.DWord, "HKCU")
                        tfntcb = Me.Label1.ForeColor.B
                        If tfntcb > 255 Then
                            RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontB", Me.Label1.ForeColor.B, RegistryValueKind.DWord, "HKCU")
                            tfntcb = Me.Label1.ForeColor.B
                        End If
                    End If
                Else
                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontB", Me.Label1.ForeColor.B, RegistryValueKind.DWord, "HKCU")
                    tfntcb = Me.Label1.ForeColor.B
                End If

                Label1.ForeColor = Color.FromArgb(tfntcr, tfntcg, tfntcb)
                Form2.ColorDialog1.Color = Label1.ForeColor

                '////////////////////////////////////////////////////////////////////////////////////
                '//
                '//  时钟显示格式注册表读取
                '//
                '////////////////////////////////////////////////////////////////////////////////////

                Dim tf As Integer
                If (Not mykey Is Nothing) Then
                    tf = mykey.GetValue("TimeFormat", -1)
                    If tf = -1 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFormat", 0, RegistryValueKind.DWord, "HKCU")
                        tf = 0
                    ElseIf tf > 6 Then
                        tf = 0
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFormat", 0, RegistryValueKind.DWord, "HKCU")
                    End If
                Else
                    tf = 0
                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFormat", 0, RegistryValueKind.DWord, "HKCU")
                End If
                Form2.ComboBox3.SelectedIndex = tf

                Dim tr As String
                If (Not mykey Is Nothing) Then
                    tr = mykey.GetValue("CustomFormat", Chr(10))
                    If tr = Chr(10) Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "CustomFormat", "HH:mm:ss", RegistryValueKind.String, "HKCU")
                        tr = "HH:mm:ss"
                    End If
                Else
                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "CustomFormat", "HH:mm:ss", RegistryValueKind.String, "HKCU")
                    tr = "HH:mm:ss"
                End If
                Form2.TextBox2.Text = tr
                '////////////////////////////////////////////////////////////////////////////////////
                '//
                '//  时钟自定义大小设置注册表读取
                '//
                '////////////////////////////////////////////////////////////////////////////////////
                Dim tformw As Integer
                Dim tformh As Integer
                If (Not mykey Is Nothing) Then
                    tformw = mykey.GetValue("CustomWidth", -1)
                    If tformw <= 0 Then
                        If Me.MySize = 1 Then
                            RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "CustomWidth", Me.Width, RegistryValueKind.DWord, "HKCU")
                        End If
                        tformw = Me.Width
                    End If
                Else
                    If Me.MySize = 1 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "CustomWidth", Me.Width, RegistryValueKind.DWord, "HKCU")
                    End If
                    tformw = Me.Width
                End If
                If (Not mykey Is Nothing) Then
                    tformh = mykey.GetValue("CustomHeight", -1)
                    If tformh <= 0 Then
                        If Me.MySize = 1 Then
                            RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "CustomHeight", Me.Height, RegistryValueKind.DWord, "HKCU")
                        End If
                        tformh = Me.Height
                    End If
                Else
                    If Me.MySize = 1 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "CustomHeight", Me.Height, RegistryValueKind.DWord, "HKCU")
                    End If
                    tformh = Me.Height
                End If
                Form2.TextBox3.Text = tformw
                Form2.TextBox4.Text = tformh
                If Me.MySize = 1 Then
                    Me.Height = tformh
                    Me.Width = tformw
                End If

                '////////////////////////////////////////////////////////////////////////////////////
                '//
                '//  时钟位置注册表读取
                '//
                '////////////////////////////////////////////////////////////////////////////////////
                a.X = (System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width - Me.Width) / 2
                a.Y = 5 * disi.DpiY * 0.01
                If SaveLoc = 1 Then
                    Dim aa As Integer
                    Dim ba As Integer
                    If (Not mykey Is Nothing) Then
                        aa = mykey.GetValue("TimeFormX", -1)
                        If aa = -1 Then
                            RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFormX", (System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width - Me.Width) / 2, RegistryValueKind.DWord, "HKCU")
                            aa = (System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width - Me.Width) / 2
                        ElseIf aa < 0 - Me.Width Then
                            RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFormX", (System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width - Me.Width) / 2, RegistryValueKind.DWord, "HKCU")
                            aa = 0
                        ElseIf aa > System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width Then
                            RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFormX", (System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width - Me.Width) / 2, RegistryValueKind.DWord, "HKCU")
                            aa = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width - Me.Width
                        Else
                            a.X = aa
                        End If
                    Else
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFormX", (System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width - Me.Width) / 2, RegistryValueKind.DWord, "HKCU")
                        aa = (System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width - Me.Width) / 2
                    End If
                    If (Not mykey Is Nothing) Then
                        ba = mykey.GetValue("TimeFormY", -1)
                        If ba = -1 Then
                            RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFormY", 5, RegistryValueKind.DWord, "HKCU")
                            ba = 5
                        ElseIf ba < 0 - Me.Height Then
                            RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFormY", 5, RegistryValueKind.DWord, "HKCU")
                            ba = 5
                        ElseIf ba > System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height Then
                            RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFormY", 5, RegistryValueKind.DWord, "HKCU")
                            ba = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height - Me.Height
                        Else
                            a.Y = ba
                        End If
                    Else
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFormY", 5, RegistryValueKind.DWord, "HKCU")
                        ba = 5
                    End If
                End If


                Me.Location = a

                AddHandler Form2.ComboBox3.SelectedIndexChanged, AddressOf Form2.ComboBox3_SelectedIndexChanged
                Call Form2.ComboBox3_SelectedIndexChanged(sender, e)
                If Form2.ComboBox3.SelectedIndex = 6 Then
                    Call Form2.Button4_Click(sender, e)
                End If

                '////////////////////////////////////////////////////////////////////////////////////
                '//
                '//  时钟主题注册表读取
                '//
                '////////////////////////////////////////////////////////////////////////////////////
                If (Not mykey Is Nothing) Then
                    TimeTheme = mykey.GetValue("TimeTheme", -1)
                    If TimeTheme = -1 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeTheme", 0, RegistryValueKind.DWord, "HKCU")
                        TimeTheme = 0
                    ElseIf TimeTheme > 3 Then
                        TimeTheme = 0
                    End If
                Else
                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeTheme", 0, RegistryValueKind.DWord, "HKCU")
                    TimeTheme = 0
                End If

                If TimeTheme = 2 Then
                    Dim cthp As String
                    If (Not mykey Is Nothing) Then
                        cthp = mykey.GetValue("CustomThemePath", Chr(10))
                        If cthp = Chr(10) Then
                            RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "CustomThemePath", "", RegistryValueKind.String, "HKCU")
                            cthp = ""
                        End If
                    Else
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "CustomThemePath", "", RegistryValueKind.String, "HKCU")
                        cthp = ""
                    End If
                    If cthp <> "" Then
                        Try
                            BackgroundImage = Image.FromFile(cthp)
                            Form2.TextBox5.Text = cthp
                            Form2.OpenFileDialog1.FileName = cthp
                            TransparencyKey = Color.FromArgb(255, 0, 255)
                        Catch ex As Exception
                            MsgBox("加载自定义背景图片失败。已恢复默认图片。" & vbCrLf & ex.Message, MsgBoxStyle.Critical, "错误")
                            Form2.OpenFileDialog1.FileName = ""
                            Form2.TextBox5.Text = ""
                            If Width >= 250 Then
                                If crmd = 0 Then
                                    BackgroundImage = My.Resources.bkgdark400
                                Else
                                    BackgroundImage = My.Resources.bkg400
                                End If
                            ElseIf Width <= 70 Then
                                If crmd = 0 Then
                                    BackgroundImage = My.Resources.bkgdark50
                                Else
                                    BackgroundImage = My.Resources.bkg50
                                End If
                            Else
                                If crmd = 0 Then
                                    BackgroundImage = My.Resources.bkgdark
                                Else
                                    BackgroundImage = My.Resources.bkg
                                End If
                            End If
                            If crmd = 0 Then
                                TransparencyKey = Color.FromArgb(1, 1, 1)
                            Else
                                TransparencyKey = Color.FromArgb(184, 184, 184)
                            End If
                            RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "CustomThemePath", "", RegistryValueKind.String, "HKCU")
                        End Try
                    End If
                End If

                '////////////////////////////////////////////////////////////////////////////////////
                '//
                '//  透明度注册表读取
                '//
                '////////////////////////////////////////////////////////////////////////////////////
                If (Not mykey Is Nothing) Then
                    CustOpacity = mykey.GetValue("TimeFormOpacity", -1)
                    If CustOpacity = -1 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFormOpacity", 100, RegistryValueKind.DWord, "HKCU")
                        CustOpacity = 100
                    ElseIf CustOpacity < 20 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFormOpacity", 20, RegistryValueKind.DWord, "HKCU")
                        CustOpacity = 20
                    ElseIf CustOpacity > 100 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFormOpacity", 100, RegistryValueKind.DWord, "HKCU")
                        CustOpacity = 100
                    End If
                Else
                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFormOpacity", 100, RegistryValueKind.DWord, "HKCU")
                    CustOpacity = 100
                End If
                If TimeTheme = 2 Or 3 Then
                    Opacity = CustOpacity * 0.01
                End If

                Form2.Label17.Text = CustOpacity & "%"
                Form2.TrackBar1.Value = CustOpacity

                mykey.Close()

            Catch ex As Exception
                MsgBox("读取注册表设置发生错误。" & vbCrLf & ex.Message, MsgBoxStyle.Critical, "错误")
            End Try
        End If


        Call formatcolorcur()
        Call Form2.formatcolorcurset()
        Call MsgForm.formatcolorcursetmsg()
        Call GPLForm.formatcolorcursetmsg()
        IsBootV = 0
    End Sub

    'API移动窗体
    Declare Function SendMessage Lib "user32" Alias "SendMessageA" (ByVal hwnd As IntPtr, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Boolean
    Declare Function ReleaseCapture Lib "user32" Alias "ReleaseCapture" () As Boolean
    Const WM_SYSCOMMAND = &H112
    Const SC_MOVE = &HF010&
    Const HTCAPTION = 2
    Private Sub Form1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown
        If Me.WindowState = FormWindowState.Normal Then
            If UseMoveV = 1 Then
                ReleaseCapture()
                SendMessage(Me.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0)
                MovedV = 1
                If SaveLoc = 1 Then
                    If UnSaveData = 0 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFormX", Me.Location.X, RegistryValueKind.DWord, "HKCU")
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFormY", Me.Location.Y, RegistryValueKind.DWord, "HKCU")
                    End If

                End If
            End If
        End If
    End Sub
    Private Sub Label1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Label1.MouseDown
        If Me.WindowState = FormWindowState.Normal Then
            If UseMoveV = 1 Then
                ReleaseCapture()
                SendMessage(Me.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0)
                MovedV = 1
                If SaveLoc = 1 Then
                    If UnSaveData = 0 Then
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFormX", Me.Location.X, RegistryValueKind.DWord, "HKCU")
                        RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFormY", Me.Location.Y, RegistryValueKind.DWord, "HKCU")
                    End If

                End If
            End If
        End If
    End Sub
    Sub formatcolorcur()
        If crmd = 0 Then
            If Me.WindowState = FormWindowState.Normal Then
                If TimeTheme = 0 Then
                    If Me.Width >= 250 Then
                        Me.BackgroundImage = My.Resources.bkgdark400
                    ElseIf Me.Width <= 70 Then
                        Me.BackgroundImage = My.Resources.bkgdark50
                    Else
                        Me.BackgroundImage = My.Resources.bkgdark
                    End If
                    Me.TransparencyKey = Color.FromArgb(1, 1, 1)
                    Opacity = 0.99
                ElseIf TimeTheme = 1 Then
                    Me.BackgroundImage = Nothing
                    Me.TransparencyKey = Color.FromArgb(255, 0, 255)
                    Opacity = 0.7
                ElseIf TimeTheme = 3 Then
                    Me.BackgroundImage = Nothing
                    Me.TransparencyKey = Color.FromArgb(255, 0, 255)
                    Opacity = CustOpacity * 0.01
                Else
                    Opacity = CustOpacity * 0.01
                End If

            End If

            If Label1.ForeColor.R = 0 And Label1.ForeColor.G = 0 And Label1.ForeColor.B = 0 Then
                Me.Label1.ForeColor = Color.White
                If UnSaveData = 0 Then
                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontR", Label1.ForeColor.R, RegistryValueKind.DWord, "HKCU")
                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontG", Label1.ForeColor.G, RegistryValueKind.DWord, "HKCU")
                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontB", Label1.ForeColor.B, RegistryValueKind.DWord, "HKCU")
                End If
                Form2.ColorDialog1.Color = Color.White
            End If
            Me.ContextMenuStrip1.BackColor = Color.FromArgb(32, 32, 32)
            Me.ContextMenuStrip1.ForeColor = Color.White
            Me.ContextMenuStrip2.BackColor = Color.FromArgb(32, 32, 32)
            Me.ContextMenuStrip2.ForeColor = Color.White
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

        Else
            If Me.WindowState = FormWindowState.Normal Then
                If TimeTheme = 0 Then
                    If Me.Width >= 250 Then
                        Me.BackgroundImage = My.Resources.bkg400
                    ElseIf Me.Width <= 70 Then
                        Me.BackgroundImage = My.Resources.bkg50
                    Else
                        Me.BackgroundImage = My.Resources.bkg
                    End If
                    Me.TransparencyKey = Color.FromArgb(184, 184, 184)
                    Opacity = 0.99
                ElseIf TimeTheme = 1 Then
                    Me.BackgroundImage = Nothing
                    Me.TransparencyKey = Color.FromArgb(255, 0, 255)
                    Opacity = 0.7
                ElseIf TimeTheme = 3 Then
                    Me.BackgroundImage = Nothing
                    Me.TransparencyKey = Color.FromArgb(255, 0, 255)
                    Opacity = CustOpacity * 0.01
                Else
                    Opacity = CustOpacity * 0.01
                End If
            End If
            Me.BackColor = Color.White
            Me.ForeColor = Color.Black
            If Label1.ForeColor.R = 255 And Label1.ForeColor.G = 255 And Label1.ForeColor.B = 255 Then
                Me.Label1.ForeColor = Color.Black
                If UnSaveData = 0 Then
                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontR", Label1.ForeColor.R, RegistryValueKind.DWord, "HKCU")
                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontG", Label1.ForeColor.G, RegistryValueKind.DWord, "HKCU")
                    RegKeyModule.AddReg("Software\CJH\TimeControl\Settings", "TimeFontB", Label1.ForeColor.B, RegistryValueKind.DWord, "HKCU")
                End If
                Form2.ColorDialog1.Color = Color.Black
            End If
            Me.ContextMenuStrip1.BackColor = Color.White
            Me.ContextMenuStrip1.ForeColor = Color.Black
            Me.ContextMenuStrip2.BackColor = Color.White
            Me.ContextMenuStrip2.ForeColor = Color.Black
            EnableDarkModeForWindow(Me.Handle, False)
        End If
    End Sub
    Private Sub h30s_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles h30s.Click
        Timer2.Interval = 30000
        NotifyIcon1.Visible = True
        NotifyIcon1.ShowBalloonTip(7000, "时间小工具", "时间小工具当前已隐藏到系统托盘，双击托盘图标或在设定的时间（30秒）之后重新显示。", ToolTipIcon.Info)
        Me.Hide()
        Timer2.Enabled = True
    End Sub

    Private Sub h1m_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles h1m.Click
        Timer2.Interval = 60000
        NotifyIcon1.Visible = True
        NotifyIcon1.ShowBalloonTip(7000, "时间小工具", "时间小工具当前已隐藏到系统托盘，双击托盘图标或在设定的时间（1分钟）之后重新显示。", ToolTipIcon.Info)
        Me.Hide()
        Timer2.Enabled = True
    End Sub

    Private Sub h5m_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles h5m.Click
        Timer2.Interval = 300000
        NotifyIcon1.Visible = True
        NotifyIcon1.ShowBalloonTip(7000, "时间小工具", "时间小工具当前已隐藏到系统托盘，双击托盘图标或在设定的时间（5分钟）之后重新显示。", ToolTipIcon.Info)
        Me.Hide()
        Timer2.Enabled = True
    End Sub

    Private Sub h10m_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles h10m.Click
        Timer2.Interval = 600000
        NotifyIcon1.Visible = True
        NotifyIcon1.ShowBalloonTip(7000, "时间小工具", "时间小工具当前已隐藏到系统托盘，双击托盘图标或在设定的时间（10分钟）之后重新显示。", ToolTipIcon.Info)
        Me.Hide()
        Timer2.Enabled = True
    End Sub

    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        Me.Show()
        NotifyIcon1.Visible = False
        Timer2.Enabled = False
    End Sub

    Private Sub ext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ext.Click
        'If MessageBox.Show("确定要关闭时钟吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = MsgBoxResult.Yes Then
        'End
        'End If
        NeedStillTopMost = 0
        Form2.ShowDialog()
        NeedStillTopMost = 1
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
                Call MsgForm.formatcolorcursetmsg()
                Call GPLForm.formatcolorcursetmsg()
            End If
        End If
    End Sub

    Private Sub NotifyIcon1_MouseDoubleClick(sender As System.Object, e As System.Windows.Forms.MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        Me.Show()
        Timer2.Enabled = False
        NotifyIcon1.Visible = False
    End Sub

    Private Sub shtbar_Click(sender As System.Object, e As System.EventArgs) Handles shtbar.Click
        Me.Show()
        Timer2.Enabled = False
        NotifyIcon1.Visible = False
    End Sub

    Private Sub FullM_Click(sender As System.Object, e As System.EventArgs) Handles FullM.Click
        If Me.WindowState = FormWindowState.Normal Then
            Me.WindowState = FormWindowState.Maximized
            'If TimeTheme = 0 Or TimeTheme = 1 Or TimeTheme = 3 Then
            If TimeTheme = 2 Then
                If Form2.TextBox5.Text = "" Then
                    Me.BackgroundImage = Nothing
                End If
            Else
                Me.BackgroundImage = Nothing
            End If
            'End If
            If TimeTheme = 0 Or 1 Then
                Opacity = 1
            End If
            Me.FullM.Text = "退出全屏"
        Else
            Me.WindowState = FormWindowState.Normal
            If TimeTheme = 0 Then
                If crmd = 0 Then
                    If Me.Width >= 250 Then
                        Me.BackgroundImage = My.Resources.bkgdark400
                    ElseIf Me.Width <= 70 Then
                        Me.BackgroundImage = My.Resources.bkgdark50
                    Else
                        Me.BackgroundImage = My.Resources.bkgdark
                    End If
                    Me.TransparencyKey = Color.FromArgb(1, 1, 1)
                    Opacity = 0.99
                Else
                    If Me.Width >= 250 Then
                        Me.BackgroundImage = My.Resources.bkg400
                    ElseIf Me.Width <= 70 Then
                        Me.BackgroundImage = My.Resources.bkg50
                    Else
                        Me.BackgroundImage = My.Resources.bkg
                    End If
                    Me.TransparencyKey = Color.FromArgb(184, 184, 184)
                    Opacity = 0.99
                End If
            ElseIf TimeTheme = 1 Then
                Me.BackgroundImage = Nothing
                'Me.TransparencyKey = Color.FromArgb(255, 0, 255)
                Opacity = 0.7
            ElseIf TimeTheme = 2 Then
                If Form2.TextBox5.Text = "" Then
                    If crmd = 0 Then
                        If Me.Width >= 250 Then
                            Me.BackgroundImage = My.Resources.bkgdark400
                        ElseIf Me.Width <= 70 Then
                            Me.BackgroundImage = My.Resources.bkgdark50
                        Else
                            Me.BackgroundImage = My.Resources.bkgdark
                        End If
                    Else
                        If Me.Width >= 250 Then
                            Me.BackgroundImage = My.Resources.bkg400
                        ElseIf Me.Width <= 70 Then
                            Me.BackgroundImage = My.Resources.bkg50
                        Else
                            Me.BackgroundImage = My.Resources.bkg
                        End If
                    End If
                End If
                Opacity = CustOpacity * 0.01
            ElseIf TimeTheme = 3 Then
                'Me.TransparencyKey = Color.FromArgb(255, 0, 255)
                Opacity = CustOpacity * 0.01
            Else
                Opacity = CustOpacity * 0.01
            End If
            Me.FullM.Text = "全屏"
        End If
    End Sub
End Class
