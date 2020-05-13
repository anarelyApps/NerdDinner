using NerdDinner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NerdDinner.Controllers
{
    public class DinnerFormViewModel
    {
        public Dinners Dinner { get; private set; }
        public SelectList Countries { get; private set; }

        public DinnerFormViewModel(Dinners dinner) {
            Dinner = dinner;
            Countries = new SelectList(PhoneValidator.Countries, dinner.Country);
        }
    }
}