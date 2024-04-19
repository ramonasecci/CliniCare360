using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CliniCare360.Models;

namespace CliniCare360.Controllers
{
    public class UsersController : Controller
    {
        private DBContext db = new DBContext();

        //pagina con lista pazienti visibile soltanto ad amministratore
        [Authorize(Roles = "admin")]
        public ActionResult ListaPazienti()
        {
            var pazienti = db.Users
                .Where(u => u.Ruolo == "user")
                .ToList();
            return View(pazienti);
        }

        //pagina del singolo paziente visilibe soltanto allo user
        [Authorize(Roles = "user")]
        public ActionResult Profilo()
        {
            var userId = Convert.ToInt32(User.Identity.Name);   
            Users users = db.Users.Find(userId);
            if (users == null)
            {
                return HttpNotFound();
            }
            //nel suo profino l'utente dovrà vedere:
            //la lista delle visite passate + btn per accedere alla prescrizione di quella visita
            ViewData["VisitePassate"] = db.Appuntamenti
                                  .Where(a => a.UserId == userId && a.Stato == "evaso")
                                  .ToList();
            //la lista dei prossimi appuntamenti 
            ViewData["VisiteFuture"] = db.Appuntamenti
                                .Where(a => a.UserId == userId && a.Stato == "prenotato")
                                .ToList();
            //potra poi accedere alla modifica di alcun dati del profilo(esclusi nome cognome codice fiscale mail e password)
            return View(users);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            var model = new Users
            {
                Ruolo = "user"
            };

            return View(model);
        }

        // POST: Users/Create
        // Per proteggere da attacchi di overposting, specifica esplicitamente le proprietà da includere nel binding.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,Nome,Cognome,DataNascita,CodFiscale,Indirizzo,Citta,Telefono,Email,Password,Ruolo")] Users users, HttpPostedFileBase ImgUser)
        {
            var emailExist = db.Users.Any(u => u.Email == users.Email);
            if (emailExist)
            {
                ModelState.AddModelError("", "L'email fornita è già utilizzata.");
            }
            var codFiscaleExist = db.Users.Any(u => u.CodFiscale == users.CodFiscale);
            if (codFiscaleExist)
            {
                ModelState.AddModelError("", "Il codice fiscale fornito è già utilizzato.");
            }

            users.Ruolo = "user";

            if (ModelState.IsValid)
            {
                if (ImgUser != null && ImgUser.ContentLength > 0)
                {
                    if (ImgUser.ContentType.StartsWith("image"))
                    {
                        using (var reader = new BinaryReader(ImgUser.InputStream))
                        {
                            users.ImgUser = reader.ReadBytes(ImgUser.ContentLength);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ImgUser", "Il file selezionato non è un'immagine valida.");
                        return View(users);
                    }
                }
                db.Users.Add(users);
                db.SaveChanges();
                return RedirectToAction("Profilo", new { id = users.UserId });
            }
            return View(users);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Users/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,Nome,Cognome,ImgUser,DataNascita,CodFiscale,Indirizzo,Citta,Telefono,Email,Password,Ruolo")] Users users)
        {
            if (ModelState.IsValid)
            {
                db.Entry(users).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(users);
        }

        //Mostra il dettaglio del paziente all'amministratore
        [Authorize(Roles = "admin")]
        public ActionResult DettaglioPaziente(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Users user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

         
            ViewData["VisitePrenotate"] = db.Appuntamenti
                                            .Where(a => a.UserId == id && a.Stato == "prenotato")
                                            .Include(a => a.Prestazioni) 
                                            .ToList();

            ViewData["VisitePassate"] = db.Appuntamenti
                                        .Where(a => a.UserId == id && a.Stato == "completato")
                                        .Include(a => a.Prestazioni) 
                                        .ToList();

            return View("DettaglioPaziente", user);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
