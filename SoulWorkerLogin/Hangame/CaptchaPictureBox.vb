Imports System.IO
Imports System.ComponentModel
Imports System.Net

Namespace Hangame
    Public Class CaptchaPictureBox
        Inherits System.Windows.Forms.PictureBox
        Private _CaptchaID As String
        Private WithEvents bWorker As BackgroundWorker
        Private myWebClient As CookiesWebClient
        Private SyncContext As System.Threading.SynchronizationContext
        Public Sub New(ByVal agent As HangameAgent)
            Me.New()
            Me.Agent = agent
        End Sub
        Public Sub New()
            MyBase.New()
            Me.bWorker = New BackgroundWorker()
            Me.bWorker.WorkerSupportsCancellation = False
            Me.bWorker.WorkerReportsProgress = False
            Me.SyncContext = System.Threading.SynchronizationContext.Current
            Me._CaptchaID = Nothing
            Me._CaptchaImage = Nothing
            Me.myWebClient = New CookiesWebClient()
            Me.myWebClient.CachePolicy = New Cache.RequestCachePolicy(Cache.RequestCacheLevel.NoCacheNoStore)
            Me.myWebClient.Headers.Set(HttpRequestHeader.UserAgent, DefineValues.Web.WebUserAgent)
            Me.myWebClient.Headers.Set(HttpRequestHeader.Referer, "http://top.hangame.co.jp/login/loginfailed.nhn?type=lfc")
            Me.myWebClient.TimeOut = 10000
            Me.myWebClient.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8")
            Me.myWebClient.Headers.Set("DNT", "1")
            Me.Size = New Size(190, 52)
            Me._CaptchaReady = False
        End Sub
        Public Property Agent() As HangameAgent
        Public ReadOnly Property CaptchaID() As String
            Get
                Return Me._CaptchaID
            End Get
        End Property
        Private _CaptchaImage As Image
        Public ReadOnly Property CaptchaImage() As Image
            Get
                Return Me._CaptchaImage
            End Get
        End Property
        Private _CaptchaReady As Boolean
        Public ReadOnly Property CaptchaReady() As Boolean
            Get
                Return Me._CaptchaReady
            End Get
        End Property

        Private Sub bWorker_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles bWorker.DoWork
            'TuOkbMm_JRwnx7aRzRl5kDySfzXJ9aumhGQo7w_UDZBbQGg3JL97q6hiQ9e4mqjSbCjePCw2G1nnnmPv-F9Modkxwr4EQEDhIGTKLM4fcRE=
            Dim theImg As Image = Nothing
            Me.myWebClient.Headers.Remove(HttpRequestHeader.Accept)
            If (Agent IsNot Nothing) Then Me.myWebClient.SetCookieContainer(Agent.WebClient.CookieContainer())
            Me.myWebClient.Headers.Set(HttpRequestHeader.Referer, "http://top.hangame.co.jp/login/loginfailed.nhn?type=lfc")
            Dim theString As String = Me.myWebClient.DownloadString(DefineValues.Urls.CaptchaKey)
            If (Not String.IsNullOrWhiteSpace(theString)) Then
                theString = theString.Substring(theString.IndexOf("(")).Trim("("c, ")"c, ";"c)
                Me.myWebClient.Headers.Set(HttpRequestHeader.Referer, "http://top.hangame.co.jp/login/loginfailed.nhn?type=lfc")
                Me.myWebClient.Headers.Set(HttpRequestHeader.Accept, "image/webp,image/*,*/*;q=0.8")
                Dim myJsonDictionary As Dictionary(Of String, String) = GetVariableValueAsDictionary(theString, DefineValues.JSON.captchaKey)
                Me._CaptchaID = myJsonDictionary(DefineValues.JSON.captchaID)
                Dim address As Uri = New Uri(DefineValues.Urls.CaptchaImg & DictionaryToQueryString(myJsonDictionary))
                'Accept: image/webp,image/*,*/*;q=0.8
                Dim theByte() As Byte = myWebClient.DownloadData(address)
                If (theByte IsNot Nothing) AndAlso (theByte.Length > 0) Then
                    Using theMemStream As New MemoryStream(theByte, False)
                        theImg = Bitmap.FromStream(theMemStream)
                    End Using
                End If
                If (theImg IsNot Nothing) Then
                    FixedSize(theImg, 310, 84)
                End If
                theByte = Nothing
            End If
            e.Result = theImg
        End Sub

        Private Sub bWorker_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles bWorker.RunWorkerCompleted
            If (e.Error IsNot Nothing) Then
                MessageBox.Show("Failed to request Captcha image." & vbNewLine & e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                If (e.Result IsNot Nothing) Then
                    Me._CaptchaImage = DirectCast(e.Result, Image)
                    Me.Image = Me._CaptchaImage
                    Me.RaiseEventCaptchaLoaded()
                Else
                    MessageBox.Show("Failed to request Captcha image.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End If
        End Sub

        Public Sub ReloadCaptcha()
            Me.Image = My.Resources.Loading
            Me.SizeMode = PictureBoxSizeMode.Zoom
            Me.bWorker.RunWorkerAsync()
            Me.RaiseEventCaptchaLoading()
        End Sub

        Private Function GetVariableValueAsDictionary(ByVal fullText As String, ByVal variableName As String) As Dictionary(Of String, String)
            Dim result As Dictionary(Of String, String) = Newtonsoft.Json.Linq.JObject.Parse(fullText).SelectToken(variableName).ToObject(Of Dictionary(Of String, String))()
            Return result
        End Function

        Private Function DictionaryToQueryString(ByVal dict As Dictionary(Of String, String)) As String
            If (dict.Count > 0) Then
                Dim theStringBuilder As New System.Text.StringBuilder()
                Dim isFirst As Boolean = True
                For Each item In dict
                    If isFirst Then
                        If (String.IsNullOrEmpty(item.Value)) Then
                            theStringBuilder.Append("?" & item.Key)
                        Else
                            theStringBuilder.Append("?" & item.Key & "=" & item.Value)
                        End If
                    Else
                        If (String.IsNullOrEmpty(item.Value)) Then
                            theStringBuilder.Append("&" & item.Key)
                        Else
                            theStringBuilder.Append("&" & item.Key & "=" & item.Value)
                        End If
                    End If
                Next
                Return theStringBuilder.ToString()
            Else
                Return String.Empty
            End If
        End Function

        Public Sub CopyCaptchaImage()
            If (Me._CaptchaReady) AndAlso (Me._CaptchaImage IsNot Nothing) Then
                Clipboard.SetImage(Me._CaptchaImage)
            End If
        End Sub

        Private Sub FixedSize(ByRef imgPhoto As Image, Width As Integer, Height As Integer)
            Dim sourceWidth As Integer = imgPhoto.Width
            Dim sourceHeight As Integer = imgPhoto.Height
            Dim sourceX As Integer = 0
            Dim sourceY As Integer = 0
            Dim destX As Integer = 0
            Dim destY As Integer = 0

            Dim nPercent As Single = 0
            Dim nPercentW As Single = 0
            Dim nPercentH As Single = 0

            nPercentW = (CSng(Width) / CSng(sourceWidth))
            nPercentH = (CSng(Height) / CSng(sourceHeight))
            If nPercentH < nPercentW Then
                nPercent = nPercentH
                destX = System.Convert.ToInt16((Width - (sourceWidth * nPercent)) / 2)
            Else
                nPercent = nPercentW
                destY = System.Convert.ToInt16((Height - (sourceHeight * nPercent)) / 2)
            End If

            Dim destWidth As Integer = CInt(sourceWidth * nPercent)
            Dim destHeight As Integer = CInt(sourceHeight * nPercent)

            Dim bmPhoto As New Bitmap(Width, Height, Imaging.PixelFormat.Format32bppArgb)
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution)

            Dim grPhoto As Graphics = Graphics.FromImage(bmPhoto)
            grPhoto.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            grPhoto.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
            grPhoto.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            grPhoto.Clear(Color.Transparent)

            grPhoto.DrawImage(imgPhoto, New Rectangle(destX, destY, destWidth, destHeight), New Rectangle(sourceX, sourceY, sourceWidth, sourceHeight), GraphicsUnit.Pixel)

            grPhoto.Dispose()
            imgPhoto.Dispose()
            imgPhoto = bmPhoto
        End Sub

        Public Shadows Sub Dispose()
            Me.bWorker.Dispose()
            MyBase.Dispose()
        End Sub

        Public Event CaptchaLoading As EventHandler
        Private Sub RaiseEventCaptchaLoading()
            Me._CaptchaReady = False
            SyncContext.Send(AddressOf OnCaptchaLoading, System.EventArgs.Empty)
        End Sub
        Private Sub OnCaptchaLoading(ByVal e As Object)
            RaiseEvent CaptchaLoading(Me, DirectCast(e, System.EventArgs))
        End Sub

        Public Event CaptchaLoaded As EventHandler
        Private Sub RaiseEventCaptchaLoaded()
            Me._CaptchaReady = True
            SyncContext.Send(AddressOf OnCaptchaLoaded, System.EventArgs.Empty)
        End Sub
        Private Sub OnCaptchaLoaded(ByVal e As Object)
            RaiseEvent CaptchaLoaded(Me, DirectCast(e, System.EventArgs))
        End Sub
    End Class
End Namespace
