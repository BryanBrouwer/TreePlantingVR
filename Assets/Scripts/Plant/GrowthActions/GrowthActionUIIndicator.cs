using UnityEngine;
using UnityEngine.UI;

namespace Plant.GrowthActions
{
    public class GrowthActionUIIndicator : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image actionIconImage;
        
        public void SetActionIcon(Sprite actionIcon)
        {
            actionIconImage.sprite = actionIcon;
        }
        
        public void SetIntervalState(int intervalState)
        {
            switch (intervalState)
            {
                case 1:
                    backgroundImage.color = Color.gray;
                    break;
                case 2:
                    backgroundImage.color = Color.yellow;
                    break;
                case 3:
                    backgroundImage.color = Color.red;
                    break;
            }
        }
    }
}