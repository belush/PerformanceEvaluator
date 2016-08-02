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
        private static readonly object syncRoot = new object();
        private static readonly int[] pagesProcessedNumber = new int[2];
        private const int NumberOfPageRequests = 5;
        private const int LimitNumberOfUrls = 15;

        //TODO: implement Automapper
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

        /// <summary>
        /// Get two values: number of processed pages and total number of pages
        /// </summary>
        /// <returns></returns>
        public int[] GetPagesProcessedNumber()
        {
            lock (syncRoot)
            {
                return pagesProcessedNumber;
            }
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

        /// <summary>
        /// Get website entity with pages response data
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
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

        private List<PageModel> GetPageModels(string urlText)
        {
            var urls = GetUrlsFromPage(urlText);
            urls = GetLimitedNumberOfUrls(urls);
            var pages = new List<PageModel>();

            foreach (var url in urls)
            {
                try
                {
                    var responseTimes = GetResponseTimes(url);
                    var pageModel = new PageModel()
                    {
                        Url = url,
                        ResponseTimes = responseTimes
                    };
                    pages.Add(pageModel);

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

        private List<string> GetLimitedNumberOfUrls(List<string> urls)
        {
            var limitNumberOfUrls = LimitNumberOfUrls;

            if (urls.Count < limitNumberOfUrls)
            {
                limitNumberOfUrls = urls.Count;
            }

            urls = urls.GetRange(0, limitNumberOfUrls);
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

        private List<ResponseTime> GetResponseTimes(string url)
        {
            var responseTimes = new List<ResponseTime>();

            for (int i = 0; i < NumberOfPageRequests; i++)
            {
                var responseTime = GetResponseTime(url);
                var responseTimeInstance = new ResponseTime()
                {
                    Time = responseTime.Milliseconds
                };
                responseTimes.Add(responseTimeInstance);
            }

            return responseTimes;
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