Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.Win32

Module Program
    <STAThread>
    Public Sub Main()
        AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf AssemblyResolver.CurrentDomain_AssemblyResolve
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException)
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf MyApplication_UnhandledException
        Application.SetCompatibleTextRenderingDefault(False)

        Try
            Dim MyssWindowsFormsApplicationBase As New MyWindowsFormsApplicationBase()
            MyssWindowsFormsApplicationBase.Run(System.Environment.GetCommandLineArgs())
        Catch ex As Exception
            MessageBox.Show(ex.Message & vbNewLine & "Detail: " & ex.StackTrace, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub MyApplication_UnhandledException(sender As Object, e As System.UnhandledExceptionEventArgs)
        If (e.IsTerminating) Then
            Try
                Classes.Log.LogManager.GeneralLog.QueueLog(DirectCast(e.ExceptionObject, Exception))
            Catch
            End Try
        Else
            Try
                With DirectCast(e.ExceptionObject, Exception)
                    MessageBox.Show(.Message & vbNewLine & "Detail: " & .StackTrace, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End With
            Catch
            End Try
        End If
    End Sub

    Class MyWindowsFormsApplicationBase
        Inherits Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase

        Public Sub New()
            MyBase.New()
            Me.IsSingleInstance = True
            Me.EnableVisualStyles = True
            Me.SaveMySettingsOnExit = False
            Me.ShutdownStyle = Global.Microsoft.VisualBasic.ApplicationServices.ShutdownMode.AfterMainFormCloses
            AddHandler Microsoft.Win32.SystemEvents.UserPreferenceChanged, AddressOf App_UserPreferenceChangedEventArgs
        End Sub

        Private Sub App_UserPreferenceChangedEventArgs(sender As Object, e As UserPreferenceChangedEventArgs)
            If (e.Category = UserPreferenceCategory.Color) Or (e.Category = UserPreferenceCategory.VisualStyle) Or (e.Category = UserPreferenceCategory.General) Then
                Leayal.Forms.AeroControl.ThemeInfo.GetAccentColor()
            End If
        End Sub

        Protected Overrides Sub OnShutdown()
            RemoveHandler Microsoft.Win32.SystemEvents.UserPreferenceChanged, AddressOf App_UserPreferenceChangedEventArgs
            DefineValues.Notify.NotifyIcon.Visible = False
            DefineValues.Notify.NotifyIcon.Dispose()
            Classes.ProcessesWatcher.Instance.Dispose()
            MyBase.OnShutdown()
        End Sub

        Protected Overrides Function OnStartup(eventArgs As StartupEventArgs) As Boolean
            If (IsSetArg(eventArgs.CommandLine, "dumpversionout")) Then
                Using fs As IO.FileStream = IO.File.Create(IO.Path.Combine(Me.Info.DirectoryPath(), "SWWebClientLiteVersion.dat"))
                    Using bw As New IO.BinaryWriter(fs)
                        With (Me.Info.Version)
                            bw.Write(Convert.ToByte(.Major))
                            bw.Write(Convert.ToByte(.Minor))
                            bw.Write(Convert.ToByte(.Build))
                            bw.Write(Convert.ToByte(.Revision))
                        End With
                    End Using
                End Using
                eventArgs.Cancel = True
                Environment.Exit(0)
            End If
            SelfUpdate.Instance.UpdaterUri = New Uri(DefineValues.Updater.UpdaterPath)
            SelfUpdate.Instance.UpdateUri = New Uri(DefineValues.Updater.PatchPath)
            SelfUpdate.Instance.VersionUri = New Uri(DefineValues.Updater.VersionPath)
            Dim theIni As New IniFile(IO.Path.Combine(Me.Info.DirectoryPath(), DefineValues.Options.Filename))
            If (theIni.GetValue(DefineValues.Updater.SectionUpdates, DefineValues.Updater.SectionUpdates_CheckAtStartup, "1") <> "0") Then
                SelfUpdate.Instance.CheckForUpdates()
            End If
            theIni.Close()
            Leayal.Forms.AeroControl.ThemeInfo.GetAccentColorEx()
            Return MyBase.OnStartup(eventArgs)
        End Function

        Protected Overrides Sub OnStartupNextInstance(eventArgs As StartupNextInstanceEventArgs)
            If (IsSetArg(eventArgs.CommandLine, "dumpversionout")) Then
                Using fs As IO.FileStream = IO.File.Create(IO.Path.Combine(Me.Info.DirectoryPath(), "SWWebClientLiteVersion.dat"))
                    Using bw As New IO.BinaryWriter(fs)
                        With (Me.Info.Version)
                            bw.Write(Convert.ToByte(.Major))
                            bw.Write(Convert.ToByte(.Minor))
                            bw.Write(Convert.ToByte(.Build))
                            bw.Write(Convert.ToByte(.Revision))
                        End With
                    End Using
                End Using
            End If
            MyBase.OnStartupNextInstance(eventArgs)
        End Sub
        Protected Overrides Function OnUnhandledException(e As UnhandledExceptionEventArgs) As Boolean
            MessageBox.Show(e.Exception.Message & vbNewLine & "Detail: " & e.Exception.StackTrace, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            e.ExitApplication = True
            Return MyBase.OnUnhandledException(e)
        End Function

        Protected Overrides Sub OnCreateMainForm()
            Me.MainForm = Global.SoulWorkerLogin.MyMainMenuForm
            DefineValues.Forms.SetSync(Threading.SynchronizationContext.Current)
        End Sub

        Private Function IsSetArg(ByVal args As IEnumerable(Of String), ByVal argName As String) As Boolean
            Return IsSetArg(args.ToArray(), argName)
        End Function

        Private Function IsSetArg(ByVal args As String(), ByVal argName As String) As Boolean
            IsSetArg = False
            If (args IsNot Nothing) AndAlso (args.Length > 0) Then
                For i As Integer = 0 To args.Length - 1
                    If (args(i).ToLower() = argName.ToLower()) Then
                        IsSetArg = True
                    End If
                Next
            End If
        End Function
    End Class
End Module
