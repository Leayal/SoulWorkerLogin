Imports Leayal.Net
Namespace Classes.Github
    Public Class GithubUserInfoBox
        Private mywebClient As ExtendedWebClient
        Public ReadOnly Property Username() As String
        Public ReadOnly Property RealName() As String
        Public ReadOnly Property AvatarUri() As Uri
        Public ReadOnly Property ProfileUri() As Uri

        Public Shared Function Parse(ByVal reader As Newtonsoft.Json.JsonTextReader) As GithubUserInfoBox
            Parse = Nothing
            If (reader IsNot Nothing) Then
                Dim name As String = String.Empty
                Dim usename As String = String.Empty
                Dim pplink As String = String.Empty
                Dim plink As String = String.Empty
                While (reader.Read())
                    If (reader.TokenType = Newtonsoft.Json.JsonToken.PropertyName) Then
                        Select Case (DirectCast(reader.Value, String))
                            Case "login"
                                usename = reader.ReadAsString()
                            Case "name"
                                name = reader.ReadAsString()
                            Case "avatar_url"
                                pplink = reader.ReadAsString()
                            Case "html_url"
                                plink = reader.ReadAsString()
                            Case Else
                                reader.Skip()
                        End Select
                    End If
                End While
                If (String.IsNullOrEmpty(name)) Then
                    name = "(Unknown)"
                End If
                If (Not String.IsNullOrEmpty(usename)) AndAlso (Not String.IsNullOrEmpty(plink)) Then
                    If (String.IsNullOrEmpty(pplink)) Then
                        Parse = New GithubUserInfoBox(usename, name, New Uri(plink))
                    Else
                        Parse = New GithubUserInfoBox(usename, name, New Uri(plink), New Uri(pplink))
                    End If
                End If
            End If
        End Function

        Public Sub New(ByVal susername As String, ByVal name As String, ByVal uprofileUri As Uri)
            Me.New(susername, name, uprofileUri, Nothing)
        End Sub
        Public Sub New(ByVal susername As String, ByVal name As String, ByVal uprofileUri As Uri, ByVal profileAvatarUri As Uri)
            InitializeComponent()
            Me.Username = susername
            Me.RealName = name
            Me.LabelReal.Text = name
            Me.TableLayoutPanel1.RowStyles(1).Height = Me.LabelReal.Size.Height
            Me.LinkLabelNickname.Text = susername
            Me.TableLayoutPanel1.RowStyles(3).Height = Me.LinkLabelNickname.Size.Height
            Me.ProfileUri = uprofileUri
            If (profileAvatarUri IsNot Nothing) Then
                Me.AvatarUri = profileAvatarUri
                Me.mywebClient = New ExtendedWebClient()
                Me.mywebClient.UserAgent = DefineValues.Web.WebUserAgent
                AddHandler Me.mywebClient.DownloadDataCompleted, AddressOf mywebClient_DownloadDataCompleted
                Me.mywebClient.DownloadDataAsync(Me.AvatarUri)
            End If
        End Sub

        Private Sub mywebClient_DownloadDataCompleted(sender As Object, e As DownloadDataFinishedEventArgs)
            If (e.Error IsNot Nothing) Then
            Else
                If (e.Result IsNot Nothing) AndAlso (e.Result.Length > 0) Then
                    Using memStream As New IO.MemoryStream(e.Result, False)
                        memStream.Position = 0
                        Me.PictureBox1.Image = Image.FromStream(memStream)
                    End Using
                End If
            End If
        End Sub

        Private Sub LinkLabelNickname_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabelNickname.LinkClicked
            If (ProfileUri IsNot Nothing) Then
                Try
                    Process.Start(ProfileUri.AbsoluteUri)
                Catch ex As Exception
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Sub
    End Class
End Namespace