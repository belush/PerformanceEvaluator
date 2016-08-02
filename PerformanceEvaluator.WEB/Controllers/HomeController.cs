using System.Linq;
using System.Web.Mvc;
using PerformanceEvaluator.WEB.Domain;

namespace PerformanceEvaluator.WEB.Controllers
{
    public class HomeController : Controller
    {
        readonly PerformanceEvaluatorService _evaluatorService;
        readonly WebsiteService _websiteService;

        public HomeController()
        {
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

            var pageModels = _evaluatorService.GetPageModels(website.Pages.ToList());

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
            var pageModels = _evaluatorService.GetPageModels(website.Pages.ToList());

            return Json(pageModels, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWebsites()
        {
            var websites = _websiteService.GetAll().ToList();
            var websiteModels = _websiteService.GetWebsiteModels(websites);
            websiteModels = websiteModels.OrderByDescending(w => w.Id).ToList();

            return Json(websiteModels, JsonRequestBehavior.AllowGet);
        }
    }
}