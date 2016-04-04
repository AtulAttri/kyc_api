﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MySql.Data.MySqlClient;
using ecomm.util;
using ecomm.util.entities;
using Newtonsoft.Json;
using System.Configuration;

namespace ecomm.dal
{
    public class DataAccess
    {
        #region Private Variables
        private string _strConnection = "";
        private Boolean _IsConnected = false;
        private Boolean _UseAutoCommit = false;
        private MySqlConnection _Sql_Connect = null;
        private MySqlTransaction _Sql_Trans = null;
        private Boolean _ReadFromIni = true;
        private Boolean _ReadFromRegistry = false;
        private ConnectionState _ConnectionStatus;
        #endregion

        #region Property Variables
        public string strConnection
        {
            get { return _strConnection; }
            set { _strConnection = value; }
        }
        public Boolean IsConnected
        {
            get { return _IsConnected; }
            set { _IsConnected = value; }
        }
        public Boolean UseAutoCommit
        {
            get { return _UseAutoCommit; }
            set { _UseAutoCommit = value; }
        }
        public MySqlConnection Sql_Connect
        {
            get { return _Sql_Connect; }
            set { _Sql_Connect = value; }
        }
        public MySqlTransaction Sql_Trans
        {
            get { return _Sql_Trans; }
            set { _Sql_Trans = value; }
        }
        public Boolean ReadFromIni
        {
            get { return _ReadFromIni; }
            set { _ReadFromIni = value; }
        }
        public Boolean ReadFromRegistry
        {
            get { return _ReadFromRegistry; }
            set { _ReadFromRegistry = value; }
        }
        public ConnectionState ConnectionStatus
        {
            get
            {
                if (Sql_Connect != null)
                {
                    return Sql_Connect.State;
                }
                else
                {
                    return ConnectionState.Closed;
                }
            }
        }


        #endregion


        /// <summary>
        /// Initialization of Connection Object 
        /// </summary>
        /// <param name="UseAutoCommitTrans"></param>
        /// <param name="ReadFromIniFile"></param>
        /// <param name="ReadFromReg"></param>
        public void Init(Boolean UseAutoCommitTrans, Boolean ReadFromIniFile, Boolean ReadFromReg)
        {
            _UseAutoCommit = UseAutoCommitTrans;
            _ReadFromIni = ReadFromIniFile;
            _ReadFromRegistry = ReadFromReg;

            CheckandSetConnection();
        }
        public void Init()
        {
            CheckandSetConnection();
        }
        /// <summary>
        /// Dispose() - Close the Connection 
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (IsConnected == true)
                {
                    Sql_Connect.Close();
                }
            }
            catch (Exception ex)
            {

                ;
            }
        }
        #region ExecuteDataTable
        // <summary>
        // Executes a stored procedure and returns a DataTable with the results.
        // </summary>
        // <param name="storedProcedureName">Name of the stored procedure to execute</param>
        // <returns></returns>
        public DataTable ExecuteDataTable(string storedProcedureName)
        {
            return ExecuteDataTable(storedProcedureName, null);
        }

        // <summary>
        // Executes a stored procedure with parameters and returns a DataTable with the results.
        // </summary>
        // <param name="storedProcedureName">Name of the stored procedure to execute</param>
        // <param name="arrParam">Parameters required by the stored procedure</param>
        // <returns>DataTable containing the result</returns>
        // <remarks></remarks>
        public DataTable ExecuteDataTable(string storedProcedureName, params MySqlParameter[] arrParam)
        {
            DataTable dt;

            if (string.IsNullOrEmpty(storedProcedureName))
            {
                throw new ArgumentNullException("storedProcedureName");
            }

            // Check/the connection
            CheckandSetConnection();

            // Define the command
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.Connection = Sql_Connect;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storedProcedureName;

                // Handle the parameters
                if (arrParam != null)
                {
                    foreach (MySqlParameter param in arrParam)
                        cmd.Parameters.Add(param);
                }

                // Define the data adapter and fill the dataset
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    dt = new DataTable();
                    da.Fill(dt);
                }

                if (Sql_Connect != null)
                    if (Sql_Connect.State == ConnectionState.Open)
                        Sql_Connect.Close();

                Sql_Connect = null;
            }
            return dt;
        }

        public DataTable ExecuteSQL(string strSql)
        {
            DataTable dt;

            if (string.IsNullOrEmpty(strSql))
            {
                throw new ArgumentNullException("SQL");
            }

            // Check/the connection
            CheckandSetConnection();


            // Define the command
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.Connection = Sql_Connect;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = strSql;

                // Define the data adapter and fill the dataset
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    dt = new DataTable();
                    da.Fill(dt);
                }
            }

            if (Sql_Connect != null)
                if (Sql_Connect.State == ConnectionState.Open)
                    Sql_Connect.Close();

            Sql_Connect = null;
            return dt;
        }

        #endregion

        #region ExecuteNonQuery
        /// <summary>
        /// Executes a stored procedure that does not return a dataTable and returns the
        /// first output parameter.
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure to execute</param>
        /// <param name="arrParam">Parameters required by the stored procedure</param>
        /// <returns>Affected Row Count</returns>
        //public int ExecuteSP(string storedProcedureName, ref List<OutCollection> OutC, params MySqlParameter[] arrParam)
        //{

        //    if (string.IsNullOrEmpty(storedProcedureName))
        //    {
        //        throw new ArgumentNullException("storedProcedureName");
        //    }

        //    int retVal = 0;

        //    // Check/the connection
        //    CheckandSetConnection();

        //    // Define the command
        //    using (MySqlCommand cmd = new MySqlCommand())
        //    {
        //        cmd.Connection = Sql_Connect;
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.CommandText = storedProcedureName;

        //        // Handle the parameters
        //        if (arrParam != null)
        //        {
        //            foreach (MySqlParameter param in arrParam)
        //            {
        //                cmd.Parameters.Add(param);

        //                if (param.Direction == ParameterDirection.Output)
        //                {
        //                    OutCollection oc = new OutCollection();
        //                    oc.strParamName = param.ParameterName;
        //                    OutC.Add(oc);
        //                }
        //            }
        //        }

        //        // Execute the stored procedure
        //        retVal = cmd.ExecuteNonQuery();

        //        // Return the output parameter value
        //        if (OutC != null)
        //        {
        //            foreach (OutCollection ocl in OutC)
        //            {
        //                ocl.strParamValue = cmd.Parameters[ocl.strParamName].Value.ToString();
        //            }
        //        }
        //    }

        //    if (Sql_Connect != null && Sql_Connect.State == ConnectionState.Open)
        //        Sql_Connect.Close();

        //    Sql_Connect = null;
        //    return retVal;
        //}

        public int ExecuteSP(string storedProcedureName,  params MySqlParameter[] arrParam)
        {

            if (string.IsNullOrEmpty(storedProcedureName))
            {
                throw new ArgumentNullException("storedProcedureName");
            }

            int retVal = 0;

            // Check/the connection
            CheckandSetConnection();

            // Define the command
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.Connection = Sql_Connect;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storedProcedureName;

                // Handle the parameters
                if (arrParam != null)
                {
                    foreach (MySqlParameter param in arrParam)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                // Execute the stored procedure
                retVal = (int) cmd.ExecuteNonQuery();

                // Return the output parameter value

                if (Sql_Connect != null)
                    if (Sql_Connect.State == ConnectionState.Open)
                        Sql_Connect.Close();

                Sql_Connect = null;
            }
            return retVal;
        }
        /// <summary>
        /// ExecuteNonQuery
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string strSQL)
        {
            if (string.IsNullOrEmpty(strSQL))
            {
                throw new ArgumentNullException("SQL");
            }

            int retVal = 0;

            // Check/the connection
            CheckandSetConnection();

            // Define the command
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.Connection = Sql_Connect;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = strSQL;

                // Execute the stored procedure
                retVal = (int)cmd.ExecuteNonQuery();

            }

            if (Sql_Connect != null)
                if (Sql_Connect.State == ConnectionState.Open)
                    Sql_Connect.Close();

            Sql_Connect = null;
            return retVal;
        }
        #endregion

        /// <summary>
        /// StartTransaction - Initiate a Transaction
        /// </summary>
        /// <returns>Boolean</returns>
        public Boolean StartTransaction()
        {
            try
            {
                if (_UseAutoCommit == false)
                {
                    if (CheckandSetConnection() == true)
                    {
                        Sql_Trans = Sql_Connect.BeginTransaction();
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        /// <summary>
        /// CommitTransaction - Commit a Transaction
        /// </summary>
        /// <returns>Boolean</returns>
        public Boolean CommitTransaction()
        {
            try
            {
                if (_UseAutoCommit == false)
                {
                    if (IsConnected == true)
                    {
                        Sql_Trans.Commit();
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        /// <summary>
        /// RollbackTransaction - Rollback a Transaction
        /// </summary>
        /// <returns>Boolean</returns>
        public Boolean RollbackTransaction()
        {
            try
            {
                if (_UseAutoCommit == false)
                {
                    if (IsConnected == true)
                    {
                        Sql_Trans.Rollback();
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        #region Parameter
        // <summary>
        // Creates a Parameter
        // </summary>
        // <param name="parameterName">Name of the parameter</param>
        // <param name="parameterValue">Value of the parameter</param>
        // <returns>SqlParameter object</returns>
        // <remarks>The parameter name should be the same as the
        // proeprty name</remarks>
        public MySqlParameter Parameter(string parameterName, object parameterValue)
        {
            return Parameter(parameterName, parameterValue, false);
        }

        // <summary>
        // Creates a Parameter
        // </summary>
        // <param name="parameterName">Name of the parameter</param>
        // <param name="parameterValue">Value of the parameter</param>
        // <param name="isOutput">True if the parameter should be an output parameter</param>
        // <returns>SqlParameter object</returns>
        // <remarks>The parameter name should be the same as the
        // proeprty name</remarks>
        public MySqlParameter Parameter(string parameterName, object parameterValue, bool isOutput)
        {
            MySqlParameter param = new MySqlParameter();

            // the name
            param.ParameterName = parameterName;

            // the value
            param.Value = parameterValue ?? DBNull.Value;

            // the direction
            if (isOutput)
                param.Direction = ParameterDirection.Output;

            return param;
        }
        /// <summary>
        /// CheckandSetConnection - Check whether the connection is Active. If Not, then activate a connection
        /// </summary>
        /// <returns></returns>
        public Boolean CheckandSetConnection()
        {

            try
            {
                if (IsConnected == false)
                {
                    Sql_Connect = new MySqlConnection();
                    Sql_Connect.ConnectionString = GetConnectionstring();
                    Sql_Connect.Open();
                }

            }
            catch (Exception ex)
            {

                return false;
            }

            return true;
        }

        /// <summary>
        /// GetConnectionstring - Get the connection string from the INI or Registry
        /// </summary>
        /// <returns></returns>
        private string GetConnectionstring()
        {
            if (strConnection.Trim().Length > 0)
                return strConnection;

            try
            {
                strConnection = ConfigurationManager.ConnectionStrings["ecomm.ConnectionString"].ConnectionString;
            }
            catch (Exception ex)
            {
                return "";
            }
            return strConnection;
        }

        #endregion


        public enum db_types
        {
            ecomm = 0,
            catalog = 1
        }

        private db_connection getConnection(int ConType)
        {
            if (ConType == 0) // ecomm for mysql
            {
                return new db_connection
                {
                    connection_string = System.Configuration.ConfigurationManager.ConnectionStrings["ecomm.ConnectionString"].ConnectionString,
                    db = "ecomm"
                };
                //return System.Configuration.ConfigurationManager.ConnectionStrings["ecomm.ConnectionString"].ConnectionString;
            }
            else if (ConType == 1) // catalog for mongo
            {
                return new db_connection
                {
                    connection_string = System.Configuration.ConfigurationManager.AppSettings["mongo.connection_string"],
                    db = System.Configuration.ConfigurationManager.AppSettings["mongo.db"],
                   // connection_string = System.Configuration.ConfigurationManager.ConnectionStrings["bulk_grass.ConnectionString"].ConnectionString,
                   // db = "bulk_grass"
                };

            }

            else
                return null;

        }


        #region ExecuteMongoDBQueries
        /// <summary>
        /// executes a FindAll command and gets all documents from a collection.
        /// return a generic list of db_row
        /// </summary>
        /// <param name="collection_name">collection to be queried</param>
        /// <returns>First output parameter</returns>
        public MongoCursor execute_mongo_db(string collection_name)
        {
            var db = new db_connection();
            db = getConnection((int)db_types.catalog);
            var client = new MongoClient(db.connection_string);
            var server = client.GetServer();
            var database = server.GetDatabase(db.db);
            var collection = database.GetCollection(collection_name);
            MongoCursor cursor = collection.FindAll();
            return cursor;
        }

        //public MongoCursor execute_mongo_db(string collection_name, QueryDocument q)
        //{

        //    var db = new db_connection();
        //    db = getConnection((int)db_types.catalog);
        //    var client = new MongoClient(db.connection_string);
        //    var server = client.GetServer();
        //    var database = server.GetDatabase(db.db);
        //    var collection = database.GetCollection(collection_name);
        //    MongoCursor cursor = collection.Find(q);
        //    return cursor;
        //}
        public MongoCursor execute_mongo_db_sort(string collection_name, string Sort_By_Field)
        {

            var db = new db_connection();
            db = getConnection((int)db_types.catalog);
            var client = new MongoClient(db.connection_string);
            var server = client.GetServer();
            var database = server.GetDatabase(db.db);
            var collection = database.GetCollection(collection_name);
            MongoCursor cursor = collection.FindAll().SetSortOrder(Sort_By_Field);
            return cursor;
        }

        public MongoCursor execute_mongo_db(string collection_name, QueryDocument q)
        {

            var db = new db_connection();
            db = getConnection((int)db_types.catalog);
            var client = new MongoClient(db.connection_string);
            var server = client.GetServer();
            var database = server.GetDatabase(db.db);
            var collection = database.GetCollection(collection_name);
            MongoCursor cursor;
            cursor = null;

            cursor = collection.Find(q);
            return cursor;
        }
        public MongoCursor execute_mongo_db(string collection_name, QueryDocument q, string[] include_fields, string[] exclude_fields)
        {

            var db = new db_connection();
            db = getConnection((int)db_types.catalog);
            var client = new MongoClient(db.connection_string);
            var server = client.GetServer();
            var database = server.GetDatabase(db.db);
            var collection = database.GetCollection(collection_name);
            MongoCursor cursor;
            cursor = null;
            
            cursor = collection.Find(q).SetFields(Fields.Include(include_fields).Exclude(exclude_fields)).SetSortOrder(SortBy.Ascending("price.mrp", "price.discount"));
            return cursor;
        }
        public MongoCursor execute_mongo_db(string collection_name,  string[] include_fields, string[] exclude_fields)
        {

            var db = new db_connection();
            db = getConnection((int)db_types.catalog);
            var client = new MongoClient(db.connection_string);
            var server = client.GetServer();
            var database = server.GetDatabase(db.db);
            var collection = database.GetCollection(collection_name);
            MongoCursor cursor;
            cursor = null;

            cursor = collection.FindAll().SetFields(Fields.Include(include_fields).Exclude(exclude_fields)).SetSortOrder(SortBy.Ascending("price.mrp", "price.discount"));
            return cursor;
        }

        public MongoCursor execute_mongo_db(string collection_name, QueryDocument q, 
                                                string[] include_fields, string[] exclude_fields, 
                                                string[] ascending_sort_fields, string[] decending_sort_fields)
        {

            var db = new db_connection();
            db = getConnection((int)db_types.catalog);
            var client = new MongoClient(db.connection_string);
            var server = client.GetServer();
            var database = server.GetDatabase(db.db);
            var collection = database.GetCollection(collection_name);
            MongoCursor cursor;
            cursor = null;
            if ((ascending_sort_fields != null) && (decending_sort_fields != null))
            {
                cursor = collection.Find(q).SetFields(Fields.Include(include_fields).Exclude(exclude_fields)).SetSortOrder(SortBy.Ascending(ascending_sort_fields).Descending(decending_sort_fields));
            }
            else if (ascending_sort_fields != null)
            {
                cursor = collection.Find(q).SetFields(Fields.Include(include_fields).Exclude(exclude_fields)).SetSortOrder(SortBy.Ascending(ascending_sort_fields));
            }
            else if (decending_sort_fields != null)
            {
                cursor = collection.Find(q).SetFields(Fields.Include(include_fields).Exclude(exclude_fields)).SetSortOrder(SortBy.Descending(decending_sort_fields));
            }
            else // if both are null
            {
                cursor = collection.Find(q).SetFields(Fields.Include(include_fields).Exclude(exclude_fields));
            }
            return cursor;
        }	

        public string mongo_write(string collection_name, BsonDocument bd_object)
        {

            var db = new db_connection();
            db = getConnection((int)db_types.catalog);
            var client = new MongoClient(db.connection_string);
            var server = client.GetServer();
            var database = server.GetDatabase(db.db);
            var collection = database.GetCollection(collection_name);
            collection.Insert(bd_object);
            return (string)bd_object.ToString();
        }

        public string mongo_update(string collection_name, string whereclause, BsonDocument bd_object, MongoDB.Driver.Builders.UpdateBuilder update)
        {
           
            var db = new db_connection();
            db = getConnection((int)db_types.catalog);
            var client = new MongoClient(db.connection_string);
            var server = client.GetServer();
            var database = server.GetDatabase(db.db);
            var collection = database.GetCollection(collection_name);
            BsonDocument document = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(whereclause);
            QueryDocument queryDoc = new QueryDocument(document);
            //MongoDB.Driver.Builders.UpdateBuilder update = MongoDB.Driver.Builders.Update.Set("Name", "Updated Shoes");
            collection.Update(queryDoc,update);
            return (string)bd_object.ToString();
        }
        public BsonDocument mongo_find_and_modify(string collection_name, QueryDocument q, UpdateBuilder u)
        {

            var db = new db_connection();
            db = getConnection((int)db_types.catalog);
            var client = new MongoClient(db.connection_string);
            var server = client.GetServer();
            var database = server.GetDatabase(db.db);
            var collection = database.GetCollection(collection_name);
            var result = collection.FindAndModify(q,null, u, true); 
            return result.ModifiedDocument;
        }

        public string mongo_remove(string collection_name, string whereclause, BsonDocument bd_object)
        {

            var db = new db_connection();
            db = getConnection((int)db_types.catalog);
            var client = new MongoClient(db.connection_string);
            var server = client.GetServer();
            var database = server.GetDatabase(db.db);
            var collection = database.GetCollection(collection_name);
            BsonDocument document = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(whereclause);
            QueryDocument queryDoc = new QueryDocument(document);
            collection.Remove(queryDoc);
            return (string)bd_object.ToString();
        }
        public void mongo_remove(string collection_name, string whereclause)
        {

            var db = new db_connection();
            db = getConnection((int)db_types.catalog);
            var client = new MongoClient(db.connection_string);
            var server = client.GetServer();
            var database = server.GetDatabase(db.db);
            var collection = database.GetCollection(collection_name);
            BsonDocument document = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(whereclause);
            QueryDocument queryDoc = new QueryDocument(document);
            collection.Remove(queryDoc);
            //return (string)bd_object.ToString();
        }


    }
        # endregion
}
