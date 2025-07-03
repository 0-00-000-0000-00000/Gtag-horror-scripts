using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace IDK83.Mods
{
    internal class KickGun
    {
        private static GameObject beam;
        private static LineRenderer line;
        private static bool setupDone = false;
        private static float fireCooldown = 0.3f;
        private static float lastFireTime = -1f;

        public static void Run()
        {
            if (!setupDone)
                SetupBeam();

            if (Time.time - lastFireTime < fireCooldown)
                return;

            if (GorillaLocomotion.GTPlayer.Instance == null) return;

            Transform hand = GorillaLocomotion.GTPlayer.Instance.rightControllerTransform;
            Vector3 origin = hand.position;
            Vector3 dir = hand.forward;

            line.SetPosition(0, origin);
            line.SetPosition(1, origin + dir * 100f);

            // Only trigger on press (not hold)
            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.75f)
            {
                lastFireTime = Time.time;

                Ray ray = new Ray(origin, dir);
                RaycastHit[] hits = Physics.RaycastAll(ray, 100f);

                foreach (var hit in hits)
                {
                    PhotonView view = hit.collider.GetComponentInParent<PhotonView>();
                    if (view != null && view.Owner != null && view.Owner != PhotonNetwork.LocalPlayer)
                    {
                        Kick(view.Owner);
                        SpawnImpactFX(hit.point);
                    }
                }
            }
        }

        private static void Kick(Player target)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CloseConnection(target);
                Debug.Log("[KickGun] Kicked " + target.NickName);
            }
            else
            {
                Debug.Log("[KickGun] You're not the host.");
            }
        }

        private static void SetupBeam()
        {
            beam = new GameObject("KickBeam");
            line = beam.AddComponent<LineRenderer>();
            line.startWidth = 0.04f;
            line.endWidth = 0.04f;
            line.material = new Material(Shader.Find("Unlit/Color"));
            line.material.color = Color.red;
            line.positionCount = 2;
            setupDone = true;
        }

        private static void SpawnImpactFX(Vector3 position)
        {
            GameObject fx = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            fx.transform.position = position;
            fx.transform.localScale = Vector3.one * 0.1f;
            fx.GetComponent<Renderer>().material.color = Color.red;
            GameObject.Destroy(fx, 0.3f);
        }
    }
}