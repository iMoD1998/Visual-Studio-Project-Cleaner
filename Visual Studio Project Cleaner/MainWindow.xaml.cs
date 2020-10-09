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
        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public class VisualStudioTempExtension : INotifyPropertyChanged
        {
            public string _ExtensionFormat;
            public string _Description;
            public bool _Enabled;
            public Regex _Pattern;

            public string Text
            {
                get
                {
                    return _Description + " (" + _ExtensionFormat + ")";
                }
            }

            public string ExtensionFormat
            {
                get { return _ExtensionFormat; }
                set {
                    _ExtensionFormat = value;
                    NotifyPropertyChanged();
                }
            }

            public string Description
            {
                get { return _Description; }
                set {
                    _Description = value;
                    NotifyPropertyChanged();
                }
            }
            public bool Enabled
            {
                get { return _Enabled; }
                set {
                    _Enabled = value;
                    NotifyPropertyChanged();
                }
            }

            public Regex Pattern
            {
                get { return _Pattern; }
                set {
                    _Pattern = value;
                    NotifyPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void NotifyPropertyChanged(String PropertyName = "")
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public ObservableCollection<VisualStudioTempExtension> FileExtensions { get; set; }

        static string ConvertBytesToString(long Size)
        {
            if (Size == 0)
                return "0.0 " + SizeSuffixes[0];
            ;
            int Magnitude = (int)Math.Log(Size, 1024);
            double AdjustedSize = (Size / Math.Pow(1024, Magnitude));

            return string.Format("{0:n2} {1}", AdjustedSize, SizeSuffixes[Magnitude]);
        }

        public class VisualStudioTempFile
        {
            string _Path;
            long _Size;

            public string Path
            {
                get { return _Path; }
                set { _Path = value; }
            }

            public long Size
            {
                get { return _Size; }
                set { _Size = value; }
            }

            public string SizeString
            {
                get
                {
                    return ConvertBytesToString(_Size);
                }
            }
        }

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

        public ICommand BrowseFileCommand
        {
            get { return new RelayCommand(BrowseFileAction); }
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
                    await Task.Delay(10);

                    Controller.SetProgress((double)CurrentIndex / (double)Files.Count());
                    Controller.SetMessage($"Deleting { TempFile.Path }");

                    if (CurrentIndex % 10 == 0)
                        throw new Exception("Fuckk off whore");

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
                var UserInput = await this.ShowMessageAsync("Are you sure?", $"Are you sure you want to delete {this.FileListDataGrid.Items.Count} selected file(s)", MessageDialogStyle.AffirmativeAndNegative);

                if(UserInput == MessageDialogResult.Affirmative)
                {
                    var ItemsToDelete = this.FileListDataGrid.SelectedItems.OfType<VisualStudioTempFile>().ToList();
                    var TotalSize = ItemsToDelete.Sum(X => X.Size);
                    var FailedDeletions = await DeleteItems(ItemsToDelete);
                    var TotalDeletedSize = TotalSize - FailedDeletions.Sum(X => X.Item1.Size);
                    var TotalDeletedFiles = ItemsToDelete.Count() - FailedDeletions.Count();

                    if (FailedDeletions.Count() == 0)
                    {
                        await this.ShowMessageAsync("Success", $"Deleted { TotalDeletedFiles } file(s) saving { ConvertBytesToString(TotalDeletedSize) }");
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
                        await this.ShowMessageAsync("Success", $"Deleted { TotalDeletedFiles } file(s) saving { ConvertBytesToString(TotalDeletedSize) }");
                    }
                    else
                    {
                        string Errors = string.Join("\n", FailedDeletions.Select(X => $"{X.Item1.Path} - { X.Item2.Message }"));

                        await this.ShowMessageAsync("Some files failed", Errors);
                    }
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.FileListDataGrid.Items.Clear();

            if (!Directory.Exists(PathBox.Text))
                return;

            var FileNames         = Directory.GetFiles(PathBox.Text, "*", ScanSubDirsCheckBox.IsChecked == true ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            var EnabledExtensions = FileExtensions.Where(X => X._Enabled);

            /*Can be optimized?*/
            foreach (var FileName in FileNames)
            {
                foreach (var Extension in EnabledExtensions)
                {
                    if (Extension.Pattern.IsMatch(FileName))
                    {
                        this.FileListDataGrid.Items.Add(new VisualStudioTempFile { Path = FileName, Size = new FileInfo(FileName).Length });
                        
                        break;
                    }
                }
            }

            this.FileStatus.Content = $"{this.FileListDataGrid.Items.Count} file(s) { ConvertBytesToString(this.FileListDataGrid.Items.OfType<VisualStudioTempFile>().ToList().Sum(X => X.Size)) }";
        }

        bool AllFileTypesCheckboxIgnoreEvent = false;
        bool FileExtensionCheckboxIgnoreEvent = false;

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

        private void FileExtensionCheckbox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (FileExtensionCheckboxIgnoreEvent)
                return;

            AllFileTypesCheckboxIgnoreEvent = true;

            var SenderCheckbox = sender as CheckBox;

            this.AllFileTypes.IsChecked = FileExtensions.All(X => X.Enabled == true);

            AllFileTypesCheckboxIgnoreEvent = false;
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

                this.SelectedFilesStatus.Content = $"{this.FileListDataGrid.SelectedItems.Count} file(s) selected { ConvertBytesToString(TotalSize) }";
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

                if(UserSelect == MessageDialogResult.Affirmative)
                {
                    this.FileListDataGrid.SelectedItems.OfType<VisualStudioTempFile>().ToList().ForEach(X => OpenFileFolder(X.Path));
                }
            }
        }

        private void GithubRepoClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/iMoD1998/Visual-Studio-Project-Cleaner");
        }
    }
}
