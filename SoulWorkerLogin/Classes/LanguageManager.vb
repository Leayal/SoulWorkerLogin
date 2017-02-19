Imports System.IO

Public Class LanguageManager
    Private languageFilePath As String
    Private myIniFile As IniFile
    Private Shared _Instance As LanguageManager
    Public Shared ReadOnly Property Instance() As LanguageManager
        Get
            If (_Instance Is Nothing) Then
                Dim iniaisdnalila As New IniFile(Path.Combine(My.Application.Info.DirectoryPath(), DefineValues.Options.Filename))
                Initialize(iniaisdnalila.GetValue(DefineValues.Options.SectionAdvanced, DefineValues.Options.SectionAdvanced_Language, "english"))
                iniaisdnalila.Close()
            End If
            Return _Instance
        End Get
    End Property
    Public Shared Sub Initialize(ByVal lang As String)
        _Instance = New LanguageManager(lang)
    End Sub
    Public Sub New(ByVal lang As String)
        Me.languageFilePath = Path.Combine(My.Application.Info.DirectoryPath(), "lang", lang & ".ini")
        If (Not File.Exists(languageFilePath)) Then
            Me.languageFilePath = Path.Combine(My.Application.Info.DirectoryPath(), "lang", "english.ini")
        End If
        Me.LoadLanguageFile(Me.languageFilePath)
    End Sub

    Private Sub LoadLanguageFile(ByVal filepath As String)
        If (Me.myIniFile IsNot Nothing) Then
            Me.myIniFile.Close()
        End If
        Me.myIniFile = New IniFile(filepath)
    End Sub

    Public Overloads Shared Function GetControlText(ByVal textID As String, ByVal defaultvalue As String) As String
        Return Instance.GetTextEx("Controls", textID, defaultvalue)
    End Function

    Public Overloads Shared Function GetMessage(ByVal textID As String, ByVal defaultvalue As String) As String
        Return Instance.GetTextEx("Messages", textID, defaultvalue)
    End Function

    Public Overloads Shared Function GetText(ByVal category As String, ByVal textID As String, ByVal defaultvalue As String) As String
        Return Instance.GetTextEx(category, textID, defaultvalue)
    End Function

    Public Function GetTextEx(ByVal category As String, ByVal textID As String, ByVal defaultvalue As String) As String
        Return Me.myIniFile.GetValue(category, textID, defaultvalue)
    End Function
End Class
