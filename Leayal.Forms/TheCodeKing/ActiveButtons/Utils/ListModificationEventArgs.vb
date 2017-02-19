Imports System

Namespace TheCodeKing.ActiveButtons.Utils
	Friend Class ListModificationEventArgs
        Inherits ListRangeEventArgs
        Public ReadOnly Property Modification As ListModification

        Public Sub New(ByVal modification As ListModification, ByVal startIndex As Integer, ByVal count As Integer)
			MyBase.New(startIndex, count)
            Me.Modification = modification
        End Sub
	End Class
End Namespace