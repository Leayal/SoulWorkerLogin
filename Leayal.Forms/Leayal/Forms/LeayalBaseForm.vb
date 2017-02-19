Imports System.Drawing

Public MustInherit Class LeayalBaseForm
    Inherits System.Windows.Forms.Form

    Protected Const WM_NCHITTEST As UInt32 = &H84
    Protected Const WM_MOUSEMOVE As UInt32 = &H200
    Protected Const HTLEFT As UInt32 = 10
    Protected Const HTRIGHT As UInt32 = 11
    Protected Const HTBOTTOMRIGHT As UInt32 = 17
    Protected Const HTBOTTOM As UInt32 = 15
    Protected Const HTBOTTOMLEFT As UInt32 = 16
    Protected Const HTTOP As UInt32 = 12
    Protected Const HTTOPLEFT As UInt32 = 13
    Protected Const HTTOPRIGHT As UInt32 = 14
    Protected Const RESIZE_HANDLE_SIZE As Integer = 2

    Protected syncContext As System.Threading.SynchronizationContext

    Public Sub New()
        MyBase.New()
        Me.DoubleBuffered = True
        Me.syncContext = System.Threading.SynchronizationContext.Current
        Me.m_WindowState = IWindowState.Normal
        Me.m_HideToTaskbarBox = False
    End Sub

    Public Shadows Property ShowIcon() As Boolean
        Get
            Return MyBase.ShowIcon
        End Get
        Set(value As Boolean)
            If (MyBase.ShowIcon <> value) Then
                MyBase.ShowIcon = value
                OnShowIconChanged()
            End If
        End Set
    End Property

    Public Shadows Property Icon() As Icon
        Get
            Return MyBase.Icon
        End Get
        Set(value As Icon)
            If (MyBase.Icon Is Nothing) Then
                If (value IsNot Nothing) Then
                    MyBase.Icon = value
                    OnIconChanged()
                End If
            Else
                If (Not MyBase.Icon.Equals(value)) Then
                    MyBase.Icon = value
                    OnIconChanged()
                End If
            End If
        End Set
    End Property

    Private m_HideToTaskbarBox As Boolean
    Public Property AllowHideToTaskbar() As Boolean
        Get
            Return Me.m_HideToTaskbarBox
        End Get
        Set(value As Boolean)
            If (Me.m_HideToTaskbarBox <> value) Then
                Me.m_HideToTaskbarBox = value
                OnHideToTaskbarBoxChanged()
            End If
        End Set
    End Property

    Private m_WindowState As IWindowState
    Public Shadows Property WindowState() As IWindowState
        Get
            Return Me.m_WindowState
        End Get
        Set(value As IWindowState)
            If (Me.m_WindowState <> value) Then
                Me.ProcessWindowState(Me.m_WindowState, value)
                Me.OnWindowStateChanged(Me.m_WindowState, value)
                Me.m_WindowState = value
            End If
        End Set
    End Property

    Private Sub ProcessWindowState(ByVal OldWindowState As IWindowState, ByVal NewWindowState As IWindowState)
        Select Case (NewWindowState)
            Case IWindowState.Minimized
                If (OldWindowState = IWindowState.HiddenToTray) Then
                    Me.RiseFromTray()
                End If
                MyBase.WindowState = FormWindowState.Minimized
            Case IWindowState.Maximized
                If (OldWindowState = IWindowState.HiddenToTray) Then
                    Me.RiseFromTray()
                End If
                MyBase.WindowState = FormWindowState.Maximized
            Case IWindowState.HiddenToTray
                'MyBase.WindowState = FormWindowState.Normal
                Me.HideToTray()
            Case Else
                If (OldWindowState = IWindowState.HiddenToTray) Then
                    Me.RiseFromTray()
                End If
                Me.m_WindowState = IWindowState.Normal
                MyBase.WindowState = FormWindowState.Normal
        End Select
    End Sub

    Private LastKnownFormWindowState As FormWindowState

    Public Sub HideToTray()
        Me.m_WindowState = IWindowState.HiddenToTray
        Me.LastKnownFormWindowState = MyBase.WindowState
        Me.ShowInTaskbar = False
        Me.Visible = False
    End Sub

    Public Sub RiseFromTray()
        MyBase.WindowState = Me.LastKnownFormWindowState
        Me.Visible = True
        Me.ShowInTaskbar = True
    End Sub

#Region "Events"
    Protected Overridable Sub OnWindowStateChanged(ByVal OldWindowState As IWindowState, ByVal NewWindowState As IWindowState)
        If (Me.syncContext IsNot Nothing) Then
            Me.syncContext.Post(AddressOf RaiseEventWindowStateChanged, New WindowStateChangedEventArgs(OldWindowState, NewWindowState))
        End If
    End Sub
    Private Sub RaiseEventWindowStateChanged(ByVal e As Object)
        RaiseEvent WindowStateChanged(Me, DirectCast(e, WindowStateChangedEventArgs))
    End Sub
    Public Event WindowStateChanged As EventHandler(Of WindowStateChangedEventArgs)
    Public Class WindowStateChangedEventArgs
        Inherits EventArgs

        Public ReadOnly Property OldState() As IWindowState
        Public ReadOnly Property NewState() As IWindowState

        Public Sub New(ByVal OldWindowState As IWindowState, ByVal NewWindowState As IWindowState)
            MyBase.New()
            Me.OldState = OldWindowState
            Me.NewState = NewWindowState
        End Sub
    End Class

    Protected Overridable Sub OnShowIconChanged()
        If (Me.syncContext IsNot Nothing) Then
            Me.syncContext.Post(AddressOf RaiseEventShowIconChanged, System.EventArgs.Empty)
        End If
    End Sub
    Private Sub RaiseEventShowIconChanged(ByVal e As Object)
        RaiseEvent ShowIconChanged(Me, DirectCast(e, EventArgs))
    End Sub
    Public Event ShowIconChanged As EventHandler

    Protected Overridable Sub OnIconChanged()
        If (Me.syncContext IsNot Nothing) Then
            Me.syncContext.Post(AddressOf RaiseEventIconChanged, System.EventArgs.Empty)
        End If
    End Sub
    Private Sub RaiseEventIconChanged(ByVal e As Object)
        RaiseEvent IconChanged(Me, DirectCast(e, EventArgs))
    End Sub
    Public Event IconChanged As EventHandler

    Protected Overridable Sub OnTopMostChanged()
        If (Me.syncContext IsNot Nothing) Then
            Me.syncContext.Post(AddressOf RaiseEventTopMostChanged, System.EventArgs.Empty)
        End If
    End Sub
    Private Sub RaiseEventTopMostChanged(ByVal e As Object)
        RaiseEvent TopMostChanged(Me, DirectCast(e, EventArgs))
    End Sub
    Public Event TopMostChanged As EventHandler

    Protected Overridable Sub OnHideToTaskbarBoxChanged()
        If (Me.syncContext IsNot Nothing) Then
            Me.syncContext.Post(AddressOf RaiseEventHideToTaskbarBoxChanged, System.EventArgs.Empty)
        End If
    End Sub
    Private Sub RaiseEventHideToTaskbarBoxChanged(ByVal e As Object)
        RaiseEvent HideToTaskbarBoxChanged(Me, DirectCast(e, EventArgs))
    End Sub
    Public Event HideToTaskbarBoxChanged As EventHandler
#End Region
End Class

Public Enum IWindowState
    Normal = System.Windows.Forms.FormWindowState.Normal
    Minimized = System.Windows.Forms.FormWindowState.Minimized
    Maximized = System.Windows.Forms.FormWindowState.Maximized
    HiddenToTray = 3
End Enum