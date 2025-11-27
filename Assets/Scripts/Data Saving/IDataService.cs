using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public interface IDataService
    {
        void Save(GameData data, bool overwrite = true);
        GameData Load(string name);
        void Delete(string name);
        void DeleteAll();
        IEnumerable<string> ListSaves();
    }
}
