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
        public PlayerData playerData;
    }

    //TODO: throw in own interface ISaveandBind
    public interface ISaveable
    {
        public SerializableGuid Id { get; set; } //init not set
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

        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //returns early/does not bind in specific scenes
            //add if necessary
            if (scene.name == "TitleScreen") return;

            Bind<Kerth, PlayerData>(gameData.playerData);
        }


        //single entity binding (one instance)
        private void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
            if (entity != null)
            {
                if (data == null) data = new TData { Id = entity.Id };

                entity.Bind(data);
            }
        }

        //multiple entity binding (ex. enemies)
        private void Bind<T, TData>(List<TData> datas) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);

            foreach (var entity in entities)
            {
                var data = datas.FirstOrDefault(d => d.Id == entity.Id);

                if (data == null)
                {
                    data = new TData { Id = entity.Id };
                    datas.Add(data);
                }

                entity.Bind(data);
            }
        }

        public void NewGame()
        {
            gameData = new GameData
            {
                Name = "new game",
                CurrentLevelName = "N Testing"
            };

            SceneManager.LoadScene(gameData.CurrentLevelName);
        }

        public void LoadGame(string saveName)
        { 
            gameData = dataService.Load(saveName);

            if (string.IsNullOrWhiteSpace(gameData.CurrentLevelName)) gameData.CurrentLevelName = "N Testing";

            SceneManager.LoadScene(gameData.CurrentLevelName);
        }

        public void ReloadGame() => LoadGame(gameData.Name);

        public void SaveGame() => dataService.Save(gameData);

        public void DeleteGame(string saveName) => dataService.Delete(saveName);

    }
}
