Imports System
Imports System.Drawing

Namespace TheCodeKing.ActiveButtons.Controls.Themes
    Public Interface ITheme
        ReadOnly Property BackColor As Color

        ReadOnly Property ButtonOffset As Point

        ReadOnly Property ControlBoxSize As Size

        ReadOnly Property FrameBorder As Size

        ReadOnly Property IsDisplayed As Boolean

        ReadOnly Property SystemButtonSize As Size
    End Interface
End Namespace