// =============================================================================== \\
//                      © Oleg Tolmachev [OKRT] 2022                               \\
// =============================================================================== \\
using Arχæolog.Scripts.Data;
using UnityEngine;

namespace Arχæolog.Scripts.Objects
{
    /// <summary>
    /// The <see cref="Item"/> class.
    /// </summary>
    internal sealed class Item : PersistableObject
    {
        #region Serialized Fields

        [SerializeField] private ItemInfo info;

        #endregion

        #region Properties
        
        /// <summary> Gets the item info. </summary>
        public ItemInfo Info => info;

        #endregion
    }
}
