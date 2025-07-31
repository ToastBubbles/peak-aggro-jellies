using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using Photon.Pun;

namespace AggressiveJellies.Patches
{
    [HarmonyPatch(typeof(SlipperyJellyfish), nameof(SlipperyJellyfish.Trigger))]
    public static class SlipperyJellyfish_Trigger_Patch
    {
        private static Rigidbody? GetFootRigidbody(Character character, BodypartType type)
        {
            var method = typeof(Character).GetMethod("GetBodypartRig", BindingFlags.NonPublic | BindingFlags.Instance);
            if (method == null) return null;

            return method.Invoke(character, new object[] { type }) as Rigidbody;
        }
        static bool Prefix(SlipperyJellyfish __instance, int targetID)
        {
            Character component = PhotonView.Find(targetID).GetComponent<Character>();
            if (component == null)
                return false; // Skip original method

            Rigidbody? footR = GetFootRigidbody(component, BodypartType.Foot_R);
            Rigidbody? footL = GetFootRigidbody(component, BodypartType.Foot_L);
            Rigidbody? hip = GetFootRigidbody(component, BodypartType.Hip);
            Rigidbody? head = GetFootRigidbody(component, BodypartType.Head);

            component.RPCA_Fall(2f);

            // Adjusted force values:
            Vector3 pushDir = component.data.lookDirection_Flat;

            footR?.AddForce(pushDir * 1000f, ForceMode.Impulse);
            footL?.AddForce(pushDir * 1000f, ForceMode.Impulse);
            hip?.AddForce(Vector3.up * 1000f, ForceMode.Impulse);
            head?.AddForce(pushDir * 5500f, ForceMode.Impulse);

            component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, 0.05f, true);

            foreach (var sfx in __instance.slipSFX)
            {
                sfx?.Play(__instance.transform.position);
            }

            return false; // Prevent original Trigger() from running
        }
    }

}
