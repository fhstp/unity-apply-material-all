#nullable enable

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace At.Ac.FhStp.ApplyMaterialAll
{
    [InitializeOnLoad]
    public static class ApplyMaterialToAll
    {
        private static GameObject? TryAsSelectedGameObject(int instanceID)
        {
            return EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        }

        private static Material? TryGetDraggedMaterial()
        {
            return DragAndDrop.objectReferences?.SingleOrDefault() as Material;
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            var currentEvent = Event.current;
            
            // Must hold alt
            if (!currentEvent.alt) return;

            // Must be hovering selection rect
            if (!selectionRect.Contains(currentEvent.mousePosition)) return;
            
            
            // Must only drag single material
            if (TryGetDraggedMaterial() is not { } material) return;

            // Must target game object
            if (TryAsSelectedGameObject(instanceID) is not { } gameObject)
                return;

            // Handle drag updated for feedback
            if (currentEvent.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                currentEvent.Use();
                return;
            }

            // Must be drag
            if (currentEvent.type != EventType.DragPerform) return;

            DragAndDrop.AcceptDrag();

            Undo.RegisterFullObjectHierarchyUndo(gameObject,
                "Apply material to all");
            var meshRenderers =
                gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach (var meshRenderer in meshRenderers)
                meshRenderer.material = material;
            EditorUtility.SetDirty(gameObject);

            currentEvent.Use();
        }

        static ApplyMaterialToAll()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }
    }
}