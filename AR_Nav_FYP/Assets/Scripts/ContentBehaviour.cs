using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AR_NAV_FYP{

    public class ContentBehaviour : MonoBehaviour {
        // Makes the objects invisable to the user.
        // Immersal SDK's OnFirstSuccessfulLocalisation method will set this active again.

        void Start() {
#if !UNITY_EDITOR
            gameObject.SetActive(false);
#endif
        }
    }
}
