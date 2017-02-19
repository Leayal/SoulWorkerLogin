Imports System.IO
Imports System.Text
Imports System.Security.Cryptography

Module Module1
    Const HashLength As Integer = 32
    Sub Main()
        Dim args() As String = Environment.GetCommandLineArgs()
        Dim originalFile As String = String.Empty
        For i As Integer = 0 To args.Length - 1
            If (args(i).EndsWith(".res")) Then
                originalFile = args(i)
            End If
        Next

        If (File.Exists(originalFile)) Then

            'ParseResource(originalFile, Path.ChangeExtension(originalFile, ".txt"))
        End If
    End Sub

    Public Sub ParseResource(originalFile As String, outputFile As String, idIndex As Integer, CountByte As String, formatstring As String)
        '. . . 5    4    2 1 len 2
        '     id  count   format
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

        Using bw = New StreamWriter(outputFile)
            Using fs = File.OpenRead(originalFile)
                Using br = New BinaryReader(fs)
                    Select Case CountByte
                        Case "1"
                            dataCount = br.ReadByte()
                            Exit Select
                        Case "2"
                            dataCount = br.ReadUInt16()
                            Exit Select
                        Case "4"
                            dataCount = br.ReadUInt32()
                            Exit Select
                        Case "8"
                            dataCount = br.ReadUInt64()
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

                        '#Region "write node"
                        For j As Int32 = 0 To format.Length - 1
                            If (j = idIndex) Then
                                bw.WriteLine(String.Format("ID={0}", format(idIndex)))
                            End If
                            Select Case format(j)
                                Case "len"
                                    Dim charBytes As [Byte]() = DirectCast(list(j + 1), Byte())
                                    value = Convert.ToUInt64(charBytes.Length / 2)
                                    For Each b As [Byte] In charBytes
                                        dataSum += b
                                        bw.Write(b)
                                    Next
                                    bw.WriteLine()
                                    Exit Select
                            End Select
                            bw.WriteLine()
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
        End Using
            currentDic = Nothing
        tmpString = Nothing
        list.Clear()
    End Sub

End Module
