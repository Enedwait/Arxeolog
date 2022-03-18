// =============================================================================== \\
//                      © Oleg Tolmachev [OKRT] 2022                               \\
// =============================================================================== \\
using UnityEngine;

namespace Arχæolog.Scripts.Objects
{
    /// <summary>
    /// The <see cref="ItemInfo"/> class.
    /// </summary>
    [CreateAssetMenu(fileName = "New item info", menuName = "Items")]
    internal sealed class ItemInfo : ScriptableObject
    {
        #region Public

        /// <summary> The name of the item. </summary>
        public new string name;

        /// <summary> The image of the item. </summary>
        public Sprite image;

        /// <summary> The item count.  </summary>
        public int count;

        #endregion
    }
}
