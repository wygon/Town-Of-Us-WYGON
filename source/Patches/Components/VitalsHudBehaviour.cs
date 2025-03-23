using Il2CppInterop.Runtime.Attributes;
using Reactor.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TownOfUs.Components;

[RegisterInIl2Cpp]
public class VitalsHudBehaviour : MonoBehaviour
{
    public VitalsHudBehaviour(IntPtr ptr) : base(ptr) {}

    internal VitalsMinigame vitalsMinigame = null!;

    [HideFromIl2Cpp]
    public IEnumerable<VitalsPanel> Targets => vitalsMinigame.vitals.ToArray();

    public void Start()
    {

        if (Targets.Count() < 16)
        {
            // dont change layout if players count is below 16
            return;
        }

        var i = 0;
        foreach (var panel in Targets)
        {
            panel.gameObject.SetActive(true);
            var row = i / 4;
            var col = i % 4;
            var panelTransform = panel.transform;
            panelTransform.localScale *= 0.75f;
            panelTransform.localPosition = new Vector3(
                                          vitalsMinigame.XStart + vitalsMinigame.XOffset * (col * 0.75f - 0.25f),
                                          vitalsMinigame.YStart +  vitalsMinigame.YOffset * row * 0.75f,
                                          panelTransform.localPosition.z
                                      );
            i++;
        }
    }
}
