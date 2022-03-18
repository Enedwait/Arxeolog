// =============================================================================== \\
//                      © Oleg Tolmachev [OKRT] 2022                               \\
// =============================================================================== \\
using Arχæolog.Classes.Data;
using UnityEngine;

namespace Arχæolog.Scripts.Data
{
    /// <summary>
    /// The <see cref="PersistableObject"/> class.
    /// </summary>
    [DisallowMultipleComponent]
    internal class PersistableObject : MonoBehaviour
    {
        #region Methods

        /// <summary>
        /// Saves the data.
        /// </summary>
        /// <param name="writer">writer.</param>
        public virtual void Save(GameDataWriter writer)
        {
            writer.Write(transform.position);
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <param name="reader">reader.</param>
        public virtual void Load(GameDataReader reader)
        {
            transform.position = reader.ReadVector3();
        }

        #endregion
    }
}
