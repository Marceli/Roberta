using System;
using System.Security.Principal;

using NUnit.Extensions.Asp;
using NUnit.Extensions.Asp.AspTester;
using NUnit.Framework;
using Vulcan.Stypendia.Bll;

namespace GUITests
{
	[TestFixture]
    public class AdresComponentTest : BaseComponentTest
	{
        private TextBoxTester Powiat;
        private TextBoxTester Kod;
        private TextBoxTester Gus;
        private TextBoxTester Kraj;
        private TextBoxTester Ulica;
        private TextBoxTester Gmina;
        private TextBoxTester Wojewodztwo;
        private TextBoxTester Poczta;
        private TextBoxTester Miejscowosc;

        private LabelTester PowiatLbl;
        private LabelTester KodLbl;
        private LabelTester GusLbl;
        private LabelTester KrajLbl;
        private LabelTester UlicaLbl;
        private LabelTester GminaLbl;
        private LabelTester WojewodztwoLbl;
        private LabelTester PocztaLbl;
        private LabelTester MiejscowoscLbl;

       
        private ButtonTester PostBtn;
        private ButtonTester SaveBtn;
        private ButtonTester NewBtn;
        private ButtonTester EditBtn;
        private ButtonTester DelBtn;
        private TextBoxTester IdTb;
        private LabelTester sessionLbl;

        protected override string GetUrl()
        {
           
            if(WindowsIdentity.GetCurrent().Name==@"WIN\Marcel")
            {
                return "http://localhost:1218/Testy/AdresTest.aspx";
            }
            else
            {
                return "http://localhost/WebStypendia/Testy/AdresTest.aspx";
            }
	    }
		


        /// <summary>
        /// Executed before each test method is run.  Override in subclasses to do subclass
        /// set up.  NOTE: [SetUp] attribute cannot be used in subclasses because it is already
        /// in use.
        /// </summary>
        protected override void SetUp()
        {
            PostBtn = new ButtonTester("PostBtn", CurrentWebForm);
            SaveBtn = new ButtonTester("SaveBtn", CurrentWebForm);
             NewBtn = new ButtonTester("NewBtn", CurrentWebForm);
             EditBtn = new ButtonTester("EditBtn", CurrentWebForm);
             DelBtn = new ButtonTester("DelBtn", CurrentWebForm);
            IdTb = new TextBoxTester("IdTb", CurrentWebForm);
            
            sessionLbl = new LabelTester("SessionLbl", CurrentWebForm);

            Powiat = new TextBoxTester("ctl02_PowiatEdit", CurrentWebForm);
            Kod = new TextBoxTester("ctl02_KodEdit", CurrentWebForm);
            Gus = new TextBoxTester("ctl02_GusEdit", CurrentWebForm);
            Kraj = new TextBoxTester("ctl02_KrajEdit", CurrentWebForm);
            Ulica = new TextBoxTester("ctl02_UlicaEdit", CurrentWebForm);
            Gmina = new TextBoxTester("ctl02_GminaEdit", CurrentWebForm);
            Wojewodztwo = new TextBoxTester("ctl02_WojewodztwoEdit", CurrentWebForm);
            Poczta = new TextBoxTester("ctl02_PocztaEdit", CurrentWebForm);
            Miejscowosc = new TextBoxTester("ctl02_MiejscowoscEdit", CurrentWebForm);
            
            PowiatLbl = new LabelTester("ctl02_Powiat", CurrentWebForm);
            KodLbl = new LabelTester("ctl02_Kod", CurrentWebForm);
            GusLbl = new LabelTester("ctl02_Gus", CurrentWebForm);
            KrajLbl = new LabelTester("ctl02_Kraj", CurrentWebForm);
            UlicaLbl = new LabelTester("ctl02_Ulica", CurrentWebForm);
            GminaLbl = new LabelTester("ctl02_Gmina", CurrentWebForm);
            WojewodztwoLbl = new LabelTester("ctl02_Wojewodztwo", CurrentWebForm);
            PocztaLbl = new LabelTester("ctl02_Poczta", CurrentWebForm);
            MiejscowoscLbl = new LabelTester("ctl02_Miejscowosc", CurrentWebForm);
            base.SetUp();
        }

        
        [Test]
        public override void DisplayTest()
        {
            Adres a = Adres.GetWellKnown();
            string unikalnaMiejscowosc = Guid.NewGuid().ToString();
            a.Miejscowosc = unikalnaMiejscowosc;
            a.Save(null);
            IdTb.Text = a.Id.ToString();
            PostBtn.Click();

            AreEqual(a.Id,int.Parse(sessionLbl.Text), "Z³y id");

            AdresDisplayTest(a, "ctl02", CurrentWebForm);

            a.Delete();
        }

        public static void AdresDisplayTest(Adres a, string prefix, WebForm currentWebForm)
        {
            LabelTester PowiatLbl = new LabelTester(GetCtrlId(prefix,"Powiat"), currentWebForm);
            LabelTester KodLbl = new LabelTester(GetCtrlId(prefix,"Kod"), currentWebForm);
            LabelTester GusLbl = new LabelTester(GetCtrlId(prefix,"Gus"), currentWebForm);
            LabelTester KrajLbl = new LabelTester(GetCtrlId(prefix,"Kraj"), currentWebForm);
            LabelTester UlicaLbl = new LabelTester(GetCtrlId(prefix,"Ulica"), currentWebForm);
            LabelTester GminaLbl = new LabelTester(GetCtrlId(prefix,"Gmina"), currentWebForm);
            LabelTester WojewodztwoLbl = new LabelTester(GetCtrlId(prefix,"Wojewodztwo"), currentWebForm);
            LabelTester PocztaLbl = new LabelTester(GetCtrlId(prefix,"Poczta"), currentWebForm);
            LabelTester MiejscowoscLbl = new LabelTester(GetCtrlId(prefix,"Miejscowosc"), currentWebForm);

            AreEqual(a.Powiat, PowiatLbl.Text, "Z³y Powiat");
            AreEqual(a.Kod, KodLbl.Text, "Z³y Kod");
            AreEqual(a.Gus, GusLbl.Text, "Z³y Gus");
            AreEqual(a.Kraj, KrajLbl.Text, "Z³y Kraj");
            AreEqual(a.Ulica, UlicaLbl.Text, "Z³y Ulica");
            AreEqual(a.Gmina, GminaLbl.Text, "Z³y Gmina");
            AreEqual(a.Wojewodztwo, WojewodztwoLbl.Text, "Z³y Wojewodztwo");
            AreEqual(a.Poczta, PocztaLbl.Text, "Z³y Poczta");
            AreEqual(a.Miejscowosc, MiejscowoscLbl.Text, "Z³y Miejscowosc");
        }

        public static void SetData(Adres a, string prefix, WebForm currentWebForm)
        {
            TextBoxTester Powiat = new TextBoxTester(GetCtrlId(prefix, "PowiatEdit"), currentWebForm);
            TextBoxTester Kod = new TextBoxTester(GetCtrlId(prefix, "KodEdit"), currentWebForm);
            TextBoxTester Gus = new TextBoxTester(GetCtrlId(prefix, "GusEdit"), currentWebForm);
            TextBoxTester Kraj = new TextBoxTester(GetCtrlId(prefix, "KrajEdit"), currentWebForm);
            TextBoxTester Ulica = new TextBoxTester(GetCtrlId(prefix, "UlicaEdit"), currentWebForm);
            TextBoxTester Gmina = new TextBoxTester(GetCtrlId(prefix, "GminaEdit"), currentWebForm);
            TextBoxTester Wojewodztwo = new TextBoxTester(GetCtrlId(prefix, "WojewodztwoEdit"), currentWebForm);
            TextBoxTester Poczta = new TextBoxTester(GetCtrlId(prefix, "PocztaEdit"), currentWebForm);
            TextBoxTester Miejscowosc = new TextBoxTester(GetCtrlId(prefix, "MiejscowoscEdit"), currentWebForm);

            Miejscowosc.Text = a.Miejscowosc;
            Powiat.Text = a.Powiat;
            Kod.Text = a.Kod;
            Gus.Text = a.Gus;
            Kraj.Text = a.Kraj;
            Ulica.Text = a.Ulica;
            Gmina.Text = a.Gmina;
            Wojewodztwo.Text = a.Wojewodztwo;
            Poczta.Text = a.Poczta;            
        }

        public static void CompareData(Adres saved, Adres fromDb)
        {
            Assert.IsTrue((saved != null && fromDb != null) || (saved == null && fromDb == null)
                , "Adres zapisany lub z bazy jest nullem, a 2 nie.");

            AreEqual(saved.Powiat, fromDb.Powiat, "Powiat");
            AreEqual(saved.Kod, fromDb.Kod, "Kod");
            AreEqual(saved.Gus, fromDb.Gus, "Gus");
            AreEqual(saved.Kraj, fromDb.Kraj, "Kraj");
            AreEqual(saved.Ulica, fromDb.Ulica, "Ulica");
            AreEqual(saved.Gmina, fromDb.Gmina, "Gmina");
            AreEqual(saved.Wojewodztwo, fromDb.Wojewodztwo, "Wojewodztwo");
            AreEqual(saved.Poczta, fromDb.Poczta, "Poczta");
            AreEqual(saved.Miejscowosc, fromDb.Miejscowosc, "Miejscowosc");
        }

        [Test]
        public override void InsertTest()
        {
            NewBtn.Click();
            Adres aToInsert = Adres.GetWellKnown();
            string unikalnaMiejscowosc = Guid.NewGuid().ToString();
            aToInsert.Miejscowosc = unikalnaMiejscowosc;

            SetData(aToInsert, "ctl02", CurrentWebForm);

            SaveBtn.Click();
            IsTrue(int.Parse(sessionLbl.Text) > 0, "Niezapisano nowego rekordu");
            Adres aFromDb = Dm.Retrieve<Adres>(int.Parse(sessionLbl.Text));

            CompareData(aToInsert, aFromDb);

            aFromDb.Delete();            
        }

        [Test]
        public override void EditTest()
        {            
            Adres a = Adres.GetWellKnown();
            a.Save(null);
            IdTb.Text = a.Id.ToString();
            PostBtn.Click();
            EditBtn.Click();

            a.Miejscowosc = Guid.NewGuid().ToString();

            SetData(a, "ctl02", CurrentWebForm);

            SaveBtn.Click();
            Adres aFromDB = (Adres)a.Resync();

            CompareData(a, aFromDB);

            aFromDB.Delete();           
        }

        //end of method
        //[Test]
        //public void AddNewAndDisplayTest()
        //{   
        //    CheckPage();
        //    Miejscowosc.Text=Guid.NewGuid().ToString();

        //    Assert.AreEqual("-1", sessionLbl.Text, "niezgodny numer sesji");
            
            
        //    //Assert.AreEqual("Jawor", miejscowosc.Text, "niezgodna miejscowoœæ- wroc³aw");
            
        //}

       


        //[Test]
        //public void TestLayout()
        //{
        //    AssertVisibility(name, true);
        //    AssertVisibility(comments, true);
        //    AssertVisibility(save, true);
        //        AssertVisibility(book, false);
        //}
        //[Test]
        //public void TestImie()
        //{
        //    string olo = imie.Text;
        //    AreEqual( "Owsiak", imie.Text,"imie");
        //}
        //[Test]
        //public void TestUserControl()
        //{
        //   // AssertEquals("imie", "MarcelInUserControl", tb.Text);

        //}

        //[Test]
        //public void TestSave()
        //{
        //    SignGuestBook("Dr. Seuss", "One Guest, Two Guest!  Guest Book, Best Book!");

        //    //AssertEquals("name", "", name.Text);
        //    //AssertEquals("comments", "", comments.Text);

        //    string[][] expected = new string[][]
        //    {
        //        new string[] {"Dr. Seuss", "One Guest, Two Guest!  Guest Book, Best Book!"}
        //    };
        //    AssertEquals("book", expected, book.TrimmedCells);
        //}

        //[Test]
        //public void TestSaveTwoItems()
        //{
        //    SignGuestBook("Dr. Seuss", "One Guest, Two Guest!  Guest Book, Best Book!");
        //    SignGuestBook("Dr. Freud", "That's quite a slip you have there.");

        //    string[][] expected = new string[][]
        //    {
        //        new string[] {"Dr. Seuss", "One Guest, Two Guest!  Guest Book, Best Book!"},
        //        new string[] {"Dr. Freud", "That's quite a slip you have there."}
        //    };
        //    AssertEquals("book", expected, book.TrimmedCells);
        //}

        //private void SignGuestBook(string nameToSign, string commentToSign)
        //{
        //    name.Text = nameToSign;
        //    comments.Text = commentToSign;
        //    save.Click();
        //}
			
	}
}
