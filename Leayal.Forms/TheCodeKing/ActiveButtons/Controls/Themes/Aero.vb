Imports System
Imports System.Drawing
Imports System.Windows.Forms

Namespace TheCodeKing.ActiveButtons.Controls.Themes
	Friend Class Aero
		Inherits ThemeBase
		Private maxFrameBorder As Size = Size.Empty

		Private minFrameBorder As Size = Size.Empty

		Public Overrides ReadOnly Property BackColor As Color
			Get
				Return Color.Transparent
			End Get
		End Property

		Public Overrides ReadOnly Property ButtonOffset As Point
			Get
                If (Me.m_buttonOffset = Point.Empty) Then
                    If (Not MyBase.IsToolbar) Then
                        Me.m_buttonOffset = New Point(0, -2)
                    Else
                        Me.m_buttonOffset = New Point(0, 0)
                    End If
                End If
                Return Me.m_buttonOffset
            End Get
		End Property

		Public Overrides ReadOnly Property ControlBoxSize As Size
			Get
				Dim systemButtonSize As Size
                If (Me.m_controlBoxSize = Size.Empty) Then
                    If (MyBase.IsToolbar) Then
                        If (Not Me.form.ControlBox) Then
                            Me.m_controlBoxSize = New Size(1, 0)
                        Else
                            Dim width As Integer = Me.SystemButtonSize.Width
                            systemButtonSize = Me.SystemButtonSize
                            Me.m_controlBoxSize = New Size(width, systemButtonSize.Height)
                        End If
                    ElseIf (If(Me.form.MaximizeBox OrElse Me.form.MinimizeBox, True, Not Me.form.ControlBox)) Then
                        Dim index As Integer = If(Me.form.ControlBox, 3, 0)
                        systemButtonSize = Me.SystemButtonSize
                        Dim num As Integer = index * systemButtonSize.Width
                        systemButtonSize = Me.SystemButtonSize
                        Me.m_controlBoxSize = New Size(num, systemButtonSize.Height)
                    ElseIf (Not Me.form.HelpButton) Then
                        systemButtonSize = Me.SystemButtonSize
                        Dim width1 As Integer = systemButtonSize.Width + 13
                        systemButtonSize = Me.SystemButtonSize
                        Me.m_controlBoxSize = New Size(width1, systemButtonSize.Height)
                    Else
                        systemButtonSize = Me.SystemButtonSize
                        Dim num1 As Integer = 2 * systemButtonSize.Width + 7
                        systemButtonSize = Me.SystemButtonSize
                        Me.m_controlBoxSize = New Size(num1, systemButtonSize.Height)
                    End If
                End If
                Return Me.m_controlBoxSize
            End Get
		End Property

		Public Overrides ReadOnly Property FrameBorder As Size
			Get
				Dim border3DSize As System.Drawing.Size
				Dim size As System.Drawing.Size
				If (Me.form.WindowState <> FormWindowState.Maximized) Then
					If (Me.minFrameBorder = System.Drawing.Size.Empty) Then
						Select Case Me.form.FormBorderStyle
							Case FormBorderStyle.FixedSingle
								border3DSize = SystemInformation.Border3DSize
								Me.minFrameBorder = New System.Drawing.Size(border3DSize.Width - 2, -4)
								Exit Select
							Case FormBorderStyle.Fixed3D
								border3DSize = SystemInformation.Border3DSize
								Me.minFrameBorder = New System.Drawing.Size(border3DSize.Width, -4)
								Exit Select
							Case FormBorderStyle.FixedDialog
							Label0:
								border3DSize = SystemInformation.Border3DSize
								Me.minFrameBorder = New System.Drawing.Size(border3DSize.Width - 1, -4)
								Exit Select
							Case FormBorderStyle.Sizable
								border3DSize = SystemInformation.FrameBorderSize
								Me.minFrameBorder = New System.Drawing.Size(border3DSize.Width - 3, 1)
								Exit Select
							Case FormBorderStyle.FixedToolWindow
								border3DSize = SystemInformation.FrameBorderSize
								Me.minFrameBorder = New System.Drawing.Size(border3DSize.Width - 8, -1)
								Exit Select
							Case FormBorderStyle.SizableToolWindow
								border3DSize = SystemInformation.FrameBorderSize
								Me.minFrameBorder = New System.Drawing.Size(border3DSize.Width - 3, 4)
								Exit Select
							Case Else
								GoTo Label0
						End Select
					End If
					size = Me.minFrameBorder
				Else
					If (Me.maxFrameBorder = System.Drawing.Size.Empty) Then
						Select Case Me.form.FormBorderStyle
							Case FormBorderStyle.Sizable
								border3DSize = SystemInformation.FrameBorderSize
								Me.maxFrameBorder = New System.Drawing.Size(border3DSize.Width + 2, 7)
								Exit Select
							Case FormBorderStyle.FixedToolWindow
								border3DSize = SystemInformation.FrameBorderSize
								Me.maxFrameBorder = New System.Drawing.Size(border3DSize.Width - 8, -1)
								Exit Select
							Case FormBorderStyle.SizableToolWindow
								border3DSize = SystemInformation.FrameBorderSize
								Me.maxFrameBorder = New System.Drawing.Size(border3DSize.Width - 3, 4)
								Exit Select
							Case Else
								border3DSize = SystemInformation.FrameBorderSize
								Me.maxFrameBorder = New System.Drawing.Size(border3DSize.Width - 3, 2)
								Exit Select
						End Select
					End If
					size = Me.maxFrameBorder
				End If
				Return size
			End Get
		End Property

		Public Overrides ReadOnly Property SystemButtonSize As Size
			Get
                If (Me.m_systemButtonSize = System.Drawing.Size.Empty) Then
                    If (Not MyBase.IsToolbar) Then
                        Dim captionButtonSize As System.Drawing.Size = SystemInformation.CaptionButtonSize
                        captionButtonSize.Height = captionButtonSize.Height + 1
                        Me.m_systemButtonSize = captionButtonSize
                    Else
                        Dim size As System.Drawing.Size = SystemInformation.SmallCaptionButtonSize
                        size.Height = size.Height + 2
                        size.Width = size.Width + 2
                        Me.m_systemButtonSize = size
                    End If
                End If
                Return Me.m_systemButtonSize
            End Get
		End Property

		Public Sub New(ByVal form As System.Windows.Forms.Form)
			MyBase.New(form)
		End Sub
	End Class
End Namespace