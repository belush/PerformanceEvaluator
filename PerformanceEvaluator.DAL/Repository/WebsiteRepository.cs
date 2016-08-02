using System.Collections.Generic;
using System.Linq;
using PerformanceEvaluator.DAL.Context;
using PerformanceEvaluator.DAL.Entities;

namespace PerformanceEvaluator.DAL.Repository
{
    public class WebsiteRepository
    {
        private readonly PerformanceEvaluatorContext _context;

        public WebsiteRepository()
        {
            _context = new PerformanceEvaluatorContext();
        }

        public IEnumerable<Website> GetAll()
        {
            return _context.Websites;
        }

        public Website Get(int id)
        {
            return _context.Websites.FirstOrDefault(w => w.Id == id);
        }

        public void Add(Website website)
        {
            _context.Websites.Add(website);
            _context.SaveChanges();
        }
    }
}
