/*
using System;
using NUnit.Framework;
using NUnit.Extensions.Asp;
using NUnit.Extensions.Asp.AspTester;
using Vulcan.Stypendia.Bll;
using Wilson.ORMapper;



namespace GUITests
{
	[TestFixture]
	public class WizardTest : WebFormTestCase
	{
        private UserControlTester Wizard;
        private UserControlTester StartNavigation;
        private UserControlTester StepNavigation;
        private UserControlTester AdresView1;
        private UserControlTester DetailsView1;
        private UserControlTester AdresView2;
        private UserControlTester DetailsView2;
        private UserControlTester FinishNavigation;
        private LinkButtonTester FirstPageLink;
        
        private ButtonTester StartNextButton;
        private ButtonTester StepNextButton;
        private ButtonTester FinishButton;
        private ButtonTester Prev2Button;
        private ButtonTester StepPreviousButton;
        
        private TextBoxTester TextBox1;
        private TextBoxTester TextBox2;
        private TextBoxTester Powiat1;
        private TextBoxTester Kod1;
        private TextBoxTester Gus1;
        private TextBoxTester Kraj1;
        private TextBoxTester Ulica1;
        private TextBoxTester Gmina1;
        private TextBoxTester Wojewodztwo1;
        private TextBoxTester Poczta1;
        private TextBoxTester Miejscowosc1;

        private TextBoxTester Powiat2;
        private TextBoxTester Kod2;
        private TextBoxTester Gus2;
        private TextBoxTester Kraj2;
        private TextBoxTester Ulica2;
        private TextBoxTester Gmina2;
        private TextBoxTester Wojewodztwo2;
        private TextBoxTester Poczta2;
        private TextBoxTester Miejscowosc2;
        private const string URL = "http://localhost/WebStypendia/Testy/AdresTransactionWizardTest.aspx";
       

        protected override void SetUp()
        {
            //page 1
            Wizard = new UserControlTester("Wizard1", CurrentWebForm);
            StartNavigation=new UserControlTester("StartNavigationTemplateContainerID",Wizard);
            StartNextButton=new ButtonTester("StartNextButton", StartNavigation);
            TextBox1 = new TextBoxTester("TextBox1", Wizard);
            TextBox2 = new TextBoxTester("TextBox2", Wizard);

            //page2
            StepNavigation=new UserControlTester("StepNavigationTemplateContainerID", Wizard);
            AdresView1=new UserControlTester("AdresView1", Wizard);
            DetailsView1=new UserControlTester("DetailsView", AdresView1);
            StepNextButton=new ButtonTester("StepNextButton", StepNavigation);
            StepPreviousButton = new ButtonTester("StepPreviousButton", StepNavigation);
            Powiat1=new TextBoxTester("Powiat",DetailsView1);
            Poczta1 = new TextBoxTester("Poczta", DetailsView1);
            Kod1=new TextBoxTester("Kod",DetailsView1);
            Gus1=new TextBoxTester("Gus",DetailsView1);
            Kraj1=new TextBoxTester("Kraj",DetailsView1);
            Ulica1=new TextBoxTester("Ulica",DetailsView1);
            Gmina1=new TextBoxTester("Gmina",DetailsView1);
            Wojewodztwo1=new TextBoxTester("Wojewodztwo",DetailsView1);
            Miejscowosc1 = new TextBoxTester("Miejscowosc", DetailsView1);

            AdresView2 = new UserControlTester("AdresView2", Wizard);
            DetailsView2 = new UserControlTester("DetailsView", AdresView2);
            Poczta2=new TextBoxTester("Poczta",DetailsView2);
            Miejscowosc2=new TextBoxTester("Miejscowosc",DetailsView2);
            Powiat2 = new TextBoxTester("Powiat", DetailsView2);
            Kod2 = new TextBoxTester("Kod", DetailsView2);
            Gus2 = new TextBoxTester("Gus", DetailsView2);
            Kraj2 = new TextBoxTester("Kraj", DetailsView2);
            Ulica2 = new TextBoxTester("Ulica", DetailsView2);
            Gmina2 = new TextBoxTester("Gmina", DetailsView2);
            Wojewodztwo2 = new TextBoxTester("Wojewodztwo", DetailsView2);
            
            
    

            //page3
            FinishNavigation=new UserControlTester("FinishNavigationTemplateContainerID",Wizard);
            FinishButton=new ButtonTester("FinishButton", FinishNavigation);
            Prev2Button = new ButtonTester("FinishPreviousButton", FinishNavigation);
 
            FirstPageLink = new LinkButtonTester("Wizard1_SideBarContainer_SideBarList_ctl00_SideBarButton", CurrentWebForm);
           
           

            Browser.GetPage(URL);
        }

        [Test]
        public void BasicConnection()
        {
            CheckPage();
        }
        [Test]
        public void InsertTest()
        {
            StartNextButton.Click();
            Assert.AreEqual("", Poczta1.Text);
            Poczta1.Text = "Poczta";
            Powiat1.Text="Powiat";
            Kod1.Text="Kod";
            Gus1.Text="Gus";
            Kraj1.Text="Kraj";
            Ulica1.Text="Ulica";
            Gmina1.Text="Gmina";
            Wojewodztwo1.Text="Województwo";
            Poczta1.Text="Poczta";
            string uniqueMiejscowosc1 = Guid.NewGuid().ToString();

            Miejscowosc1.Text = uniqueMiejscowosc1;
            StepNextButton.Click();
            Poczta2.Text = "Poczta2";
            Powiat2.Text = "Powiat2";
            Kod2.Text = "Kod2";
            Gus2.Text = "Gus2";
            Kraj2.Text = "Kraj2";
            Ulica2.Text = "Ulica2";
            Gmina2.Text = "Gmina2";
            Wojewodztwo2.Text = "Województwo2";
            Poczta2.Text = "Poczta2";
            string uniqueMiejscowosc2 = Guid.NewGuid().ToString();
            Miejscowosc2.Text = uniqueMiejscowosc2;
            FinishButton.Click();
            Prev2Button.Click();
            StepPreviousButton.Click();
            int id1 = int.Parse(TextBox1.Text);
            int id2 = int.Parse(TextBox2.Text);
            Adres adres1 = Adres.Retrieve(id1);
            Assert.AreEqual(uniqueMiejscowosc1, adres1.Miejscowosc,"z³a miejscowoœæ w pierwszym adresie");
            Adres adres2 = Adres.Retrieve(id2);
            Assert.AreEqual(uniqueMiejscowosc2, adres2.Miejscowosc, "z³a miejscowoœæ w drugim adresie");
            adres1.Delete();
            adres2.Delete();
        }
        public void CheckPage()
        {
            Assert.AreEqual(URL,Browser.CurrentUrl.ToString(), "Expected page does not match actual page. Expected=[" + URL + "]. Actual=[" + Browser.CurrentUrl.ToString() + "].");

        } 
        
			
	}
}

*/
