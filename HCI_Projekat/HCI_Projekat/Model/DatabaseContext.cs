using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Projekat.Model
{
    public enum UlogaKorisnika { KLIJENT, ADMIN, ORGANIZATOR}
    public enum TipSaradnika { RESTORAN, FOTOGRAF, KETERING, DEKORACIJE, MUZIKA }
    public enum StatusManifestacije { NOVA, U_IZRADI, ZAVRSENA}
    public enum TemaManifestacije { VENCANJE, RODJENDAN, KOKTEL_PARTY, REJV, OTVARANJE, SVE}

    public abstract class Korisnik
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Ime { get; set; }
        [Required]
        public string Prezime { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Telefon { get; set; }
        [Required]
        public string Adresa { get; set; }
        [Required]
        public UlogaKorisnika Uloga { get; set; }
        [Required]
        public bool Obrisan { get; set; }

        public Korisnik() { }
        public Korisnik(string user, string pass, string ime, string prezime, string email, string telefon, string adresa, UlogaKorisnika uloga)
        {
            Username = user;
            Password = pass;
            Ime = ime;
            Prezime = prezime;
            Email = email;
            Telefon = telefon;
            Adresa = adresa;
            Uloga = uloga;
            Obrisan = false;
        }
    }

    public class Klijent : Korisnik
    {
        public virtual List<Notifikacija> Notifikacije { get; set; }
        public virtual List<Komentar> Komentari { get; set; }
        public virtual List<Manifestacija> Manifestacije { get; set; }

        public Klijent() : base()
        {
            Notifikacije = new List<Notifikacija>();
            Komentari = new List<Komentar>();
            Manifestacije = new List<Manifestacija>();
        }
        public Klijent(string user, string pass, string ime, string prezime, string email, string telefon, string adresa) : base(user, pass, ime, prezime, email, telefon, adresa, UlogaKorisnika.KLIJENT)
        {   
            Notifikacije = new List<Notifikacija>();
            Komentari = new List<Komentar>();
            Manifestacije = new List<Manifestacija>();
        }

        public void AddKomentar(Komentar k) 
        {
            Komentari.Add(k);
            k.Klijent = this;
            k.User = this.Ime + " " + this.Prezime;
        }

        public void RemoveKomentar(Komentar k)
        {
            Komentari.Remove(k);
            k.Klijent = null;
            k.User = "";
        }

        public void AddManifestacija(Manifestacija m)
        {
            Manifestacije.Add(m);
            m.Klijent = this;
        }

        public void RemoveManifestacija(Manifestacija m)
        {
            Manifestacije.Remove(m);
            m.Klijent = null;
        }

        public void AddNotifikacija(Notifikacija n)
        {
            Notifikacije.Add(n);
            n.Klijent = this;
        }

        public void RemoveNotifikacija(Notifikacija n)
        {
            Notifikacije.Remove(n);
            n.Klijent = null;
        }
    }

    public class Organizator : Korisnik
    {
        public List<Manifestacija> Manifestacije { get; set; }
        public virtual List<Komentar> Komentari { get; set; }

        public Organizator() : base()
        {
            Manifestacije = new List<Manifestacija>();
        }
        public Organizator(string user, string pass, string ime, string prezime, string email, string telefon, string adresa) : base(user, pass, ime, prezime, email, telefon, adresa, UlogaKorisnika.ORGANIZATOR)
        {
            Manifestacije = new List<Manifestacija>();
        }

        public void AddManifestacija(Manifestacija m)
        {
            Manifestacije.Add(m);
            m.Organizator = this;
        }

        public void RemoveManifestacija(Manifestacija m)
        {
            Manifestacije.Remove(m);
            m.Organizator = null;
        }

        public void AddKomentar(Komentar k)
        {
            Komentari.Add(k);
            k.Klijent = this;
            k.User = this.Ime + " " + this.Prezime;
        }

        public void RemoveKomentar(Komentar k)
        {
            Komentari.Remove(k);
            k.Klijent = null;
            k.User = "";
        }
    }

    public class Admin : Korisnik
    {
        public Admin() : base() { }
        public Admin(string user, string pass, string ime, string prezime, string email, string telefon, string adresa) : base(user, pass, ime, prezime, email, telefon, adresa, UlogaKorisnika.ADMIN) { }
    }

    public class Manifestacija 
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public TemaManifestacije Tema { get; set; }
        [Required]
        public double Budzet { get; set; }
        [Required]
        public bool FiksanBudzet { get; set; }
        [Required]
        public int BrojGostiju { get; set; }
        [Required]
        public string MestoOdrzavanja { get; set; }
        [Required]
        public string Dekoracija { get; set; }
        [Required]
        public string Muzika { get; set; }
        [Required]
        public string DodatniZahtevi { get; set; }
        [Required]
        public StatusManifestacije Status { get; set; }
        [Required]
        public DateTime DatumOdrzavanja { get; set; }
        [Required]
        public bool Obrisana { get; set; }
        [Required]
        public virtual Klijent Klijent { get; set; }


        [Required]
        public bool MestoOdrzavanjaDone { get; set; }
        [Required]
        public bool BudzetDone { get; set; }
        [Required]
        public bool GostiDone { get; set; }
        [Required]
        public bool TemaDone { get; set; }
        [Required]
        public bool DekoracijaDone { get; set; }
        [Required]
        public bool MuzikaDone { get; set; }
        [Required]
        public bool DodatnoDone { get; set; }
        [Required]
        public bool DatumDone { get; set; }
        [Required]
        public bool RasporedDone { get; set; }

        [Required]
        public bool PredlozenoZaZavrsavanje { get; set; }

        public virtual Organizator Organizator { get; set; }
        public virtual List<Ponuda> Ponude { get; set; }
        public virtual List<Gost> Gosti { get; set; }
        public virtual List<Komentar> Komentari { get; set; }

        public Manifestacija() 
        {
            Ponude = new List<Ponuda>();
            Gosti = new List<Gost>();
            Komentari = new List<Komentar>();
        }
        public Manifestacija(TemaManifestacije tema, double budzet, bool fiks, int brGost, string restKetering, string deko, string muzika, string dodatno, DateTime datum, Organizator organizator, Klijent klijent)
        {
            Ponude = new List<Ponuda>();
            Gosti = new List<Gost>();
            Komentari = new List<Komentar>();
            Tema = tema;
            Budzet = budzet;
            FiksanBudzet = fiks;
            BrojGostiju = brGost;
            MestoOdrzavanja = restKetering;
            Dekoracija = deko;
            Muzika = muzika;
            DodatniZahtevi = dodatno;
            DatumOdrzavanja = datum;
            Organizator = organizator;
            Klijent = klijent;
            Obrisana = false;
            PredlozenoZaZavrsavanje = false;
            if(organizator == null)
            {
                Status = StatusManifestacije.NOVA;
            }
            else
            {
                Status = StatusManifestacije.U_IZRADI;
            }
        }

        public void AddGost(Gost g)
        {
            Gosti.Add(g);
            g.Manifestacija = this;
        }

        public void RemoveGost(Gost g)
        {
            Gosti.Remove(g);
            g.Manifestacija = null;
        }

        public void AddKomentar(Komentar k)
        {
            Komentari.Add(k);
            k.Manifestacija = this;
        }

        public void RemoveKomentar(Komentar k)
        {
            Komentari.Remove(k);
            k.Manifestacija = null;
        }

        public void AddPonuda(Ponuda s)
        {
            Ponude.Add(s);
            s.Manifestacije.Add(this);
        }

        public void RemovePonuda(Ponuda s)
        {
            Ponude.Remove(s);
            s.Manifestacije.Remove(this);
        }
    }

    public class Saradnik
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Naziv { get; set; }
        [Required]
        public string Adresa { get; set; }
        [Required]
        public TipSaradnika Tip { get; set; }
        [Required]
        public string Specijalizacija { get; set; }
        [Required]
        public bool Obrisan { get; set; }
        public string MapaObjekta { get; set; }
        public virtual List<Ponuda> Ponude { get; set; }
        

        public Saradnik() 
        {
            Ponude = new List<Ponuda>();
        }
        public Saradnik(string naziv, string adresa, TipSaradnika ts, string spec, string mapa)
        {
            Naziv = naziv;
            Adresa = adresa;
            Tip = ts;
            Specijalizacija = spec;
            MapaObjekta = mapa;
            Obrisan = false;
            Ponude = new List<Ponuda>();
        }

        public void AddPonuda(Ponuda p)
        {
            Ponude.Add(p);
            p.Saradnik = this;
        }

        public void RemovePonuda(Ponuda p)
        {
            Ponude.Remove(p);
            p.Saradnik = null;
        }
    }

    public class Ponuda
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Opis { get; set; }
        [Required]
        public double Cena { get; set; }
        public virtual Saradnik Saradnik { get; set; }
        public string NazivSaradnika { get; set; }
        public virtual List<Manifestacija> Manifestacije { get; set; }
        public virtual List<Sto> Stolovi { get; set; }

        public bool Obrisana { get; set; }

        public Ponuda()
        {
            Manifestacije = new List<Manifestacija>();
            Stolovi = new List<Sto>();
        }
        public Ponuda(string opis, double cena, Saradnik s)
        {
            Manifestacije = new List<Manifestacija>();
            Stolovi = new List<Sto>();
            Opis = opis;
            Cena = cena;
            Saradnik = s;
            NazivSaradnika = s.Naziv;
            Obrisana = false;
        }
    }

    public class Gost
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ImePrezime { get; set; }
        public double BrojStola { get; set; }
        public virtual Manifestacija Manifestacija { get; set; }

        public Gost() { }
        public Gost(string imePrezime, double sto, Manifestacija m)
        {
            ImePrezime = imePrezime;
            BrojStola = sto;
            Manifestacija = m;
        }

    }

    public class Notifikacija
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Posiljaoc { get; set; }
        [Required]
        public string Tekst { get; set; }
        [Required]
        public bool Dismissed { get; set; }
        public virtual Klijent Klijent { get; set; }

        public Notifikacija() { }
        public Notifikacija(string posiljaoc, string text, Klijent k)
        {
            Posiljaoc = posiljaoc;
            Tekst = text;
            Dismissed = false;
            Klijent = k;
        }
    }

    public class Komentar
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Tekst { get; set; }
        [Required]
        public bool Obrisan { get; set; }
        public virtual Manifestacija Manifestacija { get; set; }
        public virtual Korisnik Klijent { get; set; }
        public string User { get; set; }
        public DateTime DatumPostavljanja { get; set; }

        public Komentar() { }
        public Komentar(string text, Manifestacija m, Klijent k, DateTime datum)
        {
            Tekst = text;
            Manifestacija = m;
            Klijent = k;
            Obrisan = false;
            DatumPostavljanja = datum;
            User = "";
        }
    }

    public class Sto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int BrojOsoba { get; set; }
        [Required]
        public int BrojStola { get; set; }

        public Sto() { }
        public Sto(int brojOsoba, int brojStola)
        {
            BrojOsoba = brojOsoba;
            BrojStola = brojStola;
        }
    }

    public class DatabaseContext : DbContext
    {
        public DbSet<Korisnik> Korisnici { get; set; }
        public DbSet<Manifestacija> Manifestacije { get; set; }
        public DbSet<Saradnik> Saradnici { get; set; }
        public DbSet<Ponuda> Ponude { get; set; }
        public DbSet<Gost> Gosti { get; set; }
        public DbSet<Notifikacija> Notifikacije { get; set; }
        public DbSet<Komentar> Komentari { get; set; }
        public DbSet<Sto> Stolovi { get; set; }
    }
}
