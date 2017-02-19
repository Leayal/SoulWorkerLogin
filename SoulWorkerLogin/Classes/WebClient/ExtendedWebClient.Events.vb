Namespace Classes.WebClient
    Public Class DownloadStringCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        Public ReadOnly Property Result() As String
        Public Sub New(ByVal ex As Exception, ByVal isCancelled As Boolean, ByVal returnResult As String, ByVal oUserState As Object)
            MyBase.New(ex, isCancelled, oUserState)
            Me.Result = returnResult
        End Sub
    End Class

    Public Class HandledExceptionEventArgs
        Inherits System.EventArgs
        Public ReadOnly Property HandledError() As Exception
        Public Sub New(ByVal ex As Exception)
            MyBase.New()
            Me.HandledError = ex
        End Sub
    End Class
End Namespace