using System;
using System.Collections.Generic;
using System.Linq;
using PerformanceEvaluator.DAL.Entities;

namespace PerformanceEvaluator.WEB.Models
{
    public class PageResponseModel : IComparable<PageResponseModel>
    {
        public string Url { get; set; }

        public List<ResponseTime> ResponseTimes { get; set; }

        // average response time
        public int MidResponseTime
        {
            get
            {
                return (int)ResponseTimes.Average(r => r.Time);
            }
        }

        public int MinResponseTime
        {
            get
            {
                return ResponseTimes.Min(r => r.Time);
            }
        }

        public int MaxResponseTime
        {
            get
            {
                return ResponseTimes.Max(r => r.Time);
            }
        }

        public int CompareTo(PageResponseModel other)
        {
            return other.MidResponseTime.CompareTo(MidResponseTime);
        }
    }
}