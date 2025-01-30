using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PostActionEvaluator : MonoBehaviour
    {
        AvailableActions _actions;

        void Awake()
        {
            _actions = GetComponent<AvailableActions>();
        }

        public void Evaluate(ActionResult result)
        {
            if (result == null)
            {
                Debug.LogWarning("スコア付けに必要な行動結果の値がnullになっている。");
                return;
            }

            if (result.Action == "Move")
            {
                if (result.Result == "Success")
                {
                    // しばらく移動したら箱や樽を漁る行動を選択するよう促す。
                    float scavengeWeight = _actions.GetWeight("Scavenge");
                    scavengeWeight += 0.1f; // 値は調整必要かも。
                    scavengeWeight = Mathf.Clamp01(scavengeWeight);
                    _actions.SetWeight("Scavenge", scavengeWeight);
                }
                else if (result.Result == "Failure")
                {
                    //
                }
                else
                {
                    Debug.LogWarning($"移動の結果に対するスコア付けが出来ない。{result.Result}");
                }
            }
            else if (result.Action == "Attack")
            {
                if (result.Result == "Defeat")
                {
                    //
                }
                else if (result.Result == "Hit")
                {
                    //
                }
                else if (result.Result == "Corpse")
                {
                    //
                }
                else if (result.Result == "Miss")
                {
                    //
                }
                else
                {
                    Debug.LogWarning($"攻撃の結果に対するスコア付けが出来ない。{result.Result}");
                }
            }
            else if (result.Action == "Talk")
            {
                if (result.Result == "Success")
                {
                    //
                }
                else if (result.Result == "Failure")
                {
                    //
                }
                else
                {
                    Debug.LogWarning($"会話の結果に対するスコア付けが出来ない。{result.Result}");
                }
            }
            else if (result.Action == "Scavenge")
            {
                if (result.Result == "Success")
                {
                    //
                }
                else if (result.Result == "Failure")
                {
                    // 失敗する度に低下するが、0を下回ることは無い。
                    // 目の前に箱や樽があるのにAIが漁るという選択肢を取ることが出来なくなるのを防ぐため。
                    float weight = _actions.GetWeight("Scavenge");
                    weight -= 1.0f; // 値は調整必要かも。
                    weight = Mathf.Clamp01(weight);
                    _actions.SetWeight("Scavenge", weight);
                }
                else
                {
                    Debug.LogWarning($"漁るの結果に対するスコア付けが出来ない。{result.Result}");
                }
            }
            else if (result.Action == "Help")
            {
                //
            }
            else if (result.Action == "Throw")
            {
                //
            }
            else
            {
                Debug.LogWarning($"行動後のスコア付けが出来ない。{result.Action}");
            }
        }
    }
}