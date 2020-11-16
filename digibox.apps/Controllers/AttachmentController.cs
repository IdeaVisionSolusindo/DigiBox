using AutoMapper;
using digibox.services.Models;
using digibox.services.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace digibox.apps.Controllers
{
    public class AttachmentController : Controller
    {
        // GET: Attachment

        private readonly IAttachmentRepository _attachment;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IAttributeRepository _attribute;

        public AttachmentController(IAttachmentRepository attachment, IAttributeRepository attribute,  MapperConfiguration mapperConfiguration) => 
            (_attachment, _attribute, _mapperConfiguration) = (attachment, attribute ,mapperConfiguration);

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult previewAttachment(Guid id)
        {
            var dta = _attachment.FindById(id);
            var map = new Mapper(_mapperConfiguration);
            var model = map.Map<AttachmentModel>(dta);
            return PartialView("_previewAttachment", model);
        }

        public ActionResult openByReference(Guid id, string step, bool biseditable = true)
        {
            var attachment = _attachment.openByStep(id, step).ToList();
            Mapper mapper = new Mapper(_mapperConfiguration);
            var model = mapper.Map<List<AttachmentModel>>(attachment);
            ViewData["biseditable"] = biseditable;
            return PartialView("_openByReference", model);
        }

        public FileContentResult SaveAttachment(Guid id)
        {
            var dta = _attachment.FindById(id);
            return File(dta.attachment, dta.attachmenttype, dta.filename);
        }
    }
}