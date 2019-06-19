using System;
using System.Collections.Generic;
using System.Xml;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.XA.Feature.SiteMetadata.Sitemap;

namespace Sitecore.Support.XA.Feature.SiteMetadata.Sitemap
{
    public class SitemapGenerator : Sitecore.XA.Feature.SiteMetadata.Sitemap.SitemapGenerator
    {
        public SitemapGenerator() : base()
        {
        }

        public SitemapGenerator(XmlWriterSettings xmlWriterSettings) : base(xmlWriterSettings)
        {
        }

        protected override string GetFullLink(Item item, SitemapLinkOptions options)
        {
            Uri uri = null;
            string relativeUrl = LinkManager.GetItemUrl(item, options.UrlOptions);
            if (!relativeUrl.StartsWith("/", StringComparison.Ordinal))
            {
                uri = new Uri(relativeUrl);
                relativeUrl = uri.LocalPath;
            }
            return options.Scheme + Uri.SchemeDelimiter + options.TargetHostname + relativeUrl;
        }

        protected override IList<Item> ChildrenSearch(Item homeItem)
        {
            List<Item> results = new List<Item>();
            Queue<Item> searchQueue = new Queue<Item>();

            if (homeItem.HasChildren)
            {
                searchQueue.Enqueue(homeItem);
                if (homeItem.Versions.Count > 0)
                {
                    results.Add(homeItem);
                }

                while (searchQueue.Count != 0)
                {
                    Item current = searchQueue.Dequeue();
                    foreach (Item child in current.Children)
                    {
                        if (!results.Contains(child))
                        {
                            if (!ShouldBeSkipped(child))
                            {
                                //if item has no layout, then only it's children are checked
                                if (child.Versions.Count > 0)
                                {
                                    results.Add(child);
                                }
                            }

                            if (child.HasChildren)
                            {
                                searchQueue.Enqueue(child);
                            }
                        }
                    }
                }
            }

            return results;
        }
    }
}