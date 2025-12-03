#if UNITY_EDITOR
using Assets.Scripts;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    //https://github.com/adammyhre/Unity-Inventory-System/blob/master/Assets/_Project/Scripts/Persistence/Editor/SaveManagerEditor.cs

    [CustomEditor(typeof(SaveLoadSystem))]
    public class SaveManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SaveLoadSystem saveLoadSystem = (SaveLoadSystem)target;
            string gameName = saveLoadSystem.gameData.Name;

            DrawDefaultInspector();

            if (GUILayout.Button("New Game"))
            {
                saveLoadSystem.NewGame();
            }

            if (GUILayout.Button("Save Game"))
            {
                saveLoadSystem.SaveGame();
            }

            if (GUILayout.Button("Load Game"))
            {
                saveLoadSystem.LoadGame(gameName);
            }

            if (GUILayout.Button("Delete Game"))
            {
                saveLoadSystem.DeleteGame(gameName);
            }
        }
    }
}
#endif