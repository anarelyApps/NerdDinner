using NerdDinner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NerdDinner.Controllers
{
    public class RSVPController : Controller
    {
        DinnerRepository dinnerRepository = new DinnerRepository();

        // GET: RSVP
        public ActionResult Index()
        {
            return View();
        }

        [Authorize, AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Register(int id)
        {
            Dinners dinner = dinnerRepository.GetDinner(id);
            if (!dinner.IsUserRegistered(User.Identity.Name))
            {
                RSVP rsvp = new RSVP();
                rsvp.RsvpID = dinnerRepository.GetRsvpNewId() + 1;
                rsvp.AttendeeName = User.Identity.Name;
                dinner.RSVP.Add(rsvp);
                dinnerRepository.Save();
            }
            return Content("Thanks - we'll see you there!");
        }
    }
}