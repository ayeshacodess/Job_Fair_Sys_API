using Job_Fair_Sys_Data;

namespace Job_Fair_Sys_API.Models
{
    public class EventFeedbackViewModel
    {
        public int id { get; set; }
        public string feedbackContent { get; set; }
        public int companyId { get; set; }
        public EventFeedback ToEntity()
        {
            var entity = new EventFeedback
            {
                Id = this.id,
                Feedback = this.feedbackContent,
                CompanyId = this.companyId
            };
            return entity;
        }

        public static EventFeedbackViewModel ToViewModel(EventFeedback entity)
        {
            var model = new EventFeedbackViewModel
            {
                id = entity.Id,
                feedbackContent = entity.Feedback,
                // companyId = entity.CompanyId
                companyId = entity.CompanyId
            };

            return model;
        }
    }

    public class DisplayEventFeedbackViewModel
    {

        public string companyName { get; set; }
        public string feedback { get; set; }

        public static DisplayEventFeedbackViewModel ToViewModel(EventFeedback entity)
        {
            var model = new DisplayEventFeedbackViewModel
            {

                feedback = entity.Feedback,
                // companyId = entity.CompanyId
                companyName = entity.Company.Name
            };

            return model;
        }
    }
}