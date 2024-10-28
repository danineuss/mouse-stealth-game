using Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIPanicMeterMono : MonoBehaviour
    {
        [SerializeField] private EventsMono eventsMono;
        [SerializeField] private Color barDangerColor = Color.white;

        private Slider slider;
        private Image fillImage;
        private Color barDefaultColor;

        void Awake()
        {
            slider = GetComponentInChildren<Slider>();
            fillImage = slider.fillRect.GetComponent<Image>();
            barDefaultColor = fillImage.color;
        
            InitializeEvents();
        }

        private void InitializeEvents()
        {
            eventsMono.PlayerEvents.OnPanicLevelChanged += OnPanicLevelChanged;
        }

        private void OnPanicLevelChanged(float panicLevel)
        {
            slider.value = panicLevel;
        }
    }
}
