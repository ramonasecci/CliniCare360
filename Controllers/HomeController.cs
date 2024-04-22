using CliniCare360.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CliniCare360.Controllers
{
    public class HomeController : Controller
    {
        private DBContext db = new DBContext();
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            var postsInfo = db.Posts
                .Where(p => p.Tipo == "info").ToList();
            var postsPromo = db.Posts
               .Where(p => p.Tipo == "promozione").ToList();

            ViewBag.PostPromo = postsPromo;

            ViewBag.PostInfo = postsInfo;

            return View();
        }

        //pagina con le info della clinica accessibili a tutti
        public ActionResult Clinica()
        {
             return View();
        }

        //aggiunta post visibile soltanto all'amministrazione
        [Authorize(Roles = "admin")]
        public ActionResult AddPost()
        {
            return View();
        }

        //aggiunta post visibile soltanto all'amministrazione

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken] 
        public ActionResult AddPost([Bind(Include = "Titolo,Contenuto,Tipo")] Posts postModel, HttpPostedFileBase ImgPost)
        {
            if (ModelState.IsValid)
            {
                postModel.DataOraPublic = DateTime.Now;

                if (ImgPost != null && ImgPost.ContentLength > 0)
                {
                    if (ImgPost.ContentType.StartsWith("image"))
                    {
                        using (var reader = new BinaryReader(ImgPost.InputStream))
                        {
                            postModel.ImgPost = reader.ReadBytes(ImgPost.ContentLength);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ImgPost", "Il file selezionato non è un'immagine valida.");
                        return View(postModel);
                    }
                }

                db.Posts.Add(postModel);
                db.SaveChanges(); 

                return RedirectToAction("Index"); 
            }

            return View(postModel);
        }


        //modifica post accessibile solo all'amministrazione
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult EditPost(int id)
        {
            var postModel = db.Posts.Find(id);
            if (postModel == null)
            {
                return HttpNotFound();
            }
            return View(postModel);
        }

        // Gestisce la sottomissione del form di modifica
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost([Bind(Include = "PostId,Titolo,Contenuto,Tipo")] Posts postModel, HttpPostedFileBase ImgPost)
        {
            if (ModelState.IsValid)
            {
                var postToUpdate = db.Posts.Find(postModel.PostId);
                if (postToUpdate == null)
                {
                    return HttpNotFound();
                }

                postToUpdate.Titolo = postModel.Titolo;
                postToUpdate.Contenuto = postModel.Contenuto;
                postToUpdate.Tipo = postModel.Tipo;

                if (ImgPost != null && ImgPost.ContentLength > 0)
                {
                    if (ImgPost.ContentType.StartsWith("image"))
                    {
                        using (var reader = new BinaryReader(ImgPost.InputStream))
                        {
                            postToUpdate.ImgPost = reader.ReadBytes(ImgPost.ContentLength);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ImgPost", "Il file selezionato non è un'immagine valida.");
                        return View(postModel);
                    }
                }

                db.Entry(postToUpdate).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(postModel);
        }


    }
}
