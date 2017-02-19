Imports System
Imports System.Drawing
Imports System.Windows.Forms

Namespace TheCodeKing.ActiveButtons.Controls.Themes
	Friend Class ThemeBase
		Implements ITheme
        Protected m_backColor As Color = Color.Empty

        Protected m_buttonOffset As Point = New Point(0, 0)

        Protected m_controlBoxSize As Size = Size.Empty

        Protected form As System.Windows.Forms.Form

        Protected m_frameBorder As Size = Size.Empty

        Protected m_isDisplayed As Nullable(Of Boolean)

        Protected m_isToolbar As Nullable(Of Boolean)

        Protected m_systemButtonSize As Size = Size.Empty

        Public Overridable ReadOnly Property BackColor As Color Implements ITheme.BackColor
			Get
                If (Me.m_backColor = Color.Empty) Then
                    Me.m_backColor = Color.FromKnownColor(KnownColor.Control)
                End If
                Return Me.m_backColor
            End Get
		End Property

		Public Overridable ReadOnly Property ButtonOffset As Point Implements ITheme.ButtonOffset
			Get
                Return Me.m_buttonOffset
            End Get
		End Property

        Public Overridable ReadOnly Property ControlBoxSize As Size Implements ITheme.ControlBoxSize
            Get
                Dim index As Integer
                If (Me.m_controlBoxSize = Size.Empty) Then
                    If (Not Me.m_isToolbar) Then
                        If (If(Me.form.MaximizeBox OrElse Me.form.MinimizeBox, True, Not Me.form.ControlBox)) Then
                            index = If(Me.form.ControlBox, 3, 0)
                        Else
                            index = If(Me.form.HelpButton, 2, 1)
                        End If
                        Dim width As Integer = index * Me.m_systemButtonSize.Width
                        Me.m_controlBoxSize = New Size(width, Me.m_systemButtonSize.Height)
                    ElseIf (Not Me.form.ControlBox) Then
                        Me.m_controlBoxSize = New Size(0, 0)
                    Else
                        Dim num As Integer = Me.m_systemButtonSize.Width
                        Me.m_controlBoxSize = New Size(num, Me.m_systemButtonSize.Height)
                    End If
                End If
                Return Me.m_controlBoxSize
            End Get
        End Property

        Public Overridable ReadOnly Property FrameBorder As Size Implements ITheme.FrameBorder
			Get
				Dim frameBorderSize As Size
                If (Me.m_frameBorder = Size.Empty) Then
                    Select Case Me.form.FormBorderStyle
                        Case FormBorderStyle.Sizable
                            Dim width As Integer = SystemInformation.FrameBorderSize.Width
                            frameBorderSize = SystemInformation.FrameBorderSize
                            Me.m_frameBorder = New Size(width, frameBorderSize.Height + 2)
                            Exit Select
                        Case FormBorderStyle.FixedToolWindow
                            frameBorderSize = SystemInformation.Border3DSize
                            Dim num As Integer = frameBorderSize.Width + 3
                            frameBorderSize = SystemInformation.Border3DSize
                            Me.m_frameBorder = New Size(num, frameBorderSize.Height + 3)
                            Exit Select
                        Case FormBorderStyle.SizableToolWindow
                            frameBorderSize = SystemInformation.FrameBorderSize
                            Dim width1 As Integer = frameBorderSize.Width + 2
                            frameBorderSize = SystemInformation.FrameBorderSize
                            Me.m_frameBorder = New Size(width1, frameBorderSize.Height + 2)
                            Exit Select
                        Case Else
                            frameBorderSize = SystemInformation.Border3DSize
                            Dim num1 As Integer = frameBorderSize.Width + 1
                            frameBorderSize = SystemInformation.Border3DSize
                            Me.m_frameBorder = New Size(num1, frameBorderSize.Height + 3)
                            Exit Select
                    End Select
                End If
                Return Me.m_frameBorder
            End Get
		End Property

		Public ReadOnly Property IsDisplayed As Boolean Implements ITheme.IsDisplayed
			Get
                If (Not Me.m_isDisplayed.HasValue) Then
                    If (If(Me.form.ControlBox OrElse Not String.IsNullOrEmpty(Me.form.Text), Me.form.FormBorderStyle <> FormBorderStyle.None, False)) Then
                        Me.m_isDisplayed = New Nullable(Of Boolean)(True)
                    Else
                        Me.m_isDisplayed = New Nullable(Of Boolean)(False)
                    End If
                End If
                Return Me.m_isDisplayed.Value
            End Get
		End Property

		Protected ReadOnly Property IsToolbar As Boolean
			Get
                If (Not Me.m_isToolbar.HasValue) Then
                    Me.m_isToolbar = New Nullable(Of Boolean)(If(Me.form.FormBorderStyle = FormBorderStyle.FixedToolWindow, True, Me.form.FormBorderStyle = FormBorderStyle.SizableToolWindow))
                End If
                Return Me.m_isToolbar.Value
            End Get
		End Property

		Public Overridable ReadOnly Property SystemButtonSize As Size Implements ITheme.SystemButtonSize
			Get
                If (Me.m_systemButtonSize = System.Drawing.Size.Empty) Then
                    If (Not Me.IsToolbar) Then
                        Dim width As Integer = SystemInformation.CaptionButtonSize.Width
                        Dim captionHeight As Integer = SystemInformation.CaptionHeight
                        Dim height As Integer = SystemInformation.BorderSize.Height
                        Dim border3DSize As System.Drawing.Size = SystemInformation.Border3DSize
                        Me.m_systemButtonSize = New System.Drawing.Size(width, captionHeight - 2 * Math.Max(height, border3DSize.Height) - 1)
                    Else
                        Dim size As System.Drawing.Size = SystemInformation.ToolWindowCaptionButtonSize
                        size.Height = size.Height - 4
                        size.Width = size.Width - 1
                        Me.m_systemButtonSize = size
                    End If
                End If
                Return Me.m_systemButtonSize
            End Get
		End Property

		Public Sub New(ByVal form As System.Windows.Forms.Form)
			MyBase.New()
			Me.form = form
		End Sub
	End Class
End Namespace