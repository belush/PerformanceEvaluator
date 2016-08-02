using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PerformanceEvaluator.DAL.Entities;

namespace PerformanceEvaluator.WEB.Models
{
    public class PageModel : IComparable<PageModel>
    {
        public string Url { get; set; }
        public List<ResponseTime> ResponseTimes { get; set; }

        public int MidResponseTime
        {
            get
            {
                return (int)ResponseTimes.Average(r => r.Time);
            }
        }

        public int MinResponseTime {
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

        public int CompareTo(PageModel other)
        {
            return other.MidResponseTime.CompareTo(MidResponseTime);
        }
    }
}