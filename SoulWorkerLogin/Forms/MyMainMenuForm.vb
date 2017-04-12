Imports System.IO
Imports Microsoft.Win32
Imports SoulWorkerLogin.Hangame

Public Class MyMainMenuForm

    Private WithEvents theHangameClient As HangameAgent
    Private WithEvents PubCloneClient As PubClone
    Private CaptchaImgBox As CaptchaPanel
    Private _NotifyMode As Boolean

    Private WithEvents PWatcher_SW As Classes.ProcessWatcher
    Private WithEvents PWatcher_SWUpdater As Classes.ProcessWatcher

    Public Property LoggedInNotifyMode() As Boolean
        Get
            Return Me._NotifyMode
        End Get
        Set(value As Boolean)
            Me._NotifyMode = value
            If value Then
                DefineValues.Notify.NotifyIcon.ContextMenuStrip = DefineValues.Notify.LoginContextMenuStrip
            Else
                DefineValues.Notify.NotifyIcon.ContextMenuStrip = DefineValues.Notify.LogoutContextMenuStrip
            End If
        End Set
    End Property

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.haru_sd_WM1UAm_256px
        Me.AllowHideToTaskbar = True
        'If My.Computer.Info.OSFullName.IndexOf("Windows 7") > -1 Then
        '    asasd.TitleColor = Color.SkyBlue
        'End If

        Me.PubCloneClient = New PubClone()
        Me.theHangameClient = New HangameAgent(Me.PubCloneClient)
        Me.CaptchaImgBox = New CaptchaPanel(Me.theHangameClient)
        Dim theX As Integer = Convert.ToInt32(Me.Size.Width / 2)
        Dim sizeX As Integer = Convert.ToInt32(Me.CaptchaImgBox.Size.Width / 2)

        '99
        Me.boolean_FormClosing = False
        Me.LoggedInNotifyMode = False
        Me.CaptchaImgBox.Location = New Point(theX - sizeX, 97)
        Me.MyMainMenuPain.Controls.Add(Me.CaptchaImgBox)
        Me.CaptchaImgBox.BringToFront()
        Me.cacheBoolean_CaptchaBox = False

        PanelLogin.Visible = False
        PanelPrepare.Visible = True
        PanelMain.Visible = False
        'Dim myTitleBarMenu = Leayal.Forms.TheCodeKing.ActiveButtons.Controls.GetInstance(Me)
        'Dim myHideToTrayButton = New Leayal.Forms.TheCodeKing.ActiveButtons.Controls.ActiveButton()
        'myHideToTrayButton.Text = "asdasd"
        'myTitleBarMenu.Items.Add(myHideToTrayButton)

        Me.PubCloneClient.DownloadReactor()
        If (CommonMethods.IsWindowsXP()) Then
            MessageBox.Show("SoulWorker Client may not run properly on Windows XP.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        If (CommonMethods.IsSoulWorkerInstalled()) Then
            Classes.Patches.PatchManager.Instance.CheckForUpdates()
        End If
        Me.PWatcher_SW = CommonMethods.SoulWorkerProcessWatcher
        Me.PWatcher_SWUpdater = CommonMethods.SoulWorkerUpdaterProcessWatcher

        '○‬●◯❍✪
        Me.InitialItemsForNotifyIcon()
    End Sub

    Protected Overrides Sub OnSizeChanged(e As EventArgs)
        MyBase.OnSizeChanged(e)
        If (Me.Size.Width > 0 AndAlso Me.Size.Height > 0) Then Me.theSize = Me.Size
    End Sub

    Private Sub ButtonCloseForm_Click(sender As Object, e As EventArgs)
        Me.Close()
    End Sub

    Private Sub InitialItemsForNotifyIcon()
        AddHandler DefineValues.Notify.NotifyIcon.DoubleClick, AddressOf ShowMeUp
        DefineValues.Notify.LoginContextMenuStrip.Items.Add(New ToolStripMenuItem("Logout", Nothing, AddressOf ButtonLogout_Click))
        DefineValues.Notify.LoginContextMenuStrip.Items.Add(New ToolStripSeparator())
        DefineValues.Notify.LogoutContextMenuStrip.Items.Add(New ToolStripMenuItem("About", Nothing, AddressOf ButtonAbout_Click))
        DefineValues.Notify.LoginContextMenuStrip.Items.Add(New ToolStripMenuItem("About", Nothing, AddressOf ButtonAbout_Click))
        DefineValues.Notify.LogoutContextMenuStrip.Items.Add(New ToolStripMenuItem("Exit", Nothing, AddressOf ButtonExit_Click))
        DefineValues.Notify.LoginContextMenuStrip.Items.Add(New ToolStripMenuItem("Exit", Nothing, AddressOf ButtonExit_Click))
        DefineValues.Notify.LogoutContextMenuStrip.Items.Insert(0, New ToolStripSeparator())
        DefineValues.Notify.LoginContextMenuStrip.Items.Insert(0, New ToolStripSeparator())
        DefineValues.Notify.LoginContextMenuStrip.Items.Insert(0, New ToolStripMenuItem("Launch game launcher", Nothing, AddressOf ButtonLaunchReactor_Click))
        DefineValues.Notify.LoginContextMenuStrip.Items.Insert(0, New ToolStripMenuItem("Launch game", Nothing, AddressOf GameStart_Click))
        Dim aitem As ToolStripMenuItem = New ToolStripMenuItem("Show", Nothing, AddressOf ShowMeUp)
        aitem.Font = New Font(aitem.Font.FontFamily, aitem.Font.Size, FontStyle.Bold)
        DefineValues.Notify.LogoutContextMenuStrip.Items.Insert(0, aitem)
        DefineValues.Notify.LoginContextMenuStrip.Items.Insert(0, New ToolStripSeparator())
        aitem = New ToolStripMenuItem("Show", Nothing, AddressOf ShowMeUp)
        aitem.Font = New Font(aitem.Font.FontFamily, aitem.Font.Size, FontStyle.Bold)
        DefineValues.Notify.LoginContextMenuStrip.Items.Insert(0, aitem)
        aitem = Nothing
    End Sub

    Private Sub ShowMeUp(sender As Object, e As EventArgs)
        Me.WindowState = Leayal.Forms.IWindowState.Normal
        Me.BringToFront()
    End Sub

    Private Sub Form1_Shown(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Shown
        DefineValues.Notify.Refresh()
        'setCaptchaBox(True)
    End Sub

    Private Sub PWatcher_ProcessLaunched(ByVal sender As Object, ByVal e As EventArgs) Handles PWatcher_SW.ProcessLaunched
        Classes.Patches.PatchManager.Instance.CancelAsync()
        If (Classes.Patches.PatchManager.Instance.IsPatchInstalled()) Then
            Classes.Patches.PatchManager.Instance.CheckForUpdates()
        End If
        If (Classes.Patches.PatchManager.Instance.Host IsNot Nothing) Then
            Classes.Patches.PatchManager.Instance.Host.Close()
        End If
    End Sub

    Private Sub PWatcher_SWUpdater_ProcessExited(ByVal sender As Object, ByVal e As EventArgs) Handles PWatcher_SWUpdater.ProcessExited
        Classes.Patches.PatchManager.Instance.CancelAsync()
        If (Classes.Patches.PatchManager.Instance.IsPatchInstalled()) Then
            Classes.Patches.PatchManager.Instance.CheckForUpdates()
        End If
        If (Classes.Patches.PatchManager.Instance.Host IsNot Nothing) Then
            Classes.Patches.PatchManager.Instance.Host.Close()
        End If
    End Sub

    Private Sub PubCloneClient_ProgressChanged(ByVal sender As Object, ByVal e As PubCloneProgressChangedEventArg) Handles PubCloneClient.ProgressChanged
        LabelProgresStep.Text = "Downloading: " & e.Filename
        ProgressBar1.Maximum = e.Total
        ProgressBar1.Value = e.Count
    End Sub

    Private Sub PubCloneClient_DownloadCompleted(ByVal sender As Object, ByVal e As EventArgs) Handles PubCloneClient.DownloadCompleted
        PanelLogin.Visible = True
        PanelPrepare.Visible = False
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles ButtonLogin.Click
        Try
            If (String.IsNullOrWhiteSpace(Me.TextBox_ID.Text)) Then
                MessageBox.Show("Invalid Username")
            ElseIf (String.IsNullOrEmpty(Me.TextBox_PW.Text)) Then
                MessageBox.Show("Invalid Password")
            Else
                PanelLoading.Visible = True
                If (Me.cacheBoolean_CaptchaBox) Then
                    Me.theHangameClient.Login(Me.TextBox_ID.Text, Me.TextBox_PW.Text, Me.CaptchaImgBox.CaptchaID, Me.CaptchaImgBox.CaptchaString)
                Else
                    Me.theHangameClient.Login(Me.TextBox_ID.Text, Me.TextBox_PW.Text)
                End If
                PanelLogin.Visible = False
            End If
        Catch ex As HangameAgentLoginException
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        'MessageBox.Show(System.Text.Encoding.UTF8.GetString(thebyte))
    End Sub

    Private Sub theHangameClient_LoggedIn(ByVal sender As Object, ByVal e As HangameAgentLoginFinishedEventArg) Handles theHangameClient.LoggedIn
        PanelLoading.Visible = False
        If (e.Success) Then
            setCaptchaBox(False)
            Me.Label_LoggedInID.Text = "ID: " & e.Username
            PanelLogin.Visible = False
            PanelMain.Visible = True
            Dim filepath As String = Path.Combine(My.Application.Info.DirectoryPath(), DefineValues.Options.Filename)
            If (CheckBox1.Checked) Then
                Ini_SetValue(filepath, DefineValues.Options.SectionAccount, DefineValues.Options.SectionAccount_ID, e.Username)
                Ini_SetValue(filepath, DefineValues.Options.SectionAccount, DefineValues.Options.SectionAccount_SaveUser, "1")
            Else
                Ini_SetValue(filepath, DefineValues.Options.SectionAccount, DefineValues.Options.SectionAccount_SaveUser, "0")
            End If
            Me.LoggedInNotifyMode = True
        Else
            setCaptchaBox(e.ImageVerification)
            Me.Label_LoggedInID.Text = "ID: "
            PanelMain.Visible = False
            PanelLogin.Visible = True
            MessageBox.Show(e.Reason, "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End If
    End Sub

    Private theSize As Size
    Private cacheBoolean_CaptchaBox As Boolean
    Private Sub setCaptchaBox(ByVal show As Boolean)
        If (cacheBoolean_CaptchaBox <> show) Then
            Me.cacheBoolean_CaptchaBox = show
            Me.CaptchaImgBox.Visible = show
            If (show) Then
                Dim bigSize As Integer = Me.theSize.Height + Me.CaptchaImgBox.Height
                Me.Size = New Size(theSize.Width, bigSize)
                bigSize = Me.MyMainMenuPain.Height + Me.CaptchaImgBox.Height
                Me.MyMainMenuPain.Size = New Size(theSize.Width, bigSize)
            Else
                Me.Size = Me.theSize
                Dim bigSize As Integer = Me.MyMainMenuPain.Height - Me.CaptchaImgBox.Height
                Me.MyMainMenuPain.Size = New Size(theSize.Width, bigSize)
            End If
        End If
        If show Then Me.CaptchaImgBox.ForceNewCaptcha()
    End Sub

    Private Sub theHangameClient_Loggedout(ByVal sender As Object, ByVal e As HangameAgentLoginEventArg) Handles theHangameClient.Loggedout
        PanelLogin.Visible = True
        PanelMain.Visible = False
        Me.LoggedInNotifyMode = False
    End Sub

    Private Sub ButtonExit_Click(sender As Object, e As EventArgs) Handles ButtonExit.Click, ButtonExit2.Click
        Me.Close()
    End Sub

    Private Sub ButtonLogout_Click(sender As Object, e As EventArgs) Handles ButtonLogout.Click
        Me.theHangameClient.Logout()
    End Sub

    Private boolean_FormClosing As Boolean
    Private Sub Me_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles Me.FormClosing
        If (Not boolean_FormClosing) Then
            boolean_FormClosing = True
            If (e.CloseReason = CloseReason.UserClosing) AndAlso (theHangameClient.IsLoggedIn) Then
                If (MessageBox.Show("Logout and exit " & My.Application.Info.ProductName & " ?", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes) Then
                    Me.theHangameClient.Close()
                    Me.PubCloneClient.Close()
                Else
                    e.Cancel = True
                End If
            End If
            boolean_FormClosing = False
        Else
            e.Cancel = True
        End If
    End Sub

    Private Sub TextBox_ID_KeyUp(sender As Object, e As KeyEventArgs) Handles TextBox_ID.KeyUp
        If (e.KeyCode = Keys.Enter) Then
            e.SuppressKeyPress = True
            If (String.IsNullOrEmpty(Me.TextBox_PW.Text)) Then
                Me.TextBox_PW.Focus()
            Else
                ButtonLogin.PerformClick()
            End If
        End If
    End Sub

    Private Sub TextBox_PW_KeyUp(sender As Object, e As KeyEventArgs) Handles TextBox_PW.KeyUp
        If (e.KeyCode = Keys.Enter) Then
            e.SuppressKeyPress = True
            If (String.IsNullOrEmpty(Me.TextBox_ID.Text)) Then
                Me.TextBox_ID.Focus()
            Else
                ButtonLogin.PerformClick()
            End If
        End If
    End Sub

    Private Sub GameStart_Click(sender As Object, e As EventArgs) Handles GameStart.Click
        If (CommonMethods.IsSWRunning()) Then
            MessageBox.Show(Me, "Notice", "The game is running", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        ElseIf (CommonMethods.IsReactorRunning()) Then
            MessageBox.Show(Me, "Notice", "The game launcher is running", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Else
            Me.theHangameClient.LaunchSoulWorkerGame()
        End If
    End Sub

    Private Sub Ini_SetValue(ByVal iniFilePath As String, ByVal SectionName As String, ByVal KeyName As String, ByVal Value As String)
        Dim theIniFile As IniFile = New IniFile(iniFilePath)
        theIniFile.SetValue(SectionName, KeyName, Value)
        theIniFile.Save()
        theIniFile.Close()
    End Sub

    Private Function Ini_GetValue(ByVal iniFilePath As String, ByVal SectionName As String, ByVal KeyName As String, ByVal DefaultValue As String) As String
        Dim result As String = DefaultValue
        Dim theIniFile As IniFile = New IniFile(iniFilePath)
        result = theIniFile.GetValue(SectionName, KeyName, DefaultValue)
        theIniFile.Close()
        Return result
    End Function

    Private Sub PanelLogin_VisibleChanged(sender As Object, e As EventArgs) Handles PanelLogin.VisibleChanged
        If (PanelLogin.Visible) Then
            Me.TextBox_PW.Text = ""
            If (Ini_GetValue(Path.Combine(My.Application.Info.DirectoryPath(), DefineValues.Options.Filename), DefineValues.Options.SectionAccount, DefineValues.Options.SectionAccount_SaveUser, "0") = "1") Then
                Me.CheckBox1.Checked = True
                Me.TextBox_ID.Text = Ini_GetValue(Path.Combine(My.Application.Info.DirectoryPath(), DefineValues.Options.Filename), DefineValues.Options.SectionAccount, DefineValues.Options.SectionAccount_ID, "")
                Me.TextBox_PW.Focus()
            Else
                Me.CheckBox1.Checked = False
                Me.TextBox_ID.Text = ""
                Me.TextBox_ID.Focus()
            End If
        End If
    End Sub

    Private Sub ButtonLaunchReactor_Click(sender As Object, e As EventArgs) Handles ButtonLaunchReactor.Click
        Me.theHangameClient.StartReactor()
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxHideID.CheckedChanged
        If (CheckBoxHideID.Checked) Then
            Ini_SetValue(Path.Combine(My.Application.Info.DirectoryPath(), DefineValues.Options.Filename), DefineValues.Options.SectionAccount, DefineValues.Options.SectionAccount_HideID, "1")
            Label_LoggedInID.Visible = False
        Else
            Ini_SetValue(Path.Combine(My.Application.Info.DirectoryPath(), DefineValues.Options.Filename), DefineValues.Options.SectionAccount, DefineValues.Options.SectionAccount_HideID, "0")
            Label_LoggedInID.Visible = True
        End If
    End Sub

    Private Sub PanelMain_VisibleChanged(sender As Object, e As EventArgs) Handles PanelMain.VisibleChanged
        If (Ini_GetValue(Path.Combine(My.Application.Info.DirectoryPath(), DefineValues.Options.Filename), DefineValues.Options.SectionAccount, DefineValues.Options.SectionAccount_HideID, "0") = "1") Then
            CheckBoxHideID.Checked = True
            Label_LoggedInID.Visible = False
        Else
            CheckBoxHideID.Checked = False
            Label_LoggedInID.Visible = True
        End If
    End Sub

    Private Sub theHangameClient_SoulWorkerLaunching(ByVal sender As Object, ByVal e As SoulWorkerLaunchingEventArg) Handles theHangameClient.SoulWorkerLaunching
        If (Ini_GetValue(Path.Combine(My.Application.Info.DirectoryPath(), DefineValues.Options.Filename), DefineValues.Options.SectionAdvanced, DefineValues.Options.SectionAdvanced_LaunchGameCheckPoint, "1") <> "0") Then
            If (MessageBox.Show("Reached checkpoint. You can disconnect your VPN connection right now and continue launching the game." & vbNewLine & "Yes=Continue" & vbNewLine & "No=Abort", "Checkpoint", MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.No) Then
                e.Cancel = True
            End If
        End If
    End Sub

    Private Function StrToBoolean(ByVal theString As String) As Boolean
        If String.IsNullOrWhiteSpace(theString) Then
            Return False
        Else
            If (theString.ToLower.Trim = "true") Then
                Return True
            ElseIf (theString.ToLower.Trim = "false") Then
                Return False
            ElseIf (theString.ToLower.Trim = "0") Then
                Return False
            Else
                Return True
            End If
        End If
    End Function

    Private Sub theHangameClient_TermOfServiceRequired(ByVal sender As Object, ByVal e As TermOfServiceRequiredEventArg) Handles theHangameClient.TermOfServiceRequired
        Using ToServiceForm As New SoulWorkerTermOfServiceForm(Me.theHangameClient)
            ToServiceForm.StartPosition = FormStartPosition.CenterParent
            If (ToServiceForm.ShowDialog(Me) = DialogResult.OK) Then
                e.Agree = True
            End If
        End Using
    End Sub

    Private Sub theHangameClient_TermOfServiceAccepted(ByVal sender As Object, ByVal e As EventArgs) Handles theHangameClient.TermOfServiceAccepted
        MessageBox.Show("You have agreed the Term of Service of SoulWorker." & vbNewLine & "You can launch the game now.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub ButtonOption_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonOption.Click
        Using theOptionForm As New OptionForm()
            theOptionForm.ShowDialog(Me)
        End Using
    End Sub

    Private Sub ButtonManagePatch_Click(sender As Object, e As EventArgs) Handles ButtonManagePatch.Click
        If (Not CommonMethods.IsSoulWorkerInstalled()) Then
            MessageBox.Show(LanguageManager.GetMessage("Patcher_GameIsNotInstalled", "Cannot find SoulWorker game folder. Make sure you install the game properly."), "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        ElseIf (CommonMethods.IsSWUpdaterRunning()) Then
            MessageBox.Show(LanguageManager.GetMessage("Patcher_GameUpdaterIsRunning", "The game updater is currently running. Cannot modify patches while the game updater is running."), "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        ElseIf (CommonMethods.IsSWRunning()) Then
            MessageBox.Show(LanguageManager.GetMessage("Patcher_GameIsRunning", "The game is currently running. Cannot modify patches while the game is running."), "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Else
            If (Classes.Patches.PatchManager.Instance.Host Is Nothing) Then
                Dim form As New PatchManagerForm()
                form.Show()
            Else
                Classes.Patches.PatchManager.Instance.Host.Show()
            End If
        End If
    End Sub

    Private Sub ButtonAbout_Click(sender As Object, e As EventArgs)
        DerpAboutBox.Show()
    End Sub
End Class
