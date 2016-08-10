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
        private static readonly object SyncRoot = new object();
        //first parameter - processed pages number
        //second parameter - total pages number
        private static readonly int[] PagesProcessedNumber = new int[2];
        private const int NumberOfPageRequests = 5;
        private const int LimitNumberOfUrls = 15;
        private ErrorLoggingService _errorLoggingService;

        public PerformanceEvaluatorService()
        {
            _errorLoggingService = new ErrorLoggingService();
        }

        /// <summary>
        /// Get two values: number of processed pages and total number of pages
        /// </summary>
        /// <returns></returns>
        public int[] GetPagesProcessedNumber()
        {
            lock (SyncRoot)
            {
                return PagesProcessedNumber;
            }
        }
      
        /// <summary>
        /// Get website entity with pages response data
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Website GetWebsite(string url)
        {
            var website = new Website();

            if (!string.IsNullOrEmpty(url))
            {
                var pageModels = GetPageModels(url);
                var pages = GetPages(pageModels);
                website.Pages = pages;
                website.Url = url;
            }

            return website;
        }

        private List<PageResponseModel> GetPageModels(string urlText)
        {
            var urls = GetUrlsFromPage(urlText);
            urls = GetLimitedNumberOfUrls(urls);
            var pages = new List<PageResponseModel>();

            foreach (var url in urls)
            {
                var responseTimes = GetResponseTimes(url);
                var pageModel = new PageResponseModel()
                {
                    Url = url,
                    ResponseTimes = responseTimes
                };
                pages.Add(pageModel);

                lock (SyncRoot)
                {
                    PagesProcessedNumber[0] = pages.Count;
                    PagesProcessedNumber[1] = urls.Count;
                }
            }

            pages.Sort();

            lock (SyncRoot)
            {
                PagesProcessedNumber[0] = 0;
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

            return urls;
        }

        private List<ResponseTime> GetResponseTimes(string url)
        {
            var responseTimes = new List<ResponseTime>();

            for (int i = 0; i < NumberOfPageRequests; i++)
            {
                var responseTimeSpan = CalculateResponseTime(url);
                var responseTimeInstance = new ResponseTime()
                {
                    Time = responseTimeSpan.Milliseconds
                };
                responseTimes.Add(responseTimeInstance);
            }

            return responseTimes;
        }

        private TimeSpan CalculateResponseTime(string url)
        {
            var responseTime = new TimeSpan();

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                var timer = new Stopwatch();
                timer.Start();
                var response = (HttpWebResponse)request.GetResponse();
                timer.Stop();
                responseTime = timer.Elapsed;
            }
            catch (Exception exception)
            {
                responseTime = GetErrorResponseTime();
                _errorLoggingService.LogError(exception, url);
            }

            return responseTime;
        }

        private TimeSpan GetErrorResponseTime()
        {
            return TimeSpan.FromMilliseconds(-1);
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

        private List<PageResponse> GetPages(List<PageResponseModel> pageModels)
        {
            var pages = new List<PageResponse>();

            foreach (var pageModel in pageModels)
            {
                pages.Add(new PageResponse()
                {
                    Url = pageModel.Url,
                    ResponseTimes = pageModel.ResponseTimes
                });
            }

            return pages;
        }
    }
}