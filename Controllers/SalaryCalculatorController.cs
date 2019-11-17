using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Portfolio.Classes;
using System.Threading.Tasks;
using System.Threading;
using Portfolio.Models;
using Google.Apis.Calendar.v3.Data;

namespace Portfolio.Controllers
{
    public class SalaryCalculatorController : Controller
    {

        // GET: SalleryCalculator
        public async Task<ActionResult> Index(CancellationToken cancellationToken, SalaryViewModels model)
        {
            var result = await new AuthorizationCodeMvcApp(this, new AppFlowMetadata()).
                AuthorizeAsync(cancellationToken);

            if (result.Credential != null)
            {

                if (model.Eventname != null)
                {
                    if (ModelState.IsValid)
                    {
                        var service = new CalendarService(new BaseClientService.Initializer
                        {
                            HttpClientInitializer = result.Credential,
                            ApplicationName = "ASP.NET MVC Sample"
                        });
                        EventsResource.ListRequest request = service.Events.List("primary");

                        request.TimeMin = DateTime.Parse(model.Startdate);
                        request.ShowDeleted = false;
                        request.SingleEvents = true;
                        request.TimeMax = DateTime.Parse(model.Enddate);
                        request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                        List<double> WorkhoursList = new List<double> { };

                        // List events.
                        Events events = request.Execute();
                        if (events.Items != null && events.Items.Count > 0)
                        {
                            foreach (var eventItem in events.Items.Where(t => t.Summary == model.Eventname))
                            {
                                string when = eventItem.Start.DateTime.ToString();
                                string end = eventItem.End.DateTime.ToString();
                                if (String.IsNullOrEmpty(when))
                                {
                                    when = eventItem.Start.Date;
                                }
                                if (String.IsNullOrEmpty(end))
                                {
                                    end = eventItem.End.Date;
                                }
                                TimeSpan workhours = DateTime.Parse(end) - DateTime.Parse(when);

                                double realtime = (((double)workhours.Hours * 60.0 + (double)workhours.Minutes) / 60.0);

                                WorkhoursList.Add(realtime);



                            }
                            double totalworkhours = 0;
                            foreach (var item in WorkhoursList)
                            {
                                totalworkhours += item;
                            }


                            double hoursalary = Convert.ToDouble(model.Salary);

                            model.Totalhours = totalworkhours;
                            decimal totalsalary = (decimal)totalworkhours * (decimal)hoursalary;
                            model.TotalSalary = totalsalary;
                            return View(model);
                        }
                        else
                        {
                            ViewBag.error = ("No events Found");
                            return View();
                        }
                    }
                    return View();
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return new RedirectResult(result.RedirectUri);
            }
        }
    }
}