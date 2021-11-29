using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRaycast : MonoBehaviour
{
    /// <summary>
    /// 보고 있는 타겟 오브젝트를 그대로 넘겨주는 함수
    /// </summary>
    /// <param name="_raycastRange">Raycast disance</param>
    /// <param name="targerLayerMasks">Target layer mask. Need shift calculate</param>
    /// <returns>Return target RaycastHit<br/>If target is Empty return null </returns>
    /// 매개변수에 Raycast 대상이 될 LayerMask도 같이 받아서 처리해야겠다.
    /// 그래야지 철책뒤의 적에 대해서 인식을 할 수 있을 것이다.
    public RaycastHit GetRaycastTarget(float _raycastRange, LayerMask targetLayerMasks)
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, _raycastRange, targetLayerMasks)) { }

        return hit;
    }


    public List<RaycastHit> GetWeaponRaycastTarget(float _raycastRange, LayerMask targetLayerMasks, ItemGun.GunType _gunType)
    {
        List<RaycastHit> _returnHit = new List<RaycastHit>();
        RaycastHit hit;

        Vector3 dist = new Vector3(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f));

        if (_gunType != ItemGun.GunType.SG)
        {
            if (Physics.Raycast(transform.position, transform.forward + dist, out hit, _raycastRange, targetLayerMasks))
            {

                _returnHit.Add(hit);
            }
        }
        else
        {
            float _value = 0.1f;

            if (Physics.Raycast(transform.position, transform.forward + dist, out hit, _raycastRange, targetLayerMasks))
            {
                _returnHit.Add(hit);
            }
            if (Physics.Raycast(transform.position, transform.forward + new Vector3(0f, _value, 0f) + dist, out hit, _raycastRange, targetLayerMasks))
            {
                _returnHit.Add(hit);
            }
            if (Physics.Raycast(transform.position, transform.forward + new Vector3(0f, -_value, 0f) + dist, out hit, _raycastRange, targetLayerMasks))
            {
                _returnHit.Add(hit);
            }
            if (Physics.Raycast(transform.position, transform.forward + new Vector3(-_value, 0f, 0f) + dist, out hit, _raycastRange, targetLayerMasks))
            {
                _returnHit.Add(hit);
            }
            if (Physics.Raycast(transform.position, transform.forward + new Vector3(_value, 0f, 0f) + dist, out hit, _raycastRange, targetLayerMasks))
            {
                _returnHit.Add(hit);
            }

        }

        return _returnHit;
    }
}