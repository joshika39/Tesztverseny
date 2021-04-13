using System;
using Tesztverszeny;
using VersenyInfo.Core;

namespace VersenyInfo.MVVM.ViewModel
{
    public class MainViewModel : ObservableObject
    {

        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand TaskViewCommand { get; set; }
        
        public HomeViewModel HomeVm { get; set; }
        public TaskViewModel TaskVm { get; set; }
        private object _currentView;
        

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value; 
                OnPropertyChanged();
            }
        }
        
        public MainViewModel()
        {
            HomeVm = new HomeViewModel();
            // SettingsVm = new SettingsViewModel();
            TaskVm = new TaskViewModel();
            CurrentView = HomeVm;

            HomeViewCommand = new RelayCommand(o=>
            {
                CurrentView = HomeVm;
            });
            TaskViewCommand = new RelayCommand(o=>
            {
                CurrentView = TaskVm;
            });
        }
    }
}