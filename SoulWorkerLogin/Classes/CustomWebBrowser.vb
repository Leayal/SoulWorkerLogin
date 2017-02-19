Imports System.Threading

Public Class CustomWebBrowser
    Inherits System.Windows.Forms.WebBrowser

    Public Delegate Sub LockNavigateEventHandler(ByVal sender As Object, ByVal e As WebBrowserNavigatedEventArgs)
    Public Event LockedNavigate As LockNavigateEventHandler
    Protected Sub OnLockedNavigate(ByVal e As WebBrowserNavigatedEventArgs)
        Me.sync.Post(AddressOf LockedNavigateEx, e)
    End Sub
    Private Sub LockedNavigateEx(ByVal e As Object)
        RaiseEvent LockedNavigate(Me, DirectCast(e, WebBrowserNavigatedEventArgs))
    End Sub

    Public Property LockNavigate() As Boolean
    Private sync As SynchronizationContext

    Public Sub New()
        MyBase.New()
        Me.LockNavigate = False
        Me.sync = SynchronizationContext.Current
    End Sub

    Private Sub me_Navigating(ByVal sender As Object, ByVal e As WebBrowserNavigatingEventArgs) Handles Me.Navigating
        If (Me.LockNavigate) Then
            e.Cancel = True
            Dim theString As String = e.Url.OriginalString
            If (theString.StartsWith("about://")) Then
                theString = theString.Replace("about://", "http://")
            End If
            Me.OnLockedNavigate(New WebBrowserNavigatedEventArgs(New Uri(theString)))
        End If
    End Sub
End Class
