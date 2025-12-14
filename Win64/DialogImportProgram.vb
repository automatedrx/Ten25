Imports System.Windows.Forms
Imports System.ComponentModel

Public Class dialogImportProgram


    Dim _tarProjectName As String = ""
    Dim _tarIndex As Integer = -1
    Dim _tarList As List(Of Program)

    Dim _srcProjectName As String = ""
    Dim _srcIndex As Integer = -1
    Dim _srcList As List(Of Program)

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property TargetProjName As String
        Get
            Return _tarProjectName
        End Get
        Set(value As String)
            _tarProjectName = value
            lblTargetName.Text = value
        End Set
    End Property
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property TargetIndex As Integer
        Get
            Return _tarIndex
        End Get
        Set(value As Integer)
            _tarIndex = value
        End Set
    End Property
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Friend Property TargetList As List(Of Program)
        Get
            Return _tarList
        End Get
        Set(value As List(Of Program))
            _tarList = value

            lstTarget.Items.Clear()
            For n As Integer = 0 To _tarList.Count - 1
                lstTarget.Items.Add(_tarList(n).progName)
            Next
            lstTarget.Items.Add("(Add to End)")
        End Set
    End Property


    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property SourceProjName As String
        Get
            Return _srcProjectName
        End Get
        Set(value As String)
            _srcProjectName = value
            Me.lblSourceName.Text = value
        End Set
    End Property
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property SourceIndex As Integer
        Get
            Return _srcIndex
        End Get
        Set(value As Integer)
            _srcIndex = value
        End Set
    End Property
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Friend Property SourceList As List(Of Program)
        Get
            Return _srcList
        End Get
        Set(value As List(Of Program))
            _srcList = value

            lstSource.Items.Clear()
            For n As Integer = 0 To _srcList.Count - 1
                lstSource.Items.Add(_srcList(n).progName)
            Next

        End Set
    End Property

    Friend Function ShowWithData(Target_List As List(Of Program), Target_Name As String, Source_List As List(Of Program), Source_Name As String) As MsgBoxResult
        Me.TargetList = Target_List
        Me.TargetProjName = Target_Name
        Me._tarIndex = -1

        Me.SourceList = Source_List
        Me.SourceProjName = Source_Name
        Me._srcIndex = -1

        Return Me.ShowDialog
    End Function

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub lstSource_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstSource.SelectedIndexChanged
        Me._srcIndex = lstSource.SelectedIndex
        If (_srcIndex >= 0) And (_tarIndex >= 0) Then
            OK_Button.Enabled = True
        Else
            OK_Button.Enabled = False
        End If
    End Sub

    Private Sub lstTarget_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstTarget.SelectedIndexChanged
        Me._tarIndex = lstTarget.SelectedIndex
        If (_srcIndex >= 0) And (_tarIndex >= 0) Then
            OK_Button.Enabled = True
        Else
            OK_Button.Enabled = False
        End If
    End Sub
End Class
