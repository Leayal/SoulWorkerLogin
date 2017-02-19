Imports System.Collections.Generic
Imports System.Drawing

Public Class ResizableBorderlessForm
    Inherits LeayalBaseForm

    Private WithEvents myPanel As TitleBar

    Public Sub New()
        MyBase.New()
        Me.myPanel = TitleBar.FromForm(Me)
        Me.myPanel.Location = New Point(4, 4)
        Me.myPanel.Size = New Size(Me.ClientSize.Width - 8, 24)
        Me.myPanel.Visible = False
        Me.myPanel.BackColor = Color.FromArgb(100, Color.Pink)
        Me.Controls.Add(Me.myPanel)
        Me.Padding = New Padding(3)
    End Sub

    Protected Overrides Sub OnDeactivate(e As EventArgs)
        If (Me.myPanel IsNot Nothing) Then Me.myPanel.Visible = False
        MyBase.OnDeactivate(e)
    End Sub

    Protected Overrides Sub OnLostFocus(e As EventArgs)
        If (Not Me.myPanel.Focused) Then
            Me.myPanel.Visible = False
        End If
        'on
        MyBase.OnLostFocus(e)
    End Sub

    Protected Overrides Sub OnTextChanged(e As EventArgs)
        If (Me.myPanel IsNot Nothing) Then Me.myPanel.Text = Me.Text
        MyBase.OnTextChanged(e)
    End Sub

    Protected Overrides Sub OnControlAdded(e As ControlEventArgs)
        AddHandler e.Control.MouseMove, AddressOf Control_MouseMove
        If (Me.myPanel IsNot Nothing) Then Me.myPanel.BringToFront()
        MyBase.OnControlAdded(e)
    End Sub

    Protected Overrides Sub OnControlRemoved(e As ControlEventArgs)
        RemoveHandler e.Control.MouseMove, AddressOf Control_MouseMove
        MyBase.OnControlRemoved(e)
    End Sub

    Friend Sub Control_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
        OnMouseMove(e)
    End Sub

    Protected Overrides Sub OnClientSizeChanged(e As EventArgs)
        If (Me.myPanel IsNot Nothing) Then
            Me.myPanel.Size = New Size(Me.ClientSize.Width - 8, Me.myPanel.Size.Height)
        End If
        MyBase.OnClientSizeChanged(e)
    End Sub

    Protected Overrides Sub OnIconChanged()
        If (Me.myPanel IsNot Nothing) Then Me.myPanel.Icon = Me.Icon
        MyBase.OnIconChanged()
    End Sub

    Public Function ToggleTopMost() As Boolean
        Me.TopMost = Not Me.TopMost
        Return Me.TopMost
    End Function

    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
        If (Me.myPanel IsNot Nothing) Then
            If (Me.myPanel.ClientRectangle.Contains(e.Location)) Then
                Me.myPanel.Visible = True
            Else
                Me.myPanel.Visible = False
            End If
        End If
        MyBase.OnMouseMove(e)
    End Sub

    Protected Overrides Sub WndProc(ByRef m As Message)
        Dim handled As Boolean = False
        Dim screenPoint As Point = Point.Empty
        If m.Msg = WM_NCHITTEST OrElse m.Msg = WM_MOUSEMOVE Then
            Dim formSize As Size = Me.Size
            screenPoint = New Point(m.LParam.ToInt32())
            Dim clientPoint As Point = Me.PointToClient(screenPoint)

            Dim boxes As New Dictionary(Of UInt32, Rectangle)() From {
                {HTBOTTOMLEFT, New Rectangle(0, formSize.Height - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE)},
                {HTBOTTOM, New Rectangle(RESIZE_HANDLE_SIZE, formSize.Height - RESIZE_HANDLE_SIZE, formSize.Width - 2 * RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE)},
                {HTBOTTOMRIGHT, New Rectangle(formSize.Width - RESIZE_HANDLE_SIZE, formSize.Height - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE)},
                {HTRIGHT, New Rectangle(formSize.Width - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, formSize.Height - 2 * RESIZE_HANDLE_SIZE)},
                {HTTOPRIGHT, New Rectangle(formSize.Width - RESIZE_HANDLE_SIZE, 0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE)},
                {HTTOP, New Rectangle(RESIZE_HANDLE_SIZE, 0, formSize.Width - 2 * RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE)},
                {HTTOPLEFT, New Rectangle(0, 0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE)},
                {HTLEFT, New Rectangle(0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, formSize.Height - 2 * RESIZE_HANDLE_SIZE)}
            }

            For Each hitBox As KeyValuePair(Of UInt32, Rectangle) In boxes
                If hitBox.Value.Contains(clientPoint) Then
                    m.Result = New IntPtr(hitBox.Key)
                    handled = True
                    Exit For
                End If
            Next
        End If
        'MyBase.WndProc(m)
        If Not handled Then
            MyBase.WndProc(m)
        Else
            OnMouseMove(New MouseEventArgs(MouseButtons.None, 0, screenPoint.X, screenPoint.Y, 0))
        End If
    End Sub
End Class
