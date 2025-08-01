using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class WriteDirectionText : MonoBehaviour
{
  [SerializeField] public TMP_Text DirectionText;

  // private int counter;
  // Start is called before the first frame update
  void Start()
  {
      DirectionText = GetComponent<TMP_Text>();
  }

  void Update()
  {
      NativeState state = NativeStateManager.State;
      string xDirectionText = state.x_direction.ToString("F2");
      string yDirectionText = state.y_direction.ToString("F2");
      string zDirectionText = state.z_direction.ToString("F2");
      writeDirectionText(xDirectionText, yDirectionText, zDirectionText);
  }

  void writeDirectionText(string x, string y, string z)
  {
    string directionString = x + ", " + y + ", " + z;
    DirectionText.text = directionString;
  }
}
