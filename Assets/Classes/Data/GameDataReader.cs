// =============================================================================== \\
//                      © Oleg Tolmachev [OKRT] 2022                               \\
// =============================================================================== \\
using System.ComponentModel;
using System.IO;
using UnityEngine;

namespace Arχæolog.Classes.Data
{
    /// <summary>
    /// The <see cref="GameDataReader"/> class.
    /// </summary>
	internal sealed class GameDataReader
    {
        #region Fields

        [EditorBrowsable(EditorBrowsableState.Never)]
        private BinaryReader _reader;

        #endregion

        #region Init

        /// <summary>
        /// Initializes a new instance of the <see cref="GameDataReader"/> class.
        /// </summary>
        /// <param name="reader">reader.</param>
        public GameDataReader(BinaryReader reader)
        {
            _reader = reader;
        }

        #endregion

        #region Read Methods

        /// <summary>
        /// Reads float value.
        /// </summary>
        /// <returns>float value.</returns>
        public float ReadFloat() => _reader.ReadSingle();

        /// <summary>
        /// Reads integer value.
        /// </summary>
        /// <returns>integer value.</returns>
        public int ReadInt() => _reader.ReadInt32();

        /// <summary>
        /// Reads byte value.
        /// </summary>
        /// <returns>byte value.</returns>
        public int ReadByte() => _reader.ReadByte();

        /// <summary>
        /// Reads vector value.
        /// </summary>
        /// <returns>vector value.</returns>
        public Vector3 ReadVector3()
        {
            Vector3 value;
            value.x = _reader.ReadSingle();
            value.y = _reader.ReadSingle();
            value.z = _reader.ReadSingle();
            return value;
        }

        /// <summary>
        /// Reads string value.
        /// </summary>
        /// <returns>string value.</returns>
        public string ReadString() => _reader.ReadString();

        /// <summary>
        /// Reads random state value.
        /// </summary>
        /// <returns>random state value.</returns>
        public Random.State ReadRandomState() => JsonUtility.FromJson<Random.State>(_reader.ReadString());

        #endregion
    }
}
