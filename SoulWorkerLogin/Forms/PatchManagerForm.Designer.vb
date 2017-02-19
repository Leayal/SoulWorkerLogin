<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PatchManagerForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Me.ButtonRebuild = New System.Windows.Forms.Button()
        Me.LabelStep = New System.Windows.Forms.Label()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ButtonInstall = New System.Windows.Forms.Button()
        Me.ButtonUninstall = New System.Windows.Forms.Button()
        Me.LabelStatus = New System.Windows.Forms.Label()
        Me.LabelVersionDate = New System.Windows.Forms.Label()
        Me.LinkLabel1 = New System.Windows.Forms.LinkLabel()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Panel1.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ButtonRebuild
        '
        Me.ButtonRebuild.Location = New System.Drawing.Point(196, 7)
        Me.ButtonRebuild.Name = "ButtonRebuild"
        Me.ButtonRebuild.Size = New System.Drawing.Size(126, 23)
        Me.ButtonRebuild.TabIndex = 0
        Me.ButtonRebuild.Text = "Force Rebuild Patch"
        Me.ButtonRebuild.UseVisualStyleBackColor = True
        '
        'LabelStep
        '
        Me.LabelStep.AutoSize = True
        Me.LabelStep.Location = New System.Drawing.Point(3, 0)
        Me.LabelStep.Name = "LabelStep"
        Me.LabelStep.Size = New System.Drawing.Size(42, 14)
        Me.LabelStep.TabIndex = 1
        Me.LabelStep.Text = "Label1"
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(0, 17)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(332, 16)
        Me.ProgressBar1.TabIndex = 2
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.LabelStep)
        Me.Panel1.Controls.Add(Me.ProgressBar1)
        Me.Panel1.Location = New System.Drawing.Point(12, 82)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(332, 38)
        Me.Panel1.TabIndex = 3
        Me.Panel1.Tag = ""
        Me.Panel1.Visible = False
        '
        'ButtonInstall
        '
        Me.ButtonInstall.Location = New System.Drawing.Point(32, 52)
        Me.ButtonInstall.Name = "ButtonInstall"
        Me.ButtonInstall.Size = New System.Drawing.Size(126, 23)
        Me.ButtonInstall.TabIndex = 4
        Me.ButtonInstall.Text = "Install"
        Me.ButtonInstall.UseVisualStyleBackColor = True
        '
        'ButtonUninstall
        '
        Me.ButtonUninstall.Location = New System.Drawing.Point(196, 52)
        Me.ButtonUninstall.Name = "ButtonUninstall"
        Me.ButtonUninstall.Size = New System.Drawing.Size(126, 23)
        Me.ButtonUninstall.TabIndex = 4
        Me.ButtonUninstall.Text = "Uninstall"
        Me.ButtonUninstall.UseVisualStyleBackColor = True
        '
        'LabelStatus
        '
        Me.LabelStatus.AutoSize = True
        Me.LabelStatus.Location = New System.Drawing.Point(9, 12)
        Me.LabelStatus.Name = "LabelStatus"
        Me.LabelStatus.Size = New System.Drawing.Size(119, 14)
        Me.LabelStatus.TabIndex = 5
        Me.LabelStatus.Text = "Status: Not Installed"
        '
        'LabelVersionDate
        '
        Me.LabelVersionDate.AutoSize = True
        Me.LabelVersionDate.Location = New System.Drawing.Point(9, 33)
        Me.LabelVersionDate.Name = "LabelVersionDate"
        Me.LabelVersionDate.Size = New System.Drawing.Size(187, 14)
        Me.LabelVersionDate.TabIndex = 5
        Me.LabelVersionDate.Text = "Version Date: 06/09/6969 69:69"
        '
        'LinkLabel1
        '
        Me.LinkLabel1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.LinkLabel1.AutoSize = True
        Me.TableLayoutPanel1.SetColumnSpan(Me.LinkLabel1, 2)
        Me.LinkLabel1.Location = New System.Drawing.Point(4, 1)
        Me.LinkLabel1.Name = "LinkLabel1"
        Me.LinkLabel1.Size = New System.Drawing.Size(281, 28)
        Me.LinkLabel1.TabIndex = 6
        Me.LinkLabel1.TabStop = True
        Me.LinkLabel1.Text = "The translation is from SoulWorkerHQ's translation team."
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.LinkLabel1, 0, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(32, 82)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(290, 30)
        Me.TableLayoutPanel1.TabIndex = 7
        '
        'PatchManagerForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(356, 124)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.LabelVersionDate)
        Me.Controls.Add(Me.LabelStatus)
        Me.Controls.Add(Me.ButtonUninstall)
        Me.Controls.Add(Me.ButtonInstall)
        Me.Controls.Add(Me.ButtonRebuild)
        Me.DoubleBuffered = True
        Me.Font = New System.Drawing.Font("Tahoma", 9.0!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "PatchManagerForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "PatchManager"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ButtonRebuild As Button
    Friend WithEvents LabelStep As Label
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents Panel1 As Panel
    Friend WithEvents ButtonInstall As Button
    Friend WithEvents ButtonUninstall As Button
    Friend WithEvents LabelStatus As Label
    Friend WithEvents LabelVersionDate As Label
    Friend WithEvents LinkLabel1 As LinkLabel
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
End Class
