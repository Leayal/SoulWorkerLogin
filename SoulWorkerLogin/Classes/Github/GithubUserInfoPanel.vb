Imports Leayal.Net
Imports System.IO

Namespace Classes.Github
    Public Class GithubUserInfoPanel
        Inherits System.Windows.Forms.FlowLayoutPanel
        Private WithEvents myWebClient As ExtendedWebClient
        Private dic As Dictionary(Of String, GithubUserInfoBox)
        Private queue As Concurrent.ConcurrentQueue(Of String)

        Public Sub New()
            Me.dic = New Dictionary(Of String, GithubUserInfoBox)()
            Me.queue = New Concurrent.ConcurrentQueue(Of String)()
            Me.myWebClient = New ExtendedWebClient()
            Me.myWebClient.UserAgent = DefineValues.Web.WebUserAgent
            Me.myWebClient.Headers.Set(Net.HttpRequestHeader.Accept, "application/vnd.github.v3+json")
        End Sub

        Private Sub myWebClient_DownloadStringCompleted(ByVal sender As Object, ByVal e As ExtendedWebClient.DownloadStringFinishedEventArgs) Handles myWebClient.DownloadStringCompleted
            If (e.Error IsNot Nothing) Then

            Else
                If (Not String.IsNullOrWhiteSpace(e.Result)) Then
                    Using sr As New StringReader(e.Result)
                        Using jsr As New Newtonsoft.Json.JsonTextReader(sr)
                            Dim theBox As GithubUserInfoBox = GithubUserInfoBox.Parse(jsr)
                            If theBox IsNot Nothing Then
                                Dim targetName As String = DirectCast(e.UserState, String)
                                If (Me.dic.ContainsKey(targetName)) Then
                                    Me.Controls.Add(theBox)
                                    Me.dic(targetName) = theBox
                                End If
                            End If
                        End Using
                    End Using
                End If
            End If
            Me.StartWorker()
        End Sub

        Public Function ContainsUser(ByVal username As String) As Boolean
            If Me.dic.ContainsKey(username) Then
                Return (Me.dic(username) Is Nothing)
            Else
                Return False
            End If
        End Function

        Public Sub AddUser(ByVal username As String)
            If (Not ContainsUser(username)) Then
                Me.dic.Add(username, Nothing)
                Me.queue.Enqueue(username)
            End If
            Me.StartWorker()
        End Sub

        Private Sub StartWorker()
            If (Not Me.myWebClient.IsBusy) AndAlso (Not Me.queue.IsEmpty()) Then
                Dim result As String = String.Empty
                If (Me.queue.TryDequeue(result)) Then
                    Me.myWebClient.DownloadStringAsync(New Uri(String.Format(DefineValues.Urls.GithubAPIUsers, result)), result)
                End If
            End If
        End Sub

        Public Shadows Sub Dispose()
            If (Me.myWebClient.IsBusy) Then
                Me.myWebClient.CancelAsync()
            End If
            Me.myWebClient.Dispose()
            Me.dic.Clear()
            Me.dic = Nothing
            MyBase.Dispose()
        End Sub
    End Class
End Namespace