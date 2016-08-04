using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using PerformanceEvaluator.DAL.Entities;
using PerformanceEvaluator.WEB.Domain;
using PerformanceEvaluator.WEB.Models;

namespace PerformanceEvaluator.WEB.Controllers
{
    public class HomeController : Controller
    {
        readonly PerformanceEvaluatorService _evaluatorService;
        readonly WebsiteService _websiteService;
        private readonly IMapper _mapper;

        public HomeController()
        {
           var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Page, PageModel>().ReverseMap();
                cfg.CreateMap<Website, WebsiteModel>();
            });

            _mapper = mapper.CreateMapper();
            _websiteService = new WebsiteService();
            _evaluatorService = new PerformanceEvaluatorService();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string urlText)
        {
            return View();
        }

        public JsonResult GetTime(string urlText)
        {
            var website = _evaluatorService.GetWebsite(urlText);

            if (website.Pages.Any())
            {
                _websiteService.Add(website);
            }

            var pageModels = _mapper.Map<ICollection<Page>, List<PageModel>>(website.Pages);

            return Json(pageModels, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPagesProcessedNumber()
        {
            var pageProccesedNumber = _evaluatorService.GetPagesProcessedNumber();
     
            return Json(pageProccesedNumber, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ShowWebsite(int id)
        {
            var website = _websiteService.Get(id);
            var pageModels = _mapper.Map<ICollection<Page>, List<PageModel>>(website.Pages);

            return Json(pageModels, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWebsites()
        {
            var websites = _websiteService.GetAll().ToList();
            var websiteModels = _mapper.Map<List<Website>, List<WebsiteModel>>(websites);
            websiteModels = websiteModels.OrderByDescending(w => w.Id).ToList();

            return Json(websiteModels, JsonRequestBehavior.AllowGet);
        }
    }
}