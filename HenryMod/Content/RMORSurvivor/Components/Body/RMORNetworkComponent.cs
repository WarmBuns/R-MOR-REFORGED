using RMORMod.Content.Shared.Components.Body;
using RoR2;
using UnityEngine;
using RMORMod.Content.RMORSurvivor;
using UnityEngine.Networking;

namespace RMORMod.Content.RMORSurvivor.Components.Body
{
    public class RMORNetworkComponent : NetworkBehaviour
    {
        private CharacterBody characterBody;
        private OverclockController overclockController;
        public void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
            overclockController = base.GetComponent<OverclockController>();
        }

        [Server]
        public void ResetSpecialStock()
        {
            RpcResetSpecialStock();
        }

        [ClientRpc]
        private void RpcResetSpecialStock()
        {
            characterBody.skillLocator.special.stock = 0;
        }

        [Server]
        public void ExtendOverclockServer(float duration)
        {
            RpcExtendOverclock(duration);
        }

        [ClientRpc]
        private void RpcExtendOverclock(float duration)
        {
            if (this.hasAuthority && overclockController)
            {
                overclockController.ExtendOverclock(duration);
            }
        }

        [Server]
        public void AddSecondaryStockServer()
        {
            GenericSkill secondary = characterBody.skillLocator.secondary;
            if (secondary.stock < secondary.maxStock && secondary.skillDef == Skilldefs.SpecialMissile)
            {
                secondary.stock++;
                if (secondary.stock == secondary.maxStock)
                    secondary.rechargeStopwatch = 0f;
            }
        }

        private void OnDestroy()
        {
            Util.PlaySound("Play_MULT_shift_end", base.gameObject);
        }
    }
}
