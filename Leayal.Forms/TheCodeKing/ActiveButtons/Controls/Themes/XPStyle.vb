Imports System
Imports System.Drawing
Imports System.Windows.Forms

Namespace TheCodeKing.ActiveButtons.Controls.Themes
	Friend Class XPStyle
		Inherits Styled
		Public Overrides ReadOnly Property BackColor As Color
			Get
                If (Me.m_backColor = Color.Empty) Then
                    Me.m_backColor = Color.FromKnownColor(KnownColor.ActiveBorder)
                End If
                Return Me.m_backColor
            End Get
		End Property

		Public Overrides ReadOnly Property FrameBorder As Size
			Get
                If (Me.m_frameBorder = System.Drawing.Size.Empty) Then
                    Dim size As System.Drawing.Size = MyBase.FrameBorder
                    Dim width As Integer = size.Width + 2
                    size = MyBase.FrameBorder
                    Me.m_frameBorder = New System.Drawing.Size(width, size.Height)
                End If
                Return Me.m_frameBorder
            End Get
		End Property

		Public Sub New(ByVal form As System.Windows.Forms.Form)
			MyBase.New(form)
		End Sub
	End Class
End Namespace