using System.Collections.Generic;

namespace PerformanceEvaluator.DAL.Entities
{
    public class PageResponse
    {
        public int Id { get; set; }
        public string Url { get; set; }
         
        public virtual ICollection<ResponseTime> ResponseTimes { get; set; }
        public virtual Website Website { get; set; }
    }
}
