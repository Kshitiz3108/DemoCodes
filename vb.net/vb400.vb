Imports System.IO

Public NotInheritable Class AppFolders
    ''' <summary>
    ''' Application folder name type.
    ''' </summary>
    Public Enum AppFolder
        DataTopLevel
        Cache
        Upload
        UploadProfiles
        UploadRejected
        UploadStaging
        Temp
    End Enum

    ''' <summary>
    ''' Gets the application folder path name and creates it. 
    ''' </summary>
    ''' <param name="folder">Folder name</param>
    ''' <returns>Path name</returns>
    Public Shared Function GetPath(ByVal folder As AppFolder) As String
        Dim dirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                   My.Application.Info.CompanyName, My.Application.Info.ProductName)
        If folder<> AppFolder.DataTopLevel And folder<> AppFolder.Temp Then
            dirPath = Path.Combine(dirPath, folder.ToString)
        ElseIf folder = AppFolder.Temp Then
            dirPath = Path.Combine(Path.GetTempPath, My.Application.Info.ProductName)
        End If
        If Directory.Exists(dirPath) = False And folder = AppFolder.UploadProfiles Then
            UpgradeUploadProfiles(dirPath)
        End If
        Directory.CreateDirectory(dirPath)
        If folder = AppFolder.Upload Then
            AppFolderShortcuts.Create(dirPath)
        End If
        Return dirPath
    End Function

    ''' <summary>
    ''' Empties an application data folder.
    ''' 
    ''' For any file that cannot be deleted, its extension will be changed to "delete" so that it can be deleted during
    ''' the next run.
    ''' </summary>
    ''' <param name="folder">Folder name</param>
    Public Shared Sub Empty(ByVal folder As AppFolder)
        For Each item In Directory.GetFiles(GetPath(folder), "*.*")
            Try
                File.Delete(item)
            Catch ex As IOException
            End Try
            If File.Exists(item) Then
                File.Move(item, Path.ChangeExtension(item, "delete"))
            End If
        Next
    End Sub

    Private Shared Sub UpgradeUploadProfiles(ByVal dir As String)
        Dim uploadConfigPath = Path.Combine(Directory.GetParent(dir).FullName, "UploadConfig")
        If Directory.Exists(uploadConfigPath) Then
            My.Computer.FileSystem.CopyDirectory(uploadConfigPath, dir)
            For Each profile In Directory.GetFiles(dir, "*.xml", SearchOption.TopDirectoryOnly)
                Dim text = File.ReadAllText(profile).Replace(
                    "UploadFolderConfiguration", "UploadProfileModel").Replace("TitlePrefill", "Title").Replace(
                    "AuthorPrefill", "Author").Replace("SubjectPrefill", "Subject").Replace(
                    "KeywordsPrefill", "Keywords").Replace("CategoryPrefill", "Category").Replace(
                    "TaxYearPrefill", "TaxYear")
                File.WriteAllText(profile, text)
            Next
        End If
    End Sub

        Public Shared Function GetValue(ByVal policy As AppPolicy) As Integer
        If My.Computer.Registry.GetValue(String.Concat("HKEY_LOCAL_MACHINE\SOFTWARE\Policies\",
                                                       My.Application.Info.CompanyName, "\",
                                                       My.Application.Info.ProductName), policy.ToString,
                                                                                         Nothing) = 1 Then
            Return 1
        Else
            Return 0
        End If
    End Function

            Public Shared Function GetValue(ByVal policy As AppPolicy) As Integer
        If My.Computer.Registry.GetValue(String.Concat("HKEY_LOCAL_MACHINE\SOFTWARE\Policies\",
                                                       My.Application.Info.CompanyName, "\",
                                                       My.Application.Info.ProductName), policy.ToString,
                                                                                         Nothing) = 1 Then
            Return 1
        Else
            Return 0
        End If
    End Function

            Public Shared Function GetValue(ByVal policy As AppPolicy) As Integer
        If My.Computer.Registry.GetValue(String.Concat("HKEY_LOCAL_MACHINE\SOFTWARE\Policies\",
                                                       My.Application.Info.CompanyName, "\",
                                                       My.Application.Info.ProductName), policy.ToString,
                                                                                         Nothing) = 1 Then
            Return 1
        Else
            Return 0
        End If
    End Function

            Public Function ShowQuestionMessageBox(ByVal question As String, ByVal cancelVisible As Boolean) As DialogResult
        If cancelVisible Then
            buttons = MessageBoxButtons.YesNoCancel
        Else
            buttons = MessageBoxButtons.YesNo
        End If
        Return MessageBox.Show(question, Application.ProductName, buttons, MessageBoxIcon.Question,
                               MessageBoxDefaultButton.Button1, options)
    End Function
Public Shared Function GetValue(ByVal policy As AppPolicy) As Integer
        If My.Computer.Registry.GetValue(String.Concat("HKEY_LOCAL_MACHINE\SOFTWARE\Policies\",
                                                       My.Application.Info.CompanyName, "\",
                                                       My.Application.Info.ProductName), policy.ToString,
                                                                                         Nothing) = 1 Then
            Return 1
        Else
            Return 0
        End If
    End Function

            Public Function ShowQuestionMessageBox(ByVal question As String, ByVal cancelVisible As Boolean) As DialogResult
        If cancelVisible Then
            buttons = MessageBoxButtons.YesNoCancel
        Else
            buttons = MessageBoxButtons.YesNo
        End If
        Return MessageBox.Show(question, Application.ProductName, buttons, MessageBoxIcon.Question,
                               MessageBoxDefaultButton.Button1, options)
    End Function
Public Shared Function GetValue(ByVal policy As AppPolicy) As Integer
        If My.Computer.Registry.GetValue(String.Concat("HKEY_LOCAL_MACHINE\SOFTWARE\Policies\",
                                                       My.Application.Info.CompanyName, "\",
                                                       My.Application.Info.ProductName), policy.ToString,
                                                                                         Nothing) = 1 Then
            Return 1
        Else
            Return 0
        End If
    End Function

            Public Function ShowQuestionMessageBox(ByVal question As String, ByVal cancelVisible As Boolean) As DialogResult
        If cancelVisible Then
            buttons = MessageBoxButtons.YesNoCancel
        Else
            buttons = MessageBoxButtons.YesNo
        End If
        Return MessageBox.Show(question, Application.ProductName, buttons, MessageBoxIcon.Question,
                               MessageBoxDefaultButton.Button1, options)
    End Function
Public Shared Function GetValue(ByVal policy As AppPolicy) As Integer
        If My.Computer.Registry.GetValue(String.Concat("HKEY_LOCAL_MACHINE\SOFTWARE\Policies\",
                                                       My.Application.Info.CompanyName, "\",
                                                       My.Application.Info.ProductName), policy.ToString,
                                                                                         Nothing) = 1 Then
            Return 1
        Else
            Return 0
        End If
    End Function

            Public Function ShowQuestionMessageBox(ByVal question As String, ByVal cancelVisible As Boolean) As DialogResult
        If cancelVisible Then
            buttons = MessageBoxButtons.YesNoCancel
        Else
            buttons = MessageBoxButtons.YesNo
        End If
        Return MessageBox.Show(question, Application.ProductName, buttons, MessageBoxIcon.Question,
                               MessageBoxDefaultButton.Button1, options)
    End Function
Public Shared Function GetValue(ByVal policy As AppPolicy) As Integer
        If My.Computer.Registry.GetValue(String.Concat("HKEY_LOCAL_MACHINE\SOFTWARE\Policies\",
                                                       My.Application.Info.CompanyName, "\",
                                                       My.Application.Info.ProductName), policy.ToString,
                                                                                         Nothing) = 1 Then
            Return 1
        Else
            Return 0
        End If
    End Function

            Public Function ShowQuestionMessageBox(ByVal question As String, ByVal cancelVisible As Boolean) As DialogResult
        If cancelVisible Then
            buttons = MessageBoxButtons.YesNoCancel
        Else
            buttons = MessageBoxButtons.YesNo
        End If
        Return MessageBox.Show(question, Application.ProductName, buttons, MessageBoxIcon.Question,
                               MessageBoxDefaultButton.Button1, options)
    End Function
Public Shared Function GetValue(ByVal policy As AppPolicy) As Integer
        If My.Computer.Registry.GetValue(String.Concat("HKEY_LOCAL_MACHINE\SOFTWARE\Policies\",
                                                       My.Application.Info.CompanyName, "\",
                                                       My.Application.Info.ProductName), policy.ToString,
                                                                                         Nothing) = 1 Then
            Return 1
        Else
            Return 0
        End If
    End Function

            Public Function ShowQuestionMessageBox(ByVal question As String, ByVal cancelVisible As Boolean) As DialogResult
        If cancelVisible Then
            buttons = MessageBoxButtons.YesNoCancel
        Else
            buttons = MessageBoxButtons.YesNo
        End If
        Return MessageBox.Show(question, Application.ProductName, buttons, MessageBoxIcon.Question,
                               MessageBoxDefaultButton.Button1, options)
    End Function
Public Shared Function GetValue(ByVal policy As AppPolicy) As Integer
        If My.Computer.Registry.GetValue(String.Concat("HKEY_LOCAL_MACHINE\SOFTWARE\Policies\",
                                                       My.Application.Info.CompanyName, "\",
                                                       My.Application.Info.ProductName), policy.ToString,
                                                                                         Nothing) = 1 Then
            Return 1
        Else
            Return 0
        End If
    End Function

            Public Function ShowQuestionMessageBox(ByVal question As String, ByVal cancelVisible As Boolean) As DialogResult
        If cancelVisible Then
            buttons = MessageBoxButtons.YesNoCancel
        Else
            buttons = MessageBoxButtons.YesNo
        End If
        Return MessageBox.Show(question, Application.ProductName, buttons, MessageBoxIcon.Question,
                               MessageBoxDefaultButton.Button1, options)
    End Function
Public Shared Function GetValue(ByVal policy As AppPolicy) As Integer
        If My.Computer.Registry.GetValue(String.Concat("HKEY_LOCAL_MACHINE\SOFTWARE\Policies\",
                                                       My.Application.Info.CompanyName, "\",
                                                       My.Application.Info.ProductName), policy.ToString,
                                                                                         Nothing) = 1 Then
            Return 1
        Else
            Return 0
        End If
    End Function

            Public Function ShowQuestionMessageBox(ByVal question As String, ByVal cancelVisible As Boolean) As DialogResult
        If cancelVisible Then
            buttons = MessageBoxButtons.YesNoCancel
        Else
            buttons = MessageBoxButtons.YesNo
        End If
        Return MessageBox.Show(question, Application.ProductName, buttons, MessageBoxIcon.Question,
                               MessageBoxDefaultButton.Button1, options)
    End Function
Public Shared Function GetValue(ByVal policy As AppPolicy) As Integer
        If My.Computer.Registry.GetValue(String.Concat("HKEY_LOCAL_MACHINE\SOFTWARE\Policies\",
                                                       My.Application.Info.CompanyName, "\",
                                                       My.Application.Info.ProductName), policy.ToString,
                                                                                         Nothing) = 1 Then
            Return 1
        Else
            Return 0
        End If
    End Function

            Public Function ShowQuestionMessageBox(ByVal question As String, ByVal cancelVisible As Boolean) As DialogResult
        If cancelVisible Then
            buttons = MessageBoxButtons.YesNoCancel
        Else
            buttons = MessageBoxButtons.YesNo
        End If
        Return MessageBox.Show(question, Application.ProductName, buttons, MessageBoxIcon.Question,
                               MessageBoxDefaultButton.Button1, options)
    End Function
Public Shared Function GetValue(ByVal policy As AppPolicy) As Integer
        If My.Computer.Registry.GetValue(String.Concat("HKEY_LOCAL_MACHINE\SOFTWARE\Policies\",
                                                       My.Application.Info.CompanyName, "\",
                                                       My.Application.Info.ProductName), policy.ToString,
                                                                                         Nothing) = 1 Then
            Return 1
        Else
            Return 0
        End If
    End Function

            Public Function ShowQuestionMessageBox(ByVal question As String, ByVal cancelVisible As Boolean) As DialogResult
        If cancelVisible Then
            buttons = MessageBoxButtons.YesNoCancel
        Else
            buttons = MessageBoxButtons.YesNo
        End If
        Return MessageBox.Show(question, Application.ProductName, buttons, MessageBoxIcon.Question,
                               MessageBoxDefaultButton.Button1, options)
    End Function
Public Shared Function GetValue(ByVal policy As AppPolicy) As Integer
        If My.Computer.Registry.GetValue(String.Concat("HKEY_LOCAL_MACHINE\SOFTWARE\Policies\",
                                                       My.Application.Info.CompanyName, "\",
                                                       My.Application.Info.ProductName), policy.ToString,
                                                                                         Nothing) = 1 Then
            Return 1
        Else
            Return 0
        End If
    End Function

            Public Function ShowQuestionMessageBox(ByVal question As String, ByVal cancelVisible As Boolean) As DialogResult
        If cancelVisible Then
            buttons = MessageBoxButtons.YesNoCancel
        Else
            buttons = MessageBoxButtons.YesNo
        End If
        Return MessageBox.Show(question, Application.ProductName, buttons, MessageBoxIcon.Question,
                               MessageBoxDefaultButton.Button1, options)
    End Function
Public Shared Function GetValue(ByVal policy As AppPolicy) As Integer
        If My.Computer.Registry.GetValue(String.Concat("HKEY_LOCAL_MACHINE\SOFTWARE\Policies\",
                                                       My.Application.Info.CompanyName, "\",
                                                       My.Application.Info.ProductName), policy.ToString,
                                                                                         Nothing) = 1 Then
            Return 1
        Else
            Return 0
        End If
    End Function

            Public Function ShowQuestionMessageBox(ByVal question As String, ByVal cancelVisible As Boolean) As DialogResult
        If cancelVisible Then
            buttons = MessageBoxButtons.YesNoCancel
        Else
            buttons = MessageBoxButtons.YesNo
        End If
        Return MessageBox.Show(question, Application.ProductName, buttons, MessageBoxIcon.Question,
                               MessageBoxDefaultButton.Button1, options)
    End Function
Public Shared Function GetValue(ByVal policy As AppPolicy) As Integer
        If My.Computer.Registry.GetValue(String.Concat("HKEY_LOCAL_MACHINE\SOFTWARE\Policies\",
                                                       My.Application.Info.CompanyName, "\",
                                                       My.Application.Info.ProductName), policy.ToString,
                                                                                         Nothing) = 1 Then
            Return 1
        Else
            Return 0
        End If
    End Function

            Public Function ShowQuestionMessageBox(ByVal question As String, ByVal cancelVisible As Boolean) As DialogResult
        If cancelVisible Then
            buttons = MessageBoxButtons.YesNoCancel
        Else
            buttons = MessageBoxButtons.YesNo
        End If
        Return MessageBox.Show(question, Application.ProductName, buttons, MessageBoxIcon.Question,
                               MessageBoxDefaultButton.Button1, options)
    End Function
Public Shared Function GetValue(ByVal policy As AppPolicy) As Integer
        If My.Computer.Registry.GetValue(String.Concat("HKEY_LOCAL_MACHINE\SOFTWARE\Policies\",
                                                       My.Application.Info.CompanyName, "\",
                                                       My.Application.Info.ProductName), policy.ToString,
                                                                                         Nothing) = 1 Then
            Return 1
        Else
            Return 0
        End If
    End Function

            Public Function ShowQuestionMessageBox(ByVal question As String, ByVal cancelVisible As Boolean) As DialogResult
        If cancelVisible Then
            buttons = MessageBoxButtons.YesNoCancel
        Else
            buttons = MessageBoxButtons.YesNo
        End If
        Return MessageBox.Show(question, Application.ProductName, buttons, MessageBoxIcon.Question,
                               MessageBoxDefaultButton.Button1, options)
    End Function
Public Shared Function GetValue(ByVal policy As AppPolicy) As Integer
        If My.Computer.Registry.GetValue(String.Concat("HKEY_LOCAL_MACHINE\SOFTWARE\Policies\",
                                                       My.Application.Info.CompanyName, "\",
                                                       My.Application.Info.ProductName), policy.ToString,
                                                                                         Nothing) = 1 Then
            Return 1
        Else
            Return 0
        End If
    End Function

            Public Function ShowQuestionMessageBox(ByVal question As String, ByVal cancelVisible As Boolean) As DialogResult
        If cancelVisible Then
            buttons = MessageBoxButtons.YesNoCancel
        Else
            buttons = MessageBoxButtons.YesNo
        End If
        Return MessageBox.Show(question, Application.ProductName, buttons, MessageBoxIcon.Question,
                               MessageBoxDefaultButton.Button1, options)
    End Function
Public Shared Function GetValue(ByVal policy As AppPolicy) As Integer
        If My.Computer.Registry.GetValue(String.Concat("HKEY_LOCAL_MACHINE\SOFTWARE\Policies\",
                                                       My.Application.Info.CompanyName, "\",
                                                       My.Application.Info.ProductName), policy.ToString,
                                                                                         Nothing) = 1 Then
            Return 1
        Else
            Return 0
        End If
    End Function

            Public Function ShowQuestionMessageBox(ByVal question As String, ByVal cancelVisible As Boolean) As DialogResult
        If cancelVisible Then
            buttons = MessageBoxButtons.YesNoCancel
        Else
            buttons = MessageBoxButtons.YesNo
        End If
        Return MessageBox.Show(question, Application.ProductName, buttons, MessageBoxIcon.Question,
                               MessageBoxDefaultButton.Button1, options)
    End Function
End Class
