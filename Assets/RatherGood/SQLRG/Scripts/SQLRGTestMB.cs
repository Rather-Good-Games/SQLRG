using Mono.Data.Sqlite;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RatherGood.SQLRG;
using System.Data;
using System.IO;
using System;

namespace RatherGood.SQLRG
{
    public class SQLRGTestMB : MonoBehaviour
    {

        [Header("DB read write test Stuff")]

        [SerializeField] SqlDatabaseRG<SqlRG_ExampleData> dataBase = new SqlDatabaseRG<SqlRG_ExampleData>();

        [Header("Write Me")]

        [InspectorButton(nameof(WriteDB))]
        [SerializeField] bool writeDB = false;
        void WriteDB()
        {
            dataBase.WriteData(dataToWrite);
        }

        [SerializeField] SqlRG_ExampleData dataToWrite = new SqlRG_ExampleData();

        [Header("Read Me")]

        [Header("Find Single Record by ID")]

        [SerializeField] string idToFind;


        [InspectorButton(nameof(ReadSingleRecordByID))]
        [SerializeField] bool findRecordByID = false;
        void ReadSingleRecordByID()
        {
            dataRead.Clear();
            dataRead.Add(dataBase.ReadData(idToFind));
        }

        [InspectorButton(nameof(ReadDB))]
        [SerializeField] bool readDB = false;
        void ReadDB()
        {
            dataRead = dataBase.ReadAllData();
        }

        [SerializeField] List<SqlRG_ExampleData> dataRead = new List<SqlRG_ExampleData>();






        [Header("ID to delete")]

        [SerializeField] string idToDelete;

        [InspectorButton(nameof(DeleteRecordByID))]
        [SerializeField] bool deleteRecordByID = false;
        void DeleteRecordByID()
        {
            dataBase.DeleteData(idToDelete);

            ReadDB();//refresh list
        }


    }
}