using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections;

namespace TownOfUs.ImpostorRoles.KamikazeMod
{
    public class Bomb
    {
        public Transform transform;
    }

    [HarmonyPatch]
    public static class BombExtentions
    {
        public static void ClearBomb(this Bomb b)
        {
            Object.Destroy(b.transform.gameObject);
            b = null;
        }

        public static Bomb CreateBomb(this Vector3 location)
        {
            var BombPref = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            BombPref.name = "Bomb";
            BombPref.transform.localScale = new Vector3(CustomGameOptions.KamikazeDetonateRadius * ShipStatus.Instance.MaxLightRadius * 2f, 
                CustomGameOptions.KamikazeDetonateRadius * ShipStatus.Instance.MaxLightRadius * 2f, CustomGameOptions.KamikazeDetonateRadius * ShipStatus.Instance.MaxLightRadius * 2f);
            GameObject.Destroy(BombPref.GetComponent<SphereCollider>());
            BombPref.GetComponent<MeshRenderer>().material = Roles.Bomber.bombMaterial;
            BombPref.transform.position = location;
            var BombScript = new Bomb();
            BombScript.transform = BombPref.transform;
            return BombScript;
        }
    }

    public class BombTeammate
    {
        public static Bomb TempBomb = null;

        public static IEnumerator BombShowTeammate(Vector3 location)
        {
            TempBomb = BombExtentions.CreateBomb(location);

            //yield return (object)new WaitForSeconds(CustomGameOptions.KamikazeDetonateDelay);
            yield return (object)new WaitForSeconds(CustomGameOptions.KamikazeDetonateDelay+0.5f);

            try { BombExtentions.ClearBomb(TempBomb); }
            catch { }
        }
    }
}