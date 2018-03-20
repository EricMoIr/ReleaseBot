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
                ReleaseHolder = new DOM()
                {
                    Tag = "strong",
                    ClassAttribute = "Title"
                },
                ChapterNumberHolder = new DOM()
                {
                    Tag = "span",
                    ClassAttribute = "Capi"
                }
            };
            Source s2 = new Source()
            {
                URL = "http://jkanime.net",
                ReleaseHolder = new DOM()
                {
                    Tag = "a",
                    ClassAttribute = "rated_title"
                },
                ChapterNumberHolder = new DOM()
                {
                    Tag = "span",
                    ParentTag = "div",
                    ParentClassAttribute = "rated_stars",
                    ChildPosition = 1
                }
            };
            Source s3 = new Source()
            {
                URL = "http://www.animerush.tv/",
                ReleaseHolder = new DOM()
                {
                    Tag = "a",
                    ParentTag = "h3",
                    ChildPosition = 1
                },
                ChapterNumberHolder = new DOM()
                {
                    Tag = "a",
                    ParentTag = "h3",
                    ChildPosition = 1
                }
            };
            context.Sources.AddOrUpdate(s1);
            context.Sources.AddOrUpdate(s2);
            context.Sources.AddOrUpdate(s3);
        }
    }
}
