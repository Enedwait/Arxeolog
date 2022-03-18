// =============================================================================== \\
//                      © Oleg Tolmachev [OKRT] 2022                               \\
// =============================================================================== \\
using System.ComponentModel;
using UnityEngine;

namespace Arχæolog.Scripts.Objects
{
    /// <summary>
    /// The <see cref="DraggableObject"/> class.
    /// </summary>
    internal sealed class DraggableObject : MonoBehaviour
    {
        #region Fields

        [EditorBrowsable(EditorBrowsableState.Never)]
        private Vector3 _originalPosition;

        #endregion

        #region Properties

        /// <summary> Gets the selection state of the object. </summary>
        public bool IsSelected { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Drags the object to the specified position.
        /// </summary>
        /// <param name="position">position.</param>
        public void Drag(Vector2 position)
        {
            if (IsSelected)
                transform.position = new Vector3(position.x, position.y, transform.position.z);
        }

        /// <summary>
        /// Initiates the dragging of the object.
        /// </summary>
        public void BeginDrag()
        {
            if (IsSelected)
                return;
            
            IsSelected = true;
            _originalPosition = transform.position;
        }

        /// <summary>
        /// Drops the object at the current position.
        /// </summary>
        public void Drop()
        {
            if (!IsSelected)
                return;
            
            _originalPosition = transform.position;
            IsSelected = false;
        }

        /// <summary>
        /// Drops the object at the specified position.
        /// </summary>
        /// <param name="position">position.</param>
        public void Drop(Vector3 position)
        {
            if (!IsSelected)
                return;
            
            transform.position = new Vector3(position.x, position.y, transform.position.z);
            _originalPosition = transform.position;
            IsSelected = false;
        }

        /// <summary>
        /// Drops the object and returns it to the place it was dragged.
        /// </summary>
        public void Return()
        {
            if (!IsSelected)
                return;
            
            transform.position = _originalPosition;
            IsSelected = false;
        }

        #endregion
    }
}
