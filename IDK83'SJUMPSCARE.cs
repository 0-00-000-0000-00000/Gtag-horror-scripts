using GorillaLocomotion;
using TheGorillaWatch.Behaviors.Page;

namespace TheGorillaWatch.Behaviors.Mods
{
    class SpeedyMonk : ModPage
    {
        public override string modName => "SpeedyMonk";

        public override void OnUpdate()
        {
            base.OnUpdate();
            GTPlayer.Instance.jumpMultiplier = 1.5f;
            GTPlayer.Instance.maxJumpSpeed = 8.5f;
        }

        public override void Disable()
        {
            base.Disable();
            GTPlayer.Instance.jumpMultiplier = 1.1f;
            GTPlayer.Instance.maxJumpSpeed = 6.5f;
        }

        public override PageType pageType => PageType.Toggle;
    }
}