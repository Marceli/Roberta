using System;
using System.Security.Principal;
using NUnit.Extensions.Asp.AspTester;
using NUnit.Framework;
using Vulcan.Stypendia.Bll;
using System.Text;


namespace GUITests
{
	[TestFixture]
	public class LokalizacjaComponentTest : BaseComponentTest
	{		
		private ButtonTester PostBtn;
		private ButtonTester NewBtn;
		private ButtonTester EditBtn;
		private ButtonTester DelBtn;
		private ButtonTester SaveBtn;
		private ButtonTester CancelBtn;

		private TextBoxTester IdTb;
		private LabelTester sessionLbl;

        protected override string GetUrl()
        {
           
            if (WindowsIdentity.GetCurrent().Name == @"WIN\Marcel")
            {
                return "http://localhost:1218/Testy/LokalizacjaTest.aspx";
            }
            else
            {
                return "http://localhost/WebStypendia/Testy/LokalizacjaTest.aspx";
            }
        }

		private static string UnikalnyNumerTelefonu()
		{
			Random rand = new Random();
			StringBuilder str = new StringBuilder();
			str.Append("0");
			str.Append(rand.Next(10, 99).ToString());
			str.Append(rand.Next(1000000, 9999999).ToString());
			return str.ToString();
		}
		
		protected override void SetUp()
		{
			PostBtn = new ButtonTester("PostBtn", CurrentWebForm);
			NewBtn = new ButtonTester("NewBtn", CurrentWebForm);
			EditBtn = new ButtonTester("EditBtn", CurrentWebForm);
			DelBtn = new ButtonTester("DelBtn", CurrentWebForm);
			SaveBtn = new ButtonTester("SaveBtn", CurrentWebForm);
			CancelBtn = new ButtonTester("CancelBtn", CurrentWebForm);
			IdTb = new TextBoxTester("IdTb", CurrentWebForm);
			sessionLbl = new LabelTester("SessionLbl", CurrentWebForm);

            base.SetUp();
		}

		
		
		[Test]
		public override void DisplayTest()
		{
			LabelTester Nazwa = new LabelTester("ctl02_Nazwa", CurrentWebForm);
			LabelTester Skrot = new LabelTester("ctl02_Skrot", CurrentWebForm);
			LabelTester Poczta = new LabelTester("ctl02_ctl00_Poczta", CurrentWebForm);
			LabelTester Wojewodztwo = new LabelTester("ctl02_ctl00_Wojewodztwo", CurrentWebForm);
			LabelTester Gmina = new LabelTester("ctl02_ctl00_Gmina", CurrentWebForm);
			LabelTester Ulica = new LabelTester("ctl02_ctl00_Ulica", CurrentWebForm);
			LabelTester Kraj = new LabelTester("ctl02_ctl00_Kraj", CurrentWebForm);
			LabelTester Kod = new LabelTester("ctl02_ctl00_Kod", CurrentWebForm);
			LabelTester Powiat = new LabelTester("ctl02_ctl00_Powiat", CurrentWebForm);
			LabelTester Gus = new LabelTester("ctl02_ctl00_Gus", CurrentWebForm);
			LabelTester Miejscowosc = new LabelTester("ctl02_ctl00_Miejscowosc", CurrentWebForm);
			LabelTester Tel = new LabelTester("ctl02_ctl01_Tel", CurrentWebForm);
			LabelTester Tel2 = new LabelTester("ctl02_ctl01_Tel2", CurrentWebForm);
			LabelTester Fax = new LabelTester("ctl02_ctl01_Fax", CurrentWebForm);
			LabelTester Www = new LabelTester("ctl02_ctl01_Www", CurrentWebForm);
			LabelTester Email = new LabelTester("ctl02_ctl01_Email", CurrentWebForm);

			Lokalizacja l = Lokalizacja.GetWellKnown();
			string unNazwa = Guid.NewGuid().ToString();
			l.Nazwa = unNazwa;
			l.Save(null);
			int idAdres = (int)l.IdAdres;
			int idKontakt = (int)l.IdKontakt;

			IdTb.Text = l.Id.ToString();
			PostBtn.Click();
			Assert.AreEqual(l.Id, int.Parse(sessionLbl.Text), "Z³y id lokalizacji");

			Assert.AreEqual(l.Nazwa, Nazwa.Text, "z³a nazwa lokalizacji");
			Assert.AreEqual(l.Skrot, Skrot.Text, "z³y skrót lokalizacji");
			Assert.AreEqual(l.Adres.Poczta, Poczta.Text, "poczta");
			Assert.AreEqual(l.Adres.Wojewodztwo, Wojewodztwo.Text, "województwo");
			Assert.AreEqual(l.Adres.Gmina, Gmina.Text, "gmina");
			Assert.AreEqual(l.Adres.Ulica, Ulica.Text, "ulica");
			Assert.AreEqual(l.Adres.Kraj, Kraj.Text, "kraj");
			Assert.AreEqual(l.Adres.Kod, Kod.Text, "kod");
			Assert.AreEqual(l.Adres.Powiat, Powiat.Text, "powiat");
			Assert.AreEqual(l.Adres.Gus, Gus.Text, "gus");
			Assert.AreEqual(l.Adres.Miejscowosc, Miejscowosc.Text, "miejscowosc");
			Assert.AreEqual(l.Kontakt.Tel, Tel.Text, "tel");
			Assert.AreEqual(l.Kontakt.Tel2, Tel2.Text, "tel2");
			Assert.AreEqual(l.Kontakt.Fax, Fax.Text, "fax");
			Assert.AreEqual(l.Kontakt.Www, Www.Text, "www");
			Assert.AreEqual(l.Kontakt.Email, Email.Text, "email");

			
			l.Delete();
			Adres a = Dm.Retrieve<Adres>(idAdres);
			a.Delete();
			Kontakt k = Dm.Retrieve<Kontakt>(idKontakt);
			k.Delete();
		}

		[Test]
		public override void InsertTest()
		{
			TextBoxTester Nazwa = new TextBoxTester("ctl02_NazwaEdit", CurrentWebForm);
			TextBoxTester Skrot = new TextBoxTester("ctl02_SkrotEdit", CurrentWebForm);
			TextBoxTester Poczta = new TextBoxTester("ctl02_ctl00_PocztaEdit", CurrentWebForm);
			TextBoxTester Wojewodztwo = new TextBoxTester("ctl02_ctl00_WojewodztwoEdit", CurrentWebForm);
			TextBoxTester Gmina = new TextBoxTester("ctl02_ctl00_GminaEdit", CurrentWebForm);
			TextBoxTester Ulica = new TextBoxTester("ctl02_ctl00_UlicaEdit", CurrentWebForm);
			TextBoxTester Kraj = new TextBoxTester("ctl02_ctl00_KrajEdit", CurrentWebForm);
			TextBoxTester Kod = new TextBoxTester("ctl02_ctl00_KodEdit", CurrentWebForm);
			TextBoxTester Powiat = new TextBoxTester("ctl02_ctl00_PowiatEdit", CurrentWebForm);
			TextBoxTester Gus = new TextBoxTester("ctl02_ctl00_GusEdit", CurrentWebForm);
			TextBoxTester Miejscowosc = new TextBoxTester("ctl02_ctl00_MiejscowoscEdit", CurrentWebForm);
			TextBoxTester Tel = new TextBoxTester("ctl02_ctl01_TelEdit", CurrentWebForm);
			TextBoxTester Tel2 = new TextBoxTester("ctl02_ctl01_Tel2Edit", CurrentWebForm);
			TextBoxTester Fax = new TextBoxTester("ctl02_ctl01_FaxEdit", CurrentWebForm);
			TextBoxTester Www = new TextBoxTester("ctl02_ctl01_WwwEdit", CurrentWebForm);
			TextBoxTester Email = new TextBoxTester("ctl02_ctl01_EmailEdit", CurrentWebForm);

			NewBtn.Click();

			string guidNazwa = Guid.NewGuid().ToString();
			string guidUlica = Guid.NewGuid().ToString();
			string guidTelefon = UnikalnyNumerTelefonu();

			Lokalizacja l2i = Lokalizacja.GetWellKnown();
			l2i.Nazwa = guidNazwa;
			l2i.Adres.Ulica = guidUlica;
			l2i.Kontakt.Tel = guidTelefon;

			Nazwa.Text = l2i.Nazwa;
			Skrot.Text = l2i.Skrot;

			Poczta.Text = l2i.Adres.Poczta ;
			Wojewodztwo.Text = l2i.Adres.Wojewodztwo;
			Gmina.Text = l2i.Adres.Gmina;
			Ulica.Text = l2i.Adres.Ulica;
			Kraj.Text = l2i.Adres.Kraj;
			Kod.Text = l2i.Adres.Kod;
			Powiat.Text = l2i.Adres.Powiat;
			Gus.Text = l2i.Adres.Gus;
			Miejscowosc.Text = l2i.Adres.Miejscowosc;

			Tel.Text = l2i.Kontakt.Tel;
			Tel2.Text = l2i.Kontakt.Tel2;
			Fax.Text = l2i.Kontakt.Fax;
			Www.Text = l2i.Kontakt.Www;
			Email.Text = l2i.Kontakt.Email;

			SaveBtn.Click();
			Assert.IsTrue(int.Parse(sessionLbl.Text) > 0, "Niezapisano nowego rekordu");

			int idLokalizacja = 0;
			if (int.TryParse(sessionLbl.Text,out idLokalizacja))
				if (idLokalizacja > 0)
				{
					Dm.ObjectSpace.ClearTracking();
					Lokalizacja lFromDb = Dm.Retrieve<Lokalizacja>(idLokalizacja);
					int idAdres = (int)lFromDb.IdAdres;
					int idKontakt = (int)lFromDb.IdKontakt;

					Assert.AreEqual(l2i.Nazwa, lFromDb.Nazwa, "nazwa");
					Assert.AreEqual(l2i.Skrot, lFromDb.Skrot, "skrót");

					Assert.AreEqual(l2i.Kontakt.Fax, lFromDb.Kontakt.Fax, "fax");
					Assert.AreEqual(l2i.Kontakt.Tel, lFromDb.Kontakt.Tel, "telefon");
					Assert.AreEqual(l2i.Kontakt.Tel2, lFromDb.Kontakt.Tel2, "telefon 2");
					Assert.AreEqual(l2i.Kontakt.Email, lFromDb.Kontakt.Email, "e-mail");
					Assert.AreEqual(l2i.Kontakt.Www, lFromDb.Kontakt.Www, "WWW");

					Assert.AreEqual(l2i.Adres.Poczta, lFromDb.Adres.Poczta, "poczta");
					Assert.AreEqual(l2i.Adres.Wojewodztwo, lFromDb.Adres.Wojewodztwo, "woj");
					Assert.AreEqual(l2i.Adres.Gmina, lFromDb.Adres.Gmina, "gmi");
					Assert.AreEqual(l2i.Adres.Ulica, lFromDb.Adres.Ulica, "ulica");
					Assert.AreEqual(l2i.Adres.Kraj, lFromDb.Adres.Kraj, "kraj");
					Assert.AreEqual(l2i.Adres.Kod, lFromDb.Adres.Kod, "kod");
					Assert.AreEqual(l2i.Adres.Powiat, lFromDb.Adres.Powiat, "powiat");
					Assert.AreEqual(l2i.Adres.Gus, lFromDb.Adres.Gus, "gus");
					Assert.AreEqual(l2i.Adres.Miejscowosc, lFromDb.Adres.Miejscowosc, "msc");

					lFromDb.Delete();
					Adres a = Dm.Retrieve<Adres>(idAdres);
					a.Delete();
					Kontakt k = Dm.Retrieve<Kontakt>(idKontakt);
					k.Delete();
				}			
		}

		[Test]
		public override void EditTest()
		{
			LabelTester Nazwa = new LabelTester("ctl02_Nazwa", CurrentWebForm);
			LabelTester Ulica = new LabelTester("ctl02_ctl00_Ulica", CurrentWebForm);
			LabelTester Tel = new LabelTester("ctl02_ctl01_Tel", CurrentWebForm);
			TextBoxTester NazwaEdit = new TextBoxTester("ctl02_NazwaEdit", CurrentWebForm);
			TextBoxTester UlicaEdit = new TextBoxTester("ctl02_ctl00_UlicaEdit", CurrentWebForm);
			TextBoxTester TelEdit = new TextBoxTester("ctl02_ctl01_TelEdit", CurrentWebForm);

			Lokalizacja l = Lokalizacja.GetWellKnown();
			l.Save(null);
			int idLokalizacja = l.Id;
			
			IdTb.Text = l.Id.ToString();
			PostBtn.Click();
			Assert.AreEqual(l.Nazwa, Nazwa.Text, "z³a nazwa lokalizacji");
			Assert.AreEqual(l.Adres.Ulica, Ulica.Text, "ulica");
			Assert.AreEqual(l.Kontakt.Tel, Tel.Text, "tel");

			string guidNazwa = Guid.NewGuid().ToString();
			string guidUlica = Guid.NewGuid().ToString();
			string guidTelefon = UnikalnyNumerTelefonu();

			EditBtn.Click();
			NazwaEdit.Text = guidNazwa;
			UlicaEdit.Text = guidUlica;
			TelEdit.Text = guidTelefon;
			
			SaveBtn.Click();

			Dm.ObjectSpace.ClearTracking();
			Lokalizacja lFromDb = Dm.Retrieve<Lokalizacja>(idLokalizacja);
			int idAdres = (int)lFromDb.IdAdres;
			int idKontakt = (int)lFromDb.IdKontakt;

			Assert.AreEqual(guidNazwa, lFromDb.Nazwa, "nazwa");
			Assert.AreEqual(guidTelefon, lFromDb.Kontakt.Tel, "telefon");
			Assert.AreEqual(guidUlica, lFromDb.Adres.Ulica, "ulica");

			lFromDb.Delete();
			Adres a = Dm.Retrieve<Adres>(idAdres);
			a.Delete();
			Kontakt k = Dm.Retrieve<Kontakt>(idKontakt);
			k.Delete();
		}
	}
}
