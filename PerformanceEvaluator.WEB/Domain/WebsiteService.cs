﻿using System.Collections.Generic;
using PerformanceEvaluator.DAL.Entities;
using PerformanceEvaluator.DAL.Repository;
using PerformanceEvaluator.WEB.Models;

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
            _repository.Add(website);
        }

        public Website Get(int id)
        {
            return _repository.Get(id);
        }

        public IEnumerable<Website> GetAll()
        {
            return _repository.GetAll();
        }

        public List<WebsiteModel> GetWebsiteModels(List<Website> websites)
        {
            var websiteModels = new List<WebsiteModel>();

            foreach (var website in websites)
            {
                websiteModels.Add(new WebsiteModel()
                {
                    Url = website.Url,
                    Id = website.Id
                });
            }

            return websiteModels;
        }
    }
}