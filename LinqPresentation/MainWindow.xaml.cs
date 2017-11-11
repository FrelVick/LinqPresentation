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
            "All Directors"
        };
        public IEnumerable<People> Peoples;

        public LinqDB DataBase;

        //public IEnumerable<string[]> DataSource = 
        public MainWindow()
        {
            InitializeComponent();
            DataToShow.ItemsSource = files;
            DataToShowXML.ItemsSource = files;
            QueryToShow.ItemsSource = Queries;
        }

        private void DataToShowXML_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void LoadFromXML_Click(object sender, RoutedEventArgs e)
        {

            var Directors = XDocument
                .Load(@"C:\\Users\\FrelV\\source\\repos\\LinqPresentation\\LinqPresentation\\Data\\Directors.xml").Elements().Select(node => new Director((int)node.Element("FilmId"),(int)node.Element("PeopleId")));
            XMLToShow.ItemsSource = Directors;
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
            }
        }
    }
}