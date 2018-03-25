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
            Source s1 = new Source()
            {
                URL = "https://animeflv.net",
                ReleaseHolder = @"//strong[@class='Title']",
                ChapterNumberHolder = @"//span[@class='Capi']",
                Category = "Anime"
            };
            Source s2 = new Source()
            {
                URL = "http://jkanime.net",
                ReleaseHolder = @"//a[@class='rated_title']",
                ChapterNumberHolder = @"//div[@class='rated_stars']/span[1]",
                Category = "Anime"
            };
            Source s3 = new Source()
            {
                URL = "http://www.animerush.tv/",
                ReleaseHolder = @"//h3/a",
                ChapterNumberHolder = @"//h3/a",
                Category = "Anime"
            };
            Source s4 = new Source()
            {
                URL = "http://horriblesubs.info/lib/latest.php",
                ReleaseHolder = "//a[@title='See all releases for this show']",
                ChapterNumberHolder = "//td[@class='rls-label']",
                Category = "Anime"
            };
            Source s5 = new Source()
            {
                URL = "http://fanfox.net",
                ReleaseHolder = "//a[@class='chapter']",
                ChapterNumberHolder = "//a[@class='chapter']",
                Category = "Manga"
            };
            Source s6 = new Source()
            {
                URL = "https://chroniclesofelyria.com/forum",
                ReleaseHolder = "//a[@class='forum-recent-post-subject']",
                ChapterNumberHolder = "//a[@class='forum-recent-post-subject']",
                DateTimeHolder = "//span[@class='forum-recent-post-timestamp timestamp']",
                Category = "Forum"
            };
            Source s7 = new Source()
            {
                URL = "http://lhtranslation.net/",
                ReleaseHolder = "//h2[@class='title']",
                ChapterNumberHolder = "//h2[@class='title']",
                Category = "Manga"
            };
            /*
            context.DOMs.AddOrUpdate(d1);
            context.DOMs.AddOrUpdate(d2);
            context.DOMs.AddOrUpdate(d3);
            context.DOMs.AddOrUpdate(d4);
            context.DOMs.AddOrUpdate(d5);
            context.DOMs.AddOrUpdate(d6);
            context.DOMs.AddOrUpdate(d7);
            context.DOMs.AddOrUpdate(d8);
            */
            context.Sources.AddOrUpdate(s1);
            context.Sources.AddOrUpdate(s2);
            context.Sources.AddOrUpdate(s3);
            context.Sources.AddOrUpdate(s4);
            context.Sources.AddOrUpdate(s5);
            context.Sources.AddOrUpdate(s6);
            context.Sources.AddOrUpdate(s7);
            context.SaveChanges();
        }
    }
}
