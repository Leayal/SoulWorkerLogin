Imports System.Collections
Imports System.Collections.Generic

Namespace TheCodeKing.ActiveButtons.Controls
	Public Interface IActiveItems
		Inherits IList(Of IActiveItem), ICollection(Of IActiveItem), IEnumerable(Of IActiveItem), IEnumerable

	End Interface
End Namespace