using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine.Events;
using Unity.XR.CoreUtils;

public class PolicyManager : MonoBehaviour{
    public Profiles UserProfile = Profiles.Able;

    [SerializeField] private AbleBlockerManager AbleBlockers;
    [SerializeField] private StairsBlockersManager StairsBlockers;
    [SerializeField] private KeycardAccessManager KeycardBlockers;

    [SerializeField] private TMP_Dropdown ProfileDropdown;

    private void Start() {
        FillProfileDropdown();
        ProfileDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        ApplyPolicy();
    }

    private void OnDropdownValueChanged(int index) {
        String selectedOption = ProfileDropdown.options[index].text;
        Debug.Log($"Selected option: {selectedOption}");

        Profiles selected;

        System.Enum.TryParse(selectedOption, out selected);

        Debug.Log($"Selected enum value: {selected}");
        UserProfile = selected;
        ApplyPolicy();
    }

    public void ApplyPolicy(){
        switch (UserProfile) {
            case Profiles.Able: {
                    AbleBlockers.SwitchOn();
                    StairsBlockers.SwitchOff();
                    break;
                }
            case Profiles.Mobility: {
                    AbleBlockers.SwitchOff();
                    StairsBlockers.SwitchOn();
                    break;
                }
        }
    }

    private void FillProfileDropdown() {
        string[] enumNames = System.Enum.GetNames(typeof(Profiles));

        ProfileDropdown.ClearOptions();
        var options = new List<TMP_Dropdown.OptionData>();
        foreach (var enumName in enumNames) {
            options.Add(new TMP_Dropdown.OptionData(enumName));
        }
        ProfileDropdown.AddOptions(options);

        int initialIndex = System.Array.IndexOf(enumNames, UserProfile.ToString());
        ProfileDropdown.value = initialIndex;
    }
}

public enum Profiles {
    Able,
    Mobility,
}
