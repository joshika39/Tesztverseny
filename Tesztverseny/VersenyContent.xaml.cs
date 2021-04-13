using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Tesztverszeny;

namespace Tesztverseny
{
    public partial class VersenyContent : UserControl
    {
        private List<Competitor> _items = new List<Competitor>();
        private List<AnswerCheck> _answerCheckLeft = new List<AnswerCheck>();
        private List<AnswerCheck> _answerCheckRight = new List<AnswerCheck>();
        private List<string> _searchResult = new List<string>();
        private readonly bool _hasCompetitors;
        private readonly Importer _importer;
        private int _lastNumSearch = 1;
        private string _lastIdSearch = "";

        public VersenyContent(Importer importer)
        {
            InitializeComponent();

            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "Competition Inspector");

            _importer = importer;
            if (_importer.Competitors.Count != 0)
                _hasCompetitors = true;
        
            FirstConfiguration();
        }

        private void FirstConfiguration()
        {
            CompetitorId.FontSize = 16;
            CompetitorId.FontWeight = FontWeights.Bold;
            CompetitorId.LineHeight = 16;
            // LastId.FontSize = 14;
            
            
            
            TaskNumber.FontSize = 16;
            TaskNumber.LineHeight = 16;
            TaskNumber.FontWeight = FontWeights.Bold;
            TaskNumber.TextAlignment = TextAlignment.Center;
            TaskNumber.HorizontalAlignment = HorizontalAlignment.Right;
            TaskNumber.VerticalAlignment = VerticalAlignment.Center;

            SearchBoxTask.TextAlignment = TextAlignment.Center;
            SearchBoxTask.HorizontalAlignment = HorizontalAlignment.Left;
            SearchBoxTask.VerticalAlignment = VerticalAlignment.Center;
            TaskAnswer.FontSize = 16;
            TaskInfo.FontSize = 16;
            CompetitorNumber.FontSize = 16;


            
             if (_hasCompetitors)
             {
                 RTaskCheck.IsChecked = true;
                 TaskCheck.IsChecked = false;
                 IdCheck.IsChecked = true;
                 TaskCheck.Checked += TaskCheckEvent;
                 TaskCheck.Unchecked += TaskUnCheckEvent;
                 IdCheck.Click += CheckBoxClick;
                 RTaskCheck.Click += CheckBoxClick;

                 TaskCheck.Click += CheckBoxClick;
                 IdCheck.Checked += IdCheckEvent;
                 IdCheck.Unchecked += IdUnCheckEvent;
                 
                 SearchBox.GotFocus += RemoveText;
                 SearchBox.LostFocus += AddText;
                 // SearchBox.TextChanged += NewSearch;
                 IdSearch.Click += SearchStart;
                 TaskSearch.Click += SearchStart;
                 SearchBoxTask.GotFocus += RemoveText2;
                 SearchBoxTask.LostFocus += AddText2;
                 SearchBoxTask.TextChanged += NewSearch;
                 CompetitorNumber.Text = _importer.NumCompetitors.ToString();
                 SearchBoxTask.Text = "Search";
                 SearchBox.Text = "Search";
                 var winners = _importer.GetPodium();
                 var competitors = winners as Tesztverszeny.Competitor[] ?? winners.ToArray();
                 var currentPoint = competitors[0].Points;
                 var winnerList = new List<Competitor>();

                 var podium = 1;
                 foreach (var winner in competitors)
                 {
                     if (currentPoint > winner.Points)
                     {
                         podium++;
                         currentPoint = winner.Points;
                     }

                     winnerList.Add(new Competitor()
                         {Code = winner.Code, Point = winner.Points.ToString(), Position = $"{podium}. díj: "});
                 }

                 WinnerList.ItemsSource = winnerList;

                 PrintAnswered("AB123");
                 PrintTask(1);

                 foreach (var competitor in _importer.Competitors)
                 {
                     PutListCompetitor(competitor.Code, competitor.Points.ToString());
                 }
             }
             else
             {
                 var winnerList = new List<Competitor>();
                 winnerList.Add(new Competitor()
                     {Code = "N/A", Point = "N/A", Position = "N/A"});
                 
                 WinnerList.ItemsSource = winnerList;
                 PrintAnswered("AB123");
                 PutListCompetitor("N/A", "N/A");
                 SearchBoxTask.Text = "N/A";
                 CompetitorId.Text = "ID";
                 PrintTask(1);
             }
             RTaskCheck.IsEnabled = TaskCheck.IsChecked == true;
             SearchBoxTask.IsEnabled = TaskCheck.IsChecked == true;
             SearchBox.IsEnabled = IdCheck.IsChecked == true;
        }

        private void SearchStart(object sender, RoutedEventArgs e)
        {
            _lastIdSearch = SearchBox.Text;
            SearchResultList.ItemsSource = null;
            SearchCombine();
        }

        private void CheckBoxClick(object sender, RoutedEventArgs e)
        {            
            SearchResultList.ItemsSource = null;
            SearchCombine();
        }

        private void IdUnCheckEvent(object sender, RoutedEventArgs e)
        {
            SearchBox.IsEnabled = false;
        }

        private void IdCheckEvent(object sender, RoutedEventArgs e)
        {
            SearchBox.IsEnabled = true;
        }

        private void TaskUnCheckEvent(object sender, RoutedEventArgs e)
        {
            RTaskCheck.IsEnabled = false;
            SearchBoxTask.IsEnabled = false;
        }

        private void TaskCheckEvent(object sender, RoutedEventArgs e)
        {
            RTaskCheck.IsEnabled = true;
            SearchBoxTask.IsEnabled = true;
        }
        
        public void PutListCompetitor(string code, string points)
        {
            _items.Add(new Competitor() {Code = code, Point = points});
            // CompetitorList.ItemsSource = items;
        }

        private void SearchCombine()
        {
            _items = new List<Competitor>();
            foreach (var competitor in _importer.Competitors)
            {
                if (IdCheck.IsChecked ==  true && TaskCheck.IsChecked == true)
                {
                    if (competitor.Code.Contains(_lastIdSearch))
                    {
                        var answers = _importer.GetCompetitor(competitor.Code);

                        if (!int.TryParse(SearchBoxTask.Text, out _)) 
                            return;
                        
                        if (int.Parse(SearchBoxTask.Text) > 0 && int.Parse(SearchBoxTask.Text) < 15)
                        {
                            int n = int.Parse(SearchBoxTask.Text);
                            _lastNumSearch = n;
                            var result = _importer.GetTask(n);
                            if (RTaskCheck.IsChecked == true)
                            {
                                if (answers[n - 1][0] == result[2][0])
                                {
                                    _items.Add(new Competitor() {Code = competitor.Code, Point = competitor.Points.ToString()});
                                } 
                            }
                            else
                            {
                                if (answers[n - 1][0] != result[2][0])
                                {
                                    _items.Add(new Competitor() {Code = competitor.Code, Point = competitor.Points.ToString()});
                                } 
                            }
                            PrintTask(n);
                        }
                        else
                        {
                            if (int.Parse(SearchBoxTask.Text) < 1)
                            {
                                SearchBoxTask.Text = "1";
                                SearchCombine();
                            }
                            if (int.Parse(SearchBoxTask.Text) > 14)
                            {
                                SearchBoxTask.Text = "14";
                                SearchCombine();
                            }
                        }


                    }
                }
                else if (IdCheck.IsChecked == false && TaskCheck.IsChecked == true)
                {
                    var answers = _importer.GetCompetitor(competitor.Code);

                    if (!int.TryParse(SearchBoxTask.Text, out _))
                        return;
                    
                    if (int.Parse(SearchBoxTask.Text) > 0 && int.Parse(SearchBoxTask.Text) < 15)
                    {
                        int n = int.Parse(SearchBoxTask.Text);
                        _lastNumSearch = n;
                        var result = _importer.GetTask(n);
                        if (RTaskCheck.IsChecked == true)
                        {
                            if (answers[n - 1][0] == result[2][0])
                            {
                                _items.Add(new Competitor()
                                    {Code = competitor.Code, Point = competitor.Points.ToString()});
                            }
                        }
                        else
                        {
                            if (answers[n - 1][0] != result[2][0])
                            {
                                _items.Add(new Competitor()
                                    {Code = competitor.Code, Point = competitor.Points.ToString()});
                            }
                        }

                        PrintTask(n);
                    }
                    else
                    {
                        if (SearchBoxTask.Text == "")
                        {
                            return;
                        }
                        if (int.Parse(SearchBoxTask.Text) < 1)
                        {
                            SearchBoxTask.Text = "1";
                            return;
                        }
                        if (int.Parse(SearchBoxTask.Text) > 14)
                        {
                            SearchBoxTask.Text = "14";
                            return;
                        }
                    }
                }
                else if (IdCheck.IsChecked == true && TaskCheck.IsChecked == false)
                {
                    if (competitor.Code.Contains(_lastIdSearch))
                    {
                        _items.Add(new Competitor() {Code = competitor.Code, Point = competitor.Points.ToString()});
                    }
                }
            }
            if(_items.Count != 0)
                SearchResultList.ItemsSource = _items;
        }

        private void NewSearch(object sender, TextChangedEventArgs e)
        {
            if (!int.TryParse(SearchBoxTask.Text, out _))
                SearchBoxTask.Text = _lastNumSearch.ToString();
            
            // SearchResultList.ItemsSource = null;
            // SearchCombine();
        }


        private void RemoveText(object sender, EventArgs e)
        {
            SearchBox.Text = "";
        }

        private void AddText(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchBox.Text)) return;
            
            SearchBox.Text = "Search";
                
            foreach (var competitor in _importer.Competitors)
            {
                PutListCompetitor(competitor.Code, competitor.Points.ToString());
            }
        }

        private void RemoveText2(object sender, EventArgs e)
        {
            if (SearchBoxTask.Text == "Search")
            {
                SearchBoxTask.Text = "";
            }
        }

        private void AddText2(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBoxTask.Text))
            {
                SearchBoxTask.Text = "Search";
            }
        }


        void PrintText(object sender, SelectionChangedEventArgs args)
        {
            Competitor competitor = ((sender as ListBox)?.SelectedItem as Competitor);
            if (competitor != null) SearchBox.Text = competitor.Code;

            
            if (competitor != null)
            {
                PrintAnswered(competitor.Code);
            }
        }


        private void PrintAnswered(string code)
        {
            _answerCheckLeft = new List<AnswerCheck>();
            _answerCheckRight = new List<AnswerCheck>();
            if (_hasCompetitors)
            {
                var info = _importer.GetCompetitor(code);
                SearchBox.Text = code;

                for (int i = 0; i < 14; i++)
                {
                    if (i < 7)
                    {
                        if (info[i].Length == 1)
                        {
                            _answerCheckLeft.Add(new AnswerCheck()
                            {
                                FirstLetter = info[i][0].ToString(), Counter = i + 1 + ".",
                                FGColor = "#FFF",
                                BGColor = "#08db0f"
                            });
                        }
                        else if (info[i][0] == 'X')
                        {
                            _answerCheckLeft.Add(new AnswerCheck()
                            {
                                FirstLetter = info[i][0].ToString(), SecondLetter = info[i][1].ToString(), Counter = i + 1 + ".",
                                FGColor = "#000",
                                BGColor = "#00FFFFFF"
                            });
                        }
                        else
                        {
                            _answerCheckLeft.Add(new AnswerCheck()
                            {
                                FirstLetter = info[i][0].ToString(), SecondLetter = info[i][1].ToString(), Counter = i + 1 + ".",
                                FGColor = "#FFF",
                                BGColor = "#FF1A00"
                            });
                        }
                    }
                    else
                    {
                        if (info[i].Length == 1)
                        {
                            _answerCheckRight.Add(new AnswerCheck()
                            {
                                FirstLetter = info[i][0].ToString(), Counter = i + 1 + ".",
                                FGColor = "#FFF",
                                BGColor = "#08db0f"
                            });
                        }
                        else if (info[i][0] == 'X')
                        {
                            _answerCheckRight.Add(new AnswerCheck()
                            {
                                FirstLetter = info[i][0].ToString(), SecondLetter = info[i][1].ToString(), Counter = i + 1 + ".",
                                FGColor = "#000",
                                BGColor = "#00FFFFFF"
                            });
                        }
                        else
                        {
                            _answerCheckRight.Add(new AnswerCheck()
                            {
                                FirstLetter = info[i][0].ToString(), SecondLetter = info[i][1].ToString(), Counter = i + 1 + ".",
                                FGColor = "#FFF",
                                BGColor = "#FF1A00"
                            });
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 14; i++)
                {
                    if (i < 7)
                    {
                        _answerCheckLeft.Add(new AnswerCheck()
                        {
                            FirstLetter = "N/A", Counter = i + 1 + ".",
                            FGColor = "#000",
                            BGColor = "#00FFFFFF"
                        });
                    }
                    else
                    {
                        _answerCheckRight.Add(new AnswerCheck()
                        {
                            FirstLetter = "N/A", Counter = i + 1 + ".",
                            FGColor = "#000",
                            BGColor = "#00FFFFFF"
                        });
                    }
                }
            }


            CompetitorAnswerLeft.ItemsSource = _answerCheckLeft;
            CompetitorAnswerRight.ItemsSource = _answerCheckRight;
        }
        public void PrintTask(int n)
        {
            if (_hasCompetitors)
            {
                var result = _importer.GetTask(n);
                TaskAnswer.Text = $"\"{result[2][0]}\"";
                TaskInfo.Text = $"{result[0]} ({Math.Round(double.Parse(result[1]), 2)}%)";
                TaskNumber.Text = $"Feladat:";

            }
            else
            {
                TaskAnswer.Text = $"N/A";
                TaskInfo.Text = $"N/A (N/A)";
                TaskNumber.Text = $"Feladat:";
            }
        }

        private class Competitor
        {
            public string Code { get; set; }
            public string Point { get; set; }
            public string Position { get; set; }
        }

        private class AnswerCheck
        {
            public string Counter { get; set; }
            public string FirstLetter { get; set; }
            public string SecondLetter { get; set; }
            public string BGColor { get; set; }
            public string FGColor { get; set; }
        }
    }
}