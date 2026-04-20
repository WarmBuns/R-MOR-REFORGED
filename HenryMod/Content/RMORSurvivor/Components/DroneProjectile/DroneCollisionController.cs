using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RMORMod.Content.RMORSurvivor.Components.DroneProjectile
{
    public class DroneCollisionController : MonoBehaviour
    {
        public static float destroyIfNoTargetTime = 5f;

        private ProjectileTargetComponent ptc;
        private ProjectileStickOnImpact stick;
        private ProjectileController projectileController;
        private ProjectileSimple projectileSimple;
        private float projectileNoTargetStopwatch;
        private int passThroughWallsFrames = 0;

        private void Awake()
        {
            stick = base.GetComponent<ProjectileStickOnImpact>();
            ptc = base.GetComponent<ProjectileTargetComponent>();
            projectileNoTargetStopwatch = 0f;
            projectileController = base.GetComponent<ProjectileController>();
            base.TryGetComponent(out projectileSimple);
        }

        private void FixedUpdate()
        {
            if (stick && !stick.syncVictim)
            {
                if (passThroughWallsFrames > 0)
                {
                    passThroughWallsFrames--;
                    if (passThroughWallsFrames <= 0)
                    {
                        base.gameObject.layer = LayerIndex.projectile.intVal;
                    }
                }

                //Check if target lost
                if (NetworkServer.active && ptc)
                {
                    if (ptc.target != null)
                    {
                        projectileNoTargetStopwatch = 0f;
                    }
                    projectileNoTargetStopwatch += Time.fixedDeltaTime;
                    if (projectileNoTargetStopwatch >= DroneCollisionController.destroyIfNoTargetTime)
                    {
                        Destroy(base.gameObject);
                        return;
                    }
                }
            }
            else
            {
                if (NetworkServer.active && projectileSimple)
                {
                    //Reset lifetime.
                    projectileSimple.SetLifetime(30f);
                }
                passThroughWallsFrames = 0;
                base.gameObject.layer = LayerIndex.projectile.intVal;
                Destroy(base.GetComponent<ProjectileSteerTowardTarget>());
                Destroy(base.GetComponent<ProjectileSphereTargetFinder>());
                Destroy(ptc);
                Destroy(this);
                return;
            }
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerIndex.world.intVal)
            {
                base.gameObject.layer = LayerIndex.collideWithCharacterHullOnly.intVal;
                passThroughWallsFrames = 15;
            }
            else
            {
                base.gameObject.layer = LayerIndex.projectile.intVal;
            }
        }
    }
}
