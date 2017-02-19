Imports System.Net
Imports System.ComponentModel
Imports System.IO

Public Class SelfUpdate
    Implements IDisposable
    Private Shared _Instance As SelfUpdate
    Public Shared ReadOnly Property Instance() As SelfUpdate
        Get
            If (_Instance Is Nothing) Then
                _Instance = New SelfUpdate()
            End If
            Return _Instance
        End Get
    End Property

    Private WithEvents myWebClient As CookiesWebClient
    Private syncContext As Threading.SynchronizationContext
    Private LatestVersionStep As String = LanguageManager.GetMessage("UsingLatestVersion", "You're using latest WebClient Lite version")
    Private NewerVersionFoundStep As String = LanguageManager.GetMessage("NewVersionFound", "Found newer version")
    Public Sub New()
        Me.myWebClient = New CookiesWebClient()
        Me.myWebClient.TimeOut = 5000
        Me.myWebClient.Proxy = Nothing
        Me.syncContext = Threading.SynchronizationContext.Current
        Me._IsBusy = False
        Me.VersionUri = Nothing
        Me.UpdateUri = Nothing
        Me._CurrentStep = Nothing
        Me.UpdaterUri = Nothing
        Me._NewVersion = Nothing
        Me._IsNewVersion = False
        Me.Host = Nothing
        Me.UpdaterPath = Path.Combine(My.Application.Info.DirectoryPath(), "updater.exe")
    End Sub

#Region "Properties"
    Public ReadOnly Property UpdaterPath() As String
    Public Property Host() As Form
    Private _IsBusy As Boolean
    Public ReadOnly Property IsBusy() As Boolean
        Get
            Return Me._IsBusy
        End Get
    End Property

    Public Property VersionUri() As Uri
    Public Property UpdateUri() As Uri
    Public Property UpdaterUri() As Uri
    Private _CurrentStep As String
    Public ReadOnly Property CurrentStep() As String
        Get
            Return Me._CurrentStep
        End Get
    End Property
    Private _CurrentProgress As Integer
    Public ReadOnly Property CurrentProgress() As Integer
        Get
            Return Me._CurrentProgress
        End Get
    End Property

    Private _IsNewVersion As Boolean
    Public ReadOnly Property IsNewVersion() As Boolean
        Get
            Return Me._IsNewVersion
        End Get
    End Property

    Private _NewVersion As Version
    Public ReadOnly Property NewVersion() As Version
        Get
            Return Me._NewVersion
        End Get
    End Property
#End Region

    Public Sub CheckForUpdates()
        If (Not Me._IsBusy) AndAlso (Me.VersionUri IsNot Nothing) Then
            Me._IsBusy = True
            Me.RaiseEventStepChanged(LanguageManager.GetMessage("CheckingForUpdates", "Checking for Updates"))
            Me.myWebClient.DownloadDataAsync(VersionUri, "check")
        End If
    End Sub

    Public Sub ForceUpdate()
        If (Not Me.IsBusy) AndAlso (Me.IsNewVersion) Then
            Me.OnPreDownloadUpdate(Me.NewVersion)
        End If
    End Sub

    Private Sub myWebClient_DownloadDataCompleted(ByVal sender As Object, ByVal e As DownloadDataCompletedEventArgs) Handles myWebClient.DownloadDataCompleted
        If (e.Error IsNot Nothing) Then
            Me.RaiseEventHandledException(New HandledExceptionEventArgs(e.Error))
        Else
            If (e.UserState IsNot Nothing) Then
                Dim state As String = DirectCast(e.UserState, String)
                If (state = "check") Then
                    Me.OnCheckVersion(e.Result)
                End If
            End If
        End If
    End Sub

    Protected Overridable Sub OnCheckVersion(ByVal bytes As Byte())
        If (bytes IsNot Nothing) AndAlso (bytes.Length > 1) Then
            Dim myeventarg As NewVersionEventArgs
            If (bytes.Length = 4) Then
                myeventarg = New NewVersionEventArgs(bytes(0), bytes(1), bytes(2), bytes(3))
            Else
                myeventarg = New NewVersionEventArgs(bytes(0), bytes(1))
            End If
            If (myeventarg.Version.CompareTo(My.Application.Info.Version) = 0) Then
                Me.RaiseEventCheckCompleted()
            Else
                Me.RaiseEventNewVersion(myeventarg)
            End If
        End If
    End Sub

    Protected Overridable Sub OnDownloadUpdate(ByVal ver As Version)
        Me._CurrentProgress = -1
        Dim thePath As String = Path.Combine(My.Application.Info.DirectoryPath(), My.Application.Info.AssemblyName() & ".update-" & ver.ToString())
        Me.RaiseEventStepChanged(LanguageManager.GetMessage("ExtractingUpdates", "Extracting Updates"))
        If SharpCompress.Archives.SevenZip.SevenZipArchive.IsSevenZipFile(thePath & ".7z") Then
            Using archive As SharpCompress.Archives.SevenZip.SevenZipArchive = SharpCompress.Archives.SevenZip.SevenZipArchive.Open(thePath & ".7z")
                Using reader As SharpCompress.Readers.IReader = archive.ExtractAllEntries()
                    If reader.MoveToNextEntry() Then
                        Using fs As FileStream = File.Create(thePath)
                            reader.WriteEntryTo(fs)
                        End Using
                    End If
                End Using
            End Using
            Try
                File.Delete(thePath & ".7z")
            Catch
            End Try
            Me.RaiseEventStepChanged(LanguageManager.GetMessage("CheckingForUpdates", "Restarting application to perform update."))

            Using theProcess As New Process()
                theProcess.StartInfo.FileName = Me.UpdaterPath
                theProcess.StartInfo.Arguments = CommonMethods.StringTableToArgs({"-leayal",
                                                                                 "-patch:" & thePath,
                                                                                 "-destination:" & Path.Combine(My.Application.Info.DirectoryPath(), My.Application.Info.AssemblyName() & ".exe")})
                If (Not CommonMethods.IsWindowsXP()) Then theProcess.StartInfo.Verb = "runas"
                theProcess.Start()
            End Using
            Environment.Exit(0)
        Else
            Me.RaiseEventHandledException(New HandledExceptionEventArgs(New FileNotFoundException("Update content not found", thePath)))
        End If
    End Sub

    Protected Overridable Sub OnPreDownloadUpdate(ByVal ver As Version)
        If (Not Me.myWebClient.IsBusy) AndAlso (Me.UpdateUri IsNot Nothing) Then
            Me.RaiseEventBeginDownloadPatch()
            If (File.Exists(Me.UpdaterPath)) Then
                Me.RaiseEventStepChanged("Downloading new version")
                Me.myWebClient.DownloadFileAsync(Me.UpdateUri, Path.Combine(My.Application.Info.DirectoryPath(), My.Application.Info.AssemblyName() & ".update-" & ver.ToString() & ".7z"), ver)
            Else
                If (Me.UpdaterUri IsNot Nothing) Then
                    Me.RaiseEventStepChanged("Downloading updater")
                    Me.myWebClient.DownloadFileAsync(Me.UpdaterUri, Me.UpdaterPath & ".7z", New DownloadFileMeta("downloadupdater", ver))
                End If
            End If
        End If
    End Sub

    Private Sub myWebClient_DownloadFileCompleted(ByVal sender As Object, ByVal e As AsyncCompletedEventArgs) Handles myWebClient.DownloadFileCompleted
        If (e.Error IsNot Nothing) Then
            Me.RaiseEventHandledException(New HandledExceptionEventArgs(e.Error))
        Else
            Dim typeName As String = e.UserState.GetType().Name
            If (typeName = GetType(Version).Name) Then
                Me.OnDownloadUpdate(DirectCast(e.UserState, Version))
            ElseIf (typeName = GetType(DownloadFileMeta).Name) Then
                Dim state As DownloadFileMeta = DirectCast(e.UserState, DownloadFileMeta)
                If (state.State = "downloadupdater") Then
                    Me.RaiseEventStepChanged("Downloading new version")
                    If SharpCompress.Archives.SevenZip.SevenZipArchive.IsSevenZipFile(Me.UpdaterPath & ".7z") Then
                        Using archive As SharpCompress.Archives.SevenZip.SevenZipArchive = SharpCompress.Archives.SevenZip.SevenZipArchive.Open(Me.UpdaterPath & ".7z")
                            Using reader As SharpCompress.Readers.IReader = archive.ExtractAllEntries()
                                If reader.MoveToNextEntry() Then
                                    Using fs As FileStream = File.Create(Me.UpdaterPath)
                                        reader.WriteEntryTo(fs)
                                    End Using
                                End If
                            End Using
                        End Using
                        Try
                            File.Delete(Me.UpdaterPath & ".7z")
                        Catch
                        End Try
                        Me.myWebClient.DownloadFileAsync(Me.UpdateUri, Path.Combine(My.Application.Info.DirectoryPath(), My.Application.Info.AssemblyName() & ".update-" & state.Ver.ToString() & ".7z"), state.Ver)
                    Else
                        Me.RaiseEventHandledException(New HandledExceptionEventArgs(New FileNotFoundException("Updater not found", Me.UpdaterPath)))
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub myWebClient_DownloadProgressChanged(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs) Handles myWebClient.DownloadProgressChanged
        Me.RaiseEventProgressChanged(New ProgressChangedEventArgs(e.ProgressPercentage, Nothing))
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        myWebClient.Dispose()
    End Sub

#Region "Events"
    Public Event CheckCompleted As EventHandler
    Private Sub RaiseEventCheckCompleted()
        Me._IsBusy = False
        Me.syncContext.Post(AddressOf OnCheckCompleted, EventArgs.Empty)
        Me.RaiseEventStepChanged(LatestVersionStep)
    End Sub
    Private Sub OnCheckCompleted(ByVal e As Object)
        RaiseEvent CheckCompleted(Me, DirectCast(e, EventArgs))
    End Sub

    Public Event BeginDownloadPatch As EventHandler
    Private Sub RaiseEventBeginDownloadPatch()
        Me.syncContext.Post(AddressOf OnBeginDownloadPatch, EventArgs.Empty)
    End Sub
    Private Sub OnBeginDownloadPatch(ByVal e As Object)
        RaiseEvent BeginDownloadPatch(Me, DirectCast(e, EventArgs))
    End Sub

    Public Delegate Sub HandledExceptionEventHandler(ByVal sender As Object, ByVal e As HandledExceptionEventArgs)
    Public Event HandledException As HandledExceptionEventHandler
    Private Sub RaiseEventHandledException(ByVal e As HandledExceptionEventArgs)
        Me._IsBusy = Me.myWebClient.IsBusy
        Me.syncContext.Post(AddressOf OnHandledException, e)
    End Sub
    Private Sub OnHandledException(ByVal e As Object)
        Dim myevent As HandledExceptionEventArgs = DirectCast(e, HandledExceptionEventArgs)
        RaiseEvent HandledException(Me, myevent)
        Classes.Log.LogManager.GeneralLog.QueueLog(myevent.HandledError)
        MessageBox.Show(myevent.HandledError.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub
    Public Delegate Sub StepChangedEventHandler(ByVal sender As Object, ByVal e As StepChangedEventArgs)
    Public Event StepChanged As StepChangedEventHandler
    Private Sub RaiseEventStepChanged(ByVal currentStep As String)
        Me._CurrentStep = currentStep
        Me.syncContext.Post(AddressOf OnStepChanged, New StepChangedEventArgs(currentStep))
    End Sub
    Private Sub OnStepChanged(ByVal e As Object)
        RaiseEvent StepChanged(Me, DirectCast(e, StepChangedEventArgs))
    End Sub
    Public Event ProgressChanged As ProgressChangedEventHandler
    Private Sub RaiseEventProgressChanged(ByVal e As ProgressChangedEventArgs)
        Me._CurrentProgress = e.ProgressPercentage
        Me.syncContext.Post(AddressOf OnProgressChanged, e)
    End Sub
    Private Sub OnProgressChanged(ByVal e As Object)
        RaiseEvent ProgressChanged(Me, DirectCast(e, ProgressChangedEventArgs))
    End Sub
    Public Delegate Sub NewVersionEventHandler(ByVal sender As Object, ByVal e As NewVersionEventArgs)
    Public Event FoundNewVersion As NewVersionEventHandler
    Private Sub RaiseEventNewVersion(ByVal e As NewVersionEventArgs)
        Me._IsNewVersion = True
        Me._NewVersion = e.Version
        Me.syncContext.Post(AddressOf OnNewVersion, e)
    End Sub
    Private Sub OnNewVersion(ByVal e As Object)
        Dim myevent As NewVersionEventArgs = DirectCast(e, NewVersionEventArgs)
        RaiseEvent FoundNewVersion(Me, myevent)
        If (myevent.AllowUpdate) Then
            Me.OnPreDownloadUpdate(myevent.Version)
        Else
            'Prompt
            If (MessageBox.Show(String.Format(LanguageManager.GetMessage("PromptUpdateApplication", "Do you want to update the application to '{0}'?"), myevent.Version.ToString()), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes) Then
                Me.OnPreDownloadUpdate(myevent.Version)
            Else
                Me._IsBusy = False
                Me.RaiseEventStepChanged(NewerVersionFoundStep & ": " & myevent.Version.ToString())
            End If
        End If
    End Sub
#End Region

#Region "Internal Classes"
    Protected Class DownloadFileMeta
        Public ReadOnly Property State() As String
        Public ReadOnly Property Ver() As Version
        Public Sub New(ByVal sstate As String, ByVal over As Version)
            Me.State = sstate
            Me.Ver = over
        End Sub
    End Class
    Class HandledExceptionEventArgs
        Inherits System.EventArgs
        Public ReadOnly Property HandledError() As Exception
        Public Sub New(ByVal ex As Exception)
            MyBase.New()
            Me.HandledError = ex
        End Sub
    End Class
    Class StepChangedEventArgs
        Inherits System.EventArgs
        Public ReadOnly Property CurrentStep() As String
        Public Sub New(ByVal current As String)
            MyBase.New()
            Me.CurrentStep = current
        End Sub
    End Class
    Class NewVersionEventArgs
        Inherits System.EventArgs
        Public ReadOnly Property Version() As Version
        Public Property AllowUpdate() As Boolean
        Public Sub New(ByVal major As Byte, ByVal minor As Byte)
            Me.New(New Version(major, minor))
        End Sub
        Public Sub New(ByVal major As Byte, ByVal minor As Byte, ByVal build As Byte, ByVal revision As Byte)
            Me.New(New Version(major, minor, build, revision))
        End Sub
        Public Sub New(ByVal verstring As String)
            Me.New(New Version(verstring))
        End Sub
        Public Sub New(ByVal ver As Version)
            MyBase.New()
            Me.Version = ver
            Me.AllowUpdate = False
        End Sub
    End Class
#End Region
End Class
