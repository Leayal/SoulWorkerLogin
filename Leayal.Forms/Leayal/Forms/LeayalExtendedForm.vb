Imports System.Collections.Generic
Imports System.Drawing
Imports Leayal.Forms

Public Class LeayalExtendedForm
    Inherits LeayalBaseForm

    Private myTitleBar As TitleBar
    Private myContentPanel As Panel
    Private myTitleBarMenu As TheCodeKing.ActiveButtons.Controls.IActiveMenu
    Private myHideToTrayButton As TheCodeKing.ActiveButtons.Controls.ActiveButton

    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        MyBase.OnPaintBackground(e)
        If (AeroControl.IsWin10) Then
            Using penn = New Pen(AeroControl.ThemeInfo.ThemeColor)
                e.Graphics.DrawRectangle(penn, Me.borderRect)
            End Using
        End If
    End Sub

    Protected Overrides Sub OnWindowStateChanged(OldWindowState As IWindowState, NewWindowState As IWindowState)
        If (Me.WindowState <> IWindowState.HiddenToTray) Then
            'Me.RiseFromTray()
            If (AeroControl.IsWin10) Then
                AeroControl.EnableBlur(Me, True)
            End If
        End If
        MyBase.OnWindowStateChanged(OldWindowState, NewWindowState)
    End Sub

    Protected Overrides Sub OnTextChanged(e As EventArgs)
        If (AeroControl.IsWin10) Then
            If (Me.myTitleBar IsNot Nothing) Then Me.myTitleBar.Text = Me.Text
        End If
        MyBase.OnTextChanged(e)
    End Sub

    Protected Overrides Sub OnShowIconChanged()
        If (Me.ShowIcon) Then
            Me.myTitleBar.Icon = Me.Icon
        Else
            Me.myTitleBar.Icon = Nothing
        End If
        MyBase.OnShowIconChanged()
    End Sub

    Private Sub Me_SystemColorsChanged(sender As Object, e As EventArgs)
        If (AeroControl.IsWin10) Then
            If (Me.myTitleBar IsNot Nothing) Then
                Me.myTitleBar.TextColor = AeroControl.ThemeInfo.ThemeColor
                If Not AeroControl.ThemeInfo.Theme Then
                    Me.myTitleBar.BackColor = Color.FromArgb(50, Color.IndianRed)
                End If
            End If
            Me.Invalidate(borderRegion, True)
        End If
    End Sub

    Dim borderRegion As Region
    Dim borderRect As Rectangle

    Protected Overrides Sub OnSizeChanged(e As EventArgs)
        If (AeroControl.IsWin10) Then
            Me.getBoxesSizes()
            Me.borderRect = Me.ClientRectangle
            Me.borderRect.Inflate(-1, -1)
            Me.borderRegion = New Region(Me.ClientRectangle)
            Dim rect = Me.ClientRectangle
            rect.Offset(1, 1)
            rect.Inflate(-2, -2)
            borderRegion.Exclude(rect)
            If (Me.myContentPanel IsNot Nothing) Then Me.myContentPanel.Size = New Size(Me.borderRect.Size.Width - 1, Me.borderRect.Size.Height - If(Me.myTitleBar IsNot Nothing, Me.myTitleBar.Size.Height, 0))
        End If
        MyBase.OnSizeChanged(e)
    End Sub

    Private Sub getBoxesSizes()
        If (boxes Is Nothing) Then
            boxes = New Dictionary(Of UInteger, Rectangle)(8)
        End If
        Dim formSize As Size = Me.Size
        boxes.Clear()
        boxes.Add(HTBOTTOMLEFT, New Rectangle(0, formSize.Height - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE))
        boxes.Add(HTBOTTOM, New Rectangle(RESIZE_HANDLE_SIZE, formSize.Height - RESIZE_HANDLE_SIZE, formSize.Width - 2 * RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE))
        boxes.Add(HTBOTTOMRIGHT, New Rectangle(formSize.Width - RESIZE_HANDLE_SIZE, formSize.Height - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE))
        boxes.Add(HTRIGHT, New Rectangle(formSize.Width - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, formSize.Height - 2 * RESIZE_HANDLE_SIZE))
        boxes.Add(HTTOPRIGHT, New Rectangle(formSize.Width - RESIZE_HANDLE_SIZE, 0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE))
        boxes.Add(HTTOP, New Rectangle(RESIZE_HANDLE_SIZE, 0, formSize.Width - 2 * RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE))
        boxes.Add(HTTOPLEFT, New Rectangle(0, 0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE))
        boxes.Add(HTLEFT, New Rectangle(0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, formSize.Height - 2 * RESIZE_HANDLE_SIZE))
    End Sub

    Private boxes As Dictionary(Of UInt32, Rectangle)

    Public Sub New()
        MyBase.New()
        Me.Padding = New Padding(2)
        Me.m_FormBorderStyle = MyBase.FormBorderStyle
        If (Not DesignMode) Then
            If (AeroControl.IsWin10) Then
                Me.getBoxesSizes()
                MyBase.FormBorderStyle = FormBorderStyle.None
            End If
        End If
    End Sub

    Private m_FormBorderStyle As FormBorderStyle
    Public Shadows Property FormBorderStyle() As FormBorderStyle
        Get
            If ((Not DesignMode) AndAlso AeroControl.IsWin10) Then
                Return Me.m_FormBorderStyle
            Else
                Return MyBase.FormBorderStyle
            End If
        End Get
        Set(value As FormBorderStyle)
            If ((Not DesignMode) AndAlso AeroControl.IsWin10) Then
                Me.m_FormBorderStyle = value
            Else
                MyBase.FormBorderStyle = value
            End If
        End Set
    End Property

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        If (Not DesignMode) Then
            If (AeroControl.IsWin10) Then
                Me.myTitleBar = TitleBar.FromForm(Me, Me.AllowHideToTaskbar)
                Me.myTitleBar.Dock = DockStyle.Top
                Me.myTitleBar.TextColor = AeroControl.ThemeInfo.ThemeColor
                If Not AeroControl.ThemeInfo.Theme Then
                    Me.myTitleBar.BackColor = Color.IndianRed
                End If
                Me.myContentPanel = New Panel()
                Me.myContentPanel.BackColor = Me.BackColor
                Me.myContentPanel.Location = New Point(2, Me.myTitleBar.Height + 1)
                Me.myContentPanel.Size = New Size(Me.Size.Width - 0, Me.Size.Height - 2)
                While (Me.Controls.Count > 0)
                    Me.myContentPanel.Controls.Add(Me.Controls(0))
                End While
                Me.Controls.Add(Me.myContentPanel)
                Me.Controls.Add(Me.myTitleBar)
                Me.Size = New Size(Me.Size.Width, Me.Size.Height + Me.myTitleBar.Height)
            Else
                If (Me.AllowHideToTaskbar) Then
                    Me.myTitleBarMenu = TheCodeKing.ActiveButtons.Controls.GetInstance(Me)
                    Me.myHideToTrayButton = Me.CreateHideToTrayButton()
                    Me.myTitleBarMenu.Items.Add(Me.myHideToTrayButton)
                End If
            End If
        End If
    End Sub

    Protected Overrides Sub OnHideToTaskbarBoxChanged()
        MyBase.OnHideToTaskbarBoxChanged()
        If (Not DesignMode) Then
            If (AeroControl.IsWin10) Then
                If (Me.myTitleBar IsNot Nothing) Then Me.myTitleBar.ShowHideToTrayBox = Me.AllowHideToTaskbar
            Else
                If (Me.AllowHideToTaskbar) Then
                    If (Me.myTitleBarMenu IsNot Nothing) Then
                        Me.myTitleBarMenu.Items.Add(Me.myHideToTrayButton)
                    End If
                Else
                    If (Me.myTitleBarMenu IsNot Nothing) Then
                        Me.myTitleBarMenu.Items.Remove(Me.myHideToTrayButton)
                    End If
                End If
            End If
        End If
    End Sub

    Private Function CreateHideToTrayButton() As TheCodeKing.ActiveButtons.Controls.ActiveButton
        CreateHideToTrayButton = New TheCodeKing.ActiveButtons.Controls.ActiveButton()
        CreateHideToTrayButton.Text = "●"
        AddHandler CreateHideToTrayButton.Click, AddressOf HideToTray
        'CreateButton.BackColor = Color.Transparent
        'CreateButton.FlatStyle = FlatStyle.Flat
        'CreateButton.FlatAppearance.BorderSize = 0
        'CreateButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(170, Color.Gray)
        'CreateButton.FlatAppearance.MouseDownBackColor = Color.Gray
        'CreateHideToTrayButton.TabStop = False
    End Function

    Dim changingBG As Boolean = False

    Protected Overrides Sub OnBackColorChanged(e As EventArgs)
        If (Not DesignMode) Then
            If (Not changingBG) Then
                changingBG = True
                If (AeroControl.IsWin10) Then
                    If (Me.myContentPanel IsNot Nothing) Then
                        If (Not changingBG) Then
                            Me.myContentPanel.BackColor = Me.BackColor
                        End If
                    End If
                    AeroControl.EnableBlur(Me)
                End If
            End If
        End If
        changingBG = False
        MyBase.OnBackColorChanged(e)
    End Sub

    Protected Overrides Sub OnShown(e As EventArgs)
        If (Not DesignMode) Then
            If (AeroControl.IsWin10) Then AeroControl.EnableBlur(Me, False)
        End If
        MyBase.OnShown(e)
    End Sub

    Private Function DockToAnchor(ByVal dock As DockStyle) As AnchorStyles
        Select Case (dock)
            Case DockStyle.Bottom
                DockToAnchor = AnchorStyles.Left And AnchorStyles.Bottom And AnchorStyles.Right
            Case DockStyle.Top
                DockToAnchor = AnchorStyles.Left And AnchorStyles.Top And AnchorStyles.Right
            Case DockStyle.Left
                DockToAnchor = AnchorStyles.Top And AnchorStyles.Left And AnchorStyles.Bottom
            Case DockStyle.Right
                DockToAnchor = AnchorStyles.Top And AnchorStyles.Right And AnchorStyles.Bottom
            Case DockStyle.Fill
                DockToAnchor = AnchorStyles.Top And AnchorStyles.Right And AnchorStyles.Bottom And AnchorStyles.Left
            Case Else
                DockToAnchor = AnchorStyles.None
        End Select
    End Function

    Private Function IsResizable() As Boolean
        Select Case (Me.m_FormBorderStyle)
            Case FormBorderStyle.Fixed3D
                Return False
            Case FormBorderStyle.FixedDialog
                Return False
            Case FormBorderStyle.FixedSingle
                Return False
            Case FormBorderStyle.FixedToolWindow
                Return False
            Case FormBorderStyle.None
                Return False
            Case Else
                Return True
        End Select
    End Function

    Protected Overrides Sub WndProc(ByRef m As Message)
        If (AeroControl.IsWin10) AndAlso (Me.IsResizable()) AndAlso (Me.WindowState <> IWindowState.Maximized) Then
            Dim handled As Boolean = False
            If m.Msg = WM_NCHITTEST OrElse m.Msg = WM_MOUSEMOVE Then
                Dim screenPoint As New Point(m.LParam.ToInt32())
                Dim clientPoint As Point = Me.PointToClient(screenPoint)

                For Each hitBox As KeyValuePair(Of UInt32, Rectangle) In boxes
                    If hitBox.Value.Contains(clientPoint) Then
                        m.Result = New IntPtr(hitBox.Key)
                        handled = True
                        Exit For
                    End If
                Next
            End If

            If Not handled Then
                MyBase.WndProc(m)
            End If
        Else
            MyBase.WndProc(m)
        End If
    End Sub
End Class
