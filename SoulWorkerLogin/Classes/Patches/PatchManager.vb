Imports System.ComponentModel
Imports Leayal.Net

Namespace Classes.Patches
    ''' <summary>
    ''' 0: Step String
    ''' 1: Progress Value
    ''' 2: Progress Total
    ''' 
    ''' </summary>
    Public Class PatchManager
        Const myNeededData As String = "datas\data12.v"
        Const patchFolder As String = "=patches"
        Private Shared _instance As PatchManager
        Public Shared ReadOnly Property Instance() As PatchManager
            Get
                If (_instance Is Nothing) Then _instance = New PatchManager()
                Return _instance
            End Get
        End Property
        Public Property Host() As Form

        Private dunIni As IniFile
        Private SyncContext As Threading.SynchronizationContext
        Private WithEvents myWebClient As ExtendedWebClient
        Private WithEvents bWorker As BackgroundWorker
        Private WithEvents bWorker_DerpUninstallAndInstall As BackgroundWorker
        Public Sub New()
            Me.myWebClient = New ExtendedWebClient()
            Me.bWorker = New BackgroundWorker()
            Me.dunIni = New IniFile(IO.Path.Combine(DefineValues.Directory.Patches, "patches.ver"))
            Me.bWorker.WorkerSupportsCancellation = True
            Me.bWorker.WorkerReportsProgress = True
            Me.bWorker_DerpUninstallAndInstall = New BackgroundWorker()
            Me.bWorker_DerpUninstallAndInstall.WorkerSupportsCancellation = True
            Me.bWorker_DerpUninstallAndInstall.WorkerReportsProgress = True
            Me.myWebClient.Proxy = Nothing
            Me.myWebClient.CachePolicy = New Net.Cache.RequestCachePolicy(Net.Cache.RequestCacheLevel.NoCacheNoStore)
            Me.SyncContext = Threading.SynchronizationContext.Current
            Me.Language = "English"
            Me.Host = Nothing
            Me._currentStep = Nothing
            Me._IsBusy = False
            Me._currentMax = 100
        End Sub

        Public Sub CancelAsync()
            If (Me.bWorker.IsBusy) Then
                Me.bWorker.CancelAsync()
            End If
            If (Me.bWorker_DerpUninstallAndInstall.IsBusy) Then
                Me.bWorker_DerpUninstallAndInstall.CancelAsync()
            End If
            If (Me.myWebClient.IsBusy) Then
                Me.myWebClient.CancelAsync()
            End If
        End Sub

#Region "Building"
        Public Sub BuildData()
            If (Not Me.IsBusy) Then
                Me.RaiseEvent_StepChanged(LanguageManager.GetMessage("Patch_DownloadDatabase", "Download translation database"))
                Me.myWebClient.DownloadStringAsync(New Uri(DefineValues.EnglishPatch.DatabaseIni), New UserTokenWrapper("DownloadDatabase"))
            End If
        End Sub
#End Region

#Region "CheckUpdate"
        Public Sub CheckForUpdates()
            If (Not Me.IsBusy) Then
                If (Me.IsPatchExist) AndAlso (Me.IsPatchInstalled) Then
                    Me.bWorker_DerpUninstallAndInstall.RunWorkerAsync("checkforpatchupdate")
                End If
            End If
        End Sub
#End Region

#Region "Install"
        Public Sub Install()
            '1. Download .txt files
            '2. Copy data12.v to 'workspace'
            '3. Begin merge resource
            '4. Save the new .v file
            'Freaking install the patch for user
            If (Not Me.IsBusy) Then
                If (Me.IsPatchExist) Then
                    Me.bWorker_DerpUninstallAndInstall.RunWorkerAsync("install")
                Else
                    Me.RaiseEvent_HandledException(New PatchNotFound("Cannot find patch file. If you haven't build the patch, please do so."))
                End If
            End If
        End Sub
#End Region
#Region "Uninstall"
        Public Sub Uninstall()
            If (Not Me.IsBusy) Then
                Me.bWorker_DerpUninstallAndInstall.RunWorkerAsync("uninstall")
            End If
        End Sub
#End Region

#Region "WebClient Finished Events"
        Private Sub myWebClient_DownloadStringCompleted(ByVal sender As Object, ByVal e As DownloadStringFinishedEventArgs) Handles myWebClient.DownloadStringCompleted
            If (e.Error IsNot Nothing) Then
                Me.RaiseEvent_HandledException(e.Error)
            ElseIf (e.Cancelled) Then

            Else
                If (e.UserState IsNot Nothing) Then
                    Dim state As UserTokenWrapper = DirectCast(e.UserState, UserTokenWrapper)
                    Select Case (state.CurrentStep)
                        Case "DownloadDatabase"
                            Me.myWebClient.DownloadStringAsync(New Uri(DefineValues.EnglishPatch.EncryptionDatabaseIni), New UserTokenWrapper("DownloadEncryption", ParseDatabase(e.Result)))
                        Case "DownloadEncryption"
                            Dim pathdb As PatchDatabase = DirectCast(state.UserToken, PatchDatabase)
                            ParseEncryption(pathdb, e.Result)
                            Me.bWorker.RunWorkerAsync(New UserTokenWrapper("BuildThePatch", pathdb))
                    End Select
                End If
            End If
        End Sub
#End Region

#Region "My most private parts"
        Private Sub bWorker_Install_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles bWorker_DerpUninstallAndInstall.DoWork
            If (e.Argument IsNot Nothing) Then
                Dim state As String = DirectCast(e.Argument, String)
                Select Case (DirectCast(e.Argument, String))
                    Case "install"
                        Dim myLang As String = Me.Language
                        Dim localPatchFolder As String = IO.Path.Combine(DefineValues.Directory.Patches, myLang)
                        Dim myGameFolder As String = CommonMethods.GetGameFolder()
                        Dim gamePatchFolder As String = IO.Path.Combine(myGameFolder, patchFolder)
                        Dim muwhahahaData12 As String = IO.Path.Combine(myGameFolder, myNeededData)
                        Dim patchfile As String = IO.Path.Combine(localPatchFolder, myNeededData)
                        ' CommonMethods.EmptyFolder(gamePatchFolder)
                        If (My.Computer.FileSystem.DirectoryExists(localPatchFolder)) Then
                            If (CommonMethods.GetSHA1FromFile(patchfile) = dunIni.GetValue(myLang, DefineValues.EnglishPatch.patchchecksumValueKey, "nono")) Then
                                If (CommonMethods.GetSHA1FromFile(muwhahahaData12) = dunIni.GetValue(myLang, DefineValues.EnglishPatch.checksumValueKey, "nono")) Then
                                    Dim data12backup As String = muwhahahaData12 + ".dataBackup"
                                    If (Not My.Computer.FileSystem.FileExists(data12backup)) Then FakeShadowCopy(muwhahahaData12, data12backup, True)
                                    FakeShadowCopy(patchfile, muwhahahaData12, True)
                                    e.Result = "install"
                                Else
                                    Throw New PatchFailedException("Invalid or out-dated patch file, please rebuild the patch.")
                                End If
                            Else
                                Throw New PatchFailedException("Invalid patch file, please rebuild the patch.")
                            End If
                        Else
                            Throw New PatchFailedException("No patch files found. Please build or rebuild the patch first.")
                        End If
                    Case "uninstall"
                        Dim myGameFolder As String = CommonMethods.GetGameFolder()
                        Dim gamePatchFolder As String = IO.Path.Combine(myGameFolder, patchFolder)
                        Dim muwhahahaData12 As String = IO.Path.Combine(myGameFolder, myNeededData)
                        Dim data12backup As String = muwhahahaData12 + ".dataBackup"
                        If My.Computer.FileSystem.FileExists(data12backup) Then
                            FakeShadowCopy(data12backup, muwhahahaData12, True)
                            Try
                                System.IO.File.Delete(data12backup)
                            Catch ex As Exception

                            End Try
                        Else
                            Throw New PatchFailedException("There is no backup to restore.")
                        End If
                        e.Result = "uninstall"
                    Case "uninstall-outdated"
                        Dim myGameFolder As String = CommonMethods.GetGameFolder()
                        Dim gamePatchFolder As String = IO.Path.Combine(myGameFolder, patchFolder)
                        Dim muwhahahaData12 As String = IO.Path.Combine(myGameFolder, myNeededData)
                        Dim data12backup As String = muwhahahaData12 + ".dataBackup"
                        If My.Computer.FileSystem.FileExists(data12backup) Then
                            FakeShadowCopy(data12backup, muwhahahaData12, True)
                            Try
                                System.IO.File.Delete(data12backup)
                            Catch ex As Exception

                            End Try
                        Else
                            Throw New PatchFailedException("There is no backup to restore.")
                        End If
                        e.Result = "uninstall-outdated"
                    Case "checkforpatchupdate"
                        If (CommonMethods.GetSHA1FromFile(IO.Path.Combine(CommonMethods.GetGameFolder(), myNeededData)) = dunIni.GetValue(Me.Language, DefineValues.EnglishPatch.checksumValueKey, "nono")) Then
                            e.Result = "checkforpatchupdate-success"
                        Else
                            e.Result = "checkforpatchupdate-failed"
                        End If
                    Case Else
                        Throw New PatchFailedException("You violate meh.")
                End Select
            Else
                Throw New PatchFailedException("You violate meh.")
            End If
        End Sub

        Private Sub bWorker_Install_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles bWorker_DerpUninstallAndInstall.RunWorkerCompleted
            If (e.Error IsNot Nothing) Then
                Me.RaiseEvent_HandledException(e.Error)
            Else
                If (e.Result IsNot Nothing) Then
                    Select Case (DirectCast(e.Result, String))
                        Case "install"
                            Me.RaiseEvent_InstallPatchCompleted()
                        Case "uninstall"
                            Me.RaiseEvent_UninstallPatchCompleted()
                        Case "checkforpatchupdate-success"
                            Me.RaiseEvent_PatchValidChecked(True)
                        Case "checkforpatchupdate-failed"
                            Me.bWorker_DerpUninstallAndInstall.RunWorkerAsync("uninstall-outdated")
                            'Me.RaiseEvent_PatchValidChecked(False)
                        Case "uninstall-outdated"
                            Me.RaiseEvent_PatchValidChecked(False)
                    End Select
                End If
            End If
        End Sub

        Private Sub ParseEncryption(ByRef db As PatchDatabase, ByVal IniText As String)
            Using sr As New IO.StringReader(IniText)
                Dim DerpIni As New IniFile(sr, False)
                Dim totalkey = db.Keys
                Dim tmpFile As String
                Dim password As String
                For i As Integer = 0 To totalkey.Count - 1
                    tmpFile = IO.Path.GetFileNameWithoutExtension(totalkey(i))
                    password = DerpIni.GetValue("Zip Passwords", tmpFile, String.Empty)
                    If (Not String.IsNullOrEmpty(password)) Then
                        db.SetPassword(totalkey(i), password)
                    End If
                Next
                DerpIni.Close()
            End Using
        End Sub
        Private Function ParseVersion(ByVal lang As String, ByVal XmlText As String) As String
            ParseVersion = String.Empty
            Using sr As New IO.StringReader(XmlText)
                Using xr = New System.Xml.XmlTextReader(sr)
                    xr.MoveToContent()
                    If (xr.ReadToFollowing("jp")) Then
                        If (xr.ReadToFollowing("en")) Then
                            If (xr.ReadToFollowing("value")) Then
                                ParseVersion = xr.ReadInnerXml()
                            End If
                        End If
                    End If
                End Using
            End Using
        End Function
        Private Function ParseDatabase(ByVal IniText As String) As PatchDatabase
            '[Category]
            'path=datas\data12.v
            'path_a = ..\bin\Table\tb_booster_script.res
            'path_d = data12\tb_booster_script.txt
            'Format = 0 4 2 len 2 len 2 len 2
            ParseDatabase = New PatchDatabase()
            Using sr As New IO.StringReader(IniText)
                Dim DerpIni As New IniFile(sr, False)
                For Each section In DerpIni.Sections
                    ParseDatabase.Add(DerpIni.GetValue(section, "path", ""), DerpIni.GetValue(section, "path_a", ""), DerpIni.GetValue(section, "path_d", ""), DerpIni.GetValue(section, "Format", ""))
                Next
                DerpIni.Close()
            End Using
        End Function

        Private Sub bWorker_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles bWorker.DoWork
            Dim wrapper As UserTokenWrapper = DirectCast(e.Argument, UserTokenWrapper)
            e.Result = wrapper.CurrentStep
            Dim db As PatchDatabase = DirectCast(wrapper.UserToken, PatchDatabase)
            Dim collection = db.GetFileExact(myNeededData)
            Dim gameFolder As String = CommonMethods.GetGameFolder()
            Dim myLang As String = Me.Language
            Dim myPreciousRawFile As String = IO.Path.Combine(gameFolder, myNeededData)
            Dim myDerpedPatchFile As String = IO.Path.Combine(DefineValues.Directory.Patches, myLang, myNeededData)
            If (IO.File.Exists(myPreciousRawFile)) Then
                Me.RaiseEvent_StepChanged(LanguageManager.GetMessage("Patch_GetVersion", "Get patch version info"))
                Dim ver As String = ParseVersion(myLang, Me.myWebClient.DownloadString(DefineValues.EnglishPatch.DatabaseVersionXml))
                Dim dtVer As Date = DefineValues.DerpDate.ParseExact(ver)
                Dim sha1 As String = CommonMethods.GetSHA1FromFile(myPreciousRawFile)
                Dim MemDictionary As New Dictionary(Of String, Byte())()
                Me.RaiseEvent_StepChanged(LanguageManager.GetMessage("Patch_CopyingRawData", "Copying raw data"))
                'Copy while remove the file
                Dim myDerpCache As New WebClient.Cache.CacheManager(IO.Path.Combine(DefineValues.Directory.Patches, "Cache", myLang))
                Using fs = XorClass.XorFileStream.OpenRead(myPreciousRawFile)
                    Using archive As Ionic.Zip.ZipFile = Ionic.Zip.ZipFile.Read(fs)
                        Dim myNetworkResource As Uri
                        Dim entries = GetZipEntries(archive)
                        Dim translationDB As Dictionary(Of UInt64, List(Of String))
                        Dim dun As PatchInstructor
                        Dim myList = collection.List
                        RaiseEvent_MaxProgressChanged(myList.Count - 1)
                        Dim mydownloadedbytes() As Byte
                        For i As Integer = 0 To myList.Count - 1
                            translationDB = Nothing
                            dun = myList(i)
                            'https://github.com/Miyuyami/SoulWorkerHQTranslations/raw/master/English/data12/tb_achievement_script.txt
                            myNetworkResource = New Uri(CommonMethods.NetworkPathBuild(DefineValues.EnglishPatch.SourcePathJP, myLang, dun.PathOnInternet))
                            RaiseEvent_ProgressChanged(i)
                            If (dtVer = myDerpCache.GetCacheTime(myNetworkResource)) Then
                                Me.RaiseEvent_StepChanged(String.Format(LanguageManager.GetMessage("Patch_DownloadTranslation_LoadCache", "Load translation from cache: {0}"), dun.PathOnInternet))
                                Using cacheArchive = myDerpCache.Read(myNetworkResource, dtVer)
                                    If (cacheArchive IsNot Nothing) Then
                                        Using cacheStream = cacheArchive(0).OpenReader()
                                            RaiseEvent_StepChanged(String.Format(LanguageManager.GetMessage("Patch_ProcessTranslation", "Processing translation: {0}"), dun.PathOnInternet))
                                            translationDB = ResourceMerge.readTranslation(cacheStream)
                                        End Using
                                    End If
                                End Using
                            End If
                            If (translationDB Is Nothing) OrElse (translationDB.Count = 0) Then
                                Me.RaiseEvent_StepChanged(String.Format(LanguageManager.GetMessage("Patch_DownloadTranslation", "Download translation: {0}"), dun.PathOnInternet))
                                mydownloadedbytes = Me.myWebClient.DownloadData(myNetworkResource)
                                If (mydownloadedbytes IsNot Nothing) AndAlso (mydownloadedbytes.Length > 0) Then
                                    Using MemStream As New IO.MemoryStream(mydownloadedbytes, False)
                                        Me.RaiseEvent_StepChanged(String.Format(LanguageManager.GetMessage("Patch_DownloadTranslation_WriteCache", "Writing cache: {0}"), dun.PathOnInternet))
                                        myDerpCache.Write(myNetworkResource, MemStream.ToArray(), dtVer)
                                        Me.RaiseEvent_StepChanged(String.Format(LanguageManager.GetMessage("Patch_ProcessTranslation", "Processing translation: {0}"), dun.PathOnInternet))
                                        translationDB = ResourceMerge.readTranslation(MemStream)
                                    End Using
                                Else
                                    Me.RaiseEvent_StepChanged(String.Format(LanguageManager.GetMessage("Patch_DownloadTranslation_LoadCache", "Load translation from cache: {0}"), dun.PathOnInternet))
                                    Using cacheArchive = myDerpCache.Read(myNetworkResource, dtVer)
                                        If (cacheArchive IsNot Nothing) Then
                                            Using cacheStream = cacheArchive(0).OpenReader()
                                                RaiseEvent_StepChanged(String.Format(LanguageManager.GetMessage("Patch_ProcessTranslation", "Processing translation: {0}"), dun.PathOnInternet))
                                                translationDB = ResourceMerge.readTranslation(cacheStream)
                                            End Using
                                        End If
                                    End Using
                                End If
                                mydownloadedbytes = Nothing
                            End If
                            If (translationDB IsNot Nothing) AndAlso (translationDB.Count > 0) Then
                                Me.RaiseEvent_StepChanged(String.Format(LanguageManager.GetMessage("Patch_DownloadTranslation", "Process translation: {0}"), dun.PathOnInternet))
                                Using rawData As New IO.MemoryStream()
                                    If (String.IsNullOrEmpty(collection.Password)) Then
                                        entries(dun.PathInArchive.ToLower()).Extract(rawData)
                                    Else
                                        entries(dun.PathInArchive.ToLower()).ExtractWithPassword(rawData, collection.Password)
                                    End If
                                    If (rawData.Length > 0) Then
                                        archive.RemoveEntry(entries(dun.PathInArchive.ToLower()))
                                        Using myNewMemoryStream As New IO.MemoryStream()
                                            ResourceMerge.MergeResource(translationDB, rawData, myNewMemoryStream, ResourceMerge.ParseFormat(dun.FileFormat))
                                            MemDictionary.Add(dun.PathInArchive, myNewMemoryStream.ToArray())
                                        End Using
                                    End If
                                End Using
                            End If
                        Next
                        My.Computer.FileSystem.CreateDirectory(My.Computer.FileSystem.GetParentPath(myDerpedPatchFile))
                        Using outtt = IO.File.Create(myDerpedPatchFile & "z")
                            archive.Save(outtt)
                            outtt.Flush()
                        End Using
                    End Using
                End Using
                'ShadowXorFile(myPreciousRawFile, myDerpedPatchFile)
                If (Ionic.Zip.ZipFile.IsZipFile(myDerpedPatchFile & "z")) Then
                    If (MemDictionary.Count > 0) Then
                        Me.RaiseEvent_StepChanged(LanguageManager.GetMessage("Patch_ApplyTranslation", "Applying translation"))
                        RaiseEvent_MaxProgressChanged(MemDictionary.Count)
                        Using archive As Ionic.Zip.ZipFile = Ionic.Zip.ZipFile.Read(myDerpedPatchFile & "z")
                            archive.Password = collection.Password
                            Dim i As Integer = 0
                            For Each MergeData In MemDictionary
                                i += 1
                                RaiseEvent_ProgressChanged(i)
                                archive.AddEntry(MergeData.Key, MergeData.Value)
                            Next
                            archive.Save()
                        End Using
                        XorClass.XorFile(myDerpedPatchFile & "z", myDerpedPatchFile)
                        dunIni.SetValue(myLang, DefineValues.EnglishPatch.dateValueKey, ver)
                        dunIni.SetValue(myLang, DefineValues.EnglishPatch.checksumValueKey, sha1)
                        dunIni.SetValue(myLang, DefineValues.EnglishPatch.patchchecksumValueKey, CommonMethods.GetSHA1FromFile(myDerpedPatchFile))
                        dunIni.Save()
                    Else
                        Try
                            IO.File.Delete(myDerpedPatchFile)
                        Catch
                        End Try
                        dunIni.SetValue(myLang, DefineValues.EnglishPatch.dateValueKey, "")
                        dunIni.SetValue(myLang, DefineValues.EnglishPatch.checksumValueKey, "")
                        dunIni.SetValue(myLang, DefineValues.EnglishPatch.patchchecksumValueKey, "")
                        dunIni.Save()
                        MemDictionary.Clear()
                        'IO.File.Delete(myDerpedPatchFile & "z")
                        'Throw New PatchFailedException("Unknown Error: The patch has no changed contents")
                    End If
                    MemDictionary.Clear()
                    IO.File.Delete(myDerpedPatchFile & "z")
                Else
                    Try
                        IO.File.Delete(myDerpedPatchFile)
                        IO.File.Delete(myDerpedPatchFile & "z")
                    Catch
                    End Try
                    dunIni.SetValue(myLang, DefineValues.EnglishPatch.dateValueKey, "")
                    dunIni.SetValue(myLang, DefineValues.EnglishPatch.checksumValueKey, "")
                    dunIni.SetValue(myLang, DefineValues.EnglishPatch.patchchecksumValueKey, "")
                    dunIni.Save()
                    MemDictionary.Clear()
                    Throw New PatchFailedException("Invalid data file")
                End If
            Else
                Throw New IO.FileNotFoundException("Cannot find data12.v", "datas\data12.v")
            End If
        End Sub

        Private Function GetZipEntries(ByVal zipFile As Ionic.Zip.ZipFile) As Dictionary(Of String, Ionic.Zip.ZipEntry)
            GetZipEntries = New Dictionary(Of String, Ionic.Zip.ZipEntry)()
            For Each entr In zipFile.Entries()
                GetZipEntries.Add(IO.Path.Combine(entr.FileName.Split("/"c, "\"c)).ToLower(), entr)
            Next
        End Function

        Private Sub bWorker_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles bWorker.RunWorkerCompleted
            If (e.Error IsNot Nothing) Then
                Me.RaiseEvent_HandledException(e.Error)
            Else
                If (e.Result IsNot Nothing) Then
                    Select Case (DirectCast(e.Result, String))
                        Case "BuildThePatch"
                            Me.RaiseEvent_PatchBuildCompleted()
                    End Select
                End If
            End If
        End Sub

        Private Sub ShadowXorFile(ByVal fromPath As String, ByVal toPath As String)
            bWorker.ReportProgress(1, IO.Path.GetFileName(fromPath))
            My.Computer.FileSystem.CreateDirectory(My.Computer.FileSystem.GetParentPath(toPath))
            XorClass.XorFile(fromPath, toPath)
        End Sub

        Private Sub DewShadowFolderCopy(ByVal fromPath As String, ByVal toPath As String, ByVal overwrite As Boolean)
            Dim relativePath As String
            Dim toString As String
            For Each file In IO.Directory.GetFiles(fromPath, "*", IO.SearchOption.AllDirectories)
                relativePath = file.Remove(0, fromPath.Length).Trim("/"c, "\"c)
                toString = IO.Path.Combine(toPath, relativePath)
                My.Computer.FileSystem.CreateDirectory(My.Computer.FileSystem.GetParentPath(toString))
                My.Computer.FileSystem.CopyFile(file, toString, overwrite)
            Next
        End Sub

        Private Sub FakeShadowCopy(ByVal fromPath As String, ByVal toPath As String, ByVal overwrite As Boolean)
            My.Computer.FileSystem.CreateDirectory(My.Computer.FileSystem.GetParentPath(toPath))
            IO.File.Copy(fromPath, toPath, overwrite)
        End Sub
#End Region

#Region "Properties"
        Private _Lang As String
        Public Property Language() As String
            Get
                Return Me._Lang
            End Get
            Set(value As String)
                Me._Lang = value
                Me.RaiseEvent_LanguageChanged()
            End Set
        End Property
        Private _IsBusy As Boolean
        Public ReadOnly Property IsBusy() As Boolean
            Get
                Return (Me.myWebClient.IsBusy Or Me.bWorker.IsBusy Or Me.bWorker_DerpUninstallAndInstall.IsBusy)
            End Get
        End Property
        Public ReadOnly Property IsPatchExist() As Boolean
            Get
                Return IO.File.Exists(IO.Path.Combine(DefineValues.Directory.Patches, Me.Language, myNeededData))
            End Get
        End Property

        Public ReadOnly Property IsPatchInstalled() As Boolean
            Get
                Return IO.File.Exists(IO.Path.Combine(CommonMethods.GetGameFolder(), myNeededData + ".dataBackup"))
            End Get
        End Property

        Public ReadOnly Property PatchVersionDate() As String
            Get
                Return dunIni.GetValue(Me.Language, DefineValues.EnglishPatch.dateValueKey, "")
            End Get
        End Property
        Private _currentStep As String
        Public ReadOnly Property CurrentStep() As String
            Get
                Return Me._currentStep
            End Get
        End Property
        Private _currentMax As Integer
        Public ReadOnly Property CurrentMax() As Integer
            Get
                Return Me._currentMax
            End Get
        End Property
#End Region

#Region "Events"
        Public Event StepChanged As EventHandler(Of StringEventArgs)
        Private Sub RaiseEvent_StepChanged(ByVal str As String)
            Me._currentStep = str
            Me.SyncContext.Post(AddressOf OnStepChanged, New StringEventArgs(str))
        End Sub
        Private Sub OnStepChanged(ByVal e As Object)
            RaiseEvent StepChanged(Me, DirectCast(e, StringEventArgs))
        End Sub

        Public Event LanguageChanged As EventHandler
        Private Sub RaiseEvent_LanguageChanged()
            Me.SyncContext.Post(AddressOf OnLanguageChanged, System.EventArgs.Empty)
        End Sub
        Private Sub OnLanguageChanged(ByVal e As Object)
            RaiseEvent LanguageChanged(Me, DirectCast(e, System.EventArgs))
        End Sub
        Public Event ProgressChanged As ProgressChangedEventHandler
        Private Sub RaiseEvent_ProgressChanged(ByVal value As Integer)
            Me.SyncContext.Post(AddressOf OnProgressChanged, New ProgressChangedEventArgs(value, Nothing))
        End Sub
        Private Sub OnProgressChanged(ByVal e As Object)
            RaiseEvent ProgressChanged(Me, DirectCast(e, ProgressChangedEventArgs))
        End Sub
        Public Event MaxProgressChanged As EventHandler(Of IntegerEventArgs)
        Private Sub RaiseEvent_MaxProgressChanged(ByVal max As Integer)
            Me._currentMax = max
            Me.SyncContext.Post(AddressOf OnMaxProgressChanged, New IntegerEventArgs(max))
        End Sub
        Private Sub OnMaxProgressChanged(ByVal e As Object)
            RaiseEvent MaxProgressChanged(Me, DirectCast(e, IntegerEventArgs))
        End Sub
        Public Event InstallPatchCompleted As EventHandler
        Private Sub RaiseEvent_InstallPatchCompleted()
            Me.SyncContext.Post(AddressOf OnInstallPatchCompleted, System.EventArgs.Empty)
        End Sub
        Private Sub OnInstallPatchCompleted(ByVal e As Object)
            If (Me.Host IsNot Nothing) Then
                RaiseEvent InstallPatchCompleted(Me, DirectCast(e, System.EventArgs))
            Else
                MessageBox.Show(LanguageManager.GetMessage("Patch_Installed", "The patch has been installed"))
            End If
        End Sub
        Public Event UninstallPatchCompleted As EventHandler
        Private Sub RaiseEvent_UninstallPatchCompleted()
            Me.SyncContext.Post(AddressOf OnUninstallPatchCompleted, System.EventArgs.Empty)
        End Sub
        Private Sub OnUninstallPatchCompleted(ByVal e As Object)
            If (Me.Host IsNot Nothing) Then
                RaiseEvent UninstallPatchCompleted(Me, DirectCast(e, System.EventArgs))
            Else
                MessageBox.Show(LanguageManager.GetMessage("Patch_Uninstalled", "The patch has been uninstalled"))
            End If
        End Sub
        Public Event PatchBuildCompleted As EventHandler
        Private Sub RaiseEvent_PatchBuildCompleted()
            RaiseEvent_StepChanged(LanguageManager.GetMessage("Patch_PatchBuildCompleted", "Patch build completed."))
            Me.SyncContext.Post(AddressOf OnPatchBuildCompleted, System.EventArgs.Empty)
        End Sub
        Private Sub OnPatchBuildCompleted(ByVal e As Object)
            If (Me.Host IsNot Nothing) Then
                RaiseEvent PatchBuildCompleted(Me, DirectCast(e, System.EventArgs))
            Else
                MessageBox.Show(LanguageManager.GetMessage("Patch_RebuildCompleted", "The patch has been built successfully."))
            End If
        End Sub
        Public Delegate Sub HandledExceptionEventHandler(ByVal sender As Object, ByVal e As HandledExceptionEventArgs)
        Public Event HandledException As HandledExceptionEventHandler
        Private Sub RaiseEvent_HandledException(ByVal ex As Exception)
            Me.SyncContext.Post(AddressOf OnHandledException, New HandledExceptionEventArgs(ex))
        End Sub
        Private Sub OnHandledException(ByVal e As Object)
            Dim myevent As HandledExceptionEventArgs = DirectCast(e, HandledExceptionEventArgs)
            If (Host IsNot Nothing) Then
                RaiseEvent HandledException(Me, myevent)
            Else
                MessageBox.Show(myevent.HandledError.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Sub

        Public Event PatchValidChecked As EventHandler(Of PatchValidEventArgs)
        Private Sub RaiseEvent_PatchValidChecked(ByVal valid As Boolean)
            Me.SyncContext.Post(AddressOf OnPatchValidChecked, New PatchValidEventArgs(valid))
        End Sub
        Private Sub OnPatchValidChecked(ByVal e As Object)
            Dim myevent As PatchValidEventArgs = DirectCast(e, PatchValidEventArgs)
            If (Me.Host IsNot Nothing) Then
                RaiseEvent PatchValidChecked(Me, myevent)
            Else
                If (myevent.Valid = False) Then
                    MessageBox.Show(LanguageManager.GetMessage("Patch_Checked_InvalidPatch", "Invalid or out-dated patch file, please rebuild the patch."))
                End If
            End If
        End Sub
#End Region

#Region "Internal Classes"
        Public Class PatchValidEventArgs
            Inherits System.EventArgs
            Public ReadOnly Property Valid() As Boolean
            Public Sub New(ByVal val As Boolean)
                MyBase.New()
                Me.Valid = val
            End Sub
        End Class
        Public Class IntegerEventArgs
            Inherits System.EventArgs
            Public ReadOnly Property Value() As Integer
            Public Sub New(ByVal val As Integer)
                MyBase.New()
                Me.Value = val
            End Sub
        End Class
        Public Class StringEventArgs
            Inherits System.EventArgs
            Public ReadOnly Property Text() As String
            Public Sub New(ByVal msg As String)
                MyBase.New()
                Me.Text = msg
            End Sub
        End Class
        Public Class PatchFailedException
            Inherits System.Exception
            Public Sub New(ByVal msg As String)
                MyBase.New(msg)
            End Sub
        End Class
        Public Class PatchNotFound
            Inherits System.Exception
            Public Sub New(ByVal msg As String)
                MyBase.New(msg)
            End Sub
        End Class
        Protected Class BWorkerTokenWrapper
            Public ReadOnly Property UserToken() As Object
            Public ReadOnly Property Version() As String
            Public ReadOnly Property CurrentStep() As String
            Public Sub New(ByVal sstep As String)
                Me.New(sstep, Nothing)
            End Sub
            Public Sub New(ByVal sstep As String, ByVal token As Object)
                Me.CurrentStep = sstep
                Me.UserToken = token
            End Sub
        End Class
        Protected Class UserTokenWrapper
            Public ReadOnly Property UserToken() As Object
            Public ReadOnly Property CurrentStep() As String
            Public Sub New(ByVal sstep As String)
                Me.New(sstep, Nothing)
            End Sub
            Public Sub New(ByVal sstep As String, ByVal token As Object)
                Me.CurrentStep = sstep
                Me.UserToken = token
            End Sub
        End Class
        Protected Class PatchDatabase
            Private innerList As Dictionary(Of String, PatchInstructorCollection)
            Public ReadOnly Property Item(ByVal dataname As String) As ObjectModel.ReadOnlyCollection(Of PatchInstructor)
                Get
                    Return GetValue(dataname).AsReadOnly()
                End Get
            End Property
            Public Sub New()
                Me.innerList = New Dictionary(Of String, PatchInstructorCollection)()
            End Sub

            Public Sub Add(ByVal path As String, ByVal path_a As String, ByVal path_d As String, ByVal format As String)
                EnsureGetValue(path).List.Add(New PatchInstructor(path, path_a, path_d, format))
            End Sub

            Public ReadOnly Property Keys() As Dictionary(Of String, PatchInstructorCollection).KeyCollection
                Get
                    Return Me.innerList.Keys
                End Get
            End Property

            Public Sub SetPassword(ByVal path As String, ByVal password As String)
                EnsureGetValue(path).Password = password
            End Sub

            Private Function GetValue(ByVal key As String) As List(Of PatchInstructor)
                Dim aaaa As String = GetKey(key)
                If (String.IsNullOrEmpty(aaaa)) Then
                    GetValue = Nothing
                Else
                    GetValue = Me.innerList(GetKey(aaaa)).List
                End If
            End Function

            Private Function GetKey(ByVal key As String) As String
                GetKey = Nothing
                For i As Integer = 0 To Me.innerList.Keys.Count - 1
                    If (Me.innerList.Keys(i).ToLower() = key.ToLower()) Then
                        GetKey = Me.innerList.Keys(i)
                        Exit For
                    End If
                Next
            End Function
            Public Function GetFileExact(ByVal key As String) As PatchInstructorCollection
                GetFileExact = Nothing
                For i As Integer = 0 To Me.innerList.Keys.Count - 1
                    If (Me.innerList.Keys(i).ToLower() = key.ToLower()) Then
                        GetFileExact = Me.innerList(Me.innerList.Keys(i))
                        Exit For
                    End If
                Next
            End Function
            Public Function GetFile(ByVal key As String) As PatchInstructorCollection
                GetFile = Nothing
                For i As Integer = 0 To Me.innerList.Keys.Count - 1
                    If (IO.Path.GetFileName(Me.innerList.Keys(i)).ToLower() = key.ToLower()) Then
                        GetFile = Me.innerList(Me.innerList.Keys(i))
                        Exit For
                    End If
                Next
            End Function
            Private Function EnsureGetValue(ByVal key As String) As PatchInstructorCollection
                EnsureGetValue = Nothing
                For i As Integer = 0 To Me.innerList.Keys.Count - 1
                    If (Me.innerList.Keys(i).ToLower() = key.ToLower()) Then
                        EnsureGetValue = Me.innerList(Me.innerList.Keys(i))
                        Exit For
                    End If
                Next
                If (EnsureGetValue Is Nothing) Then
                    EnsureGetValue = New PatchInstructorCollection()
                    Me.innerList.Add(key, EnsureGetValue)
                End If
            End Function
        End Class
        Protected Class PatchInstructorCollection
            Public ReadOnly Property List() As List(Of PatchInstructor)
            Public Property Password() As String
            Public Sub New(ByVal pass As String)
                Me.List = New List(Of PatchInstructor)()
                Me.Password = pass
            End Sub
            Public Sub New()
                Me.New(String.Empty)
            End Sub
        End Class
        Protected Class PatchInstructor
            Public ReadOnly Property DataTarget() As String
            Public ReadOnly Property PathInArchive() As String
            Public ReadOnly Property PathOnInternet() As String
            Public ReadOnly Property FileFormat() As String
            Public Sub New(ByVal path As String, ByVal path_a As String, ByVal path_d As String, ByVal format As String)
                Me.DataTarget = path
                Me.PathInArchive = path_a
                Me.PathOnInternet = path_d
                Me.FileFormat = format
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
#End Region
    End Class
End Namespace
