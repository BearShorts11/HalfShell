#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Helpers
{
    public class EditorFindInvalidAABB
    {
        [MenuItem("Tools/FindInvalidAABB")]
        public static void FindInvalidAABB()
        {
            var invalidObjects = GetInvalidAABB();
            if (invalidObjects.Count > 0)
                Debug.LogError($"Found invalidAABB: objects count {invalidObjects.Count}");
            else
                Debug.Log("Not found invalidAABB");
        }

        // Returns invalid objects with wrong scale: for example like -4.019843e-11 or +6.26252e+29
        private static List<GameObject> GetInvalidAABB()
        {
            var result = new List<GameObject>();
            var allObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include,FindObjectsSortMode.InstanceID);

            foreach (var obj in allObjects)
            {
                var rectTransform = obj.GetComponent<RectTransform>();
                if (rectTransform != null)
                    continue;

                var position = obj.transform.position;
                var scale = obj.transform.localScale;

                if (IsValueInvalid(position.x) || IsValueInvalid(position.y) || IsValueInvalid(position.z)
                    || IsValueInvalid(scale.x) || IsValueInvalid(scale.y) || IsValueInvalid(scale.z))
                {
                    result.Add(obj);
                    Debug.LogError($"Found invalidAABB object {GetObjectPath(obj.transform)}");
                }
            }

            return result;
        }

        private static bool IsValueInvalid(float value)
        {
            return value < 0f;
        }

        private static string GetObjectPath(Transform transform)
        {
            var result = "";
            while (transform != null)
            {
                result = transform.gameObject.name + "/" + result;
                transform = transform.parent;
            }

            return result;
        }
    }
}

#endif