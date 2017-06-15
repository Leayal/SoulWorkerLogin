Namespace DefineValues
    Public NotInheritable Class Forms
        Private Shared mySyncContext As Threading.SynchronizationContext
        Public Shared ReadOnly Property SyncContext() As Threading.SynchronizationContext
            Get
                If (mySyncContext Is Nothing) Then
                    Return Threading.SynchronizationContext.Current
                Else
                    Return mySyncContext
                End If
            End Get
        End Property
        Public Shared Sub SetSync(ByVal sync As Threading.SynchronizationContext)
            mySyncContext = sync
        End Sub
    End Class
    Public NotInheritable Class Notify
        Private Shared notificationIcon As NotifyIcon
        Public Shared ReadOnly Property NotifyIcon() As NotifyIcon
            Get
                If (notificationIcon Is Nothing) Then
                    notificationIcon = New NotifyIcon()
                    notificationIcon.Visible = False
                    notificationIcon.ContextMenuStrip = LogoutContextMenuStrip
                    notificationIcon.Icon = My.Resources.haru_sd_WM1UAm_256px
                    notificationIcon.Text = "SoulWorker WebClient Lite"
                End If
                Return notificationIcon
            End Get
        End Property
        Public Shared Sub Refresh()
            NotifyIcon.Visible = False
            NotifyIcon.Visible = True
        End Sub
        Private Shared _LoginContextMenuStrip As ContextMenuStrip
        Public Shared ReadOnly Property LoginContextMenuStrip() As ContextMenuStrip
            Get
                If (_LoginContextMenuStrip Is Nothing) Then
                    _LoginContextMenuStrip = New ContextMenuStrip()
                End If
                Return _LoginContextMenuStrip
            End Get
        End Property
        Private Shared _LogoutContextMenuStrip As ContextMenuStrip
        Public Shared ReadOnly Property LogoutContextMenuStrip() As ContextMenuStrip
            Get
                If (_LogoutContextMenuStrip Is Nothing) Then
                    _LogoutContextMenuStrip = New ContextMenuStrip()
                End If
                Return _LogoutContextMenuStrip
            End Get
        End Property
    End Class
    Public NotInheritable Class DerpDate
        Public Const MyDateFormat As String = "d/MMM/yyyy h:mm tt"
        Public Shared Function ParseExact(ByVal dtStr As String) As DateTime
            Return DateTime.ParseExact(dtStr, MyDateFormat, Globalization.CultureInfo.InvariantCulture)
        End Function
    End Class
    Public NotInheritable Class Directory
        Public Shared ReadOnly Property Patches() As String
            Get
                Return IO.Path.Combine(My.Application.Info.DirectoryPath(), "Patches")
            End Get
        End Property
    End Class
    Public NotInheritable Class IniName
        Public Const ServerVer As String = "ServerVer.ini"
        Public Const ClientVer As String = "Ver.ini"
        Public Const GeneralClient As String = "General.ini"
        Public NotInheritable Class Ver
            Public Const Section As String = "Client"
            Public Const Key As String = "ver"
        End Class
        Public NotInheritable Class General
            Public Const SectionNetwork As String = "Network Info"
            Public Const KeyIP As String = "IP"
            Public Const KeyPort As String = "PORT"
        End Class
    End Class

    Public NotInheritable Class Options
        Public Const Filename As String = "Option.ini"
        Public Const SectionAccount As String = "Account"
        Public Const SectionAccount_HideID As String = "bHideID"
        Public Const SectionAccount_ID As String = "sID"
        Public Const SectionAccount_SaveUser As String = "bSaveUser"
        Public Const SectionAdvanced As String = "Advanced"
        Public Const SectionAdvanced_LaunchGameCheckPoint As String = "bLaunchGameCheckPoint"
        Public Const SectionAdvanced_Language As String = "sLanguage"
    End Class

    Public NotInheritable Class Web
        Public Const PostId As String = "strmemberid"
        Public Const PostPw As String = "strpassword"
        Public Const ReactorStr As String = "	reactorStr = "
        Public Const GameStartArg As String = "gs"
        Public Const ErrorCodeVariable As String = "var " & JSON.ErrorCode & " = "
        Public Const MaintenanceVariable As String = "var openCloseTypeCd = "
        Public Const MessageVariable As String = "var msg = "
        Public Const ToServiceParam As String = "agree"
        Public Const ToServiceValue As String = "on"
        Public Const WebUserAgent As String = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko"
    End Class

    Public NotInheritable Class JSON
        Public Const msgCode As String = "ret"
        Public Const ErrorCode As String = "errCode"
        Public Const captchaKey As String = "key"
        Public Const captchaID As String = "captchaid"
    End Class

    Public NotInheritable Class Encoding
        Private Shared _shift_jis As System.Text.Encoding
        Private Shared _iso_post As System.Text.Encoding
        Public Shared ReadOnly Property shift_jis() As System.Text.Encoding
            Get
                If (_shift_jis Is Nothing) Then _shift_jis = System.Text.Encoding.GetEncoding("shift_jis")
                Return _shift_jis
            End Get
        End Property
        Public Shared ReadOnly Property iso_post() As System.Text.Encoding
            Get
                If (_iso_post Is Nothing) Then _iso_post = System.Text.Encoding.GetEncoding("iso-8859-1")
                Return _iso_post
            End Get
        End Property
    End Class

    Public NotInheritable Class Urls
        Public Const GithubAPIUsers As String = "https://api.github.com/users/{0}"
        Public Const SoulworkerHQTranslationThread As String = "http://soulworkerhq.com/Discussion-Official-Soul-Worker-English-Patch"
        Public Const SoulworkerHome As String = "http://soulworker.hangame.co.jp/"
        Public Const HangameLogin As String = "https://id.hangame.co.jp/login.nhn"
        Public Const HangameLogout As String = "http://id.hangame.co.jp/logout.nhn"
        Public Const SoulworkerGameStart As String = "http://soulworker.hangame.co.jp/gamestart.nhn"
        Public Const CaptchaImg As String = "http://id.hangame.co.jp/captchaimg.nhn"
        Private Shared random As Random
        Public Shared ReadOnly Property CaptchaKey() As String
            Get
                If (random Is Nothing) Then random = New Random()
                Return "http://id.hangame.co.jp/captchakey.nhn?_callback=window.__jindo2_callback.$" & random.Next(4000, 5999).ToString() & "&null"
            End Get
        End Property
        Public Const SoulworkerToService As String = "http://soulworker.hangame.co.jp/entry.nhn"
        Public Const SoulworkerToServiceCompleted As String = "http://soulworker.hangame.co.jp/entryComplete.nhn"
        Public Const SoulworkerReactorGameStart As String = "http://soulworker.hangame.co.jp/reactor/gameStart.nhn"
        Public Const SoulworkerReactorMetric As String = "http://soulworker.hangame.co.jp/metrics.nhn?reactor=gamestart"
        Public Const SoulworkerReactor As String = "http://soulworker.hangame.co.jp/reactor/reactor.nhn"
        Public Const SoulworkerRegistCheck As String = "http://soulworker.hangame.co.jp/reactor/registCheck.nhn"
        Public Const ReactorUpdateXML As String = "http://down.hangame.co.jp/jp/purple/plii/j_sw/j_sw_update.xml"
        Public Const SoulWorkerScouter As String = "http://scouter.hangame.co.jp/?soulworker.hangame.co.jp/"
        Public Const SoulworkerSettingsHome As String = "http://down.hangame.co.jp/jp/purple/plii/j_sw/"
    End Class

    Public NotInheritable Class Updater
        Public Const UpdaterPath As String = "https://sites.google.com/site/a2511346854864321/WebClientLiteUpdater.7z?attredirects=0&d=1"
        Public Const PatchPath As String = "https://sites.google.com/site/a2511346854864321/SoulWorker%20WebClient%20Lite.7z?attredirects=0&d=1"
        Public Const VersionPath As String = "https://sites.google.com/site/a2511346854864321/SWWebClientLiteVersion.dat?attredirects=0&d=1"

        Public Const SectionUpdates As String = "Updates"
        Public Const SectionUpdates_CheckAtStartup As String = "bCheckAtStartup"
    End Class

    Public NotInheritable Class EnglishPatch
        Public Const DatabaseIni As String = "https://github.com/Miyuyami/SoulWorkerHQTranslations/raw/master/jp/English/TranslationPackData.ini"
        Public Const EncryptionDatabaseIni As String = "https://github.com/Miyuyami/SoulWorkerHQTranslations/raw/master/jp/datas.ini"
        Public Const DatabaseVersionXml As String = "https://github.com/Miyuyami/SoulWorkerHQTranslations/raw/master/LanguagePacks.xml"
        Public Const SourcePathJP As String = "https://github.com/Miyuyami/SoulWorkerHQTranslations/raw/master/jp"
        Public Const dateValueKey As String = "date"
        Public Const checksumValueKey As String = "checksum"
        Public Const patchchecksumValueKey As String = "patch"
    End Class

    Public NotInheritable Class Filenames
        Public Const SoulWorkerProcess As String = "SoulWorker100"
        Public Const SoulWorkerFilename As String = SoulWorkerProcess & ".exe"
        Public Const SWGameLauncherProcess As String = "reactor"
        Public Const SWGameLauncherFilename As String = SWGameLauncherProcess & ".exe"
        Public Const SWUpdaterProcess As String = "outbound"
        Public Const SWUpdaterFilename As String = SWUpdaterProcess & ".exe"
        Private Shared theReactorFolder As String
        Private Shared theReactorFullPath As String
        Public Shared ReadOnly Property ReactorFolder() As String
            Get
                If (String.IsNullOrEmpty(theReactorFolder)) Then theReactorFolder = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Downloaded Program Files")
                'If (String.IsNullOrEmpty(theReactorFolder)) Then theReactorFolder = IO.Path.Combine(My.Application.Info.DirectoryPath, "Reactor")
                Return theReactorFolder
            End Get
        End Property
        Public Shared ReadOnly Property ReactorExecutablePath() As String
            Get
                If (String.IsNullOrEmpty(theReactorFullPath)) Then theReactorFullPath = IO.Path.Combine(ReactorFolder(), SWGameLauncherFilename)
                Return theReactorFullPath
            End Get
        End Property
    End Class
End Namespace
