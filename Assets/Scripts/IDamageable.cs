using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public interface IDamageable
    {
        public float Health { get; set; }
        public float maxHealth { get; }

        public void Damage(float amount);
    }
}
