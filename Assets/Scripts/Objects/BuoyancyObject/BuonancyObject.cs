using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BuonancyObject : MonoBehaviour
{
    public Transform[] Floaters;

    [SerializeField] private float UnderWaterDrag = 3;
    [SerializeField] private float UnderWaterAngularDrag = 1;
    [SerializeField] private float AirDrag = 0;
    [SerializeField] private float AirAngularDrag = 0.05f;
    [SerializeField] private float FloatingPower = 15f;

    private Rigidbody m_Rigidbody;
    private bool _underWater;
    private int _floatersUnderWater;
    private float _waterHeight;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        _waterHeight = GameObject.FindWithTag("Ocean").transform.position.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _floatersUnderWater = 0;
        foreach (Transform t in Floaters)
        {
            float difference = t.position.y - _waterHeight;
            if (difference < 0)
            {
                m_Rigidbody.AddForceAtPosition(Vector3.up * FloatingPower * Mathf.Abs(difference), t.position, ForceMode.Force);
                _floatersUnderWater += 1;
                if (!_underWater)
                {
                    _underWater = true;
                    SwitchState(true);
                }
            }
        }

        if (_underWater && _floatersUnderWater == 0)
        {
            _underWater = false;
            SwitchState(false);
        }
    }

    private void SwitchState(bool isUnderWater)
    {
        if( isUnderWater )
        {
            m_Rigidbody.drag = UnderWaterDrag;
            m_Rigidbody.angularDrag = UnderWaterAngularDrag;
        }
        else
        {
            m_Rigidbody.drag = AirDrag;
            m_Rigidbody.angularDrag = AirAngularDrag;
        }
    }
}
