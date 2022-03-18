// =============================================================================== \\
//                      © Oleg Tolmachev [OKRT] 2022                               \\
// =============================================================================== \\
using System;
using Arχæolog.Classes.Data;
using Arχæolog.Scripts.Data;
using UnityEngine;

namespace Arχæolog.Scripts.Objects
{
    /// <summary>
    /// The <see cref="Sack"/> class.
    /// </summary>
    internal sealed class Sack : PersistableObject
    {
        #region Serialized Fields

        [SerializeField] private ItemInfo goldBar;

        #endregion

        #region Properties

        /// <summary> Gets the gold bar info. </summary>
        public ItemInfo GoldBar => goldBar;

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified amount of items to the sack.
        /// </summary>
        /// <param name="count">count of items.</param>
        public void Add(int count = 1)
        {
            goldBar.count += count;
            OnChanged();
        }

        /// <summary>
        /// Clears the sack.
        /// </summary>
        public void Clear()
        {
            goldBar.count = 0;
            OnChanged();
        }

        /// <summary>
        /// Saves the sack data.
        /// </summary>
        /// <param name="writer">writer.</param>
        public override void Save(GameDataWriter writer)
        {
            writer.Write(goldBar.count);
        }

        /// <summary>
        /// Loads the sack data.
        /// </summary>
        /// <param name="reader">reader.</param>
        public override void Load(GameDataReader reader)
        {
            goldBar.count = reader.ReadInt();
        }

        #endregion

        #region Events

        /// <summary> Occurs when the sack's content changed. </summary>
        public event EventHandler Changed;

        /// <summary> Raises the 'Changed' event of the sack. </summary>
        private void OnChanged() => Changed?.Invoke(this, EventArgs.Empty);

        #endregion
    }
}
