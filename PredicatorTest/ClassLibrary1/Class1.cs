using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace ClassLibrary1
{
    [TestFixture]
    public class Class1
    {
        Predicate<string> StringChecker = new Predicate<string>(delegate(string target) { return target.Length > 0; });
        Predicate<string> IntChecker = new Predicate<string>(delegate(string target) { return target == "0"; });
        [Test]
        public void Basic()
        {
            Assert.That(StringChecker("olo"));
        }
        [Test]
        public void Composite()
        {
            var predicates = new List<Predicate<string>> ()
                                 {
                                     StringChecker,
                                     IntChecker
                                 };
            ;
            Assert.That(predicates.FindAll(p => p.Invoke("olo")),NUnit.Framework.SyntaxHelpers.Has.Count(1));
        }
    }
}
