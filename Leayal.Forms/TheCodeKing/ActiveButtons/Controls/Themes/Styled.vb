Imports System
Imports System.Drawing
Imports System.Windows.Forms

Namespace TheCodeKing.ActiveButtons.Controls.Themes
	Friend Class Styled
		Inherits ThemeBase
		Public Overrides ReadOnly Property BackColor As Color
			Get
				Return Color.Transparent
			End Get
		End Property

		Public Overrides ReadOnly Property FrameBorder As Size
			Get
				Dim border3DSize As Size
                If (Me.m_frameBorder = Size.Empty) Then
                    Dim formBorderStyle As System.Windows.Forms.FormBorderStyle = Me.form.FormBorderStyle
                    If (formBorderStyle <> System.Windows.Forms.FormBorderStyle.Sizable) Then
                        If (formBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow) Then
                            GoTo Label1
                        End If
                        border3DSize = SystemInformation.Border3DSize
                        Dim width As Integer = border3DSize.Width + 2
                        border3DSize = SystemInformation.Border3DSize
                        Me.m_frameBorder = New Size(width, border3DSize.Height + 2)
                        GoTo Label0
                    End If
Label1:
                    border3DSize = SystemInformation.FrameBorderSize
                    Dim num As Integer = border3DSize.Width + 1
                    border3DSize = SystemInformation.FrameBorderSize
                    Me.m_frameBorder = New Size(num, border3DSize.Height + 1)
Label0:
                End If
                Return Me.m_frameBorder
            End Get
		End Property

		Public Overrides ReadOnly Property SystemButtonSize As Size
			Get
                If (Me.m_systemButtonSize = System.Drawing.Size.Empty) Then
                    If (Not MyBase.IsToolbar) Then
                        Dim captionButtonSize As System.Drawing.Size = SystemInformation.CaptionButtonSize
                        captionButtonSize.Height = captionButtonSize.Height - 2
                        captionButtonSize.Width = captionButtonSize.Width - 2
                        Me.m_systemButtonSize = captionButtonSize
                    Else
                        Dim size As System.Drawing.Size = MyBase.SystemButtonSize
                        size.Height = size.Height + 2
                        size.Width = size.Width - 1
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