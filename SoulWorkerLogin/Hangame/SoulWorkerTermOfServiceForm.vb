Imports System.ComponentModel
Imports SoulWorkerLogin.Hangame

Public Class SoulWorkerTermOfServiceForm

    Private myAgent As HangameAgent
    Private WithEvents bWorker As BackgroundWorker
    Private thePicPoc As PictureBox

    Public Sub New(ByVal agent As HangameAgent)
        Me.myAgent = agent
        ' This call is required by the designer.
        InitializeComponent()
        Me.bWorker = New BackgroundWorker()
        Me.bWorker.WorkerReportsProgress = False
        Me.bWorker.WorkerSupportsCancellation = False
        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub SoulWorkerTermOfServiceForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        thePicPoc = New PictureBox()
        thePicPoc.SizeMode = PictureBoxSizeMode.AutoSize
        thePicPoc.Image = My.Resources.Loading
        Dim theX As Integer = Convert.ToInt32(Me.Size.Width / 2)
        Dim theY As Integer = Convert.ToInt32(Me.Size.Height / 2)
        Dim sizeX As Integer = Convert.ToInt32(thePicPoc.Size.Width / 2)
        Dim sizeY As Integer = Convert.ToInt32(thePicPoc.Size.Height / 2)
        thePicPoc.Location = New Point(theX - sizeX, theY - sizeY)
        Me.Controls.Add(thePicPoc)
        Me.bWorker.RunWorkerAsync()
    End Sub

    Private Sub bWorker_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles bWorker.DoWork
        Dim theString As String = Me.myAgent.GetTermOfService()
        If (String.IsNullOrEmpty(theString)) Then
            Throw New System.Net.WebException("Failed to request Term of Service")
        Else
            e.Result = theString
        End If
    End Sub

    Private Sub bWorker_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles bWorker.RunWorkerCompleted
        If (e.Error IsNot Nothing) Then
            MessageBox.Show(e.Error.Message & vbNewLine & e.Error.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.Close()
        Else
            AddControls(DirectCast(e.Result, String))
        End If
    End Sub

    Private Sub AddControls(ByVal HtmlString As String)
        Me.WebBrowser1.LockNavigate = False
        If (Me.WebBrowser1.Document Is Nothing) Then
            Me.WebBrowser1.DocumentText = HtmlString
        Else
            Me.WebBrowser1.Document.OpenNew(True)
            Me.WebBrowser1.Document.Write(HtmlString)
        End If
        Me.WebBrowser1.LockNavigate = True
        thePicPoc.Visible = False
        TableLayoutPanel1.Visible = True
    End Sub

    Private Sub WebBrowser1_LockedNavigate(ByVal sender As Object, ByVal e As WebBrowserNavigatedEventArgs) Handles WebBrowser1.LockedNavigate
        Try
            Process.Start(e.Url.OriginalString)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ButtonAgree_Click(sender As Object, e As EventArgs) Handles ButtonAgree.Click
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub ButtonDisagree_Click(sender As Object, e As EventArgs) Handles ButtonDisagree.Click
        Me.Close()
    End Sub
End Class