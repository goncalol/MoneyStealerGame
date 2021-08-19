using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Powers
{
    public abstract class Power : MonoBehaviour
    {
        protected int timeBonusActive;

        protected Sprite iconSprite;

        private float sliderStep = .1f;

        public abstract void Execute();
        public abstract void OnPowerFinished();

        protected IEnumerator CountDown()
        {
            var sliderObj = GameController.Instance.PowerSliderGroup;
            var slider = sliderObj.transform.GetChild(0).GetComponent<Slider>() ;
            sliderObj.SetActive(true);
            sliderObj.transform.GetChild(1).GetComponent<Image>().sprite = GetComponent<SpriteRenderer>().sprite;
            for (float i = timeBonusActive; i > 0; i-= sliderStep)
            {
                slider.value = i / timeBonusActive;
                yield return new WaitForSeconds(sliderStep);
            }

            sliderObj.SetActive(false);

            OnPowerFinished();
        }

    }
}
