Imports SoulWorkerLogin.Classes.Patches

Public Class PatchManagerForm
    Private myManager As Classes.Patches.PatchManager
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        Me.myManager = Classes.Patches.PatchManager.Instance()
        Me.myManager.Host = Me
        Me.ProgressBar1.Maximum = Me.myManager.CurrentMax
        Me.RegisterEvents()
        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Private Sub RegisterEvents()
        AddHandler Me.myManager.HandledException, AddressOf Me.myManager_HandledException
        AddHandler Me.myManager.LanguageChanged, AddressOf Me.myManager_LanguageChanged
        AddHandler Me.myManager.PatchBuildCompleted, AddressOf Me.myManager_PatchBuildCompleted
        AddHandler Me.myManager.InstallPatchCompleted, AddressOf Me.myManager_InstallPatchCompleted
        AddHandler Me.myManager.UninstallPatchCompleted, AddressOf Me.myManager_UninstallPatchCompleted
        AddHandler Me.myManager.StepChanged, AddressOf Me.myManager_StepChanged
        AddHandler Me.myManager.MaxProgressChanged, AddressOf Me.myManager_MaxProgressChanged
        AddHandler Me.myManager.ProgressChanged, AddressOf Me.myManager_ProgressChanged
    End Sub

    Private Sub UnregisterEvents()
        RemoveHandler Me.myManager.HandledException, AddressOf Me.myManager_HandledException
        RemoveHandler Me.myManager.LanguageChanged, AddressOf Me.myManager_LanguageChanged
        RemoveHandler Me.myManager.PatchBuildCompleted, AddressOf Me.myManager_PatchBuildCompleted
        RemoveHandler Me.myManager.InstallPatchCompleted, AddressOf Me.myManager_InstallPatchCompleted
        RemoveHandler Me.myManager.UninstallPatchCompleted, AddressOf Me.myManager_UninstallPatchCompleted
        RemoveHandler Me.myManager.StepChanged, AddressOf Me.myManager_StepChanged
        RemoveHandler Me.myManager.MaxProgressChanged, AddressOf Me.myManager_MaxProgressChanged
        RemoveHandler Me.myManager.ProgressChanged, AddressOf Me.myManager_ProgressChanged
    End Sub

    Private Sub PatchManagerForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.RefreshStatus()
        Me.LoadStatus()
    End Sub

    Private Sub myManager_LanguageChanged(ByVal sender As Object, ByVal e As EventArgs)
        Me.RefreshStatus()
    End Sub

    Private Sub LoadStatus()
        If (Me.myManager.IsBusy()) Then
            Me.LabelStep.Text = Me.myManager.CurrentStep
            Panel1.Visible = True
            Me.ButtonInstall.Enabled = False
            Me.ButtonRebuild.Enabled = False
            Me.ButtonUninstall.Enabled = False
        Else
            If (Me.myManager.IsPatchExist()) Then
                Me.ButtonInstall.Enabled = True
            Else
                Me.ButtonInstall.Enabled = False
            End If
            Me.ButtonRebuild.Enabled = True
            Me.ButtonUninstall.Enabled = True
            Panel1.Visible = False
        End If
    End Sub

    Private Sub RefreshStatus()
        If (Me.myManager.IsPatchExist()) Then
            Me.ButtonRebuild.Text = LanguageManager.GetControlText("PatchManager_rebuild", "Force Rebuild Patch")
            If (Me.myManager.IsPatchInstalled()) Then
                Me.LabelStatus.Text = "Status: " & LanguageManager.GetText("PatchManager", "statusInstalled", "Installed")
                Me.LabelStatus.ForeColor = Color.DarkGreen
            Else
                Me.LabelStatus.Text = "Status: " & LanguageManager.GetText("PatchManager", "statusNotInstalled", "Not Installed")
                Me.LabelStatus.ForeColor = Color.Goldenrod
            End If
        Else
            Me.ButtonRebuild.Text = LanguageManager.GetControlText("PatchManager_build", "Build Patch")
            Me.LabelStatus.Text = "Status: " & LanguageManager.GetText("PatchManager", "statusNotBuilt", "Not Build Yet")
            Me.LabelStatus.ForeColor = Color.DarkRed
        End If
        Me.LabelVersionDate.Text = "Version Date: " & Me.myManager.PatchVersionDate()
    End Sub

    Private Sub myManager_PatchBuildCompleted(ByVal sender As Object, ByVal e As EventArgs)
        Me.RefreshStatus()
        Me.ButtonInstall.Enabled = True
        Me.ButtonRebuild.Enabled = True
        Panel1.Visible = False
    End Sub

    Private Sub myManager_ProgressChanged(ByVal sender As Object, ByVal e As ComponentModel.ProgressChangedEventArgs)
        Me.ProgressBar1.Value = e.ProgressPercentage
    End Sub

    Private Sub myManager_HandledException(ByVal sender As Object, ByVal e As Classes.Patches.PatchManager.HandledExceptionEventArgs)
        Me.LoadStatus()
        MessageBox.Show(Me, e.HandledError.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

    Private Sub myManager_MaxProgressChanged(sender As Object, e As PatchManager.IntegerEventArgs)
        Me.ProgressBar1.Maximum = e.Value
    End Sub

    Private Sub myManager_StepChanged(sender As Object, e As PatchManager.StringEventArgs)
        Me.Panel1.Visible = True
        Me.LabelStep.Text = e.Text
    End Sub

    Private Sub PatchManager_Closed(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Closed
        Me.UnregisterEvents()
        Me.myManager.Host = Nothing
        Me.Dispose()
    End Sub

    Private Sub ButtonRebuild_Click(sender As Object, e As EventArgs) Handles ButtonRebuild.Click
        If (Not Me.myManager.IsBusy) Then
            If (MessageBox.Show(Me, LanguageManager.GetMessage("Patch_WarnPromptRebuild", "While the translation patch is in building. Please don't modify any data files in your game folder. That mean: No updating, no moding, no deleting, no changes." & vbNewLine & "Do you want to continue?"), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = DialogResult.Yes) Then
                Me.ButtonRebuild.Enabled = False
                Me.myManager.BuildData()
            End If
        End If
    End Sub

    Private Sub ButtonInstall_Click(sender As Object, e As EventArgs) Handles ButtonInstall.Click
        If (Not Me.myManager.IsBusy) Then
            Me.ButtonInstall.Enabled = False
            Me.ButtonUninstall.Enabled = False
            Me.myManager.Install()
        End If
    End Sub

    Private Sub myManager_InstallPatchCompleted(ByVal sender As Object, ByVal e As EventArgs)
        Me.LabelStatus.Text = "Status: " & LanguageManager.GetText("PatchManager", "statusInstalled", "Installed")
        Me.LabelStatus.ForeColor = Color.DarkGreen
        Me.ButtonInstall.Enabled = True
        Me.ButtonUninstall.Enabled = True
    End Sub

    Private Sub myManager_UninstallPatchCompleted(ByVal sender As Object, ByVal e As EventArgs)
        Me.LabelStatus.Text = "Status: " & LanguageManager.GetText("PatchManager", "statusNotInstalled", "Not Installed")
        Me.LabelStatus.ForeColor = Color.Goldenrod
        Me.ButtonUninstall.Enabled = True
        Me.ButtonInstall.Enabled = True
    End Sub

    Private Sub ButtonUninstall_Click(sender As Object, e As EventArgs) Handles ButtonUninstall.Click
        If (Not Me.myManager.IsBusy) Then
            Me.ButtonUninstall.Enabled = False
            Me.ButtonInstall.Enabled = False
            Me.myManager.Uninstall()
        End If
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Try
            Process.Start(DefineValues.Urls.SoulworkerHQTranslationThread)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class