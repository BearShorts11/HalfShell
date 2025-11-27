using Assets.Scripts.Data_Saving;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    //https://www.youtube.com/watch?v=z1sMhGIgfoo - tutorial for base save system

    [Serializable]
    public class GameData
    { 
        public string Name { get; set; }

        public string CurrentLevelName { get; set; }
    }

    public class SaveLoadSystem : PersistentSingleton<SaveLoadSystem>
    {
        [SerializeField] public GameData gameData;

        IDataService dataService;

        protected override void Awake()
        {
            base.Awake();
            dataService = new FileDataService(new JsonSerializer());
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
