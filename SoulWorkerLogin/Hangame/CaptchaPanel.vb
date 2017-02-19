Public Class CaptchaPanel

    Private ready As Boolean

    Public Sub New(ByVal agent As Hangame.HangameAgent)

        ' This call is required by the designer.
        InitializeComponent()

        Me.CaptchaPictureBox1.Agent = agent
        Me.ready = False
        Me.Visible = False
        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Public ReadOnly Property CaptchaString() As String
        Get
            Return Me.TextBox1.Text()
        End Get
    End Property

    Public ReadOnly Property CaptchaID() As String
        Get
            Return Me.CaptchaPictureBox1.CaptchaID()
        End Get
    End Property

    Private Sub buttonCopy_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonCopy.Click
        CaptchaPictureBox1.CopyCaptchaImage()
    End Sub

    Private Sub CaptchaPictureBox1_CaptchaLoading(ByVal sender As Object, ByVal e As EventArgs) Handles CaptchaPictureBox1.CaptchaLoading
        ready = False
    End Sub

    Private Sub CaptchaPictureBox1_CaptchaLoaded(ByVal sender As Object, ByVal e As EventArgs) Handles CaptchaPictureBox1.CaptchaLoaded
        ready = True
    End Sub

    Public Sub ForceNewCaptcha()
        Me.ButtonReload.PerformClick()
    End Sub

    Private Sub buttonReload_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonReload.Click
        Me.CaptchaPictureBox1.ReloadCaptcha()
        Me.TextBox1.Text = String.Empty
    End Sub
End Class
