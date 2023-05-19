using System.Linq;

namespace Job_Fair_Sys_Data.Repositories
{
    public class AccountRepository : BaseRepository<User>
    {
        public AccountRepository() : base()
        {

        }

        public User IsAuthorizedUser(string username, string password)
        {
            var user = _dbContext.Users.FirstOrDefault(x => 
            (x.Username.ToLower().Equals(username.ToLower())
            ) && x.Password.Equals(password));

            return user;
        }
    }
}
