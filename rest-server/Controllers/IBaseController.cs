using System;
using System.Collections.Concurrent;

namespace rest_server.Controllers
{
    public interface IBaseController<TS,T>
    {
        protected internal T Get(TS id);
        protected internal ConcurrentDictionary<TS, T> GetAll();
        protected internal void Save(String[] param);
        protected internal void Update(TS id, String[] param);
        protected internal void Delete(TS id);
    }
}