Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Public Class TitleBar
    Inherits System.Windows.Forms.Panel

    Private WithEvents myCloseButton As TitleBarButton
    Private WithEvents myMaximizeButton As TitleBarButton
    Private WithEvents myMinimizeButton As TitleBarButton
    Private WithEvents myMinimizeToTrayButton As TitleBarButton
    Private WithEvents myParent As LeayalBaseForm
    Private WithEvents myPictureBox As PictureBox
    Private WithEvents myControlsBox As FlowLayoutPanel
    Dim iconRect As Rectangle
    Dim textRect As Rectangle

    Dim drag As Boolean
    Dim mousex As Integer
    Dim mousey As Integer

    Public Property TitleFont() As Font
    Private _TextColor As Color
    Public Property TextColor() As Color
        Get
            Return Me._TextColor
        End Get
        Set(value As Color)
            Me._TextColor = value
            Me.RefreshIconAndTitle()
            Me.changeButtonTextColor()
        End Set
    End Property
    Private _TitleColor As Color
    Public Property TitleColor() As Color
        Get
            Return Me._TitleColor
        End Get
        Set(value As Color)
            Me._TitleColor = value
            If (value = Me.BackColor) Then
                If (Me.myPictureBox IsNot Nothing) Then
                    Me.Controls.Remove(Me.myPictureBox)
                    Me.myPictureBox.Dispose()
                    Me.myPictureBox = Nothing
                End If
            Else
                If (Me.myPictureBox Is Nothing) Then
                    Me.myPictureBox = New PictureBox()
                    Me.myPictureBox.BackColor = Color.Transparent

                End If
                Dim asdasdad As Image
                If (Me.myPictureBox.Image IsNot Nothing) Then
                    asdasdad = Me.myPictureBox.Image
                    Me.myPictureBox.Image = Nothing
                    asdasdad.Dispose()
                End If
                Me.myPictureBox.SizeMode = PictureBoxSizeMode.StretchImage
                asdasdad = New Bitmap(10, 10, PixelFormat.Format32bppArgb)
                Using gr As Graphics = Graphics.FromImage(asdasdad)
                    gr.Clear(value)
                End Using
                Me.myPictureBox.Image = ChangeOpacity(asdasdad, 0.5)
                Me.Controls.Add(Me.myPictureBox)
                Me.myPictureBox.Size = Me.Size
                Me.myPictureBox.Location = New Point(0, 0)
                Me.myPictureBox.SendToBack()
                asdasdad = Nothing
            End If
        End Set
    End Property
    Public Property TextBGColor() As Color
    'Public Shadows Property Text() As String
    '    Get
    '        Return MyBase.Text
    '    End Get
    '    Set(value As String)
    '        MyBase.Text = value
    '        Me.OnTextChanged(System.EventArgs.Empty)
    '    End Set
    'End Property

    Public Shadows ReadOnly Property Focused() As Boolean
        Get
            Return (MyBase.Focused Or
                If(Me.myCloseButton Is Nothing, False, Me.myCloseButton.Focused) Or
                If(Me.myMaximizeButton Is Nothing, False, Me.myMaximizeButton.Focused) Or
                If(Me.myMinimizeButton Is Nothing, False, Me.myMinimizeButton.Focused) Or
                If(Me.myMinimizeToTrayButton Is Nothing, False, Me.myMinimizeToTrayButton.Focused) Or
                If(Me.myPictureBox Is Nothing, False, Me.myPictureBox.Focused))
        End Get
    End Property

    Private m_Icon As Icon
    Public Property Icon() As Icon
        Get
            Return Me.m_Icon
        End Get
        Set(value As Icon)
            If (Me.m_Icon IsNot Nothing) Then
                If (Not Me.m_Icon.Equals(value)) Then
                    Me.m_Icon = value
                    OnIconChanged((value Is Nothing))
                End If
            Else
                If value IsNot Nothing Then
                    Me.m_Icon = value
                    OnIconChanged((value Is Nothing))
                End If
            End If
        End Set
    End Property

    Protected Overridable Sub OnIconChanged(ByVal value As Boolean)
        If (value) Then
            Dim newRegion As New Region(Me.iconRect)
            newRegion.Complement(Me.textRect)
            Me.Invalidate(newRegion, True)
        Else
            Me.Invalidate(Me.iconRect, True)
        End If
    End Sub

    Public Shared Function FromForm(ByVal target As Form) As TitleBar
        Return FromForm(target, False)
    End Function

    Protected Overrides Sub OnTextChanged(e As EventArgs)
        Me.calculateTextRect()
        Me.Invalidate(Me.textRect, True)
        MyBase.OnTextChanged(e)
    End Sub

    Private Sub calculateTextRect()
        Dim asizeF = TextRenderer.MeasureText(Me.Text, Me.TitleFont)
        If (m_Icon IsNot Nothing) Then

        End If
        Dim sss = If(Me.myControlsBox IsNot Nothing, Me.myControlsBox.Location.X, Me.ClientSize.Width - 23)
        If (sss < 24) Then
            sss = 0
        Else
            sss = sss - 23
        End If
        If (sss > asizeF.Width) Then
            Me.textRect = New Rectangle(23, 5, sss, asizeF.Height)
        Else
            Me.textRect = New Rectangle(23, 5, asizeF.Width, asizeF.Height)
        End If
        'Dim aRectangle As New Rectangle(Me.textRect.X, Me.textRect.Y, Convert.ToInt32(asizeF.Width), Convert.ToInt32(asizeF.Height))
    End Sub

    Public Shared Function FromForm(ByVal target As Form, ByVal minToTray As Boolean) As TitleBar
        If (target.ShowIcon) Then
            Return New TitleBar(target.Icon, target.Text, target.ControlBox, target.MaximizeBox, target.MinimizeBox, minToTray)
        Else
            Return New TitleBar(Nothing, target.Text, target.ControlBox, target.MaximizeBox, target.MinimizeBox, minToTray)
        End If
    End Function

    Public Sub New()
        Me.New(Nothing, String.Empty, True, True, True, False)
    End Sub

    Public Sub New(ByVal ico As Icon, ByVal title As String, ByVal closeButton As Boolean, ByVal maxButton As Boolean, ByVal minButton As Boolean, ByVal minTrayButton As Boolean)
        MyBase.New()
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.iconRect = New Rectangle(2, 2, 20, 20)
        Me.textRect = New Rectangle(23, 5, 50, Me.FontHeight)
        Me.Size = New Size(10, 26)
        Me.Text = title
        Me.TextBGColor = Color.Transparent
        Me.TextColor = Color.Black
        Me.TitleFont = New Font(Me.Font, FontStyle.Bold)
        Me.myControlsBox = New FlowLayoutPanel()
        AddHandler Me.myControlsBox.ClientSizeChanged, AddressOf myControlsBox_ClientSizeChanged
        Me.myControlsBox.AutoSize = True
        Me.myControlsBox.AutoSizeMode = AutoSizeMode.GrowAndShrink
        Me.myControlsBox.RightToLeft = RightToLeft.Yes
        Me.Controls.Add(Me.myControlsBox)
        If (ico IsNot Nothing) Then
            Me.Icon = ico
        Else
            Me.Icon = Nothing
        End If
        Me.myCloseButton = CreateButton("✕")
        Me.AddButon(Me.myCloseButton, 0, closeButton)
        Me.myMaximizeButton = CreateButton("🗖")
        Me.AddButon(Me.myMaximizeButton, 1, maxButton)
        Me.myMinimizeButton = CreateButton("_")
        Me.AddButon(Me.myMinimizeButton, 2, minButton)
        Me.myMinimizeToTrayButton = CreateButton("●")
        Me.m_ShowHideToTrayBox = minTrayButton
        Me.AddButon(Me.myMinimizeToTrayButton, 3, minTrayButton)
        Me.BackColor = Color.Transparent
    End Sub

    Private Sub myControlsBox_ClientSizeChanged(sender As Object, e As EventArgs)
        Me.myControlsBox.Location = New Point(Me.ClientSize.Width - Me.myControlsBox.ClientSize.Width, 0)
        Me.calculateTextRect()
    End Sub

    Protected Overrides Sub OnDoubleClick(e As EventArgs)
        If (lastMouseButtons = MouseButtons.Left) Then
            If (Me.myMaximizeButton.Visible) Then
                Me.myMaximizeButton.PerformClick()
            End If
            MyBase.OnDoubleClick(e)
        End If
    End Sub

    Dim lastMouseButtons As MouseButtons

    Protected Overrides Sub OnMouseClick(e As MouseEventArgs)
        Select Case (e.Button)
            Case MouseButtons.Left
                lastMouseButtons = MouseButtons.Left
            Case MouseButtons.Right
                lastMouseButtons = MouseButtons.Right
            Case Else
                lastMouseButtons = MouseButtons.None
                MyBase.OnMouseClick(e)
        End Select
    End Sub

    Private Sub AddButon(ByVal target As ButtonBase, ByVal index As Integer, Optional ByVal visible As Boolean = True)
        target.Visible = visible
        Me.myControlsBox.Controls.Add(target)
        If (Me.myControlsBox.Controls.Count < index) Then
        Else
            Me.myControlsBox.Controls.SetChildIndex(target, index)
        End If
    End Sub

    Private Sub AdjustButtons2()
        Dim theW As Integer
        Dim i As Integer = 0
        For Each contr As Control In Me.myControlsBox.Controls
            If (contr.GetType().Name = GetType(Button).Name) Then
                i += 1
                theW = i * 23
                contr.Location = New Point(Me.Size.Width - theW, 1)
            End If
        Next
    End Sub

    Protected Overrides Sub OnSizeChanged(e As EventArgs)
        If (Me.myPictureBox IsNot Nothing) Then
            Me.myPictureBox.Size = Me.Size
        End If
        If (Me.myControlsBox IsNot Nothing) Then
            Me.myControlsBox.Location = New Point(Me.ClientSize.Width - Me.myControlsBox.ClientSize.Width, 0)
            Me.calculateTextRect()
        End If
        MyBase.OnSizeChanged(e)
    End Sub

    Protected Overrides Sub OnFontChanged(e As EventArgs)
        Me.calculateTextRect()
        MyBase.OnFontChanged(e)
    End Sub

    Private Function CreateButton(ByVal text As String) As Button
        CreateButton = New TitleBarButton()
        CreateButton.Font = New Font(CreateButton.Font, FontStyle.Bold)
        CreateButton.Margin = New Padding(0)
        CreateButton.Text = text
        CreateButton.FlatStyle = FlatStyle.Flat
        CreateButton.FlatAppearance.BorderSize = 0
        CreateButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(170, Color.Gray)
        CreateButton.FlatAppearance.MouseDownBackColor = Color.Gray
        CreateButton.ForeColor = Me.TextColor
        CreateButton.TabStop = False
        CreateButton.Size = New Size(23, 23)
        CreateButton.BackColor = Color.Transparent
        'CreateButton.UseCompatibleTextRendering = True
        'CreateButton.Font = New Font("Marlett", CreateButton.Font.Style)
        CreateButton.Anchor = AnchorStyles.Top And AnchorStyles.Right
    End Function

    Private Sub changeButtonTextColor()
        If (Me.myControlsBox IsNot Nothing) Then
            For Each but As Control In Me.myControlsBox.Controls
                but.ForeColor = Me.TextColor
            Next
        End If
    End Sub

    Private Sub myCloseButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles myCloseButton.Click
        If (Me.myParent IsNot Nothing) Then
            Me.myParent.Close()
        End If
    End Sub

    Private Sub myMinimizeButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles myMinimizeButton.Click
        If (Me.myParent IsNot Nothing) Then
            Me.myParent.WindowState = IWindowState.Minimized
        End If
    End Sub

    Private Sub myMaximizeButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles myMaximizeButton.Click
        If (Me.myParent IsNot Nothing) Then
            If (Me.myParent.WindowState = IWindowState.Maximized) Then
                Me.myParent.WindowState = IWindowState.Normal
            Else
                Me.myParent.WindowState = IWindowState.Maximized
            End If
        End If
    End Sub

    Private Sub myMinimizeToTrayButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles myMinimizeToTrayButton.Click
        If (Me.myParent IsNot Nothing) Then
            Me.myParent.WindowState = IWindowState.HiddenToTray
            'Me.myParent.WindowState 
        End If
    End Sub

    'Private Sub myParent_Shown(ByVal sender As Object, ByVal e As EventArgs) Handles myParent.Shown
    '    If (Me.myParent.WindowState <> FormWindowState.Minimized) Then
    '        Me.myParent.ShowInTaskbar = True
    '    End If
    'End Sub

    Public Shadows Sub Dispose()
        MyBase.Dispose()
    End Sub

    Private Sub Make_ControlBackground(ByVal Target As Control, OpacityValue As Single)
        Target.BackColor = System.Drawing.Color.FromArgb(Convert.ToInt32(OpacityValue * 100), 255, 255, 255)
    End Sub

    Private Sub myPictureBox_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles myPictureBox.Paint
        Me.DrawIconAndTitle(e)
    End Sub

    Private Sub DrawIconAndTitle(ByVal e As PaintEventArgs)
        If (e.ClipRectangle.Contains(Me.iconRect)) Then
            If (Me.Icon IsNot Nothing) Then
                e.Graphics.DrawIcon(Me.Icon, Me.iconRect)
            End If
        End If
        If (Not Me.textRect.IsEmpty) AndAlso (e.ClipRectangle.Contains(Me.textRect)) Then
            If (Not String.IsNullOrWhiteSpace(Me.Text)) Then
                Dim asdasd = e.Graphics.BeginContainer()
                e.Graphics.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit
                If (Me.TextBGColor <> Color.Transparent) Then
                    Using penasd = New SolidBrush(Me.TextBGColor)
                        e.Graphics.FillRectangle(penasd, textRect)
                    End Using
                End If
                TextRenderer.DrawText(e.Graphics, Me.Text, Me.TitleFont, Me.textRect, Me.TextColor, Me.BackColor, TextFormatFlags.Default)
                e.Graphics.EndContainer(asdasd)
            End If
        End If
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        If (Me.myPictureBox Is Nothing) Then
            Me.DrawIconAndTitle(e)
        End If
        MyBase.OnPaint(e)
    End Sub

    Public Sub RefreshIconAndTitle()
        If (Me.myPictureBox Is Nothing) Then
            Me.Invalidate(Me.textRect)
        Else
            Me.myPictureBox.Invalidate(Me.textRect)
        End If
    End Sub

    Shared Function ChangeOpacity(ByVal img As Image, ByVal opacityvalue As Single) As Bitmap
        Dim bmp As New Bitmap(img.Width, img.Height)
        ' Determining Width and Height of Source Image
        Using graphics__1 As Graphics = Graphics.FromImage(bmp)
            Dim colormatrix As New System.Drawing.Imaging.ColorMatrix()
            colormatrix.Matrix33 = opacityvalue
            Dim imgAttribute As New System.Drawing.Imaging.ImageAttributes()
            imgAttribute.SetColorMatrix(colormatrix, System.Drawing.Imaging.ColorMatrixFlag.[Default], System.Drawing.Imaging.ColorAdjustType.Bitmap)
            graphics__1.DrawImage(img, New Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, img.Width, img.Height,
                GraphicsUnit.Pixel, imgAttribute)
        End Using
        ' Releasing all resource used by graphics 
        Return bmp
    End Function

    Private Sub myPictureBox_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles myPictureBox.MouseDown
        OnMouseDown(e)
    End Sub

    Private Sub myPictureBox_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles myPictureBox.MouseMove
        OnMouseMove(e)
    End Sub

    Private Sub myPictureBox_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles myPictureBox.MouseUp
        OnMouseUp(e)
    End Sub

    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        If (Me.myParent IsNot Nothing) Then
            If e.Button = Windows.Forms.MouseButtons.Left Then
                drag = True
                mousex = Windows.Forms.Cursor.Position.X - Me.myParent.Left
                mousey = Windows.Forms.Cursor.Position.Y - Me.myParent.Top
            End If
        End If
        MyBase.OnMouseDown(e)
    End Sub
    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
        If (Me.myParent IsNot Nothing) Then
            If drag Then
                Me.myParent.Top = Windows.Forms.Cursor.Position.Y - mousey
                Me.myParent.Left = Windows.Forms.Cursor.Position.X - mousex
            End If
        End If
        MyBase.OnMouseMove(e)
    End Sub

    Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
        If e.Button = Windows.Forms.MouseButtons.Left Then
            drag = False
        End If
        MyBase.OnMouseUp(e)
    End Sub

    Protected Overrides Sub OnParentChanged(e As EventArgs)
        Me.myParent = DirectCast(Me.FindForm(), LeayalBaseForm)
        Me.Size = New Size(Me.myParent.Size.Width, 26)
        MyBase.OnParentChanged(e)
    End Sub

    Private m_ShowHideToTrayBox As Boolean
    Public Property ShowHideToTrayBox() As Boolean
        Get
            Return Me.m_ShowHideToTrayBox
        End Get
        Set(value As Boolean)
            Me.m_ShowHideToTrayBox = value
            Me.OnShowHideToTrayBoxChanged()
        End Set
    End Property

    Protected Overridable Sub OnShowHideToTrayBoxChanged()
        Me.myMinimizeToTrayButton.Visible = Me.m_ShowHideToTrayBox
    End Sub
End Class
