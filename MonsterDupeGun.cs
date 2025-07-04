using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;

namespace IDK83.Mods
{
    internal class MonsterLockDuplicateGun : MonoBehaviourPunCallbacks
    {
        private static GameObject beam;
        private static LineRenderer line;
        private static bool setupDone = false;

        private static MonoBehaviour lockedMonsterScript; // reference to the locked monster's main AI script
        private static GameObject lockedMonsterObject;    // reference to locked monster GameObject

        private static bool isLocking = false;

        private static Transform rightHand => GorillaLocomotion.GTPlayer.Instance?.rightControllerTransform;
        private static Transform leftHand => GorillaLocomotion.GTPlayer.Instance?.leftControllerTransform;

        private static float maxDistance = 30f;

        // Input polling helpers (adjust these to your input system)
        private static bool HoldB() => ControllerInputPoller.instance.leftControllerIndexFloat > 0.75f;  // Hold B on left controller
        private static bool HoldA() => ControllerInputPoller.instance.rightControllerIndexFloat > 0.75f; // Hold A on right controller

        public static void Run()
        {
            if (!setupDone) SetupBeam();

            if (GorillaLocomotion.GTPlayer.Instance == null) return;

            Vector3 origin = rightHand.position;
            Vector3 dir = rightHand.forward;

            if (isLocking && lockedMonsterObject != null)
            {
                // Keep beam locked on the monster position
                Vector3 lockedPos = lockedMonsterObject.transform.position + Vector3.up * 1.5f; // Aim near head
                line.SetPosition(0, origin);
                line.SetPosition(1, lockedPos);

                if (!HoldB())
                {
                    // Release lock when B released
                    isLocking = false;
                    lockedMonsterObject = null;
                    lockedMonsterScript = null;
                    line.enabled = false;
                }
            }
            else
            {
                // Normal beam, no lock
                line.SetPosition(0, origin);
                line.SetPosition(1, origin + dir * maxDistance);
                line.enabled = true;

                if (HoldB())
                {
                    // Try to lock monster in front
                    Ray ray = new Ray(origin, dir);
                    if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
                    {
                        var mb = hit.collider.GetComponentInParent<MonoBehaviour>();
                        if (mb != null && (mb.GetType().Name.Contains("HorrorAI") || mb.GetType().Name.Contains("EnemyController")))
                        {
                            lockedMonsterObject = mb.gameObject;
                            lockedMonsterScript = mb;
                            isLocking = true;
                        }
                    }
                }
            }

            // Duplicate monster on Hold A while locked
            if (HoldA() && lockedMonsterObject != null)
            {
                DuplicateMonsterServerRpc(lockedMonsterObject.GetComponent<PhotonView>().ViewID);
            }
        }

        private static void SetupBeam()
        {
            beam = new GameObject("LockDuplicateBeam");
            line = beam.AddComponent<LineRenderer>();
            line.startWidth = 0.03f;
            line.endWidth = 0.03f;
            line.material = new Material(Shader.Find("Unlit/Color"));
            line.material.color = Color.cyan;
            line.positionCount = 2;
            line.enabled = false;
            setupDone = true;
        }

        // SERVER-SIDE duplication via RPC
        [PunRPC]
        private void RPC_DuplicateMonster(int viewID)
        {
            PhotonView originalPV = PhotonView.Find(viewID);
            if (originalPV == null) return;

            GameObject originalGO = originalPV.gameObject;
            Vector3 spawnPos = originalGO.transform.position + originalGO.transform.forward * 2f + Vector3.up * 0.5f;

            // Instantiate a clone on the network with the same prefab as original
            GameObject clone = PhotonNetwork.Instantiate(originalGO.name, spawnPos, originalGO.transform.rotation);

            // Copy scripts data if necessary - depending on monster scripts implementation
            // Usually monster scripts will initialize on their own, so just spawning is enough
        }

        // Call this from client to ask server to duplicate monster
        private static void DuplicateMonsterServerRpc(int viewID)
        {
            // Use Photon RPC system, send from local player to all including server
            PhotonView.Get(GorillaLocomotion.GTPlayer.Instance).RPC("RPC_DuplicateMonster", RpcTarget.MasterClient, viewID);
        }
    }
}