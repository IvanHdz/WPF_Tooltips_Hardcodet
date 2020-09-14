using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace ToolTips
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public string Header
        {
            get { return "Hello " + Environment.UserName; }
        }

        public string Content
        {
            get { return "The contents of this ToolTip were retrieved from the view model.\nCurrent time: " + DateTime.Now.ToLongTimeString(); }
        }


        public MainViewModel()
        {
            //fire a change event for the Content property every second
            var timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            timer.Tick += (s, e) => OnPropertyChanged("Content");
            timer.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
