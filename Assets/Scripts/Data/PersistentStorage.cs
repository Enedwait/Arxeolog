// =============================================================================== \\
//                      © Oleg Tolmachev [OKRT] 2022                               \\
// =============================================================================== \\
using System.IO;
using Arχæolog.Classes.Data;
using UnityEngine;

namespace Arχæolog.Scripts.Data
{
    /// <summary>
    /// The <see cref="PersistentStorage"/> class.
    /// </summary>
	internal sealed class PersistentStorage : MonoBehaviour
    {
        #region Properties
        
        public string SavePath { get; private set; }

        #endregion

        #region Lifecycle Methods

        void Awake()
        {
            SavePath = Path.Combine(Application.persistentDataPath, "savedgame.dat");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Saves the specified persistable object.
        /// </summary>
        /// <param name="persistableObject">persistable object.</param>
        public void Save(PersistableObject persistableObject)
        {
            if (persistableObject == null)
                return;

            using (var writer = new BinaryWriter(File.Open(SavePath, FileMode.Create)))
                persistableObject.Save(new GameDataWriter(writer));
        }

        /// <summary>
        /// Loads the specified persistable object.
        /// </summary>
        /// <param name="persistableObject">persistable object.</param>
        public void Load(PersistableObject persistableObject)
        {
            if (persistableObject == null)
                return;

            if (!File.Exists(SavePath))
                return;

            using (var reader = new BinaryReader(File.Open(SavePath, FileMode.Open)))
                persistableObject.Load(new GameDataReader(reader));
        }

        #endregion
    }
}
