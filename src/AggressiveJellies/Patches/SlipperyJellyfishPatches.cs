using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace AggressiveJellies
{
    [HarmonyPatch(typeof(SlipperyJellyfish))]
    public static class SlipperyJellyfishPatches
    {
        private static readonly float moveSpeed = 3f;
        private static readonly float detectionRadius = 18f;

        private static Rigidbody? GetFootRigidbody(Character character, BodypartType type)
        {
            var method = typeof(Character).GetMethod("GetBodypartRig", BindingFlags.NonPublic | BindingFlags.Instance);
            if (method == null) return null;

            return method.Invoke(character, new object[] { type }) as Rigidbody;
        }


        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        private static void SlipperyJellyfish_Update_Postfix(SlipperyJellyfish __instance)
        {
            LookAtAndMoveTowardsNearestPlayer(__instance);
        }

        private static void LookAtAndMoveTowardsNearestPlayer(SlipperyJellyfish jellyfish)
        {
            Collider[] players = Physics.OverlapSphere(jellyfish.transform.position, detectionRadius);
            Character? nearestPlayer = null;
            float nearestDistSqr = float.MaxValue;

            foreach (var col in players)
            {
                Character? player = col.GetComponentInParent<Character>();
                if (player != null && player.IsLocal)
                {
                    Rigidbody? foot = GetFootRigidbody(player, BodypartType.Foot_R);
                    if (foot == null) continue;

                    float distSqr = (foot.position - jellyfish.transform.position).sqrMagnitude;
                    if (distSqr < nearestDistSqr)
                    {
                        nearestDistSqr = distSqr;
                        nearestPlayer = player;
                    }
                }
            }

            if (nearestPlayer == null) return;

            Rigidbody? targetFoot = GetFootRigidbody(nearestPlayer, BodypartType.Foot_R);
            if (targetFoot == null) return;

            Vector3 direction = targetFoot.position - jellyfish.transform.position;
            direction.y = 0f;
            if (direction == Vector3.zero) return;

            Vector3 moveDir = direction.normalized;
            Vector3 nextPos = jellyfish.transform.position + moveDir * moveSpeed * Time.deltaTime;

            RaycastHit wallHit;
            if (Physics.Raycast(jellyfish.transform.position + Vector3.up * 0.75f, moveDir, out wallHit, 1f, LayerMask.GetMask("Terrain", "Map")) &&
                Vector3.Angle(wallHit.normal, Vector3.up) > 60f)
            {
                jellyfish.transform.rotation = Quaternion.Slerp(jellyfish.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
                return;
            }

            Vector3 forwardOffset = moveDir * moveSpeed * Time.deltaTime;
            Vector3 rayStart = jellyfish.transform.position + Vector3.up * 1.5f + forwardOffset;

            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit groundHit, 5f, LayerMask.GetMask("Terrain", "Map")))
            {
                nextPos.y = groundHit.point.y + 0.1f;
                jellyfish.transform.position = nextPos;
                Quaternion terrainRot = Quaternion.FromToRotation(jellyfish.transform.up, groundHit.normal) * jellyfish.transform.rotation;
                jellyfish.transform.rotation = Quaternion.Slerp(jellyfish.transform.rotation, terrainRot, Time.deltaTime * 5f);
            }
            else
            {
                jellyfish.transform.position += forwardOffset;
                jellyfish.transform.rotation = Quaternion.Slerp(jellyfish.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
            }
        }

        // Optional: Patch Trigger() or OnTriggerEnter() similarly
    }
}
