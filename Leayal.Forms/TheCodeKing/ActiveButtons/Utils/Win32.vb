Imports System
Imports System.Runtime.InteropServices

Namespace TheCodeKing.ActiveButtons.Utils
	Friend Class Win32
		Public Const WS_CHILD As UInteger = 1073741824

		Public Const WS_EX_LAYERED As UInteger = 524288

		Public Const WS_CLIPSIBLINGS As UInteger = 67108864

		Public Const WM_ACTIVATEAPP As UInteger = 28

		Public Const WM_SIZE As Integer = 5

		Public Shared version As Integer

		Public ReadOnly Shared Property DwmIsCompositionEnabled As Boolean
			Get
				Dim flag As Boolean
				flag = If(Win32.version < 6, False, Win32.DwmIsCompositionEnabled32())
				Return flag
			End Get
		End Property

		Shared Sub New()
			Win32.version = Environment.OSVersion.Version.Major
		End Sub

		Public Sub New()
			MyBase.New()
		End Sub

		<DllImport("dwmapi.dll", CharSet:=CharSet.None, EntryPoint:="DwmIsCompositionEnabled", ExactSpelling:=False, PreserveSig:=False)>
		Private Shared Function DwmIsCompositionEnabled32() As Boolean
		End Function

		<DllImport("user32.dll", CharSet:=CharSet.None, ExactSpelling:=False)>
		Public Shared Function GetDesktopWindow() As IntPtr
		End Function
	End Class
End Namespace