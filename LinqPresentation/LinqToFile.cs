using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LinqPresentation
{
    public partial class MainWindow
    {
        #region struct

        public struct Movie
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public int Year { get; set; }
            public int Lenght { get; set; }

            public Movie(int iD, string name, int year, int lenght)
            {
                ID = iD;
                Name = name ?? throw new ArgumentNullException(nameof(name));
                Year = year;
                Lenght = lenght;
            }
        }

        public struct People
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }

            public People(int iD, string name, string surname)
            {
                ID = iD;
                Name = name ?? throw new ArgumentNullException(nameof(name));
                Surname = surname ?? throw new ArgumentNullException(nameof(surname));
            }
        }

        public struct Director
        {
            public int FilmId { get; set; }
            public int PeopleId { get; set; }

            public Director(int filmId, int peopleId)
            {
                FilmId = filmId;
                PeopleId = peopleId;
            }
        }

        public struct Role
        {
            public int filmId { get; set; }
            public int peopleId { get; set; }
            public string roleName { get; set; }

            public Role(int filmId, int peopleId, string roleName)
            {
                this.filmId = filmId;
                this.peopleId = peopleId;
                this.roleName = roleName ?? throw new ArgumentNullException(nameof(roleName));
            }
        }

        public struct Query
        {
            public string Name { get; set; }
        }

        #endregion

        public IEnumerable<Movie> Movies;
        public IEnumerable<Director> Directors;
        public IEnumerable<Role> Roles;
        public string[] files = {"Movies", "Directors", "Peoples", "Roles"};

        private void DataShow(DataGrid targetDataGrid, ComboBox sourceCheckBox)
        {
            switch (sourceCheckBox.SelectedValue)
            {
                case "Movies":
                    targetDataGrid.ItemsSource = Movies;
                    break;
                case "Directors":
                    targetDataGrid.ItemsSource = Directors;
                    break;
                case "Peoples":
                    targetDataGrid.ItemsSource = Peoples;
                    break;
                case "Roles":
                    targetDataGrid.ItemsSource = Roles;
                    break;
                default:
                    targetDataGrid.ItemsSource = null;
                    break;
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataShow(DataShowMemory, DataToShow);
        }

        private void LoadToMemory_Click(object sender, RoutedEventArgs e)
        {
            Movies = Properties.Resources.movie.Split('\n')
                .Where(csvLine => !string.IsNullOrWhiteSpace(csvLine))
                .Select(csvLine => new Movie(Convert.ToInt32(csvLine.Split(';')[0]), csvLine.Split(';')[1],
                    Convert.ToInt32(csvLine.Split(';')[2]), Convert.ToInt32(csvLine.Split(';')[3])));
            Directors = Properties.Resources.director.Split('\n')
                .Where(csvLine => !string.IsNullOrWhiteSpace(csvLine))
                .Select(csvLine =>
                    new Director(Convert.ToInt32(csvLine.Split(';')[0]), Convert.ToInt32(csvLine.Split(';')[1])));

            Peoples = Properties.Resources.people.Split('\n')
                .Where(csvLine => !string.IsNullOrWhiteSpace(csvLine))
                .Select(csvLine => new People(Convert.ToInt32(csvLine.Split(';')[0]), csvLine.Split(';')[1],
                    csvLine.Split(';')[2]));
            Roles = Properties.Resources.role.Split('\n')
                .Where(csvLine => !string.IsNullOrWhiteSpace(csvLine))
                .Select(csvLine => new Role(Convert.ToInt32(csvLine.Split(';')[0]),
                    Convert.ToInt32(csvLine.Split(';')[1]), csvLine.Split(';')[2]));
        }
    }
}