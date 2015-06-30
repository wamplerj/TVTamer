using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace TvTamer.Core.UnitTests
{
    [TestFixture]
    public class FailingTest
    {
        [Test]
        public void FailMofo()
        {
            Assert.IsTrue(false);
        }
    }
}
