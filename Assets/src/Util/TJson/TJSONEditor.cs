using System.Collections;
using UnityEngine;
using System;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SteveD.TJSON
{
    [CustomEditor(typeof(TJSON))]
    [CanEditMultipleObjects]
    public class TJSONEditor : Editor
    {
        private static Dictionary<Type, List<Type>> SubTypesCache = new Dictionary<Type, List<Type>>();

        private TJSON tjsonFile;
        private object objectToEdit;

        private bool objectDirty;

        private void OnEnable()
        {
            tjsonFile = (TJSON)target;
            serializedObject.Update();
            tjsonFile.Data = serializedObject.FindProperty("Data").stringValue;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (!_initialize()) return;

            _renderData(objectToEdit, objectToEdit.GetType());

            _updateFile();
        }

        private bool _initialize()
        {
            if (tjsonFile == null) return false;
            if (objectToEdit == null)
            {
                if (string.IsNullOrEmpty(tjsonFile.Data))
                {
                    objectToEdit = _renderNullable(null, typeof(ISerializableRoot));
                    if(objectToEdit != null)
                        tjsonFile.Data = TJSONParser.Encode(objectToEdit);
                }
                else
                {
                    objectToEdit = TJSONParser.Parse(tjsonFile.Data);
                }

                if (objectToEdit == null) return false;
            }

            objectDirty = false;

            return true;
        }

        private void _updateFile()
        {
            if (objectDirty)
            {
                serializedObject.Update();
                tjsonFile.Data = TJSONParser.Encode(objectToEdit);
                serializedObject.FindProperty("Data").stringValue = tjsonFile.Data;
                EditorUtility.SetDirty(tjsonFile);
                AssetDatabase.SaveAssets();
            }
        }

        private object _renderNullable(object data, Type type)
        {
            if (!SubTypesCache.ContainsKey(type))
            {
                SubTypesCache[type] = new List<Type>() { null };
                SubTypesCache[type].AddRange(AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                    .Where(x => type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract));
            }

            var subTypeCache = SubTypesCache[type];

            var serializableRootOptions = new string[subTypeCache.Count];

            for(var i = 0; i < subTypeCache.Count; i++)
            {
                var subType = subTypeCache[i];
                if(subType == null)
                {
                    serializableRootOptions[i] = "Select Object";
                }
                else
                {
                    serializableRootOptions[i] = subType.ToString();
                }
            }

            var currentType = subTypeCache.IndexOf(data == null ? null : data.GetType());

            var selectedType = EditorGUILayout.Popup("Object Type:", currentType, serializableRootOptions);
            if (selectedType != currentType)
            {
                return Activator.CreateInstance(subTypeCache[selectedType]);
            }

            return null;
        }

        private object _renderData(object data, Type dataType)
        {

            if (dataType.IsEquivalentTo(typeof(float)))
            {
                return _renderFloat(data, dataType);
            }

            else if (dataType.IsEquivalentTo(typeof(int)))
            {
                return _renderInt(data, dataType);
            }

            else if (dataType.IsEquivalentTo(typeof(string)))
            {
                return _renderString(data, dataType);
            }

            else if (dataType.IsEquivalentTo(typeof(bool)))
            {
                return _renderBool(data, dataType);
            }

            else if (dataType.IsEquivalentTo(typeof(Vector3)))
            {
                return _renderVector3(data, dataType);
            }

            else if (dataType.IsEquivalentTo(typeof(Quaternion)))
            {
                return _renderQuaternion(data, dataType);
            }

            else if (typeof(IList).IsAssignableFrom(dataType))
            {
                return _renderList(data, dataType);
            }

            else if (Attribute.IsDefined(dataType, typeof(SerializableAttribute)) || ((dataType.IsAbstract || dataType.IsInterface) && data == null))
            {
                return _renderObject(data, dataType);
            }

            else
            {
                return _renderError(dataType);
            }
        }

        private object _renderFloat(object data, Type type)
        {
            return EditorGUILayout.FloatField((float)data); ;
        }
        private object _renderInt(object data, Type type)
        {
            return EditorGUILayout.IntField((int)data);
        }
        private object _renderBool(object data, Type type)
        {
            return EditorGUILayout.Toggle((bool)data);
        }
        private object _renderString(object data, Type type)
        {
            return EditorGUILayout.TextField((string)data);
        }
        private object _renderVector3(object data, Type type)
        {
            return EditorGUILayout.Vector3Field("", (Vector3)data);
        }
        private object _renderQuaternion(object data, Type type)
        {
            return Maths.V42Q(EditorGUILayout.Vector4Field("", Maths.Q2V4((Quaternion)data)));
        }
        private object _renderList(object data, Type type)
        {
            if (data == null)
            {
                var listGenericType = typeof(List<>).MakeGenericType(type.GenericTypeArguments);
                data = Activator.CreateInstance(listGenericType);
                objectDirty = true;
            }

            var dataList = (IList)data;
            var listDataType = type.GenericTypeArguments[0];

            GUILayout.BeginVertical("GroupBox");

            for (var i = 0; i < dataList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("" + i);

                var inline = _isInlineType(listDataType);

                if (inline)
                {
                    dataList[i] = _newValueRender(dataList[i], listDataType);
                }

                if (GUILayout.Button("(remove)"))
                {
                    EditorGUILayout.EndHorizontal();
                    objectDirty = true;
                    dataList.RemoveAt(i);
                    i--;
                }
                else
                {
                    EditorGUILayout.EndHorizontal();

                    if (!inline)
                    {
                        dataList[i] = _newValueRender(dataList[i], dataList[i] == null ? listDataType : dataList[i].GetType());
                    }
                }

            }

            if (GUILayout.Button("(new)"))
            {
                if(listDataType.IsAbstract || listDataType.IsInterface)
                {
                    dataList.Add(null);
                }
                else
                {
                    dataList.Add(Activator.CreateInstance(listDataType));
                }
            }

            GUILayout.EndVertical();

            return data;
        }
        private object _renderObject(object data, Type dataType)
        {
            //allow user to switch object type

            GUILayout.BeginVertical("HelpBox");

            var dataSwitch = _renderNullable(data, dataType);

            if(dataSwitch != null)
            {
                data = dataSwitch;
                objectDirty = true;
            }

            if (data == null)
            {
                EditorGUILayout.LabelField("null");

                GUILayout.EndVertical();
                return data;
            }

            var dataFields = data.GetType().GetFields().Where(x => !x.IsStatic);

            foreach (var field in dataFields)
            {
                if (Attribute.IsDefined(field, typeof(NonSerializedAttribute)))
                    continue;//skip this property

                var inline = _isInlineType(field.FieldType);

                if (inline)
                {
                    EditorGUILayout.BeginHorizontal();
                }

                EditorGUILayout.LabelField(field.Name + ":");

                field.SetValue(data, _newValueRender(field.GetValue(data), field.FieldType));

                if (inline)
                {
                    EditorGUILayout.EndHorizontal();
                }
            }

            GUILayout.EndVertical();

            return data;
        }
        private object _renderError(Type type)
        {
            EditorGUILayout.LabelField("Error, " + type.ToString() + " not encodable");
            return null;
        }

        private object _newValueRender(object oldValue, Type dataType)
        {
            var newValue = _renderData(oldValue, dataType);
            if (!object.Equals(oldValue,newValue))
            {
                objectDirty = true;
                return newValue;
            }
            return oldValue;
        }

        private bool _isInlineType(Type dataType)
        {
            return dataType.IsEquivalentTo(typeof(float)) ||
                dataType.IsEquivalentTo(typeof(int)) ||
                dataType.IsEquivalentTo(typeof(string)) ||
                dataType.IsEquivalentTo(typeof(bool)) ||
                dataType.IsEquivalentTo(typeof(Vector3)) ||
                dataType.IsEquivalentTo(typeof(Quaternion));
        }
    }
}