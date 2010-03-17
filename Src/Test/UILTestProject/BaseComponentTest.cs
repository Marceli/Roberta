using NUnit.Extensions.Asp;
using NUnit.Framework;

namespace GUITests
{       
    public abstract class BaseComponentTest : WebFormTestCase
    {
        protected abstract string GetUrl();
       
        protected override void SetUp()
        {
            Browser.GetPage(GetUrl());
        }

        [Test]
        public void BasicConnection()
        {
            CheckPage();
        }
        
		public void CheckPage()
        {
            AreEqual(GetUrl(), Browser.CurrentUrl.ToString(), "Expected page does not match actual page. Expected=[" + GetUrl() + "]. Actual=[" + Browser.CurrentUrl.ToString() + "].");
        }

        
        public abstract void DisplayTest();
        
		
        public abstract void InsertTest();
        
		
        public abstract void EditTest();

        protected static string GetCtrlId(string prefix, string name)
        {
            return string.Format("{0}_{1}", prefix, name);
        }                
    }
}
