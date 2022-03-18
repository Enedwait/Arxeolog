// =============================================================================== \\
//                      © Oleg Tolmachev [OKRT] 2022                               \\
// =============================================================================== \\
using Arχæolog.Scripts.Objects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Arχæolog.Scripts.UI
{
    /// <summary>
    /// The <see cref="CollectedItemDisplay"/> class.
    /// </summary>
    internal sealed class CollectedItemDisplay : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private ItemInfo item;
        [SerializeField] private TextMeshProUGUI textCount;
        [SerializeField] private Image image;

        #endregion

        #region Properies

        /// <summary> Gets the count of items. </summary>
        public int Count => item.count;

        #endregion

        #region Lifecycle

        void Start()
        {
            UpdateDisplay();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the item info.
        /// </summary>
        /// <param name="itemInfo">item info.</param>
        public void Set(ItemInfo itemInfo)
        {
            item = itemInfo;
            UpdateDisplay();
        }

        /// <summary>
        /// Adds the specified amount of the current item.
        /// </summary>
        /// <param name="count">count.</param>
        public void Add(int count = 0)
        {
            if (item != null)
            {
                item.count += count;
                UpdateDisplay();
            }
        }

        /// <summary>
        /// Updates visuals accordingly.
        /// </summary>
        private void UpdateDisplay()
        {
            textCount.text = $"{item?.count} x";
            image.sprite = item?.image;
        }

        /// <summary>
        /// Clears the set of items.
        /// </summary>
        public void Clear()
        {
            if (item != null)
            {
                item.count = 0;
                UpdateDisplay();
            }
        }

        #endregion
    }
}
