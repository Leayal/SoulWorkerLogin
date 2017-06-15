Imports System.IO
Imports Leayal

Module Module1
    Const HashLength As Integer = 32

    Public Sub Main(args As String())
        If (args.Length < 6) Then
            Console.WriteLine("Soul Worker *.res File Parser by Leayal (Original from Miyu)")
            Console.WriteLine("")
            Console.WriteLine("Usage: <table> <output> <IDindex> <bytenCount> [<byten> [<byten>...]]")
            Console.WriteLine("")
            Console.WriteLine("<table> should be the table *.res file")
            Console.WriteLine("<output> should be *.txt file which parsed result will be in")
            Console.WriteLine("         you can use new\*.res to create another directory")
            Console.WriteLine("<IDindex>  the arg index where the <byten> is the ID")
            Console.WriteLine("<bytenCount> define byten to be read at the beginning")
            Console.WriteLine("             of the file to determine how many rows")
            Console.WriteLine("<byten> each consists of the number of bytes for")
            Console.WriteLine("        each data type in the archive format")
            Console.WriteLine("        <byten> must be 1, 2, 4, 8 or len")
            Console.WriteLine("        number of files in archive is first <byten>")
            Console.WriteLine("        after len comes a <byten>")
            Console.WriteLine("")
            Console.WriteLine("Press any key to close the application.")
            Console.ReadKey(True)
        Else
            Dim theInputRes As String = args(0)
            If Not System.IO.File.Exists(theInputRes) Then
                writeLog("ERROR: Second Arg->.res is not found.")
                Console.WriteLine("ERROR: Second Arg->.res is not found.")
                Environment.Exit(-2)
            End If
            Dim theOutputRes As String = args(1)
            If String.IsNullOrWhiteSpace(theOutputRes) Then
                theOutputRes = System.IO.Path.ChangeExtension(theInputRes, ".txt")
            End If
            If (System.IO.Path.IsPathRooted(theOutputRes)) Then My.Computer.FileSystem.CreateDirectory(My.Computer.FileSystem.GetParentPath(theOutputRes))
            Dim tmp3 As String = args(2)
            Dim theIDIndex As Integer = 0
            If Not Integer.TryParse(tmp3, theIDIndex) Then
                writeLog("ERROR: Fourth Arg->Index is invalid. Must be a integer.")
                Console.WriteLine("ERROR: Fourth Arg->Index is invalid. Must be a integer.")
                Environment.[Exit](-4)
            End If
            Dim countByte As String = args(3)
            Dim sb As Text.StringBuilder = New Text.StringBuilder()
            For cou As Integer = 4 To args.Length - 1
                sb.Append(" " + args(cou))
            Next
            Dim theParam As String = sb.ToString()
            If Not String.IsNullOrWhiteSpace(theParam) Then
                theParam = theParam.Trim()
            Else
                If Not Integer.TryParse(tmp3, theIDIndex) Then
                    writeLog("ERROR: Fifth Arg->File format not found.")
                    Console.WriteLine("ERROR: Fifth Arg->File format not found.")
                    Environment.[Exit](-5)
                End If
            End If
            Try
                Dim datacount As UInt64 = ParseResource(theInputRes, theOutputRes, theIDIndex, countByte, theParam)
                Console.WriteLine(Convert.ToString((Convert.ToString(String.Format("SUCCESS: {0} rows parsed", datacount) & vbLf & "Input Resource: ") & theInputRes) + vbLf & "Output File: ") & theOutputRes)
                Environment.[Exit](1)
            Catch ex As Exception
                Try
                    System.IO.File.Delete(theOutputRes)
                Catch
                End Try
                writeLog("ERROR: While merging->" + ex.Message)
                Console.WriteLine("ERROR: While merging->" + ex.Message)
                Environment.[Exit](-6)
            End Try
        End If
    End Sub

    Public Sub writeLog(theText As String)
        Try
            Using theWriter As New System.IO.StreamWriter(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\swfru.log", True)
                theWriter.WriteLine(Convert.ToString("[" + System.DateTime.Now.ToString() + "]") & theText)
                theWriter.Flush()
            End Using
        Catch ex As Exception
            Console.WriteLine("ERROR: While write log->" + ex.Message)
        End Try
    End Sub

    Public Function ParseResource(originalFile As String, outputFile As String, idIndex As Integer, CountByte As String, formatstring As String) As UInt64
        '. . . 5    4    2 1 len 2
        '     id  count   format
        Dim di As SortedDictionary(Of UInt32, Object()) = New SortedDictionary(Of UInt32, Object())()
        Dim format As String() = formatstring.Split(" "c)

        Dim totalLen As Integer = 0
        For Each punpun As String In format
            If punpun.ToLower() = "len" Then
                totalLen += 1
            End If
        Next

        'Dim currentDic As List(Of String) = Nothing
        Dim list As New List(Of Object)()

        Dim dataCount As Object = 0

        Dim currentID As UInt64 = 0
        Using ofs = File.Create(outputFile)
            Using bw = New StreamWriter(ofs, System.Text.Encoding.Unicode)
                Using fs = File.OpenRead(originalFile)
                    Using br = New BinaryReader(fs, System.Text.Encoding.Unicode)
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
                                        list.Add(br.ReadByte())
                                        Exit Select
                                    Case "2"
                                        list.Add(br.ReadUInt16())
                                        Exit Select
                                    Case "4"
                                        list.Add(br.ReadUInt32())
                                        Exit Select
                                    Case "8"
                                        list.Add(br.ReadUInt64())
                                        Exit Select
                                    Case "len"
                                        Select Case format(System.Threading.Interlocked.Increment(j))
                                            Case "1"
                                                list.Add(br.ReadByte())
                                                Exit Select
                                            Case "2"
                                                list.Add(br.ReadUInt16())
                                                Exit Select
                                            Case "4"
                                                list.Add(br.ReadUInt32())
                                                Exit Select
                                            Case "8"
                                                list.Add(br.ReadUInt64())
                                                Exit Select
                                        End Select
                                        list.Add(New String(br.ReadChars(value)))
                                        Exit Select
                                End Select
                            Next
                            '#End Region

                            '#Region "Process Node"
                            For listCount As Integer = 0 To list.Count - 1
                                If (listCount = idIndex) Then
                                    bw.WriteLine(String.Format("[Index={0}]", list(listCount)))
                                Else
                                    If (TypeOf list(listCount) Is [Byte]) Then
                                        bw.WriteLine(String.Format("byte={0}", list(listCount)))
                                    ElseIf (TypeOf list(listCount) Is UInt16) Then
                                        bw.WriteLine(String.Format("ushort={0}", list(listCount)))
                                    ElseIf (TypeOf list(listCount) Is UInt32) Then
                                        bw.WriteLine(String.Format("uint={0}", list(listCount)))
                                    ElseIf (TypeOf list(listCount) Is UInt64) Then
                                        bw.WriteLine(String.Format("ulong={0}", list(listCount)))
                                    ElseIf (TypeOf list(listCount) Is [String]) Then
                                        bw.WriteLine(String.Format("string={0}", list(listCount)))
                                    ElseIf (TypeOf list(listCount) Is Byte()) Then
                                        Dim theByte As Byte() = DirectCast(list(listCount), Byte())
                                        bw.Write("string=")
                                        bw.WriteLine(bw.Encoding.GetString(theByte))
                                    Else
                                        bw.WriteLine(String.Format("UnknownType={0}", list(listCount)))
                                    End If
                                End If
                            Next
                            bw.WriteLine()

                            '#End Region
                        Next
                    End Using
                End Using
            End Using
        End Using
        list.Clear()
        Return Convert.ToUInt64(dataCount)
    End Function

End Module
