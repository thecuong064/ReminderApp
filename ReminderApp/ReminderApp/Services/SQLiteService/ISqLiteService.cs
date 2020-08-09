using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ReminderApp.Services.SQLiteService
{
    public interface ISqLiteService
    {
        #region Gets

        T GetWithChildren<T>(string primaryKey, bool isRecursive = false) where T : class, new();
        T Get<T>(string primarykey) where T : class, new();
        T Get<T>(Expression<Func<T, bool>> predicate) where T : class, new();
        IEnumerable<T> GetList<T>(Expression<Func<T, bool>> predicate) where T : class, new();
        IEnumerable<T> GetList<T>() where T : class, new();

        #endregion

        #region Inserts

        int Insert<T>(T obj);

        int InsertAll<T>(IEnumerable<T> list);

        void InsertWithChildren<T>(T obj, bool isRecursive = false);

        #endregion

        #region Updates

        int Update<T>(T obj);

        void UpdateWithChildren<T>(T obj);

        int UpdateAll<T>(IEnumerable<T> list);

        #endregion

        #region Deletes
    
        int Delete<T>(string id);

        void Delete<T>(T obj, bool isRecursive = false);

        int DeleteAll<T>();

        void DeleteAll<T>(IEnumerable<T> obj);

        #endregion
    }
}
