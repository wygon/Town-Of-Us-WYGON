using Il2CppInterop.Runtime.Attributes;
using Reactor.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TownOfUs.Components;

[RegisterInIl2Cpp]
public class MeetingHudBehaviour : MonoBehaviour
{
    public MeetingHudBehaviour(IntPtr ptr) : base(ptr)
    {
    }

    internal MeetingHud meetingHud = null!;

    [HideFromIl2Cpp]
    public IEnumerable<PlayerVoteArea> Targets => meetingHud.playerStates.OrderBy(p => p.AmDead);

    public void Start()
    {

        if (Targets.Count() < 16)
        {
            // dont change layout if players count is below 16
            return;
        }

        var i = 0;
        foreach (var button in Targets)
        {

            button.gameObject.SetActive(true);

            var row = i / 4;
            var col = i % 4;
            var buttonTransform = button.transform;
            buttonTransform.localScale *= 0.75f;
            buttonTransform.localPosition = meetingHud.VoteOrigin +
                                      new Vector3(
                                          meetingHud.VoteButtonOffsets.x * col * 0.75f - 0.375f,
                                          meetingHud.VoteButtonOffsets.y * row * 0.75f,
                                          buttonTransform.localPosition.z
                                      );
            i++;
        }
    }

}
