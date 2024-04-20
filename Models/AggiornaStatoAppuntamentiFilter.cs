using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using CliniCare360.Models;

namespace CliniCare360.Models
{
    public class AggiornaStatoAppuntamentiFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
       {            
            using (var db = new DBContext())
            {
                var dataOdierna = DateTime.Today;
                var appuntamentiDaAggiornare = db.Appuntamenti
                                                  .Where(a => DbFunctions.TruncateTime(a.Data) < dataOdierna && a.Stato != "evaso")
                                                  .ToList();

                foreach (var appuntamento in appuntamentiDaAggiornare)
                {
                    appuntamento.Stato = "evaso";
                }

                db.SaveChanges();
            }
        }
    }
}