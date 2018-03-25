using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Migrations
{
    using Domain;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ReleaseContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ReleaseContext context)
        {
            DOM d1 = new DOM()
            {
                Tag = "strong",
                ClassAttribute = "Title",
                XPath = @"//strong[@class='Title']"
            };
            DOM d2 = new DOM()
            {
                Tag = "span",
                ClassAttribute = "Capi",
                XPath = @"//span[@class='Capi']"
            };
            DOM d3 = new DOM()
            {
                Tag = "a",
                ClassAttribute = "rated_title",
                XPath = @"//a[@class='rated_title']"
            };
            DOM d4 = new DOM()
            {
                Tag = "span",
                ParentTag = "div",
                ParentClassAttribute = "rated_stars",
                ChildPosition = 1,
                XPath = @"//div[@class='rated_stars']/span[1]"
            };
            DOM d5 = new DOM()
            {
                Tag = "a",
                ParentTag = "h3",
                ChildPosition = 1,
                XPath = @"//h3/a"
            };
            DOM d6 = new DOM()
            {
                Tag = "a",
                ParentTag = "h3",
                ChildPosition = 1,
                XPath = @"//h3/a"
            };
            DOM d7 = new DOM()
            {
                XPath = "//a[@title='See all releases for this show']"
            };
            DOM d8 = new DOM()
            {
                XPath = "//td[@class='rls-label']"
            };
            DOM d9 = new DOM()
            {
                XPath = "//a[@class='chapter']"
            };
            DOM d10 = new DOM()
            {
                XPath = "//a[@class='chapter']"
            };
            DOM d12 = new DOM()
            {
                XPath = "//a[@class='forum-recent-post-subject']"
            };
            Source s1 = new Source()
            {
                URL = "https://animeflv.net",
                ReleaseHolder = d1,
                ChapterNumberHolder = d2
            };
            Source s2 = new Source()
            {
                URL = "http://jkanime.net",
                ReleaseHolder = d3,
                ChapterNumberHolder = d4
            };
            Source s3 = new Source()
            {
                URL = "http://www.animerush.tv/",
                ReleaseHolder = d5,
                ChapterNumberHolder = d6
            };
            Source s4 = new Source()
            {
                URL = "http://horriblesubs.info/lib/latest.php",
                ReleaseHolder = d7,
                ChapterNumberHolder = d8
            };
            Source s5 = new Source()
            {
                URL = "http://fanfox.net",
                ReleaseHolder = d9,
                ChapterNumberHolder = d10
            };
            Source s6 = new Source()
            {
                URL = "https://chroniclesofelyria.com/forum",
                ReleaseHolder = d12,
                ChapterNumberHolder = d12,
                Category = "Forum"
            };
            /*context.DOMs.AddOrUpdate(d1);
            context.DOMs.AddOrUpdate(d2);
            context.DOMs.AddOrUpdate(d3);
            context.DOMs.AddOrUpdate(d4);
            context.DOMs.AddOrUpdate(d5);
            context.DOMs.AddOrUpdate(d6);
            context.DOMs.AddOrUpdate(d7);
            context.DOMs.AddOrUpdate(d8);*/
            context.Sources.AddOrUpdate(s1);
            context.Sources.AddOrUpdate(s2);
            context.Sources.AddOrUpdate(s3);
            context.Sources.AddOrUpdate(s4);
            context.Sources.AddOrUpdate(s5);
            context.Sources.AddOrUpdate(s6);
            context.SaveChanges();
        }
    }
}
