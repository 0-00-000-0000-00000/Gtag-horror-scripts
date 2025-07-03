using UnityEngine;

namespace IDK83.Mods
{
    internal class StickyPlatforms
    {
        private static GameObject leftPlat = null;
        private static GameObject rightPlat = null;

        public static void StickyPlatformMod()
        {
            Transform leftHand = GorillaTagger.Instance.leftHandTransform;
            Transform rightHand = GorillaTagger.Instance.rightHandTransform;

            if (ControllerInputPoller.instance.leftGrab && leftPlat == null)
                leftPlat = CreateStickyPlatform(leftHand);

            if (ControllerInputPoller.instance.rightGrab && rightPlat == null)
                rightPlat = CreateStickyPlatform(rightHand);

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

            // Lock hands to platform while held
            if (leftPlat != null)
                leftHand.position = leftPlat.transform.position;

            if (rightPlat != null)
                rightHand.position = rightPlat.transform.position;
        }

        private static GameObject CreateStickyPlatform(Transform hand)
        {
            GameObject plat = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plat.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
            plat.transform.position = hand.position;
            plat.transform.rotation = hand.rotation;

            plat.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
            plat.GetComponent<Renderer>().material.color = new Color(0.7f, 0.1f, 0.9f); // Sticky purple

            plat.AddComponent<GorillaSurfaceOverride>().overrideIndex = 0;
            plat.layer = 18;
            plat.transform.SetParent(null); // Sticky stays where spawned

            return plat;
        }
    }
}