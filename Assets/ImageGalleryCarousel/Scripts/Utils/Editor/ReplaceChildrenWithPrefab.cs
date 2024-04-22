using UnityEditor;
using UnityEngine;

/// <summary>
/// Provides an editor window to replace all children of a specified parent GameObject with instances of a specified prefab.
/// </summary>
public class ReplaceChildrenWithPrefab : EditorWindow {
    private static GameObject Prefab;
    private static GameObject ParentObject; // Static to retain the selection across instances

    private void OnGUI() {
        // Allow users to change ParentObject even if it's pre-selected
        ParentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object", ParentObject, typeof(GameObject), true);
        Prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", Prefab, typeof(GameObject), false);

        if (GUILayout.Button("Replace Children") && ParentObject != null && Prefab != null) {
            ReplaceChildren(ParentObject, Prefab);
        }
    }

    [MenuItem("Tools/Replace Children With Prefab")]
    public static void ShowWindow() {
        var window = GetWindow<ReplaceChildrenWithPrefab>("Replace Children");
        if (Selection.activeGameObject != null) {
            ParentObject = Selection.activeGameObject; // Prepopulate if something is selected
        }
    }

    // Add a new menu item in the hierarchy context menu
    [MenuItem("GameObject/Replace Children With Prefab", false, 0)]
    public static void ReplaceChildrenMenu(MenuCommand menuCommand) {
        ParentObject = menuCommand.context as GameObject; // Set the ParentObject from the context
        ShowWindow(); // Open the window
    }

    private static void ReplaceChildren(GameObject parentObject, GameObject prefab) {
        if (parentObject == null || prefab == null) {
            EditorUtility.DisplayDialog("Error", "Parent Object and Prefab must be assigned.", "OK");
            return;
        }

        Undo.RecordObject(parentObject, "Replace Children With Prefab");

        var children = parentObject.transform.childCount;
        var allChildren = new GameObject[children];

        for (var i = 0; i < children; i++) {
            allChildren[i] = parentObject.transform.GetChild(i).gameObject;
        }

        foreach (var child in allChildren) {
            var newChild = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (newChild != null) {
                Undo.RegisterCreatedObjectUndo(newChild, "Replace Child");
                newChild.transform.SetParent(parentObject.transform);
                newChild.transform.localPosition = child.transform.localPosition;
                newChild.transform.localRotation = child.transform.localRotation;
                newChild.transform.localScale = child.transform.localScale;
                newChild.name = child.name;
            }

            Undo.DestroyObjectImmediate(child);
        }
    }
}