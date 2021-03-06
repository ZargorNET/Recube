using System;
using System.Collections.Generic;
using System.Threading;
using Recube.Api.Entities;

namespace Recube.Core.Entities
{
    public class EntityRegistry : IEntityRegistry
    {
        private readonly Dictionary<int, IEntity> _dictionary = new Dictionary<int, IEntity>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private int _lastId;

        public IEntity RegisterEntity(Func<int, IEntity> func)
        {
            var id = GetFreeId();

            var ent = func.Invoke(id);

            _lock.EnterWriteLock();
            try
            {
                _dictionary.TryAdd(id, ent);
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            return ent;
        }

        public IEntity? DeregisterEntity(int id)
        {
            IEntity ent;
            _lock.EnterWriteLock();
            try
            {
                _dictionary.Remove(id, out ent);
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            return ent;
        }

        public IEntity? GetEntityById(int id)
        {
            _lock.EnterReadLock();
            try
            {
                return _dictionary.GetValueOrDefault(id);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        private int GetFreeId()
        {
            _lock.EnterWriteLock();
            try
            {
                var started = _lastId;
                while (true)
                {
                    var id = ++_lastId;
                    if (started == id) throw new InvalidOperationException("All ids occupied");

                    var ent = _dictionary.GetValueOrDefault(id);
                    if (ent == null)
                        return id;
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}