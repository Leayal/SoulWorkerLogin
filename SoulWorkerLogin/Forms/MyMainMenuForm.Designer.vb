<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MyMainMenuForm
    Inherits Leayal.Forms.LeayalExtendedForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.ButtonLogin = New System.Windows.Forms.Button()
        Me.ButtonExit = New System.Windows.Forms.Button()
        Me.TextBox_ID = New System.Windows.Forms.TextBox()
        Me.PanelLogin = New System.Windows.Forms.Panel()
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Username = New System.Windows.Forms.Label()
        Me.TextBox_PW = New System.Windows.Forms.TextBox()
        Me.PanelPrepare = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.LabelProgresStep = New System.Windows.Forms.Label()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.PanelMain = New System.Windows.Forms.Panel()
        Me.ButtonManagePatch = New System.Windows.Forms.Button()
        Me.ButtonOption = New System.Windows.Forms.Button()
        Me.CheckBoxHideID = New System.Windows.Forms.CheckBox()
        Me.Label_LoggedInID = New System.Windows.Forms.Label()
        Me.ButtonLaunchReactor = New System.Windows.Forms.Button()
        Me.ButtonExit2 = New System.Windows.Forms.Button()
        Me.ButtonLogout = New System.Windows.Forms.Button()
        Me.GameStart = New System.Windows.Forms.Button()
        Me.PanelLoading = New System.Windows.Forms.PictureBox()
        Me.MyMainMenuPain = New System.Windows.Forms.Panel()
        Me.PanelLogin.SuspendLayout()
        Me.PanelPrepare.SuspendLayout()
        Me.PanelMain.SuspendLayout()
        CType(Me.PanelLoading, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MyMainMenuPain.SuspendLayout()
        Me.SuspendLayout()
        '
        'ButtonLogin
        '
        Me.ButtonLogin.Location = New System.Drawing.Point(124, 58)
        Me.ButtonLogin.Name = "ButtonLogin"
        Me.ButtonLogin.Size = New System.Drawing.Size(75, 23)
        Me.ButtonLogin.TabIndex = 1
        Me.ButtonLogin.TabStop = False
        Me.ButtonLogin.Text = "Login"
        Me.ButtonLogin.UseVisualStyleBackColor = True
        '
        'ButtonExit
        '
        Me.ButtonExit.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonExit.Location = New System.Drawing.Point(205, 58)
        Me.ButtonExit.Name = "ButtonExit"
        Me.ButtonExit.Size = New System.Drawing.Size(75, 23)
        Me.ButtonExit.TabIndex = 2
        Me.ButtonExit.TabStop = False
        Me.ButtonExit.Text = "Exit"
        Me.ButtonExit.UseVisualStyleBackColor = True
        '
        'TextBox_ID
        '
        Me.TextBox_ID.ForeColor = System.Drawing.Color.FromArgb(CType(CType(1, Byte), Integer), CType(CType(1, Byte), Integer), CType(CType(1, Byte), Integer))
        Me.TextBox_ID.Location = New System.Drawing.Point(78, 5)
        Me.TextBox_ID.Name = "TextBox_ID"
        Me.TextBox_ID.Size = New System.Drawing.Size(202, 21)
        Me.TextBox_ID.TabIndex = 3
        '
        'PanelLogin
        '
        Me.PanelLogin.Controls.Add(Me.CheckBox1)
        Me.PanelLogin.Controls.Add(Me.Label1)
        Me.PanelLogin.Controls.Add(Me.Username)
        Me.PanelLogin.Controls.Add(Me.ButtonExit)
        Me.PanelLogin.Controls.Add(Me.TextBox_PW)
        Me.PanelLogin.Controls.Add(Me.TextBox_ID)
        Me.PanelLogin.Controls.Add(Me.ButtonLogin)
        Me.PanelLogin.Location = New System.Drawing.Point(13, 12)
        Me.PanelLogin.Name = "PanelLogin"
        Me.PanelLogin.Size = New System.Drawing.Size(305, 89)
        Me.PanelLogin.TabIndex = 4
        Me.PanelLogin.Visible = False
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.Location = New System.Drawing.Point(20, 62)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(91, 17)
        Me.CheckBox1.TabIndex = 5
        Me.CheckBox1.TabStop = False
        Me.CheckBox1.Text = "Remember ID"
        Me.CheckBox1.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(19, 34)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(53, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Password"
        '
        'Username
        '
        Me.Username.AutoSize = True
        Me.Username.Location = New System.Drawing.Point(54, 8)
        Me.Username.Name = "Username"
        Me.Username.Size = New System.Drawing.Size(18, 13)
        Me.Username.TabIndex = 4
        Me.Username.Text = "ID"
        '
        'TextBox_PW
        '
        Me.TextBox_PW.ForeColor = System.Drawing.Color.FromArgb(CType(CType(1, Byte), Integer), CType(CType(1, Byte), Integer), CType(CType(1, Byte), Integer))
        Me.TextBox_PW.Location = New System.Drawing.Point(78, 31)
        Me.TextBox_PW.Name = "TextBox_PW"
        Me.TextBox_PW.Size = New System.Drawing.Size(202, 21)
        Me.TextBox_PW.TabIndex = 3
        Me.TextBox_PW.UseSystemPasswordChar = True
        '
        'PanelPrepare
        '
        Me.PanelPrepare.Controls.Add(Me.Label3)
        Me.PanelPrepare.Controls.Add(Me.LabelProgresStep)
        Me.PanelPrepare.Controls.Add(Me.ProgressBar1)
        Me.PanelPrepare.Location = New System.Drawing.Point(13, 12)
        Me.PanelPrepare.Name = "PanelPrepare"
        Me.PanelPrepare.Size = New System.Drawing.Size(305, 89)
        Me.PanelPrepare.TabIndex = 5
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(93, 8)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(126, 13)
        Me.Label3.TabIndex = 1
        Me.Label3.Text = "Preparing game launcher"
        '
        'LabelProgresStep
        '
        Me.LabelProgresStep.AutoSize = True
        Me.LabelProgresStep.Location = New System.Drawing.Point(3, 47)
        Me.LabelProgresStep.Name = "LabelProgresStep"
        Me.LabelProgresStep.Size = New System.Drawing.Size(151, 13)
        Me.LabelProgresStep.TabIndex = 1
        Me.LabelProgresStep.Text = "Checking for Reactor Updates"
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(3, 63)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(299, 23)
        Me.ProgressBar1.TabIndex = 0
        '
        'PanelMain
        '
        Me.PanelMain.Controls.Add(Me.ButtonManagePatch)
        Me.PanelMain.Controls.Add(Me.ButtonOption)
        Me.PanelMain.Controls.Add(Me.CheckBoxHideID)
        Me.PanelMain.Controls.Add(Me.Label_LoggedInID)
        Me.PanelMain.Controls.Add(Me.ButtonLaunchReactor)
        Me.PanelMain.Controls.Add(Me.ButtonExit2)
        Me.PanelMain.Controls.Add(Me.ButtonLogout)
        Me.PanelMain.Controls.Add(Me.GameStart)
        Me.PanelMain.Location = New System.Drawing.Point(2, 2)
        Me.PanelMain.Name = "PanelMain"
        Me.PanelMain.Size = New System.Drawing.Size(330, 107)
        Me.PanelMain.TabIndex = 5
        Me.PanelMain.Visible = False
        '
        'ButtonManagePatch
        '
        Me.ButtonManagePatch.Location = New System.Drawing.Point(3, 55)
        Me.ButtonManagePatch.Name = "ButtonManagePatch"
        Me.ButtonManagePatch.Size = New System.Drawing.Size(324, 23)
        Me.ButtonManagePatch.TabIndex = 9
        Me.ButtonManagePatch.Text = "Mange Patches"
        Me.ButtonManagePatch.UseVisualStyleBackColor = True
        '
        'ButtonOption
        '
        Me.ButtonOption.Location = New System.Drawing.Point(128, 81)
        Me.ButtonOption.Name = "ButtonOption"
        Me.ButtonOption.Size = New System.Drawing.Size(75, 23)
        Me.ButtonOption.TabIndex = 6
        Me.ButtonOption.Text = "Option"
        Me.ButtonOption.UseVisualStyleBackColor = True
        '
        'CheckBoxHideID
        '
        Me.CheckBoxHideID.AutoSize = True
        Me.CheckBoxHideID.Location = New System.Drawing.Point(266, 3)
        Me.CheckBoxHideID.Name = "CheckBoxHideID"
        Me.CheckBoxHideID.Size = New System.Drawing.Size(61, 17)
        Me.CheckBoxHideID.TabIndex = 8
        Me.CheckBoxHideID.Text = "Hide ID"
        Me.CheckBoxHideID.UseVisualStyleBackColor = True
        '
        'Label_LoggedInID
        '
        Me.Label_LoggedInID.AutoSize = True
        Me.Label_LoggedInID.Location = New System.Drawing.Point(3, 3)
        Me.Label_LoggedInID.Name = "Label_LoggedInID"
        Me.Label_LoggedInID.Size = New System.Drawing.Size(25, 13)
        Me.Label_LoggedInID.TabIndex = 7
        Me.Label_LoggedInID.Text = "ID: "
        '
        'ButtonLaunchReactor
        '
        Me.ButtonLaunchReactor.Font = New System.Drawing.Font("Tahoma", 7.0!)
        Me.ButtonLaunchReactor.Location = New System.Drawing.Point(168, 19)
        Me.ButtonLaunchReactor.Name = "ButtonLaunchReactor"
        Me.ButtonLaunchReactor.Size = New System.Drawing.Size(159, 33)
        Me.ButtonLaunchReactor.TabIndex = 6
        Me.ButtonLaunchReactor.Text = "Launch GameLauncher" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(Reactor Launcher)"
        Me.ButtonLaunchReactor.UseVisualStyleBackColor = True
        '
        'ButtonExit2
        '
        Me.ButtonExit2.Location = New System.Drawing.Point(252, 81)
        Me.ButtonExit2.Name = "ButtonExit2"
        Me.ButtonExit2.Size = New System.Drawing.Size(75, 23)
        Me.ButtonExit2.TabIndex = 5
        Me.ButtonExit2.Text = "Exit"
        Me.ButtonExit2.UseVisualStyleBackColor = True
        '
        'ButtonLogout
        '
        Me.ButtonLogout.Location = New System.Drawing.Point(3, 81)
        Me.ButtonLogout.Name = "ButtonLogout"
        Me.ButtonLogout.Size = New System.Drawing.Size(75, 23)
        Me.ButtonLogout.TabIndex = 5
        Me.ButtonLogout.Text = "Logout"
        Me.ButtonLogout.UseVisualStyleBackColor = True
        '
        'GameStart
        '
        Me.GameStart.Location = New System.Drawing.Point(3, 19)
        Me.GameStart.Name = "GameStart"
        Me.GameStart.Size = New System.Drawing.Size(159, 33)
        Me.GameStart.TabIndex = 5
        Me.GameStart.Text = "Launch game"
        Me.GameStart.UseVisualStyleBackColor = True
        '
        'PanelLoading
        '
        Me.PanelLoading.BackColor = System.Drawing.Color.Transparent
        Me.PanelLoading.Image = Global.SoulWorkerLogin.My.Resources.Resources.Loading
        Me.PanelLoading.Location = New System.Drawing.Point(116, 7)
        Me.PanelLoading.Name = "PanelLoading"
        Me.PanelLoading.Size = New System.Drawing.Size(100, 100)
        Me.PanelLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.PanelLoading.TabIndex = 0
        Me.PanelLoading.TabStop = False
        Me.PanelLoading.Visible = False
        '
        'MyMainMenuPain
        '
        Me.MyMainMenuPain.BackColor = System.Drawing.SystemColors.Control
        Me.MyMainMenuPain.Controls.Add(Me.PanelLoading)
        Me.MyMainMenuPain.Controls.Add(Me.PanelMain)
        Me.MyMainMenuPain.Controls.Add(Me.PanelLogin)
        Me.MyMainMenuPain.Controls.Add(Me.PanelPrepare)
        Me.MyMainMenuPain.Location = New System.Drawing.Point(5, 5)
        Me.MyMainMenuPain.Name = "MyMainMenuPain"
        Me.MyMainMenuPain.Size = New System.Drawing.Size(332, 110)
        Me.MyMainMenuPain.TabIndex = 6
        '
        'MyMainMenuForm
        '
        Me.AcceptButton = Me.ButtonLogin
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Lavender
        Me.CancelButton = Me.ButtonExit
        Me.ClientSize = New System.Drawing.Size(345, 122)
        Me.Controls.Add(Me.MyMainMenuPain)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!)
        Me.ForeColor = System.Drawing.Color.FromArgb(CType(CType(1, Byte), Integer), CType(CType(1, Byte), Integer), CType(CType(1, Byte), Integer))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimumSize = New System.Drawing.Size(342, 120)
        Me.Name = "MyMainMenuForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "SoulWorker WebClient Lite"
        Me.TransparencyKey = System.Drawing.Color.Lavender
        Me.PanelLogin.ResumeLayout(False)
        Me.PanelLogin.PerformLayout()
        Me.PanelPrepare.ResumeLayout(False)
        Me.PanelPrepare.PerformLayout()
        Me.PanelMain.ResumeLayout(False)
        Me.PanelMain.PerformLayout()
        CType(Me.PanelLoading, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MyMainMenuPain.ResumeLayout(False)
        Me.MyMainMenuPain.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ButtonLogin As Button
    Friend WithEvents ButtonExit As Button
    Friend WithEvents TextBox_ID As TextBox
    Friend WithEvents PanelLogin As Panel
    Friend WithEvents Username As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents TextBox_PW As TextBox
    Friend WithEvents PanelPrepare As Panel
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents Label3 As Label
    Friend WithEvents LabelProgresStep As Label
    Friend WithEvents PanelMain As Panel
    Friend WithEvents ButtonExit2 As Button
    Friend WithEvents ButtonLogout As Button
    Friend WithEvents GameStart As Button
    Friend WithEvents CheckBox1 As CheckBox
    Friend WithEvents ButtonLaunchReactor As Button
    Friend WithEvents PanelLoading As PictureBox
    Friend WithEvents Label_LoggedInID As Label
    Friend WithEvents CheckBoxHideID As CheckBox
    Friend WithEvents ButtonOption As Button
    Friend WithEvents ButtonManagePatch As Button
    Friend WithEvents MyMainMenuPain As Panel
End Class
