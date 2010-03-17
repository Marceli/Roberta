using System;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using NUnit.Extensions.Asp.AspTester;
using Vulcan.Stypendia.Bll;
using NUnit.Extensions.Asp;


namespace GUITests
{
    [TestFixture]
    public class KontaktComponentTest :BaseComponentTest
    {
        private ButtonTester PostBtn;
        private ButtonTester NewBtn;
        private ButtonTester EditBtn;
        private ButtonTester DelBtn;
        private ButtonTester SaveBtn;
        private ButtonTester CancelBtn;

        private TextBoxTester IdTb;
        private LabelTester sessionLbl;

        

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

        private static string UnikalnyNumerTelefonu()
        {
            Random rand = new Random();
            StringBuilder str = new StringBuilder();
            str.Append("0");
            str.Append(rand.Next(10, 99).ToString());
            str.Append(rand.Next(1000000, 9999999).ToString());
            return str.ToString();
        }
       
        [Test]
        public override void DisplayTest()
        {
            Kontakt k = Kontakt.GetWellKnown();
            k.Tel = UnikalnyNumerTelefonu();
            k.Save(null);

            IdTb.Text = k.Id.ToString();
            PostBtn.Click();

            Assert.AreEqual(k.Id, int.Parse(sessionLbl.Text), "Z造 id");

            KontaktDisplayTest(k, "ctl02", CurrentWebForm);

            k.Delete();
        }

        public static void KontaktDisplayTest(Kontakt k, string prefix, WebForm currentWebForm)
        {
            LabelTester Fax = new LabelTester(GetCtrlId(prefix,"Fax"), currentWebForm);
            LabelTester Tel = new LabelTester(GetCtrlId(prefix, "Tel"), currentWebForm);
            LabelTester Tel2 = new LabelTester(GetCtrlId(prefix, "Tel2"), currentWebForm);
            LabelTester Email = new LabelTester(GetCtrlId(prefix, "Email"), currentWebForm);
            LabelTester Www = new LabelTester(GetCtrlId(prefix, "Www"), currentWebForm);

            Assert.AreEqual(k.Fax, Fax.Text, "z造 fax");
            Assert.AreEqual(k.Tel, Tel.Text, "z造 telefon");
            Assert.AreEqual(k.Tel2, Tel2.Text, "z造 telefon 2");
            Assert.AreEqual(k.Email, Email.Text, "z造 e-mail");
            Assert.AreEqual(k.Www, Www.Text, "z造 adres www");
        }

        public static void SetData(Kontakt k2i, string prefix, WebForm currentWebForm)
        {
            TextBoxTester Fax = new TextBoxTester(GetCtrlId(prefix,"FaxEdit"), currentWebForm);
            TextBoxTester Tel = new TextBoxTester(GetCtrlId(prefix,"TelEdit"), currentWebForm);
            TextBoxTester Tel2 = new TextBoxTester(GetCtrlId(prefix,"Tel2Edit"), currentWebForm);
            TextBoxTester Email = new TextBoxTester(GetCtrlId(prefix,"EmailEdit"), currentWebForm);
            TextBoxTester Www = new TextBoxTester(GetCtrlId(prefix,"WwwEdit"), currentWebForm);

            Fax.Text = k2i.Fax;
            Tel.Text = k2i.Tel;
            Tel2.Text = k2i.Tel2;
            Email.Text = k2i.Email;
            Www.Text = k2i.Www;
        }

        public static void CompareData(Kontakt saved, Kontakt fromDb)
        {
            Assert.IsTrue((saved != null && fromDb != null) || (saved == null && fromDb == null)
                , "Kontakt zapisany lub z bazy jest nullem, a 2 nie.");

            Assert.AreEqual(saved.Fax, fromDb.Fax, "fax");
            Assert.AreEqual(saved.Tel, fromDb.Tel, "telefon");
            Assert.AreEqual(saved.Tel2, fromDb.Tel2, "telefon 2");
            Assert.AreEqual(saved.Email, fromDb.Email, "e-mail");
            Assert.AreEqual(saved.Www, fromDb.Www, "WWW");
        }

        [Test]
        public override void InsertTest()
        {
            NewBtn.Click();

            Kontakt k2i = Kontakt.GetWellKnown();
            k2i.Tel = UnikalnyNumerTelefonu();

            SetData(k2i, "ctl02", CurrentWebForm);

            SaveBtn.Click();
            Assert.IsTrue(int.Parse(sessionLbl.Text) > 0, "Niezapisano nowego rekordu");

            Kontakt kontaktFromDb = Dm.Retrieve<Kontakt>(int.Parse(sessionLbl.Text));

            CompareData(k2i, kontaktFromDb);

            kontaktFromDb.Delete();
        }

        [Test]
        public override void EditTest()
        {
            Kontakt k = Kontakt.GetWellKnown();
            k.Save(null);
            IdTb.Text = k.Id.ToString();
            PostBtn.Click();

            KontaktDisplayTest(k, "ctl02", CurrentWebForm);

            EditBtn.Click();

            k.Tel = UnikalnyNumerTelefonu();

            SetData(k, "ctl02", CurrentWebForm);

            SaveBtn.Click();
            Kontakt kontaktFromDb = (Kontakt)k.Resync();

            CompareData(k, kontaktFromDb);

            kontaktFromDb.Delete();
        }

        protected override string GetUrl()
        {
            
            if (WindowsIdentity.GetCurrent().Name == @"WIN\Marcel")
            {
                return "http://localhost:1218/Testy/KontaktTest.aspx";
            }
            else
            {
                return "http://localhost/WebStypendia/Testy/KontaktTest.aspx";
            }

        }
    }
}
