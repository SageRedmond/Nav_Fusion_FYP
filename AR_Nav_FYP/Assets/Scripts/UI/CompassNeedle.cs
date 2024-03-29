using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassNeedle : MonoBehaviour
{
    [SerializeField] private NavController NavInfo;

    private Vector3 kReferenceVector;
    private Vector3 _mTempVector;
    private float _mTempAngle;

    public bool compassActive = false;
    private void Update() {
        if (compassActive) {
            _mTempVector = NavInfo.cameraPose.forward;
            _mTempVector.y = 0f;
            _mTempVector = _mTempVector.normalized;

            kReferenceVector = NavInfo.nextPathPointVector();

            _mTempVector = _mTempVector - kReferenceVector;
            _mTempVector.y = 0;
            _mTempVector = _mTempVector.normalized;

            if (_mTempVector == Vector3.zero) {
                _mTempVector = new Vector3(1, 0, 0);
            }

            //  add 90 degrees to set it relative to the z-axis

            _mTempAngle = Mathf.Atan2(_mTempVector.x, _mTempVector.z);
            _mTempAngle = (_mTempAngle * Mathf.Rad2Deg + 90.0f) * 2f;

            Debug.Log("Angle: " + _mTempAngle);

            transform.rotation = Quaternion.AngleAxis(_mTempAngle, kReferenceVector);
        }
    }
}
