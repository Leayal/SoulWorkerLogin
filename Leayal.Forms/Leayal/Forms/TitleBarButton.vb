Friend Class TitleBarButton
    Inherits Button

    Public Sub New()
        MyBase.New()
    End Sub

    Protected Overrides Sub OnPaint(pevent As PaintEventArgs)
        Dim asdasd = pevent.Graphics.BeginContainer()
        pevent.Graphics.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit
        MyBase.OnPaint(pevent)
        'TextRenderer.DrawText(pevent.Graphics, Me.Text, Me.Font, New System.Drawing.Point(1, 1), Me.ForeColor)
        pevent.Graphics.EndContainer(asdasd)
    End Sub
End Class
