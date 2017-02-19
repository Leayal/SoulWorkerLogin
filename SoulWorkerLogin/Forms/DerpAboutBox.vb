Public NotInheritable Class DerpAboutBox

    Private Sub AboutBox1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Text = "About " & My.Application.Info.AssemblyName
        Me.Icon = My.Resources.haru_sd_WM1UAm_256px
        GithubUserInfoPanel1.AddUser("Leayal")
        GithubUserInfoPanel1.AddUser("Elizthe")
        GithubUserInfoPanel1.AddUser("HakubaWhite")
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub
End Class
