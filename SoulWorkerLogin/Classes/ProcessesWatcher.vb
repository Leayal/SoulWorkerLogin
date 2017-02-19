Imports System.Management
Imports System.IO

Namespace Classes
    Public Class ProcessesWatcher
        Implements IDisposable

        Private Shared _instance As ProcessesWatcher
        Public Shared ReadOnly Property Instance() As ProcessesWatcher
            Get
                If (_instance Is Nothing) Then
                    _instance = New ProcessesWatcher()
                End If
                Return _instance
            End Get
        End Property

        Public Shared Function GetWatcher(ByVal processPath As String) As ProcessWatcher
            Return Instance.GetWatcherEx(processPath)
        End Function

        Private myDict As Dictionary(Of String, ProcessWatcher)
        Public Sub New()
            Me.myDict = New Dictionary(Of String, ProcessWatcher)()
        End Sub

        Public Function GetWatcherEx(ByVal processPath As String) As ProcessWatcher
            GetWatcherEx = Nothing
            Dim lowerOne As String = processPath.ToLower()
            If (Me.myDict.Count > 0) Then
                If (Me.myDict.ContainsKey(lowerOne)) Then
                    GetWatcherEx = Me.myDict(lowerOne)
                End If
            End If
            If (GetWatcherEx Is Nothing) Then
                GetWatcherEx = New ProcessWatcher(processPath)
                Me.myDict.Add(lowerOne, GetWatcherEx)
            End If
        End Function

        Public Sub RemoveWatcherEx(ByVal processPath As String)
            Dim lowerOne As String = processPath.ToLower()
            If (Me.myDict.Count > 0) Then
                If (Me.myDict.ContainsKey(lowerOne)) Then
                    Me.myDict(lowerOne).Dispose()
                    Me.myDict.Remove(lowerOne)
                End If
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            For Each asd In myDict.Values
                asd.Dispose()
            Next
            Me.myDict.Clear()
            Me.myDict = Nothing
        End Sub
    End Class

    Public Class ProcessWatcher
        Implements IDisposable
        Public ReadOnly Property IsRunning() As Boolean
            Get
                If (Me.ProcessInstance Is Nothing) Then
                    Return False
                Else
                    Return (Not Me.ProcessInstance.HasExited)
                End If
            End Get
        End Property
        Private WithEvents _ProcessInstance As Process
        Public ReadOnly Property ProcessInstance() As Process
            Get
                Return Me._ProcessInstance
            End Get
        End Property
        Private _ProcessPath As String
        Public ReadOnly Property ProcessPath() As String
            Get
                Return Me._ProcessPath
            End Get
        End Property

        Private WithEvents processStartEvent As ManagementEventWatcher

        Public Sub New(ByVal processPath As String)
            Me._ProcessPath = processPath
            Dim myprocess As Process = FindProcess(processPath)
            Me.processStartEvent = New ManagementEventWatcher("SELECT ProcessID FROM Win32_ProcessStartTrace WHERE ProcessName = '" & Path.GetFileName(Me.ProcessPath) & "'")
            If (myprocess IsNot Nothing) Then
                Me.SetProcess(myprocess)
                Me.processStartEvent.Stop()
            Else
                Me._ProcessInstance = Nothing
                Me.processStartEvent.Start()
            End If
        End Sub

        Private Function FindProcess(ByVal str As String) As Process
            FindProcess = Nothing
            Dim myList() As Process = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(str))
            If (myList IsNot Nothing) AndAlso (myList.Length > 0) Then
                Dim myLowerName As String = Me.ProcessPath.ToLower()
                For i As Integer = 0 To myList.Length - 1
                    If (CommonMethods.GetExecutablePath(myList(i)).ToLower() = myLowerName) Then
                        FindProcess = myList(i)
                    Else
                        myList(i).Close()
                    End If
                Next
                myList = Nothing
            End If
        End Function


        Public Sub SetPath(ByVal str As String)
            Dim asdasdasd As String = Path.GetFileName(str)

            If (Path.GetFileName(Me.ProcessPath).ToLower() <> asdasdasd) Then
                If (Me.processStartEvent IsNot Nothing) Then
                    Me.processStartEvent.Stop()
                    Me.processStartEvent.Dispose()
                End If
                Me.processStartEvent = New ManagementEventWatcher("SELECT ProcessID FROM Win32_ProcessStartTrace WHERE ProcessName = '" & asdasdasd & "'")
            End If

            Dim myprocess As Process = FindProcess(str)
            Me._ProcessPath = str
            If (myprocess IsNot Nothing) Then
                Me.SetProcess(myprocess)
                Me.processStartEvent.Stop()
            Else
                Me._ProcessInstance = Nothing
                Me.processStartEvent.Start()
            End If
        End Sub

        Private Sub SetProcess(ByVal myprocess As Process)
            Me._ProcessInstance = myprocess
            Me._ProcessInstance.EnableRaisingEvents = True
            Me.RaiseEvent_ProcessLaunched()
        End Sub

        Private Sub processStartEvent_EventArrived(ByVal sender As Object, ByVal e As EventArrivedEventArgs) Handles processStartEvent.EventArrived
            Try
                Dim myProcID As Integer = Convert.ToInt32(e.NewEvent.Properties("ProcessID").Value)
                Dim pathString As String = CommonMethods.GetExecutablePath(myProcID)
                If Not String.IsNullOrEmpty(pathString) Then
                    If (pathString.ToLower() = Me.ProcessPath.ToLower()) Then
                        Me.SetProcess(Process.GetProcessById(myProcID))
                        Me.processStartEvent.Stop()
                    End If
                End If
            Catch ex As ArgumentException
                If (Me._ProcessInstance IsNot Nothing) Then Me._ProcessInstance.Close()
                Me._ProcessInstance = Nothing
                Log.LogManager.GeneralLog.QueueLog(ex)
            End Try
        End Sub

        Private Sub _ProcessInstance_Exited(ByVal sender As Object, ByVal e As EventArgs) Handles _ProcessInstance.Exited
            If (Me._ProcessInstance IsNot Nothing) Then Me._ProcessInstance.Close()
            Me._ProcessInstance = Nothing
            Me.processStartEvent.Start()
            Me.RaiseEvent_ProcessExited()
        End Sub

        Public Event ProcessExited As EventHandler
        Private Sub RaiseEvent_ProcessExited()
            DefineValues.Forms.SyncContext.Post(AddressOf OnProcessExited, EventArgs.Empty)
        End Sub
        Private Sub OnProcessExited(ByVal e As Object)
            RaiseEvent ProcessExited(Me, DirectCast(e, EventArgs))
        End Sub
        Public Event ProcessLaunched As EventHandler
        Private Sub RaiseEvent_ProcessLaunched()
            DefineValues.Forms.SyncContext.Post(AddressOf OnProcessLaunched, EventArgs.Empty)
        End Sub
        Private Sub OnProcessLaunched(ByVal e As Object)
            RaiseEvent ProcessLaunched(Me, DirectCast(e, EventArgs))
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            If (Me._ProcessInstance IsNot Nothing) Then
                Me._ProcessInstance.EnableRaisingEvents = False
                Me._ProcessInstance.Close()
            End If
            Me._ProcessInstance = Nothing
            If (Me.processStartEvent IsNot Nothing) Then
                Me.processStartEvent.Stop()
                Me.processStartEvent.Dispose()
            End If
            Me.processStartEvent = Nothing
        End Sub
    End Class
End Namespace
