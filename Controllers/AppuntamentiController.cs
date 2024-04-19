using Antlr.Runtime.Misc;
using CliniCare360.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CliniCare360.Controllers
{
    public class AppuntamentiController : Controller
    {
        private DBContext db = new DBContext();

        //PAGINA ACCESSIBILE SOLO AGLI UTENTI LOGGATI CHE MOSTRI GLI APPUNTAMENTI DISPONIBILI PER LA PRESTAZIONE SELEZIONATA
        /*LO STATO DEGLI APPUNTAMENTI POTRà ESSERE
         -disponibile
        -prenotato
        -disdetto
        -evaso*/

        [Authorize(Roles = "user")]
        public ActionResult AppDispByPrest(int? id)
        {        
            // Carica tutti gli appuntamenti disponibili per la prestazione selezionata
            var appPrests = db.Appuntamenti
                .Where(a => a.Stato == "disponibile" && a.PrestazioneId == id && a.Data > DateTime.Now)
                .ToList();

            // Ordina gli appuntamenti in memoria per la data e ora combinata
            var appPrestsSorted = appPrests.OrderBy(a => a.Data.Add(a.Ora)).ToList();

            return View(appPrestsSorted);
        }

        //PAGINA ACCESSIBILE SOLO AGLI UTENTI LOGGATI CHE MOSTRA I DETTAGLI DELL'APPUNTAMENTO SELEZIONATO E CHIEDE CONFERMA DI PRENOTAZIONE
        [Authorize(Roles = "user")]
        public ActionResult ConfermaApp(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var appSelect = db.Appuntamenti
                .Where(a => a.AppId == id)
                .FirstOrDefault();

            if (appSelect == null || appSelect.Stato != "disponibile")
            {
                ViewBag.ErrorMessage = "L'appuntamento selezionato non è più disponibile.";
                return RedirectToAction("AppDispByPrest"); 
            }

            return View(appSelect);
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        [ValidateAntiForgeryToken]
        //azione che conferma l'appuntamento mod stato e associa id user
        public ActionResult ConfermaAppuntamento(Appuntamenti app)
        {
            

            var appuntamento = db.Appuntamenti.Find(app.AppId);

            //verifica se l'appuntamento non esiste e manda messaggio di errore
            if (appuntamento == null)
            {
               
                ViewBag.ErrorMessage = "L'appuntamento non esiste.";
                return View("Errore"); // Assicurati di avere una View "Errore" per gestire queste situazioni
            }

            //verifica se l'appuntamento è disponibile e manda messaggio di errore

            if (appuntamento.Stato != "disponibile")
            {               
                ViewBag.ErrorMessage = "L'appuntamento non è più disponibile.";
                return View("Errore"); 
            }

            // Se l'appuntamento esiste ed è disponibile, aggiorniamo lo stato
            appuntamento.Stato = "prenotato";
            appuntamento.UserId = Convert.ToInt32(User.Identity.Name);
            db.SaveChanges();

            //reindirizza al dettaglio dell'appuntamento con messaggio di conferma
            TempData["MessaggioConferma"] = "La tua visita è confermata. Siamo pronti a prenderti a cuore con la massima professionalità";
            return RedirectToAction("DettaglioApp", new { id = appuntamento.AppId });
        }


        //accessibile solo agli admin per visione di tutti gli appuntamenti 
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult ListaApp()
        {            
            return View();          
        }


        //gestione degli appuntamenti e filtro visualizzazione per stato
        [Authorize(Roles = "admin")]
        public JsonResult ListaAppTot()
        {
            var dataRisposta = new List<object>();

            var appuntamenti = db.Appuntamenti.ToList();

            foreach (var app in appuntamenti)
            {
                var pren = new
                {
                    Id = Convert.ToInt32(app.AppId),
                    Data = app.Data.ToString("dd/MM/yyyy"),
                    Ora = app.Ora.ToString(@"hh\:mm"),
                    Stato= app.Stato.ToString(),
                    Prestazione = app.Prestazioni.Nome.ToString()
                };

                dataRisposta.Add(pren);
            }

            return Json(new { success = true, data = dataRisposta }, JsonRequestBehavior.AllowGet);
        }

        //creazione appuntamenti accessibile solo all'amministrazione, potrà inserire solo la data, l'ora e selezionare la prestazione
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult CreaApp()
        {
            var model = new Appuntamenti
            {
                // imposto valori di defoult non modificabili dall'utenza per seguire la logica della prenotazione
                Stato = "disponibile",     
                MedicoId = null, 
                UserId = null,
                NoteVisita = null,
                Prescrizione = null,

            };

            ViewBag.PrestazioneId = new SelectList(db.Prestazioni.ToList(), "PrestazioneId", "Nome");
            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult CreaApp(Appuntamenti app)
        {            
            if (ModelState.IsValid)
            {            
                db.Appuntamenti.Add(app);
                db.SaveChanges();
                TempData["MessaggioConferma"] = "Ben fatto! Hai aggiunto con successo un nuovo appuntamento";
                return RedirectToAction("DettaglioApp", new { id = app.AppId });
            }
           
            TempData["ErrorMessage"] = "Ci sono alcuni errori di compilazione. Si prega di correggerli.";
            ViewBag.PrestazioneId = new SelectList(db.Prestazioni.ToList(), "PrestazioneId", "Nome", app.PrestazioneId);
            return View(app);
        }





        //dettaglio appuntamento accessibile solo all'amministrazione e all'utente

        [HttpGet]
        [Authorize]
        public ActionResult DettaglioApp(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appuntamenti appuntamenti = db.Appuntamenti.Find(id);
            if (appuntamenti == null)
            {
                return HttpNotFound();
            }
            return View(appuntamenti);
        }

        //pagina accessibile solo all'amministrazione per la modifica dell'appuntamento selezionato,
        //il medico durante la visita potrà aggiungere le note della visita la prescrizione e potra associare la sua identità
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult ModificaApp(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Appuntamenti appuntamento = db.Appuntamenti.Find(id);
            if (appuntamento == null)
            {
                return HttpNotFound();
            }

            ViewBag.Medici = new SelectList(db.Medici.Select(m => new SelectListItem
            {
                Value = m.MedicoId.ToString(),
                Text = m.Nome + " " + m.Cognome
            }).ToList(), "Value", "Text");


            ViewBag.StatiAppuntamento = new List<SelectListItem>
            {
                new SelectListItem { Text = "Disponibile", Value = "disponibile" },
                new SelectListItem { Text = "Prenotato", Value = "prenotato" },
                new SelectListItem { Text = "Evaso", Value = "evaso" }  
            };
            
            return View(appuntamento);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult ModificaApp(Appuntamenti model)
        {
            if (ModelState.IsValid)
            {
                var appuntamento = db.Appuntamenti.Find(model.AppId);
                if (appuntamento == null)
                {
                    return HttpNotFound();
                }

                appuntamento.NoteVisita = model.NoteVisita;
                appuntamento.Stato = model.Stato;
                appuntamento.Prescrizione = model.Prescrizione;
                appuntamento.MedicoId = model.MedicoId;

 
                db.Entry(appuntamento).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("DettaglioPaziente","Users", new { id = appuntamento.UserId }); 
            }

            return View(model);
        }


        //accessibile solo all'admin disdice appuntamento, (riporta null il campo userid e stato su disponibile)
        [HttpPost]
        [Authorize(Roles = "user")]
        [ValidateAntiForgeryToken]
        public ActionResult DisdiciAppuntamento(int AppId)
        {
            var appID = Convert.ToInt32(AppId);
            var appuntamento = db.Appuntamenti.Find(appID);
            if (appuntamento != null)
            {                
                appuntamento.Stato = "disponibile";
                appuntamento.UserId = null;
                db.Entry(appuntamento).State = EntityState.Modified;
                db.SaveChanges();
                TempData["MessaggioConfermaDisdetta"] = "La disdetta dell'appuntamento è stata confermata con successo";
                return RedirectToAction("Profilo","Users",new {id= Convert.ToInt32(User.Identity.Name)}); 
            }
            return View("Error"); 
        }











    }
}