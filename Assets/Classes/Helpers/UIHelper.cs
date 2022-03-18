// =============================================================================== \\
//                      © Oleg Tolmachev [OKRT] 2022                               \\
// =============================================================================== \\
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Arχæolog.Classes.Helpers
{
    /// <summary>
    /// The <see cref="UIHelper"/> class.
    /// </summary>
    public static class UIHelper
    {
        #region Properties

        /// <summary> Gets the UI layer. </summary>
        public static int UILayer => LayerMask.NameToLayer("UI");

        #endregion

        #region  Methods

        private static GameObject[] UIElements(List<RaycastResult> eventSystemRaysastResults)
        {
            List<GameObject> list = new List<GameObject>();

            // iterate through all the results and check which of them belong to UI (layer)
            for (int index = 0; index < eventSystemRaysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                // check the layer
                if (curRaysastResult.gameObject.layer == UILayer)
                    list.Add(curRaysastResult.gameObject);
            }

            // check the found objects
            if (list.Count > 0)
                return list.ToArray();

            return null;
        }

        /// <summary>
        /// Performs raycast by the current event system.
        /// </summary>
        /// <param name="useTouch">this flag indicates whether to check touch input or mouse.</param>
        /// <returns>list of raycast results.</returns>
        static List<RaycastResult> GetEventSystemRaycastResults(bool useTouch)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);

            // check the input method
            if (useTouch) eventData.position = Touchscreen.current.primaryTouch.position.ReadValue();
            else eventData.position = Mouse.current.position.ReadValue();

            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);

            return raysastResults;
        }

        /// <summary>
        /// Gets the UI elements underneath the current "cursor".
        /// </summary>
        /// <param name="useTouch">this flag indicates whether to check touch input or mouse.</param>
        /// <returns>array of UI elements</returns>
        public static GameObject[] GetUIElements(bool useTouch) => UIElements(GetEventSystemRaycastResults(useTouch));

        #endregion
    }
}
