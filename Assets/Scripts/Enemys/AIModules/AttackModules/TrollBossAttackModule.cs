using Enemys.Bosses;

namespace Enemys.AIModules
{ 
    public class TrollBossAttackModule : RangeAttackModule
    {
        public void ChangeParameters(TrollBossStage stage)
        {
            _attackDelay = stage.AttackDelay;
            _dispersion = stage.Dispersion;
            _bulletsPerShot = stage.BulletsPerShot;
            _attackDistance = stage.AttackDistance;
        }
    }
}