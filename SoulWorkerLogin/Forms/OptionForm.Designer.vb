<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class OptionForm
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
        Me.CheckBoxAllowCheckpointLaunchingSW = New System.Windows.Forms.CheckBox()
        Me.ButtonOpenUpdaterForm = New System.Windows.Forms.Button()
        Me.CheckBoxCheckUpdateStartup = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'CheckBoxAllowCheckpointLaunchingSW
        '
        Me.CheckBoxAllowCheckpointLaunchingSW.AutoSize = True
        Me.CheckBoxAllowCheckpointLaunchingSW.Location = New System.Drawing.Point(12, 12)
        Me.CheckBoxAllowCheckpointLaunchingSW.Name = "CheckBoxAllowCheckpointLaunchingSW"
        Me.CheckBoxAllowCheckpointLaunchingSW.Size = New System.Drawing.Size(238, 17)
        Me.CheckBoxAllowCheckpointLaunchingSW.TabIndex = 0
        Me.CheckBoxAllowCheckpointLaunchingSW.Text = "Allow checkpoint while launching SoulWorker"
        Me.CheckBoxAllowCheckpointLaunchingSW.UseVisualStyleBackColor = True
        '
        'ButtonOpenUpdaterForm
        '
        Me.ButtonOpenUpdaterForm.Location = New System.Drawing.Point(12, 59)
        Me.ButtonOpenUpdaterForm.Name = "ButtonOpenUpdaterForm"
        Me.ButtonOpenUpdaterForm.Size = New System.Drawing.Size(238, 23)
        Me.ButtonOpenUpdaterForm.TabIndex = 1
        Me.ButtonOpenUpdaterForm.Text = "Check for Updates"
        Me.ButtonOpenUpdaterForm.UseVisualStyleBackColor = True
        '
        'CheckBoxCheckUpdateStartup
        '
        Me.CheckBoxCheckUpdateStartup.AutoSize = True
        Me.CheckBoxCheckUpdateStartup.Location = New System.Drawing.Point(12, 35)
        Me.CheckBoxCheckUpdateStartup.Name = "CheckBoxCheckUpdateStartup"
        Me.CheckBoxCheckUpdateStartup.Size = New System.Drawing.Size(166, 17)
        Me.CheckBoxCheckUpdateStartup.TabIndex = 2
        Me.CheckBoxCheckUpdateStartup.Text = "Check for Updates at startup"
        Me.CheckBoxCheckUpdateStartup.UseVisualStyleBackColor = True
        '
        'OptionForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(265, 94)
        Me.Controls.Add(Me.CheckBoxCheckUpdateStartup)
        Me.Controls.Add(Me.ButtonOpenUpdaterForm)
        Me.Controls.Add(Me.CheckBoxAllowCheckpointLaunchingSW)
        Me.DoubleBuffered = True
        Me.Font = New System.Drawing.Font("Tahoma", 8.0!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "OptionForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Option"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents CheckBoxAllowCheckpointLaunchingSW As CheckBox
    Friend WithEvents ButtonOpenUpdaterForm As Button
    Friend WithEvents CheckBoxCheckUpdateStartup As CheckBox
End Class
