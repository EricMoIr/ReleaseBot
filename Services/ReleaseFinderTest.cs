using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    [TestClass]
    public class ReleaseFinderTest
    {
        //I "should" test GetReleases as a whole, but since it loads a webpage... 
        //it doesn't fit FIRST so it's not a valid unit test
        //and even if it did, I'd eventually get booted by the sources' servers

        [TestMethod]
        public void FindChapterEmptyString()
        {
            Assert.AreEqual("0", ReleaseFinder.FindChapter(""));
        }
        [TestMethod]
        public void FindChapterNullString()
        {
            Assert.AreEqual("0", ReleaseFinder.FindChapter(null));
        }
        [TestMethod]
        public void FindChapterNumbersEverywhere()
        {
            Assert.AreEqual("123", ReleaseFinder.FindChapter("name321episode123"));
        }
        [TestMethod]
        public void FindChapterNoNumbersAtTheEnd()
        {
            Assert.AreEqual("123", ReleaseFinder.FindChapter("name321episode123:"));
        }

        [TestMethod]
        public void FindTitleEmptyString()
        {
            Assert.AreEqual("", ReleaseFinder.FindTitle(""));
        }
        [TestMethod]
        public void FindTitleNullString()
        {
            Assert.AreEqual("", ReleaseFinder.FindTitle(null));
        }
        [TestMethod]
        public void FindTitleNumbersEverywhere()
        {
            Assert.AreEqual("name321", ReleaseFinder.FindTitle("name321episode123"));
        }
        [TestMethod]
        public void FindTitleJustTitle()
        {
            Assert.AreEqual("123", ReleaseFinder.FindTitle("name321"));
        }
    }
}
