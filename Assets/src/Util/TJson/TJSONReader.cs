using System.Text;

namespace SteveD.TJSON.Reader
{
    internal static class TJSONReader
    {
        internal static readonly string OBJECTTYPE = ".OBJECTTYPE";
        internal static readonly string LISTTYPE = ".LISTTYPE";

        internal enum JsonDataType
        {
            List,
            Object,
            String,
            Bool,
            Int,
            Float,
            Vector3,
            Quaternion,
            Null,
            UNKNOWN
        }

        internal static JsonDataType ReadType(string inputData)
        {
            if (inputData.StartsWith("{\""+LISTTYPE+"\":") || inputData.StartsWith("["))
            {
                return JsonDataType.List;
            }

            if (inputData[0] == '{')
            {
                return JsonDataType.Object;
            }

            if (inputData.StartsWith("\"("))
            {
                var inputtxt = ReadString(inputData);
                var sections = inputtxt.Split(',');
                if (sections.Length == 3)
                {
                    return JsonDataType.Vector3;
                }
                else if(sections.Length == 4)
                {
                    return JsonDataType.Quaternion;
                }
            }

            if (inputData[0] == '\"' || inputData[0] == '\'')
            {
                return JsonDataType.String;
            }

            if (inputData.ToLower().StartsWith("true") || inputData.ToLower().StartsWith("false"))
            {
                return JsonDataType.Bool;
            }

            if (inputData.ToLower().StartsWith("null"))
            {
                return JsonDataType.Null;
            }

            var dotIndex = inputData.Contains(".") ? inputData.IndexOf('.') : int.MaxValue;
            var commaIndex = inputData.Contains(",") ? inputData.IndexOf(',') : int.MaxValue;

            if((dotIndex == int.MaxValue && commaIndex == int.MaxValue) ||
                dotIndex > commaIndex)
            {
                if(int.TryParse(inputData, out var r))
                {
                    return JsonDataType.Int;
                }
            }
            else if(commaIndex > dotIndex)
            {
                if(float.TryParse(inputData, out var r))
                {
                    return JsonDataType.Float;
                }
            }

            return JsonDataType.UNKNOWN;
        }

        internal static string ReadNextField(string inputData, out string propName, out string propData)
        {
            propName = ReadString(inputData);

            inputData = inputData.Substring(propName.Length + 3);

            inputData = ReadNextValue(inputData, out propData);

            return inputData;
        }

        internal static string ReadNextValue(string inputData, out string valueData)
        {
            var dataType = ReadType(inputData);

            if(dataType == JsonDataType.Object)
            {
                valueData = "{" + ReadObject(inputData) + "}";
            }
            else if(dataType == JsonDataType.List)
            {
                if (inputData.StartsWith("{\""+ LISTTYPE + "\":"))
                {
                    valueData = "{" + ReadObject(inputData) + "}";
                }
                else
                {
                    valueData = "[" + ReadArray(inputData) + "]";
                }
            }
            else if(dataType == JsonDataType.String || dataType == JsonDataType.Vector3 || dataType == JsonDataType.Quaternion)
            {
                valueData = "\"" + ReadString(inputData) + "\"";
            }
            else
            {
                if (inputData.Contains(","))
                {
                    valueData = inputData.Substring(0, inputData.IndexOf(','));
                }
                else
                {
                    valueData = inputData;
                }
            }

            inputData = inputData.Substring(valueData.Length);
            if (inputData.Length > 0 && inputData[0] == ',')
            {
                inputData = inputData.Substring(1);
            }

            return inputData;
        }

        internal static string ReadArray(string inputData)
        {
            inputData = inputData.Substring(1);

            var stringBuilder = new StringBuilder();

            var inQuotes = false;
            var currentQuoteChar = '\"';
            var escape = false;
            var flatCount = 1;
            foreach (var character in inputData)
            {
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
                else if (!inQuotes && character == '[')
                {
                    flatCount++;
                }
                else if (!inQuotes && character == ']')
                {
                    flatCount--;
                    if (flatCount == 0)
                    {
                        break;
                    }
                }

                stringBuilder.Append(character);
            }

            return stringBuilder.ToString();
        }

        internal static string ReadObject(string inputData)
        {
            inputData = inputData.Substring(1);

            var stringBuilder = new StringBuilder();

            var inQuotes = false;
            var currentQuoteChar = '\"';
            var escape = false;
            var curlyCount = 1;
            foreach (var character in inputData)
            {
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
                else if(!inQuotes && character == '{')
                {
                    curlyCount++;
                }
                else if(!inQuotes && character == '}')
                {
                    curlyCount--;
                    if(curlyCount == 0)
                    {
                        break;
                    }
                }

                stringBuilder.Append(character);
            }

            return stringBuilder.ToString();
        }

        internal static string ReadString(string inputData)
        {
            var quoteType = inputData[0];
            var stringBuilder = new StringBuilder();
            var escape = false;

            inputData = inputData.Substring(1);

            foreach(var character in inputData)
            {
                if (escape)
                {
                    escape = false;
                }
                else if (character == '\\')
                {
                    escape = true;
                }
                else if (character == quoteType)
                {
                    break;
                }

                stringBuilder.Append(character);
            }

            return stringBuilder.ToString();
        }
    }

}