using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmericanEagle.Tests
{
    [TestFixture]
    public class DummyTests
    {
        [Test]
        public void DummyTestToLoadAssembliesForCodeCoverage()
        {
            // TODO: Add your test code here
            Type foo;
            foo = typeof(AmericanEagle.SDK.Interceptors.AddMemberInterceptor);
            Assert.IsNotNull(foo);

            foo = typeof(AmericanEagle.bScript.GetMemberAccountDetails);           
            Assert.IsNotNull(foo);
        }
    }
}
