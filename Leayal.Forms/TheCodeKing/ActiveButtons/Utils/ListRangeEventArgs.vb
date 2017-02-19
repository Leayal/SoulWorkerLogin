Imports System

Namespace TheCodeKing.ActiveButtons.Utils
	Friend Class ListRangeEventArgs
        Inherits EventArgs
        Public ReadOnly Property Count As Integer
        Public ReadOnly Property StartIndex As Integer
        Public Sub New(ByVal startIndex As Integer, ByVal count As Integer)
			MyBase.New()
            Me.StartIndex = startIndex
            Me.Count = count
        End Sub
	End Class
End Namespace