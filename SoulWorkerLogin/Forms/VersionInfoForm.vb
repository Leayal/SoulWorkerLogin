Public Class VersionInfoForm
    Private WithEvents myUpdater As SelfUpdate
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        Me.Icon = My.Resources.haru_sd_WM1UAm_256px
        Me.myUpdater = SelfUpdate.Instance()
        Me.myUpdater.Host = Me
        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Private Sub VersionInfoForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.LoadProgress()
    End Sub

    Private Sub myUpdater_StepChanged(ByVal sender As Object, ByVal e As SelfUpdate.StepChangedEventArgs) Handles myUpdater.StepChanged
        Me.ValueLabelNewVersion.Text = e.CurrentStep
    End Sub

    Private Sub myUpdater_ProgressChanged(ByVal sender As Object, ByVal e As ComponentModel.ProgressChangedEventArgs) Handles myUpdater.ProgressChanged
        If (e.ProgressPercentage > -1) Then
            Me.ValueLabelNewVersion.Text = Me.myUpdater.CurrentStep & ": " & e.ProgressPercentage & "%"
        End If
    End Sub

    Private Sub myUpdater_FoundNewVersion(ByVal sender As Object, ByVal e As SelfUpdate.NewVersionEventArgs) Handles myUpdater.FoundNewVersion
        Me.ButtonUpdate.Text = LanguageManager.GetMessage("ButtonUpdate_UpdateReady", "Update")
        Me.ButtonUpdate.Enabled = True
    End Sub

    Private Sub myUpdater_CheckCompleted(ByVal sender As Object, ByVal e As EventArgs) Handles myUpdater.CheckCompleted
        Me.ButtonUpdate.Text = LanguageManager.GetMessage("ButtonUpdate_CheckReady", "Check for Updates")
        Me.ButtonUpdate.Enabled = True
    End Sub

    Private Sub myUpdater_BeginDownloadPatch(ByVal sender As Object, ByVal e As EventArgs) Handles myUpdater.BeginDownloadPatch
        Me.ButtonUpdate.Enabled = False
    End Sub

    Private Sub myUpdater_HandledException(ByVal sender As Object, ByVal e As SelfUpdate.HandledExceptionEventArgs) Handles myUpdater.HandledException
        Me.ButtonUpdate.Enabled = Not Me.myUpdater.IsBusy
    End Sub

    Private Sub ButtonUpdate_Click(sender As Object, e As EventArgs) Handles ButtonUpdate.Click
        Me.ButtonUpdate.Enabled = False
        If (Not Me.myUpdater.IsNewVersion) Then
            Me.myUpdater.CheckForUpdates()
        Else
            Me.myUpdater.ForceUpdate()
        End If
    End Sub

    Private Sub LoadProgress()
        'Me.myUpdater.CurrentStep
        Me.ValueLabelVersion.Text = My.Application.Info.Version.ToString()
        If (Me.myUpdater.IsBusy) Then
            Me.ButtonUpdate.Enabled = False
            Me.ValueLabelNewVersion.Text = Me.myUpdater.CurrentStep()
        Else
            Me.ButtonUpdate.Enabled = True
            If (Me.myUpdater.IsNewVersion) Then
                Me.ButtonUpdate.Text = LanguageManager.GetMessage("ButtonUpdate_UpdateReady", "Update")
                Me.ValueLabelNewVersion.Text = LanguageManager.GetMessage("NewVersionFound", "Found newer version") & ": " & Me.myUpdater.NewVersion.ToString()
            Else
                Me.ButtonUpdate.Text = LanguageManager.GetMessage("ButtonUpdate_CheckReady", "Check for Updates")
                Me.ValueLabelNewVersion.Text = Me.myUpdater.CurrentStep()
            End If
        End If
    End Sub

    Private Sub MyBase_Closed(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Closed
        Me.myUpdater.Host = Nothing
        Me.Dispose()
    End Sub
End Class