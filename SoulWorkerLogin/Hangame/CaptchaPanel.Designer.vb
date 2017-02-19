<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CaptchaPanel
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.ButtonCopy = New System.Windows.Forms.Button()
        Me.ButtonReload = New System.Windows.Forms.Button()
        Me.CaptchaPictureBox1 = New SoulWorkerLogin.Hangame.CaptchaPictureBox()
        CType(Me.CaptchaPictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TextBox1
        '
        Me.TextBox1.AcceptsReturn = True
        Me.TextBox1.AcceptsTab = True
        Me.TextBox1.AllowDrop = True
        Me.TextBox1.Font = New System.Drawing.Font("Tahoma", 8.25!)
        Me.TextBox1.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.TextBox1.Location = New System.Drawing.Point(3, 70)
        Me.TextBox1.MaxLength = 10
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(203, 21)
        Me.TextBox1.TabIndex = 3
        Me.TextBox1.WordWrap = False
        '
        'ButtonCopy
        '
        Me.ButtonCopy.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonCopy.Image = Global.SoulWorkerLogin.My.Resources.Resources._20131104_527708869c600
        Me.ButtonCopy.Location = New System.Drawing.Point(212, 53)
        Me.ButtonCopy.Name = "ButtonCopy"
        Me.ButtonCopy.Size = New System.Drawing.Size(56, 38)
        Me.ButtonCopy.TabIndex = 1
        Me.ButtonCopy.TabStop = False
        Me.ButtonCopy.UseVisualStyleBackColor = True
        '
        'ButtonReload
        '
        Me.ButtonReload.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonReload.Image = Global.SoulWorkerLogin.My.Resources.Resources._30587_74468_refresh_reload
        Me.ButtonReload.Location = New System.Drawing.Point(212, 3)
        Me.ButtonReload.Name = "ButtonReload"
        Me.ButtonReload.Size = New System.Drawing.Size(56, 44)
        Me.ButtonReload.TabIndex = 0
        Me.ButtonReload.TabStop = False
        Me.ButtonReload.UseVisualStyleBackColor = True
        '
        'CaptchaPictureBox1
        '
        Me.CaptchaPictureBox1.Agent = Nothing
        Me.CaptchaPictureBox1.Location = New System.Drawing.Point(3, 3)
        Me.CaptchaPictureBox1.Name = "CaptchaPictureBox1"
        Me.CaptchaPictureBox1.Size = New System.Drawing.Size(203, 64)
        Me.CaptchaPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.CaptchaPictureBox1.TabIndex = 2
        Me.CaptchaPictureBox1.TabStop = False
        '
        'CaptchaPanel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.CaptchaPictureBox1)
        Me.Controls.Add(Me.ButtonCopy)
        Me.Controls.Add(Me.ButtonReload)
        Me.DoubleBuffered = True
        Me.Name = "CaptchaPanel"
        Me.Size = New System.Drawing.Size(269, 94)
        CType(Me.CaptchaPictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ButtonReload As Button
    Friend WithEvents ButtonCopy As Button
    Friend WithEvents CaptchaPictureBox1 As Hangame.CaptchaPictureBox
    Friend WithEvents TextBox1 As TextBox
End Class
