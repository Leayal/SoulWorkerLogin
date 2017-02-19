Imports System.Net
Imports System.IO

Namespace Classes.WebClient
    Public Class ExtendedWebClient
        Private WithEvents myInnerWebClient As CustomWebClient
        Private SyncContext As Threading.SynchronizationContext
        Public Sub New()
            Me.myInnerWebClient = New CustomWebClient()
            Me.SyncContext = Threading.SynchronizationContext.Current
            Me._IsBusy = False
            Me._LastURL = Nothing
            Me._LastLocalPath = Nothing
        End Sub

#Region "Properties"
        Public Property Retry() As Short
        Private _IsBusy As Boolean
        Public ReadOnly Property IsBusy() As Boolean
            Get
                Return (Me._IsBusy Or Me.myInnerWebClient.IsBusy)
            End Get
        End Property


        Private _LastURL As Uri
        Public ReadOnly Property LastURL() As Uri
            Get
                Return Me._LastURL
            End Get
        End Property

        Private _LastLocalPath As String
        Public ReadOnly Property LastLocalPath() As String
            Get
                Return Me._LastLocalPath
            End Get
        End Property
#End Region

#Region "Download String"
        Public Overloads Function DownloadString(ByVal address As String) As String
            Return Me.DownloadString(New Uri(address))
        End Function
        Public Overloads Function DownloadString(ByVal address As Uri) As String
            Dim result As String = Nothing
            For i As Short = 0 To Me.Retry
                Try
                    result = Me.myInnerWebClient.DownloadString(address)
                Catch webEx As WebException
                    If (IsHTTP(address)) Then
                        If (webEx.Response IsNot Nothing) Then
                            If (Not IsWorthRetry(DirectCast(webEx.Response, HttpWebResponse))) Then
                                Exit For
                            End If
                        End If
                    End If
                End Try
            Next
            Return result
        End Function
#End Region

#Region "Download Data"
        Public Overloads Function DownloadData(ByVal address As String) As Byte()
            Return Me.DownloadData(New Uri(address))
        End Function
        Public Overloads Function DownloadData(ByVal address As Uri) As Byte()
            Dim result As Byte() = Nothing
            For i As Short = 0 To Me.Retry
                Try
                    result = Me.myInnerWebClient.DownloadData(address)
                Catch webEx As WebException
                    If (IsHTTP(address)) Then
                        If (webEx.Response IsNot Nothing) Then
                            If (Not IsWorthRetry(DirectCast(webEx.Response, HttpWebResponse))) Then
                                Exit For
                            End If
                        End If
                    End If
                End Try
            Next
            Return result
        End Function
#End Region

#Region "Download File(s)"
        Public Overloads Sub DownloadFile(ByVal address As String, ByVal localpath As String)
            Me.DownloadFile(New Uri(address), localpath)
        End Sub
        Public Overloads Sub DownloadFile(ByVal address As Uri, ByVal localpath As String)
            IsFileInUse(localpath)
            For i As Short = 0 To Me.Retry
                Try
                    Me.myInnerWebClient.DownloadFile(address, localpath & ".dtmp")
                    File.Delete(localpath)
                    File.Move(localpath & ".dtmp", localpath)
                Catch webEx As WebException
                    If (IsHTTP(address)) Then
                        If (webEx.Response IsNot Nothing) Then
                            If (Not IsWorthRetry(DirectCast(webEx.Response, HttpWebResponse))) Then
                                Exit For
                            End If
                        End If
                    End If
                End Try
            Next
        End Sub

        Private Sub myInnerWebClient_DownloadFileCompleted(ByVal sender As Object, ByVal e As ComponentModel.AsyncCompletedEventArgs) Handles myInnerWebClient.DownloadFileCompleted
            If (e.Error IsNot Nothing) Then
                With (e.Error.GetType())
                    If (.Name = GetType(WebException).Name) Then
                        Dim adasd As WebException = DirectCast(e.Error, WebException)
                        If (adasd.Response IsNot Nothing) Then
                            Dim asdasdasdasd As HttpWebResponse = DirectCast(adasd.Response, HttpWebResponse)
                            If (IsWorthRetry(asdasdasdasd)) Then

                            End If
                        End If
                    Else
                        Me.RaiseEvent_HandledException(e.Error)
                    End If
                End With
            End If
        End Sub
#End Region

#Region "Private Methods"
        Private Sub IsFileInUse(ByVal path As String)
            If (File.Exists(path)) Then
                File.Open(path, FileMode.Open).Close()
            End If
        End Sub
        Private Function IsHTTP(ByVal address As Uri) As Boolean
            If (address.Scheme = Uri.UriSchemeHttp) Then
                Return True
            ElseIf (address.Scheme = Uri.UriSchemeHttps) Then
                Return True
            Else
                Return False
            End If
        End Function
        Private Function IsWorthRetry(ByVal response As HttpWebResponse) As Boolean
            Select Case (response.StatusCode)
                Case HttpStatusCode.Gone
                    Return False
                Case HttpStatusCode.NotAcceptable
                    Return False
                Case HttpStatusCode.NotImplemented
                    Return False
                Case HttpStatusCode.ProxyAuthenticationRequired
                    Return False
                Case HttpStatusCode.Forbidden
                    Return False
                Case HttpStatusCode.Unauthorized
                    Return False
                Case HttpStatusCode.PreconditionFailed
                    Return False
                Case HttpStatusCode.NotFound
                    Return False
                Case Else
                    If (response.StatusCode > 500) Then
                        Return False
                    Else
                        Return True
                    End If
            End Select
        End Function
#End Region

#Region "Events"
        Public Delegate Sub HandledExceptionEventHandler(ByVal sender As Object, ByVal e As HandledExceptionEventArgs)
        Public Event HandledException As HandledExceptionEventHandler
        Private Sub RaiseEvent_HandledException(ByVal ex As Exception)
            Me.SyncContext.Post(AddressOf OnHandledException, New HandledExceptionEventArgs(ex))
        End Sub
        Private Sub OnHandledException(ByVal ex As Object)
            RaiseEvent HandledException(Me, DirectCast(ex, HandledExceptionEventArgs))
        End Sub
#End Region
    End Class
End Namespace
