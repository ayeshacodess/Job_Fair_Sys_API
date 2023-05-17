using Job_Fair_Sys_Data;

namespace Job_Fair_Sys_API.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string username { get; set; }
        public string role { get; set; }
        public string email { get; set; }
        public string aridNumber { get; set; }
        public string cgpa { get; set; }
        public string name { get; set; }


        public static UserViewModel ToViewModel(User user)
        {
            var model = new UserViewModel
            {
                Id = user.Id,
                email = user.Email,
                role = user.Role,
                username = user.Username,
            };

            return model;
        }
    }
}