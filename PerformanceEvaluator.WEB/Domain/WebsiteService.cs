using System.Collections.Generic;
using PerformanceEvaluator.DAL.Entities;
using PerformanceEvaluator.DAL.Repository;

namespace PerformanceEvaluator.WEB.Domain
{
    public class WebsiteService
    {
        private readonly WebsiteRepository _repository;

        public WebsiteService()
        {
            _repository = new WebsiteRepository();
        }

        public void Add(Website website)
        {
            if (website != null)
            {
                _repository.Add(website);
            }
        }

        public Website Get(int id)
        {
            return _repository.Get(id);
        }

        public IEnumerable<Website> GetAll()
        {
            return _repository.GetAll();
        }
    }
}