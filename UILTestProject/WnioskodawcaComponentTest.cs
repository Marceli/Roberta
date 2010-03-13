using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using NUnit.Extensions.Asp.AspTester;
using NUnit.Extensions.Asp;
using Vulcan.Stypendia.Bll;
using Wilson.ORMapper;

namespace GUITests
{
    [TestFixture]
    public class WnioskodawcaComponentTest: BaseComponentTest
    {
        #region fields

        private ButtonTester PostBtn;
        private ButtonTester NewBtn;
        private ButtonTester EditBtn;
        private ButtonTester DelBtn;
        private ButtonTester SaveBtn;
        private ButtonTester CancelBtn;

        private TextBoxTester IdTb;
        private LabelTester sessionLbl;

        #endregion // fields

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

        protected override string GetUrl()
        {
           
            if (WindowsIdentity.GetCurrent().Name == @"WIN\Marcel")
            {
                return "http://localhost:1218/Testy/WnioskodawcaTest.aspx";
            }
            else
            {
                return "http://localhost/WebStypendia/Testy/WnioskodawcaTest.aspx";
            }
        }

        [Test]
        public override void DisplayTest()
        {
            //CheckPage();

            Wnioskodawca s = Wnioskodawca.GetWellKnown(string.Empty, DateTime.Now);
            s.Save(null);

            IdTb.Text = s.Id.ToString();
            PostBtn.Click();

            WnioskodawcaDisplayTest(s, "ctl02", CurrentWebForm);

            s.Delete();
        }

        public static void WnioskodawcaDisplayTest(Wnioskodawca s, string prefix, WebForm currentWebForm)
        {
            LabelTester imie = new LabelTester(GetCtrlId(prefix,"Imie"), currentWebForm);
            LabelTester imie2 = new LabelTester(GetCtrlId(prefix,"Imie2"), currentWebForm);
            LabelTester nazwisko = new LabelTester(GetCtrlId(prefix,"Nazwisko"), currentWebForm);
            LabelTester pesel = new LabelTester(GetCtrlId(prefix,"Pesel"), currentWebForm);
            LabelTester nip = new LabelTester(GetCtrlId(prefix,"Nip"), currentWebForm);
            LabelTester typ = new LabelTester(GetCtrlId(prefix, "WnioskodawcaTyp"), currentWebForm);
            LabelTester uzytkownikWprowadzajacy = new LabelTester(GetCtrlId(prefix,"UzytkownikWprowadzajacy"), currentWebForm);
            LabelTester dataWprowadzenia = new LabelTester(GetCtrlId(prefix,"DataWprowadzenia"), currentWebForm);

            Assert.AreEqual(s.Imie, imie.Text, "B³êdne imiê.");
            Assert.AreEqual(s.Imie2, imie2.Text, "B³êdne imiê2.");
            Assert.AreEqual(s.Nazwisko, nazwisko.Text, "B³êdne nazwisko.");
            Assert.AreEqual(s.Pesel, pesel.Text, "B³êdny pesel.");
            Assert.AreEqual(s.Nip.ToString(), nip.Text, "B³êdny NIP.");
            Assert.AreEqual(s.WnioskodawcaTyp.ToString(), typ.Text, "B³êdny typ.");
            Assert.AreEqual(s.UzytkownikWprowadzajacy, uzytkownikWprowadzajacy.Text, "B³êdny u¿ytkownik wprowadzaj¹cy.");
//            Assert.IsTrue(s.DataWprowadzenia == DateTime.Parse(dataWprowadzenia.Text), "B³êdna data wprowadzenia.");

            AdresComponentTest.AdresDisplayTest(s.AdresZameldowania, prefix+"_ctl00", currentWebForm);
            KontaktComponentTest.KontaktDisplayTest(s.Kontakt, prefix+"_ctl01", currentWebForm);
        }

        public static void SetData(Wnioskodawca s, string prefix, WebForm currentWebForm)
        {
            TextBoxTester imie = new TextBoxTester(GetCtrlId(prefix, "ImieEdit"), currentWebForm);
            TextBoxTester imie2 = new TextBoxTester(GetCtrlId(prefix, "Imie2Edit"), currentWebForm);
            TextBoxTester nazwisko = new TextBoxTester(GetCtrlId(prefix, "NazwiskoEdit"), currentWebForm);
            TextBoxTester pesel = new TextBoxTester(GetCtrlId(prefix, "PeselEdit"), currentWebForm);
            TextBoxTester nip = new TextBoxTester(GetCtrlId(prefix, "NipEdit"), currentWebForm);
            DropDownListTester typ = new DropDownListTester(GetCtrlId(prefix, "WnioskodawcaTypEdit"), currentWebForm);
            LabelTester uzytkownikWprowadzajacy = new LabelTester(GetCtrlId(prefix, "UzytkownikWprowadzajacyEdit"), currentWebForm);
            LabelTester dataWprowadzenia = new LabelTester(GetCtrlId(prefix, "DataWprowadzeniaEdit"), currentWebForm);

            imie.Text = s.Imie;
            imie2.Text = s.Imie2;
            nazwisko.Text = s.Nazwisko;
            pesel.Text = s.Pesel;
            nip.Text = s.Nip;
            typ.SelectedValue =s.WnioskodawcaTyp.ToString();
            //uzytkownikWprowadzajacy.Text = s.UzytkownikWprowadzajacy;
            //dataWprowadzenia.Text = s.DataWprowadzenia.ToString();

            AdresComponentTest.SetData(s.AdresZameldowania, prefix+"_ctl00", currentWebForm);
            KontaktComponentTest.SetData(s.Kontakt, prefix+"_ctl01", currentWebForm);
        }

        public static void CompareData(Wnioskodawca saved, Wnioskodawca fromDb)
        {
            Assert.IsTrue((saved != null && fromDb != null) || (saved == null && fromDb == null)
                , "Wnioskodawca zapisany lub z bazy jest nullem, a 2 nie.");

            Assert.AreEqual(saved.Imie, fromDb.Imie, "B³êdne imiê.");
            Assert.AreEqual(saved.Imie2, fromDb.Imie2, "B³êdne imiê2.");
            Assert.AreEqual(saved.Nazwisko, fromDb.Nazwisko, "B³êdne nazwisko.");
            Assert.AreEqual(saved.Pesel, fromDb.Pesel, "B³êdny PESEL.");
            Assert.AreEqual(saved.Nip, fromDb.Nip, "B³êdny NIP.");
            Assert.AreEqual(saved.UzytkownikWprowadzajacy, fromDb.UzytkownikWprowadzajacy, "B³êdny uzytkownik wprowadzaj¹cy.");
            // to nigdy siê nie bêdzie zgadzaæ
            //Assert.AreEqual(saved.DataWprowadzenia, fromDb.DataWprowadzenia, "B³êdna data wprowadzenia.");
            Assert.AreEqual(saved.WnioskodawcaTyp, fromDb.WnioskodawcaTyp, "B³êdny typ.");

            AdresComponentTest.CompareData(saved.AdresZameldowania, fromDb.AdresZameldowania);
            KontaktComponentTest.CompareData(saved.Kontakt, fromDb.Kontakt);
        }

        [Test]
        public override void InsertTest()
        {
            Wnioskodawca s = Wnioskodawca.GetWellKnown(string.Empty, DateTime.Now);
            

            NewBtn.Click();

            s.Nazwisko = Guid.NewGuid().ToString();
            SetData (s, "ctl02", CurrentWebForm);

            SaveBtn.Click();

            Assert.IsTrue(int.Parse(sessionLbl.Text) > 0, "Niezapisano nowego rekordu");

            Wnioskodawca fromDb = Dm.Retrieve<Wnioskodawca>(int.Parse(sessionLbl.Text));

            CompareData(s, fromDb);

            fromDb.Delete();
        }

        [Test]
        public override void EditTest()
        {
            Wnioskodawca s = Wnioskodawca.GetWellKnown(string.Empty, DateTime.Now);

            s.Save(null);

            IdTb.Text = s.Id.ToString();
            PostBtn.Click();

            WnioskodawcaDisplayTest(s, "ctl02", CurrentWebForm);

            EditBtn.Click();

            s.Nazwisko = Guid.NewGuid().ToString();
            SetData(s, "ctl02", CurrentWebForm);

            SaveBtn.Click();

            Wnioskodawca fromDb = (Wnioskodawca)s.Resync();

            CompareData(s, fromDb);

            fromDb.Delete();
        }
    }
}
