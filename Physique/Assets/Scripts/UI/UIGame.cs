using a;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIGame : MonoBehaviour
    {
        [SerializeField] private Image powerImage;

        private BallPlayer _ballPlayer;

        private void Start()
        {
            _ballPlayer = GameManager.Instance._ballPlayer;
            _ballPlayer.OnChargeChanged += ShowPowerProgress;
            powerImage.fillAmount = 0f;
        }

        private void OnDestroy()
        {
            if (_ballPlayer != null)
                _ballPlayer.OnChargeChanged -= ShowPowerProgress;
        }

        private void ShowPowerProgress(float charge01)
        {
            powerImage.fillAmount = charge01;
        }
    }
}