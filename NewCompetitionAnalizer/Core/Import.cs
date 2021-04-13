using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Tesztverszeny
{
    public struct Competitor
    {
        public string Code { get; set; }
        public char[] Answers { get; set; }
        public int Points { get; set; }
        
    }

    public class Importer
    {
        public List<Competitor> Competitors { get; } = new List<Competitor>();
        public char[] Correct { get; private set; }
        public int NumCompetitors { get; private set; }

        /* Method to import the file with the answers, and write the file with the points
         * string location : the location of the file that stores the answers
         */
        public void Import(string location)
        {
            
            var reader = new StreamReader(location);
            
            // setup a folder to store appdata
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var specificFolderData = Path.Combine(folder, "VersenyInfo");
            specificFolderData += "/data/";
            Directory.CreateDirectory(specificFolderData);

            var uniquePointName = Path.GetFileName(location);
            
            var writer = new StreamWriter(specificFolderData + "pontok-" + uniquePointName);
            
            NumCompetitors = 0;
            
            // The first line is the correct answer
            Correct = reader.ReadLine()?.ToCharArray();
            
            while (!reader.EndOfStream)
            {
                var competitor = new Competitor();
                
                // Separate the competitor's code with their answers  
                var temp = reader.ReadLine()?.Split();
                if (temp == null) continue;
            
                // Set the data
                competitor.Code = temp[0];
                competitor.Answers = temp[1].ToCharArray();
                competitor.Points = CalculatePoints(competitor.Answers);
            
                // Save data to a list
                Competitors.Add(competitor);
                
                // Write the points to the file 
                writer.WriteLine($"{temp[0]} {competitor.Points}");
                
                NumCompetitors++;
            }
            writer.Close();
        }


        /* Function to get the answers of a competitor and its correction
         * string code : the code of the competitor
         * return : An array of strings. If the string is two chars long, the second one is the correct answer
         */
        public string[] GetCompetitor(string code)
        {
            var result = new string[14];
            
            /* Some linq black magic
             * Gets the first competitor where competitor.Code == code
             */
            var competitor = Competitors.ToArray().First(x => x.Code == code);

            var answers = competitor.Answers;

            for (var i = 0; i < 14; i++)
            {
                result[i] = answers[i] == Correct[i] ? $"{answers[i]}" : $"{answers[i]}{Correct[i]}";
            }
            
            return result;
        }

        
        /* Function to get the number and percentage of correct solutions of a given task 
         * int n : number of the task
         * return : a string array which first element is the number and the second is the percentage,
         *  the third is the answer itself.
         */
        public string[] GetTask(int n)
        {
            n -= 1;
            // Count number of correct solutions of a task 
            var numCorrect = Competitors.Count(competitor => competitor.Answers[n] == Correct[n]);
            
            // Return number of correct solution and the percentage 
            return new[] {Convert.ToString(numCorrect), 
                Convert.ToString(Convert.ToDouble(numCorrect) / NumCompetitors * 100.0, CultureInfo.CurrentCulture), Correct[n].ToString()};
        }

        
        /* Function to get the podiums
         * return : The competitors who had the best 3 points
         */
        public IEnumerable<Competitor> GetPodium()
        {
            var first  = new List<Competitor>();
            var second = new List<Competitor>();
            var third  = new List<Competitor>();

            first.Add(Competitors[0]);

            foreach (var competitor in Competitors)
            {
                // Sort the competitors into a top 3 in probably the worst way
                if (first[0].Points < competitor.Points)
                {
                    third = second;
                    second = first;
                    first = new List<Competitor> {competitor};
                }
                else if (first[0].Points == competitor.Points) first.Add(competitor);
                else if (second[0].Points < competitor.Points)
                {
                    third = second;
                    second = new List<Competitor> {competitor};
                }
                else if (second[0].Points == competitor.Points) second.Add(competitor);
                else if (third[0].Points < competitor.Points) third = new List<Competitor> {competitor};
                else if (third[0].Points < competitor.Points) third.Add(competitor);
            }
            
            // After the inefficient loop, a super efficient way to append the lists
            var result = new List<Competitor>(first.Count +
                                              second.Count +
                                              third.Count);
            result.AddRange(first);
            result.AddRange(second);
            result.AddRange(third);
            
            return result.ToArray();
        }  
        
        
        /* Function to calculate the points
         * char[] answers : the array that stores the answers of the competitor
         * return : the points
         */
        private int CalculatePoints(IReadOnlyList<char> answers)
        {
            var points = 0;
            
            for (var i = 0; i < 14; i++)
            {
                // If incorrect or skipped, then go to the next task
                if (answers[i] != Correct[i]) continue;

                // Increase the point counter with the correct amount based on the task number
                points += i switch
                {
                    < 5 => 3,
                    < 10 => 4,
                    < 13 => 5,
                    _ => 6
                };
            }

            return points;
        }
    }
}