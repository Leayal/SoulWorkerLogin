Namespace Classes.Github
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class GithubUserInfoBox
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
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
            Me.PictureBox1 = New System.Windows.Forms.PictureBox()
            Me.LabelReal = New System.Windows.Forms.Label()
            Me.LinkLabelNickname = New System.Windows.Forms.LinkLabel()
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
            Me.Label2 = New System.Windows.Forms.Label()
            CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.TableLayoutPanel1.SuspendLayout()
            Me.SuspendLayout()
            '
            'PictureBox1
            '
            Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.PictureBox1.Location = New System.Drawing.Point(3, 3)
            Me.PictureBox1.Name = "PictureBox1"
            Me.PictureBox1.Size = New System.Drawing.Size(99, 94)
            Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            Me.PictureBox1.TabIndex = 0
            Me.PictureBox1.TabStop = False
            '
            'LabelReal
            '
            Me.LabelReal.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.LabelReal.AutoSize = True
            Me.LabelReal.Location = New System.Drawing.Point(27, 100)
            Me.LabelReal.Name = "LabelReal"
            Me.LabelReal.Size = New System.Drawing.Size(51, 13)
            Me.LabelReal.TabIndex = 1
            Me.LabelReal.Text = "realname"
            '
            'LinkLabelNickname
            '
            Me.LinkLabelNickname.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.LinkLabelNickname.AutoSize = True
            Me.LinkLabelNickname.Location = New System.Drawing.Point(24, 127)
            Me.LinkLabelNickname.Name = "LinkLabelNickname"
            Me.LinkLabelNickname.Size = New System.Drawing.Size(56, 13)
            Me.LinkLabelNickname.TabIndex = 2
            Me.LinkLabelNickname.TabStop = True
            Me.LinkLabelNickname.Text = "LinkLabel1"
            '
            'TableLayoutPanel1
            '
            Me.TableLayoutPanel1.ColumnCount = 1
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.TableLayoutPanel1.Controls.Add(Me.LinkLabelNickname, 0, 3)
            Me.TableLayoutPanel1.Controls.Add(Me.PictureBox1, 0, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.LabelReal, 0, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.Label2, 0, 2)
            Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            Me.TableLayoutPanel1.RowCount = 4
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100.0!))
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 13.0!))
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 13.0!))
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 13.0!))
            Me.TableLayoutPanel1.Size = New System.Drawing.Size(105, 141)
            Me.TableLayoutPanel1.TabIndex = 3
            '
            'Label2
            '
            Me.Label2.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.Label2.AutoSize = True
            Me.Label2.Location = New System.Drawing.Point(21, 113)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(62, 13)
            Me.Label2.TabIndex = 1
            Me.Label2.Text = "Codename:"
            '
            'GithubUserInfoBox
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.Font = New System.Drawing.Font("Tahoma", 8.25!)
            Me.Name = "GithubUserInfoBox"
            Me.Size = New System.Drawing.Size(105, 141)
            CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.TableLayoutPanel1.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents PictureBox1 As PictureBox
        Friend WithEvents LabelReal As Label
        Friend WithEvents LinkLabelNickname As LinkLabel
        Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
        Friend WithEvents Label2 As Label
    End Class
End Namespace
