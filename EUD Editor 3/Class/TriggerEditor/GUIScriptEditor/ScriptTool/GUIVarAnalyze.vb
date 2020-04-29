﻿Partial Public Class GUIScriptManager

    Private Function AddAble(cname As String, curtype As String, fname As String, ftype As String) As Boolean
        If fname.Trim <> "" Then
            If cname <> fname Then
                Return False
            End If
        End If
        If ftype.Trim <> "" Then
            If curtype <> ftype Then
                Return False
            End If
        End If


        Return True
    End Function

    '현재 위치로부터 변수들을 구하는 함수들을 만들것
    Public Function GetLocalVar(normalscr As ScriptBlock, Optional type As String = "", Optional varname As String = "") As List(Of ScriptBlock)
        Dim fname As String = varname
        Dim ftype As String = type

        Dim rscr As New List(Of ScriptBlock)
        If normalscr Is Nothing Then
            Return rscr
        End If

        If normalscr.isfolder Then
            If normalscr.child.Count >= 1 Then


                normalscr = normalscr.child.Last
            End If
        End If

        While normalscr.Parent IsNot Nothing
            Dim index As Integer = normalscr.Parent.child.IndexOf(normalscr)
            For i = index To 0 Step -1
                Dim tscr As ScriptBlock = normalscr.Parent.child(i)
                Select Case tscr.ScriptType
                    Case ScriptBlock.EBlockType.vardefine
                        Dim cname As String = tscr.value
                        Dim curtype As String = tscr.value2

                        If AddAble(cname, curtype, fname, ftype) Then
                            rscr.Add(tscr)
                        End If
                    Case ScriptBlock.EBlockType.funargs
                        Dim arglist As List(Of ScriptBlock) = tscr.child
                        For k = 0 To arglist.Count - 1
                            Dim cname As String = arglist(k).value
                            Dim curtype As String = arglist(k).value2

                            If AddAble(cname, curtype, fname, ftype) Then
                                rscr.Add(arglist(k))
                            End If
                        Next
                End Select
            Next
            normalscr = normalscr.Parent

            'If normalscr.ScriptType = ScriptBlock.EBlockType._for Then
            '    Dim values As String() = normalscr.ForvarName
            '    For i = 0 To values.Count - 1


            '        If varname.Trim = "" Then
            '            Dim tscr As ScriptBlock = New ScriptBlock(ScriptBlock.EBlockType.vardefine, "vardefine", False, False, values(i).Trim, Nothing)
            '            tscr.value2 = "var"
            '            rscr.Add(tscr)
            '        Else
            '            If values(i).Trim = varname Then
            '                Dim tscr As ScriptBlock = New ScriptBlock(ScriptBlock.EBlockType.vardefine, "vardefine", False, False, values(i).Trim, Nothing)
            '                tscr.value2 = "var"
            '                rscr.Add(tscr)
            '            End If
            '        End If
            '        'MsgBox(values(i))
            '    Next
            'End If
        End While

        '인덱스가 0이 될대까지 위로 올라감.
        '인덱스가 0이 되면

        Return rscr
    End Function
    Public Function GetGlobalVar(GUIEditor As GUIScriptEditor, Optional type As String = "", Optional varname As String = "") As List(Of ScriptBlock)
        Dim rscr As New List(Of ScriptBlock)

        Dim fname As String = varname
        Dim ftype As String = type
        For i = 0 To GUIEditor.ItemCount - 1
            If GUIEditor.GetItems(i).ScriptType = ScriptBlock.EBlockType.vardefine Then
                Dim cname As String = GUIEditor.GetItems(i).value
                Dim curtype As String = GUIEditor.GetItems(i).value2
                If AddAble(cname, curtype, fname, ftype) Then
                    rscr.Add(GUIEditor.GetItems(i))
                End If
            End If
        Next

        Return rscr
    End Function
    Public Function GetExternVar(GUIEditor As GUIScriptEditor, Optional type As String = "", Optional varname As String = "", Optional nspace As String = "") As List(Of ScriptBlock)
        Dim rscr As New List(Of ScriptBlock)

        Dim fname As String = varname
        Dim ftype As String = type
        For i = 0 To GUIEditor.ExternFile.Count - 1
            For j = 0 To GUIEditor.ExternFile(i).Funcs.VariableCount - 1
                Dim extername As String = GUIEditor.ExternFile(i).nameSpaceName

                If nspace.Trim <> "" And nspace <> extername Then
                    Continue For
                End If
                'MsgBox(nspace & ":" & GUIEditor.ExternFile(i).nameSpaceName)


                Dim vname As String = GUIEditor.ExternFile(i).Funcs.GetVariableNames(j)
                Dim vtype As String = GUIEditor.ExternFile(i).Funcs.GetVariableType(j)



                Dim scrtype As ScriptBlock.EBlockType
                Dim cname As String = GUIEditor.ExternFile(i).nameSpaceName & "." & vname
                Dim curtype As String 'value2

                Dim cscr As ScriptBlock = Nothing

                If vtype.Trim = "" Then
                    '타입이 없음 = 일반 var
                    'ScriptBlock.EBlockType.vardefine
                    scrtype = ScriptBlock.EBlockType.vardefine
                    curtype = "var"
                Else
                    '타입이 있음 = const var
                    If IsNumeric(vtype) Then
                        '일반 값이 들어있는 상수변수
                        scrtype = ScriptBlock.EBlockType.vardefine
                        curtype = "const"

                        cscr = New ScriptBlock(ScriptBlock.EBlockType.rawcode, "rawcode", False, False, vtype, GUIEditor)
                    Else
                        '오브젝트
                        scrtype = ScriptBlock.EBlockType.vardefine
                        curtype = "object"


                        Dim objname As String = ""
                        Dim makestyle As String = ""
                        If vtype.IndexOf("(") <> -1 Then
                            objname = vtype.Split("(").First
                        End If

                        If objname.IndexOf(".") = -1 Then
                            '기본 생성자
                            makestyle = "constructor"
                        Else
                            objname = objname.Split(".").First
                            makestyle = objname.Split(".").Last
                        End If


                        cscr = New ScriptBlock(ScriptBlock.EBlockType.varuse, objname, False, False, makestyle, GUIEditor)
                    End If
                End If

                Dim scr As New ScriptBlock(scrtype, "vardefine", False, False, cname, GUIEditor)
                scr.value2 = curtype
                If cscr IsNot Nothing Then
                    scr.AddChild(cscr)
                End If

                If nspace.Trim = "" Then
                    If AddAble(cname, curtype, fname, ftype) Then
                        rscr.Add(scr)
                    End If
                Else
                    If AddAble(cname, curtype, nspace & "." & fname, ftype) Then
                        rscr.Add(scr)
                    End If
                End If


                'MsgBox(vname & "," & vtype)
            Next

        Next


        Return rscr
    End Function



    Public Function GetAllVar(normalscr As ScriptBlock, GUIEditor As GUIScriptEditor, Optional type As String = "", Optional varname As String = "") As List(Of ScriptBlock)
        Dim rscr As New List(Of ScriptBlock)

        rscr.AddRange(GetLocalVar(normalscr, type, varname))
        rscr.AddRange(GetGlobalVar(GUIEditor, type, varname))
        rscr.AddRange(GetExternVar(GUIEditor, type, varname))


        Return rscr
    End Function

End Class