using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    //https://www.youtube.com/watch?v=z1sMhGIgfoo 

    public class JsonSerializer : ISerializer
    {
        public T Deserialize<T>(string json)
        {
            throw new NotImplementedException();
        }

        public string Serialize<T>(T obj)
        {
            throw new NotImplementedException();
        }
    }
}
