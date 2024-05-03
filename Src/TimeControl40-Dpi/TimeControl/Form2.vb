Public Class Form2
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        On Error GoTo errcode
            If TextBox1.Text = "" Then
                MsgBox("隐藏时间不能为空！", MsgBoxStyle.Critical, "错误")
            ElseIf TextBox1.Text = "0" Then
                MsgBox("隐藏时间不能为0！", MsgBoxStyle.Critical, "错误")
            Else
                    If ComboBox1.SelectedIndex = 0 Then '秒
                    Form1.Timer2.Interval = TextBox1.Text * 1000
                    Form1.Timer1.Enabled = False
                    Form1.Timer2.Enabled = True
                        Form1.Hide()
                        Me.Close()
                    ElseIf ComboBox1.SelectedIndex = 1 Then '分
                    Form1.Timer2.Interval = TextBox1.Text * 1000 * 60
                    Form1.Timer1.Enabled = False
                        Form1.Timer2.Enabled = True
                        Form1.Hide()
                        Me.Close()
                    ElseIf ComboBox1.SelectedIndex = 2 Then '时
                    Form1.Timer2.Interval = TextBox1.Text * 1000 * 60 * 60
                    Form1.Timer1.Enabled = False
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
        ComboBox1.SelectedIndex = 0
        ComboBox1.SelectedText = "秒"
        Label1.Text = "时钟小工具 版本：" & My.Application.Info.Version.ToString & "开发分支：" & Form1.DEVBRANCH & vbCrLf & vbCrLf & "版权所有 © 2023 CJH。保留所有权利。"
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

    Private Sub CheckBox1_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBox1.CheckedChanged
        If Form1.TopMost = True Then
            CheckBox1.Checked = False
            Form1.TopMost = False
        Else
            CheckBox1.Checked = True
            Form1.TopMost = True
        End If
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBox2.CheckedChanged
        If Form1.UseMoveV = 1 Then
            CheckBox2.Checked = False
            Form1.UseMoveV = 0
        Else
            CheckBox2.Checked = True
            Form1.UseMoveV = 1
        End If
    End Sub

    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click
        If (MessageBox.Show("确定退出时钟小工具吗？", "时钟小工具", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.Yes) Then
            End
        End If
    End Sub
End Class