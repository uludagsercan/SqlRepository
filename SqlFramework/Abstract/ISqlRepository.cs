using SqlFramework.Concrete.SqlRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlFramework.Abstract
{
    public interface ISqlRepository<T> :IGenericRepository<T>
    {
    }
}
