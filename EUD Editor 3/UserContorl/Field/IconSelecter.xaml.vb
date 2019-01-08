﻿Imports MaterialDesignThemes.Wpf

Public Class IconSelecter
    Private DatFile As SCDatFiles.DatFiles
    Private ObjectID As Integer
    Private Parameter As String

    Private DatCommand As DatCommand

    Public Sub Init(_DatFile As SCDatFiles.DatFiles, _ObjectID As Integer, _Parameter As String)
        DatFile = _DatFile
        ObjectID = _ObjectID
        Parameter = _Parameter

        Field.Init(DatFile, ObjectID, Parameter)

        DataContext = pjData.BindingManager.DatBinding(DatFile, Parameter, ObjectID)
    End Sub


    Private Sub Click(sender As Object, e As RoutedEventArgs) Handles btn.Click
        'CodeList.SetFliter(CodeSelecter.ESortType.n123)

        Dim valueType As SCDatFiles.DatFiles = pjData.Dat.ParamInfo(DatFile, Parameter, SCDatFiles.EParamInfo.ValueType)
        If valueType <> SCDatFiles.DatFiles.None Then
            CodeList.ListReset(valueType)
            CodeSelect.IsOpen = True
        End If

    End Sub

    Private Sub CodeList_Select(Code As Object, e As RoutedEventArgs)
        CodeSelect.IsOpen = False
        Field.ValueText.Text = Code(1)
        'MsgBox(Code(1))
    End Sub
End Class
