using hs_curate.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Diagnostics;

namespace hs_curate.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Bot(int minobjects, int minstars, bool canbearemix)
        {
            var mainclient = new RestClient("http://community.gethopscotch.com/api/v1/projects/newest");
            var mainrequest = new RestRequest();
            mainrequest.Method = Method.Get;
            RestResponse mainresponse = mainclient.Execute(mainrequest);
            dynamic newestjsonresponse = JObject.Parse(mainresponse.Content.ToString());
            foreach (var item in newestjsonresponse.projects)
            {
                var client = new RestClient($"http://community.gethopscotch.com/api/v1/projects/{item.uuid}");
                var request = new RestRequest();
                request.Method = Method.Get;
                RestResponse response = client.Execute(request);
                dynamic jsonresponse = JObject.Parse(response.Content.ToString());
                int totalobjects = 0;
                bool isremix = false;
                if (jsonresponse.number_of_stars >= minstars)
                {
                    foreach (var anotheritem in jsonresponse.objects)
                    {
                        totalobjects += 1;
                    }
                    if (totalobjects >= minobjects)
                    {
                        if (jsonresponse.user_id != jsonresponse.original_user.id || canbearemix == true)
                        {
                            if (jsonresponse.user_id != jsonresponse.original_user.id)
                            {
                                isremix = true;
                            }
                            ViewBag.Message += $"<div style=\"background-color:gray; padding:center; width:50%;\"><img width=\"90%\" height=\"90%\" src=\"{jsonresponse.screenshot_url}\" alt=\"oops i made a mistake\"><br /><h2>{jsonresponse.title}</h2><br /><p>By: {jsonresponse.user.nickname}<br />Remix: {isremix}<br />Plays: {jsonresponse.play_count}<br />Hearts: {jsonresponse.number_of_stars}<br />Plants: {jsonresponse.plants}<br /><a href=\"https://c.gethopscotch.com/p/{jsonresponse.uuid}\">Play Project</a><br />Objects: {totalobjects}<br></p></div><br />";
                        }
                    }
                }
            }
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}