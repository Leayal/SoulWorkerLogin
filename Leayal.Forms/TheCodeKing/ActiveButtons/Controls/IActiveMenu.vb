Imports System
Imports System.Windows.Forms

Namespace TheCodeKing.ActiveButtons.Controls
	Public Interface IActiveMenu
		ReadOnly Property Items As IActiveItems

		Property ToolTip As System.Windows.Forms.ToolTip
	End Interface
End Namespace