using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
namespace System.Runtime.CompilerServices
{
    sealed class CallerMemberNameAttribute : Attribute { }
}
namespace Agent.Data
{
    public class VM : INotifyPropertyChanged
    {
        protected bool Set<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
