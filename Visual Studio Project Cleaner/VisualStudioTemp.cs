using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Visual_Studio_Project_Cleaner
{
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
            set
            {
                _ExtensionFormat = value;
                NotifyPropertyChanged();
            }
        }

        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
                NotifyPropertyChanged();
            }
        }
        public bool Enabled
        {
            get { return _Enabled; }
            set
            {
                _Enabled = value;
                NotifyPropertyChanged();
            }
        }

        public Regex Pattern
        {
            get { return _Pattern; }
            set
            {
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
}
