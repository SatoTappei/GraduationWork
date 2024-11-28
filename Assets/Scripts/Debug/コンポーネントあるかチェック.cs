using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class コンポーネントあるかチェック : MonoBehaviour
{
    void Start()
    {
        { if (!TryGetComponent(out AttackToSurrounding _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out Defeated _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out EntryToDungeon _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out EscapeFromDungeon _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out MovementToDirection _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out MovementToTarget _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out ScavengeToSurrounding _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out TalkToSurrounding _)) Debug.Log("ないよ"); }

        { if (!TryGetComponent(out ActionLog _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out AvailableActions _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out Blackboard _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out DamageApply _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out DamageEffect _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out DefeatedEffect _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out DoorOpenApply _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out EntryEffect _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out ExploreRecord _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out InformationStock _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out ItemInventory _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out LineApply _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out MovementPath _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out SubGoalEffect _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out SubGoalPath _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out CommentApply _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out StatusBuffApply _)) Debug.Log("ないよ"); }
        { if (!TryGetComponent(out StatusBuffEffect _)) Debug.Log("ないよ"); }
    }
}
