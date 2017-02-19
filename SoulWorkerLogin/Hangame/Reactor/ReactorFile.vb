Namespace Hangame.Reactor
    Public Class ReactorFile
        Private vName As String
        Public ReadOnly Property Name() As String
            Get
                Return Me.vName
            End Get
        End Property

        Private vVersion As String
        Public ReadOnly Property Version() As String
            Get
                Return Me.vVersion
            End Get
        End Property

        Private vSelf As Boolean
        Public ReadOnly Property Self() As Boolean
            Get
                Return Me.vSelf
            End Get
        End Property

        Private vNecessary As Boolean
        Public ReadOnly Property Necessary() As Boolean
            Get
                Return Me.vNecessary
            End Get
        End Property

        Private vUrl As String
        Public ReadOnly Property Url() As String
            Get
                Return Me.vUrl
            End Get
        End Property

        Public Sub New(ByVal sName As String, sVersion As String, ByVal bSelf As Boolean, ByVal sUrl As String, ByVal bNecessary As Boolean)
            Me.vName = sName
            Me.vVersion = sVersion
            Me.vSelf = bSelf
            Me.vUrl = sUrl
            Me.vNecessary = bNecessary
        End Sub

        Public Sub New(ByVal sName As String, sVersion As String, ByVal bSefl As Boolean, ByVal sUrl As String)
            Me.New(sName, sVersion, bSefl, sUrl, True)
        End Sub

        Public Sub New(ByVal sName As String, sVersion As String, ByVal bSefl As Boolean)
            Me.New(sName, sVersion, bSefl, String.Empty, True)
        End Sub

        Public Sub New(ByVal sName As String, sVersion As String)
            Me.New(sName, sVersion, False)
        End Sub
    End Class
End Namespace
