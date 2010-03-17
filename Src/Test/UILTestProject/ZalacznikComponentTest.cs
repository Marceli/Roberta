using System;
using System.Security.Principal;
using NUnit.Extensions.Asp.AspTester;
using NUnit.Framework;
using Vulcan.Stypendia.Bll;

namespace GUITests
{
	[TestFixture]
	public class ZalacznikComponentTest : BaseComponentTest
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
                return "http://localhost:1218/Testy/ZalacznikTest.aspx";
            }
            else
            {
                return "http://localhost/WebStypendia/Testy/ZalacznikTest.aspx";
            }
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
			LabelTester Nazwa = new LabelTester("ctl01_Nazwa", CurrentWebForm);
			LabelTester Opis = new LabelTester("ctl01_Opis", CurrentWebForm);
			Zalacznik z = Zalacznik.GetWellKnown();
			string unNazwa = Guid.NewGuid().ToString();
			string unOpis = Guid.NewGuid().ToString();
			z.Nazwa = unNazwa;
			z.Opis = unOpis;
			z.Save(null);

			IdTb.Text = z.Id.ToString();
			PostBtn.Click();
			AreEqual(z.Id, int.Parse(sessionLbl.Text), "Z³y id");
			AreEqual(z.Nazwa, Nazwa.Text, "z³a nazwa");
			AreEqual(z.Opis, Opis.Text, "z³y opis");									
			z.Delete();
		}

		[Test]
		public override void InsertTest()
		{
			NewBtn.Click();
			TextBoxTester Nazwa = new TextBoxTester("ctl01_NazwaEdit", CurrentWebForm);
			TextBoxTester Opis = new TextBoxTester("ctl01_OpisEdit", CurrentWebForm);
			Zalacznik z2i = Zalacznik.GetWellKnown();
			string unNazwa = Guid.NewGuid().ToString();
			string unOpis = Guid.NewGuid().ToString();
			z2i.Nazwa = unNazwa;
			z2i.Opis = unOpis;
			Nazwa.Text = z2i.Nazwa;
			Opis.Text = z2i.Opis;			
			SaveBtn.Click();
			IsTrue(int.Parse(sessionLbl.Text) > 0, "Niezapisano nowego rekordu");
			Zalacznik zalacznikFromDb = Dm.Retrieve<Zalacznik>(int.Parse(sessionLbl.Text));			
			AreEqual(z2i.Nazwa, zalacznikFromDb.Nazwa, "nazwa");
			AreEqual(z2i.Opis, zalacznikFromDb.Opis, "opis");
			zalacznikFromDb.Delete();
		}
		[Test]
		public override void EditTest()
		{
			LabelTester Nazwa = new LabelTester("ctl01_Nazwa", CurrentWebForm);
			LabelTester Opis = new LabelTester("ctl01_Opis", CurrentWebForm);
			Zalacznik z = Zalacznik.GetWellKnown();			
			z.Save(null);
			IdTb.Text = z.Id.ToString();
			PostBtn.Click();
			AreEqual(z.Nazwa, Nazwa.Text, "nazwa");
			AreEqual(z.Opis, Opis.Text, "opis");

			EditBtn.Click();
			TextBoxTester NazwaEdit = new TextBoxTester("ctl01_NazwaEdit", CurrentWebForm);
			TextBoxTester OpisEdit = new TextBoxTester("ctl01_OpisEdit", CurrentWebForm);

			string unNazwa = Guid.NewGuid().ToString();
			string unOpis = Guid.NewGuid().ToString();
			NazwaEdit.Text = unNazwa;
			OpisEdit.Text = unOpis;
			SaveBtn.Click();
			Zalacznik zalacznikFromDb = (Zalacznik)z.Resync();
			AreEqual(unNazwa, zalacznikFromDb.Nazwa, "Nazwa");
			AreEqual(unOpis, zalacznikFromDb.Opis, "Opis");
			zalacznikFromDb.Delete();
		}
	}
}
