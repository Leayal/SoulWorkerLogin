Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text

Public NotInheritable Class CommonMethods
    Private Shared IsSWUpdaterRunning_Watcher As Classes.ProcessWatcher = Classes.ProcessesWatcher.GetWatcher(DefineValues.Filenames.SWUpdaterFilename)
    Private Shared IsSWRunning_Watcher As Classes.ProcessWatcher = Classes.ProcessesWatcher.GetWatcher(DefineValues.Filenames.SoulWorkerFilename)
    Public Shared ReadOnly Property SoulWorkerProcessWatcher() As Classes.ProcessWatcher
        Get
            Return IsSWRunning_Watcher
        End Get
    End Property
    Public Shared ReadOnly Property SoulWorkerUpdaterProcessWatcher() As Classes.ProcessWatcher
        Get
            Return IsSWUpdaterRunning_Watcher
        End Get
    End Property

    Public Shared Function IsSWUpdaterRunning() As Boolean
        Return IsSWUpdaterRunning_Watcher.IsRunning
        'IsSWUpdaterRunning = False
        'Dim myList() As Process = Process.GetProcessesByName(DefineValues.Filenames.SWUpdaterProcess)
        'If (myList IsNot Nothing) AndAlso (myList.Length > 0) Then
        '    Dim myPath As String = Path.Combine(GetGameFolder(), DefineValues.Filenames.SWUpdaterFilename).ToLower()
        '    For i As Integer = 0 To myList.Length - 1
        '        If (GetExecutablePath(myList(i)).ToLower() = myPath) Then
        '            IsSWUpdaterRunning = True
        '        End If
        '        myList(i).Close()
        '    Next
        '    myList = Nothing
        'End If
    End Function

    Public Shared Function IsReactorRunning() As Boolean
        Return Classes.ProcessesWatcher.GetWatcher(DefineValues.Filenames.ReactorExecutablePath).IsRunning
    End Function

    Public Shared Function IsSWRunning() As Boolean
        Return IsSWRunning_Watcher.IsRunning
    End Function

    Public Overloads Shared Function GetExecutablePath(ByVal Process As Process) As String
        'If running on Vista or later use the new function
        If Environment.OSVersion.Version.Major >= 6 Then
            Return GetExecutablePathAboveVista(Process.Id)
        Else
            Return Process.MainModule.FileName
        End If
    End Function

    Public Overloads Shared Function GetExecutablePath(ByVal ProcessID As Integer) As String
        'If running on Vista or later use the new function
        If Environment.OSVersion.Version.Major >= 6 Then
            Return GetExecutablePathAboveVista(ProcessID)
        Else
            GetExecutablePath = String.Empty
            Using pro As Process = Process.GetProcessById(ProcessID)
                GetExecutablePath = pro.MainModule.FileName
            End Using
        End If
    End Function

    Private Shared Function GetExecutablePathAboveVista(ProcessId As Integer) As String
        GetExecutablePathAboveVista = String.Empty
        Dim buffer = New StringBuilder(1024)
        Dim hprocess As IntPtr = OpenProcess(ProcessAccessFlags.PROCESS_QUERY_LIMITED_INFORMATION, False, ProcessId)
        If hprocess <> IntPtr.Zero Then
            Try
                If QueryFullProcessImageName(hprocess, 0, buffer, buffer.Capacity) Then
                    GetExecutablePathAboveVista = buffer.ToString()
                End If
            Finally
                CloseHandle(hprocess)
            End Try
        End If
    End Function

    <DllImport("kernel32.dll")>
    Private Shared Function QueryFullProcessImageName(hprocess As IntPtr, dwFlags As Integer, lpExeName As StringBuilder, ByRef size As Integer) As Boolean
    End Function
    <DllImport("kernel32.dll")>
    Private Shared Function OpenProcess(dwDesiredAccess As ProcessAccessFlags, bInheritHandle As Boolean, dwProcessId As Integer) As IntPtr
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Shared Function CloseHandle(hHandle As IntPtr) As Boolean
    End Function

    Private Enum ProcessAccessFlags
        PROCESS_QUERY_LIMITED_INFORMATION = &H1000
    End Enum

    Public Shared Function GetSHA1FromFile(ByVal path As String) As String
        Dim sb As New Text.StringBuilder()
        Using fs As FileStream = File.OpenRead(path)
            Using sha1 As System.Security.Cryptography.SHA1 = System.Security.Cryptography.SHA1.Create()
                Dim bytes() As Byte = sha1.ComputeHash(fs)
                For i As Integer = 0 To bytes.Length - 1
                    sb.AppendFormat("{0:X2}", bytes(i))
                Next
            End Using
        End Using
        Return sb.ToString()
    End Function
    Public Shared Sub EmptyFolder(ByVal path As String)
        If (Directory.Exists(path)) Then
            For Each file In IO.Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                Try
                    IO.File.Delete(file)
                Catch
                End Try
            Next
            For Each founddir In IO.Directory.GetDirectories(path, "*", SearchOption.AllDirectories)
                Try
                    IO.Directory.Delete(founddir, True)
                Catch
                End Try
            Next
        End If
    End Sub
    Public Shared Function NetworkPathBuild(ParamArray path As String()) As String
        Dim theStrinbBuilder As New Text.StringBuilder()
        If (path.Length > 0) Then
            For i As Integer = 0 To path.Length - 1
                If (i = 0) Then
                    theStrinbBuilder.Append(path(i).Replace("\", "/"))
                Else
                    theStrinbBuilder.Append("/" & path(i).Replace("\", "/"))
                End If
            Next
        End If
        Return theStrinbBuilder.ToString()
    End Function

    Private Shared cacheString As String
    Public Shared Function GetGameFolder() As String
        If (String.IsNullOrWhiteSpace(cacheString)) Then
            cacheString = DirectCast(My.Computer.Registry.GetValue(Path.Combine({My.Computer.Registry.LocalMachine.Name(), "SOFTWARE", "HanPurple", "J_SW"}), "folder", String.Empty), String)
            If (Not String.IsNullOrWhiteSpace(cacheString)) Then
                If (CorrectPath(cacheString)) Then
                    My.Computer.Registry.SetValue(Path.Combine({My.Computer.Registry.LocalMachine.Name(), "SOFTWARE", "HanPurple", "J_SW"}), "folder", cacheString, Microsoft.Win32.RegistryValueKind.String)
                End If
                If (File.Exists(Path.Combine(cacheString, DefineValues.Filenames.SoulWorkerFilename))) Then
                    IsSWRunning_Watcher.SetPath(Path.Combine(cacheString, DefineValues.Filenames.SoulWorkerFilename))
                    IsSWUpdaterRunning_Watcher.SetPath(Path.Combine(cacheString, DefineValues.Filenames.SWUpdaterFilename))
                End If
            End If
            Return cacheString
        Else
            If (File.Exists(Path.Combine(cacheString, DefineValues.Filenames.SoulWorkerFilename))) Then
                If (CorrectPath(cacheString)) Then
                    My.Computer.Registry.SetValue(Path.Combine({My.Computer.Registry.LocalMachine.Name(), "SOFTWARE", "HanPurple", "J_SW"}), "folder", cacheString, Microsoft.Win32.RegistryValueKind.String)
                End If
                Return cacheString
            Else
                Dim tmpcacheString As String = DirectCast(My.Computer.Registry.GetValue(Path.Combine({My.Computer.Registry.LocalMachine.Name(), "SOFTWARE", "HanPurple", "J_SW"}), "folder", String.Empty), String)
                If (Not String.IsNullOrWhiteSpace(tmpcacheString)) Then
                    If (CorrectPath(tmpcacheString)) Then
                        My.Computer.Registry.SetValue(Path.Combine({My.Computer.Registry.LocalMachine.Name(), "SOFTWARE", "HanPurple", "J_SW"}), "folder", tmpcacheString, Microsoft.Win32.RegistryValueKind.String)
                    End If
                    If (File.Exists(Path.Combine(tmpcacheString, DefineValues.Filenames.SoulWorkerFilename))) Then
                        IsSWRunning_Watcher.SetPath(Path.Combine(tmpcacheString, DefineValues.Filenames.SoulWorkerFilename))
                        IsSWUpdaterRunning_Watcher.SetPath(Path.Combine(tmpcacheString, DefineValues.Filenames.SWUpdaterFilename))
                        cacheString = tmpcacheString
                    End If
                Else
                    cacheString = String.Empty
                End If
                Return cacheString
            End If
        End If
    End Function

    Private Shared Function CorrectPath(ByRef outString As String) As Boolean
        CorrectPath = False
        If (outString.IndexOf("\\") > -1) Or (outString.IndexOf("/") > -1) Then
            CorrectPath = True
            outString = Path.GetFullPath(outString)
        End If
    End Function

    Public Shared Function IsSoulWorkerInstalled() As Boolean
        Dim tmpPath As String = GetGameFolder()
        If (String.IsNullOrWhiteSpace(tmpPath)) Then
            Return False
        Else
            Return (File.Exists(Path.Combine(tmpPath, DefineValues.Filenames.SoulWorkerFilename)))
        End If
    End Function

    Public Shared Function IsWindowsXP() As Boolean
        If (My.Computer.Info.OSFullName.IndexOf("Windows XP") > -1) Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Overloads Shared Function StringTableToArgs(ParamArray myArray As String()) As String
        If (myArray IsNot Nothing) AndAlso (myArray.Length > 0) Then
            Dim theStringBuilder As New Text.StringBuilder()
            For i As Integer = 0 To myArray.Length - 1
                If (i = 0) Then
                    If (myArray(i).IndexOf(" "c) > -1) Then
                        theStringBuilder.Append("""" & myArray(i) & """")
                    Else
                        theStringBuilder.Append(myArray(i))
                    End If
                Else
                    If (myArray(i).IndexOf(" "c) > -1) Then
                        theStringBuilder.Append(" """ & myArray(i) & """")
                    Else
                        theStringBuilder.Append(" " & myArray(i))
                    End If
                End If
            Next
            Return theStringBuilder.ToString()
        Else
            Return String.Empty
        End If
    End Function

    Public Overloads Shared Function StringTableToArgs(ByVal strings As IEnumerable(Of String)) As String
        Return StringTableToArgs(strings.ToArray())
    End Function

    Public Overloads Shared Sub OutputToLog(ByVal Message As String)
        Classes.Log.LogManager.GeneralLog.QueueLog(Message)
    End Sub

    Public Overloads Shared Sub OutputToLog(ByVal Ex As Exception)
        Classes.Log.LogManager.GeneralLog.QueueLog(Ex)
    End Sub
End Class
