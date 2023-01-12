using Controllers.Pool;
using DG.Tweening;
using Managers;
using Signals;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Controllers.Player;

namespace Controllers.Player
{
    public class PlayerPhysicsController : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField] private PlayerManager manager;
        [SerializeField] private new Collider collider;
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private PlayerMovementController movementController;

        #endregion

        #endregion

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("StageArea"))
            {
                manager.ForceCommand.Execute();
                CoreGameSignals.Instance.onStageAreaEntered?.Invoke();
                InputSignals.Instance.onDisableInput?.Invoke();
                DOVirtual.DelayedCall(3, () =>
                {
                    var result = other.transform.parent.GetComponentInChildren<PoolController>()
                        .TakeStageResult(manager.StageValue);
                    if (result)
                    {
                        if (manager.StageValue == 0)
                        {
                            Image stageImage1 = GameObject.Find("StageImage1").GetComponent<Image>();
                            stageImage1.color = new Color(0, 255, 0);
                        }

                        else if (manager.StageValue == 1)
                        {
                            Image stageImage2 = GameObject.Find("StageImage2").GetComponent<Image>();
                            stageImage2.color = new Color(0, 255, 0);
                        }

                        else if (manager.StageValue == 2)
                        {
                            Image stageImage3 = GameObject.Find("StageImage3").GetComponent<Image>();
                            stageImage3.color = new Color(0, 255, 0);
                        }

                        CoreGameSignals.Instance.onStageAreaSuccessful?.Invoke(manager.StageValue);
                        InputSignals.Instance.onEnableInput?.Invoke();
                        
                    }
                    else CoreGameSignals.Instance.onLevelFailed?.Invoke();
                });
                return;
            }

           /* if (other.CompareTag("Finish"))
            {
                CoreGameSignals.Instance.onFinishAreaEntered?.Invoke();
                InputSignals.Instance.onDisableInput?.Invoke();
                CoreGameSignals.Instance.onLevelSuccessful?.Invoke();
                return;
            } */ 

            if (other.CompareTag("MiniGame"))
            {
                //Debug.Log("Minigame Started");
                movementController.IsOnMiniGame(true);
                return;
            }

        }

        private int rewardCount;
        public TextMeshProUGUI rewardText;

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Reward"))
            {
                if (movementController.hasStopped)
                {
                    string materialName = other.GetComponent<Renderer>().material.name;

                    string extractedNumbers = string.Empty;

                    foreach (char c in materialName)
                    {
                        if (char.IsDigit(c))
                        {
                            extractedNumbers += c;
                        }
                    }

                    if (!string.IsNullOrEmpty(extractedNumbers))
                    {
                        rewardCount = int.Parse(extractedNumbers);
                        rewardText.text = "+ " + rewardCount;
                        rewardText.transform.GetChild(0).gameObject.GetComponent<Image>().enabled = true;
                        gameObject.SetActive(false);
                        Invoke("DisableReward", 3f);
                        return;
                    }
                }
            }
        }

        void DisableReward(){
            rewardText.transform.parent.gameObject.SetActive(false);
            CoreGameSignals.Instance.onFinishAreaEntered?.Invoke();
            InputSignals.Instance.onDisableInput?.Invoke();
            CoreGameSignals.Instance.onLevelSuccessful?.Invoke();
            return;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            var transform1 = manager.transform;
            var position = transform1.position;
            Gizmos.DrawSphere(new Vector3(position.x, position.y - 1.2f, position.z + 1f), 1.65f);
        }

        internal void OnReset()
        {
        }
    }
}