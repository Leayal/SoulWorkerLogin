Namespace Hangame
    Public Class TermOfServiceRequiredException
        Inherits System.Exception
        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal msg As String)
            MyBase.New(msg)
        End Sub
    End Class
    Public Class HangameAgentLoginException
        Inherits System.Exception
        'Public Overrides ReadOnly Property Message() As String
        Public ReadOnly Property ImageVerification() As Boolean
        Public Sub New(ByVal Reason As String)
            MyBase.New(Reason)
            Me.ImageVerification = False
        End Sub
        Public Sub New(ByVal Username As String, ByVal Reason As String)
            Me.New("Failed to log '" & Username & "' in." & vbNewLine & " Reason: " & Reason)
        End Sub

        Public Sub New(ByVal Username As String, ByVal Reason As String, ByVal captcha As Boolean)
            Me.New("Failed to log '" & Username & "' in." & vbNewLine & " Reason: " & Reason)
            Me.ImageVerification = captcha
        End Sub
    End Class
End Namespace
