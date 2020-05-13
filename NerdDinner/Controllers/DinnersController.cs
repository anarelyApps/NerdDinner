using NerdDinner.Helpers;
using NerdDinner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NerdDinner.Controllers
{
    public class DinnersController : Controller
    {
        DinnerRepository dinnerRepository = new DinnerRepository();

        // GET: Dinners
        public ActionResult Index(int? page)
        {
            const int pagesize = 5;

            var upcomingDinners = dinnerRepository.FindUpcomingDinners();
            var paginatedDinners = new PaginatedList<Dinners>(upcomingDinners, page ?? 0, pagesize);

           return View(paginatedDinners);
        }

        public ActionResult Details(int id) {
            Dinners dinner = dinnerRepository.GetDinner(id);
            if (dinner == null)
                return View("NotFound");
            else
                return View("Details", dinner);
        }

       [Authorize]
        public ActionResult Edit(int id)
        {
            Dinners dinner = dinnerRepository.GetDinner(id);

            // ViewData["Countries"] = new SelectList(PhoneValidator.Countries, dinner.Country);
            if (!dinner.IsHostedBy(User.Identity.Name))
                return View("InvalidOwner");

            return View(new DinnerFormViewModel(dinner));
        }

        //
        // POST: /Dinners/Edit/2
        [AcceptVerbs(HttpVerbs.Post),Authorize]
        public ActionResult Edit(int id, FormCollection formValues)
        {
            Dinners dinner = dinnerRepository.GetDinner(id);

            if (!dinner.IsHostedBy(User.Identity.Name))
                return View("InvalidOwner");

            try
            {
                UpdateModel(dinner);
                dinnerRepository.Save();
                return RedirectToAction("Details", new { id = dinner.DinnerID });
            }
            catch
            {
                // ModelState.AddModelErrors(dinner.GetRuleViolations());

                
                return View(new DinnerFormViewModel(dinner));
            }
        }

        // GET: /Dinners/Create
        [Authorize(Users = "scottgu,billg,anarely006@gmail.com")]
        public ActionResult Create()
        {
            Dinners dinner = new Dinners()
            {
                EventDate = DateTime.Now.AddDays(7)
            };
            return View(new DinnerFormViewModel(dinner));
        }

        // POST: /Dinners/Create
        [AcceptVerbs(HttpVerbs.Post), Authorize]
        public ActionResult Create(Dinners dinner)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    dinner.DinnerID = dinnerRepository.GetDinnerNewId()+1;
                    dinner.HostedBy = User.Identity.Name;

                    RSVP rsvp = new RSVP();
                    rsvp.RsvpID = dinnerRepository.GetRsvpNewId() + 1;
                    rsvp.AttendeeName = User.Identity.Name;
                    dinner.RSVP.Add(rsvp);

                    dinnerRepository.Add(dinner);
                    dinnerRepository.Save();
                    return RedirectToAction("Details", new { id = dinner.DinnerID });
                }
                catch (Exception e)
                {
                  //ModelState.AddRuleViolations(dinner.GetRuleViolations());
                }
            }
            return View(new DinnerFormViewModel(dinner));
        }

        //
        // HTTP GET: /Dinners/Delete/1
        public ActionResult Delete(int id)
        {
            Dinners dinner = dinnerRepository.GetDinner(id);
            if (dinner == null)
                return View("NotFound");
            else
                return View(dinner);
        }

        // HTTP POST: /Dinners/Delete/1
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(int id, string confirmButton)
        {
            Dinners dinner = dinnerRepository.GetDinner(id);
            if (dinner == null)
                return View("NotFound");
            dinnerRepository.Delete(dinner);
            dinnerRepository.Save();
            return View("Deleted");
        }
    }
}