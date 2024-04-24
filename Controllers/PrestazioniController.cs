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
    public class PrestazioniController : Controller
    {
        private DBContext db = new DBContext();

        // GET: Prestazioni
        //accessibile a tutti
        public ActionResult Index()
        {
            return View(db.Prestazioni.ToList());
        }


        //accessibile solo all'admin
        // GET: Prestazioni/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prestazioni prestazioni = db.Prestazioni.Find(id);
            if (prestazioni == null)
            {
                return HttpNotFound();
            }
            return View(prestazioni);
        }

        //accessibile solo all'admin
        // GET: Prestazioni/Create
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PrestazioneId,Nome,Descrizione,Costo")] Prestazioni prestazioni, HttpPostedFileBase ImgServizio)
        {
            if (ModelState.IsValid)
            {
                if (ImgServizio != null && ImgServizio.ContentLength > 0)
                {
                    if (ImgServizio.ContentType.StartsWith("image"))
                    {
                        using (var reader = new BinaryReader(ImgServizio.InputStream))
                        {
                            prestazioni.ImgServizio = reader.ReadBytes(ImgServizio.ContentLength);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ImgServizio", "Il file selezionato non è un'immagine valida.");
                        return View(prestazioni);
                    }
                }
                db.Prestazioni.Add(prestazioni);
                db.SaveChanges();
                TempData["SuccessMessageCreate"] = "La nuova prestazione è stata creata con successo.";
                return RedirectToAction("Index");
              
            }

            return View(prestazioni);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prestazioni prestazioni = db.Prestazioni.Find(id);
            if (prestazioni == null)
            {
                return HttpNotFound();
            }
            return View(prestazioni);
        }

        //accessibile solo all'admin
        // POST: Prestazioni/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PrestazioneId,Nome,Descrizione,Costo")] Prestazioni prestazioni, HttpPostedFileBase ImgServizio)
        {
            if (ModelState.IsValid)
            {
   
                if (ImgServizio != null && ImgServizio.ContentLength > 0)
                {
                    if (ImgServizio.ContentType.StartsWith("image"))
                    {
                        using (var reader = new BinaryReader(ImgServizio.InputStream))
                        {
                            prestazioni.ImgServizio = reader.ReadBytes(ImgServizio.ContentLength);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ImgServizio", "Il file selezionato non è un'immagine valida.");
                        return View(prestazioni);
                    }
                }
                else
                {
                    
                    var existingPrestazione = db.Prestazioni.AsNoTracking().FirstOrDefault(p => p.PrestazioneId == prestazioni.PrestazioneId);
                    if (existingPrestazione != null)
                    {
                        prestazioni.ImgServizio = existingPrestazione.ImgServizio;
                    }
                }

                db.Entry(prestazioni).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessageEdit"] = "La nuova prestazione è stata modificata con successo.";
                return RedirectToAction("Index");
               
            }
            return View(prestazioni);
        }



        //accessibile solo all'admin
        // POST: Prestazioni/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Prestazioni prestazioni = db.Prestazioni.Find(id);
            var appId = db.Appuntamenti
                .Where(a => a.PrestazioneId == id).ToList();
            if(appId.Count > 0)
            {
                foreach (var app in appId)
                {
                    app.PrestazioneId = 1;
                }
            }
            db.Prestazioni.Remove(prestazioni);          
            db.SaveChanges();
            TempData["SuccessMessageDelete"] = "La prestazione è stata eliminata con successo.";
            return RedirectToAction("Index");
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
