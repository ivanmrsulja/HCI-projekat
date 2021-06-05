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
                if (View != null)
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
                Ponude = new ObservableCollection<Ponuda>((from pon in db.Ponude where pon.Saradnik.Obrisan == false && pon.Obrisana == false select pon));
                View = CollectionViewSource.GetDefaultView(Ponude);
                GroupView = true;
            }
        }

        public void Pretraga_Click(object sender, EventArgs e)
        {
            using (var db = new DatabaseContext())
            {
                if(search.Text.Trim() == "")
                {
                    search.Text = "";
                    Ponude = new ObservableCollection<Ponuda>((from pon in db.Ponude where pon.Saradnik.Obrisan == false select pon));
                }
                else
                {
                    Ponude = new ObservableCollection<Ponuda>((from pon in db.Ponude where pon.Saradnik.Obrisan == false && pon.Saradnik.Naziv.Contains(search.Text.Trim()) select pon));
                }
                View = CollectionViewSource.GetDefaultView(Ponude);
                ponude.ItemsSource = View;
                GroupView = true;
            }
        }

        public void DodajPonudu_Click(object sender, EventArgs e)
        {
            Ponuda selected = (Ponuda)ponude.SelectedItem;
            using (var db = new DatabaseContext())
            {
                Ponuda toAdd = (from pon in db.Ponude where pon.Id == selected.Id select pon).FirstOrDefault();
                Manifestacija toUpdate = (from man in db.Manifestacije where man.Id == Manifestacija.Id select man).FirstOrDefault();
                foreach (Ponuda p in toUpdate.Ponude)
                {
                    Console.WriteLine(p.Stolovi.Count);
                    if(p.Stolovi.Count > 0 && toAdd.Stolovi.Count > 0 && toAdd.Saradnik.Id != p.Saradnik.Id)
                    {
                        var w1 = new OkForm("Prostor za održavanje\nje već odabran.\nMožete uzimati samo\nnjihove ponude.", "Prostor već odabran", true);
                        w1.ShowDialog();
                        return;
                    }
                    if(p.Id == toAdd.Id)
                    {
                        var w2 = new OkForm("Ponuda je već dodata\nu manifestaciju.", "Ponuda već dodata", true);
                        w2.ShowDialog();
                        return;
                    }
                    foreach(Ponuda ukljucena in toUpdate.Ponude)
                    {
                        foreach(Sto uks in ukljucena.Stolovi)
                        {
                            foreach(Sto st in toAdd.Stolovi)
                            {
                                if(uks.BrojStola == st.BrojStola)
                                {
                                    var w2 = new OkForm("Već uključene ponude sadrže (pojedine) iste stolove kao i ova.", "Stolovi dva puta rezervisani", true);
                                    w2.ShowDialog();
                                    return;
                                }
                            }
                        }
                    }
                }
                toUpdate.AddPonuda(toAdd);
                toUpdate.PredlozenoZaZavrsavanje = false;
                Manifestacija.PredlozenoZaZavrsavanje = false;
                db.SaveChanges();
                ParentData.ItemsSource = new ObservableCollection<Ponuda>((from man in db.Manifestacije where man.Id == Manifestacija.Id select man.Ponude).FirstOrDefault().ToList());
            }
            var wk = new OkForm("Ponuda je dodata\nu manifestaciju.", "Ponuda dodata", true);
            wk.ShowDialog();
        }

    }
}
