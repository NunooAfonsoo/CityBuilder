using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResourceTypes;
using UnityEngine.UI;
using TMPro;
using Constants;

namespace Resources
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        private int wood;
        private int stone;
        private int iron;
        private int gold;

        [SerializeField] private Image woodBG;
        [SerializeField] private Image stoneBG;
        [SerializeField] private Image ironBG;
        [SerializeField] private Image goldBG;

        [SerializeField] private TextMeshProUGUI woodText;
        [SerializeField] private TextMeshProUGUI stoneText;
        [SerializeField] private TextMeshProUGUI ironText;
        [SerializeField] private TextMeshProUGUI goldText;

        public static event Action<Type, bool> OnResourceChanged;
        private Coroutine ChangeBackgroundColorCoroutine;
        private void Awake()
        {
            Instance = this;

            OnResourceChanged += UpdateResourceUI;

            ColorUtility.TryParseHtmlString(Colors.ResourceUIBackgroundColor, out Color normalBGColor);

            woodBG.color = normalBGColor;
            stoneBG.color = normalBGColor;
            ironBG.color = normalBGColor;
            goldBG.color = normalBGColor;

            UpdateResourceUI(typeof(ResourceTypes.Tree), true);
            UpdateResourceUI(typeof(Stone), true);
            UpdateResourceUI(typeof(Iron), true);
            UpdateResourceUI(typeof(Gold), true);
        }

        private void UpdateResourceUI(Type type, bool increase)
        {
            if(ChangeBackgroundColorCoroutine != null)
            {
                StopCoroutine(ChangeBackgroundColorCoroutine);
            }

            if (type == typeof(ResourceTypes.Tree))
            {
                woodText.text = wood.ToString();
                StartCoroutine(ChangeBackgroundColor(woodBG, increase));
            }
            else if (type == typeof(Stone))
            {
                stoneText.text = stone.ToString();
                StartCoroutine(ChangeBackgroundColor(stoneBG, increase));
            }
            else if (type == typeof(Iron))
            {
                ironText.text = iron.ToString();
                StartCoroutine(ChangeBackgroundColor(ironBG, increase));
            }
            else if (type == typeof(Gold))
            {
                goldText.text = gold.ToString();
                StartCoroutine(ChangeBackgroundColor(goldBG, increase));
            }

        }

        private IEnumerator ChangeBackgroundColor(Image resourceImage, bool increase)
        {
            Color color;
            if(increase)
            {
                ColorUtility.TryParseHtmlString(Colors.ResourceUIBackgroundGainColor, out color);
            }
            else
            {
                ColorUtility.TryParseHtmlString(Colors.ResourceUIBackgroundLossColor, out color);
            }

            resourceImage.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

            resourceImage.color = color;
            yield return new WaitForSecondsRealtime(0.5f);

            ColorUtility.TryParseHtmlString(Colors.ResourceUIBackgroundColor, out color);
            resourceImage.color = color;
            resourceImage.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        public void AddResource(Type type, int quantity)
        {
            if(type == typeof(ResourceTypes.Tree))
            {
                wood += quantity;
            }
            else if(type == typeof(Stone))
            {
                stone += quantity;
            }
            else if (type == typeof(Iron))
            {
                iron += quantity;
            }
            else if (type == typeof(Gold))
            {
                gold += quantity;
            }
            OnResourceChanged?.Invoke(type, quantity >= 0);
        }

        public void UseResource(Type[] type, int[] quantity)
        {
            for(int i = 0; i < type.Length; i++)
            {
                if (type[i] == typeof(ResourceTypes.Tree))
                {
                    wood -= quantity[i];
                }
                else if (type[i] == typeof(Stone))
                {
                    stone -= quantity[i];
                }
                else if (type[i] == typeof(Iron))
                {
                    iron -= quantity[i];
                }
                else if (type[i] == typeof(Gold))
                {
                    gold += quantity[i];
                }
                OnResourceChanged?.Invoke(type[i], quantity[i] >= 0);
            }
        }

        public int GetResource(Type type)
        {
            if (type == typeof(ResourceTypes.Tree))
            {
                return wood;
            }
            else if (type == typeof(Stone))
            {
                return stone;
            }
            else if (type == typeof(Iron))
            {
                return iron;
            }
            else if (type == typeof(Gold))
            {
                return gold;
            }
            return -1;
        }
    }
}
