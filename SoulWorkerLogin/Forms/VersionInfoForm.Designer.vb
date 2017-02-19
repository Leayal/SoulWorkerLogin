<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class VersionInfoForm
    Inherits System.Windows.Forms.Form

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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.LabelVersion = New System.Windows.Forms.Label()
        Me.ValueLabelVersion = New System.Windows.Forms.Label()
        Me.ValueLabelNewVersion = New System.Windows.Forms.Label()
        Me.ButtonUpdate = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.LabelVersion, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.ValueLabelVersion, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.ValueLabelNewVersion, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonUpdate, 0, 2)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(4)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(374, 97)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'LabelVersion
        '
        Me.LabelVersion.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.LabelVersion.AutoSize = True
        Me.LabelVersion.Location = New System.Drawing.Point(62, 6)
        Me.LabelVersion.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelVersion.Name = "LabelVersion"
        Me.LabelVersion.Size = New System.Drawing.Size(62, 19)
        Me.LabelVersion.TabIndex = 0
        Me.LabelVersion.Text = "Version"
        '
        'ValueLabelVersion
        '
        Me.ValueLabelVersion.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.ValueLabelVersion.AutoSize = True
        Me.ValueLabelVersion.Location = New System.Drawing.Point(280, 6)
        Me.ValueLabelVersion.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.ValueLabelVersion.Name = "ValueLabelVersion"
        Me.ValueLabelVersion.Size = New System.Drawing.Size(0, 19)
        Me.ValueLabelVersion.TabIndex = 0
        '
        'ValueLabelNewVersion
        '
        Me.ValueLabelNewVersion.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.ValueLabelNewVersion.AutoSize = True
        Me.TableLayoutPanel1.SetColumnSpan(Me.ValueLabelNewVersion, 2)
        Me.ValueLabelNewVersion.Location = New System.Drawing.Point(150, 38)
        Me.ValueLabelNewVersion.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.ValueLabelNewVersion.Name = "ValueLabelNewVersion"
        Me.ValueLabelNewVersion.Size = New System.Drawing.Size(73, 19)
        Me.ValueLabelNewVersion.TabIndex = 0
        Me.ValueLabelNewVersion.Text = "Checking"
        '
        'ButtonUpdate
        '
        Me.ButtonUpdate.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.TableLayoutPanel1.SetColumnSpan(Me.ButtonUpdate, 2)
        Me.ButtonUpdate.Font = New System.Drawing.Font("Tahoma", 9.0!)
        Me.ButtonUpdate.Location = New System.Drawing.Point(102, 68)
        Me.ButtonUpdate.Name = "ButtonUpdate"
        Me.ButtonUpdate.Size = New System.Drawing.Size(169, 24)
        Me.ButtonUpdate.TabIndex = 1
        Me.ButtonUpdate.Text = "Update"
        Me.ButtonUpdate.UseVisualStyleBackColor = True
        '
        'VersionInfoForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 19.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(374, 97)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Font = New System.Drawing.Font("Tahoma", 12.0!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "VersionInfoForm"
        Me.Text = "Version Info"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents LabelVersion As Label
    Friend WithEvents ValueLabelVersion As Label
    Friend WithEvents ValueLabelNewVersion As Label
    Friend WithEvents ButtonUpdate As Button
End Class
