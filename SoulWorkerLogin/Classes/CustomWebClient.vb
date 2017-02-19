Imports System.Net

Public Class CustomWebClient
    Inherits WebClient

    Public Property UserAgent() As String
    Public Property TimeOut() As Integer

    Public Sub New()
        Me.New(5000)
    End Sub

    Public Sub New(ByVal iTimeOut As Integer)
        Me.TimeOut = iTimeOut
        Me.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko"
    End Sub

    Protected Overrides Function GetWebRequest(address As Uri) As WebRequest
        If (address.Scheme.ToLower = Uri.UriSchemeHttp Or address.Scheme.ToLower = Uri.UriSchemeHttps) Then
            Dim theHttpRequest As HttpWebRequest = DirectCast(MyBase.GetWebRequest(address), HttpWebRequest)
            theHttpRequest.Timeout = Me.TimeOut
            theHttpRequest.UserAgent = Me.UserAgent
            Return theHttpRequest
        Else
            Return MyBase.GetWebRequest(address)
        End If
    End Function
End Class
