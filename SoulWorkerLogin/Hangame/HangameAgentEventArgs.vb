Namespace Hangame
    Public Class TermOfServiceRequiredEventArg
        Inherits System.EventArgs

        Public Property Agree() As Boolean

        Public Sub New()
            MyBase.New()
            Me.Agree = False
        End Sub
    End Class
    Public Class SoulWorkerLaunchingEventArg
        Inherits System.EventArgs
        Public ReadOnly Property Username() As String
        Public Property Cancel() As Boolean

        Public Sub New(ByVal sUsername As String)
            Me.Username = sUsername
            Me.Cancel = False
        End Sub
    End Class

    Public Class HangameAgentLoginEventArg
        Inherits System.EventArgs
        Public ReadOnly Property Username() As String

        Public Sub New(ByVal sUsername As String)
            Me.Username = sUsername
        End Sub
    End Class

    Public Class HangameAgentLoginFinishedEventArg
        Inherits System.EventArgs
        Public ReadOnly Property Username() As String
        Public ReadOnly Property Reason() As String
        Public ReadOnly Property Success() As Boolean
        Public ReadOnly Property ImageVerification() As Boolean

        Public Sub New(ByVal sUsername As String)
            Me.New(sUsername, True, String.Empty)
        End Sub

        Public Sub New(ByVal sUsername As String, ByVal sReason As String)
            Me.New(sUsername, False, sReason)
        End Sub

        Public Sub New(ByVal sUsername As String, ByVal sReason As String, ByVal captcha As Boolean)
            Me.New(sUsername, False, sReason, captcha)
        End Sub

        Public Sub New(ByVal sUsername As String, ByVal bSuccess As Boolean, ByVal sReason As String)
            Me.New(sUsername, bSuccess, sReason, False)
        End Sub

        Public Sub New(ByVal sUsername As String, ByVal bSuccess As Boolean, ByVal sReason As String, ByVal captcha As Boolean)
            Me.Username = sUsername
            Me.Success = bSuccess
            Me.Reason = sReason
            Me.ImageVerification = captcha
        End Sub
    End Class
End Namespace