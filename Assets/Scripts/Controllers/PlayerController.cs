// =============================================================================== \\
//                      © Oleg Tolmachev [OKRT] 2022                               \\
// =============================================================================== \\
using System;
using System.ComponentModel;
using Arχæolog.Classes.Data;
using Arχæolog.Classes.Helpers;
using Arχæolog.Scripts.Data;
using Arχæolog.Scripts.Objects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Arχæolog.Scripts.Controllers
{
    /// <summary>
    /// The <see cref="PlayerController"/> class.
    /// </summary>
    internal sealed class PlayerController : PersistableObject
    {
        #region Serialized Fields

        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private ItemInfo showel;

        #endregion

        #region Fields

        [EditorBrowsable(EditorBrowsableState.Never)]
        private DraggableObject _draggableObject;

        #endregion

        #region Properties

        /// <summary> Gets the showel info </summary>
        public ItemInfo Showel => showel;

        #endregion

        #region Lifecycle Methods

        void LateUpdate()
        {
            Drag();
        }

        #endregion

        #region Methods

        private void Drag()
        {
            if (_draggableObject == null)
                return;

            if (playerInput.currentControlScheme == "Mouse&Keyboard")
                _draggableObject?.Drag(LevelController.Instance.GetWorldPosition(Mouse.current.position.ReadValue()));
        }

        #endregion

        #region Player Input

        void OnClick(InputValue value)
        {
            // check if we can proceed
            if (LevelController.Instance.IsWon)
                return;

            bool useTouch = false;
            float click = 0;
            Vector2 clickPosition = Vector2.zero;

            // check the current control scheme
            if (playerInput.currentControlScheme == "Mouse&Keyboard") // if "Mouse" ("Mouse&Keyboard") scheme then we need only position and click value for "click"/"release" processing
            {
                clickPosition = Mouse.current.position.ReadValue();
                click = value.Get<float>();
            }
            else if (playerInput.currentControlScheme == "Touch") // if "Touch" scheme then we need the phase of the touch input
            {
                useTouch = true;
                clickPosition = Touchscreen.current.primaryTouch.position.ReadValue();

                TouchState state = value.Get<TouchState>();
                // check what touch state phase is current
                if (state.phase == TouchPhase.Began) // assume as "click"
                    click = 1;
                else if (state.phase == TouchPhase.Canceled || state.phase == TouchPhase.Ended || state.phase == TouchPhase.None) // assume as "release"
                    click = 0;
                else // assume as "drag"
                {
                    _draggableObject?.Drag(LevelController.Instance.GetWorldPosition(state.position));
                    return;
                }
            }

            Vector3 worldPosition = LevelController.Instance.GetWorldPosition(clickPosition);

            // check if the draggable object is attached
            if (_draggableObject != null)
            {
                // heck the click value
                if (click > 0) // process the "click"
                {
                    _draggableObject.Drag(worldPosition);
                }
                else // process the "release"
                {
                    Sack sack = null;
                    var elements = UIHelper.GetUIElements(useTouch);
                    foreach (var element in elements)
                    {
                        sack = element?.GetComponent<Sack>();
                        if (sack != null)
                            break;
                    }

                    // check if the sack is found
                    if (sack != null) 
                    {
                        Item item = _draggableObject.GetComponent<Item>();
                        // check if the draggable object is an item
                        if (item != null) // then place the item in the sack
                        {
                            _draggableObject.Drop();
                            sack.Add(item.Info.count);
                            Destroy(_draggableObject.gameObject);
                            _draggableObject = null;
                            return;
                        }
                    }

                    // if sack nor found or draggable object ain't an item then we need to check if the tile exists underneath the draggable object
                    if (LevelController.Instance.HasTileBelow(worldPosition, out Vector3 center)) // then we can drop the draggable object on the tile
                        _draggableObject.Drop(center);
                    else // we can't drop the draggable object here so move it back to where it belongs
                        _draggableObject?.Return();

                    _draggableObject = null;
                }

                return;
            }

            // if we had no draggable object attached then we need to check if we in the "click" state
            if (click > 0) // then we need to determine what's there under cursor
            {
                RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
                if (hit.transform != null)
                {
                    string layer = LayerMask.LayerToName(hit.transform.gameObject.layer);
                    // check if we have an item under the cursor
                    if (layer.Equals("Item")) // then pick it up
                    {
                        DraggableObject draggableObject = hit.transform.GetComponent<DraggableObject>();
                        // check if we already have some other draggable object attached
                        if (_draggableObject != null && _draggableObject != draggableObject) // then return it to where it belongs 
                            _draggableObject.Return();

                        // pick the item
                        _draggableObject = draggableObject;
                        _draggableObject.BeginDrag();
                    }

                    return;
                }

                // check if we can dig here
                if (showel.count > 0 && LevelController.Instance.DigAt(worldPosition))
                {
                    // dig
                    showel.count--;
                    OnDug();
                }
            }
        }

        #endregion

        #region Storage

        /// <summary>
        /// Saves the player data.
        /// </summary>
        /// <param name="writer">writer.</param>
        public override void Save(GameDataWriter writer)
        {
            writer.Write(showel.count);
        }

        /// <summary>
        /// Loads the player data.
        /// </summary>
        /// <param name="reader">reader.</param>
        public override void Load(GameDataReader reader)
        {
            showel.count = reader.ReadInt();
        }

        #endregion

        #region Events

        /// <summary> Occurs when the player dug the cell. </summary>
        public event EventHandler Dug;

        /// <summary> Raises the 'Dug' event. </summary>
        private void OnDug() => Dug?.Invoke(this, EventArgs.Empty);

        #endregion
    }
}
