using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using UnityEditor;
using System.Linq;
using SteveD.TJSON.Reader;

namespace SteveD.TJSON
{
    public static class TJSONParser
    {
        [Serializable]
        private class butts
        {
            public float butt = 0.5f;
        }

        [Serializable]
        private class farts : butts
        {
            public static string Butts = "BUtts";
            public string Poot = "fert";
            public List<butts> Counts = new List<butts> { new butts { butt = 0.25f } };
        }

        private static string testString = "{\n\".OBJECTTYPE\": \"TJSONParser+farts\",\n\"Poot\": \"asdf\",\n\"Counts\": {\n\".LISTTYPE\":\"TJSONParser+butts\",\"Items\":[{\n\".OBJECTTYPE\": \"TJSONParser+butts\",\n\"butt\": 0.25\n}]\n},\n\"butt\": 0.5\n}";

        [MenuItem("TJSONParserTestMenu/Test")]
        public static void Test()
        {
            var f = Parse(testString);

            Debug.Log(Encode(f));
        }

        public static object Parse(string data)
        {
            data = _fullyTrimString(data);

            return _parse(data);
        }

        private static object _parse(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;

            var dataType = TJSONReader.ReadType(data);

            switch (dataType)
            {
                case TJSONReader.JsonDataType.List:
                    return _parseList(data);
                case TJSONReader.JsonDataType.Object:
                    return _parseObject(data);
                case TJSONReader.JsonDataType.Bool:
                    return data.ToLower().StartsWith("true");
                case TJSONReader.JsonDataType.Int:
                    return int.Parse(data);
                case TJSONReader.JsonDataType.Float:
                    return float.Parse(data);
                case TJSONReader.JsonDataType.String:
                    return _parseString(data);
                case TJSONReader.JsonDataType.Quaternion:
                    return _parseQuaternion(data);
                case TJSONReader.JsonDataType.Vector3:
                    return _parseVector3(data);
                case TJSONReader.JsonDataType.Null:
                    return null;
                default:
                    Debug.LogError("Unknown Json Data Type");
                    return null;
            }
        }

        private static object _parseList(string data)
        {
            //chop off the {}
            data = data.Substring(1, data.Length - 2);

            Type listType = null;
            string listData = null;

            for (var i = 0; i < 2; i++)
            {
                data = TJSONReader.ReadNextField(data, out var propName, out var propData);
                if (propName == TJSONReader.LISTTYPE)
                {
                    var listTypeClassName = TJSONReader.ReadString(propData);
                    listType = Type.GetType(listTypeClassName);
                }
                else
                {
                    listData = propData;
                }
            }

            data = listData.Substring(1, listData.Length - 2);

            var listObj = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(listType));

            while (!string.IsNullOrEmpty(data))
            {
                data = TJSONReader.ReadNextValue(data, out var listItemData);
                listObj.Add(_parse(listItemData));
            }

            return listObj;
        }

        private static object _parseObject(string data)
        {
            data = data.Substring(1, data.Length - 2);

            var fields = new Dictionary<string, string>();

            while (!string.IsNullOrEmpty(data))
            {
                data = TJSONReader.ReadNextField(data, out var propName, out var propData);
                fields.Add(propName, propData);
            }

            if (!fields.ContainsKey(TJSONReader.OBJECTTYPE))
            {
                //not a parseable object
                return null;
            }

            var objectType = Type.GetType(TJSONReader.ReadString(fields[TJSONReader.OBJECTTYPE]));
            fields.Remove(TJSONReader.OBJECTTYPE);

            var newObject = Activator.CreateInstance(objectType);

            foreach (var kvp in fields)
            {
                objectType.GetField(kvp.Key).SetValue(newObject, _parse(kvp.Value));
            }

            return newObject;
        }

        private static object _parseString(string data)
        {
            return TJSONReader.ReadString(data);
        }

        private static Quaternion _parseQuaternion(string data)
        {
            data = data.Substring(2, data.Length - 4);
            var parts = data.Split(',');
            return new Quaternion(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
        }
        private static Vector3 _parseVector3(string data)
        {
            data = data.Substring(2, data.Length - 4);
            var parts = data.Split(',');
            return new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
        }

        private static object _parseNumber(string data)
        {
            if (data.Contains("."))
            {
                return float.Parse(data);
            }
            return int.Parse(data);
        }

        public static string Encode(object data)
        {
            if (data == null)
            {
                return "null";
            }

            if (data is float || data is int || data is string || data is bool)
            {
                return _encodeBasic(data);
            }

            if(data is Vector3 || data is Quaternion)
            {
                return _encodeStruct(data);
            }

            if (data is IList)
            {
                var listType = data.GetType().GetGenericArguments()[0];
                return _encodeList((IList)data, listType);
            }

            if (Attribute.IsDefined(data.GetType(), typeof(SerializableAttribute)))
            {
                return _encodeObject(data);
            }

            return "null";
        }

        private static string _encodeList(IList data, Type listType)
        {
            var dataAccumulator = new StringBuilder("{\"" + TJSONReader.LISTTYPE + "\":\"");
            dataAccumulator.Append(listType.ToString());
            dataAccumulator.Append("\",\"Items\":[");

            var first = true;

            foreach (var item in data)
            {
                if (!first)
                    dataAccumulator.Append(",");
                first = false;

                dataAccumulator.Append(Encode(item));
            }

            dataAccumulator.Append("]}");

            return dataAccumulator.ToString();
        }

        private static string _encodeObject(object data)
        {
            var dataAccumulator = new StringBuilder("{");

            var dataType = data.GetType();

            var dataFields = data.GetType().GetFields().Where(x => !x.IsStatic);

            //push the object type name for later parsing
            dataAccumulator.Append("\"" + TJSONReader.OBJECTTYPE + "\":\"");
            dataAccumulator.Append(data.GetType().ToString());
            dataAccumulator.Append("\"");

            foreach (var field in dataFields)
            {
                if (Attribute.IsDefined(field, typeof(NonSerializedAttribute)))
                    continue;//skip this property

                dataAccumulator.Append(",");

                dataAccumulator.Append("\"");
                dataAccumulator.Append(field.Name);
                dataAccumulator.Append("\":");
                dataAccumulator.Append(Encode(field.GetValue(data)));
            }

            dataAccumulator.Append("}");

            return dataAccumulator.ToString();
        }

        private static string _encodeBasic(object data)
        {
            return data == null ? "\"null\"" : (data is string ? ("\"" + data.ToString() + "\"") : data.ToString());
        }

        private static string _encodeStruct(object data){
            return "\"" + data.ToString() + "\"";
        }

        private static string _fullyTrimString(string data)
        {
            var stringBuilder = new StringBuilder();

            var inQuotes = false;
            var currentQuoteChar = '\"';
            var escape = false;
            foreach (var character in data)
            {
                if (Char.IsWhiteSpace(character) && (!inQuotes || escape))
                {
                    //don't push char
                    continue;
                }

                if (escape)
                {
                    escape = false;
                }
                else if (character == '\\')
                {
                    escape = true;
                }
                else if (!inQuotes && (character == '\"' || character == '\''))
                {
                    inQuotes = true;
                    currentQuoteChar = character;
                }
                else if (inQuotes && character == currentQuoteChar)
                {
                    inQuotes = false;
                }

                stringBuilder.Append(character);
            }

            return stringBuilder.ToString();
        }
    }
}