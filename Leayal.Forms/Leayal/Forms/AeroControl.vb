Imports System.Text
Imports System.Runtime.InteropServices
Imports System.Drawing
Public NotInheritable Class AeroControl

    Private Shared SyncContext As System.Threading.SynchronizationContext = System.Threading.SynchronizationContext.Current

    Public Class ThemeInfo
        Public Shared _instance As ThemeInfo = New ThemeInfo()
        Public Shared ReadOnly Property Instance() As ThemeInfo
            Get
                Return _instance
            End Get
        End Property

        Private Shared _ThemeColor As Color = Color.DarkGoldenrod
        Private Shared _Theme As Boolean = False
        Public Shared ReadOnly Property Theme() As Boolean
            Get
                Return _Theme
            End Get
        End Property
        Public Shared ReadOnly Property ThemeColor() As Color
            Get
                Return _ThemeColor
            End Get
        End Property
        <DllImport("uxtheme.dll", EntryPoint:="#95")>
        Protected Shared Function GetImmersiveColorFromColorSetEx(dwImmersiveColorSet As UInteger, dwImmersiveColorType As UInteger, bIgnoreHighContrast As Boolean, dwHighContrastCacheMode As UInteger) As UInteger
        End Function
        <DllImport("uxtheme.dll", EntryPoint:="#96")>
        Protected Shared Function GetImmersiveColorTypeFromName(pName As IntPtr) As UInteger
        End Function
        <DllImport("uxtheme.dll", EntryPoint:="#98")>
        Protected Shared Function GetImmersiveUserColorSetPreference(bForceCheckRegistry As Boolean, bSkipCheckOnFail As Boolean) As Integer
        End Function

        <System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions>
        <Security.SecurityCritical>
        Public Shared Sub GetAccentColorEx()
            If IsWin7 Then
                _ThemeColor = Color.DarkGoldenrod
            Else
                Dim pElementName As IntPtr = Marshal.StringToHGlobalUni("ImmersiveStartSelectionBackground")
                Try
                    If (pElementName <> IntPtr.Zero) Then
                        Dim colourset As Integer = GetImmersiveUserColorSetPreference(False, False)
                        Dim type As UInteger = GetImmersiveColorTypeFromName(pElementName)
                        Dim colourdword As UInteger = GetImmersiveColorFromColorSetEx(CUInt(colourset), type, False, 0)
                        Dim colourbytes As Byte() = New Byte(3) {}
                        colourbytes(0) = CByte((&HFF000000UI And colourdword) >> 24)
                        ' A
                        colourbytes(1) = CByte((&HFF0000 And colourdword) >> 16)
                        ' B
                        colourbytes(2) = CByte((&HFF00 And colourdword) >> 8)
                        ' G
                        colourbytes(3) = CByte(&HFF And colourdword)
                        ' R
                        _ThemeColor = Color.FromArgb(colourbytes(0), colourbytes(3), colourbytes(2), colourbytes(1))
                        _Theme = True
                        Marshal.FreeCoTaskMem(pElementName)
                    Else
                        _ThemeColor = Color.DarkGoldenrod
                    End If
                Catch ex As AccessViolationException
                    If (pElementName <> IntPtr.Zero) Then Marshal.FreeCoTaskMem(pElementName)
                    _ThemeColor = Color.DarkGoldenrod
                End Try
            End If
            'Dim asd As New System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptionsAttribute()
        End Sub

        Public Shared Sub GetAccentColor()
            GetAccentColorEx()
            RaiseEvent_ColorChanged()
        End Sub

        Public Shared Event ColorChanged As EventHandler
        Private Shared Sub RaiseEvent_ColorChanged()
            If (SyncContext IsNot Nothing) Then
                SyncContext.Post(AddressOf OnColorChanged, System.EventArgs.Empty)
            End If
        End Sub
        Private Shared Sub OnColorChanged(ByVal e As Object)
            RaiseEvent ColorChanged(SyncContext, DirectCast(e, EventArgs))
        End Sub
    End Class
    Private Shared _IsWin10 As Boolean = IsWindows10()
    Public Shared ReadOnly Property IsWin10() As Boolean
        Get
            Return _IsWin10
        End Get
    End Property
    Private Shared _IsWin7 As Boolean = IsWindows7()
    Public Shared ReadOnly Property IsWin7() As Boolean
        Get
            Return _IsWin7
        End Get
    End Property
    Public Shared Function IsWindows10() As Boolean
        If (My.Computer.Info.OSFullName.IndexOf("Windows 10") > -1) Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Shared Function IsWindows7() As Boolean
        If (My.Computer.Info.OSFullName.IndexOf("Windows 7") > -1) Then
            Return True
        Else
            Return False
        End If
    End Function

#Region "Windows 10"
    Friend Enum AccentState
        ACCENT_DISABLED = 0
        ACCENT_ENABLE_GRADIENT = 1
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2
        ACCENT_ENABLE_BLURBEHIND = 3
        ACCENT_INVALID_STATE = 4
    End Enum

    <StructLayout(LayoutKind.Sequential)>
    Friend Structure AccentPolicy
        Public AccentState As AccentState
        Public AccentFlags As Integer
        Public GradientColor As Integer
        Public AnimationId As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Friend Structure WindowCompositionAttributeData
        Public Attribute As WindowCompositionAttribute
        Public Data As IntPtr
        Public SizeOfData As Integer
    End Structure

    Friend Enum WindowCompositionAttribute
        ' ...
        WCA_ACCENT_POLICY = 19
        ' ...
    End Enum

    ''' <summary>
    ''' Interaction logic for MainWindow.xaml
    ''' </summary>
    <DllImport("user32.dll")>
    Friend Shared Function SetWindowCompositionAttribute(hwnd As IntPtr, ByRef data As WindowCompositionAttributeData) As Integer
    End Function
#End Region

    Public Shared Sub RefreshBlur(targetForm As System.Windows.Forms.Form)
        DisableBlur(targetForm)
        EnableBlur(targetForm, True)
    End Sub

    Public Shared Sub DisableBlur(targetForm As System.Windows.Forms.Form)
        If IsWin10 Then
            Dim accent = New AccentPolicy()
            accent.AccentState = AccentState.ACCENT_DISABLED

            Dim accentStructSize = Marshal.SizeOf(accent)

            Dim accentPtr = Marshal.AllocHGlobal(accentStructSize)
            Marshal.StructureToPtr(accent, accentPtr, False)

            Dim data = New WindowCompositionAttributeData()
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY
            data.SizeOfData = accentStructSize
            data.Data = accentPtr

            SetWindowCompositionAttribute(targetForm.Handle, data)

            Marshal.FreeHGlobal(accentPtr)
        End If
    End Sub

    Public Shared Sub EnableBlur(targetForm As System.Windows.Forms.Form, ByVal keepBG As Boolean)
        If IsWin10 Then
            Dim accent = New AccentPolicy()
            accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND

            Dim accentStructSize = Marshal.SizeOf(accent)

            Dim accentPtr = Marshal.AllocHGlobal(accentStructSize)
            Marshal.StructureToPtr(accent, accentPtr, False)

            Dim data = New WindowCompositionAttributeData()
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY
            data.SizeOfData = accentStructSize
            data.Data = accentPtr

            SetWindowCompositionAttribute(targetForm.Handle, data)

            Marshal.FreeHGlobal(accentPtr)
            If Not keepBG Then
                targetForm.BackColor = Color.Lavender
                targetForm.TransparencyKey = Color.Lavender
                'AddHandler targetForm.Paint, AddressOf TargetForm_Paint
            End If
        End If
    End Sub

    Public Shared Sub EnableBlur(targetForm As System.Windows.Forms.Form)
        If IsWin10 Then
            Dim accent = New AccentPolicy()
            accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND

            Dim accentStructSize = Marshal.SizeOf(accent)

            Dim accentPtr = Marshal.AllocHGlobal(accentStructSize)
            Marshal.StructureToPtr(accent, accentPtr, False)

            Dim data = New WindowCompositionAttributeData()
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY
            data.SizeOfData = accentStructSize
            data.Data = accentPtr

            SetWindowCompositionAttribute(targetForm.Handle, data)

            Marshal.FreeHGlobal(accentPtr)
            targetForm.TransparencyKey = targetForm.BackColor
            'AddHandler targetForm.Paint, AddressOf TargetForm_Paint
        End If
    End Sub
End Class
