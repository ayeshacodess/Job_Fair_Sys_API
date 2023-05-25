using System.Linq;

namespace Job_Fair_Sys_Data.Repositories
{
    public class AccountRepository : BaseRepository<User>
    {
        public AccountRepository() : base()
        {

        }

        //public User IsAuthorizedUser(string username, string password)
        //{
        //    var user = _dbContext.Users.FirstOrDefault(x => 
        //    (x.Username.ToLower().Equals(username.ToLower())
        //    ) && x.Password.Equals(password));

        //    return user;
        //}
        public User IsAuthorizedUser(string username, string password)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            return user;
        }
        public User GetUser(string email)
        {
            return _dbContext.Users.Where(e => e.Username == email).FirstOrDefault();
        }
        public void RemoveUser(User user)
        {
            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();
        }
    }
}
