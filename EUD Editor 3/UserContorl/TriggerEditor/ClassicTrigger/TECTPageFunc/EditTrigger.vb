﻿Partial Public Class TECTPage
    Private Sub TriggerEditBtn_Click(sender As Object, e As RoutedEventArgs)
        EditTrigger()
    End Sub


    Public Sub EditTrigger()
        If TListBox.SelectedItem IsNot Nothing Then

            Dim trg As Trigger = TListBox.SelectedItem
            Dim listboxindex As Integer = TListBox.SelectedIndex





            '편집기 열기
            '편집기에 들어갈 trg
            Dim edittrg As Trigger = trg.DeepCopy()
            EditWindow.Visibility = Visibility.Visible

            OpenStoryBoard.Begin(Me)
            Dim TrigEditctrl As New TriggerEditControl(Scripter, edittrg)
            InputDialog.Content = TrigEditctrl



            AddHandler TrigEditctrl.OkayBtnEvent, Sub()
                                                      Dim tcheck As Boolean = False
                                                      For k = 0 To 7
                                                          If trg.PlayerEnabled(k) Then
                                                              tcheck = True
                                                          End If
                                                      Next
                                                      For k = 0 To 6
                                                          If trg.ForceEnabled(k) Then
                                                              tcheck = True
                                                          End If
                                                      Next
                                                      If Not tcheck Then
                                                          '모든 트리거가 꺼져있어서 추가불가능
                                                          Return
                                                      End If



                                                      edittrg.CopyTo(trg)
                                                      'trg를 수정한다.
                                                      'GetTrgBlock(TListBox.SelectedItem).Refresh()

                                                      PlayerListReset()

                                                      Dim CurrentPage As Integer = PlayerList.SelectedIndex

                                                      If CurrentPage < 8 Then
                                                          If Not trg.PlayerEnabled(CurrentPage) Then
                                                              For i = 0 To trg.PlayerEnabled.Count - 1
                                                                  If trg.PlayerEnabled(i) Then
                                                                      If CurrentPage <> i Then
                                                                          PlayerList.SelectedIndex = i
                                                                          'RefreshTriggerPage()
                                                                      End If
                                                                      Exit For
                                                                  End If
                                                              Next
                                                          End If
                                                      Else
                                                          If Not trg.ForceEnabled(CurrentPage - 8) Then
                                                              For i = 0 To trg.ForceEnabled.Count - 1
                                                                  If trg.ForceEnabled(i) Then
                                                                      If (CurrentPage - 8) <> i Then
                                                                          PlayerList.SelectedIndex = CurrentPage
                                                                          'RefreshTriggerPage()
                                                                      End If
                                                                      Exit For
                                                                  End If
                                                              Next
                                                          End If
                                                      End If


                                                      pjData.SetDirty(True)
                                                      trg.PropertyChangeAll()
                                                      'InputDialog.Content = Nothing
                                                      'EditWindow.Visibility = Visibility.Hidden
                                                      CloseStoryBoard.Begin(Me)
                                                  End Sub



            AddHandler TrigEditctrl.CancelBtnEvent, Sub()
                                                        'InputDialog.Content = Nothing
                                                        'EditWindow.Visibility = Visibility.Hidden
                                                        CloseStoryBoard.Begin(Me)
                                                    End Sub



        End If
    End Sub


End Class
