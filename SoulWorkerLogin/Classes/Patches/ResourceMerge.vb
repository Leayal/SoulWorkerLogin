Imports System.IO
Imports System.Text
Imports System.Security.Cryptography

Namespace Classes.Patches
    Public NotInheritable Class ResourceMerge
        Private Const HashLength As Int32 = 32

        Public Shared Function ParseFormat(ByVal formatString As String) As ResourceFormatMeta
            If (String.IsNullOrWhiteSpace(formatString)) Then
                Return Nothing
            Else
                Dim theSplitted() As String = formatString.Split(New Char() {" "c}, 3)
                Dim num As Integer = 0
                If (Int32.TryParse(theSplitted(0), num)) Then
                    Return New ResourceFormatMeta(num, theSplitted(1), theSplitted(2))
                Else
                    Return New ResourceFormatMeta(theSplitted(0), theSplitted(1))
                End If
            End If
        End Function

        Public Class ResourceFormatMeta
            'idIndex As Integer, CountByte As String, formatstring As String
            Public ReadOnly Property IdIndex() As Integer
            Public ReadOnly Property CountByte() As String
            Public ReadOnly Property formatstring() As String
            Public Sub New(ByVal id As Integer, ByVal count As String, ByVal format As String)
                Me.IdIndex = id
                Me.CountByte = count
                Me.formatstring = format
            End Sub
            Public Sub New(ByVal count As String, ByVal format As String)
                Me.New(0, count, format)
            End Sub
        End Class

        Public Overloads Shared Function readTranslation(ByVal file As String) As Dictionary(Of UInt64, List(Of String))
            Dim result As Dictionary(Of UInt64, List(Of String))
            Using theStreamReader As New StreamReader(file)
                result = readTranslation(theStreamReader)
            End Using
            Return result
        End Function

        Public Overloads Shared Function readTranslation(ByVal reader As Stream) As Dictionary(Of UInt64, List(Of String))
            Dim result As Dictionary(Of UInt64, List(Of String))
            Using theStreamReader As New StreamReader(reader)
                result = readTranslation(theStreamReader)
            End Using
            Return result
        End Function

        Public Overloads Shared Function readTranslation(ByVal reader As StreamReader) As Dictionary(Of UInt64, List(Of String))
            Dim theTranslation As New Dictionary(Of UInt64, List(Of String))()
            Dim currentNode As List(Of String) = Nothing
            Dim bufferedString As String = Nothing
            Dim tmpIndex As UInt64 = 0
            While Not reader.EndOfStream
                bufferedString = reader.ReadLine()
                If Not String.IsNullOrWhiteSpace(bufferedString) Then
                    If bufferedString.ToLower().StartsWith("id=") Then
                        tmpIndex = UInt64.Parse(bufferedString.Remove(0, 3))
                        If Not theTranslation.ContainsKey(tmpIndex) Then
                            currentNode = New List(Of String)()
                            theTranslation.Add(tmpIndex, currentNode)
                        Else
                            currentNode = theTranslation(tmpIndex)
                        End If
                    Else
                        If currentNode IsNot Nothing Then
                            If (bufferedString.Length > 511) Then
                                bufferedString = "0"
                            End If
                            If bufferedString.IndexOf("\n") > -1 Then
                                'must be \\n, even with @string
                                bufferedString = bufferedString.Replace("\n", vbLf)
                            End If
                            currentNode.Add(bufferedString)
                        End If
                    End If
                End If
            End While
            Return theTranslation
        End Function

        Public Overloads Shared Sub MergeResource(ByVal database As Dictionary(Of UInt64, List(Of String)), originalFile As Stream, outputFile As Stream, meta As ResourceFormatMeta)
            MergeResource(database, originalFile, outputFile, meta.IdIndex, meta.CountByte, meta.formatstring)
        End Sub

        Public Overloads Shared Sub MergeResource(ByVal databaseStream As Stream, originalFile As Stream, outputFile As Stream, idIndex As Integer, CountByte As String, formatstring As String)
            MergeResource(databaseStream, originalFile, outputFile, idIndex, CountByte, formatstring)
        End Sub

        Public Overloads Shared Sub MergeResource(database As Dictionary(Of UInt64, List(Of String)), originalFile As Stream, outputFile As Stream, idIndex As Integer, CountByte As String, formatstring As String)
            '. . . 5    4    2 1 len 2
            '     id  count   format
            originalFile.Position = 0
            Dim format As String() = formatstring.Split(" "c)

            Dim totalLen As Integer = 0
            For Each punpun As String In format
                If punpun.ToLower() = "len" Then
                    totalLen += 1
                End If
            Next

            Dim currentDic As List(Of String) = Nothing
            Dim list As New List(Of Object)()

            Dim dataCount As Object = 0
            Dim dataSum As UInt64 = 0
            Dim hashLength__1 As UInt16
            Dim hash(HashLength - 1) As Byte

            Dim tmpString As String = Nothing
            Dim currentID As UInt64 = 0

            Using bw = New BinaryWriter(outputFile)
                Using br = New BinaryReader(originalFile)
                    Select Case CountByte
                        Case "1"
                            dataCount = br.ReadByte()
                            bw.Write(CByte(dataCount))
                            Exit Select
                        Case "2"
                            dataCount = br.ReadUInt16()
                            bw.Write(CUShort(dataCount))
                            Exit Select
                        Case "4"
                            dataCount = br.ReadUInt32()
                            bw.Write(CUInt(dataCount))
                            Exit Select
                        Case "8"
                            dataCount = br.ReadUInt64()
                            bw.Write(CULng(dataCount))
                            Exit Select
                    End Select

                    Dim value As UInt64 = Convert.ToUInt64(dataCount)

                    For i As Int64 = 0 To Convert.ToInt64(dataCount) - 1
                        list.Clear()
                        '#Region "read node"
                        For j As Int32 = 0 To format.Length - 1
                            Select Case format(j)
                                Case "1"
                                    value = br.ReadByte()
                                    list.Add(Convert.ToByte(value))
                                    Exit Select
                                Case "2"
                                    value = br.ReadUInt16()
                                    list.Add(Convert.ToUInt16(value))
                                    Exit Select
                                Case "4"
                                    value = br.ReadUInt32()
                                    list.Add(Convert.ToUInt32(value))
                                    Exit Select
                                Case "8"
                                    value = br.ReadUInt64()
                                    list.Add(Convert.ToUInt64(value))
                                    Exit Select
                                Case "len"
                                    Select Case format(System.Threading.Interlocked.Increment(j))
                                        Case "1"
                                            value = br.ReadByte()
                                            list.Add(Convert.ToByte(value))
                                            Exit Select
                                        Case "2"
                                            value = br.ReadUInt16()
                                            list.Add(Convert.ToUInt16(value))
                                            Exit Select
                                        Case "4"
                                            value = br.ReadUInt32()
                                            list.Add(Convert.ToUInt32(value))
                                            Exit Select
                                        Case "8"
                                            value = br.ReadUInt64()
                                            list.Add(Convert.ToUInt64(value))
                                            Exit Select
                                    End Select
                                    Dim strBytes(Convert.ToInt32(value * 2) - 1) As Byte
                                    list.Add(strBytes)
                                    For k As Integer = 0 To strBytes.Length - 1
                                        strBytes(k) = br.ReadByte()
                                    Next
                                    Exit Select
                            End Select
                        Next
                        '#End Region

                        '#Region "Process Node"
                        currentID = Convert.ToUInt64(list(idIndex))
                        If database.ContainsKey(currentID) Then
                            Dim lenCount As Integer = 0
                            currentDic = database(currentID)
                            If currentDic.Count = totalLen Then
                                For formatCou As Int32 = 0 To format.Length - 1
                                    If format(formatCou) = "len" Then
                                        formatCou += 1
                                        tmpString = currentDic(lenCount)
                                        If (tmpString <> "0") AndAlso (tmpString <> "null") Then
                                            list(formatCou) = Encoding.Unicode.GetBytes(currentDic(lenCount))
                                            Select Case format(formatCou)
                                                Case "1"
                                                    list(formatCou - 1) = Convert.ToByte(currentDic(lenCount).Length)
                                                    Exit Select
                                                Case "2"
                                                    list(formatCou - 1) = Convert.ToUInt16(currentDic(lenCount).Length)
                                                    Exit Select
                                                Case "4"
                                                    list(formatCou - 1) = Convert.ToUInt32(currentDic(lenCount).Length)
                                                    Exit Select
                                                Case "8"
                                                    list(formatCou - 1) = Convert.ToUInt64(currentDic(lenCount).Length)
                                                    Exit Select
                                            End Select
                                        End If
                                        'System.Windows.Forms.MessageBox.Show("ID: " + currentID.ToString() + "\nString: " + currentDic[lenCount] + "\nOriginal: " + System.Text.Encoding.Unicode.GetString((byte[])list[formatCou + 1]));
                                        lenCount += 1
                                    End If
                                Next
                            End If
                        End If
                        '#End Region

                        '#Region "write node"
                        For j As Int32 = 0 To format.Length - 1
                            Select Case format(j)
                                Case "1"
                                    value = Convert.ToByte(list(j))
                                    bw.Write(Convert.ToByte(value))
                                    Exit Select
                                Case "2"
                                    value = Convert.ToUInt16(list(j))
                                    bw.Write(Convert.ToUInt16(value))
                                    Exit Select
                                Case "4"
                                    value = Convert.ToUInt32(list(j))
                                    bw.Write(Convert.ToUInt32(value))
                                    Exit Select
                                Case "8"
                                    value = Convert.ToUInt64(list(j))
                                    bw.Write(Convert.ToUInt64(value))
                                    Exit Select
                                Case "len"
                                    Dim charBytes As [Byte]() = DirectCast(list(j + 1), Byte())
                                    value = Convert.ToUInt64(charBytes.Length / 2)

                                    Select Case format(System.Threading.Interlocked.Increment(j))
                                        Case "1"
                                            bw.Write(Convert.ToByte(value))
                                            Exit Select
                                        Case "2"
                                            bw.Write(Convert.ToUInt16(value))
                                            Exit Select
                                        Case "4"
                                            bw.Write(Convert.ToUInt32(value))
                                            Exit Select
                                        Case "8"
                                            bw.Write(Convert.ToUInt64(value))
                                            Exit Select
                                    End Select

                                    For Each b As [Byte] In charBytes
                                        dataSum += b
                                        bw.Write(b)
                                    Next

                                    Exit Select
                            End Select
                            dataSum += value
                            '#End Region
                        Next
                    Next
                    hashLength__1 = Convert.ToUInt16(HashLength)
                    bw.Write(hashLength__1)

                    Dim result() As Byte
                    Using myMD5 As MD5 = MD5.Create()
                        result = myMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(Convert.ToString(dataSum)))
                    End Using
                    Dim sb As New StringBuilder()
                    For n As Int32 = 0 To result.Length - 1
                        sb.Append(result(n).ToString("x2"))
                    Next

                    Dim hashString As String = sb.ToString()
                    sb.Clear()

                    For i As Int32 = 0 To hashString.Length - 1
                        hash(i) = Convert.ToByte(hashString(i))
                    Next

                    bw.Write(hash)
                End Using
            End Using
            currentDic = Nothing
            tmpString = Nothing
            database.Clear()
            list.Clear()
        End Sub
    End Class
End Namespace
