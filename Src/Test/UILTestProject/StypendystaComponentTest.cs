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
    public class StypendystaComponentTest: BaseComponentTest
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
                return "http://localhost:1218/Testy/StypendystaTest.aspx";
            }
            else
            {
                return "http://localhost/WebStypendia/Testy/StypendystaTest.aspx";
            }
        }

        [Test]
        public override void DisplayTest()
        {
            //CheckPage();

            Stypendysta s = Stypendysta.GetWellKnown(string.Empty, DateTime.Now);
            s.Szkola.Save(null);
            s.Szkola = s.Szkola;
            s.Save(null);

            IdTb.Text = s.Id.ToString();
            PostBtn.Click();

            StypendystaDisplayTest(s, "ctl02", CurrentWebForm);

            s.Delete();
        }

        public static void StypendystaDisplayTest(Stypendysta s, string prefix, WebForm currentWebForm)
        {
            LabelTester imie = new LabelTester(GetCtrlId(prefix,"Imie"), currentWebForm);
            LabelTester imie2 = new LabelTester(GetCtrlId(prefix,"Imie2"), currentWebForm);
            LabelTester nazwisko = new LabelTester(GetCtrlId(prefix,"Nazwisko"), currentWebForm);
            LabelTester pesel = new LabelTester(GetCtrlId(prefix,"Pesel"), currentWebForm);
            LabelTester nip = new LabelTester(GetCtrlId(prefix,"Nip"), currentWebForm);
            LabelTester szkola = new LabelTester(GetCtrlId(prefix,"Szkola"), currentWebForm);
            LabelTester uzytkownikWprowadzajacy = new LabelTester(GetCtrlId(prefix,"UzytkownikWprowadzajacy"), currentWebForm);
            LabelTester dataWprowadzenia = new LabelTester(GetCtrlId(prefix,"DataWprowadzenia"), currentWebForm);

            Assert.AreEqual(s.Imie, imie.Text, "B��dne imi�.");
            Assert.AreEqual(s.Imie2, imie2.Text, "B��dne imi�2.");
            Assert.AreEqual(s.Nazwisko, nazwisko.Text, "B��dne nazwisko.");
            Assert.AreEqual(s.Pesel, pesel.Text, "B��dny pesel.");
            Assert.AreEqual(s.Nip, nip.Text, "B��dny NIP.");
            Assert.AreEqual(s.Szkola.ToString(), szkola.Text, "B��dna szko�a.");
            Assert.AreEqual(s.UzytkownikWprowadzajacy, uzytkownikWprowadzajacy.Text, "B��dny u�ytkownik wprowadzaj�cy.");
//            Assert.IsTrue(s.DataWprowadzenia == DateTime.Parse(dataWprowadzenia.Text), "B��dna data wprowadzenia.");

            AdresComponentTest.AdresDisplayTest(s.AdresZameldowania, prefix+"_ctl01", currentWebForm);
            KontaktComponentTest.KontaktDisplayTest(s.Kontakt, prefix+"_ctl00", currentWebForm);
        }

        public static void SetData(Stypendysta s, string prefix, WebForm currentWebForm)
        {
            TextBoxTester imie = new TextBoxTester(GetCtrlId(prefix, "ImieEdit"), currentWebForm);
            TextBoxTester imie2 = new TextBoxTester(GetCtrlId(prefix, "Imie2Edit"), currentWebForm);
            TextBoxTester nazwisko = new TextBoxTester(GetCtrlId(prefix, "NazwiskoEdit"), currentWebForm);
            TextBoxTester pesel = new TextBoxTester(GetCtrlId(prefix, "PeselEdit"), currentWebForm);
            TextBoxTester nip = new TextBoxTester(GetCtrlId(prefix, "NipEdit"), currentWebForm);
            DropDownListTester szkola = new DropDownListTester(GetCtrlId(prefix, "SzkolaEdit"), currentWebForm);
            LabelTester uzytkownikWprowadzajacy = new LabelTester(GetCtrlId(prefix, "UzytkownikWprowadzajacyEdit"), currentWebForm);
            LabelTester dataWprowadzenia = new LabelTester(GetCtrlId(prefix, "DataWprowadzeniaEdit"), currentWebForm);

            imie.Text = s.Imie;
            imie2.Text = s.Imie2;
            nazwisko.Text = s.Nazwisko;
            pesel.Text = s.Pesel;
            nip.Text = s.Nip;
            szkola.SelectedValue = s.Szkola.Id.ToString();
            //uzytkownikWprowadzajacy.Text = s.UzytkownikWprowadzajacy;
            //dataWprowadzenia.Text = s.DataWprowadzenia.ToString();

            AdresComponentTest.SetData(s.AdresZameldowania, prefix + "_ctl01", currentWebForm);
            KontaktComponentTest.SetData(s.Kontakt, prefix + "_ctl00", currentWebForm);
        }

        public static void CompareData(Stypendysta saved, Stypendysta fromDb)
        {
            Assert.IsTrue((saved != null && fromDb != null) || (saved == null && fromDb == null)
                , "Stypendysta zapisany lub z bazy jest nullem, a 2 nie.");

            Assert.AreEqual(saved.Imie, fromDb.Imie, "B��dne imi�.");
            Assert.AreEqual(saved.Imie2, fromDb.Imie2, "B��dne imi�2.");
            Assert.AreEqual(saved.Nazwisko, fromDb.Nazwisko, "B��dne nazwisko.");
            Assert.AreEqual(saved.Pesel, fromDb.Pesel, "B��dny PESEL.");
            Assert.AreEqual(saved.Nip, fromDb.Nip, "B��dny NIP.");
            Assert.AreEqual(saved.UzytkownikWprowadzajacy, fromDb.UzytkownikWprowadzajacy, "B��dny uzytkownik wprowadzaj�cy.");
            // to nigdy si� nie b�dzie zgadza�
            //Assert.AreEqual(saved.DataWprowadzenia, fromDb.DataWprowadzenia, "B��dna data wprowadzenia.");
            Assert.AreEqual(saved.Szkola.Id, fromDb.Szkola.Id, "B��dna szko�a.");

            AdresComponentTest.CompareData(saved.AdresZameldowania, fromDb.AdresZameldowania);
            KontaktComponentTest.CompareData(saved.Kontakt, fromDb.Kontakt);
        }

        [Test]
        public override void InsertTest()
        {
            Stypendysta s = Stypendysta.GetWellKnown(string.Empty, DateTime.Now);
            s.Szkola.Save(null);
            s.Szkola = s.Szkola;

            NewBtn.Click();

            s.Nazwisko = Guid.NewGuid().ToString();
            SetData (s, "ctl02", CurrentWebForm);

            SaveBtn.Click();

            Assert.IsTrue(int.Parse(sessionLbl.Text) > 0, "Niezapisano nowego rekordu");

            Stypendysta fromDb = Dm.Retrieve<Stypendysta>(int.Parse(sessionLbl.Text));

            CompareData(s, fromDb);

            fromDb.Delete();
        }

        [Test]
        public override void EditTest()
        {
/*
            Szkola sz = null;

            Stypendysta s = Stypendysta.GetWellKnown(string.Empty, DateTime.Now);

            s.Szkola.Save(null);
            s.Szkola = s.Szkola;
            sz = s.Szkola;
            s.Save(null);

            IdTb.Text = s.Id.ToString();
            PostBtn.Click();

            StypendystaDisplayTest(s, "ctl02", CurrentWebForm);

            EditBtn.Click();

            s.Nazwisko = Guid.NewGuid().ToString();
            SetData(s, "ctl02", CurrentWebForm);

            SaveBtn.Click();

            Stypendysta fromDb = (Stypendysta)s.Resync();

            CompareData(s, fromDb);

            fromDb.Delete();
            sz.Delete();
 */ 
        }
    }
}
