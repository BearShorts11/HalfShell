using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    //https://www.youtube.com/watch?v=z1sMhGIgfoo - tutorial for base save system

    [Serializable] public class GameData
    {
        public string Name;

        public string CurrentLevelName;
    }

    //TODO: throw in own interface ISaveandBind
    public interface ISaveable
    {
        SerializableGuid Id { get; set; }
    }

    public interface IBind<TData> where TData : ISaveable
    {
        SerializableGuid Id { get; set; }
        void Bind(TData tData);
    }


    public class SaveLoadSystem : PersistentSingleton<SaveLoadSystem>
    {
        public GameData gameData;

        IDataService dataService;

        protected override void Awake()
        {
            base.Awake();
            dataService = new FileDataService(new JsonSerializer());
        }

        private void Update()
        {
            //to find file path
            //Debug.Log(Application.persistentDataPath);
        }

        public void NewGame()
        {
            gameData = new GameData
            {
                Name = "new game",
                CurrentLevelName = "TitleScreen"
            };

            SceneManager.LoadScene(gameData.CurrentLevelName);
        }

        public void LoadGame(string saveName)
        { 
            gameData = dataService.Load(saveName);

            if (string.IsNullOrWhiteSpace(gameData.CurrentLevelName)) gameData.CurrentLevelName = "TitleScreen";

            SceneManager.LoadScene(gameData.CurrentLevelName);
        }

        public void ReloadGame() => LoadGame(gameData.Name);

        public void SaveGame() => dataService.Save(gameData);

        public void DeleteGame(string saveName) => dataService.Delete(saveName);

    }
}
