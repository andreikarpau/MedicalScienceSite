using System.Collections.Generic;
using BTTechnologies.MedScience.MVC.BTTClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace MedScienceUnitTests.MVCHelpersTests
{
    [TestClass]
    public class BTTSimplePagerModelTest
    {
        [TestMethod]
        public void TestPagerWithOnePage()
        {
            BTTSimplePagerModel model = new BTTSimplePagerModel(1, 1, GetTempUrl);
            int? firstPage;
            int? lastPage;
            IDictionary<int, string> results = model.GetPagesNumbers(out firstPage, out lastPage);

            Assert.AreEqual(results.Count, 1);
            Assert.AreEqual(firstPage, null);
            Assert.AreEqual(lastPage, null);
            TestResultsDictionary(1, results);
        }

        [TestMethod]
        public void TestPagerWithSecondPageSelected()
        {
            BTTSimplePagerModel model = new BTTSimplePagerModel(2, 6, GetTempUrl);
            int? firstPage;
            int? lastPage;
            IDictionary<int, string> results = model.GetPagesNumbers(out firstPage, out lastPage);

            Assert.AreEqual(results.Count, 5);
            Assert.AreEqual(firstPage, null);
            Assert.AreEqual(lastPage, 6);
            TestResultsDictionary(1, results);
        }

        [TestMethod]
        public void TestPagerSelectedPageInMiddleSelected()
        {
            BTTSimplePagerModel model = new BTTSimplePagerModel(6, 15, GetTempUrl);
            int? firstPage;
            int? lastPage;
            IDictionary<int, string> results = model.GetPagesNumbers(out firstPage, out lastPage);

            Assert.AreEqual(results.Count, 7);
            Assert.AreEqual(firstPage, 1);
            Assert.AreEqual(lastPage, 15);
            TestResultsDictionary(3, results);
        }

        [TestMethod]
        public void TestPagerSelectedPageOneBeforeLastSelected()
        {
            BTTSimplePagerModel model = new BTTSimplePagerModel(14, 15, GetTempUrl);
            int? firstPage;
            int? lastPage;
            IDictionary<int, string> results = model.GetPagesNumbers(out firstPage, out lastPage);

            Assert.AreEqual(results.Count, 5);
            Assert.AreEqual(firstPage, 1);
            Assert.AreEqual(lastPage, null);
            TestResultsDictionary(11, results);
        }
        
        private void TestResultsDictionary(int firstPage, IEnumerable<KeyValuePair<int, string>> results)
        {
            int index = firstPage;
            foreach (KeyValuePair<int, string> keyValuePair in results.OrderBy(i => i.Key))
            {
                Assert.AreEqual(keyValuePair.Key, index);
                Assert.AreEqual(keyValuePair.Value, GetTempUrl(index));
                index++;
            }
        }

        private string GetTempUrl(int i)
        {
            return string.Format("www.a1/{0}", i);
        }
    }
}
