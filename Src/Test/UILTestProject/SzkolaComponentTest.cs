using System;
using System.Security.Principal;
using NUnit.Extensions.Asp;
using NUnit.Extensions.Asp.AspTester;
using NUnit.Framework;
using Vulcan.Stypendia.Bll;
using Wilson.ORMapper;
using System.Text;


namespace GUITests
{
	[TestFixture]
	public class SzkolaComponentTest : BaseComponentTest
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
               return "http://localhost:1218/Testy/SzkolaTest.aspx";
           }
           else
           {
               return "http://localhost/WebStypendia/Testy/SzkolaTest.aspx";
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
/*
			LabelTester Nazwa = new LabelTester("ctl01_Nazwa", CurrentWebForm);
			LabelTester Skrot = new LabelTester("ctl01_Skrot", CurrentWebForm);
			LabelTester Nip = new LabelTester("ctl01_Nip", CurrentWebForm);
			LabelTester Regon = new LabelTester("ctl01_Regon", CurrentWebForm);
			LabelTester Us = new LabelTester("ctl01_Us", CurrentWebForm);
			LabelTester Poczta = new LabelTester("ctl01_ctl00_Poczta", CurrentWebForm);
			LabelTester Wojewodztwo = new LabelTester("ctl01_ctl00_Wojewodztwo", CurrentWebForm);
			LabelTester Gmina = new LabelTester("ctl01_ctl00_Gmina", CurrentWebForm);
			LabelTester Ulica = new LabelTester("ctl01_ctl00_Ulica", CurrentWebForm);
			LabelTester Kraj = new LabelTester("ctl01_ctl00_Kraj", CurrentWebForm);
			LabelTester Kod = new LabelTester("ctl01_ctl00_Kod", CurrentWebForm);
			LabelTester Powiat = new LabelTester("ctl01_ctl00_Powiat", CurrentWebForm);
			LabelTester Gus = new LabelTester("ctl01_ctl00_Gus", CurrentWebForm);
			LabelTester Miejscowosc = new LabelTester("ctl01_ctl00_Miejscowosc", CurrentWebForm);
			LabelTester Tel = new LabelTester("ctl01_ctl01_Tel", CurrentWebForm);
			LabelTester Tel2 = new LabelTester("ctl01_ctl01_Tel2", CurrentWebForm);
			LabelTester Fax = new LabelTester("ctl01_ctl01_Fax", CurrentWebForm);
			LabelTester Www = new LabelTester("ctl01_ctl01_Www", CurrentWebForm);
			LabelTester Email = new LabelTester("ctl01_ctl01_Email", CurrentWebForm);

			Szkola s = Szkola.GetWellKnown();
			string unNazwa = Guid.NewGuid().ToString();
			s.Nazwa = unNazwa;
			s.Save(null);
			int idAdres = (int)s.IdAdres;
			int idKontakt = (int)s.IdKontakt;

			IdTb.Text = s.Id.ToString();
			PostBtn.Click();
			Assert.AreEqual(s.Id, int.Parse(sessionLbl.Text), "Z造 id szkoly");

			Assert.AreEqual(s.Nazwa, Nazwa.Text, "z豉 nazwa szkoly");
			Assert.AreEqual(s.Skrot, Skrot.Text, "z造 skr鏒 szkoly");
			Assert.AreEqual(s.Nip, Nip.Text, "z造 nip");
			Assert.AreEqual(s.Regon, Regon.Text, "z造 regon");
			Assert.AreEqual(s.Us, Us.Text, "z造 us");

			Assert.AreEqual(s.Adres.Poczta, Poczta.Text, "poczta");
			Assert.AreEqual(s.Adres.Wojewodztwo, Wojewodztwo.Text, "wojew鏚ztwo");
			Assert.AreEqual(s.Adres.Gmina, Gmina.Text, "gmina");
			Assert.AreEqual(s.Adres.Ulica, Ulica.Text, "ulica");
			Assert.AreEqual(s.Adres.Kraj, Kraj.Text, "kraj");
			Assert.AreEqual(s.Adres.Kod, Kod.Text, "kod");
			Assert.AreEqual(s.Adres.Powiat, Powiat.Text, "powiat");
			Assert.AreEqual(s.Adres.Gus, Gus.Text, "gus");
			Assert.AreEqual(s.Adres.Miejscowosc, Miejscowosc.Text, "miejscowosc");
			Assert.AreEqual(s.Kontakt.Tel, Tel.Text, "tel");
			Assert.AreEqual(s.Kontakt.Tel2, Tel2.Text, "tel2");
			Assert.AreEqual(s.Kontakt.Fax, Fax.Text, "fax");
			Assert.AreEqual(s.Kontakt.Www, Www.Text, "www");
			Assert.AreEqual(s.Kontakt.Email, Email.Text, "email");
					
			s.Delete();
			Adres a = Dm.Retrieve<Adres>(idAdres);
			a.Delete();
			Kontakt k = Dm.Retrieve<Kontakt>(idKontakt);
			k.Delete();
 */ 
		}

		[Test]
		public override void InsertTest()
		{
/*
			TextBoxTester Nazwa = new TextBoxTester("ctl01_NazwaEdit", CurrentWebForm);
			TextBoxTester Skrot = new TextBoxTester("ctl01_SkrotEdit", CurrentWebForm);
			
			TextBoxTester Nip = new TextBoxTester("ctl01_NipEdit", CurrentWebForm);
			TextBoxTester Regon = new TextBoxTester("ctl01_RegonEdit", CurrentWebForm);
			TextBoxTester Us = new TextBoxTester("ctl01_UsEdit", CurrentWebForm);

			TextBoxTester Poczta = new TextBoxTester("ctl01_ctl00_PocztaEdit", CurrentWebForm);
			TextBoxTester Wojewodztwo = new TextBoxTester("ctl01_ctl00_WojewodztwoEdit", CurrentWebForm);
			TextBoxTester Gmina = new TextBoxTester("ctl01_ctl00_GminaEdit", CurrentWebForm);
			TextBoxTester Ulica = new TextBoxTester("ctl01_ctl00_UlicaEdit", CurrentWebForm);
			TextBoxTester Kraj = new TextBoxTester("ctl01_ctl00_KrajEdit", CurrentWebForm);
			TextBoxTester Kod = new TextBoxTester("ctl01_ctl00_KodEdit", CurrentWebForm);
			TextBoxTester Powiat = new TextBoxTester("ctl01_ctl00_PowiatEdit", CurrentWebForm);
			TextBoxTester Gus = new TextBoxTester("ctl01_ctl00_GusEdit", CurrentWebForm);
			TextBoxTester Miejscowosc = new TextBoxTester("ctl01_ctl00_MiejscowoscEdit", CurrentWebForm);
			TextBoxTester Tel = new TextBoxTester("ctl01_ctl01_TelEdit", CurrentWebForm);
			TextBoxTester Tel2 = new TextBoxTester("ctl01_ctl01_Tel2Edit", CurrentWebForm);
			TextBoxTester Fax = new TextBoxTester("ctl01_ctl01_FaxEdit", CurrentWebForm);
			TextBoxTester Www = new TextBoxTester("ctl01_ctl01_WwwEdit", CurrentWebForm);
			TextBoxTester Email = new TextBoxTester("ctl01_ctl01_EmailEdit", CurrentWebForm);

			NewBtn.Click();

			string guidNazwa = Guid.NewGuid().ToString();
			string guidUs = Guid.NewGuid().ToString();
			string guidUlica = Guid.NewGuid().ToString();
			string guidTelefon = UnikalnyNumerTelefonu();

			Szkola s2i = Szkola.GetWellKnown();
			s2i.Nazwa = guidNazwa;
			s2i.Us = guidUs;
			s2i.Adres.Ulica = guidUlica;
			s2i.Kontakt.Tel = guidTelefon;

			Nazwa.Text = s2i.Nazwa;
			Skrot.Text = s2i.Skrot;
			Regon.Text = s2i.Regon;
			Nip.Text = s2i.Nip;
			Us.Text = s2i.Us;

			Poczta.Text = s2i.Adres.Poczta;
			Wojewodztwo.Text = s2i.Adres.Wojewodztwo;
			Gmina.Text = s2i.Adres.Gmina;
			Ulica.Text = s2i.Adres.Ulica;
			Kraj.Text = s2i.Adres.Kraj;
			Kod.Text = s2i.Adres.Kod;
			Powiat.Text = s2i.Adres.Powiat;
			Gus.Text = s2i.Adres.Gus;
			Miejscowosc.Text = s2i.Adres.Miejscowosc;

			Tel.Text = s2i.Kontakt.Tel;
			Tel2.Text = s2i.Kontakt.Tel2;
			Fax.Text = s2i.Kontakt.Fax;
			Www.Text = s2i.Kontakt.Www;
			Email.Text = s2i.Kontakt.Email;

			SaveBtn.Click();
			Assert.IsTrue(int.Parse(sessionLbl.Text) > 0, "Niezapisano nowego rekordu");

			int idSzkola = 0;
			if (int.TryParse(sessionLbl.Text, out idSzkola))
				if (idSzkola > 0)
				{
					Dm.ObjectSpace.ClearTracking();
					Szkola sFromDb = Dm.Retrieve<Szkola>(idSzkola);
					int idAdres = (int)sFromDb.IdAdres;
					int idKontakt = (int)sFromDb.IdKontakt;

					Assert.AreEqual(s2i.Nazwa, sFromDb.Nazwa, "nazwa");
					Assert.AreEqual(s2i.Skrot, sFromDb.Skrot, "skr鏒");
					Assert.AreEqual(s2i.Regon, sFromDb.Regon, "regon");
					Assert.AreEqual(s2i.Nip, sFromDb.Nip, "nip");
					Assert.AreEqual(s2i.Us, sFromDb.Us, "us");

					Assert.AreEqual(s2i.Kontakt.Fax, sFromDb.Kontakt.Fax, "fax");
					Assert.AreEqual(s2i.Kontakt.Tel, sFromDb.Kontakt.Tel, "telefon");
					Assert.AreEqual(s2i.Kontakt.Tel2, sFromDb.Kontakt.Tel2, "telefon 2");
					Assert.AreEqual(s2i.Kontakt.Email, sFromDb.Kontakt.Email, "e-mail");
					Assert.AreEqual(s2i.Kontakt.Www, sFromDb.Kontakt.Www, "WWW");

					Assert.AreEqual(s2i.Adres.Poczta, sFromDb.Adres.Poczta, "poczta");
					Assert.AreEqual(s2i.Adres.Wojewodztwo, sFromDb.Adres.Wojewodztwo, "woj");
					Assert.AreEqual(s2i.Adres.Gmina, sFromDb.Adres.Gmina, "gmi");
					Assert.AreEqual(s2i.Adres.Ulica, sFromDb.Adres.Ulica, "ulica");
					Assert.AreEqual(s2i.Adres.Kraj, sFromDb.Adres.Kraj, "kraj");
					Assert.AreEqual(s2i.Adres.Kod, sFromDb.Adres.Kod, "kod");
					Assert.AreEqual(s2i.Adres.Powiat, sFromDb.Adres.Powiat, "powiat");
					Assert.AreEqual(s2i.Adres.Gus, sFromDb.Adres.Gus, "gus");
					Assert.AreEqual(s2i.Adres.Miejscowosc, sFromDb.Adres.Miejscowosc, "msc");

					sFromDb.Delete();
					Adres a = Dm.Retrieve<Adres>(idAdres);
					a.Delete();
					Kontakt k = Dm.Retrieve<Kontakt>(idKontakt);
					k.Delete();
				}
 */ 
		}

		[Test]
		public override void EditTest()
		{
/*
			LabelTester Nazwa = new LabelTester("ctl01_Nazwa", CurrentWebForm);
			LabelTester Us = new LabelTester("ctl01_Us", CurrentWebForm);
			LabelTester Ulica = new LabelTester("ctl01_ctl00_Ulica", CurrentWebForm);
			LabelTester Tel = new LabelTester("ctl01_ctl01_Tel", CurrentWebForm);
			TextBoxTester NazwaEdit = new TextBoxTester("ctl01_NazwaEdit", CurrentWebForm);
			TextBoxTester UsEdit = new TextBoxTester("ctl01_UsEdit", CurrentWebForm);
			TextBoxTester UlicaEdit = new TextBoxTester("ctl01_ctl00_UlicaEdit", CurrentWebForm);
			TextBoxTester TelEdit = new TextBoxTester("ctl01_ctl01_TelEdit", CurrentWebForm);

			Szkola s = Szkola.GetWellKnown();
			s.Save(null);
			int idSzkola = s.Id;

			IdTb.Text = s.Id.ToString();
			PostBtn.Click();
			Assert.AreEqual(s.Nazwa, Nazwa.Text, "z豉 nazwa lokalizacji");
			Assert.AreEqual(s.Us, Us.Text, "us");
			Assert.AreEqual(s.Adres.Ulica, Ulica.Text, "ulica");
			Assert.AreEqual(s.Kontakt.Tel, Tel.Text, "tel");

			string guidNazwa = Guid.NewGuid().ToString();
			string guidUs = Guid.NewGuid().ToString();
			string guidUlica = Guid.NewGuid().ToString();
			string guidTelefon = UnikalnyNumerTelefonu();

			EditBtn.Click();
			NazwaEdit.Text = guidNazwa;
			UsEdit.Text = guidUs;
			UlicaEdit.Text = guidUlica;
			TelEdit.Text = guidTelefon;

			SaveBtn.Click();

			Dm.ObjectSpace.ClearTracking();
			Szkola sFromDb = Dm.Retrieve<Szkola>(idSzkola);
			int idAdres = (int)sFromDb.IdAdres;
			int idKontakt = (int)sFromDb.IdKontakt;

			Assert.AreEqual(guidNazwa, sFromDb.Nazwa, "nazwa");
			Assert.AreEqual(guidUs, sFromDb.Us, "US");
			Assert.AreEqual(guidTelefon, sFromDb.Kontakt.Tel, "telefon");
			Assert.AreEqual(guidUlica, sFromDb.Adres.Ulica, "ulica");

			sFromDb.Delete();
			Adres a = Dm.Retrieve<Adres>(idAdres);
			a.Delete();
			Kontakt k = Dm.Retrieve<Kontakt>(idKontakt);
			k.Delete();
 */ 
		}

	    
	}
}
