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

        //public IEnumerable<string[]> DataSource = 
        public MainWindow()
        {
            InitializeComponent();
            DataToShow.ItemsSource = files;
            QueryToShow.ItemsSource = Queries;
        }

        private void ConnectDB_Click(object sender, RoutedEventArgs e)
        {
            DataBase = new LinqDB("Data Source=linqsql.database.windows.net;Initial Catalog=LinqDB;Integrated Security=False;User ID=FrelVick;Password=Pogosty$1010;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

            var longFilms = DataBase.Movies.Where(movie => movie.Length >= 120);
            var Directors = DataBase.Directors.GroupBy(direct => direct.People)
                .Select(grouping => new
                {
                    grouping.Key.Name,
                    grouping.Key.Surname,
                    FilmCount = grouping.Count()
                });


            DataShowDB.ItemsSource = Directors;


        }

        private void QueryToShow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (QueryToShow.SelectedItem)
            {
                case "Films longer than 2h":
                    DataShowDB.ItemsSource = DataBase.Movies.Where(movie => movie.Length >= 120);
                    break;
                case "Count films by director":
                    DataShowDB.ItemsSource = DataBase.Directors.GroupBy(direct => direct.People)
                        .Select(grouping => new
                        {
                            grouping.Key.Name,
                            grouping.Key.Surname,
                            FilmCount = grouping.Count()
                        }).OrderBy(d => d.FilmCount);
                    break;
                case "All films":
                    DataShowDB.ItemsSource = DataBase.Movies;
                    break;
                case "1990's characters":
                    DataShowDB.ItemsSource = DataBase.Roles
                        .Join(DataBase.Movies, r => r.FilmId, m => m.Id, (r, m) => new {r, m})
                        .Where(t => t.m.Year == 1990)
                        .Select(t => new
                        {
                            t.m.Year,
                            Movie = t.m.Name,
                            Role = t.r.Role1
                        });
                    /* same querie
                    from r in DataBase.Roles
                    join m in DataBase.Movies on r.FilmId equals m.Id
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
                    DataShowDB.ItemsSource = DataBase.Peoples; break;
                case "All Directors":
                    DataShowDB.ItemsSource = DataBase.Directors.GroupBy(d => d.People).Select(d => new
                    {
                        d.Key.Name,
                        d.Key.Surname,
                        Film = string.Join(", ", d.Select(i => i.Movie.Name))
                    }); break;
                case "Director-Actor":
                    DataShowDB.ItemsSource = DataBase.Directors
                        .Join(DataBase.Roles, d => d.FilmId, r => r.FilmId, (d, r) => new {d, r})
                        .Where(@t => @t.d.PeopleId == @t.r.PeopleId)
                        .Select(@t => new
                        {
                            @t.d.People.Name,
                            @t.d.People.Surname,
                            movie = @t.d.Movie.Name,
                            Role = @t.r.Role1
                        });
                    break;
                case "UPDATE all films length +1":
                    var toUpdate =
                        from films in DataBase.Movies
                        select films;
                    foreach (var film in toUpdate)
                    {
                        film.Length += 1;
                    }
                    try
                    {
                        DataBase.SubmitChanges();
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
                        from films in DataBase.Movies
                        select films;
                    foreach (var film in Update)
                    {
                        film.Length -= 1;
                    }
                    try
                    {
                        DataBase.SubmitChanges();
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