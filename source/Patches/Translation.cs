using System;
using System.Collections.Generic;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;

namespace TownOfUs.Patches
{
    [HarmonyPatch]

    public class TranslationPatches
    {
        public static int CurrentLanguage = 0;

        public static Dictionary<int, string> Languages = new()
        {
            {0, "Off"},
            {1, "Polish"}
        };

        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
        [HarmonyPrefix]

        public static void Prefix(MainMenuManager __instance)
        {
            if (__instance.newsButton != null)
            {

                var translationtoggle = UnityEngine.Object.Instantiate(__instance.newsButton, null);
                translationtoggle.name = "translationtoggle";

                translationtoggle.transform.localScale = new Vector3(0.44f, 0.84f, 1f);

                PassiveButton passive = translationtoggle.GetComponent<PassiveButton>();
                passive.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();

                translationtoggle.gameObject.transform.SetParent(GameObject.Find("RightPanel").transform);
                var pos = translationtoggle.gameObject.AddComponent<AspectPosition>();
                pos.Alignment = AspectPosition.EdgeAlignments.LeftBottom;
                pos.DistanceFromEdge = new Vector3(2.1f, 2.7f, 8f);

                passive.OnClick.AddListener((Action)(() =>
                {
                    int num = CurrentLanguage + 1;
                    CurrentLanguage = num >= Languages.Count ? 0 : num;
                    var text = translationtoggle.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>();
                    text.text = $"Translation: {Languages[CurrentLanguage]}";
                    Debug.Log("Language set to " + Languages[CurrentLanguage]);
                }));

                var text = translationtoggle.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>();
                __instance.StartCoroutine(Effects.Lerp(0.1f, new System.Action<float>((p) =>
                {
                    text.text = $"Translation: {Languages[CurrentLanguage]}";
                    pos.AdjustPosition();
                })));

                translationtoggle.transform.GetChild(0).transform.localScale = new Vector3(translationtoggle.transform.localScale.x + 1, 1f, 1f);
                translationtoggle.transform.GetChild(0).transform.localPosition -= new Vector3(1.5f,0f,0f);
                translationtoggle.transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
                translationtoggle.transform.GetChild(2).GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
                translationtoggle.GetComponent<NewsCountButton>().DestroyImmediate();
                translationtoggle.transform.GetChild(3).gameObject.DestroyImmediate();
            }
        }
    }
}