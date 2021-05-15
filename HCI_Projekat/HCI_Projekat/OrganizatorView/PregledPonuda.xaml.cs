using HCI_Projekat.Model;
using HCI_Projekat.VlalidationForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HCI_Projekat.OrganizatorView
{
    /// <summary>
    /// Interaction logic for PregledPonuda.xaml
    /// </summary>
    public partial class PregledPonuda : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private ICollectionView _View;
        public ICollectionView View
        {
            get
            {
                return _View;
            }
            set
            {
                _View = value;
                OnPropertyChanged("View");
            }
        }

        private bool _GroupView;
        public bool GroupView
        {
            get
            {
                return _GroupView;
            }
            set
            {
                if (value != _GroupView && View != null)
                {
                    View.GroupDescriptions.Clear();
                    if (value)
                    {
                        View.GroupDescriptions.Add(new PropertyGroupDescription("Saradnik"));
                    }
                    _GroupView = value;
                    OnPropertyChanged("GroupView");

                    OnPropertyChanged("View");
                }
            }
        }

        public ObservableCollection<Ponuda> Ponude { get; set; }
        public DataGrid ParentData { get; set; }
        public Manifestacija Manifestacija { get; set; }

        public PregledPonuda(Manifestacija current, DataGrid parentData)
        {
            InitializeComponent();
            DataContext = this;

            Manifestacija = current;
            ParentData = parentData;

            using (var db = new DatabaseContext())
            {
                Ponude = new ObservableCollection<Ponuda>((from pon in db.Ponude where pon.Saradnik.Obrisan == false select pon));
                View = CollectionViewSource.GetDefaultView(Ponude);
                GroupView = true;
            }
        }

        public void DodajPonudu_Click(object sender, EventArgs e)
        {
            Ponuda selected = (Ponuda)ponude.SelectedItem;
            using (var db = new DatabaseContext())
            {
                Ponuda toRemove = (from pon in db.Ponude where pon.Id == selected.Id select pon).FirstOrDefault();
                Manifestacija toUpdate = (from man in db.Manifestacije where man.Id == Manifestacija.Id select man).FirstOrDefault();
                toUpdate.AddPonuda(toRemove);
                db.SaveChanges();
                ParentData.ItemsSource = new ObservableCollection<Ponuda>((from man in db.Manifestacije where man.Id == Manifestacija.Id select man.Ponude).FirstOrDefault().ToList());
            }
            var wk = new OkForm("Ponuda je dodata\nu manifestaciju.", "Ponuda dodata");
            wk.ShowDialog();
        }

    }
}
