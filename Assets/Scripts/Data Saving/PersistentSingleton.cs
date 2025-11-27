using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class PersistentSingleton<T> : MonoBehaviour where T : Component
    {
        //https://www.youtube.com/watch?v=LFOXge7Ak3E - tutorial
        //GitHub from:
        //https://github.com/adammyhre/Unity-Inventory-System/blob/master/Assets/_Project/Scripts/Utility/PersistentSingleton.cs#L4 
        

        public bool UnparentOnAwake = true;
        public static bool HasInstance => instance != null;
        public static T Current => instance;

        protected static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<T>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name + "AutoCreated";
                        instance = obj.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        protected virtual void Awake() => InitializeSingleton();

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (UnparentOnAwake)
            {
                transform.SetParent(null);
            }

            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(transform.gameObject);
                enabled = true;
            }
            else
            {
                if (this != instance)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
