using Job_Fair_Sys_Data;

namespace Job_Fair_Sys_API.Models
{
    public class EventFeedbackViewModel
    {
        public int id { get; set; }
        public string feedback { get; set; }
        public int companyId { get; set; }

        public EventFeedback ToEntity()
        {
            var entity = new EventFeedback
            {
                Id = this.id,
                Feedback = this.feedback,
                CompanyId = this.companyId
            };
            return entity;
        }

        public static EventFeedbackViewModel ToViewModel(EventFeedback entity)
        {
            var model = new EventFeedbackViewModel
            {
                id = entity.Id,
                feedback = entity.Feedback,
                companyId = entity.CompanyId
            };

            return model;
        }
    }
}