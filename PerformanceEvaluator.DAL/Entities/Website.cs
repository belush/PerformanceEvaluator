using System.Collections.Generic;

namespace PerformanceEvaluator.DAL.Entities
{
    public class Website
    {
        public int Id { get; set; }
        public string Url { get; set; }

        public virtual ICollection<PageResponse> Pages { get; set; }
    }
}
