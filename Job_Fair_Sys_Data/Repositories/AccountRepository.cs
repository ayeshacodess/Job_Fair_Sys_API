using System.Linq;

namespace Job_Fair_Sys_Data.Repositories
{
    public class AccountRepository : BaseRepository<User>
    {
        public AccountRepository() : base()
        {

        }

        public User IsAuthorizedUser(string email, string password)
        {
            var user = _dbContext.Users.FirstOrDefault(x => 
            (x.Username.ToLower().Equals(email.ToLower())
            ) && x.Password.Equals(password));

            return user;
        }
    }
}
