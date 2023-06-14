using Job_Fair_Sys_API.Models;
using Job_Fair_Sys_Data.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
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

                var models = new List<DisplayEventFeedbackViewModel>();
                foreach (var item in feedbacks)
                {
                    models.Add(DisplayEventFeedbackViewModel.ToViewModel(item));
                }

                return Request.CreateResponse(HttpStatusCode.OK, models);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("api/eventFeedback/save")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> AddFeedback()
        {
            var httpRequest = HttpContext.Current.Request;

            using (var reader = new StreamReader(await Request.Content.ReadAsStreamAsync()))
            {
                try
                {
                    string requestBody = reader.ReadToEnd();
                    var feedback = JsonConvert.DeserializeObject<EventFeedbackViewModel>(requestBody);
                    if (feedback == null) return Request.CreateResponse(HttpStatusCode.NotAcceptable);

                    var entity = _eventFeedbackRepository.GetCompanyFeedback(feedback.companyId);

                    if (entity == null)
                    {
                        var newEntity = feedback.ToEntity();
                        _eventFeedbackRepository.Add(newEntity);
                    }
                    else
                    {
                        entity.CompanyId = feedback.companyId;
                        entity.Feedback = feedback.feedbackContent;

                        _eventFeedbackRepository.DbContext.Entry(entity).CurrentValues.SetValues(entity);
                        _eventFeedbackRepository.SaveChanges();
                    }

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
            }
        }

        [HttpPost]
        [Route("api/feedback/update")]
            public async System.Threading.Tasks.Task<HttpResponseMessage> UpdateFeedback()
        
            {
                var httpRequest = HttpContext.Current.Request;

                using (var reader = new StreamReader(await Request.Content.ReadAsStreamAsync()))
                {
                    try
                    {
                            string requestBody = reader.ReadToEnd();

                            var feedback = JsonConvert.DeserializeObject<EventFeedbackViewModel>(requestBody);
                             if (feedback == null || feedback.id == 0)
                             return Request.CreateResponse(HttpStatusCode.NotAcceptable);

                             var entity = _eventFeedbackRepository.GetCompanyFeedback(feedback.id);

                            if (entity == null) return Request.CreateResponse(HttpStatusCode.NotFound);

                            entity.CompanyId = feedback.companyId;
                         entity.Feedback = feedback.feedbackContent;

                         _eventFeedbackRepository.DbContext.Entry(entity).CurrentValues.SetValues(entity);
                         _eventFeedbackRepository.SaveChanges();

                         return Request.CreateResponse(HttpStatusCode.OK, feedback);
                    }
                    catch (Exception ex)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                    }
                }
            }
    }
}