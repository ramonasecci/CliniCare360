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
    public class MediciController : Controller
    {
        private DBContext db = new DBContext();

        // GET: Medici
        public ActionResult Index()
        {
            return View(db.Medici.ToList());
        }

        // pagina di creazione del medico accessibile soltanto all'amministrazione
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Medici/Create
        // pagina postPer la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // POST: Medici/Create
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Nome,Cognome,Specializzazione,Esperienza,PatologieTrattate")] Medici medico, HttpPostedFileBase ImgMedico)
        {
            if (ModelState.IsValid)
            {
                if (ImgMedico != null && ImgMedico.ContentLength > 0)
                {
                    if (ImgMedico.ContentType.StartsWith("image"))
                    {
                        using (var reader = new BinaryReader(ImgMedico.InputStream))
                        {
                            medico.ImgMedico = reader.ReadBytes(ImgMedico.ContentLength);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ImgMedico", "Il file selezionato non è un'immagine valida.");
                        return View(medico);
                    }
                }

                db.Medici.Add(medico);
                db.SaveChanges();
                TempData["SuccessCreateMedico"] = "Medico creato con successo.";
                return RedirectToAction("Index");
            }
           
            return View(medico);
        }

        //pagina per la modifica del medico accessibile solo all'amministratore
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Medici medici = db.Medici.Find(id);
            if (medici == null)
            {
                return HttpNotFound();
            }
            return View(medici);
        }

        //post per la modifica del medico accessibile solo all'amministratore
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MedicoId,Nome,Cognome,Specializzazione,Esperienza,PatologieTrattate")] Medici medico, HttpPostedFileBase ImgMedico)
        {
            if (ModelState.IsValid)
            {
                if (ImgMedico != null && ImgMedico.ContentLength > 0)
                {
                    if (ImgMedico.ContentType.StartsWith("image"))
                    {
                        using (var reader = new BinaryReader(ImgMedico.InputStream))
                        {
                            medico.ImgMedico = reader.ReadBytes(ImgMedico.ContentLength);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ImgMedico", "Il file selezionato non è un'immagine valida.");
                        return View(medico);
                    }
                }
                else
                {
                    var existingMedico = db.Medici.AsNoTracking().FirstOrDefault(m => m.MedicoId == medico.MedicoId);
                    if (existingMedico != null)
                    {
                        medico.ImgMedico = existingMedico.ImgMedico;
                    }
                }

                db.Entry(medico).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessageEditMedico"] = "I dati del medico sono stati aggiornati con successo.";
                return RedirectToAction("Index");
            }
            return View(medico);
        }

        //get delete accessibile solo all'amministrazione
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Medici medici = db.Medici.Find(id);
            if (medici == null)
            {
                return HttpNotFound();
            }
            return View(medici);
        }

        // POST: Medici/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Medici medici = db.Medici.Find(id);
            db.Medici.Remove(medici);
            db.SaveChanges();
            TempData["SuccesseDeleteMedico"] = "I dati del medico sono stati eliminati con successo.";
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
