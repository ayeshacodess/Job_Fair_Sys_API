using Job_Fair_Sys_API.Models;
using Job_Fair_Sys_Data.Repositories;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Job_Fair_Sys_API.Controllers 
{
    public class EventFeedBackController : ApiController
    {
        private readonly EventFeedbackRepository _eventFeedbackRepository;

        public EventFeedBackController()
        {
            _eventFeedbackRepository = new EventFeedbackRepository();
        }

        [Route("api/feedbacks")]
        public HttpResponseMessage Get()
        {
            try
            {
                var feedbacks = _eventFeedbackRepository.GetEventFeedbacks();

                var models = new List<EventFeedbackViewModel>();
                foreach (var item in feedbacks)
                {
                    models.Add(EventFeedbackViewModel.ToViewModel(item));
                }

                return Request.CreateResponse(HttpStatusCode.OK, models);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("api/feedback/add")]
        public HttpResponseMessage AddFeedback(EventFeedbackViewModel model)
        {
            try
            {
                if (model == null) return Request.CreateResponse(HttpStatusCode.NotAcceptable);

                var entity = model.ToEntity();
                _eventFeedbackRepository.Add(entity);

                return Request.CreateResponse(HttpStatusCode.OK, model);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("api/feedback/update")]
        public HttpResponseMessage UpdateFeedback(EventFeedbackViewModel model)
        {
            try
            {
                if (model == null || model.id == 0)
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable);

                var entity = _eventFeedbackRepository.GetCompanyFeedback(model.id);

                if (entity == null) return Request.CreateResponse(HttpStatusCode.NotFound);

                entity.CompanyId = model.companyId;
                entity.Feedback = model.feedback;

                _eventFeedbackRepository.DbContext.Entry(entity).CurrentValues.SetValues(entity);
                _eventFeedbackRepository.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, model);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}