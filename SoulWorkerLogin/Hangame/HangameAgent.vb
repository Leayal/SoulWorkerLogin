Imports System.Collections.Specialized
Imports System.Text
Imports System.Threading
Imports System.Net
Imports System.IO

Namespace Hangame
    Public Class HangameAgent

#Region "Events"
        Public Delegate Sub HangameAgentLoginFinishedEventHandler(ByVal sender As Object, ByVal e As HangameAgentLoginFinishedEventArg)
        Public Event LoggedIn As HangameAgentLoginFinishedEventHandler
        Private Sub RaiseEventLoggedIn(ByVal e As HangameAgentLoginFinishedEventArg)
            SyncContext.Post(AddressOf OnLoggedIn, e)
        End Sub
        Private Sub OnLoggedIn(e As Object)
            RaiseEvent LoggedIn(Me, DirectCast(e, HangameAgentLoginFinishedEventArg))
            theTickTock.Start()
        End Sub

        Public Delegate Sub HangameAgentLogoutEventHandler(ByVal sender As Object, ByVal e As HangameAgentLoginEventArg)
        Public Event Loggedout As HangameAgentLogoutEventHandler
        Private Sub RaiseEventLogout(ByVal e As HangameAgentLoginEventArg)
            SyncContext.Post(AddressOf OnLoggedout, e)
        End Sub
        Private Sub OnLoggedout(e As Object)
            RaiseEvent Loggedout(Me, DirectCast(e, HangameAgentLoginEventArg))
            theTickTock.Stop()
        End Sub

        Public Delegate Sub SoulWorkerLaunchingEventHandler(ByVal sender As Object, ByVal e As SoulWorkerLaunchingEventArg)
        Public Event SoulWorkerLaunching As SoulWorkerLaunchingEventHandler
        Private Sub OnSoulWorkerLaunching(e As Object)
            Dim theArgs As Object() = DirectCast(e, Object())
            Dim theEvent As SoulWorkerLaunchingEventArg = DirectCast(theArgs(0), SoulWorkerLaunchingEventArg)
            RaiseEvent SoulWorkerLaunching(Me, theEvent)
            If (Not theEvent.Cancel) Then
                Dim arg2 As Byte() = DirectCast(theArgs(1), Byte())
                ParseParamAndLaunchGame(arg2)
            End If
        End Sub
        Private Sub RaiseEventSoulWorkerLaunching(ByVal e As SoulWorkerLaunchingEventArg, ByVal result As Byte())
            SyncContext.Send(AddressOf OnSoulWorkerLaunching, New Object() {e, result})
        End Sub

        Public Delegate Sub TermOfServiceRequiredEventHandler(ByVal sender As Object, ByVal e As TermOfServiceRequiredEventArg)
        Public Event TermOfServiceRequired As TermOfServiceRequiredEventHandler
        Private Sub RaiseEventTermOfServiceRequired(ByVal e As TermOfServiceRequiredEventArg)
            SyncContext.Send(AddressOf OnTermOfServiceRequired, e)
        End Sub
        Private Sub OnTermOfServiceRequired(ByVal e As Object)
            RaiseEvent TermOfServiceRequired(Me, DirectCast(e, TermOfServiceRequiredEventArg))
        End Sub
        Public Event TermOfServiceAccepted As EventHandler
        Private Sub RaiseEventTermOfServiceAccepted()
            SyncContext.Post(AddressOf OnTermOfServiceAccepted, Nothing)
        End Sub
        Private Sub OnTermOfServiceAccepted(ByVal e As Object)
            RaiseEvent TermOfServiceAccepted(Me, System.EventArgs.Empty)
        End Sub
#End Region

        Private WithEvents theWebClient As CookiesWebClient
        Private acc_Username As String
        Private acc_Password As String
        Private acc_IsLoggedIn As Boolean
        Private SyncContext As SynchronizationContext
        Private PubCloneClient As PubClone
        Private WithEvents theTickTock As Timers.Timer
        Private Shared cacheString As String

        Public Sub New()
            Me.New(New PubClone())
        End Sub

        Public Sub New(ByVal vPubCloneClient As PubClone)
            Me.New(vPubCloneClient, String.Empty, String.Empty)
        End Sub

        Public Sub New(ByVal vPubCloneClient As PubClone, ByVal sUsername As String, ByVal sPassword As String)
            Me.New(SynchronizationContext.Current, vPubCloneClient, sUsername, sPassword)
        End Sub

        Public Sub New(ByVal vSyncContext As SynchronizationContext, ByVal vPubCloneClient As PubClone)
            Me.New(vSyncContext, vPubCloneClient, String.Empty, String.Empty)
        End Sub

        Public Sub New(ByVal vSyncContext As SynchronizationContext, ByVal vPubCloneClient As PubClone, ByVal sUsername As String, ByVal sPassword As String)
            Me.theWebClient = New CookiesWebClient()
            Me.theWebClient.CachePolicy = New Cache.RequestCachePolicy(Cache.RequestCacheLevel.NoCacheNoStore)
            Me.theWebClient.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8")
            Me.theWebClient.Encoding = DefineValues.Encoding.shift_jis
            Me.acc_Username = sUsername
            Me.acc_Password = sPassword
            Me.acc_IsLoggedIn = False
            Me.SyncContext = vSyncContext
            Me.PubCloneClient = vPubCloneClient
            Me.theTickTock = New Timers.Timer(System.Convert.ToDouble(5 * 60 * 1000))
            Me.theTickTock.AutoReset = True
            Me.theTickTock.Enabled = True
            Me.theTickTock.Stop()
        End Sub

        Private Sub theTickTock_Tick(ByVal sender As Object, ByVal e As Timers.ElapsedEventArgs) Handles theTickTock.Elapsed
            If (Not Me.theWebClient.IsBusy()) Then
                Me.theWebClient.DownloadDataAsync(New Uri(DefineValues.Urls.SoulWorkerScouter), "scouter")
            End If
        End Sub

        Private Sub theWebClient_DownloadDataCompleted(ByVal sender As Object, ByVal e As DownloadDataCompletedEventArgs) Handles theWebClient.DownloadDataCompleted
            If (e.UserState IsNot Nothing) Then
                Dim State As String = DirectCast(e.UserState, String)
                If (State = "scouter") Then
                    Me.theWebClient.UpdateCookies(New Uri(DefineValues.Urls.SoulworkerHome), Me.theWebClient.ResponseCookies())
                ElseIf (State = "LaunchSoulWorkerGame2") Then
                    Me.theWebClient.UploadDataAsync(New Uri(DefineValues.Urls.SoulworkerReactorGameStart), "POST", New Byte() {}, "LaunchSoulWorkerGame3")
                End If
            End If
        End Sub

        Private Sub theWebClient_DownloadFileCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs) Handles theWebClient.DownloadFileCompleted
            If (e.Error IsNot Nothing) Then
                CommonMethods.OutputToLog(e.Error)
                MessageBox.Show(e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                If (e.UserState IsNot Nothing) Then
                    Dim ZipFilePath As String = DirectCast(e.UserState, String)
                    Dim latestVersion As String = String.Empty
                    Using TheZip As Classes.ZipStorer = Classes.ZipStorer.Open(ZipFilePath, FileAccess.Read)
                        Using MemStream As New MemoryStream()
                            Dim thelist = TheZip.ReadCentralDir()
                            If (TheZip.ExtractFile(thelist(0), MemStream)) Then
                                MemStream.Position = 0
                                Using sr As New StreamReader(MemStream)
                                    Dim TheIni As IniFile = New IniFile(sr, False)
                                    latestVersion = TheIni.GetValue(DefineValues.IniName.Ver.Section, DefineValues.IniName.Ver.Key, "")
                                    TheIni.Close()
                                End Using
                            End If
                            thelist.Clear()
                        End Using
                    End Using
                    File.Delete(ZipFilePath)
                    If (String.IsNullOrWhiteSpace(latestVersion)) Then
                        MessageBox.Show(LanguageManager.GetMessage("HangameLogin_CheckForGameUpdateFailed", "Failed to check for SoulWorker updates"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Else
                        latestVersion = latestVersion.Trim()
                        Dim TheClientIni As IniFile = New IniFile(Path.Combine(CommonMethods.GetGameFolder(), DefineValues.IniName.ClientVer))
                        If (latestVersion = TheClientIni.GetValue(DefineValues.IniName.Ver.Section, DefineValues.IniName.Ver.Key, "")) Then
                            Me.theWebClient.Headers.Set(HttpRequestHeader.Referer, "http://soulworker.hangame.co.jp/reactor/reactor.nhn")
                            Me.theWebClient.UploadDataAsync(New Uri(DefineValues.Urls.SoulworkerRegistCheck), "POST", New Byte() {}, "LaunchSoulWorkerGame1")
                        Else
                            Me.StartReactor()
                        End If
                        TheClientIni.Close()
                    End If
                End If
            End If
        End Sub

        Private Sub theWebClient_UploadValuesCompleted(ByVal sender As Object, ByVal e As UploadValuesCompletedEventArgs) Handles theWebClient.UploadValuesCompleted
            If (e.Error IsNot Nothing) Then
                CommonMethods.OutputToLog(e.Error)
                RaiseEventLoggedIn(New HangameAgentLoginFinishedEventArg(Me.acc_Username, LanguageManager.GetMessage("HangameLogin_AuthorizeFailed", "Authorize failed. Cannot connect to Hangame server. Maybe your IP/Region is blocked?") & vbNewLine & e.Error.Message))
            Else
                Dim state As String = DirectCast(e.UserState, String)
                If (state = "AgreeToService") Then
                    theWebClient.Headers.Remove(HttpRequestHeader.Referer)
                    If (e.Result.Length > 0) Then
                        RaiseEventTermOfServiceAccepted()
                    End If
                End If
            End If
        End Sub


        'encodeId=swhqtest2&encodeFlg=true&strmemberid=swhqtest2&strpassword=testswhq2&usercaptcha=%82%A0%82%A4%82%E8%82%E8%82%B7%82%C8&hcaptcha=3rDcXrQJpIOk_DFsU8668SMP-ly5JAvK4Ikff2XbplJCcvs3IQHO1o5snrPKIuDR3Hap35H9zkPT-EV-X8CahNkxwr4EQEDhIGTKLM4fcRE%3D
        Public Overloads Sub Login(ByVal vUsername As String, ByVal vPassword As String, ByVal captchaID As String, ByVal captchaString As String)
            If (Not Me.theWebClient.IsBusy()) Then
                If (Me.acc_IsLoggedIn) Then
                    If (vUsername = Me.acc_Username) Then
                        Throw New HangameAgentLoginException("You already logged in.")
                    Else
                        Me.Logout(False)
                        Me.acc_Username = vUsername
                        Me.acc_Password = vPassword
                        Dim values = New NameValueCollection()
                        If (Not String.IsNullOrWhiteSpace(captchaID)) Then
                            values.Set("encodeId", Me.acc_Username)
                            values.Set("encodeFlg", "true")
                        End If
                        values.Set(DefineValues.Web.PostId, Me.acc_Username)
                        values.Set(DefineValues.Web.PostPw, Me.acc_Password)
                        If (Not String.IsNullOrWhiteSpace(captchaID)) Then
                            values.Set("usercaptcha", captchaString)
                            values.Set("hcaptcha", captchaID)
                            theWebClient.Headers.Set(HttpRequestHeader.Referer, "http://top.hangame.co.jp/login/loginfailed.nhn?type=lfc")
                        End If
                        theWebClient.Headers.Set(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded")
                        theWebClient.UploadStringAsync(New Uri(DefineValues.Urls.HangameLogin), "POST", ValuesToString(values, DefineValues.Encoding.shift_jis), "login")
                    End If
                Else
                    Me.acc_Username = vUsername
                    Me.acc_Password = vPassword
                    Dim values = New NameValueCollection()
                    If (Not String.IsNullOrWhiteSpace(captchaID)) Then
                        values.Set("encodeId", Me.acc_Username)
                        values.Set("encodeFlg", "true")
                    End If
                    values.Set(DefineValues.Web.PostId, Me.acc_Username)
                    values.Set(DefineValues.Web.PostPw, Me.acc_Password)
                    If (Not String.IsNullOrWhiteSpace(captchaID)) Then
                        values.Set("usercaptcha", captchaString)
                        values.Set("hcaptcha", captchaID)
                        theWebClient.Headers.Set(HttpRequestHeader.Referer, "http://top.hangame.co.jp/login/loginfailed.nhn?type=lfc")
                    End If
                    theWebClient.Headers.Set(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded")
                    'theWebClient.UploadValuesAsync(New Uri(DefineValues.Urls.HangameLogin), "POST", values, "login")
                    'MessageBox.Show(ValuesToString(values))
                    'Encoding.GetEncoding("shift-jis")
                    'MessageBox.Show(ValuesToString(values, DefineValues.Encoding.shift_jis))
                    theWebClient.UploadStringAsync(New Uri(DefineValues.Urls.HangameLogin), "POST", ValuesToString(values, DefineValues.Encoding.shift_jis), "login")
                End If
            End If
        End Sub

        Private Function ValuesToString(ByVal e As NameValueCollection, ByVal encoding As System.Text.Encoding) As String
            Dim theStrinbUilder As New StringBuilder()
            Dim keys() As String = e.AllKeys
            If (keys.Length > 0) Then
                For i As Integer = 0 To keys.Length - 1
                    If (i = 0) Then
                        theStrinbUilder.Append(UrlEncode(e.GetKey(i), encoding) & "=" & UrlEncode(e.Get(i), encoding))
                    Else
                        theStrinbUilder.Append("&" & UrlEncode(e.GetKey(i), encoding) & "=" & UrlEncode(e.Get(i), encoding))
                    End If
                Next
            End If
            Return theStrinbUilder.ToString()
        End Function

        Private Function ValuesToBytes(ByVal e As NameValueCollection, ByVal encode As Encoding) As Byte()
            Dim theResult() As Byte = Nothing
            Dim keys() As String = e.AllKeys
            If (keys.Length > 0) Then
                Using theMemStream As New MemoryStream()
                    Using sr As New StreamWriter(theMemStream, encode)
                        For i As Integer = 0 To keys.Length - 1
                            If (i = 0) Then
                                sr.Write(UrlEncode(e.GetKey(i), encode) & "=" & UrlEncode(e.Get(i), encode))
                            Else
                                sr.Write("&" & UrlEncode(e.GetKey(i), encode) & "=" & UrlEncode(e.Get(i), encode))
                            End If
                        Next
                    End Using
                    theResult = theMemStream.ToArray()
                End Using
            End If
            Return theResult
        End Function

        Private Function UrlEncode(ByVal input As String, ByVal encode As System.Text.Encoding) As String
            Dim result As New Text.StringBuilder()
            Dim count As Integer = 0
            Dim up As Boolean = False
            For Each cha In Web.HttpUtility.UrlEncode(input, encode)
                If (cha = "%"c) Then
                    up = True
                    count = 0
                    result.Append("%")
                Else
                    If up Then
                        result.Append(Char.ToUpper(cha))
                        count += 1
                        If count = 2 Then up = False
                    Else
                        result.Append(cha)
                    End If
                End If
            Next
            Return result.ToString()
        End Function

        Private Sub WriteOut(ByVal Text As String)
            Using TheStreamWriter As New StreamWriter("Output.txt", True)
                TheStreamWriter.WriteLine("----------------------------------------------------------------------------------------------------" & vbNewLine & Text & vbNewLine & "----------------------------------------------------------------------------------------------------")
                TheStreamWriter.Flush()
            End Using
        End Sub

        ''' <summary>
        ''' Logout then login with different account.
        ''' </summary>
        ''' <param name="vUsername"></param>
        ''' <param name="vPassword"></param>
        Public Overloads Sub Login(ByVal vUsername As String, ByVal vPassword As String)
            Me.Login(vUsername, vPassword, String.Empty, String.Empty)
        End Sub

        Public Sub StartReactor()
            If (Me.acc_IsLoggedIn) Then
                Me.theWebClient.DownloadStringAsync(New Uri(DefineValues.Urls.SoulworkerGameStart), "startreactor")
                'If pubPlugin.IsReactorInstalled() = 1 Then
                '    Try
                '        pubPlugin.StartReactor(GetVariableValue(gameStartResponse, DefineValues.Web.ReactorStr)(0))
                '        Throw New Exception("Update the game client Using the game launcher." & vbLf & "When it finished, Close it And try 'Ready to Play' again.")
                '    Catch generatedExceptionName As IndexOutOfRangeException
                '        Throw New Exception("Validation failed. Maybe your IP/Region is blocked?")
                '    End Try
                'Else
                '    Throw New Exception("Run the game from the website first to install the plugin and reactor!")
                'End If
            Else
                Throw New HangameAgentLoginException("You must login first.")
            End If
        End Sub

        Private Sub theWebClient_UploadStringCompleted(ByVal sender As Object, ByVal e As UploadStringCompletedEventArgs) Handles theWebClient.UploadStringCompleted
            Dim State As String = String.Empty
            If (e.UserState IsNot Nothing) Then
                State = DirectCast(e.UserState, String)
            End If
            If (e.Error IsNot Nothing) Then
                CommonMethods.OutputToLog(e.Error)
                'MessageBox.Show(LanguageManager.GetMessage("HangameLogin_ValidationFailed", "Validation failed. Cannot connect to Hangame server. Maybe your IP/Region is blocked?") & vbNewLine & e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                If (State = "login") Then
                    RaiseEventLoggedIn(New HangameAgentLoginFinishedEventArg(Me.acc_Username, LanguageManager.GetMessage("HangameLogin_ValidationFailed", "Validation failed. Cannot connect to Hangame server. Maybe your IP/Region is blocked?")))
                Else
                    MessageBox.Show(LanguageManager.GetMessage("HangameLogin_ValidationFailed", "Validation failed. Cannot connect to Hangame server. Maybe your IP/Region is blocked?") & vbNewLine & e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Else
                If (State = "login") Then
                    theWebClient.Headers.Remove(HttpRequestHeader.Referer)
                    theWebClient.Headers.Remove(HttpRequestHeader.ContentType)
                    'Dim jisEncode = Encoding.GetEncoding("shift-jis")
                    Dim loginResponse = e.Result
                    Try
                        If (loginResponse.IndexOf(DefineValues.Web.MessageVariable) > -1) Then
                            Dim messages As String() = GetVariableValues(loginResponse, DefineValues.Web.MessageVariable)
                            If messages(0).Length > 0 Then
                                If (messages(0).Contains("入力されたパスワードが間違っています")) Then
                                    'WriteOut([String].Join(vbLf, messages))
                                    'incorrect password
                                    Throw New HangameAgentLoginException(Me.acc_Username, LanguageManager.GetMessage("HangameLogin_WrongLoginInfo", "ID not exist or incorrect password." & vbNewLine & "Don't try too many attempts in a short time or you will have to pass the captcha."))
                                ElseIf (messages(0).Contains("入力されたIDが間違っているか、入力されたIDが存在しません")) Then
                                    'WriteOut([String].Join(vbLf, messages))
                                    'ID not exist
                                    Throw New HangameAgentLoginException(Me.acc_Username, LanguageManager.GetMessage("HangameLogin_WrongLoginInfo", "ID not exist or incorrect password." & vbNewLine & "Don't try too many attempts in a short time or you will have to pass the captcha."))
                                ElseIf (messages(0).Contains("入力されたIDは外部サービスIDで登録された会員です。")) Then
                                    'ID is registered with external service
                                    Throw New HangameAgentLoginException(Me.acc_Username, LanguageManager.GetMessage("HangameLogin_ExternalIDService", "ID was registered with external service." & vbNewLine & "WebClient Lite can't handle these accounts (yet). Instead, please use your Internet Explorer Browser to authorize the login and play."))
                                ElseIf (messages(0).Contains("パスワードの認証に連続して失敗しました。")) Then
                                    Throw New HangameAgentLoginException(Me.acc_Username, LanguageManager.GetMessage("HangameLogin_TooManyAttempts", "Too many login attempts." & vbNewLine & "Please login with image verification."), True)
                                ElseIf (messages(0).Contains("ログインの失敗回数が多いため、画像認証が必要です。")) Then
                                    Throw New HangameAgentLoginException(Me.acc_Username, LanguageManager.GetMessage("HangameLogin_TooManyAttempts", "Too many login attempts." & vbNewLine & "Please login with image verification."), True)
                                ElseIf (messages(0).Contains("画像認証に失敗しました。")) Then
                                    Throw New HangameAgentLoginException(Me.acc_Username, LanguageManager.GetMessage("HangameLogin_WrongImageVerification", "Image verfication is incorrect." & vbNewLine & "Please try again."), True)
                                Else
                                    WriteOut([String].Join(vbLf, messages))
                                    Throw New HangameAgentLoginException(Me.acc_Username, [String].Join(vbLf, messages))
                                End If
                            Else
                                WriteOut([String].Join(vbLf, messages))
                            End If
                        Else
                            Dim theCookies As String = theWebClient.ResponseHeaders.Item(HttpResponseHeader.SetCookie)
                            If (String.IsNullOrWhiteSpace(theCookies)) Then
                                RaiseEventLoggedIn(New HangameAgentLoginFinishedEventArg(Me.acc_Username, LanguageManager.GetMessage("HangameLogin_AuthorizeFailed", "Authorize failed. Cannot connect to Hangame server. Maybe your IP/Region is blocked?")))
                            Else
                                If (theCookies.Contains("login=hangame=") AndAlso theCookies.Contains("loginstatus=hangame")) Then
                                    Me.acc_IsLoggedIn = True
                                    RaiseEventLoggedIn(New HangameAgentLoginFinishedEventArg(Me.acc_Username))
                                Else
                                    RaiseEventLoggedIn(New HangameAgentLoginFinishedEventArg(Me.acc_Username, LanguageManager.GetMessage("HangameLogin_AuthorizeFailed", "Authorize failed. Cannot connect to Hangame server. Maybe your IP/Region is blocked?")))
                                End If
                            End If
                        End If
                    Catch loginEx As HangameAgentLoginException
                        RaiseEventLoggedIn(New HangameAgentLoginFinishedEventArg(Me.acc_Username, loginEx.Message, loginEx.ImageVerification))
                    End Try
                End If
            End If
        End Sub

        Private Sub theWebClient_DownloadStringCompleted(ByVal sender As Object, ByVal e As DownloadStringCompletedEventArgs) Handles theWebClient.DownloadStringCompleted
            If (e.Error IsNot Nothing) Then
                CommonMethods.OutputToLog(e.Error)
                MessageBox.Show(LanguageManager.GetMessage("HangameLogin_ValidationFailed", "Validation failed. Cannot connect to Hangame server. Maybe your IP/Region is blocked?") & vbNewLine & e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                If (Not e.Cancelled) Then
                    If (e.UserState IsNot Nothing) Then
                        Dim State As String = DirectCast(e.UserState, String)
                        If (State = "startreactor") Then
                            Dim gameStartResponse As String = e.Result()
                            Try
                                If GetVariableValues(gameStartResponse, DefineValues.Web.ErrorCodeVariable)(0) = "03" Then
                                    'Throw New TermOfServiceRequiredException("To play the game you need to accept the Terms of Service.")
                                    Dim myEvent As TermOfServiceRequiredEventArg = New TermOfServiceRequiredEventArg()
                                    RaiseEventTermOfServiceRequired(myEvent)
                                    If myEvent.Agree Then
                                        Dim values = New NameValueCollection(1)
                                        values(DefineValues.Web.ToServiceParam) = DefineValues.Web.ToServiceValue
                                        theWebClient.UploadValuesAsync(New Uri(DefineValues.Urls.SoulworkerToServiceCompleted), "POST", values, "AgreeToService")
                                    End If
                                ElseIf GetVariableValues(gameStartResponse, DefineValues.Web.MaintenanceVariable)(0) = "C" Then
                                    Throw New Exception(LanguageManager.GetMessage("HangameLogin_GameInMaintenance", "Game is under maintenance."))
                                Else
                                    PubCloneClient.StartReactor(GetVariableValues(gameStartResponse, DefineValues.Web.ReactorStr)(0))
                                End If
                            Catch generatedExceptionName As IndexOutOfRangeException
                                'Throw New Exception("Validation failed. Maybe your IP/Region is blocked?")
                                MessageBox.Show(LanguageManager.GetMessage("HangameLogin_ValidationFailed", "Validation failed. Cannot connect to Hangame server. Maybe your IP/Region is blocked?"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Catch ex As Exception
                                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End Try
                        ElseIf (State = "logout") Then
                            Me.theWebClient.Headers.Remove(HttpRequestHeader.Referer)
                            Me.theWebClient.ClearCookies()
                            Me.acc_IsLoggedIn = False
                            RaiseEventLogout(New HangameAgentLoginEventArg(Me.acc_Username))
                        ElseIf (State = "logoutSilent") Then
                            Me.theWebClient.Headers.Remove(HttpRequestHeader.Referer)
                            Me.theWebClient.ClearCookies()
                            Me.acc_IsLoggedIn = False
                        End If
                    End If
                End If
            End If
        End Sub

        Public Function GetTermOfService() As String
            Dim theBytes As Byte() = Me.theWebClient.DownloadData(DefineValues.Urls.SoulworkerToService)
            If (theBytes IsNot Nothing) AndAlso (theBytes.Length > 0) Then
                Using theMemStream As New MemoryStream(Encoding.Convert(DefineValues.Encoding.shift_jis, Encoding.UTF8, theBytes))
                    Dim document = New HtmlAgilityPack.HtmlDocument()
                    document.Load(theMemStream, Encoding.UTF8)
                    'document.LoadHtml(theString)
                    Dim documentHeader = document.DocumentNode.SelectSingleNode("//head")
                    For Each scriptDom In documentHeader.Descendants("script").ToArray()
                        documentHeader.RemoveChild(scriptDom)
                    Next
                    For Each metaDom In documentHeader.Descendants("meta").ToArray()
                        If (metaDom.GetAttributeValue("charset", "") = "shift_jis") Then
                            metaDom.SetAttributeValue("charset", "utf-8")
                        ElseIf (metaDom.GetAttributeValue("property", "").StartsWith("og:")) Then
                            documentHeader.RemoveChild(metaDom)
                        End If
                    Next
                    If (document IsNot Nothing) Then
                        Dim elements = document.DocumentNode.Descendants("div")
                        For Each dom In elements
                            If (dom.GetAttributeValue("class", "") = "agreement_Box") Then
                                dom.SetAttributeValue("style", "overflow-y: visible; border: none; font-family: Tahoma; font-size: 14px")
                                Dim nodeString As New System.Text.StringBuilder()
                                Using sr As New StringWriter(nodeString)
                                    dom.WriteTo(sr)
                                End Using
                                Dim asd = HtmlAgilityPack.HtmlNode.CreateNode(nodeString.ToString())
                                document.DocumentNode.SelectSingleNode("//body").RemoveAllChildren()
                                document.DocumentNode.SelectSingleNode("//body").AppendChild(asd)
                                nodeString.Clear()
                                Exit For
                            End If
                        Next
                        Dim theStringBuilder As New System.Text.StringBuilder()
                        Using theStringWriter As New StringWriter(theStringBuilder)
                            document.Save(theStringWriter)
                        End Using
                        document.Save("out.html")
                        Return theStringBuilder.ToString()
                    Else
                        Return String.Empty
                    End If
                End Using
            Else
                Return String.Empty
            End If
        End Function

        Private Sub theWebClient_UploadDataCompleted(ByVal sender As Object, ByVal e As UploadDataCompletedEventArgs) Handles theWebClient.UploadDataCompleted
            Dim State As String = String.Empty
            If (e.UserState IsNot Nothing) Then
                State = DirectCast(e.UserState, String)
            End If
            If (e.Error IsNot Nothing) Then
                CommonMethods.OutputToLog(e.Error)
                MessageBox.Show(LanguageManager.GetMessage("HangameLogin_ValidationFailed", "Validation failed. Cannot connect to Hangame server. Maybe your IP/Region is blocked?") & vbNewLine & e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                If (Not String.IsNullOrEmpty(State)) Then
                    If (State = "LaunchSoulWorkerGame1") Then
                        Try
                            Dim theString As String = Encoding.UTF8.GetString(e.Result)
                            If (GetVariableValueAsInteger(theString, DefineValues.JSON.msgCode) = 0) Then
                                Me.theWebClient.DownloadDataAsync(New Uri(DefineValues.Urls.SoulworkerReactorMetric), "LaunchSoulWorkerGame2")
                            Else
                                If (GetVariableValueAsString(theString, DefineValues.JSON.ErrorCode) = "03") Then
                                    Dim myEvent As TermOfServiceRequiredEventArg = New TermOfServiceRequiredEventArg()
                                    RaiseEventTermOfServiceRequired(myEvent)
                                    If myEvent.Agree Then
                                        Dim values = New NameValueCollection(1)
                                        values(DefineValues.Web.ToServiceParam) = DefineValues.Web.ToServiceValue
                                        'http://soulworker.hangame.co.jp/entry.nhn
                                        theWebClient.Headers.Set(HttpRequestHeader.Referer, "http://soulworker.hangame.co.jp/entry.nhn")
                                        theWebClient.UploadValuesAsync(New Uri(DefineValues.Urls.SoulworkerToServiceCompleted), "POST", values, "AgreeToService")
                                    End If
                                Else
                                    MessageBox.Show(GetVariableValueAsString(theString, DefineValues.Web.GameStartArg), "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                End If
                            End If
                        Catch ex As IndexOutOfRangeException
                            MessageBox.Show(ex.StackTrace, "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        End Try
                    ElseIf (State = "LaunchSoulWorkerGame3") Then
                        Me.theWebClient.Headers.Remove(HttpRequestHeader.Referer)
                        RaiseEventSoulWorkerLaunching(New SoulWorkerLaunchingEventArg(Me.acc_Username), e.Result)
                    End If
                End If
            End If
        End Sub

        Private Sub ParseParamAndLaunchGame(ByVal theByte As Byte())
            Dim theSWFolder As String = CommonMethods.GetGameFolder()
            Dim fullasdasdexe As String = Path.Combine(theSWFolder, "soulworker100.exe")
            If (My.Computer.FileSystem.FileExists(fullasdasdexe)) Then
                Dim theString As String = Encoding.UTF8.GetString(theByte)
                If (String.IsNullOrWhiteSpace(theString)) Then
                    MessageBox.Show("Invalid access token.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Else
                    Dim ini As IniFile = New IniFile(Path.Combine(theSWFolder, DefineValues.IniName.GeneralClient))
                    Try
                        If (GetVariableValueAsInteger(theString, DefineValues.JSON.msgCode) = 0) Then
                            Dim gameStartArgs As String() = New String(3) {}
                            Dim oriLaunchString As String = GetVariableValueAsString(theString, DefineValues.Web.GameStartArg)
                            gameStartArgs(0) = oriLaunchString.Split(" "c)(0)
                            gameStartArgs(1) = ini.GetValue(DefineValues.IniName.General.SectionNetwork, DefineValues.IniName.General.KeyIP, String.Empty)
                            gameStartArgs(2) = ini.GetValue(DefineValues.IniName.General.SectionNetwork, DefineValues.IniName.General.KeyPort, String.Empty)

                            Dim startInfo = New ProcessStartInfo()
                            startInfo.UseShellExecute = True
                            If (Not CommonMethods.IsWindowsXP()) Then startInfo.Verb = "runas"
                            startInfo.Arguments = String.Join(" ", gameStartArgs.Select(Function(s) """" + s + """"))
                            startInfo.WorkingDirectory = theSWFolder
                            startInfo.FileName = fullasdasdexe
                            Process.Start(startInfo).Close()
                        Else
                            MessageBox.Show(GetVariableValueAsString(theString, DefineValues.Web.GameStartArg), "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        End If
                    Catch ex As IndexOutOfRangeException
                        MessageBox.Show("Invalid access token." & vbNewLine & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                    ini.Close()
                End If
            End If
        End Sub

        Public Sub LaunchSoulWorkerGame()
            If (Not Me.theWebClient.IsBusy()) Then
                Dim TempFileName As String = Path.GetTempFileName() & ".zip"
                If (CommonMethods.IsSoulWorkerInstalled()) Then
                    Me.theWebClient.DownloadFileAsync(New Uri(DefineValues.Urls.SoulworkerSettingsHome & DefineValues.IniName.ServerVer + ".zip"), TempFileName, TempFileName)
                Else
                    Me.StartReactor()
                End If
            End If
        End Sub

        ''' <summary>
        ''' Logout current account.
        ''' </summary>
        Public Sub Logout()
            Me.Logout(True)
        End Sub

        Private Sub Logout(ByVal bRaiseEvent As Boolean)
            If (Me.acc_IsLoggedIn) Then
                If (Not Me.theWebClient.IsBusy) Then
                    Me.theWebClient.Headers.Set(HttpRequestHeader.Referer, "http://ad2.hangame.co.jp/adshow?unit=0002K0&ac=1092868")
                    If (bRaiseEvent) Then
                        Me.theWebClient.DownloadStringAsync(New Uri(DefineValues.Urls.HangameLogout), "logout")
                    Else
                        Me.theWebClient.DownloadStringAsync(New Uri(DefineValues.Urls.HangameLogout), "logoutSilent")
                    End If
                End If
            End If
        End Sub

        Private Function GetVariableValues(ByVal fullText As String, ByVal variableName As String) As String()
            Dim result As String
            Dim valueIndex As Integer = fullText.IndexOf(variableName)

            If valueIndex = -1 Then
                Throw New IndexOutOfRangeException()
            End If

            result = fullText.Substring(valueIndex + variableName.Length + 1)
            result = result.Substring(0, result.IndexOf(""""c))

            Return result.Split(" "c)
        End Function

        Private Function GetVariableValueAsOjbect(ByVal fullText As String, ByVal variableName As String) As Object
            Dim result As Object = Nothing
            Using theStringReader As New StringReader(fullText)
                Using jsonreader As New Newtonsoft.Json.JsonTextReader(theStringReader)
                    While (jsonreader.Read())
                        If (jsonreader.TokenType = Newtonsoft.Json.JsonToken.PropertyName) Then
                            Dim theString As String = DirectCast(jsonreader.Value, String)
                            If (theString = variableName) Then
                                If (jsonreader.Read()) Then
                                    result = jsonreader.Value()
                                End If
                                Exit While
                            End If
                        End If
                    End While
                End Using
            End Using
            Return result
        End Function

        Private Function GetVariableValueAsInteger(ByVal fullText As String, ByVal variableName As String) As Integer?
            Dim result As Integer? = -1
            Using theStringReader As New StringReader(fullText)
                Using jsonreader As New Newtonsoft.Json.JsonTextReader(theStringReader)
                    While (jsonreader.Read())
                        If (jsonreader.TokenType = Newtonsoft.Json.JsonToken.PropertyName) Then
                            Dim theString As String = DirectCast(jsonreader.Value, String)
                            If (theString = variableName) Then
                                result = jsonreader.ReadAsInt32()
                                Exit While
                            End If
                        End If
                    End While
                End Using
            End Using
            Return result
        End Function

        Private Function GetVariableValueAsString(ByVal fullText As String, ByVal variableName As String) As String
            Dim result As String = Nothing
            Using theStringReader As New StringReader(fullText)
                Using jsonreader As New Newtonsoft.Json.JsonTextReader(theStringReader)
                    While (jsonreader.Read())
                        If (jsonreader.TokenType = Newtonsoft.Json.JsonToken.PropertyName) Then
                            Dim theString As String = DirectCast(jsonreader.Value, String)
                            If (theString = variableName) Then
                                result = jsonreader.ReadAsString()
                                Exit While
                            End If
                        End If
                    End While
                End Using
            End Using
            Return result
        End Function

        Public ReadOnly Property IsLoggedIn() As Boolean
            Get
                Return Me.acc_IsLoggedIn
            End Get
        End Property

        Public ReadOnly Property WebClient() As CookiesWebClient
            Get
                Return Me.theWebClient
            End Get
        End Property

        Public Sub Close()
            Me.Logout(False)
            Me.theWebClient.Dispose()
            Me.theTickTock.Stop()
            Me.theTickTock.Dispose()
        End Sub
    End Class
End Namespace
