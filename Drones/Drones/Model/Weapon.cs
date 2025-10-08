using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieSurvivor.Model
{
    public abstract class Weapon
    {
        public float _damage {  get; set; }
        public float _fireRate { get; set; }
        protected float _cooldownTimer = 0f;

    }
}
