// =============================================================================== \\
//                      © Oleg Tolmachev [OKRT] 2022                               \\
// =============================================================================== \\
using Arχæolog.Scripts.Objects;
using Arχæolog.Scripts.UI;
using UnityEngine;

namespace Arχæolog.Scripts.Controllers
{
    /// <summary>
    /// The <see cref="UIController"/> class.
    /// </summary>
    internal sealed class UIController : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private PlayerController player;
        [SerializeField] private Sack sack;
        [SerializeField] private CollectedItemDisplay showel;
        [SerializeField] private CollectedItemDisplay goldBar;
        [SerializeField] private GameObject panelVictory;
        [SerializeField] private GameObject panelDefeat;

        #endregion

        #region Properties

        /// <summary> Gets the <see cref="UIController"/> instance. </summary>
        public static UIController Instance { get; private set; }

        #endregion

        #region Lifecycle Methods

        void Awake()
        {
            Instance = this;

            player.Dug += Player_Dug;
            sack.Changed += Sack_Changed;
        }

        void Start()
        {
            Restart();
        }

        void OnDestroy()
        {
            player.Dug -= Player_Dug;
            sack.Changed -= Sack_Changed;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Restarts the UI.
        /// </summary>
        public void Restart()
        {
            showel.Set(player.Showel);
            goldBar.Set(sack.GoldBar);
            panelVictory.SetActive(false);
            panelDefeat.SetActive(false);
        }

        /// <summary>
        /// Sets the 'Win' UI.
        /// </summary>
        public void Win()
        {
            panelVictory.SetActive(true);
        }

        /// <summary>
        /// Sets the 'Lose' UI.
        /// </summary>
        public void Lose()
        {
            panelDefeat.SetActive(true);
        }

        #endregion

        #region Events Handling

        /// <summary>
        /// Handles the 'Changed' event of the sack.
        /// </summary>
        /// <param name="sender">sender.</param>
        /// <param name="e">event args.</param>
        private void Sack_Changed(object sender, System.EventArgs e) => goldBar.Set(sack.GoldBar);

        /// <summary>
        /// Handles the 'Dug' event of the player.
        /// </summary>
        /// <param name="sender">sender.</param>
        /// <param name="e">event args.</param>
        private void Player_Dug(object sender, System.EventArgs e) => showel.Set(player.Showel);

        #endregion
    }
}
