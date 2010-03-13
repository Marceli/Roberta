using System;
using System.Security.Principal;
using NUnit.Extensions.Asp.AspTester;
using NUnit.Framework;
using Vulcan.Stypendia.Bll;

namespace GUITests
{
    [TestFixture]
    public class LokalizacjaListTest : BaseComponentTest
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
                return "http://localhost/WebStypendia/Testy/AdmLokalizacjaListTest.aspx";
            }
        }

        protected override void TearDown()
        {
            Dm.DeleteSql<Lokalizacja>("Sygnatura like 'SSS%'");
 	        base.TearDown();
        }
       
        protected override void SetUp()
        {
            button = new ButtonTester("LokalizacjaGrid_SearchBtn", CurrentWebForm);
            filterValue = new TextBoxTester("LokalizacjaGrid_ValueTxt", CurrentWebForm);
            next = new MyLinkTester("Next", CurrentWebForm);
            prev = new MyLinkTester("Prev", CurrentWebForm);
            last = new MyLinkTester("Last", CurrentWebForm);
            first = new MyLinkTester("First", CurrentWebForm);
//            itemsCount = new LabelTester("ItemsCount", CurrentWebForm);
            currentPage = new LabelTester("CurrentPage", CurrentWebForm);
            filterField = new DropDownListTester("LokalizacjaGrid_FilterList", CurrentWebForm);
            
            Dm.DeleteSql<Lokalizacja>("Nazwa like 'SSS%'");
            Lokalizacja l1;
            for (int i = 0; i < 80; i++)
            {
                l1 = Lokalizacja.GetWellKnown();
                l1.Nazwa = "SSS" + i.ToString().PadLeft(3, '0');
                l1.Skrot = "LP" + i.ToString().PadLeft(3, '0');

                l1.Save(null);
            }
 
            base.SetUp();
        }
       
        public override void DisplayTest()
        {            
        }

        [Test]
        public void NavigationTest()
        {
            filterValue.Text = "SSS";
            button.Click();
            Assert.AreEqual("Strona 1 z 9", currentPage.Text);
            next.Click();
            Assert.AreEqual("Strona 2 z 9", currentPage.Text);
            next.Click();
            Assert.AreEqual("Strona 3 z 9", currentPage.Text);
            prev.Click();
            Assert.AreEqual("Strona 2 z 9", currentPage.Text);
            last.Click();
            Assert.AreEqual("Strona 4 z 9", currentPage.Text);
            first.Click();
            Assert.AreEqual("Strona 1 z 9", currentPage.Text);
        }
/*
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
*/
       
        public override void InsertTest()
        {

        }
       
        public override void EditTest()
        {

        } 
    }
}
