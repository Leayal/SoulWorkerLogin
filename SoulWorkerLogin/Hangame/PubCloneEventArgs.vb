Public Class PubCloneProgressChangedEventArg
    Inherits System.EventArgs

    Private vFileName As String
    Private vCount As Integer
    Private vTotal As Integer

    Public Sub New(ByVal sFileName As String, ByVal iCount As Integer, ByVal iTotal As Integer)
        Me.vCount = iCount
        Me.vTotal = iTotal
        Me.vFileName = sFileName
    End Sub

    Public Sub New(ByVal iCount As Integer, ByVal iTotal As Integer)
        Me.New(String.Empty, iCount, iTotal)
    End Sub

    Public ReadOnly Property Count() As Integer
        Get
            Return Me.vCount
        End Get
    End Property

    Public ReadOnly Property Total() As Integer
        Get
            Return Me.vTotal
        End Get
    End Property

    Public ReadOnly Property Filename() As String
        Get
            Return Me.vFileName
        End Get
    End Property
End Class
