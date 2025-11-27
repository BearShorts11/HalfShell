using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Data_Saving
{
    //https://www.youtube.com/watch?v=z1sMhGIgfoo

    public class FileDataService : IDataService
    {
        ISerializer serializer;
        string dataPath;
        string fileExtension;

        public FileDataService(ISerializer serializer)
        {
            this.dataPath = Application.persistentDataPath;
            this.fileExtension = ".json";
            this.serializer = serializer;
        }

        private string GetPathToFile(string fileName)
        { 
            return Path.Combine(dataPath, fileName);
        }


        public void Delete(string name)
        {
            string fileLocation = GetPathToFile(name);

            if (File.Exists(fileLocation))
            { 
                File.Delete(fileLocation);
            }
        }

        public void DeleteAll()
        {
            //WARNING: will delete everything in folder. Make sure only saved data is in this folder
            foreach (string filePath in Directory.GetFiles(dataPath))
            {
                File.Delete(filePath);
            }
        }

        public IEnumerable<string> ListSaves()
        {
            foreach (string path in Directory.GetFiles(dataPath))
            {
                if (Path.GetExtension(path) == fileExtension)
                {
                    yield return Path.GetFileNameWithoutExtension(path);
                }
            }
        }

        public GameData Load(string name)
        {
            string fileLocation = GetPathToFile(name);

            if (!File.Exists(fileLocation))
            {
                throw new IOException($"{name}{fileExtension} does not exist- no persisted GameData");
            }

            return serializer.Deserialize<GameData>(File.ReadAllText(fileLocation));
        }

        public void Save(GameData data, bool overwrite = true)
        {
            string fileLocation = GetPathToFile(data.Name);

            //not wholy necessary i believe
            if (!overwrite && File.Exists(fileLocation))
            {
                throw new IOException($"{data.Name}{fileExtension} already exists and cannot be overwritten");
            }

            //will always overwrite data to a save
            File.WriteAllText(fileLocation, serializer.Serialize(data));
        }
    }
}
