using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace RatherGood.SQLRG
{
    public static class SQLRGExtensions
    {

        /// <summary>
        /// Find the key in a Dictionary<string, object> and return the value converted to the type T. 
        /// Also works on enum 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictStringObj"></param>
        /// <param name="key"></param>
        /// <param name="outValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool GetProperty<T>(this Dictionary<string, object> dictStringObj, string key, out T outValue, T defaultValue = default)
        {
            outValue = defaultValue;


            if (dictStringObj.ContainsKey(key))
            {
                try
                {
                    //if enum
                    if (typeof(T).IsEnum)
                    {
                        if (string.IsNullOrEmpty((string)dictStringObj[key]))
                            return false;

                        outValue = (T)System.Enum.Parse(typeof(T), (string)dictStringObj[key], false); //TryParse won't work here
                        return true;

                    }

                    //if others
                    outValue = (T)System.Convert.ChangeType(dictStringObj[key], typeof(T));
                    return true;
                }
                catch (System.InvalidCastException)
                {
                    Debug.Log("Invaid Cast.");
                    return false;
                }

            }

            return false;
        }

        public static T ValueOrDefault<T>(this IDataReader rdr, string columnName, T defaultValue = default)
        {

            int fieldCount = rdr.FieldCount;

            if (fieldCount < 0)
                return defaultValue;

            int idx = rdr.GetOrdinal(columnName);

            if (idx < fieldCount)
            {
                return rdr.ValueOrDefault(idx, defaultValue);
            }

            return defaultValue;
        }

        //should be faster than by column name
        public static T ValueOrDefault<T>(this IDataReader rdr, int columnIndex, T defaultValue = default)
        {
            if (!rdr.IsDBNull(columnIndex))
                return (T)rdr[columnIndex];
            return defaultValue;
        }


        public static int SafeGetInt32(this IDataReader reader, string columnName, int defaultValue)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (!reader.IsDBNull(ordinal))
            {
                return reader.GetInt32(ordinal);
            }
            else
            {
                return defaultValue;
            }
        }

        public static List<Vector2JsonConv> ToListVector2JsonConv(this List<Vector2> inList)
        {
            List<Vector2JsonConv> retList = null;

            if (inList != null && inList.Count > 0)
            {
                retList = new List<Vector2JsonConv>();

                for (int i = 0; i < inList.Count; i++)
                {
                    retList.Add(new Vector2JsonConv(inList[i].x, inList[i].y));
                }
            }
            return retList;

        }

        public static List<Vector2> ToListVector2(this List<Vector2JsonConv> inList)
        {
            List<Vector2> retList = null;

            if (inList != null && inList.Count > 0)
            {
                retList = new List<Vector2>();

                for (int i = 0; i < inList.Count; i++)
                {
                    retList.Add(new Vector2(inList[i].x, inList[i].y));
                }
            }
            return retList;

        }



        public static List<DictStringObjItem> GetItemList(this Dictionary<string, object> dictStringObj)
        {
            List<DictStringObjItem> newList = new List<DictStringObjItem>();

            foreach (var pair in dictStringObj)
            {
                newList.Add(new DictStringObjItem(pair.Key, pair.Value));
            }
            return newList;
        }


        public static Dictionary<string, object> BuildDictStringObj(this List<DictStringObjItem> listStringObj)
        {
            Dictionary<string, object> dictStringObj = new Dictionary<string, object>();

            foreach (var listItem in listStringObj)
            {
                dictStringObj.Add(listItem.layerKey, listItem.valueObj);
            }
            return dictStringObj;


        }


    }

    [System.Serializable]
    public struct Vector2JsonConv
    {
        public float x;
        public float y;


        public Vector2JsonConv(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

    }

    [System.Serializable]
    public struct DictStringObjItem
    {
        public string layerKey;
        public string layerValue;
        public object valueObj;

        public DictStringObjItem(string layerKey, object valueObj)
        {
            this.layerKey = layerKey;
            this.valueObj = valueObj;

            if (valueObj != null)
                this.layerValue = valueObj.ToString();
            else
                this.layerValue = "";
        }

    }
}