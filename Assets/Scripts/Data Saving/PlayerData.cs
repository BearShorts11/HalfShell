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

        //PlayerBehavior data
        public Vector3 position;
        public Quaternion rotation;
        public float Health;
        public float Armor;

        //PlayerShooting data
        public Dictionary<ShellBase.ShellType, int> AmmoCounts;
        /// <summary>
        /// because *stacks* magazine is saved in reversed order and loaded in correct order. 
        /// Saved as ints to reference shell type in ShellBase.ShellType enum. 
        /// Cannot save custom objects directly
        /// </summary>
        public Stack<int> ReversedMagazine;
        /// <summary>
        /// holds enum number of shell type
        /// </summary>
        public int Chamber; 
    }
}
