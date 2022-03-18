// =============================================================================== \\
//                      © Oleg Tolmachev [OKRT] 2022                               \\
// =============================================================================== \\
using System.ComponentModel;
using System.IO;
using UnityEngine;

namespace Arχæolog.Classes.Data
{
    /// <summary>
    /// The <see cref="GameDataWriter"/> class.
    /// </summary>
    internal sealed class GameDataWriter
    {
        #region Fields

        [EditorBrowsable(EditorBrowsableState.Never)]
        private BinaryWriter _writer;

        #endregion

        #region Init

        /// <summary>
        /// Initializes a new instance of the <see cref="GameDataWriter"/> class.
        /// </summary>
        /// <param name="writer">writer.</param>
        public GameDataWriter(BinaryWriter writer)
        {
            _writer = writer;
        }

        #endregion

        #region Write Methods

        /// <summary>
        /// Writes float value.
        /// </summary>
        /// <returns>float value.</returns>
        public void Write(float value) => _writer.Write(value);

        /// <summary>
        /// Writes integer value.
        /// </summary>
        /// <returns>integer value.</returns>
        public void Write(int value) => _writer.Write(value);

        /// <summary>
        /// Writes byte value.
        /// </summary>
        /// <returns>byte value.</returns>
        public void Write(byte value) => _writer.Write(value);

        /// <summary>
        /// Writes vector value.
        /// </summary>
        /// <returns>vector value.</returns>
        public void Write(Vector3 value)
        {
            _writer.Write(value.x);
            _writer.Write(value.y);
            _writer.Write(value.z);
        }

        /// <summary>
        /// Writes string value.
        /// </summary>
        /// <returns>string value.</returns>
        public void Write(string value) => _writer.Write(value);

        /// <summary>
        /// Writes random state value.
        /// </summary>
        /// <returns>random state value.</returns>
        public void Write(Random.State value) => _writer.Write(JsonUtility.ToJson(value));

        #endregion
    }
}
