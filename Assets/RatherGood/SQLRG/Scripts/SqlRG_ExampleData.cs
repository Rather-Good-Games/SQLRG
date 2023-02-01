using RatherGood.SQLRG;
using System.Collections;
using UnityEngine;

using Mono.Data.Sqlite;
using System.Data;
using System;

using System.Collections.Generic;
using Newtonsoft.Json;

namespace RatherGood.SQLRG
{
    [System.Serializable]
    public class SqlRG_ExampleData : ISqlRG
    {



        //MyData
        public string id;

        public string iAmAString = "YesIAmAString";

        public bool iAmABool = true;

        public int iAmAnInteger = 1234;

        public float iAmAfloat = 1.234f;

        public MyTestEnum myTestEnum = MyTestEnum.none;

        public IAmACustomStruct iAmACustomStruct;


        public enum MyTestEnum
        {
            none = 0,
            one = 1,
            two = 2,
            three = 3,

        }

        [System.Serializable]
        public struct IAmACustomStruct
        {
            public string iAmAString;

            public bool iAmABool;

            public int iAmAnInteger;

        }


        //Implement Interface

        public Dictionary<string, object> DBData
        {
            get
            {
                //Build dict and return
                Dictionary<string, object> retData = new Dictionary<string, object>();

                retData["ID"] = id;
                retData["iAmAString"] = iAmAString;
                retData["iAmABool"] = iAmABool;
                retData["iAmAnInteger"] = iAmAnInteger;
                retData["iAmAfloat"] = iAmAfloat;
                retData["myTestEnum"] = myTestEnum;

                retData["iAmACustomStruct"] = JsonConvert.SerializeObject(iAmACustomStruct);

                return retData;

            }
            set
            {
                //Assign values from dict
                //
                value.GetProperty("ID", out id, "");
                value.GetProperty("iAmAString", out iAmAString, "");
                value.GetProperty("iAmABool", out iAmABool, false);
                value.GetProperty("iAmAnInteger", out iAmAnInteger, 0);
                value.GetProperty("iAmAfloat", out iAmAfloat, 0.0f);

                value.GetProperty("myTestEnum", out myTestEnum, MyTestEnum.none);

                if (value.GetProperty("iAmACustomStruct", out string iAmACustomStructString, ""))
                {
                    iAmACustomStruct = JsonConvert.DeserializeObject<IAmACustomStruct>(iAmACustomStructString);
                }

            }

        }


        public string TABLE_NAME => "Data_EXAMPLE";

        public DBXref[] DbXref => dbXref;

        public string ID_NAME => DbXref[0].column_name;



        //Static ref, only need tto buid this once
        static readonly DBXref[] dbXref = {
            new DBXref("ID", "TEXT PRIMARY KEY", typeof(string)),

            new DBXref("iAmAString", "TEXT", typeof(string)),

            new DBXref("iAmABool", "INTEGER", typeof(bool)),

            new DBXref("iAmAnInteger", "INTEGER", typeof(string)),

            new DBXref("iAmAfloat", "REAL", typeof(string)),

            new DBXref("myTestEnum", "TEXT", typeof(string)),

            new DBXref("iAmACustomStruct", "TEXT", typeof(string)),

        };





    }




}




