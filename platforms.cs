using System.Collections.Generic;
using UnityEngine;

namespace IDK83.Mods
{
    internal class Platforms
    {
        private static GameObject leftPlat = null;
        private static GameObject rightPlat = null;

        public static void PlatformMod()
        {
            Transform leftHand = GorillaTagger.Instance.leftHandTransform;
            Transform rightHand = GorillaTagger.Instance.rightHandTransform;

            if (ControllerInputPoller.instance.leftGrab && leftPlat == null)
                leftPlat = CreatePlatformOnHand(leftHand);

            if (ControllerInputPoller.instance.rightGrab && rightPlat == null)
                rightPlat = CreatePlatformOnHand(rightHand);

            if (ControllerInputPoller.instance.leftGrabRelease && leftPlat != null)
            {
                Object.Destroy(leftPlat);
                leftPlat = null;
            }

            if (ControllerInputPoller.instance.rightGrabRelease && rightPlat != null)
            {
                Object.Destroy(rightPlat);
                rightPlat = null;
            }
        }

        private static GameObject CreatePlatformOnHand(Transform hand)
        {
            GameObject plat = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plat.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
            plat.transform.position = hand.position;
            plat.transform.rotation = hand.rotation;

            plat.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
            plat.GetComponent<Renderer>().material.color = new Color(0.6f, 0.2f, 0.8f); // Purple

            plat.AddComponent<GorillaSurfaceOverride>().overrideIndex = 0;
            plat.layer = 18; // Gorilla collision layer
            plat.transform.SetParent(hand); // Stay attached smoothly

            return plat;
        }
    }
}