Imports System
Imports System.Windows.Forms

Namespace TheCodeKing.ActiveButtons.Controls
	Public Module ActiveMenu
		Public Function GetInstance(ByVal form As System.Windows.Forms.Form) As IActiveMenu
			Return ActiveMenuImpl.GetInstance(form)
		End Function
	End Module
End Namespace