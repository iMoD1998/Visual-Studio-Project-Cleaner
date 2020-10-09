using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using System.IO;
using System.Text.RegularExpressions;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Threading;
using MahApps.Metro;
using MahApps.Metro.Converters;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Visual_Studio_Project_Cleaner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public ICommand BrowseFileCommand
        {
            get { return new RelayCommand(BrowseFileAction); }
        }

        public ObservableCollection<VisualStudioTempExtension> FileExtensions { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            FileExtensions = new ObservableCollection<VisualStudioTempExtension>();
            FileExtensions.Add(new VisualStudioTempExtension { Description = "Last resouce editor state", ExtensionFormat = ".aps", Pattern = new Regex(@"^.*\.aps$") });
            FileExtensions.Add(new VisualStudioTempExtension { Description = "Backup file", ExtensionFormat = ".bak", Pattern = new Regex(@"^.*\.bak$") });
            FileExtensions.Add(new VisualStudioTempExtension { Description = "Source Browser Database", ExtensionFormat = ".bsc", Pattern = new Regex(@"^.*\.bsc$") });
            FileExtensions.Add(new VisualStudioTempExtension { Description = "Debugging infomation", ExtensionFormat = ".dbg", Pattern = new Regex(@"^.*\.dbg$") });
            FileExtensions.Add(new VisualStudioTempExtension { Description = "Exports file", ExtensionFormat = ".exp", Pattern = new Regex(@"^.*\.exp$") });
            FileExtensions.Add(new VisualStudioTempExtension { Description = "Minimum rebuild linker file", ExtensionFormat = ".idb", Pattern = new Regex(@"^.*\.idb$") });
            FileExtensions.Add(new VisualStudioTempExtension { Description = "Incremental linker file", ExtensionFormat = ".iik", Pattern = new Regex(@"^.*\.iik$") });
            FileExtensions.Add(new VisualStudioTempExtension { Description = "Incremental precompiled header", ExtensionFormat = ".ipch", Pattern = new Regex(@"^.*\.ipch$") });
            FileExtensions.Add(new VisualStudioTempExtension { Description = "Build helper", ExtensionFormat = ".lastbuildstate", Pattern = new Regex(@"^.*\.lastbuildstate$") });
            FileExtensions.Add(new VisualStudioTempExtension { Description = "Build log", ExtensionFormat = ".log", Pattern = new Regex(@"^.*\.log$") });
            FileExtensions.Add(new VisualStudioTempExtension { Description = "Linker MAP file", ExtensionFormat = ".map", Pattern = new Regex(@"^.*\.map$") });
            FileExtensions.Add(new VisualStudioTempExtension { Description = "Intellisense database", ExtensionFormat = ".ncb", Pattern = new Regex(@"^.*\.ncb$") });
            FileExtensions.Add(new VisualStudioTempExtension { Description = "Object file", ExtensionFormat = ".o/.obj", Pattern = new Regex(@"^.*\.(obj|o)$") });
            FileExtensions.Add(new VisualStudioTempExtension { Description = "Visual Studio options", ExtensionFormat = ".opt", Pattern = new Regex(@"^.*\.opt$") });
            FileExtensions.Add(new VisualStudioTempExtension { Description = "Pre Compiled Header", ExtensionFormat = ".pch", Pattern = new Regex(@"^.*\.pch$") });
            FileExtensions.Add(new VisualStudioTempExtension { Description = "Compiled Resource", ExtensionFormat = ".res", Pattern = new Regex(@"^.*\.res$") });
            FileExtensions.Add(new VisualStudioTempExtension { Description = "Source Browser infomation", ExtensionFormat = ".sbr", Pattern = new Regex(@"^.*\.sbr$") });
            FileExtensions.Add(new VisualStudioTempExtension { Description = "Intellisense database", ExtensionFormat = ".sdf", Pattern = new Regex(@"^.*\.sdf$") });
            FileExtensions.Add(new VisualStudioTempExtension { Description = "Intellisense database", ExtensionFormat = ".VC.db", Pattern = new Regex(@"^.*\.VC.db$") });
            FileExtensions.Add(new VisualStudioTempExtension { Description = "Dependency file infomation", ExtensionFormat = ".tlog", Pattern = new Regex(@"^.*\.tlog$") });

            /*Some of these are risky if you dont compile libs*/
            //FileExtensions.Add(new VisualStudioTempExtension { Description = "Static library", ExtensionFormat = ".lib", Pattern = new Regex(@"^.*\.lib$") });
            //FileExtensions.Add(new VisualStudioTempExtension { Description = "Dynamic link library", ExtensionFormat = ".dll", Pattern = new Regex(@"^.*\.dll$") });
            FileExtensions.Add(new VisualStudioTempExtension { Description = "Executable", ExtensionFormat = ".exe", Pattern = new Regex(@"^.*\.exe$") });
            FileExtensions.Add(new VisualStudioTempExtension { Description = "Xbox Executable", ExtensionFormat = ".xex", Pattern = new Regex(@"^.*\.xex$") });
            //FileExtensions.Add(new VisualStudioTempExtension { Description = "Program database", ExtensionFormat = ".pdb", Pattern = new Regex(@"^.*\.pdb$") });

            this.DataContext = this;
        }

        private void BrowseFileAction(object obj)
        {
            var FolderDialog = new CommonOpenFileDialog();
            FolderDialog.Title = "Select Projects Folder";
            FolderDialog.IsFolderPicker = true;

            FolderDialog.AddToMostRecentlyUsedList = false;
            FolderDialog.AllowNonFileSystemItems = false;
            FolderDialog.EnsureFileExists = true;
            FolderDialog.EnsurePathExists = true;
            FolderDialog.EnsureReadOnly = false;
            FolderDialog.EnsureValidNames = true;
            FolderDialog.Multiselect = false;
            FolderDialog.ShowPlacesList = true;

            if (FolderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.PathBox.Text = FolderDialog.FileName;
            }
        }

        async Task<List<Tuple<VisualStudioTempFile, Exception>>> DeleteItems( List<VisualStudioTempFile> Files )
        {
            int CurrentIndex = 0;

            var Controller = await this.ShowProgressAsync("Please wait...", "");

            List<Tuple<VisualStudioTempFile, Exception>> Errors = new List<Tuple<VisualStudioTempFile, Exception>>();

            foreach (var TempFile in Files)
            {
                try
                {
                    Controller.SetProgress((double)CurrentIndex / (double)Files.Count());
                    Controller.SetMessage($"Deleting { TempFile.Path }");

                    File.Delete(TempFile.Path);
                }
                catch (Exception E)
                {
                    Errors.Add(new Tuple<VisualStudioTempFile, Exception>(TempFile, E));
                }

                CurrentIndex++;
            }

            await Controller.CloseAsync();

            return Errors;
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.FileListDataGrid.SelectedItems.Count > 0)
            {
                var UserInput = await this.ShowMessageAsync("Are you sure?", $"Are you sure you want to delete {this.FileListDataGrid.SelectedItems.Count} selected file(s)", MessageDialogStyle.AffirmativeAndNegative);

                if(UserInput == MessageDialogResult.Affirmative)
                {
                    var ItemsToDelete     = this.FileListDataGrid.SelectedItems.OfType<VisualStudioTempFile>().ToList();
                    var TotalSize         = ItemsToDelete.Sum(X => X.Size);
                    var FailedDeletions   = await DeleteItems(ItemsToDelete);
                    var TotalDeletedSize  = TotalSize - FailedDeletions.Sum(X => X.Item1.Size);
                    var TotalDeletedFiles = ItemsToDelete.Count() - FailedDeletions.Count();

                    if (FailedDeletions.Count() == 0)
                    {
                        await this.ShowMessageAsync("Success", $"Deleted { TotalDeletedFiles } file(s) saving { Util.ConvertBytesToString(TotalDeletedSize) }");
                    }
                    else
                    {
                        string Errors = string.Join("\n", FailedDeletions.Select(X => $"{X.Item1.Path} - { X.Item2.Message }"));

                        await this.ShowMessageAsync("Some files failed", Errors);
                    }
                }
            }
            else
            {
                if (this.FileListDataGrid.Items.Count == 0)
                    return;

                var UserInput = await this.ShowMessageAsync("Are you sure?", $"Are you sure you want to delete {this.FileListDataGrid.Items.Count} file(s)", MessageDialogStyle.AffirmativeAndNegative);

                if (UserInput == MessageDialogResult.Affirmative)
                {
                    var ItemsToDelete     = this.FileListDataGrid.Items.OfType<VisualStudioTempFile>().ToList();
                    var TotalSize         = ItemsToDelete.Sum(X => X.Size);
                    var FailedDeletions   = await DeleteItems(ItemsToDelete);
                    var TotalDeletedSize  = TotalSize - FailedDeletions.Sum(X => X.Item1.Size);
                    var TotalDeletedFiles = ItemsToDelete.Count() - FailedDeletions.Count();

                    if (FailedDeletions.Count() == 0)
                    {
                        await this.ShowMessageAsync("Success", $"Deleted { TotalDeletedFiles } file(s) saving { Util.ConvertBytesToString(TotalDeletedSize) }");
                    }
                    else
                    {
                        string Errors = string.Join("\n", FailedDeletions.Select(X => $"{X.Item1.Path} - { X.Item2.Message }"));

                        await this.ShowMessageAsync("Some files failed", Errors);
                    }
                }
            }
        }

        private async Task<List<VisualStudioTempFile>> GetFiles(string Path, bool ScanSubDirs, List<VisualStudioTempExtension> ExtensionsToSearch)
        {
            List<VisualStudioTempFile> TempFiles = new List<VisualStudioTempFile>();

            var FileNames         = Directory.GetFiles(Path, "*", ScanSubDirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            var EnabledExtensions = ExtensionsToSearch.Where(X => X.Enabled);

            /*Can be optimized?*/
            foreach (var FileName in FileNames)
            {
                foreach (var Extension in EnabledExtensions)
                {
                    if (Extension.Pattern.IsMatch(FileName))
                    {
                        //this.FileListDataGrid.Items.Add(new VisualStudioTempFile { Path = FileName, Size = new FileInfo(FileName).Length });
                        TempFiles.Add(new VisualStudioTempFile { Path = FileName, Size = new FileInfo(FileName).Length });
                        break;
                    }
                }
            }

            return TempFiles;
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.FileListDataGrid.Items.Clear();

            if (!Directory.Exists(PathBox.Text))
                return;

            string Path                   = PathBox.Text;
            bool   ScanSubDirs            = ScanSubDirsCheckBox.IsEnabled;
            var    FileExtensionsToSearch = this.FileExtensions.ToList();

            if (!FileExtensionsToSearch.Any(X => X.Enabled))
            {
                await this.ShowMessageAsync("No filetypes selected", "Select a filetype in settings -> file types ");

                this.SettingsFlyout.IsOpen = true;

                return;
            }

            var Controller = await this.ShowProgressAsync("Scanning files...", "Please wait...");
            Controller.SetIndeterminate();

            try
            {
                List<VisualStudioTempFile> TempFiles = await Task.Run(() => GetFiles(Path, ScanSubDirs, FileExtensionsToSearch));

                TempFiles.ForEach(X => this.FileListDataGrid.Items.Add(X));

                this.FileStatus.Content = $"{this.FileListDataGrid.Items.Count} file(s) { Util.ConvertBytesToString(this.FileListDataGrid.Items.OfType<VisualStudioTempFile>().ToList().Sum(X => X.Size)) }";
                
                await Controller.CloseAsync();
            }
            catch ( Exception E )
            {
                if(Controller.IsOpen == true)
                {
                    await Controller.CloseAsync();
                }

                if (E is UnauthorizedAccessException)
                {
                    await this.ShowMessageAsync("Error", "Not enough permissions to access " + PathBox.Text);
                }

                this.FileStatus.Content = $"0 file(s) 0 bytes";
            }
        }

        private void FileListDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.FileListDataGrid.SelectedItems.Count > 0)
            {
                this.DataGridContextMenu.IsEnabled = true;
                this.SelectedFilesStatus.Visibility = Visibility.Visible;

                long TotalSize = 0;

                foreach (var File in this.FileListDataGrid.SelectedItems.OfType<VisualStudioTempFile>())
                {
                    TotalSize += File.Size;
                }

                this.SelectedFilesStatus.Content = $"{this.FileListDataGrid.SelectedItems.Count} file(s) selected { Util.ConvertBytesToString(TotalSize) }";
            }
            else
            {
                this.DataGridContextMenu.IsEnabled = false;
                this.SelectedFilesStatus.Visibility = Visibility.Hidden;
            }
        }

        private void CopyPathMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.FileListDataGrid.SelectedItems.Count == 1)
            {
                var Item = this.FileListDataGrid.SelectedItem as VisualStudioTempFile;
                Clipboard.SetText(Item.Path);
            }
            else if (this.FileListDataGrid.SelectedItems.Count > 1)
            {
                Clipboard.SetText(String.Join("\n", this.FileListDataGrid.SelectedItems.OfType<VisualStudioTempFile>().Select(X => X.Path)));
            }
        }

        private void OpenFileFolder(string FilePath)
        {
            if (!File.Exists(FilePath))
                return;

            var FileParentDir = new FileInfo(FilePath).Directory.FullName;

            if (Directory.Exists(FileParentDir))
            {
                ProcessStartInfo StartInfo = new ProcessStartInfo
                {
                    Arguments = FileParentDir,
                    FileName = "explorer.exe"
                };

                Process.Start(StartInfo);
            }
        }

        private async void ViewInExplorerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.FileListDataGrid.SelectedItems.Count == 1)
            {
                var Item = this.FileListDataGrid.SelectedItem as VisualStudioTempFile;
                OpenFileFolder(Item.Path);
            }
            else if (this.FileListDataGrid.SelectedItems.Count > 1)
            {
                var UserSelect = await this.ShowMessageAsync("Are you sure?", $"Are you sure you want to open { this.FileListDataGrid.SelectedItems.Count } explorers", MessageDialogStyle.AffirmativeAndNegative);

                if (UserSelect == MessageDialogResult.Affirmative)
                {
                    this.FileListDataGrid.SelectedItems.OfType<VisualStudioTempFile>().ToList().ForEach(X => OpenFileFolder(X.Path));
                }
            }
        }

        bool AllFileTypesCheckboxIgnoreEvent = false;

        private void AllFileTypes_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (AllFileTypesCheckboxIgnoreEvent)
                return;

            var ShouldCheck = this.AllFileTypes.IsChecked == true;

            FileExtensionCheckboxIgnoreEvent = true;

            foreach (var Item in FileExtensions)
            {
                Item.Enabled = ShouldCheck;
            }

            FileExtensionCheckboxIgnoreEvent = false;
        }

        bool FileExtensionCheckboxIgnoreEvent = false;

        private void FileExtensionCheckbox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (FileExtensionCheckboxIgnoreEvent)
                return;

            AllFileTypesCheckboxIgnoreEvent = true;

            var SenderCheckbox = sender as CheckBox;

            this.AllFileTypes.IsChecked = FileExtensions.All(X => X.Enabled == true);

            AllFileTypesCheckboxIgnoreEvent = false;
        }

        private void GithubRepoClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/iMoD1998/Visual-Studio-Project-Cleaner");
        }
    }
}
