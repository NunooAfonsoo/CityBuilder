using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ResourceTypes;
using Constants;
using System;

namespace Resources
{
    public class ResourcesUI : MonoBehaviour
    {
        [SerializeField] private Image woodTextBG;
        [SerializeField] private Image stoneTextBG;
        [SerializeField] private Image ironTextBG;
        [SerializeField] private Image goldTextBG;

        [SerializeField] private TextMeshProUGUI woodText;
        [SerializeField] private TextMeshProUGUI stoneText;
        [SerializeField] private TextMeshProUGUI ironText;
        [SerializeField] private TextMeshProUGUI goldText;

        private Coroutine ChangeBackgroundColorCoroutine;
        
        private void Awake()
        {
            ColorUtility.TryParseHtmlString(Colors.ResourceUIBackgroundColor, out Color normalBGColor);

            woodTextBG.color = normalBGColor;
            stoneTextBG.color = normalBGColor;
            ironTextBG.color = normalBGColor;
            goldTextBG.color = normalBGColor;

            UpdateResourceUI(typeof(ResourceTypes.Tree), true, 0);
            UpdateResourceUI(typeof(Stone), true, 0);
            UpdateResourceUI(typeof(Iron), true, 0);
            UpdateResourceUI(typeof(Gold), true, 0);

            ChangeBackgroundColorCoroutine = null;
        }

        private void Start()
        {
            Resource.OnResourceHarvested += Resource_OnResourceHarvested;
        }

        private void Resource_OnResourceHarvested(object sender, Resource.OnResourceHarvestedArgs e)
        {
            if (ChangeBackgroundColorCoroutine != null)
            {
                StopCoroutine(ChangeBackgroundColorCoroutine);
            }

            UpdateResourceUI(sender.GetType(), true, e.harvestAmount);
        }

        private void UpdateResourceUI(Type type, bool increase, int newAmount)
        {
            if (type == typeof(ResourceTypes.Tree))
            {
                int.TryParse(woodText.text, out int wood);

                woodText.text = (wood + newAmount).ToString();
                ChangeBackgroundColorCoroutine = StartCoroutine(ChangeBackgroundColor(woodTextBG, increase));
            }
            else if (type == typeof(Stone))
            {
                int.TryParse(stoneText.text, out int stone);

                stoneText.text = (stone + newAmount).ToString();
                ChangeBackgroundColorCoroutine = StartCoroutine(ChangeBackgroundColor(stoneTextBG, increase));
            }
            else if (type == typeof(Iron))
            {
                int.TryParse(ironText.text, out int iron);

                ironText.text = (iron + newAmount).ToString();
                ChangeBackgroundColorCoroutine = StartCoroutine(ChangeBackgroundColor(ironTextBG, increase));
            }
            else if (type == typeof(Gold))
            {
                int.TryParse(goldText.text, out int gold);

                goldText.text = (gold + newAmount).ToString();
                ChangeBackgroundColorCoroutine = StartCoroutine(ChangeBackgroundColor(goldTextBG, increase));
            }
        }

        private IEnumerator ChangeBackgroundColor(Image resourceImage, bool increase)
        {
            Color color;
            if (increase)
            {
                ColorUtility.TryParseHtmlString(Colors.ResourceUIBackgroundGainColor, out color);
            }
            else
            {
                ColorUtility.TryParseHtmlString(Colors.ResourceUIBackgroundLossColor, out color);
            }

            float backgroundInceasedSize = 1.2f;
            resourceImage.transform.localScale = new Vector3(backgroundInceasedSize, backgroundInceasedSize, backgroundInceasedSize);

            resourceImage.color = color;
            yield return new WaitForSecondsRealtime(0.5f);

            ColorUtility.TryParseHtmlString(Colors.ResourceUIBackgroundColor, out color);
            resourceImage.color = color;
            resourceImage.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}