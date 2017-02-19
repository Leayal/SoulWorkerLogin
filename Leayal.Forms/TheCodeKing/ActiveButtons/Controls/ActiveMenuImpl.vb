Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports Leayal.Forms.TheCodeKing.ActiveButtons.Controls.Themes
Imports Leayal.Forms.TheCodeKing.ActiveButtons.Utils

Namespace TheCodeKing.ActiveButtons.Controls
	Friend Class ActiveMenuImpl
		Inherits Form
		Implements IActiveMenu
		Private ReadOnly Shared parents As Dictionary(Of Form, IActiveMenu)

		Private ReadOnly components As IContainer

        Private ReadOnly m_items As ActiveItemsImpl

        Private ReadOnly originalMinSize As System.Drawing.Size

        Private ReadOnly m_parentForm As LeayalExtendedForm

        Private ReadOnly spillOverMode As TheCodeKing.ActiveButtons.Controls.SpillOverMode

		Private ReadOnly themeFactory As TheCodeKing.ActiveButtons.Controls.Themes.ThemeFactory

		Private containerMaxWidth As Integer

		Private isActivated As Boolean

		Private theme As ITheme

        Private m_tooltip As System.Windows.Forms.ToolTip

        Protected Overrides ReadOnly Property CreateParams As System.Windows.Forms.CreateParams
			Get
				Dim p As System.Windows.Forms.CreateParams = MyBase.CreateParams
				p.Style = 1073741824
				Dim style As System.Windows.Forms.CreateParams = p
				style.Style = style.Style Or 67108864
				Dim exStyle As System.Windows.Forms.CreateParams = p
				exStyle.ExStyle = exStyle.ExStyle And 524288
				p.Parent = Win32.GetDesktopWindow()
				Return p
			End Get
		End Property

		Public ReadOnly Property Items As IActiveItems Implements IActiveMenu.Items
			Get
                Return Me.m_items
            End Get
		End Property

        Public Property ToolTip As System.Windows.Forms.ToolTip Implements IActiveMenu.ToolTip
            Get
                Dim toolTip1 As System.Windows.Forms.ToolTip = Me.m_tooltip
                If (toolTip1 Is Nothing) Then
                    Dim toolTip2 As System.Windows.Forms.ToolTip = New System.Windows.Forms.ToolTip()
                    Dim toolTip3 As System.Windows.Forms.ToolTip = toolTip2
                    Me.m_tooltip = toolTip2
                    toolTip1 = toolTip3
                End If
                Return toolTip1
            End Get
            Set(ByVal value As System.Windows.Forms.ToolTip)
                Me.m_tooltip = value
            End Set
        End Property

        Shared Sub New()
			ActiveMenuImpl.parents = New Dictionary(Of Form, IActiveMenu)()
		End Sub

		Private Sub New(ByVal form As System.Windows.Forms.Form)
			MyBase.New()
			Me.InitializeComponent()
            Me.m_items = New ActiveItemsImpl()
            AddHandler Me.m_items.CollectionModified, New EventHandler(Of ListModificationEventArgs)(AddressOf Me.ItemsCollectionModified)
            Me.m_parentForm = form
            MyBase.Show(form)
            AddHandler Me.m_parentForm.Disposed, New EventHandler(AddressOf Me.ParentFormDisposed)
            MyBase.Visible = False
			Me.isActivated = form.WindowState <> FormWindowState.Minimized
			Me.themeFactory = New TheCodeKing.ActiveButtons.Controls.Themes.ThemeFactory(form)
			Me.theme = Me.themeFactory.GetTheme()
			Me.originalMinSize = form.MinimumSize
			Me.AttachHandlers()
			Me.ToolTip.ShowAlways = True
			MyBase.TopMost = form.TopMost
			MyBase.TopMost = False
			Me.spillOverMode = TheCodeKing.ActiveButtons.Controls.SpillOverMode.IncreaseSize
		End Sub

		Protected Sub AttachHandlers()
            AddHandler Me.m_parentForm.Deactivate, New EventHandler(AddressOf Me.ParentFormDeactivate)
            AddHandler Me.m_parentForm.Activated, New EventHandler(AddressOf Me.ParentFormActivated)
            AddHandler Me.m_parentForm.SizeChanged, New EventHandler(AddressOf Me.ParentRefresh)
            AddHandler Me.m_parentForm.VisibleChanged, New EventHandler(AddressOf Me.ParentRefresh)
            AddHandler Me.m_parentForm.Move, New EventHandler(AddressOf Me.ParentRefresh)
            AddHandler Me.m_parentForm.SystemColorsChanged, New EventHandler(AddressOf Me.TitleButtonSystemColorsChanged)
            If (Not Win32.DwmIsCompositionEnabled) Then
				Me.BackColor = Color.FromKnownColor(KnownColor.ActiveCaption)
				MyBase.TransparencyKey = Me.BackColor
			Else
				Me.BackColor = Color.Fuchsia
				MyBase.TransparencyKey = Color.Fuchsia
			End If
		End Sub

		Private Sub CalcSize()
			Dim left As Integer = 0
			Dim i As Integer = Me.Items.Count - 1
			While i >= 0
				Dim button As ThemedItem = DirectCast(Me.Items(i), ThemedItem)
				button.Theme = Me.theme
				button.Left = left
				Dim width As Integer = Me.Items(i).Width
				Dim buttonOffset As Point = Me.theme.ButtonOffset
				left = left + width + buttonOffset.X
				buttonOffset = Me.theme.ButtonOffset
				button.Top = buttonOffset.Y
				i = i - 1
			End While
			Me.containerMaxWidth = left
			If (Me.spillOverMode = TheCodeKing.ActiveButtons.Controls.SpillOverMode.IncreaseSize) Then
				Dim num As Integer = Me.containerMaxWidth
				Dim controlBoxSize As System.Drawing.Size = Me.theme.ControlBoxSize
				Dim width1 As Integer = num + controlBoxSize.Width
				controlBoxSize = Me.theme.FrameBorder
				Dim num1 As Integer = width1 + controlBoxSize.Width
				controlBoxSize = Me.theme.FrameBorder
				Dim w As Integer = num1 + controlBoxSize.Width
                Me.m_parentForm.MinimumSize = Me.originalMinSize
                If (Me.m_parentForm.MinimumSize.Width <= w) Then
                    Dim size As Form = Me.m_parentForm
                    controlBoxSize = Me.ParentForm.MinimumSize
                    size.MinimumSize = New System.Drawing.Size(w, controlBoxSize.Height)
                End If
            End If
		End Sub

		Protected Overrides Sub Dispose(ByVal disposing As Boolean)
			If (If(Not disposing, False, Me.components IsNot Nothing)) Then
				Me.components.Dispose()
			End If
			MyBase.Dispose(disposing)
		End Sub

		Public Shared Function GetInstance(ByVal form As System.Windows.Forms.Form) As IActiveMenu
			If (Not ActiveMenuImpl.parents.ContainsKey(form)) Then
				ActiveMenuImpl.parents.Add(form, New ActiveMenuImpl(form))
			End If
			Return ActiveMenuImpl.parents(form)
		End Function

		Private Sub InitializeComponent()
			MyBase.SuspendLayout()
			Me.AutoSize = True
			MyBase.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
			MyBase.ClientSize = New System.Drawing.Size(10, 10)
			MyBase.ControlBox = False
			MyBase.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
			MyBase.MaximizeBox = False
			MyBase.MinimizeBox = False
			MyBase.Name = "ActiveMenu"
			MyBase.ShowIcon = False
			MyBase.ShowInTaskbar = False
			MyBase.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
			MyBase.ResumeLayout(False)
		End Sub

		Private Sub ItemsCollectionModified(ByVal sender As Object, ByVal e As ListModificationEventArgs)
			MyBase.Controls.Clear()
			For Each button As ActiveButton In Me.Items
				MyBase.Controls.Add(button)
			Next
			Me.CalcSize()
			Me.OnPosition()
		End Sub

		Protected Overrides Sub OnLoad(ByVal e As EventArgs)
			MyBase.OnLoad(e)
			MyBase.BringToFront()
		End Sub

		Private Sub OnPosition()
			If (Not MyBase.IsDisposed) Then
				If (If(Me.theme Is Nothing, False, Me.theme.IsDisplayed)) Then
					Dim top As Integer = Me.theme.FrameBorder.Height
					Dim left As Integer = Me.theme.FrameBorder.Width + Me.theme.ControlBoxSize.Width
                    MyBase.Top = top + Me.m_parentForm.Top
                    MyBase.Left = Me.m_parentForm.Left + Me.m_parentForm.Width - Me.containerMaxWidth - left
                    MyBase.Visible = If(Not Me.theme.IsDisplayed, False, Me.isActivated)
					If (MyBase.Visible) Then
						If (Me.Items.Count > 0) Then
                            MyBase.Opacity = Me.m_parentForm.Opacity
                            If (Not Me.m_parentForm.Visible) Then
                                MyBase.Visible = False
                            Else
                                MyBase.Opacity = Me.m_parentForm.Opacity
                            End If
						End If
						If (Me.spillOverMode = TheCodeKing.ActiveButtons.Controls.SpillOverMode.Hide) Then
							For Each b As ActiveButton In Me.Items
                                If (b.Left + MyBase.Left - Me.theme.FrameBorder.Width + 2 >= Me.m_parentForm.Left) Then
                                    b.Visible = True
                                Else
                                    b.Visible = False
								End If
							Next
						End If
					End If
				Else
					MyBase.Visible = False
				End If
			End If
		End Sub

		Private Sub ParentFormActivated(ByVal sender As Object, ByVal e As EventArgs)
			Me.ToolTip.ShowAlways = True
		End Sub

		Private Sub ParentFormDeactivate(ByVal sender As Object, ByVal e As EventArgs)
			Me.ToolTip.ShowAlways = False
		End Sub

		Private Sub ParentFormDisposed(ByVal sender As Object, ByVal e As EventArgs)
			Dim form As System.Windows.Forms.Form = DirectCast(sender, System.Windows.Forms.Form)
			If (form IsNot Nothing) Then
				If (ActiveMenuImpl.parents.ContainsKey(form)) Then
					ActiveMenuImpl.parents.Remove(form)
				End If
			End If
		End Sub

		Protected Sub ParentRefresh(ByVal sender As Object, ByVal e As EventArgs)
            If (Me.m_parentForm.WindowState <> IWindowState.Minimized) Then
                Me.isActivated = True
                Me.OnPosition()
            Else
                Me.isActivated = False
				MyBase.Visible = False
			End If
		End Sub

		Private Sub TitleButtonSystemColorsChanged(ByVal sender As Object, ByVal e As EventArgs)
			Me.theme = Me.themeFactory.GetTheme()
			Me.CalcSize()
			Me.OnPosition()
		End Sub
	End Class
End Namespace