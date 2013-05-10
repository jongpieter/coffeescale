using System.Net;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Coffee.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
	        return View(JsonConvert.DeserializeObject<HomeModel>(
	            new WebClient().DownloadString("http://scaleweb.blob.core.windows.net/coffee/state")));
        }
    }

	public class HomeModel
	{
		public decimal NumberOfCups { get; set; }
	}
}
