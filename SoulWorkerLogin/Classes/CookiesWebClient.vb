Imports System.Net

Public Class CookiesWebClient
    Inherits System.Net.WebClient

    Public Sub ClearCookies()
        Me._container = New CookieContainer()
    End Sub

    Public Sub SetCookieContainer(ByVal cc As CookieContainer)
        Me._container = cc
    End Sub

    Public ReadOnly Property CookieContainer() As CookieContainer
        Get
            Return Me._container
        End Get
    End Property

    Public Sub UpdateCookies(ByVal vUri As Uri, ByVal vCookieCollection As CookieCollection)
        If (Me._container IsNot Nothing) Then
            Me._container.Add(vUri, vCookieCollection)
        End If
    End Sub

    Public Sub New()
        MyBase.New()
        Me.ClearCookies()
        Me.TimeOut = 10000
        Me.UserAgent = DefineValues.Web.WebUserAgent
    End Sub
    Public Property TimeOut() As Integer
    Public Property UserAgent() As String

    Private _container As CookieContainer
    Public Property ResponseCookies() As CookieCollection
        Get
            Return m_ResponseCookies
        End Get
        Private Set
            m_ResponseCookies = Value
        End Set
    End Property
    Private m_ResponseCookies As CookieCollection

    Protected Overrides Function GetWebRequest(address As Uri) As WebRequest
        If (address.Scheme = "http") Or ((address.Scheme = "https")) Then
            Dim request As HttpWebRequest = DirectCast(MyBase.GetWebRequest(address), HttpWebRequest)
            request.CookieContainer = _container
            request.UserAgent = UserAgent
            request.Timeout = Me.TimeOut
            'request.ClientCertificates.Add(new X509Certificate());
            Return request
        Else
            Return MyBase.GetWebRequest(address)
        End If
    End Function

    Protected Overrides Function GetWebResponse(request As WebRequest) As WebResponse
        If (request.RequestUri.Scheme = "http") Or ((request.RequestUri.Scheme = "https")) Then
            Dim response As HttpWebResponse = DirectCast(MyBase.GetWebResponse(request), HttpWebResponse)
            If Me.ResponseCookies IsNot Nothing Then
                Me.ResponseCookies.Add(response.Cookies)
            Else
                Me.ResponseCookies = response.Cookies
            End If

            Return response
        Else
            Return MyBase.GetWebResponse(request)
        End If

    End Function

    Protected Overrides Function GetWebResponse(request As WebRequest, result As IAsyncResult) As WebResponse
        If (request.RequestUri.Scheme = "http") Or ((request.RequestUri.Scheme = "https")) Then
            Dim response As HttpWebResponse = DirectCast(MyBase.GetWebResponse(request, result), HttpWebResponse)
            If Me.ResponseCookies IsNot Nothing Then
                Me.ResponseCookies.Add(response.Cookies)
            Else
                Me.ResponseCookies = response.Cookies
            End If
            Return response
        Else
            Return MyBase.GetWebResponse(request, result)
        End If
    End Function
End Class
