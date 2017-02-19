Imports System
Imports System.Windows.Forms
Imports Leayal.Forms.TheCodeKing.ActiveButtons.Utils

Namespace TheCodeKing.ActiveButtons.Controls.Themes
	Friend Class ThemeFactory
		Private ReadOnly form As System.Windows.Forms.Form

		Public Sub New(ByVal form As System.Windows.Forms.Form)
			MyBase.New()
			Me.form = form
		End Sub

		Public Function GetTheme() As ITheme
			Dim aero As ITheme
			If (Win32.DwmIsCompositionEnabled) Then
				aero = New TheCodeKing.ActiveButtons.Controls.Themes.Aero(Me.form)
			ElseIf (If(Not Application.RenderWithVisualStyles, False, Win32.version > 6)) Then
				aero = New Styled(Me.form)
			ElseIf (Not Application.RenderWithVisualStyles) Then
				aero = New Standard(Me.form)
			Else
				aero = New XPStyle(Me.form)
			End If
			Return aero
		End Function
	End Class
End Namespace