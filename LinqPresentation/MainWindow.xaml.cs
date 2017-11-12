using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace LinqPresentation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public string[] Queries =
        {
            "Films longer than 2h",
            "Count films by director",
            "1990's characters",
            "All films",
            "All Peoples",
            "All Directors",
            "Director-Actor",
            "UPDATE all films length +1",
            "UPDATE all films length -1"
        };
        public IEnumerable<People> Peoples;

        public LinqDB DataBase;
        public LinqPresentationDataContext LinqPresentationDataContext;

        //public IEnumerable<string[]> DataSource = 
        public MainWindow()
        {
            InitializeComponent();
            DataToShow.ItemsSource = files;
            QueryToShow.ItemsSource = Queries;
        }

        private void ConnectDB_Click(object sender, RoutedEventArgs e)
        {
            LinqPresentationDataContext = new LinqPresentationDataContext("Data Source=linqsql.database.windows.net;Initial Catalog=LinqDB;Integrated Security=False;User ID=FrelVick;Password=Pogosty$1010;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            //DataBase = new LinqDB("Data Source=linqsql.database.windows.net;Initial Catalog=LinqDB;Integrated Security=False;User ID=FrelVick;Password=Pogosty$1010;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

            


            DataShowDB.ItemsSource = Directors;


        }

        private void QueryToShow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (QueryToShow.SelectedItem)
            {
                case "Films longer than 2h":
                    DataShowDB.ItemsSource = LinqPresentationDataContext.Movies.Where(movie => movie.Length >= 120);//DataBase.Movies.Where(movie => movie.Length >= 120);
                    break;
                case "Count films by director":
                    DataShowDB.ItemsSource = LinqPresentationDataContext.Directors.GroupBy(direct => direct.People)
                        .Select(grouping => new
                        {
                            grouping.Key.Name,
                            grouping.Key.Surname,
                            FilmCount = grouping.Count()
                        }).OrderBy(d => d.FilmCount);
                    break;
                case "All films":
                    DataShowDB.ItemsSource = LinqPresentationDataContext.Movies;
                    break;
                case "1990's characters":
                    DataShowDB.ItemsSource = LinqPresentationDataContext.Roles
                        .Join(LinqPresentationDataContext.Movies, r => r.FilmId, m => m.Id, (r, m) => new {r, m})
                        .Where(t => t.m.Year == 1990)
                        .Select(t => new
                        {
                            t.m.Year,
                            Movie = t.m.Name,
                            Role = t.r.Role1
                        });
                    /* same querie
                    from r in LinqPresentationDataContext.Roles
                    join m in LinqPresentationDataContext.Movies on r.FilmId equals m.Id
                    where m.Year == 1990
                    select new
                    {
                        Year = m.Year,
                        Movie = m.Name,
                        Role = r.Role1
                    };
                    */
                    break;
                case "All Peoples":
                    DataShowDB.ItemsSource = LinqPresentationDataContext.Peoples; break;
                case "All Directors":
                    DataShowDB.ItemsSource = LinqPresentationDataContext.Directors.GroupBy(d => d.People).Select(d => new
                    {
                        d.Key.Name,
                        d.Key.Surname,
                        Film = string.Join(", ", d.Select(i => i.Movy.Name))
                    }); break;
                case "Director-Actor":
                    DataShowDB.ItemsSource = LinqPresentationDataContext.Directors
                        .Join(LinqPresentationDataContext.Roles, d => d.FilmId, r => r.FilmId, (d, r) => new {d, r})
                        .Where(@t => @t.d.PeopleId == @t.r.PeopleId)
                        .Select(@t => new
                        {
                            @t.d.People.Name,
                            @t.d.People.Surname,
                            movie = @t.d.Movy.Name,
                            Role = @t.r.Role1
                        });
                    break;
                case "UPDATE all films length +1":
                    var toUpdate =
                        from films in LinqPresentationDataContext.Movies
                        select films;
                    foreach (var film in toUpdate)
                    {
                        film.Length += 1;
                    }
                    try
                    {
                        LinqPresentationDataContext.SubmitChanges();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                        throw;
                    }
                    DataShowDB.ItemsSource = toUpdate;
                    break;
                case "UPDATE all films length -1":
                    var Update =
                        from films in LinqPresentationDataContext.Movies
                        select films;
                    foreach (var film in Update)
                    {
                        film.Length -= 1;
                    }
                    try
                    {
                        LinqPresentationDataContext.SubmitChanges();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                        throw;
                    }
                    DataShowDB.ItemsSource = Update;
                    break;
            }
        }
    }
}