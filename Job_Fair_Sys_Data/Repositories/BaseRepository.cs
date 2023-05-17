namespace Job_Fair_Sys_Data.Repositories
{
    public class BaseRepository<T> where T : class
    {
        protected readonly JobFairMgtEntities _dbContext;

        public BaseRepository()
        {
            _dbContext = new JobFairMgtEntities();
        }

        public JobFairMgtEntities DbContext {
            get => _dbContext;
        }

        public bool SaveChanges()
        {
            return _dbContext.SaveChanges() > 0;
        }

        //public virtual T GetById(int id)
        //{
        //    return _context.Set<T>().Find(id);
        //}

        //public virtual IEnumerable<T> GetAll()
        //{
        //    return _context.Set<T>().ToList();
        //}

        //public virtual void Add(T entity)
        //{
        //    _context.Set<T>().Add(entity);
        //}

        ////public virtual void Update(T entity)
        ////{
        ////    _context.Set<T>().Update(entity);
        ////}

        //public virtual void Delete(T entity)
        //{
        //    _context.Set<T>().Remove(entity);
        //}

        //public virtual void Save()
        //{
        //    _context.SaveChanges();
        //}
    }
}
