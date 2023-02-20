using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

public class IconChangerEditor : EditorWindow
{
    private static Object[] selectedObjects;

    private Texture2D icons;
    private Vector2 scrollPos;

    private int pathNumber;
    public string[] pathsArr;

    private int tabIndex;
    
    [MenuItem("Utilities/Icon Changer")]
    private static void ShowWindow()
    {
        var window = GetWindow<IconChangerEditor>();
        window.titleContent = new UnityEngine.GUIContent("Icon Changer");

        window.maxSize = new Vector2(302, 400);
        window.minSize = window.maxSize;

        window.Show();
    }

    private void Awake()
    {
        Selection.selectionChanged += Repaint;
    }

    private void UpdateLabel()
    {
        GUILayout.Label($"Currently selected objects: {selectedObjects.Length}");
    }

    private void OnGUI()
    {
        
        selectedObjects = Selection.objects;

        tabIndex = GUILayout.Toolbar(tabIndex, new string[] { "Icons", "Paths" });

        switch (tabIndex)
        {
            case 0:
                
                GUILayout.Space(15);
                UpdateLabel();
                GUILayout.Space(15);
                FetchIcons();
                
                GUI.backgroundColor = Color.red;
        
                if (GUILayout.Button("Reset Icon"))
                {
                    foreach (var selectedObject in selectedObjects)
                    {
                        var monoImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(selectedObject)) as MonoImporter;

                        if (!monoImporter) return;
                    
                        monoImporter.SetIcon(Texture2D.normalTexture);
                        monoImporter.SaveAndReimport();
                    }
                }
                break;
            
            case 1:

                pathNumber = EditorGUILayout.IntField("Number of Paths: ", pathNumber);

                EditorGUIUtility.labelWidth = 35;
                
                for (int i = 0; i < pathNumber; i++)
                {
                    pathsArr[i] = EditorGUILayout.TextField("Path: ", pathsArr[i]);
                }
                
                break;
            
            default:
                break;
        }
    }

    private void FetchIcons()
    {
        ArrayUtility.Add(ref pathsArr, "Assets/Scripts/IconTool/Icons");
        
        var assets = AssetDatabase.FindAssets("", pathsArr);

        int index = 0;

        var buttonPosition = new Rect(new Vector2(10, 40), new Vector2(50, 50));

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true);
        
        foreach (var asset in assets)
        {
            var path = AssetDatabase.GUIDToAssetPath(asset);
            var item = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
            var icon = item as Texture2D;

            var button = GUI.Button(buttonPosition, icon);
            
            if (button)
            {
                foreach (var selectedObject in selectedObjects)
                {
                    var monoImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(selectedObject)) as MonoImporter;

                    if (!monoImporter) return;
                    
                    monoImporter.SetIcon(icon);
                    monoImporter.SaveAndReimport();
                }
            }

            index++;
            
            buttonPosition.x += 55;
            
            if (index % 5 == 0)
            {
                buttonPosition.x = 10;
                buttonPosition.y += 55;
            }
        }
        
        EditorGUILayout.EndScrollView();
        
    }
}