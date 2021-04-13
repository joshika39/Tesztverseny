using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Tesztverszeny;
using VersenyInfo.MVVM.ViewModel;

namespace VersenyInfo.MVVM.View
{
    public partial class HomeView : UserControl
    {
        private List<Competitor> _items = new List<Competitor>();
        private List<AnswerCheck> _answerCheckLeft = new List<AnswerCheck>();

        private List<AnswerCheck> _answerCheckRight = new List<AnswerCheck>();

        
        private readonly bool _hasCompetitors;
        private Importer _importer;

        public HomeView()
        {
            
            InitializeComponent();
            
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "VersenyInfo", "Versenyek");

            _importer = new Importer();
            _importer.Import(specificFolder + "/valaszok.txt");
            
            if (_importer.Competitors.Count != 0)
            {
                _hasCompetitors = true;
                FirstConfiguration();
            }
        }
        
        private void FirstConfiguration()
        {
            
            if (_hasCompetitors)
            {
                
                
                SearchBox.KeyDown += StartSearch;
                // SearchBox.Text = "Mamma mia";
                CompetitorNumber.Text = $"Összes versenyzők száma: {_importer.NumCompetitors}";
                string[] medals =
                {
                    @"D:\学校\Jetbrains\C#\VersenyInfo\VersenyInfo\Images\first.png",
                    @"D:\学校\Jetbrains\C#\VersenyInfo\VersenyInfo\Images\second.png",
                    @"D:\学校\Jetbrains\C#\VersenyInfo\VersenyInfo\Images\third.png"
                };
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

                    switch (podium)
                    {
                        case 1:
                            winnerList.Add(new Competitor()
                            {
                                Code = winner.Code, Point = winner.Points.ToString(),
                                ImageSource = medals[0]
                            });
                            break;
                        case 2:
                            winnerList.Add(new Competitor()
                            {
                                Code = winner.Code, Point = winner.Points.ToString(),
                                ImageSource = medals[1]
                            });
                            break;
                        case 3:
                            winnerList.Add(new Competitor()
                            {
                                Code = winner.Code, Point = winner.Points.ToString(),
                                ImageSource = medals[2]
                            });
                            break;
                    }
                }

                WinnerList.ItemsSource = winnerList;

                PrintAnswered("AB123");
                foreach (var competitor in _importer.Competitors)
                {
                    _items.Add(new Competitor() {Code = competitor.Code, Point = competitor.Points.ToString()});
                }
                SearchResultList.ItemsSource = _items;
            }
            else
            {
                var winnerList = new List<Competitor>();
                winnerList.Add(new Competitor()
                    {Code = "N/A", Point = "N/A", ImageSource = "N/A"});

                WinnerList.ItemsSource = winnerList;
                PrintAnswered("AB123");
                PutListCompetitor("N/A", "N/A");
            }

        }

       

        private void PutListCompetitor(string code, string points)
        {
            _items.Add(new Competitor() {Code = code, Point = points});
            SearchResultList.ItemsSource = _items;
        }

        /* Method: Binds the data to the Left and Right answer grid
         * The 'N/A' part is called if the importer's competitor count is equals to 0
         */
        private void PrintAnswered(string code)
        {
            _answerCheckLeft = new List<AnswerCheck>();
            _answerCheckRight = new List<AnswerCheck>();
            if (_hasCompetitors)
            {
                var info = _importer.GetCompetitor(code);
                // SearchBox.Text = code;

                for (int i = 0; i < 14; i++)
                {
                    if (i < 7)
                    {
                        if (info[i].Length == 1)
                        {
                            _answerCheckLeft.Add(new AnswerCheck()
                            {
                                FirstLetter = info[i][0].ToString(), Counter = i + 1 + ".",
                                FGColor = "#272537",
                                BGColor = "Aquamarine"
                            });
                        }
                        else if (info[i][0] == 'X')
                        {
                            _answerCheckLeft.Add(new AnswerCheck()
                            {
                                FirstLetter = info[i][0].ToString(), SecondLetter = info[i][1].ToString(),
                                Counter = i + 1 + ".",
                                FGColor = "Azure",
                                BGColor = "#00FFFFFF"
                            });
                        }
                        else
                        {
                            _answerCheckLeft.Add(new AnswerCheck()
                            {
                                FirstLetter = info[i][0].ToString(), SecondLetter = info[i][1].ToString(),
                                Counter = i + 1 + ".",
                                FGColor = "Azure",
                                BGColor = "Firebrick"
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
                                FGColor = "#272537",
                                BGColor = "Aquamarine"
                            });
                        }
                        else if (info[i][0] == 'X')
                        {
                            _answerCheckRight.Add(new AnswerCheck()
                            {
                                FirstLetter = info[i][0].ToString(), SecondLetter = info[i][1].ToString(),
                                Counter = i + 1 + ".",
                                FGColor = "Azure",
                                BGColor = "#00FFFFFF"
                            });
                        }
                        else
                        {
                            _answerCheckRight.Add(new AnswerCheck()
                            {
                                FirstLetter = info[i][0].ToString(), SecondLetter = info[i][1].ToString(),
                                Counter = i + 1 + ".",
                                FGColor = "Azure",
                                BGColor = "Firebrick"
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


        /* ListItem Selection EventHandler:
         * The code(Id) of selected item will be passed to the PrintAnswered(string code) method
         */
        void PrintText(object sender, SelectionChangedEventArgs args)
        {
            

            Competitor competitor = ((sender as ListBox)?.SelectedItem as Competitor);
            // if (competitor != null) SearchBox.Text = competitor.Code;


            if (competitor != null)
            {
                PrintAnswered(competitor.Code);
            }
        }

        //Classes for the Data Binding
        private class Competitor
        {
            public string Code { get; set; }
            public string Point { get; set; }
            public string ImageSource { get; set; }
        }

        private class AnswerCheck
        {
            public string Counter { get; set; }
            public string FirstLetter { get; set; }
            public string SecondLetter { get; set; }
            public string BGColor { get; set; }
            public string FGColor { get; set; }
        }

        public void Search()
        {
            int searchResult = 0;
            SearchResultList.ItemsSource = null;
            _items = new List<Competitor>();
            foreach (var competitor in _importer.Competitors)
            {
                if (competitor.Code.Contains(SearchBox.Text))
                {
                    searchResult++;
                    _items.Add(new Competitor() {Code = competitor.Code, Point = competitor.Points.ToString()});
                }
            }
            if(_items.Count != 0)
                SearchResultList.ItemsSource = _items;
            CompetitorNumberFromSearch.Text = $"Találatok száma: {searchResult}";
        }
        
        private void StartSearch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Search();
            }
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            Search();
        }
    }
}