Imports System.Windows.Forms
Imports System.ComponentModel

Public Class DialogVarNamesSingle

    Dim _progNum As Integer = -1
    Dim _varNum As Integer = -1
    Dim _varName As String = ""

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property ProgNum As Integer
        Get
            Return _progNum
        End Get
        Set(value As Integer)
            _progNum = value
        End Set
    End Property
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property VarNum As Integer
        Get
            Return _varNum
        End Get
        Set(value As Integer)
            _varNum = value
            If _progNum = 0 Then
                Me.Text = "System "
            Else
                Me.Text = "Program "
            End If
            Me.Text &= "Variable " & value & " Name:"
        End Set
    End Property

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property VarName As String
        Get
            Return _varName
        End Get
        Set(value As String)
            _varName = value
            txtName.Text = value
        End Set
    End Property


    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me._varName = txtName.Text
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub DialogVarNames_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        txtName.Select()
        txtName.SelectAll()
    End Sub
End Class
