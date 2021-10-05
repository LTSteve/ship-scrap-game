using SteveD.TJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

[CustomEditor(typeof(ShipBlueprintManager))]
[CanEditMultipleObjects]
public class ShipBlueprintManagerEditor : Editor
{
    SerializedProperty BlueprintTJSON;
    SerializedProperty AutoSave;

    private bool recalculate = false;

    private void OnEnable()
    {
        BlueprintTJSON = serializedObject.FindProperty("BlueprintTJSON");
        AutoSave = serializedObject.FindProperty("AutoSave");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //EditorGUILayout.PropertyField(BlueprintTJSON);

        if(((ShipBlueprintManager)target).BlueprintTJSON != null)
        {
            AutoSave.boolValue = EditorGUILayout.Toggle("AutoSave", AutoSave.boolValue);

            if (!AutoSave.boolValue)
            {
                if (GUILayout.Button("Save"))
                {
                    ((ShipBlueprintManager)target).SaveBlueprint();
                }
            }
        }

        EditorGUILayout.BeginHorizontal();

        var currentValue = (TJSON)BlueprintTJSON.GetValue();
        var newValue = EditorGUILayout.ObjectField("Blueprint File: ", currentValue, typeof(TJSON), false);
        if(newValue != currentValue)
        {
            BlueprintTJSON.SetValue(newValue);
            recalculate = true;
        }

        if (GUILayout.Button("New"))
        {
            //open new window
            BlueprintLoaderPopup.Init(BlueprintTJSON, () => { recalculate = true; });
        }

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        if (recalculate)
        {
            ((ShipBlueprintManager)target).RecalculateBlueprint();
            recalculate = false;
        }
    }

    public class BlueprintLoaderPopup : EditorWindow
    {
        private SerializedProperty target;
        private Action recalculateBlueprint;

        private string blueprintName = "data";

        private static readonly string emptyShipData = "{\".OBJECTTYPE\":\"ShipBlueprintList\",\"Parts\":{\".LISTTYPE\":\"ShipPartModel\",\"Items\":[{\".OBJECTTYPE\":\"ShipPartModel\",\"PrefabLocation\":\"ShipParts/struct/structural block\",\"Offset\":\"(0.0, 0.0, 0.0)\",\"Rotation\":\"(0.0, 0.0, 0.0, 1.0)\",\"TreeAddress\":\"\"}]}}";

        public static void Init(SerializedProperty target, Action recalculateBlueprint)
        {
            var window = ScriptableObject.CreateInstance<BlueprintLoaderPopup>();
            window.target = target;
            window.recalculateBlueprint = recalculateBlueprint;
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);
            window.ShowPopup();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Create New ", EditorStyles.wordWrappedLabel);
            GUILayout.Space(70);

            EditorGUILayout.LabelField("Blueprint Name: ");
            blueprintName = EditorGUILayout.TextField(blueprintName);

            if (GUILayout.Button("Create") && !string.IsNullOrEmpty(blueprintName))
            {
                //create new tjson
                var asset = new TJSON();

                asset.Data = emptyShipData;

                AssetDatabase.CreateAsset(asset, "Assets/data/ShipBlueprints/" + blueprintName + ".asset");
                AssetDatabase.SaveAssets();

                target.SetValue(asset);
                target.serializedObject.ApplyModifiedProperties();

                recalculateBlueprint();

                this.Close();
            }
            if (GUILayout.Button("Cancel")) this.Close();
        }
    }
}
