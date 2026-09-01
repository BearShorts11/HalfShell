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

        /// <summary>
        /// Damages this object. Called from other scripts when damage to this should occur. 
        /// </summary>
        /// <param name="amount"></param>
        public void TakeDamage(float amount);

        /// <summary>
        /// Damages this object, can take shell type into consideration.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="shellType"></param>
        public void TakeDamage(float amount, ShellBase.ShellType shellType);
    }
}
