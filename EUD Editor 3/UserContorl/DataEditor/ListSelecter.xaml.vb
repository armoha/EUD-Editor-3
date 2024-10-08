﻿Imports System.ComponentModel

Public Class ListSelecter
    Private factoryPanel As FrameworkElementFactory = New FrameworkElementFactory(GetType(WrapPanel))
    Private itemTemplate As ItemsPanelTemplate = New ItemsPanelTemplate()


    Private IsFirstLoad As Boolean = False
    Private IsComboBox As Boolean
    Private StartIndex As Integer

    Public Event ListSelect As RoutedEventHandler
    Public Event OpenWindow As RoutedEventHandler
    Public Event OpenTab As RoutedEventHandler

    Private LastSelectIndex As Integer = -1

    Private timer As New Date
    Private Sub CodeIndexerList_KeyDown(sender As Object, e As KeyEventArgs) Handles CodeIndexerList.PreviewKeyDown
        If timer.AddMilliseconds(50) < Now Then
            timer = Now
        Else
            Exit Sub
        End If
        If Fliter.SortType <> ESortType.Tree And Not Fliter.IsIcon Then
            If CodeIndexerList.SelectedItem Is Nothing Then
                Exit Sub
            End If
            Dim ListboxItem As ListBoxItem

            If e.Key = Key.Down Then
                If CodeIndexerList.SelectedIndex + 1 < CodeIndexerList.Items.Count Then
                    ListboxItem = CodeIndexerList.Items(CodeIndexerList.SelectedIndex + 1)
                Else
                    ListboxItem = CodeIndexerList.SelectedItem
                End If
            ElseIf e.Key = Key.Up Then
                If CodeIndexerList.SelectedIndex > 0 Then
                    ListboxItem = CodeIndexerList.Items(CodeIndexerList.SelectedIndex - 1)
                Else
                    ListboxItem = CodeIndexerList.SelectedItem
                End If
            Else
                Exit Sub
            End If
            Dim Selectindex As Integer = ListboxItem.Tag

            If LastSelectIndex = -1 Or LastSelectIndex <> Selectindex Then
                LastSelectIndex = Selectindex
                Dim returnval() As Integer = {CurrentPage, LastSelectIndex}
                RaiseEvent ListSelect(returnval, e)
            End If
        End If
    End Sub

    Private Sub CodeIndexerList_Select(sender As Object, e As MouseButtonEventArgs) Handles CodeIndexerList.MouseLeftButtonUp
        If Fliter.SortType <> ESortType.Tree And Not Fliter.IsIcon Then
            If CodeIndexerList.SelectedItem Is Nothing Then
                Exit Sub
            End If
            Dim ListboxItem As ListBoxItem = CodeIndexerList.SelectedItem

            Dim Selectindex As Integer = ListboxItem.Tag
            If LastSelectIndex = -1 Or LastSelectIndex <> Selectindex Then
                LastSelectIndex = Selectindex
                Dim returnval() As Integer = {CurrentPage, LastSelectIndex}
                RaiseEvent ListSelect(returnval, e)
            End If
        End If
    End Sub
    Private Sub CodeIndexerImage_Select(sender As Object, e As MouseButtonEventArgs) Handles CodeIndexerImage.MouseLeftButtonUp
        If Fliter.SortType <> ESortType.Tree And Fliter.IsIcon Then
            If CodeIndexerImage.SelectedItem Is Nothing Then
                Exit Sub
            End If
            Dim ListboxItem As ListBoxItem = CodeIndexerImage.SelectedItem

            Dim Selectindex As Integer = ListboxItem.Tag

            If LastSelectIndex = -1 Or LastSelectIndex <> Selectindex Then
                LastSelectIndex = Selectindex
                Dim returnval() As Integer = {CurrentPage, LastSelectIndex}
                RaiseEvent ListSelect(returnval, e)
            End If
        End If
    End Sub

    Private Sub CodeIndexerTree_SelectedItemChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Object))
        If Fliter.SortType = ESortType.Tree Then
            If e.NewValue IsNot Nothing Then
                Dim selectNode As TreeViewItem = e.NewValue

                If selectNode.Items.Count = 0 Then
                    Dim returnval() As Integer = {CurrentPage, selectNode.Tag}
                    RaiseEvent ListSelect(returnval, e)
                End If
            End If
        End If
        ' RaiseEvent ListSelect(sender, e)
    End Sub

    'Private Sub TreeView_PreviewMouseRightButtonDown(sender As Object, e As MouseButtonEventArgs)
    '    MsgBox("dpfj")
    '    Dim TreeViewItem As TreeViewItem = VisualUpwardSearch(e.OriginalSource)

    '    If (TreeViewItem Is Nothing) Then

    '        TreeViewItem.Focus()
    '        e.Handled = True
    '    End If
    'End Sub

    'Private Function VisualUpwardSearch(source As DependencyObject) As TreeViewItem
    '    While (source IsNot Nothing And Not (source.GetType = GetType(TreeViewItem)))
    '        source = VisualTreeHelper.GetParent(source)
    '    End While

    '    Return CType(source, TreeViewItem)
    'End Function

    Private MenuOpendSelectIndex As Integer
    Private Sub ContextMenu_Opened(sender As Object, e As RoutedEventArgs)
        Dim index As Integer
        Select Case Fliter.SortType
            Case ESortType.Tree
                If CodeIndexerTree.SelectedItem Is Nothing Then
                    Exit Sub
                End If


                Dim selectNode As TreeViewItem = CodeIndexerTree.SelectedItem
                index = selectNode.Tag
                'MsgBox(index)
            Case Else
                If Fliter.IsIcon Then
                    If CodeIndexerImage.SelectedItem Is Nothing Then
                        Exit Sub
                    End If
                    Dim ListboxItem As ListBoxItem = CodeIndexerImage.SelectedItem

                    index = ListboxItem.Tag


                    'MsgBox(index)
                Else
                    If CodeIndexerList.SelectedItem Is Nothing Then
                        Exit Sub
                    End If
                    Dim ListboxItem As ListBoxItem = CodeIndexerList.SelectedItem

                    index = ListboxItem.Tag



                    'MsgBox(index)
                End If
        End Select

        MenuOpendSelectIndex = index
        If SCCodeCount(CurrentPage) <= MenuOpendSelectIndex Then
            If IsComboBox Then
                Dim ContextMenu As ContextMenu = FindResource("OnlyOpenWindon")
                For i = 0 To ContextMenu.Items.Count - 1
                    If ContextMenu.Items(i).GetType = GetType(MenuItem) Then
                        Dim MenuItem As MenuItem = ContextMenu.Items(i)
                        MenuItem.IsEnabled = False
                    End If
                Next
            Else
                Dim ContextMenu As ContextMenu = FindResource("ContextMenu")
                For i = 0 To ContextMenu.Items.Count - 1
                    If ContextMenu.Items(i).GetType = GetType(MenuItem) Then
                        Dim MenuItem As MenuItem = ContextMenu.Items(i)
                        MenuItem.IsEnabled = False
                    End If
                Next
            End If

            Exit Sub
        Else
            If IsComboBox Then
                Dim ContextMenu As ContextMenu = FindResource("OnlyOpenWindon")
                For i = 0 To ContextMenu.Items.Count - 1
                    If ContextMenu.Items(i).GetType = GetType(MenuItem) Then
                        Dim MenuItem As MenuItem = ContextMenu.Items(i)
                        MenuItem.IsEnabled = True
                    End If
                Next
            Else
                Dim ContextMenu As ContextMenu = FindResource("ContextMenu")
                For i = 0 To ContextMenu.Items.Count - 1
                    If ContextMenu.Items(i).GetType = GetType(MenuItem) Then
                        Dim MenuItem As MenuItem = ContextMenu.Items(i)
                        MenuItem.IsEnabled = True
                    End If
                Next
            End If
        End If


        If Not IsComboBox Then
            Dim ContextMenu As ContextMenu = FindResource("ContextMenu")
            Dim MenuItem As MenuItem = ContextMenu.Items(1)
            MenuItem.IsEnabled = pjData.DataManager.DataObjectPasteAble(CurrentPage, index)
        End If

    End Sub


    Private Sub MenuItem_Click(sender As Object, e As RoutedEventArgs)
        TabItemTool.WindowTabItem(CurrentPage, MenuOpendSelectIndex)
    End Sub

    Private Sub OpentabMenuItem_Click(sender As Object, e As RoutedEventArgs)
        RaiseEvent OpenTab(MenuOpendSelectIndex, e)
    End Sub

    Private Sub OpenWindowMenuItem_Click(sender As Object, e As RoutedEventArgs)
        RaiseEvent OpenWindow(MenuOpendSelectIndex, e)
    End Sub

    Private Sub CopyItem_Click(sender As Object, e As RoutedEventArgs)
        pjData.DataManager.CopyDatObject(CurrentPage, MenuOpendSelectIndex)
    End Sub

    Private Sub PasteItem_Click(sender As Object, e As RoutedEventArgs)
        pjData.DataManager.PasteDatObject(CurrentPage, MenuOpendSelectIndex)
    End Sub

    Private Sub ResetItem_Click(sender As Object, e As RoutedEventArgs)
        pjData.DataManager.ResetDatObject(CurrentPage, MenuOpendSelectIndex)
    End Sub


    Private LoadCmp As Boolean = False
    Public Sub New()
        ' 디자이너에서 이 호출이 필요합니다.
        InitializeComponent()

        ' InitializeComponent() 호출 뒤에 초기화 코드를 추가하세요.
        Fliter = New tFliter
        factoryPanel.SetValue(WrapPanel.IsItemsHostProperty, True)
        itemTemplate.VisualTree = factoryPanel
        TreeviewItemDic = New Dictionary(Of Integer, TreeViewItem)

        LoadCmp = True
    End Sub


    Private Structure UnitName
        Public Name As String
        Public index As Integer

        Public Sub New(tname As String, tindex As Integer)
            Name = tname
            index = tindex
        End Sub
    End Structure

    Private CurrentPage As SCDatFiles.DatFiles
    Public ReadOnly Property Page As SCDatFiles.DatFiles
        Get
            Return CurrentPage
        End Get
    End Property

    Public Enum ESortType
        n123
        ABC
        Tree
    End Enum
    Private Fliter As tFliter
    Private Structure tFliter
        Public fliterText As String

        Public IsEdit As Boolean
        Public IsIcon As Boolean
        Private TSortType As ESortType
        Public ReadOnly Property SortType As ESortType
            Get
                Return TSortType
            End Get
        End Property



        Public Sub SetFliter(type As ESortType)
            TSortType = type
        End Sub
    End Structure
    Public Sub SetFliter(tfliter As ESortType)


        Fliter.SetFliter(tfliter)
    End Sub


    Public Sub ListReset(Optional pagetype As SCDatFiles.DatFiles = SCDatFiles.DatFiles.None, Optional combobox As Boolean = True, Optional _StartIndex As Integer = 0)
        If pagetype = SCDatFiles.DatFiles.None Then
            pagetype = CurrentPage
        Else
            CurrentPage = pagetype
        End If

        LastSelectIndex = -1
        IsComboBox = combobox
        StartIndex = _StartIndex
        If Not IsFirstLoad Then
            IsFirstLoad = True
            If CurrentPage <= SCDatFiles.DatFiles.orders Then
                If IsComboBox Then
                    CodeIndexerImage.ContextMenu = FindResource("OnlyOpenWindon")
                    CodeIndexerList.ContextMenu = FindResource("OnlyOpenWindon")
                End If
            Else
                CodeIndexerImage.ContextMenu = Nothing
                CodeIndexerList.ContextMenu = Nothing

                L_IconBtn.Visibility = Visibility.Collapsed
                L_IsEditBtn.Visibility = Visibility.Collapsed

                If CurrentPage = SCDatFiles.DatFiles.portdata Then
                    BtnsortTree.Visibility = Visibility.Collapsed
                End If
            End If




        End If


        Select Case Fliter.SortType
            Case ESortType.n123, ESortType.ABC
                If Fliter.IsIcon Then
                    CodeIndexerImage.Visibility = Visibility.Visible
                    CodeIndexerTree.Visibility = Visibility.Hidden
                    CodeIndexerList.Visibility = Visibility.Hidden
                Else
                    CodeIndexerImage.Visibility = Visibility.Hidden
                    CodeIndexerTree.Visibility = Visibility.Hidden
                    CodeIndexerList.Visibility = Visibility.Visible
                End If
            Case ESortType.Tree
                CodeIndexerImage.Visibility = Visibility.Hidden
                CodeIndexerTree.Visibility = Visibility.Visible
                CodeIndexerList.Visibility = Visibility.Hidden
        End Select



        ListResetData(pagetype, StartIndex)




    End Sub
    Private Function GetIcon(iconIndex As Integer, isSizeBig As Boolean) As Border
        Dim imgSource As ImageSource = scData.GetIcon(iconIndex)
        Dim bitmap As New Image
        bitmap.BeginInit()
        bitmap.Source = imgSource
        If isSizeBig Then
            bitmap.Width = 56
            bitmap.Height = 56
        Else
            bitmap.Width = 30
            bitmap.Height = 30
        End If
        bitmap.EndInit()

        Dim tborder As New Border
        tborder.Child = bitmap
        tborder.Background = Brushes.Black
        If isSizeBig Then
            tborder.Margin = New Thickness(-7, -7, -7, -7)
        Else
            tborder.Margin = New Thickness(-8, -10, -10, -10)
        End If

        Return tborder
    End Function
    Private Function GetImage(grpIndex As Integer, isSizeBig As Boolean) As Border
        Dim imgSource As ImageSource = scData.GetGRPImage(grpIndex, 12)
        Dim bitmap As New Image
        bitmap.BeginInit()
        bitmap.Source = imgSource
        If isSizeBig Then
            bitmap.Width = 56
            bitmap.Height = 56
        Else
            bitmap.Width = 30
            bitmap.Height = 30
        End If
        bitmap.EndInit()

        Dim tborder As New Border
        tborder.Child = bitmap
        tborder.Background = Brushes.Black
        If isSizeBig Then
            tborder.Margin = New Thickness(-7, -7, -7, -7)
        Else
            tborder.Margin = New Thickness(-8, -10, -10, -10)
        End If

        Return tborder
    End Function

    Private Sub ListResetData(pagetype As SCDatFiles.DatFiles, StartIndex As Integer)
        '리스트에 들어갈 거는 리스트이름과 아이콘, 그룹패스뿐임.
        '이것만 잘 넘겨주면 됨
        Dim ObjectNames As New List(Of String)
        Dim ObjectImages As New List(Of Border)

        Select Case pagetype
            Case SCDatFiles.DatFiles.units
                For i = 0 To SCUnitCount - 1
                    Dim tGraphics As Integer = pjData.Dat.Data(SCDatFiles.DatFiles.units, "Graphics", i)
                    Dim tSprite As Integer = pjData.Dat.Data(SCDatFiles.DatFiles.flingy, "Sprite", tGraphics)
                    Dim timage As Integer = pjData.Dat.Data(SCDatFiles.DatFiles.sprites, "Image File", tSprite)

                    ObjectNames.Add(pjData.CodeLabel(pagetype, i))
                    ObjectImages.Add(GetImage(timage, Fliter.IsIcon))
                Next
            Case SCDatFiles.DatFiles.weapons
                For i = 0 To SCWeaponCount - 1
                    Dim tLabel As Integer = pjData.Dat.Data(SCDatFiles.DatFiles.weapons, "Label", i) - 1
                    Dim tIcon As Integer = pjData.Dat.Data(SCDatFiles.DatFiles.weapons, "Icon", i)

                    Dim cname As String = pjData.CodeLabel(pagetype, i)

                    ObjectNames.Add(cname)
                    ObjectImages.Add(GetIcon(tIcon, Fliter.IsIcon))
                Next
            Case SCDatFiles.DatFiles.flingy
                For i = 0 To SCFlingyCount - 1
                    Dim tSprite As Integer = pjData.Dat.Data(SCDatFiles.DatFiles.flingy, "Sprite", i)
                    Dim timage As Integer = pjData.Dat.Data(SCDatFiles.DatFiles.sprites, "Image File", tSprite)

                    Dim cname As String = pjData.CodeLabel(pagetype, i)


                    ObjectNames.Add(cname)
                    ObjectImages.Add(GetImage(timage, Fliter.IsIcon))
                Next
            Case SCDatFiles.DatFiles.sprites
                For i = 0 To SCSpriteCount - 1
                    Dim timage As Integer = pjData.Dat.Data(SCDatFiles.DatFiles.sprites, "Image File", i)

                    Dim cname As String = pjData.CodeLabel(pagetype, i)

                    ObjectNames.Add(cname)
                    ObjectImages.Add(GetImage(timage, Fliter.IsIcon))
                Next
            Case SCDatFiles.DatFiles.images
                For i = 0 To SCImageCount - 1
                    Dim tooltip As String = pjData.Dat.ToolTip(pagetype, i)
                    Dim cname As String = pjData.CodeLabel(pagetype, i)

                    ObjectNames.Add(cname)
                    ObjectImages.Add(GetImage(i, Fliter.IsIcon))
                Next
            Case SCDatFiles.DatFiles.upgrades
                For i = 0 To SCUpgradeCount - 1
                    Dim tIcon As Integer = pjData.Dat.Data(SCDatFiles.DatFiles.upgrades, "Icon", i)

                    Dim cname As String = pjData.CodeLabel(pagetype, i)

                    ObjectNames.Add(cname)
                    ObjectImages.Add(GetIcon(tIcon, Fliter.IsIcon))
                Next
            Case SCDatFiles.DatFiles.techdata
                For i = 0 To SCTechCount - 1
                    Dim tIcon As Integer = pjData.Dat.Data(SCDatFiles.DatFiles.techdata, "Icon", i)

                    Dim cname As String = pjData.CodeLabel(pagetype, i)

                    ObjectNames.Add(cname)
                    ObjectImages.Add(GetIcon(tIcon, Fliter.IsIcon))
                Next
            Case SCDatFiles.DatFiles.orders
                For i = 0 To SCOrderCount - 1
                    Dim tIcon As Integer = pjData.Dat.Data(SCDatFiles.DatFiles.orders, "Highlight", i)

                    Dim cname As String = pjData.CodeLabel(pagetype, i)

                    ObjectNames.Add(cname)
                    ObjectImages.Add(GetIcon(tIcon, Fliter.IsIcon))
                Next
            Case SCDatFiles.DatFiles.portdata
                For i = 0 To SCPortdataCount - 1
                    Dim cname As String = scData.PortdataName(i)

                    ObjectNames.Add(cname)
                Next
            Case SCDatFiles.DatFiles.sfxdata
                For i = 0 To SCSfxdataCount - 1
                    Dim cname As String = scData.SfxCodeName(i)

                    ObjectNames.Add(cname)
                Next
            Case SCDatFiles.DatFiles.ButtonData
                '어캐작성하노 ㅋㅋ 
                For i = 0 To SCUnitCount - 1
                    Dim tGraphics As Integer = pjData.Dat.Data(SCDatFiles.DatFiles.units, "Graphics", i)
                    Dim tSprite As Integer = pjData.Dat.Data(SCDatFiles.DatFiles.flingy, "Sprite", tGraphics)
                    Dim timage As Integer = pjData.Dat.Data(SCDatFiles.DatFiles.sprites, "Image File", tSprite)

                    ObjectNames.Add(pjData.CodeLabel(pagetype, i))
                    ObjectImages.Add(GetImage(timage, Fliter.IsIcon))
                Next
        End Select


        Select Case Fliter.SortType
            Case ESortType.n123, ESortType.ABC
                Dim SelectItem As ListBoxItem = Nothing

                Dim tList As New List(Of UnitName)
                If Fliter.SortType = ESortType.ABC Then
                    For i = 0 To ObjectNames.Count - 1
                        tList.Add(New UnitName(ObjectNames(i), i))
                    Next
                    Try
                        tList.Sort(Function(x, y) x.Name.CompareTo(y.Name))
                    Catch ex As Exception

                    End Try

                End If

                If Fliter.IsIcon Then
                    CodeIndexerImage.Items.Clear()

                    For i = 0 To ObjectNames.Count - 1
                        Dim index As Integer
                        If Fliter.SortType = ESortType.ABC Then
                            index = tList(i).index
                        Else
                            index = i
                        End If

                        Dim tListItem As New ListBoxItem()
                        tListItem.Tag = index
                        tListItem.Content = ObjectImages(index)

                        If index = StartIndex Then
                            SelectItem = tListItem
                        End If


                        Dim CheckPss As Boolean = False
                        If Fliter.IsEdit Then
                            Dim IsEdit As Boolean = pjData.Dat.GetDatFile(pagetype).CheckDirty(index)
                            If Not IsEdit Then
                                CheckPss = True
                            End If
                        Else
                            If Fliter.fliterText IsNot Nothing Then
                                If Fliter.fliterText = "" Or ObjectNames(index).ToLower.IndexOf(Fliter.fliterText.ToLower) >= 0 Then
                                    CheckPss = True
                                End If
                            Else
                                CheckPss = True
                            End If
                        End If
                        If CheckPss Then
                            CodeIndexerImage.Items.Add(tListItem)
                        End If
                    Next
                Else
                    CodeIndexerList.Items.Clear()
                    For i = 0 To ObjectNames.Count - 1
                        Dim index As Integer
                        If Fliter.SortType = ESortType.ABC Then
                            index = tList(i).index
                        Else
                            index = i
                        End If


                        Dim unitname As String = ObjectNames(index)  '"[" & Format(i, "000") & "]  " & pjData.UnitName(i)

                        Dim textblock As New TextBlock
                        textblock.TextWrapping = TextWrapping.WrapWithOverflow

                        If pagetype <= SCDatFiles.DatFiles.orders Then
                            Dim NameBinding As Binding = New Binding("Name")
                            NameBinding.Source = pjData.BindingManager.UIManager(pagetype, index)
                            textblock.SetBinding(TextBlock.TextProperty, NameBinding)
                        Else
                            textblock.Text = ObjectNames(index)
                        End If


                        'textblock.Text = unitname

                        Dim stackpanel As New DockPanel

                        If pagetype <= SCDatFiles.DatFiles.orders Then
                            textblock.Padding = New Thickness(15, 0, 0, 0)
                            stackpanel.Children.Add(ObjectImages(index))
                        End If

                        stackpanel.Children.Add(textblock)

                        Dim tListItem As New ListBoxItem()
                        tListItem.Tag = index
                        tListItem.Content = stackpanel

                        If pagetype <= SCDatFiles.DatFiles.orders Then
                            Dim BackBinding As Binding = New Binding("Back")
                            BackBinding.Source = pjData.BindingManager.UIManager(pagetype, index)
                            tListItem.SetBinding(ListBoxItem.BackgroundProperty, BackBinding)
                        End If



                        If index = StartIndex Then
                            SelectItem = tListItem
                            If IsComboBox Then
                                tListItem.Background = Application.Current.Resources("PrimaryHueDarkBrush")
                                tListItem.Foreground = Application.Current.Resources("PrimaryHueDarkForegroundBrush")
                            End If
                        End If


                        Dim CheckPss As Boolean = False
                        If Fliter.IsEdit Then
                            Dim IsEdit As Boolean = pjData.Dat.GetDatFile(pagetype).CheckDirty(index)
                            If Not IsEdit Then
                                CheckPss = True
                            End If
                        Else
                            If Fliter.fliterText IsNot Nothing Then
                                If Fliter.fliterText = "" Or ObjectNames(index).ToLower.IndexOf(Fliter.fliterText.ToLower) >= 0 Then
                                    CheckPss = True
                                End If
                            Else
                                CheckPss = True
                            End If
                        End If
                        If CheckPss Then
                            CodeIndexerList.Items.Add(tListItem)
                        End If

                    Next
                End If
                If IsComboBox And (pagetype = SCDatFiles.DatFiles.units Or pagetype = SCDatFiles.DatFiles.weapons) Then
                    Dim textblock As New TextBlock
                    textblock.TextWrapping = TextWrapping.Wrap
                    textblock.Text = Tool.GetText("None")
                    textblock.Padding = New Thickness(15, 0, 0, 0)


                    Dim stackpanel As New DockPanel
                    stackpanel.Children.Add(GetIcon(0, Fliter.IsIcon))
                    stackpanel.Children.Add(textblock)

                    Dim tListItem As New ListBoxItem()
                    tListItem.Tag = CodeIndexerList.Items.Count
                    tListItem.Content = stackpanel

                    If CodeIndexerList.Items.Count = StartIndex Then
                        SelectItem = tListItem
                        If IsComboBox Then
                            tListItem.Background = Application.Current.Resources("PrimaryHueDarkBrush")
                            tListItem.Foreground = Application.Current.Resources("PrimaryHueDarkForegroundBrush")
                        End If
                    End If


                    CodeIndexerList.Items.Add(tListItem)
                End If
                CodeIndexerList.ScrollIntoView(SelectItem)
            Case ESortType.Tree
                CodeIndexerTree.Items.Clear()
                TreeviewItemDic.Clear()


                CodeIndexerTree.BeginInit()
                For i = 0 To ObjectNames.Count - 1
                    Dim Codename As String = ObjectNames(i)
                    Dim CodeGroup As String
                    Dim Imageborder As Border

                    If CurrentPage <= SCDatFiles.DatFiles.orders Then
                        Imageborder = ObjectImages(i)
                    Else
                        Imageborder = Nothing
                    End If


                    If CurrentPage = SCDatFiles.DatFiles.sfxdata Then
                        CodeGroup = Codename
                        Codename = Codename.Split("\").Last

                        CodeGroup = CodeGroup.Replace(Codename, "")

                    Else
                        If CurrentPage = SCDatFiles.DatFiles.ButtonData Then
                            CodeGroup = ""
                        Else
                            CodeGroup = pjData.Dat.Group(CurrentPage, i)
                        End If
                    End If

                    If Fliter.fliterText IsNot Nothing Then
                        If Fliter.fliterText = "" Or Codename.ToLower.IndexOf(Fliter.fliterText.ToLower) >= 0 Then
                            AddTreeList(CodeIndexerTree, CodeGroup, Codename, Imageborder, Fliter.IsIcon, i, i = StartIndex)
                        End If
                    Else
                        AddTreeList(CodeIndexerTree, CodeGroup, Codename, Imageborder, Fliter.IsIcon, i, i = StartIndex)
                    End If


                Next
                CodeIndexerTree.EndInit()
        End Select
    End Sub


    Public Sub RefreshTreeviewItem(key As SCDatFiles.DatFiles, ObjectID As String)
        If key = CurrentPage And Fliter.SortType = ESortType.Tree Then
            Dim TargetItem As TreeViewItem = TreeviewItemDic(ObjectID)
            Dim _ParentTreeview As Object = DeleteMe(TargetItem)

            While _ParentTreeview IsNot Nothing
                Dim parentTreeview As TreeViewItem = _ParentTreeview
                If parentTreeview.Items.Count = 0 Then
                    _ParentTreeview = DeleteMe(parentTreeview)
                Else
                    _ParentTreeview = parentTreeview.Parent
                    If _ParentTreeview.GetType = GetType(TreeView) Then
                        Exit While
                    End If
                End If
            End While  '부모가 있을 경우
            '만약에 부모의 자식이 하나도 없을 경우 그 부모도 지운다.(이걸 부모가 TreeView일때까지 반복



            MoveTreeList(CodeIndexerTree, TargetItem)
            CodeIndexerTree.Items.Add(TargetItem)
            'TreeviewItemDic(ObjectID).Header = "ㅎㅎ"
        End If
    End Sub
    Private Function DeleteMe(TargetItem As TreeViewItem) As TreeViewItem
        If TargetItem.Parent.GetType = GetType(TreeViewItem) Then
            Dim parentTreeview As TreeViewItem = TargetItem.Parent
            parentTreeview.Items.Remove(TargetItem)
            Return parentTreeview
        ElseIf TargetItem.Parent.GetType = GetType(TreeView) Then
            Dim parentTreeview As TreeView = TargetItem.Parent
            parentTreeview.Items.Remove(TargetItem)
            Return Nothing
        End If
        Return Nothing
    End Function



    'Dim TempTarget As TreeViewItem = parrentTreeview
    'If TempTarget.Items.Count = 0 Then '부모의 수가 아무것도 없을 경우 자신을 지운다.



    'If TempTarget.Parent.GetType = GetType(TreeViewItem) Then
    'ElseIf TempTarget.Parent.GetType = GetType(TreeView) Then
    'End If

    'End If


    Private TreeviewItemDic As Dictionary(Of Integer, TreeViewItem)
    Private Sub MoveTreeList(tv As TreeView, TreeItem As TreeViewItem)
        Dim index As Integer = TreeItem.Tag
        Dim itemPath As String = pjData.Dat.Group(CurrentPage, index)
        Dim groups As String() = itemPath.Split({"\"}, StringSplitOptions.RemoveEmptyEntries)

        Dim ItempColl As ItemCollection = tv.Items
        Dim SelectTreeitem As TreeViewItem = Nothing

        Dim PassGroup As Boolean = False


        For gindex = 0 To groups.Count - 1
            PassGroup = False
            For i = 0 To ItempColl.Count - 1
                Dim ttreeitem As TreeViewItem = ItempColl(i)
                If groups(gindex) = ttreeitem.Header.ToString Then
                    SelectTreeitem = ttreeitem
                    ItempColl = SelectTreeitem.Items
                    PassGroup = True

                    Exit For
                End If
            Next
            If Not PassGroup Then
                Dim ttreeitem As New TreeViewItem()

                ttreeitem.Header = groups(gindex)
                ItempColl.Add(ttreeitem)

                SelectTreeitem = ttreeitem
                ItempColl = SelectTreeitem.Items
            End If
        Next

        ItempColl.Add(TreeItem)
    End Sub
    Private Sub AddTreeList(tv As TreeView, itemPath As String, itemName As String, timage As Border, Isbig As Boolean, index As Integer, isSelect As Boolean)
        Dim groups As String() = itemPath.Split({"\"}, StringSplitOptions.RemoveEmptyEntries)

        Dim ItempColl As ItemCollection = tv.Items
        Dim SelectTreeitem As TreeViewItem = Nothing

        Dim PassGroup As Boolean = False


        For gindex = 0 To groups.Count - 1
            PassGroup = False
            For i = 0 To ItempColl.Count - 1
                Dim ttreeitem As TreeViewItem = ItempColl(i)
                If groups(gindex) = ttreeitem.Header.ToString Then
                    SelectTreeitem = ttreeitem
                    ItempColl = SelectTreeitem.Items
                    PassGroup = True

                    Exit For
                End If
            Next
            If Not PassGroup Then
                Dim ttreeitem As New TreeViewItem()

                ttreeitem.Header = groups(gindex)
                ItempColl.Add(ttreeitem)

                SelectTreeitem = ttreeitem
                ItempColl = SelectTreeitem.Items
            End If
        Next

        Dim CodeItem As New TreeViewItem()
        '만약 
        If Isbig Then
            If ItempColl.Count = 0 Then
                If SelectTreeitem Is Nothing Then
                    tv.ItemsPanel = itemTemplate
                Else
                    SelectTreeitem.ItemsPanel = itemTemplate
                End If
            End If




            CodeItem.Header = timage
            CodeItem.Margin = New Thickness(0, 0, -32, 0)

            CodeItem.Tag = index
            ItempColl.Add(CodeItem)

            TreeviewItemDic.Add(index, CodeItem)
        Else
            Dim textblock As New TextBlock
            textblock.TextWrapping = TextWrapping.WrapWithOverflow
            textblock.Padding = New Thickness(15, 8, 0, 0)

            If CurrentPage <= SCDatFiles.DatFiles.orders Then
                Dim NameBinding As Binding = New Binding("Name")
                NameBinding.Source = pjData.BindingManager.UIManager(CurrentPage, index)
                textblock.SetBinding(TextBlock.TextProperty, NameBinding)
            Else
                textblock.Text = itemName
            End If




            Dim stackpanel As New StackPanel
            stackpanel.Margin = New Thickness(-8, -8, -12, -8)
            stackpanel.Height = 32
            stackpanel.Width = 500
            stackpanel.Orientation = Orientation.Horizontal
            If timage IsNot Nothing Then
                timage.Margin = New Thickness(timage.Margin.Left + 8, timage.Margin.Top, timage.Margin.Right, timage.Margin.Bottom)
                stackpanel.Children.Add(timage)
            End If

            stackpanel.Children.Add(textblock)



            CodeItem.Header = stackpanel
            CodeItem.Tag = index
            ItempColl.Add(CodeItem)

            If isSelect And IsComboBox Then
                CodeItem.Background = Application.Current.Resources("PrimaryHueDarkBrush")
                stackpanel.Background = Application.Current.Resources("PrimaryHueDarkBrush")
                textblock.Foreground = Application.Current.Resources("PrimaryHueDarkForegroundBrush")

                Dim _ParentTreeview As Object = CodeItem.Parent
                While _ParentTreeview.GetType = GetType(TreeViewItem)
                    Dim parrentTreeview As TreeViewItem = _ParentTreeview
                    parrentTreeview.IsExpanded = True

                    _ParentTreeview = parrentTreeview.Parent
                End While



            End If
            TreeviewItemDic.Add(index, CodeItem)
        End If

    End Sub




    Private Sub ToolBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Fliter.IsIcon = L_IconBtn.IsSelected
        Fliter.IsEdit = L_IsEditBtn.IsSelected

        ListReset(SCDatFiles.DatFiles.None, IsComboBox, StartIndex)
    End Sub


    Private Sub FliterKeyDown(sender As Object, e As KeyEventArgs)
        If (e.Key = Key.Return) Then
            Fliter.fliterText = FliterText.Text
            ListReset(SCDatFiles.DatFiles.None, IsComboBox, StartIndex)
        End If
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        FliterText.Text = ""
        Fliter.fliterText = ""
        ListReset(SCDatFiles.DatFiles.None, IsComboBox, StartIndex)
    End Sub


    Private Sub ListBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If LoadCmp Then
            Select Case SortListBox.SelectedIndex
                Case 0
                    SetFliter(ESortType.n123)
                    ListReset(SCDatFiles.DatFiles.None, IsComboBox, StartIndex)
                Case 1
                    SetFliter(ESortType.ABC)
                    ListReset(SCDatFiles.DatFiles.None, IsComboBox, StartIndex)
                Case 2
                    SetFliter(ESortType.Tree)
                    ListReset(SCDatFiles.DatFiles.None, IsComboBox, StartIndex)
                Case -1
                    LoadCmp = False
                    SortListBox.SelectedIndex = Fliter.SortType
                    LoadCmp = True
            End Select

        End If
    End Sub

End Class

