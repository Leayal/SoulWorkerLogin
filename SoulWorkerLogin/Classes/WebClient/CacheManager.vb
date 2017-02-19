Imports System.Globalization
Imports System.IO
Imports Ionic.Zip

Namespace Classes.WebClient.Cache
    Public Class CacheManager
        'Const timeFolder As String = "Time"
        'Const dataFolder As String = "Data"
        Dim innerDict As Dictionary(Of String, FileInfo)
        Dim mydirectoryinfo As DirectoryInfo
        Public ReadOnly Property StoragePath() As String
            Get
                Return Me.mydirectoryinfo.FullName
            End Get
        End Property
        Public Sub New(ByVal localstorage As String)
            Me.mydirectoryinfo = Directory.CreateDirectory(localstorage)
            Me.innerDict = New Dictionary(Of String, FileInfo)()
        End Sub

        Public Sub ReadFolder()
            Dim myFound() As FileInfo = Me.mydirectoryinfo.GetFiles("*.cache", SearchOption.TopDirectoryOnly)
            If (myFound IsNot Nothing) AndAlso (myFound.Length > 0) Then
                For i As Integer = 0 To myFound.Length - 1
                    Me.innerDict.Add(myFound(i).Name.ToUpper(), myFound(i))
                Next
            End If
        End Sub

        Public Overloads Function ContainsEntry(ByVal address As Uri) As Boolean
            Return Me.ContainsEntry((StringToSHA1(address) & ".cache").ToUpper())
        End Function

        Private Overloads Function ContainsEntry(ByVal id As String) As Boolean
            Return EnsureGetEntry(id).Exists()
        End Function

        Public Overloads Sub Purge(ByVal address As Uri)
            Me.Purge((StringToSHA1(address) & ".cache").ToUpper())
        End Sub

        Private Overloads Sub Purge(ByVal id As String)
            EnsureGetEntry(id).Delete()
        End Sub

        Public Overloads Function GetCacheTime(ByVal address As Uri) As Date?
            Return Me.GetCacheTime((StringToSHA1(address) & ".cache").ToUpper())
        End Function

        Private Overloads Function GetCacheTime(ByVal myID As String) As Date?
            If (ContainsEntry(myID)) Then
                Return Me.innerDict(myID).LastWriteTimeUtc
            End If
        End Function

        Public Function Read(ByVal address As Uri, ByVal accessDate As Date) As Ionic.Zip.ZipFile
            Read = Nothing
            Dim myID As String = (StringToSHA1(address) & ".cache").ToUpper()
            If (Me.ContainsEntry(myID)) Then
                Dim cacheDate As Date? = GetCacheTime(myID)
                If (cacheDate IsNot Nothing) Then
                    If (cacheDate = accessDate) Then
                        Dim derpHerp As String = Path.Combine(Me.StoragePath, myID)
                        If (ZipFile.IsZipFile(derpHerp, False)) Then
                            Read = ZipFile.Read(derpHerp)
                        End If
                    End If
                End If
            End If
        End Function

        Public Sub Write(ByVal address As Uri, ByVal bytes() As Byte, ByVal dt As Date)
            Dim myID As String = (StringToSHA1(address) & ".cache").ToUpper()
            Dim sFileInfo As FileInfo = EnsureGetEntry(myID)
            sFileInfo.Delete()
            Using archive As New ZipFile(sFileInfo.FullName)
                archive.AddEntry("0", bytes)
                archive.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression
                archive.Save()
            End Using
            sFileInfo.LastWriteTimeUtc = dt
        End Sub

        Public Sub Write(ByVal address As Uri, ByVal src As Stream, ByVal dt As Date)
            Dim myID As String = (StringToSHA1(address) & ".cache").ToUpper()
            Dim sFileInfo As FileInfo = EnsureGetEntry(myID)
            sFileInfo.Delete()
            Using archive As New ZipFile(sFileInfo.FullName)
                archive.AddEntry("0", src)
                archive.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression
                archive.Save()
            End Using
            sFileInfo.LastWriteTimeUtc = dt
        End Sub

        Public Sub PurgeAllCache()
            CommonMethods.EmptyFolder(Me.mydirectoryinfo.FullName)
        End Sub

        Private Overloads Function StringToSHA1(ByVal srcUri As Uri) As String
            Return StringToSHA1(srcUri.AbsoluteUri)
        End Function

        Private Overloads Function StringToSHA1(ByVal srcString As String) As String
            Dim theBytes() As Byte = Text.Encoding.UTF8.GetBytes(srcString)
            Dim theStringBuilder As New Text.StringBuilder()
            Using mysha1 = System.Security.Cryptography.SHA1.Create()
                theBytes = mysha1.ComputeHash(theBytes)
            End Using
            For i As Integer = 0 To theBytes.Length - 1
                theStringBuilder.Append(theBytes(i).ToString("x2"))
            Next
            Return theStringBuilder.ToString()
        End Function

        Private Overloads Function EnsureGetEntry(ByVal id As String) As FileInfo
            If (Me.innerDict.ContainsKey(id)) Then
                Return Me.innerDict(id)
            Else
                Dim dun As New FileInfo(Path.Combine(Me.StoragePath, id))
                Me.innerDict.Add(id, dun)
                Return dun
            End If
        End Function
    End Class
End Namespace
