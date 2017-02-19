Imports System

Namespace TheCodeKing.ActiveButtons.Utils
	Friend Class ListItemEventArgs
		Inherits EventArgs
        Public ReadOnly Property ItemIndex As Integer

        Public Sub New(ByVal itemIndex As Integer)
			MyBase.New()
            Me.ItemIndex = itemIndex
        End Sub
	End Class
End Namespace