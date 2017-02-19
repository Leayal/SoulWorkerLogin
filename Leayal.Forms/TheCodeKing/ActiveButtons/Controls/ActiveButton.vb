Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports Leayal.Forms.TheCodeKing.ActiveButtons.Controls
Imports Leayal.Forms.TheCodeKing.ActiveButtons.Utils
Imports Leayal.Forms.TheCodeKing.ActiveButtons.Controls.Themes

Namespace TheCodeKing.ActiveButtons.Controls
	Public Class ActiveButton
		Inherits Button
		Implements IActiveItem, ThemedItem
		Private buttonSize As System.Drawing.Size

		Private buttonX As Integer

		Private buttonY As Integer

        Private m_text As String

        Private textSize As System.Drawing.Size

        Private m_theme As ITheme

        Public Shadows Property BackColor As Color
			Get
				Return MyBase.BackColor
			End Get
			Set(ByVal value As Color)
				MyBase.BackColor = value
			End Set
		End Property

		Public Shadows ReadOnly Property Height As Integer Implements IActiveItem.Height
			Get
				Return MyBase.Height
			End Get
		End Property

		Public Shadows ReadOnly Property Left As Integer Implements IActiveItem.Left
			Get
				Return MyBase.Left
			End Get
		End Property

        Public Shadows Property Text() As String
            Get
                Return Me.m_text
            End Get
            Set(ByVal value As String)
                Me.m_text = value
                Me.CalcButtonSize()
            End Set
        End Property

        Property ExplicitLeft As Integer Implements ThemedItem.Left
			Get
				Return MyBase.Left
			End Get
			Set(ByVal value As Integer)
				MyBase.Left = value
			End Set
		End Property

        Property Theme As ITheme Implements ThemedItem.Theme
            Get
                Return Me.m_theme
            End Get
            Set(ByVal value As ITheme)
                Me.m_theme = value
                Me.CalcButtonSize()
            End Set
        End Property

        Property ExplicitTop As Integer Implements ThemedItem.Top
			Get
				Return MyBase.Top
			End Get
			Set(ByVal value As Integer)
				MyBase.Top = value
			End Set
		End Property

		Public Shadows ReadOnly Property Top As Integer Implements IActiveItem.Top
			Get
				Return MyBase.Top
			End Get
		End Property

		Public Shadows ReadOnly Property Width As Integer Implements IActiveItem.Width
			Get
				Return MyBase.Width
			End Get
		End Property

		Public Sub New()
			MyBase.New()
			Me.Initialize()
		End Sub

		Private Sub ActiveButton_Paint(ByVal sender As Object, ByVal e As PaintEventArgs)
			Using brush As SolidBrush = New SolidBrush(Color.FromKnownColor(KnownColor.ControlText))
				e.Graphics.DrawString(Me.Text, MyBase.Font, brush, CSng(Me.buttonX), CSng(Me.buttonY))
			End Using
		End Sub

		Private Sub ActiveButton_SystemColorsChanged(ByVal sender As Object, ByVal e As EventArgs)
			Me.CalcButtonSize()
		End Sub

		Private Sub CalcButtonSize()
            If (Me.m_theme Is Nothing) Then
                Me.buttonSize = SystemInformation.CaptionButtonSize
            Else
                Me.buttonSize = Me.m_theme.SystemButtonSize
                If (Me.BackColor = Color.Empty) Then
                    Me.BackColor = Me.m_theme.BackColor
                End If
			End If
			MyBase.Width = Me.buttonSize.Width
			MyBase.Height = Me.buttonSize.Height
			Using e As Graphics = Graphics.FromHwnd(MyBase.Handle)
				Dim sizeF As System.Drawing.SizeF = e.MeasureString(Me.Text, Me.Font)
				Me.textSize = sizeF.ToSize()
			End Using
			If (MyBase.Width < Me.textSize.Width + 6) Then
				MyBase.Width = Me.textSize.Width + 6
			End If
			Me.buttonX = (MyBase.Width - Me.textSize.Width) / 2 - 1
			Me.buttonY = (MyBase.Height - Me.textSize.Height) / 2 - 1
		End Sub

		Protected Sub Initialize()
			If (If(Win32.DwmIsCompositionEnabled, False, Not Application.RenderWithVisualStyles)) Then
				MyBase.BackColor = Color.FromKnownColor(KnownColor.Control)
			Else
				MyBase.BackColor = Color.Transparent
			End If
			Me.Font = New System.Drawing.Font(MyBase.Font.FontFamily, 7.5!, FontStyle.Regular)
			MyBase.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
			AddHandler MyBase.Paint,  New PaintEventHandler(AddressOf Me.ActiveButton_Paint)
			AddHandler MyBase.SystemColorsChanged,  New EventHandler(AddressOf Me.ActiveButton_SystemColorsChanged)
			Me.CalcButtonSize()
		End Sub
	End Class
End Namespace