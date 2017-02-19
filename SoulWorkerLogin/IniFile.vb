Public Class IniFile
    Private sFilename As String
    Private tmpStringBuild As System.Text.StringBuilder
    Private o_Sections As New System.Collections.Generic.Dictionary(Of String, IniSection)()

#Region "Constructors"
    Public Sub New(filePath As String)
        Me.sFilename = filePath
        Me.tmpStringBuild = New System.Text.StringBuilder()
        If System.IO.File.Exists(filePath) Then
            Using theReader As New System.IO.StreamReader(filePath)
                ReadIniFromTextStream(theReader)
            End Using
        End If
    End Sub


    Public Sub New(Stream As System.IO.TextReader, Optional CloseAfterRead As Boolean = True)
        Me.sFilename = String.Empty
        Me.tmpStringBuild = New System.Text.StringBuilder()
        Me.ReadIniFromTextStream(Stream)
        If CloseAfterRead Then
            Stream.Close()
        End If
    End Sub
#End Region

#Region "Methods"
    Public Function GetValue(section As String, key As String, defaultValue As String) As String
        For Each theNode In Me.o_Sections
            If theNode.Key.ToLower() = section.ToLower() Then
                For Each theInsideNode In Me.o_Sections(theNode.Key).IniKeyValues
                    If theInsideNode.Key.ToLower() = key.ToLower() Then
                        Return theInsideNode.Value.Value
                    End If
                Next
            End If
        Next
        Return defaultValue
    End Function

    Public Sub SetValue(section As String, key As String, value As String)
        If checkSection(section) = False Then
            Me.o_Sections.Add(section, New IniSection())
        End If
        For Each theNode In Me.o_Sections(section).IniKeyValues
            If theNode.Key.ToLower() = key.ToLower() Then
                theNode.Value.Value = value
                Return
            End If
        Next
        Me.o_Sections(section).IniKeyValues.Add(key, New IniKeyValue(value))
    End Sub

    Public Sub Save()
        Me.Save(System.Text.Encoding.UTF8)
    End Sub

    Public Sub Save(encode As System.Text.Encoding)
        If String.IsNullOrWhiteSpace(Me.sFilename) Then
            Return
        End If
        Using theWriter As New System.IO.StreamWriter(Me.sFilename, False, encode)
            WriteToStream(theWriter)
        End Using
    End Sub

    Public Sub SaveAs(newPath As String)
        Me.SaveAs(newPath, System.Text.Encoding.UTF8)
    End Sub

    Public Sub SaveAs(newPath As String, encode As System.Text.Encoding)
        If String.IsNullOrWhiteSpace(newPath) Then
            Return
        End If
        Me.sFilename = newPath
        Using theWriter As New System.IO.StreamWriter(newPath, False, encode)
            WriteToStream(theWriter)
        End Using
    End Sub

    Public Sub Close()
        If (Me.tmpStringBuild IsNot Nothing) Then Me.tmpStringBuild.Clear()
        Me.sFilename = Nothing
        If (Me.o_Sections IsNot Nothing) Then Me.o_Sections.Clear()
        Me.tmpStringBuild = Nothing
        Me.o_Sections = Nothing
    End Sub

    Public Overrides Function ToString() As String
        Me.tmpStringBuild.Clear()
        For Each Section_loopVariable In Me.o_Sections
            If Section_loopVariable.Value.IsComment = False Then
                Me.tmpStringBuild.AppendLine("[" + Section_loopVariable.Key + "]")
            Else
                Me.tmpStringBuild.AppendLine(";[" + Section_loopVariable.Key + "]")
            End If
            For Each KeyValue_loopVariable In Section_loopVariable.Value.IniKeyValues
                If Section_loopVariable.Value.IsComment = False Then
                    Me.tmpStringBuild.AppendLine(KeyValue_loopVariable.Key + "=" + KeyValue_loopVariable.Value.Value)
                Else
                    Me.tmpStringBuild.AppendLine(";" + KeyValue_loopVariable.Key + "=" + KeyValue_loopVariable.Value.Value)
                End If
            Next
        Next
        Return Me.tmpStringBuild.ToString()
    End Function
#End Region

#Region "Properties"
    Public ReadOnly Property Sections() As System.Collections.ObjectModel.ReadOnlyCollection(Of String)
        Get
            Return New System.Collections.ObjectModel.ReadOnlyCollection(Of String)(Me.o_Sections.Keys.ToList())
        End Get
    End Property

    Public ReadOnly Property SectionCount() As Integer
        Get
            Return Me.o_Sections.Keys.Count
        End Get
    End Property
#End Region

#Region "Private Methods"
    Private Function checkSection(theKey As String) As Boolean
        For Each theNode In Me.o_Sections
            If theNode.Key.ToLower() = theKey.ToLower() Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Sub ReadIniFromTextStream(Stream As System.IO.TextReader)
        Dim lineBuffer As String = String.Empty
        Dim splitBuffer As String() = Nothing
        Dim sectionBuffer As IniSection = Nothing
        Dim pumBuffer As Integer
        Dim spli As Char() = New Char() {"="c}
        'weird ..... but we don't need to new char[] for each read buffer
        While (InlineAssignHelper(pumBuffer, Stream.Read())) <> -1
            If (ChrW(pumBuffer) = ControlChars.Lf) Then
                If Not String.IsNullOrWhiteSpace(lineBuffer) Then
                    ' move this line here because no need to check for every read char
                    If lineBuffer.StartsWith("[") AndAlso lineBuffer.EndsWith("]") Then
                        sectionBuffer = New IniSection(False)
                        Me.o_Sections.Add(lineBuffer.Substring(1, lineBuffer.Length - 2), sectionBuffer)
                        lineBuffer = String.Empty
                    ElseIf lineBuffer.IndexOf("=") > -1 Then
                        splitBuffer = lineBuffer.Split(spli, 2)
                        ' make sure it split just one time
                        sectionBuffer.IniKeyValues.Add(splitBuffer(0).Trim(), New IniKeyValue(splitBuffer(1).Trim()))
                        lineBuffer = String.Empty
                    End If
                End If
            ElseIf ChrW(pumBuffer) = ControlChars.Cr Then
            ElseIf pumBuffer = 0 Then
            Else
                lineBuffer += ChrW(pumBuffer)
            End If
        End While
        If Not String.IsNullOrWhiteSpace(lineBuffer) Then
            'This will make sure last line without \n will not be discarded
            If lineBuffer.StartsWith("[") AndAlso lineBuffer.EndsWith("]") Then
                sectionBuffer = New IniSection(False)
                Me.o_Sections.Add(lineBuffer.Substring(1, lineBuffer.Length - 2), sectionBuffer)
                lineBuffer = String.Empty
            ElseIf lineBuffer.IndexOf("=") > -1 Then
                splitBuffer = lineBuffer.Split("="c)
                sectionBuffer.IniKeyValues.Add(splitBuffer(0).Trim(), New IniKeyValue(splitBuffer(1).Trim()))
                lineBuffer = String.Empty
            End If
        End If
        spli = Nothing
        sectionBuffer = Nothing
        lineBuffer = Nothing
        splitBuffer = Nothing
    End Sub

    Private Sub WriteToStream(theStream As System.IO.TextWriter)
        Me.tmpStringBuild.Clear()
        For Each Section_loopVariable In Me.o_Sections
            If Section_loopVariable.Value.IsComment = False Then
                Me.tmpStringBuild.AppendLine("[" + Section_loopVariable.Key + "]")
            Else
                Me.tmpStringBuild.AppendLine(";[" + Section_loopVariable.Key + "]")
            End If
            For Each KeyValue_loopVariable In Section_loopVariable.Value.IniKeyValues
                If Section_loopVariable.Value.IsComment = False Then
                    Me.tmpStringBuild.AppendLine(KeyValue_loopVariable.Key + "=" + KeyValue_loopVariable.Value.Value)
                Else
                    Me.tmpStringBuild.AppendLine(";" + KeyValue_loopVariable.Key + "=" + KeyValue_loopVariable.Value.Value)
                End If
            Next
        Next
        theStream.Write(Me.tmpStringBuild.ToString())
        theStream.Flush()
    End Sub
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function
#End Region
End Class

Public Class IniSection
    Private m_IsComment As Boolean
    Public Property IsComment() As Boolean
        Get
            Return Me.m_IsComment
        End Get
        Set
            Me.m_IsComment = Value
        End Set
    End Property

    Private m_CommentList As System.Collections.Generic.List(Of String)
    Public ReadOnly Property CommentList() As System.Collections.Generic.List(Of String)
        Get
            Return Me.m_CommentList
        End Get
    End Property

    Private m_ListOfIniKeyValue As System.Collections.Generic.Dictionary(Of String, IniKeyValue)
    Public ReadOnly Property IniKeyValues() As System.Collections.Generic.Dictionary(Of String, IniKeyValue)
        Get
            Return Me.m_ListOfIniKeyValue
        End Get
    End Property

    Public Sub New()
        Me.New(False)
    End Sub

    Public Sub New(IsComment As Boolean)
        Me.m_IsComment = IsComment
        Me.m_ListOfIniKeyValue = New System.Collections.Generic.Dictionary(Of String, IniKeyValue)()
        Me.m_CommentList = New System.Collections.Generic.List(Of String)()
    End Sub

    Public Sub Clear()
        Me.m_ListOfIniKeyValue.Clear()
        Me.m_CommentList.Clear()
    End Sub
End Class

Public Class IniKeyValue
    Private m_IsComment As Boolean
    Public Property IsComment() As Boolean
        Get
            Return Me.m_IsComment
        End Get
        Set
            Me.m_IsComment = Value
        End Set
    End Property

    Private m_Value As String
    Public Property Value() As String
        Get
            Return Me.m_Value
        End Get
        Set
            Me.m_Value = Value
        End Set
    End Property

    Public Sub New(Value As String)
        Me.New(Value, False)
    End Sub

    Public Sub New(Value As String, IsComment As Boolean)
        Me.m_IsComment = IsComment
        Me.m_Value = Value
    End Sub
End Class