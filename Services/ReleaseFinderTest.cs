using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Persistence.Domain;
using System;
using System.Collections.Generic;
using System.IO;
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
        public void FindDetailsSameParent()
        {
            string text = File.ReadAllText(Directory.GetCurrentDirectory() + @"\TestWeb1.html");
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(text);
            string xpathRelease = @"//div[@class='parent']/div[1]/div[1]/div[1]";
            string xpathEpisode = @"//div[@class='parent']/div[1]/div[1]/div[1]";
            HtmlNode singleRelease = htmlDoc.DocumentNode.SelectNodes(xpathRelease)[0];
            HtmlNode chapterNumber = htmlDoc.DocumentNode.SelectNodes(xpathEpisode)[0];

            string[] details = new string[] { "Name", "12" };
            string[] result = ReleaseFinder.FindDetails(singleRelease, chapterNumber);
            CollectionAssert.AreEqual(details, result);
        }
        [TestMethod]
        public void FindDetailsDifferentParent()
        {
            string text = File.ReadAllText(Directory.GetCurrentDirectory() + @"\TestWeb2.html");
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(text);
            string xpathRelease = @"//div[@class='parent']/div[1]/div[1]/div[1]";
            string xpathEpisode = @"//div[@class='parent']/div[1]/div[1]/div[2]";
            HtmlNode singleRelease = htmlDoc.DocumentNode.SelectNodes(xpathRelease)[0];
            HtmlNode chapterNumber = htmlDoc.DocumentNode.SelectNodes(xpathEpisode)[0];

            string[] details = new string[] { "Name", "12" };
            string[] result = ReleaseFinder.FindDetails(singleRelease, chapterNumber);
            CollectionAssert.AreEqual(details, result);
        }
        [TestMethod]
        public void FindDetailsSameParentWithDivider()
        {
            string text = File.ReadAllText(Directory.GetCurrentDirectory() + @"\TestWeb3.html");
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(text);
            string xpathRelease = @"//div[@class='parent']/div[1]/div[1]/div[1]";
            string xpathEpisode = @"//div[@class='parent']/div[1]/div[1]/div[1]";
            HtmlNode singleRelease = htmlDoc.DocumentNode.SelectNodes(xpathRelease)[0];
            HtmlNode chapterNumber = htmlDoc.DocumentNode.SelectNodes(xpathEpisode)[0];

            string[] details = new string[] { "Name", "12" };
            string[] result = ReleaseFinder.FindDetails(singleRelease, chapterNumber);
            CollectionAssert.AreEqual(details, result);
        }
        [TestMethod]
        public void FindDetailsSameParentWithDividerTitleHasNumbers()
        {
            string text = File.ReadAllText(Directory.GetCurrentDirectory() + @"\TestWeb4.html");
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(text);
            string xpathRelease = @"//div[@class='parent']/div[1]/div[1]/div[1]";
            string xpathEpisode = @"//div[@class='parent']/div[1]/div[1]/div[1]";
            HtmlNode singleRelease = htmlDoc.DocumentNode.SelectNodes(xpathRelease)[0];
            HtmlNode chapterNumber = htmlDoc.DocumentNode.SelectNodes(xpathEpisode)[0];

            string[] details = new string[] { "Name1", "12" };
            string[] result = ReleaseFinder.FindDetails(singleRelease, chapterNumber);
            CollectionAssert.AreEqual(details, result);
        }
        [TestMethod]
        public void FindDetailsSameParentChapterHasDecimals()
        {
            string text = File.ReadAllText(Directory.GetCurrentDirectory() + @"\TestWeb5.html");
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(text);
            string xpathRelease = @"//div[@class='parent']/div[1]/div[1]/div[1]";
            string xpathEpisode = @"//div[@class='parent']/div[1]/div[1]/div[1]";
            HtmlNode singleRelease = htmlDoc.DocumentNode.SelectNodes(xpathRelease)[0];
            HtmlNode chapterNumber = htmlDoc.DocumentNode.SelectNodes(xpathEpisode)[0];

            string[] details = new string[] { "Name", "1.2" };
            string[] result = ReleaseFinder.FindDetails(singleRelease, chapterNumber);
            CollectionAssert.AreEqual(details, result);
        }
        [TestMethod]
        public void FindDetailsDifferentParentChapterHasDecimals()
        {
            string text = File.ReadAllText(Directory.GetCurrentDirectory() + @"\TestWeb6.html");
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(text);
            string xpathRelease = @"//div[@class='parent']/div[1]/div[1]/div[1]";
            string xpathEpisode = @"//div[@class='parent']/div[1]/div[1]/div[2]";
            HtmlNode singleRelease = htmlDoc.DocumentNode.SelectNodes(xpathRelease)[0];
            HtmlNode chapterNumber = htmlDoc.DocumentNode.SelectNodes(xpathEpisode)[0];

            string[] details = new string[] { "Name", "1.2" };
            string[] result = ReleaseFinder.FindDetails(singleRelease, chapterNumber);
            CollectionAssert.AreEqual(details, result);
        }
        [TestMethod]
        public void FindDetailsSameParentWithDividerChapterHasDecimals()
        {
            string text = File.ReadAllText(Directory.GetCurrentDirectory() + @"\TestWeb7.html");
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(text);
            string xpathRelease = @"//div[@class='parent']/div[1]/div[1]/div[1]";
            string xpathEpisode = @"//div[@class='parent']/div[1]/div[1]/div[1]";
            HtmlNode singleRelease = htmlDoc.DocumentNode.SelectNodes(xpathRelease)[0];
            HtmlNode chapterNumber = htmlDoc.DocumentNode.SelectNodes(xpathEpisode)[0];

            string[] details = new string[] { "Name", "1.2" };
            string[] result = ReleaseFinder.FindDetails(singleRelease, chapterNumber);
            CollectionAssert.AreEqual(details, result);
        }
        [TestMethod]
        public void FindDetailsSameParentWithDividerTitleHasNumbersChapterHasDecimals()
        {
            string text = File.ReadAllText(Directory.GetCurrentDirectory() + @"\TestWeb8.html");
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(text);
            string xpathRelease = @"//div[@class='parent']/div[1]/div[1]/div[1]";
            string xpathEpisode = @"//div[@class='parent']/div[1]/div[1]/div[1]";
            HtmlNode singleRelease = htmlDoc.DocumentNode.SelectNodes(xpathRelease)[0];
            HtmlNode chapterNumber = htmlDoc.DocumentNode.SelectNodes(xpathEpisode)[0];

            string[] details = new string[] { "Name1", "1.2" };
            string[] result = ReleaseFinder.FindDetails(singleRelease, chapterNumber);
            CollectionAssert.AreEqual(details, result);
        }
        [TestMethod]
        public void FindDetailsSameParentMovie()
        {
            string text = File.ReadAllText(Directory.GetCurrentDirectory() + @"\TestWeb9.html");
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(text);
            string xpathRelease = @"//div[@class='parent']/div[1]/div[1]/div[1]";
            string xpathEpisode = @"//div[@class='parent']/div[1]/div[1]/div[1]";
            HtmlNode singleRelease = htmlDoc.DocumentNode.SelectNodes(xpathRelease)[0];
            HtmlNode chapterNumber = htmlDoc.DocumentNode.SelectNodes(xpathEpisode)[0];

            string[] details = new string[] { "Movie Name", "0" };
            string[] result = ReleaseFinder.FindDetails(singleRelease, chapterNumber);
            CollectionAssert.AreEqual(details, result);
        }
    }
}
