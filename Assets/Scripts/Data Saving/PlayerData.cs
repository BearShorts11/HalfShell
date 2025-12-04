using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{

    [Serializable]
    public class PlayerData : ISaveable
    {
        [SerializeField] private SerializableGuid _id;
        public SerializableGuid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Vector3 position;
        public Quaternion rotation;

        //public float Health;
        //public float Armor;
    }
}
