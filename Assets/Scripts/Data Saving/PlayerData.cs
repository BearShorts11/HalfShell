using System;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class PlayerData : ISaveable
    {
        public SerializableGuid Id { get ; set ; }

        public Vector3 position;
        public Quaternion rotation;
    }
}
