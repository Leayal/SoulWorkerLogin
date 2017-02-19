Imports System.IO

Public Class OptionForm
    Private loadedup As Boolean
    Private theIniFile As IniFile
    Public Sub New()
        Me.loadedup = False
        Me.theIniFile = New IniFile(Path.Combine(My.Application.Info.DirectoryPath(), DefineValues.Options.Filename))
        ' This call is required by the designer.
        InitializeComponent()
        If (Me.theIniFile.GetValue(DefineValues.Options.SectionAdvanced, DefineValues.Options.SectionAdvanced_LaunchGameCheckPoint, "1") = "0") Then
            CheckBoxAllowCheckpointLaunchingSW.Checked = False
        Else
            CheckBoxAllowCheckpointLaunchingSW.Checked = True
        End If
        If (Me.theIniFile.GetValue(DefineValues.Updater.SectionUpdates, DefineValues.Updater.SectionUpdates_CheckAtStartup, "1") = "0") Then
            CheckBoxCheckUpdateStartup.Checked = False
        Else
            CheckBoxCheckUpdateStartup.Checked = True
        End If
        ' Add any initialization after the InitializeComponent() call.
        loadedup = True
    End Sub

    Public ReadOnly Property Ini() As IniFile
        Get
            Return Me.theIniFile
        End Get
    End Property

    Private Sub OptionForm_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
        Me.theIniFile.Save()
    End Sub

    Private Sub CheckBoxAllowCheckpointLaunchingSW_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxAllowCheckpointLaunchingSW.CheckedChanged
        If (loadedup) Then
            If (CheckBoxAllowCheckpointLaunchingSW.Checked) Then
                Me.theIniFile.SetValue(DefineValues.Options.SectionAdvanced, DefineValues.Options.SectionAdvanced_LaunchGameCheckPoint, "1")
            Else
                Me.theIniFile.SetValue(DefineValues.Options.SectionAdvanced, DefineValues.Options.SectionAdvanced_LaunchGameCheckPoint, "0")
            End If
        End If
    End Sub

    Private Sub ButtonOpenUpdaterForm_Click(sender As Object, e As EventArgs) Handles ButtonOpenUpdaterForm.Click
        If (SelfUpdate.Instance.Host IsNot Nothing) Then
            SelfUpdate.Instance.Host.Show()
        Else
            Dim updaterform As New VersionInfoForm()
            updaterform.Show()
        End If
    End Sub

    Private Sub CheckBoxCheckUpdateStartup_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxCheckUpdateStartup.CheckedChanged
        If (loadedup) Then
            If (CheckBoxCheckUpdateStartup.Checked) Then
                Me.theIniFile.SetValue(DefineValues.Updater.SectionUpdates, DefineValues.Updater.SectionUpdates_CheckAtStartup, "1")
            Else
                Me.theIniFile.SetValue(DefineValues.Updater.SectionUpdates, DefineValues.Updater.SectionUpdates_CheckAtStartup, "0")
            End If
        End If
    End Sub
End Class