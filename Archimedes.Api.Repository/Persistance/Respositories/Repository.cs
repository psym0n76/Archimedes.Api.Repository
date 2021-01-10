using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace Archimedes.Api.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        
        protected readonly DbContext Context;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly object LockingObject = new object();

        public Repository(DbContext context)
        {
            Context = context;
            CheckConnection();
        }


        public void CheckConnection()
        {
            try
            {
                lock (LockingObject)
                {
                    const int retry = 20;
                    var retryCounter = 0;

                    while (!Context.Database.CanConnect() && retryCounter < retry)
                    {
                        retryCounter++;
                        Thread.Sleep(5000);
                        Logger.Warn($"Database connection denied - retry {retryCounter} out of {retry}");
                    }

                    Logger.Info($"Database connection success - retry {retryCounter} out of {retry}");
                }
            }
            catch (Exception e)
            {
                Logger.Error(ErrorContent(e));
            }
        }

        public TEntity Get(long id)
        {
            try
            {
                return Context.Set<TEntity>().Find(id);
            }
            catch (Exception e)
            {
                return ErrorMessage(e);
            }
        }

        public IEnumerable<TEntity> GetAll()
        {
            // Note that here I've repeated Context.Set<TEntity>() in every method and this is causing
            // too much noise. I could get a reference to the DbSet returned from this method in the 
            // constructor and store it in a private field like _entities. This way, the implementation
            // of our methods would be cleaner:
            // 
            // _entities.ToList();
            // _entities.Where();
            // _entities.SingleOrDefault();
            // 
            // I didn't change it because I wanted the code to look like the videos. But feel free to change
            // this on your own.

            try
            {
                return Context.Set<TEntity>().ToList();
            }
            catch (Exception e)
            {
                return ErrorMessages(e);
            }

        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return Context.Set<TEntity>().Where(predicate);
            }
            catch (Exception e)
            {
                return ErrorMessages(e);
            }
        }



        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return Context.Set<TEntity>().SingleOrDefault(predicate);
            }
            catch (Exception a)
            {
                return ErrorMessage(a);
            }

        }

        public void Add(TEntity entity)
        {
            try
            {
                Context.Set<TEntity>().Add(entity);
            }
            catch (Exception e)
            {
                ErrorMessages(e);
            }
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            try
            {
                Context.Set<TEntity>().AddRange(entities);
            }
            catch (Exception e)
            {
                ErrorMessage(e);
            }
        }

        public void Remove(TEntity entity)
        {
            try
            {
                Context.Set<TEntity>().Remove(entity);
            }
            catch (Exception e)
            {
                ErrorMessage(e);
            }
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            try
            {
                Context.Set<TEntity>().RemoveRange(entities);
            }
            catch (Exception e)
            {
                ErrorMessage(e);
            }
        }

        public void Truncate()
        {
            try
            {
                // hack adding s to end of table name
                Context.Database.ExecuteSqlRaw($"TRUNCATE TABLE {typeof(TEntity).Name}s");
            }
            catch (Exception e)
            {
                ErrorMessage(e);
            }
        }

        private static IEnumerable<TEntity> ErrorMessages(Exception e)
        {
            Logger.Error(ErrorContent(e));
            return default;
        }

        private static string ErrorContent(Exception e)
        {
            return $"Error from Repository \n\nErrorMessage: {e.Message} \n\nStackTrace: {e.StackTrace}";
        }

        private static TEntity ErrorMessage(Exception e)
        {
            Logger.Error(ErrorContent(e));
            return default;
        }
    }
}