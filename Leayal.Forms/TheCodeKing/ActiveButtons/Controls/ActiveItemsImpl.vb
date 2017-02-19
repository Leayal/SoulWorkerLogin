Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports Leayal.Forms.TheCodeKing.ActiveButtons.Utils

Namespace TheCodeKing.ActiveButtons.Controls
	Friend Class ActiveItemsImpl
		Inherits ListWithEvents(Of IActiveItem)
		Implements IActiveItems, IList(Of IActiveItem), ICollection(Of IActiveItem), IEnumerable(Of IActiveItem), IEnumerable
		Public Sub New()
			MyBase.New()
		End Sub
	End Class
End Namespace