using System;
using System.Security.Principal;
using NUnit.Extensions.Asp.AspTester;
using NUnit.Framework;
using Vulcan.Stypendia.Bll;


namespace GUITests
{
    [TestFixture]
    public class WniosekListTest : BaseComponentTest
    {
        private MyLinkTester first;
        private MyLinkTester last;
        private MyLinkTester prev;
        private MyLinkTester next;
        private LabelTester currentPage;
        private TextBoxTester filterValue;
        private ButtonTester button;
        private LabelTester itemsCount;
        private DropDownListTester filterField;
        
       
        
       
        protected override string GetUrl()
        {

            if (WindowsIdentity.GetCurrent().Name == @"WIN\Marcel")
            {
                return "http://localhost:1218/Testy/WniosekListTest.aspx";
            }
            else
            {
                return "http://localhost/WebStypendia/Testy/WniosekListTest.aspx";
            }
        }
        protected override void  TearDown()
        {
            Dm.DeleteSql<Wniosek>("Sygnatura like 'SS%'");
 	        base.TearDown();
        }
       
        protected override void SetUp()
        {
            button=new ButtonTester("WniosekList1_SearchBtn", CurrentWebForm);
            filterValue = new TextBoxTester("WniosekList1_ValueTxt", CurrentWebForm);
            next = new MyLinkTester("Next", CurrentWebForm);
            prev = new MyLinkTester("Prev", CurrentWebForm);
            last = new MyLinkTester("Last", CurrentWebForm);
            first = new MyLinkTester("First", CurrentWebForm);
            itemsCount = new LabelTester("ItemsCount", CurrentWebForm);
            currentPage = new LabelTester("CurrentPage", CurrentWebForm);
            filterField = new DropDownListTester("WniosekList1_FilterList", CurrentWebForm);
            Dm.DeleteSql<Wniosek>("Sygnatura like 'SS%'");
            Wniosek w1;
            for (int i = 0; i < 80; i++)
            {
                w1 = Wniosek.GetWellKnown("olo", DateTime.Now);
                w1.Sygnatura = "SS" + i.ToString().PadLeft(3, '0');
                w1.Wnioskodawca.Pesel = "WP" + i.ToString().PadLeft(3, '0');
                w1.Wnioskodawca.Nazwisko = "WN" + i.ToString().PadLeft(3, '0');
                w1.Stypendysta.Pesel = "SP" + i.ToString().PadLeft(3, '0');
                w1.Stypendysta.Nazwisko = "SN" + i.ToString().PadLeft(3, '0');

                w1.Save(null);
            }
            base.SetUp();
        }



        
        public override void DisplayTest()
        {
             

        }
        [Test]
        public void NavigationTest()
        {
            filterValue.Text = "SS";
            button.Click();
            Assert.AreEqual("Strona 1 z 4", currentPage.Text);
            next.Click();
            Assert.AreEqual("Strona 2 z 4", currentPage.Text);
            next.Click();
            Assert.AreEqual("Strona 3 z 4", currentPage.Text);
            prev.Click();
            Assert.AreEqual("Strona 2 z 4", currentPage.Text);
            last.Click();
            Assert.AreEqual("Strona 4 z 4", currentPage.Text);
            first.Click();
            Assert.AreEqual("Strona 1 z 4", currentPage.Text);
        }
        [Test]
        public void FilterTest()
        {
            filterValue.Text = "SS";
            button.Click();
            Assert.AreEqual("80",itemsCount.Text);
            filterValue.Text = "SS067";
            button.Click();
            Assert.AreEqual("1", itemsCount.Text);
            filterValue.Text = "SS06";
            button.Click();
            Assert.AreEqual("10", itemsCount.Text);
  
        }
        [Test]
        public void DropDownTest()
        {
            filterValue.Text = "WP";
            filterField.SelectedValue = "WnioskodawcaPesel";
            button.Click();
            Assert.AreEqual("80", itemsCount.Text);
            filterValue.Text = "SN04";
            filterField.SelectedValue = "StypendystaNazwisko";
            button.Click();
            Assert.AreEqual("10", itemsCount.Text);
        }

       
        public override void InsertTest()
        {

        }
       
        public override void EditTest()
        {

        }
    }
}
