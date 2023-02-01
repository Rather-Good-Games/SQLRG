using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Data.Sqlite;
using UnityEngine;
using System.Data;
using System.IO;
using UnityEditor;

namespace RatherGood.SQLRG
{

    [System.Serializable]
    public class SqlDatabaseRG<T> where T : ISqlRG, new()
    {
        [Tooltip("Database file name")]
        [SerializeField] private string dbFileName = "DBTestMe.sqlite";

        [Tooltip("directory: Currently hardcoded to inside the 'streamingAssets' path.")]
        [SerializeField] private string dbPathDirectory = "SQLRG";

        public IDbConnection dbConnection;

        //Keep a dummy copy to read static data? Cheating?
        T dataDummy = new T();


        protected void InitAndOpen()
        {
            string dbSonnectionString = "URI=file:" + Path.Combine(Application.streamingAssetsPath, dbPathDirectory, dbFileName);
            dbConnection = new SqliteConnection(dbSonnectionString);
            dbConnection.Open();
        }


        protected void CloseDB()
        {
            if (dbConnection != null)
                dbConnection.Close();
        }


        public void WriteData(T dataToWrite)
        {
            InitAndOpen();

            ExecuteCommand(GetCreateTableSqlCmd()); //Make tabe if doesn't exist

            string writeSql = GetWriteRowSqlCmdWArgs(dataToWrite, out SqliteParameter[] args);

            if (!string.IsNullOrEmpty(writeSql))
                ExecuteCommand(writeSql, args);

            CloseDB();


        }


        public void WriteData(T[] dataToWrite)
        {
            if (dataToWrite == null || dataToWrite.Length == 0)
                return;

            InitAndOpen();

            ExecuteCommand(GetCreateTableSqlCmd()); //Make tabe if doesn't exist

            string writeSql;

            foreach (var nextData in dataToWrite)
            {

                writeSql = GetWriteRowSqlCmdWArgs(nextData, out SqliteParameter[] args);

                if (!string.IsNullOrEmpty(writeSql))
                    ExecuteCommand(writeSql, args);
            }

            CloseDB();


        }


        public List<T> ReadAllData()
        {
            InitAndOpen();

            System.Data.IDataReader reader = GetAllTableData();

            T readData;

            List<T> retData = new List<T>();

            while (reader.Read())
            {
                readData = new T();

                readData.DBData = GetValuesFromRow(reader);

                retData.Add(readData);
            }

            return retData;

        }

        public T ReadData(string id)
        {
            InitAndOpen();

            System.Data.IDataReader reader = GetAllTableData(id);

            T readData = new T();

            while (reader.Read())
            {
                readData.DBData = GetValuesFromRow(reader);

                break; //THERE CAN BE ONLY ONE!
            }

            return readData;

        }

        public Dictionary<string, object> GetValuesFromRow(IDataReader reader)
        {
            int fieldCount = reader.FieldCount;

            string key;
            object valueObj;

            Dictionary<string, object> newDict = new Dictionary<string, object>();

            for (int i = 0; i < fieldCount; i++)
            {
                key = reader.GetName(i);
                valueObj = reader.GetValue(i);
                newDict.Add(key, valueObj);
            }

            return newDict;

        }

        public void DeleteData(string id)
        {
            InitAndOpen();
            ExecuteCommand(GetDeleteRecordSqlCmd(id)); //Make table if doesn't exist   
            CloseDB();
        }


        protected int ExecuteCommand(string sql, params SqliteParameter[] args)
        {
            int numRows = 0;

            IDbCommand dbcmd = dbConnection.CreateCommand();

            dbcmd.CommandText = sql;

            foreach (SqliteParameter arg in args)
            {
                dbcmd.Parameters.Add(arg);
            }

            try
            {
                numRows = dbcmd.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                Debug.Log("ExecuteCommand() failed:" + ex);
            }

            return numRows;

        }

        protected IDataReader GetAllTableData()
        {
            IDbCommand dbcmd = dbConnection.CreateCommand();
            dbcmd.CommandText = "SELECT * FROM " + dataDummy.TABLE_NAME;
            IDataReader reader = dbcmd.ExecuteReader();
            return reader;
        }

        protected IDataReader GetAllTableData(string keyValue)
        {
            IDbCommand dbcmd = dbConnection.CreateCommand();
            dbcmd.CommandText = "SELECT * FROM " + dataDummy.TABLE_NAME + " WHERE " + dataDummy.ID_NAME + " = '" + keyValue + "'";
            return dbcmd.ExecuteReader();
        }


        protected IDataReader ExecuteCommandGetReaderResults(string sql)
        {
            IDbCommand dbcmd = dbConnection.CreateCommand();
            dbcmd.CommandText = sql;
            return dbcmd.ExecuteReader();
        }



        //save commmon string for later queries.
        private static string createTableSqlCmd;

        protected string GetCreateTableSqlCmd()
        {

            if (string.IsNullOrEmpty(createTableSqlCmd))
            {
                createTableSqlCmd = "CREATE TABLE IF NOT EXISTS " + dataDummy.TABLE_NAME + " (";

                for (int i = 0; i < dataDummy.DbXref.Length; i++)
                {
                    createTableSqlCmd += dataDummy.DbXref[i].column_name + " " + dataDummy.DbXref[i].data_type;
                    if (i < dataDummy.DbXref.Length - 1)
                        createTableSqlCmd += ", ";
                }

                createTableSqlCmd += ")";
            }

            return createTableSqlCmd;

        }



        protected string GetFindRecordSqlCmd(string id)
        {
            return "SELECT * FROM " + dataDummy.TABLE_NAME + " WHERE " + dataDummy.ID_NAME + " = '" + id + "'" + " LIMIT 1";

        }

        protected string GetDeleteRecordSqlCmd(string id)
        {
            return "DELETE FROM " + dataDummy.TABLE_NAME + " WHERE " + dataDummy.ID_NAME + " = '" + id + "'";
        }




        protected string GetWriteRowSqlCmdWArgs(T data, out SqliteParameter[] args)
        {
            return GetWriteRowSqlCmdWArgs(data.DBData, out args);

        }


        //Save garbage collection
        List<string> tempBuildWriteCmd = new List<string>();

        List<SqliteParameter> tempArgsOut = new List<SqliteParameter>();

        string tempWritePartialRowSqlCmd;

        protected string GetWriteRowSqlCmdWArgs(Dictionary<string, object> vtfDict, out SqliteParameter[] args)
        {

            int index;

            //Only write values from included dictionary
            foreach (var pair in vtfDict)
            {
                index = Array.FindIndex(dataDummy.DbXref, x => x.column_name == pair.Key);

                if (index >= 0)
                {
                    tempBuildWriteCmd.Add(dataDummy.DbXref[index].column_name);
                    tempArgsOut.Add(new SqliteParameter("@" + dataDummy.DbXref[index].column_name, Convert.ChangeType(pair.Value, dataDummy.DbXref[index].unityType)));
                }
                else
                {
                    Debug.Log("GetWriteRowSqlCmdWArgs(): " + pair.Key + " Not in DB");
                }
            }

            tempWritePartialRowSqlCmd = "INSERT OR REPLACE INTO " + dataDummy.TABLE_NAME + " (";

            for (int i = 0; i < tempBuildWriteCmd.Count; i++)
            {
                tempWritePartialRowSqlCmd += tempBuildWriteCmd[i];
                if (i < tempBuildWriteCmd.Count - 1)
                    tempWritePartialRowSqlCmd += ", ";
            }

            tempWritePartialRowSqlCmd += ") VALUES (";

            for (int i = 0; i < tempBuildWriteCmd.Count; i++)
            {
                tempWritePartialRowSqlCmd += "@" + tempBuildWriteCmd[i];
                if (i < tempBuildWriteCmd.Count - 1)
                    tempWritePartialRowSqlCmd += ", ";
            }

            tempWritePartialRowSqlCmd += ")";

            args = tempArgsOut.ToArray();

            return tempWritePartialRowSqlCmd;
        }





    }


}