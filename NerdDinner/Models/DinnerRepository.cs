using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NerdDinner.Models
{
    public class DinnerRepository
    {
        private NerdDinnerDataContext db = new NerdDinnerDataContext();

        public IQueryable<Dinners> FindAllDinners() {
            return db.Dinners;
        }
        public IQueryable<Dinners> FindUpcomingDinners() {
            return from dinner in db.Dinners
                  // where dinner.EventDate > DateTime.Now
                   orderby dinner.EventDate
                   select dinner;
        }

        public Dinners GetDinner(int id) {
            return db.Dinners.SingleOrDefault(d => d.DinnerID == id);
        }

        public int GetRsvpNewId() {
            return db.RSVP.Count();
        }

        public int GetDinnerNewId()
        {
            return db.Dinners.Count();
        }

        public void Add(Dinners dinner) {
            db.Dinners.InsertOnSubmit(dinner);
        }
        public void Delete(Dinners dinner) {
            db.RSVP.DeleteAllOnSubmit(dinner.RSVP);
            db.Dinners.DeleteOnSubmit(dinner);
        }

        public void Save() {
            db.SubmitChanges();
        }

        public IQueryable<Dinners> FindByLocation(float latitude, float longitude) {
            var dinners = from dinner in FindUpcomingDinners()
                          join i in db.NearestDinners(latitude, longitude)
                          on dinner.DinnerID equals i.DinnerID
                          select dinner;
            return dinners;
        }
    }
}