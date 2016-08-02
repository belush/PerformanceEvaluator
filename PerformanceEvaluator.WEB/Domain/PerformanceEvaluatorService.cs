using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using PerformanceEvaluator.DAL.Entities;
using PerformanceEvaluator.WEB.Models;

namespace PerformanceEvaluator.WEB.Domain
{
    public class PerformanceEvaluatorService
    {
        private static object syncRoot = new object();
        private static int[] pagesProcessedNumber = new int[2];

        public List<PageModel> GetPageModels(string urlText)
        {
            var requestsNumber = 5;
            var urls = GetUrlsFromPage(urlText);
            var pages = new List<PageModel>();
            //get only 15 records to see result faster
            urls = GetUrls(urls, 15);

            foreach (var url in urls)
            {
                try
                {
                    var responseTimes = new List<ResponseTime>();

                    for (int i = 0; i < requestsNumber; i++)
                    {
                        var responseTime = GetResponseTime(url);
                        responseTimes.Add(new ResponseTime()
                        {
                            Time = responseTime.Milliseconds
                        });
                    }

                    pages.Add(new PageModel()
                    {
                        Url = url,
                        ResponseTimes = responseTimes
                    });

                    lock (syncRoot)
                    {
                        pagesProcessedNumber[0] = pages.Count;
                        pagesProcessedNumber[1] = urls.Count;
                    }
                    
                }
                catch (Exception) { } //TODO: handle exceptions
            }

            pages.Sort();

            lock (syncRoot)
            {
                pagesProcessedNumber[0] = 0;
            }

            return pages;
        }

        public int[] GetPagesProcessedNumber()
        {
            lock (syncRoot)
            {
                return pagesProcessedNumber;
            }
        }

        public List<PageModel> GetPageModels(List<Page> pages)
        {
            var pageModels = new List<PageModel>();

            foreach (var page in pages)
            {
                pageModels.Add(new PageModel()
                {
                    Url = page.Url,
                    ResponseTimes = page.ResponseTimes.ToList()
                });
            }

            return pageModels;
        }

        public List<Page> GetPages(List<PageModel> pageModels)
        {
            var pages = new List<Page>();

            foreach (var pageModel in pageModels)
            {
                pages.Add(new Page()
                {
                    Url = pageModel.Url,
                    ResponseTimes = pageModel.ResponseTimes
                });
            }

            return pages;
        }

        public Website GetWebsite(string url)
        {
            var pageModels = GetPageModels(url);
            var pages = GetPages(pageModels);
            var website = new Website()
            {
                Pages = pages,
                Url = url
            };

            return website;
        }

        private List<string> GetUrls(List<string> urls, int urlsMaxNumber = 100)
        {
            if (urls.Count < urlsMaxNumber)
            {
                urlsMaxNumber = urls.Count;
            }

            urls = urls.GetRange(0, urlsMaxNumber);
            return urls;
        }

        private List<string> GetUrlsFromPage(string url)
        {
            var urls = new List<string>();

            try
            {
                var web = new HtmlWeb();
                var page = web.Load(url);
                var linkNodes = page.DocumentNode.SelectNodes("//a[@href]");

                foreach (var linkNode in linkNodes)
                {
                    var hrefText = linkNode.Attributes["href"].Value;
                    var absoluteUrl = GetAbsoluteUrl(url, hrefText);

                    if (absoluteUrl.Contains(url))
                    {
                        urls.Add(absoluteUrl);
                    }
                }

                urls = urls.Distinct().ToList();
            }
            catch (Exception) { }

            return urls;
        }

        private TimeSpan GetResponseTime(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            var timer = new Stopwatch();
            timer.Start();
            var response = (HttpWebResponse)request.GetResponse();
            timer.Stop();
            var responseTime = timer.Elapsed;

            return responseTime;
        }

        private string GetAbsoluteUrl(string baseUrl, string url)
        {
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);

            if (!uri.IsAbsoluteUri)
            {
                uri = new Uri(new Uri(baseUrl), uri);
            }

            return uri.ToString();
        }
    }
}