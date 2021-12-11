using Dapper;
using HA.HALoG5BWService.DatabaseAccess.Contract;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HA.HALoG5BWService.DatabaseAccess.Impl
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, new()
    {
        private readonly string _connectionString;

        #region Constructor

        public GenericRepository(string config)
        {
            _connectionString = config;
        }

        #endregion Constructor

        #region Generic Methods

        /// <summary>
        /// Get Generic Object from database
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="storedProcedure"></param>
        /// <returns>Model Object</returns>
        public T Get(object parameters, string storedProcedure)
        {
            T model = null;
            using (IDbConnection connection = new MySqlConnection(_connectionString))
            { model = connection.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure).FirstOrDefault(); }

            return model;
        }

        /// <summary>
        /// Get dynamic type object from DB
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="storedProcedure"></param>
        /// <returns>Dynamic object</returns>
        public object GetValue(object parameters, string storedProcedure)
        {
            object model = null;
            using (IDbConnection connection = new MySqlConnection(_connectionString))
            { model = connection.Query(storedProcedure, parameters, commandType: CommandType.StoredProcedure).FirstOrDefault(); }

            return model;
        }

        /// <summary>
        /// Get list of object from db
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="storedProcedure"></param>
        /// <returns>List of Object</returns>
        public List<T> GetAll(object parameters, string storedProcedure)
        {
            List<T> listT = new List<T>();
            using (IDbConnection connection = new MySqlConnection(_connectionString))
            { listT = connection.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure).ToList(); }
            return listT;
        }

        /// <summary>
        /// Insert Values
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="storedProcedure"></param>
        /// <returns>Long value</returns>
        public virtual long Insert(object parameters, string storedProcedure)
        {
            long id = 0;
            using (IDbConnection connection = new MySqlConnection(_connectionString))
            { id = connection.ExecuteScalar<long>(storedProcedure, parameters, commandType: CommandType.StoredProcedure); }
            return id;
        }

        /// <summary>
        /// Insert Values
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="storedProcedure"></param>
        /// <returns>Object</returns>
        public T InsertEntity(object parameters, string storedProcedure)
        {
            T model = new T();
            using (IDbConnection connection = new MySqlConnection(_connectionString))
            { model = connection.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure).FirstOrDefault(); }
            return model;
        }

        /// <summary>
        /// Update Values
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="storedProcedure"></param>
        /// <returns>int Status</returns>
        public virtual int Update(object parameters, string storedProcedure)
        {
            int rowsAffected = 0;
            using (IDbConnection connection = new MySqlConnection(_connectionString))
            {
                rowsAffected = connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
            return rowsAffected;
        }

        /// <summary>
        /// Execute StoredProcedure
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="storedProcedure"></param>
        public virtual void ExecWithStoreProcedure(object parameters, string storedProcedure)
        {
            using (IDbConnection connection = new MySqlConnection(_connectionString))
            { connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure); }
        }

        #endregion Generic Methods
    }
}