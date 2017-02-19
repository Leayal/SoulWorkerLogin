Imports System.IO
Imports System.Net
Imports SoulWorkerLogin.Hangame.Reactor
Imports System.ComponentModel

Namespace Hangame
    Public Class PubClone

#Region "Events"
        Public Delegate Sub PubCloneProgressChangedEventHandler(ByVal sender As Object, ByVal e As PubCloneProgressChangedEventArg)
        Public Event ProgressChanged As PubCloneProgressChangedEventHandler
        Private Sub OnProgressChanged(e As PubCloneProgressChangedEventArg)
            RaiseEvent ProgressChanged(Me, e)
        End Sub

        Public Delegate Sub PubCloneDownloadCompletedEventHandler(ByVal sender As Object, ByVal e As EventArgs)
        Public Event DownloadCompleted As PubCloneDownloadCompletedEventHandler
        Private Sub OnDownloadCompleted(e As EventArgs)
            RaiseEvent DownloadCompleted(Me, e)
        End Sub
#End Region

        Private theReactorFolder As String
        Private theReactorFullPath As String
        Private MyWebClient As CustomWebClient
        Private WithEvents bWorker As BackgroundWorker
        Public Sub New()
            'Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Downloaded Program Files")
            'Path.Combine(My.Application.Info.DirectoryPath(), "Reactor")
            Me.theReactorFolder = DefineValues.Filenames.ReactorFolder
            Me.theReactorFullPath = DefineValues.Filenames.ReactorExecutablePath
            Me.MyWebClient = New CustomWebClient()
            Me.MyWebClient.CachePolicy = New Cache.RequestCachePolicy(Cache.RequestCacheLevel.NoCacheNoStore)
            Me.MyWebClient.UserAgent = "Purple"
            Me.bWorker = New BackgroundWorker()
            Me.bWorker.WorkerReportsProgress = True
            Me.bWorker.WorkerSupportsCancellation = False
        End Sub

        Public Function IsReactorDownloaded() As Boolean
            If (My.Computer.FileSystem.DirectoryExists(Me.theReactorFolder)) Then
                If (My.Computer.FileSystem.FileExists(Me.theReactorFullPath)) Then
                    Return True
                End If
            End If
            Return False
        End Function

        Public Function CheckForReactorUpdates() As ReactorFile()
            Dim FixList As ReactorFile() = GetFixList()
            Dim UpdateList As New List(Of ReactorFile)()
            Dim theReactorFile As ReactorFile = Nothing
            Dim tmpFilename As String = Nothing
            Dim currentVer As Version = Nothing
            Dim requiredVer As Version = Nothing
            For i As Integer = 0 To FixList.Length - 1
                theReactorFile = FixList(i)
                tmpFilename = Path.Combine(Me.theReactorFolder, theReactorFile.Name)
                If (File.Exists(tmpFilename)) Then
                    If (tmpFilename.ToLower.EndsWith(".xml")) Then
                        currentVer = getXMLVersion(tmpFilename)
                        requiredVer = New Version(theReactorFile.Version().Trim())
                        If (currentVer <> requiredVer) Then
                            UpdateList.Add(theReactorFile)
                        End If
                    Else
                        currentVer = getFileVersion(tmpFilename)
                        requiredVer = New Version(theReactorFile.Version().Trim())
                        If (currentVer <> requiredVer) Then
                            UpdateList.Add(theReactorFile)
                        End If
                    End If
                Else
                    UpdateList.Add(theReactorFile)
                End If
            Next
            Return UpdateList.ToArray()
            'Me.MyWebClient.DownloadString("http://down.hangame.co.jp/jp/purple/plii/version.xml")
        End Function

        Private Function GetFixList() As ReactorFile()
            Dim newList As New List(Of ReactorFile)()
            Dim DefaultUrl As String = "http://down.hangame.co.jp/jp/purple/plii/common/reactor_common/"
            Dim theString As String = Me.MyWebClient.DownloadString(DefineValues.Urls.ReactorUpdateXML)
            If (String.IsNullOrWhiteSpace(theString)) Then
                Throw New WebException("Failed to check for Reactor Updates. Please check your internet connection.")
            Else
                Using xmlStream As StringReader = New StringReader(theString)
                    Using theXML As Xml.XmlTextReader = New Xml.XmlTextReader(xmlStream)
                        Dim tmpName As String = Nothing
                        Dim tmpVersion As String = Nothing
                        Dim tmpSelf As String = Nothing
                        Dim tmpUrl As String = Nothing
                        While (theXML.Read)
                            If (theXML.Name.ToLower() = "file") Then
                                tmpName = Nothing
                                tmpVersion = Nothing
                                tmpSelf = Nothing
                                tmpUrl = Nothing
                                If (theXML.HasAttributes()) Then
                                    tmpName = theXML.GetAttribute("name")
                                    tmpVersion = theXML.GetAttribute("version")
                                    tmpSelf = theXML.GetAttribute("self")
                                    tmpUrl = theXML.GetAttribute("url")
                                    If (String.IsNullOrWhiteSpace(tmpUrl)) Then
                                        If (Not String.IsNullOrWhiteSpace(DefaultUrl)) Then
                                            tmpUrl = DefaultUrl
                                        End If
                                    End If
                                    If (Not String.IsNullOrWhiteSpace(tmpSelf)) Then
                                        If (tmpSelf.Trim.ToLower() = "true") Then
                                            newList.Add(New ReactorFile(tmpName, tmpVersion, True, tmpUrl))
                                        Else
                                            newList.Add(New ReactorFile(tmpName, tmpVersion, False, tmpUrl))
                                        End If
                                    Else
                                        newList.Add(New ReactorFile(tmpName, tmpVersion, False, tmpUrl))
                                    End If
                                End If
                            ElseIf (theXML.Name.ToLower() = "update") Then
                                If (theXML.HasAttributes()) Then
                                    Dim theTempDefaultUrl As String = theXML.GetAttribute("url")
                                    If (Not String.IsNullOrWhiteSpace(theTempDefaultUrl)) Then
                                        DefaultUrl = theTempDefaultUrl
                                    End If
                                End If
                            End If
                        End While
                    End Using
                End Using
            End If
            theString = Nothing
            Return newList.ToArray()
        End Function

        Private Function getFileVersion(ByVal filePath As String) As Version
            Dim result As Version = New Version("0.0.0.0")
            With (FileVersionInfo.GetVersionInfo(filePath))
                result = New Version(.FileMajorPart, .FileMinorPart, .FileBuildPart, .FilePrivatePart)
            End With
            Return result
        End Function

        Private Function getXMLVersion(ByVal filePath As String) As Version
            Dim result As Version = New Version("0.0.0.0")
            Using xmlStream As StreamReader = New StreamReader(filePath)
                Using theXML As Xml.XmlTextReader = New Xml.XmlTextReader(xmlStream)
                    While (theXML.Read)
                        If (theXML.Name.ToLower() = "version") Then
                            result = New Version(theXML.ReadString())
                            Exit While
                        End If
                    End While
                End Using
            End Using
            Return result
        End Function

        Private Sub Unzip(ByVal ZipFilePath As String, ByVal DeleteAfterExtraction As Boolean)
            Using TheZip As Classes.ZipStorer = Classes.ZipStorer.Open(ZipFilePath, FileAccess.Read)
                Dim thelist = TheZip.ReadCentralDir()
                Dim baseDir As String = My.Computer.FileSystem.GetParentPath(ZipFilePath)
                Dim fullDir As String = Nothing
                For i As Integer = 0 To thelist.Count - 1
                    fullDir = Path.Combine(baseDir, thelist(i).FilenameInZip)
                    My.Computer.FileSystem.CreateDirectory(My.Computer.FileSystem.GetParentPath(fullDir))
                    Using myFileStream As New FileStream(fullDir, FileMode.Create, FileAccess.ReadWrite, FileShare.Read)
                        TheZip.ExtractFile(thelist(i), myFileStream)
                        myFileStream.Flush()
                    End Using
                Next
                thelist.Clear()
            End Using
            If (DeleteAfterExtraction) Then File.Delete(ZipFilePath)
        End Sub

        Public Sub RedownloadReactor()
            If (Not Me.bWorker.IsBusy()) Then
                Me.bWorker.RunWorkerAsync(New Object() {True})
            End If
            'http://down.hangame.co.jp/jp/purple/plii/j_sw/miracle/reactor.exe
        End Sub

        Private Sub bWorker_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles bWorker.ProgressChanged
            OnProgressChanged(DirectCast(e.UserState(), PubCloneProgressChangedEventArg))
        End Sub

        Private Sub bWorker_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles bWorker.DoWork
            If (e.Argument IsNot Nothing) Then
                Dim Param As Object() = DirectCast(e.Argument, Object())

                If (DirectCast(Param(0), Boolean) = True) Then
                    My.Computer.FileSystem.CreateDirectory(Me.theReactorFolder)
                    Dim theDownloadList As ReactorFile() = GetFixList()
                    If (theDownloadList IsNot Nothing) AndAlso (theDownloadList.Length > 0) Then
                        Dim tmpCall As ReactorFile = Nothing
                        Dim tmpPath As String = Nothing
                        For i As Integer = 0 To theDownloadList.Length - 1
                            tmpCall = theDownloadList(i)
                            bWorker.ReportProgress(0, New PubCloneProgressChangedEventArg(tmpCall.Name, 1 + i, theDownloadList.Length))
                            If (tmpCall.Name.ToLower = "reactor.exe") Then
                                tmpPath = Path.Combine(Me.theReactorFolder, tmpCall.Name)
                                Me.MyWebClient.DownloadFile(tmpCall.Url & tmpCall.Name, tmpPath)
                            Else
                                tmpPath = Path.Combine(Me.theReactorFolder, tmpCall.Name & ".zip")
                                Me.MyWebClient.DownloadFile(tmpCall.Url & tmpCall.Name & ".zip", tmpPath)
                                Unzip(tmpPath, True)
                            End If
                        Next
                    End If
                Else
                    My.Computer.FileSystem.CreateDirectory(Me.theReactorFolder)
                    Dim UpdateList As ReactorFile() = CheckForReactorUpdates()
                    If (UpdateList IsNot Nothing) AndAlso (UpdateList.Length > 0) Then
                        Dim tmpCall As ReactorFile = Nothing
                        Dim tmpPath As String = Nothing
                        For i As Integer = 0 To UpdateList.Length - 1
                            tmpCall = UpdateList(i)
                            bWorker.ReportProgress(0, New PubCloneProgressChangedEventArg(tmpCall.Name, 1 + i, UpdateList.Length))
                            If (tmpCall.Name.ToLower = "reactor.exe") Then
                                tmpPath = Path.Combine(Me.theReactorFolder, tmpCall.Name)
                                Me.MyWebClient.DownloadFile(tmpCall.Url & tmpCall.Name, tmpPath)
                            Else
                                tmpPath = Path.Combine(Me.theReactorFolder, tmpCall.Name & ".zip")
                                Me.MyWebClient.DownloadFile(tmpCall.Url & tmpCall.Name & ".zip", tmpPath)
                                Unzip(tmpPath, True)
                            End If
                        Next
                    End If
                End If
                e.Result = "true"
                If (Param.Length > 1) Then
                    Dim strParam As String = DirectCast(Param(1), String)
                    If (Not String.IsNullOrWhiteSpace(strParam)) Then
                        Using theProcess As New System.Diagnostics.Process()
                            theProcess.StartInfo.Arguments = strParam & " -updateurl:" & DefineValues.Urls.ReactorUpdateXML
                            theProcess.StartInfo.FileName = Me.theReactorFullPath
                            If (Not CommonMethods.IsWindowsXP()) Then theProcess.StartInfo.Verb = "runas"
                            theProcess.Start()
                        End Using
                        e.Result = "false"
                    End If
                End If
            End If
        End Sub

        Private Sub bWorker_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles bWorker.RunWorkerCompleted
            If (e.Cancelled) Then
                Environment.Exit(1)
            Else
                If (e.Error IsNot Nothing) Then
                    CommonMethods.OutputToLog(e.Error)
                    MessageBox.Show(e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Environment.Exit(1)
                Else
                    If (e.Result IsNot Nothing) Then
                        If (DirectCast(e.Result, String) = "false") Then

                        Else
                            OnDownloadCompleted(New EventArgs())
                        End If
                    Else
                        OnDownloadCompleted(New EventArgs())
                    End If
                End If
            End If
        End Sub

        Public Sub DownloadReactor()
            If (Not Me.bWorker.IsBusy()) Then
                Me.bWorker.RunWorkerAsync(New Object() {False})
            End If
        End Sub

        Public Sub StartReactor(ByVal strParam As String)
            If (Not Me.bWorker.IsBusy()) Then
                Me.bWorker.RunWorkerAsync(New Object() {False, strParam})
            End If
        End Sub

        Public Sub Close()
            Me.MyWebClient.Dispose()
        End Sub
    End Class
End Namespace
