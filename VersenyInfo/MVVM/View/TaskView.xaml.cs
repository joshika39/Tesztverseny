using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tesztverszeny;

namespace VersenyInfo.MVVM.View
{
    public partial class TaskView : UserControl
    {
        private readonly bool _hasCompetitors;
        private readonly Importer _importer;
        private int _lastNumSearch = 1;

        public TaskView()
        {
            InitializeComponent();
            
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "VersenyInfo", "Versenyek");

            _importer = new Importer();
            _importer.Import(specificFolder + "/valaszok.txt");
            if (_importer.Competitors.Count != 0)
                _hasCompetitors = true;
            
            if (_hasCompetitors)
            {
                SearchBox.TextChanged += ValidSearch;
                SearchBox.KeyDown += StartSearch;
                
                PrintTask(1);
            }
        }

        private void StartSearch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Search();
            }
        }

        private void ValidSearch(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(SearchBox.Text, out _))
                _lastNumSearch = int.Parse(SearchBox.Text);
            else if(SearchBox.Text != "")
                SearchBox.Text = _lastNumSearch.ToString();

        }
        
        private void Search(object sender, RoutedEventArgs e)
        {
            Search();
        }

        public void PrintTask(int n)
        {
            if (_hasCompetitors)
            {
                var result = _importer.GetTask(n);
                TaskAnswer.Text = $"\"{result[2][0]}\"";
                TaskRightAnswer.Text = $"{result[0]} ({Math.Round(double.Parse(result[1]), 2)}%)";

            }
            else
            {
                TaskAnswer.Text = $"N/A";
                TaskRightAnswer.Text = $"N/A (N/A)";
            }
        }

        private void Search()
        {
            PrintTask(_lastNumSearch);
        }
    }
}