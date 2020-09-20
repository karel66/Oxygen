using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Oxygen;

namespace UnitTests
{
    [TestClass]
    public class Demo : Oxygen.Flow
    {
        [TestMethod]
        public void GoogleSearchTest()
        {
           var result = 
                CreateContext(BrowserBrand.FireFox, new Uri("https://www.google.com/"))
                | Fill("input[name=q]","OxygenFlow")
                | Click("input[type=submit]")
                ;

            Assert.IsFalse(result.HasProblem, result.ProblemCause.ToString());
        }
    }
}
