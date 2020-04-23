﻿Partial Public Class MacroManager
    Public luaReturnstr As String

    Public Sub echo(t As String)
        luaReturnstr = luaReturnstr & t
    End Sub
    Public Function ParseUnit(unit As String) As String
        Dim rint As Integer = pjData.GetUnitIndex(unit)
        If rint = -1 Then
            Return unit
        Else
            Return rint
        End If
    End Function
    Public Function ParseLocation(loc As String) As String
        Dim rint As Integer = pjData.GetLocationIndex(loc)
        If rint = -1 Then
            Return loc
        Else
            Return rint
        End If
    End Function
    Public Function ParseSwitchName(switch As String) As String
        Dim rint As Integer = pjData.GetSwitchIndex(switch)
        If rint = -1 Then
            Return switch
        Else
            Return rint
        End If
    End Function


    Public Function ParseWeapon(ObjectName As String) As String
        Dim rint As Integer = pjData.GetWeaponIndex(ObjectName)
        If rint = -1 Then
            Return ObjectName
        Else
            Return rint
        End If
    End Function
    Public Function ParseFlingy(ObjectName As String) As String
        Dim rint As Integer = pjData.GetFlingyIndex(ObjectName)
        If rint = -1 Then
            Return ObjectName
        Else
            Return rint
        End If
    End Function
    Public Function ParseSprites(ObjectName As String) As String
        Dim rint As Integer = pjData.GetSpriteIndex(ObjectName)
        If rint = -1 Then
            Return ObjectName
        Else
            Return rint
        End If
    End Function
    Public Function ParseImages(ObjectName As String) As String
        Dim rint As Integer = pjData.GetImageIndex(ObjectName)
        If rint = -1 Then
            Return ObjectName
        Else
            Return rint
        End If
    End Function
    Public Function ParseUpgrades(ObjectName As String) As String
        Dim rint As Integer = pjData.GetUpgradeIndex(ObjectName)
        If rint = -1 Then
            Return ObjectName
        Else
            Return rint
        End If
    End Function
    Public Function ParseTechdata(ObjectName As String) As String
        Dim rint As Integer = pjData.GetTechIndex(ObjectName)
        If rint = -1 Then
            Return ObjectName
        Else
            Return rint
        End If
    End Function
    Public Function ParseOrders(ObjectName As String) As String
        Dim rint As Integer = pjData.GetOrderIndex(ObjectName)
        If rint = -1 Then
            Return ObjectName
        Else
            Return rint
        End If
    End Function






    Public Function DatOffset(DatName As String, Paramater As String) As String
        Dim datfile As SCDatFiles.DatFiles = pjData.Dat.GetDatFileE(DatName)

        Paramater = Paramater.Replace("_", " ").Trim

        Dim Start As Integer = pjData.Dat.GetDatFile(datfile).GetParamInfo(Paramater, SCDatFiles.EParamInfo.VarStart)
        Dim Size As Byte = pjData.Dat.GetDatFile(datfile).GetParamInfo(Paramater, SCDatFiles.EParamInfo.Size)
        Dim Length As Byte = Size * pjData.Dat.GetDatFile(datfile).GetParamInfo(Paramater, SCDatFiles.EParamInfo.VarArray)

        Dim offset As String = "0x" & Hex(Tool.GetOffset(datfile, Paramater)).ToUpper()
        Return offset
    End Function
    Public Function GetDatFile(DatName As String, Paramater As String, index As String) As String
        Dim offset As String = DatOffset(DatName, Paramater)
        Dim datfile As SCDatFiles.DatFiles = pjData.Dat.GetDatFileE(DatName)

        Paramater = Paramater.Replace("_", " ").Trim

        Dim Start As Integer = pjData.Dat.GetDatFile(datfile).GetParamInfo(Paramater, SCDatFiles.EParamInfo.VarStart)
        Dim Size As Byte = pjData.Dat.GetDatFile(datfile).GetParamInfo(Paramater, SCDatFiles.EParamInfo.Size)
        Dim Length As Byte = Size * pjData.Dat.GetDatFile(datfile).GetParamInfo(Paramater, SCDatFiles.EParamInfo.VarArray)


        Dim AddOffset As String
        If Start = 0 Then
            AddOffset = index
        Else
            AddOffset = index & " - " & Start
        End If
        AddOffset = AddOffset & " * " & Length


        Dim action As String = ""
        Select Case Size
            Case 1
                action = "bread"
            Case 2
                action = "wread"
            Case 4
                action = "dwread"
        End Select
        action = action & "(" & offset & " + " & AddOffset & ")"


        Return action
    End Function
    Public Function SetDatFile(DatName As String, Paramater As String, index As String, Value As String, Modifier As String) As String
        Dim offset As String = DatOffset(DatName, Paramater)
        Dim datfile As SCDatFiles.DatFiles = pjData.Dat.GetDatFileE(DatName)

        Paramater = Paramater.Replace("_", " ").Trim

        Dim Start As Integer = pjData.Dat.GetDatFile(datfile).GetParamInfo(Paramater, SCDatFiles.EParamInfo.VarStart)
        Dim Size As Byte = pjData.Dat.GetDatFile(datfile).GetParamInfo(Paramater, SCDatFiles.EParamInfo.Size)
        Dim Length As Byte = Size * pjData.Dat.GetDatFile(datfile).GetParamInfo(Paramater, SCDatFiles.EParamInfo.VarArray)


        Dim AddOffset As String
        If Start = 0 Then
            AddOffset = index
        Else
            AddOffset = index & " - " & Start
        End If
        AddOffset = AddOffset & " * " & Length

        Dim rOffset As String = offset & " + " & AddOffset


        Dim action As String = ""

        Select Case Size
            Case 1, 2
                If IsNumeric(index) Then
                    '인덱스가 상수일 경우
                    Dim rindex As Integer = index * Length
                    Dim f As Integer = rindex Mod 4

                    Dim flag As String
                    If Size = 1 Then
                        flag = "FF"
                    Else
                        flag = "FFFF"
                    End If

                    For i = 0 To f - 1
                        flag = flag & "00"
                    Next

                    If IsNumeric(Value) Then
                        Dim iValue As UInteger
                        If Size = 1 Then
                            iValue = Value * 1
                        Else
                            iValue = Value * 65536
                        End If
                        action = String.Format("SetMemoryXEPD(EPD({0}), {1}, {2}, {3})", rOffset, Modifier, iValue, "0x" & flag)
                    Else
                        If Size = 1 Then
                            action = String.Format("SetMemoryXEPD(EPD({0}), {1}, {2}, {3})", rOffset, Modifier, Value, "0x" & flag)
                        Else
                            action = String.Format("SetMemoryXEPD(EPD({0}), {1}, {2}, {3})", rOffset, Modifier, Value & " * 65536", "0x" & flag)
                        End If
                    End If
                Else
                    '인덱스가 변수 일 경우
                    'local ModifierDict = {
                    '    [SetTo] =  7,
                    '    [Add] =  8,
                    '    [Subtract] =  9,
                    '}
                    Select Case Modifier
                        Case 7
                            If Size = 1 Then
                                action = String.Format("bwrite({0} ,{1})", rOffset, Value)
                            Else
                                action = String.Format("wwrite({0} ,{1})", rOffset, Value)
                            End If
                        Case 8
                            If Size = 1 Then
                                action = String.Format("bwrite({0} ,bread({0}) + {1})", rOffset, Value)
                            Else
                                action = String.Format("wwrite({0} ,wread({0}) + {1})", rOffset, Value)
                            End If
                        Case 9
                            If Size = 1 Then
                                action = String.Format("bwrite({0} ,bread({0}) - {1})", rOffset, Value)
                            Else
                                action = String.Format("wwrite({0} ,wread({0}) - {1})", rOffset, Value)
                            End If
                    End Select


                End If
                    Case 4
                action = String.Format("SetMemory({0}, {1}, {2})", rOffset, Modifier, Value)
        End Select


        Return action
    End Function
    Public Function ConditionDatFile(DatName As String, Paramater As String, index As String, Value As String, Modifier As String) As String
        Dim offset As String = DatOffset(DatName, Paramater)
        Dim datfile As SCDatFiles.DatFiles = pjData.Dat.GetDatFileE(DatName)
        Dim Start As Integer = pjData.Dat.GetDatFile(datfile).GetParamInfo(Paramater, SCDatFiles.EParamInfo.VarStart)
        Dim Size As Byte = pjData.Dat.GetDatFile(datfile).GetParamInfo(Paramater, SCDatFiles.EParamInfo.Size)
        Dim Length As Byte = Size * pjData.Dat.GetDatFile(datfile).GetParamInfo(Paramater, SCDatFiles.EParamInfo.VarArray)


        Dim AddOffset As String
        If Start = 0 Then
            AddOffset = index
        Else
            AddOffset = index & " - " & Start
        End If
        AddOffset = AddOffset & " * " & Length

        Dim rOffset As String = offset & " + " & AddOffset


        Dim action As String = ""

        Select Case Size
            Case 1, 2
                If IsNumeric(index) Then
                    '인덱스가 상수일 경우
                    Dim rindex As Integer = index * Length
                    Dim f As Integer = rindex Mod 4

                    Dim flag As String
                    If Size = 1 Then
                        flag = "FF"
                    Else
                        flag = "FFFF"
                    End If

                    For i = 0 To f - 1
                        flag = flag & "00"
                    Next

                    If IsNumeric(Value) Then
                        Dim iValue As UInteger
                        If Size = 1 Then
                            iValue = Value * 1
                        Else
                            iValue = Value * 65536
                        End If
                        action = String.Format("MemoryXEPD(EPD({0}), {1}, {2}, {3})", rOffset, Modifier, iValue, "0x" & flag)
                    Else
                        If Size = 1 Then
                            action = String.Format("MemoryXEPD(EPD({0}), {1}, {2}, {3})", rOffset, Modifier, Value, "0x" & flag)
                        Else
                            action = String.Format("MemoryXEPD(EPD({0}), {1}, {2}, {3})", rOffset, Modifier, Value & " * 65536", "0x" & flag)
                        End If
                    End If
                Else
                    'local ComparisonDict = {
                    '    [AtLeast] = 0,
                    '    [AtMost] = 1,
                    '    [Exactly] = 10,
                    '}
                    Select Case Modifier
                        Case 0
                            If Size = 1 Then
                                action = String.Format("(bread({0}) > {1})", rOffset, Value)
                            Else
                                action = String.Format("(wread({0}) > {1})", rOffset, Value)
                            End If
                        Case 1
                            If Size = 1 Then
                                action = String.Format("(bread({0}) < {1})", rOffset, Value)
                            Else
                                action = String.Format("(wread({0}) < {1})", rOffset, Value)
                            End If
                        Case 10
                            If Size = 1 Then
                                action = String.Format("(bread({0}) == {1})", rOffset, Value)
                            Else
                                action = String.Format("(wread({0}) == {1})", rOffset, Value)
                            End If
                    End Select


                End If
            Case 4
                action = String.Format("Memory({0}, {1}, {2})", rOffset, Modifier, Value)
        End Select


        Return action
    End Function


End Class
