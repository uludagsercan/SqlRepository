using SqlFramework.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlFramework.Concrete.SqlRepository;
using System.Linq.Expressions;

namespace SqlFramework.Concrete.SqlRepositoryFacade
{
    public class GenericRepositoryFacade<T> where T : class,new()
    {
        private readonly SqlRepository<T> _repository;
        
        public GenericRepositoryFacade(string connection)
        {
            _repository = new SqlRepository<T>(connection);
        }

        public void Save(T entity)
        {
            _repository.Insert(entity);
        }
        public void Update(T entity)
        {
            _repository.Update(entity);
        }
        public void Delete(T entity)
        {
            _repository.Delete(entity);
        }
        public T Get(Expression<Func<T,bool>>filter)
        {
            return _repository.Get(filter);
        }
        public List<T> GetAll(Expression<Func<T, bool>> filter = null)
        {
            return filter==null ? _repository.GetAll().ToList() : _repository.GetAll(filter).ToList();
        }

    }
}
