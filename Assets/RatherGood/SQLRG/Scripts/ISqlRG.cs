using System.Collections;
using UnityEngine;

using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;

namespace RatherGood.SQLRG
{

    //
    /// <summary>
    /// Interface to read write a cass to/from a DB
    /// Haxx, Interfaces cannot ahve static data therefor need to create static data for fields to return for classes that use this as noted.
    /// See example
    /// </summary>
    public interface ISqlRG
    {


        /// <summary>
        /// Return static string to match DB tabe
        /// </summary>
        public string TABLE_NAME { get; }

        /// <summary>
        /// ID name in table usually first column name
        /// Example:
        /// public string ID_NAME => DbXref[0].column_name;
        /// </summary>
        public string ID_NAME { get; }

        public DBXref[] DbXref { get; }


        //EXAMPLE setup

        //public DBXref[] DbXref => dbXref;

        //static readonly DBXref[] dbXref = {
        //    new DBXref("ID", "TEXT PRIMARY KEY", typeof(string)),
        //    new DBXref("iAmAString", "TEXT", typeof(string)),
        //    new DBXref("iAmABool", "INTEGER", typeof(bool)),
        //    new DBXref("iAmAnInteger", "INTEGER", typeof(string)),
        //    new DBXref("iAmAfloat", "REAL", typeof(string)),
        //    new DBXref("myTestEnum", "TEXT", typeof(string)),
        //    new DBXref("iAmACustomStruct", "TEXT", typeof(string)),
        //};


        /// <summary>
        /// Pass in a 'Dictionary<string, object>' to set class values
        /// Read to get values converted to a 'Dictionary<string, object>'
        /// </summary>
        public Dictionary<string, object> DBData { get; set; }

    }



    [System.Serializable]
    public struct DBXref
    {
        public string column_name;
        public string data_type;
        public Type unityType;

        public DBXref(string column_name, string data_type, Type unityType)
        {
            this.column_name = column_name;
            this.data_type = data_type;
            this.unityType = unityType;
        }

    }
}