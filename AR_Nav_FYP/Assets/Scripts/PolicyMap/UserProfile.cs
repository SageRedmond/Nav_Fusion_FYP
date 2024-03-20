using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AR_NAV_FYP.Policy {
    public class UserProfile : MonoBehaviour
    {
        [Header("Keycard Access")]
        public List<string> KeyCardAccess = new List<string>(); //load form json

        [Header("Accessability")]
         public bool DontUseStairs;

        public bool IsWheelchairUser;

        [Header("Personal")]
        public bathroomPrefrence bathroomPref;

        private void Awake() {
            fillKeyCardDictionary();
        }

        private void fillKeyCardDictionary() {
            // FUTURE: JSON
            KeyCardAccess.Add("2ndFloor_Eolas_Engi");
        }

    }

    public enum bathroomPrefrence {
        Mens,
        Womens,
        Disability,
        All_Gender
    }
}