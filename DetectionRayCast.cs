//Free to use. Please mention my name "Eugene Chu" Twitter: @LenZ_Chu if you can ;3 https://twitter.com/LenZ_Chu
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionRayCast : MonoBehaviour {

    [Header("detect setup")]
    public Light _light;
    public LayerMask detectLayer;
    public LayerMask obsticleMaskLayer;
    public enum DetectFacing
    {
        TransformForward,
        CamLockTransformForward,
        CamForward
    }
    public DetectFacing detectFacing = DetectFacing.TransformForward; //facing detect by camera or transform forward

    public float detectDistance = 10;
    public float lostDistance = 10;
    [Range(0, 180)]
    public float detectAngle = 40;
    public float sightHeight = 1;

    [Header("updateTicker")]
    public float ticker = 1;
    float _ticker;

    [Header("detect debug")]
    public GameObject inSightTarget;
    public GameObject detectTarget;
    public float distTarget;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.1f);
        if (detectFacing != DetectFacing.TransformForward && GetCam() != null) Gizmos.DrawRay(GetDetectTr().position, GetDetectForward());
        if (detectTarget != null)
        {
            if (inSightTarget)
            {
                Gizmos.color = new Color(1, 0, 0, 1);
                Gizmos.DrawSphere(inSightTarget.transform.position, 1);
            }
            else
            {
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Gizmos.DrawSphere(detectTarget.transform.position, 2);
            }
            Gizmos.DrawLine(OffsetCenter(GetDetectTr()) , OffsetCenter(detectTarget.transform));

        }
        Gizmos.color = new Color(1, 1, 1, 0.5f);


        Gizmos.DrawWireSphere(OffsetCenter(transform), lostDistance);
        //radius
#if UNITY_EDITOR

        UnityEditor.Handles.color = new Color(1, 0, 0, 0.1f);
        UnityEditor.Handles.DrawSolidArc(OffsetCenter(GetDetectTr()),
            transform.up,
             GetDetectForward(),
            detectAngle,
            detectDistance);
        UnityEditor.Handles.DrawSolidArc(OffsetCenter(GetDetectTr()),
            transform.up,
             GetDetectForward(),
            -detectAngle,
            detectDistance);

        float _distTarget = distTarget;
        if (inSightTarget == null) _distTarget = detectDistance;

        UnityEditor.Handles.color = new Color(1, 0, 0, 0.1f);
        UnityEditor.Handles.DrawSolidArc(OffsetCenter(GetDetectTr()) + (GetDetectForward() * (_distTarget)),
            GetDetectForward(),
            GetDetectRight(),
            360,
            (detectAngle/6 )*( _distTarget/10));

        UnityEditor.Handles.DrawSolidArc(OffsetCenter(GetDetectTr()),
            transform.right,
             GetDetectForward(),
            detectAngle,
            detectDistance);
        UnityEditor.Handles.DrawSolidArc(OffsetCenter(GetDetectTr()),
            transform.right,
             GetDetectForward(),
            -detectAngle,
            detectDistance);
            
#endif

    }
    private void OnValidate()
    {
        if (lostDistance < detectDistance) lostDistance = detectDistance;

        if (_light != null)
        {
            sightHeight = 0;
            detectAngle = _light.spotAngle / 2;
            detectDistance = _light.range;
        }

    }
    // Update is called once per frame
    void Update () {
        TickerUpdate();
    }

    void TickerUpdate() //updated per sec
    {
        if (_ticker <= 0)
        {
            //insert logic
            DetectLogic();
            _ticker = ticker;
        }
        else
        {
            _ticker -= Time.deltaTime;
        }
    }
    //==detect Tools
    Vector3 OffsetCenter(Transform _tr)
    {
        if (detectFacing != DetectFacing.TransformForward ) return _tr.position + (GetCam().transform.forward);
        else return _tr.position + (transform.up * sightHeight) ;
        //return _tr.position + (transform.up * sightHeight) + (transform.forward * 0.5f);

    }
    Camera GetCam()
    {
       if (Application.isPlaying) return  Camera.main;
        else return Camera.current;

    }
    //============
    Vector3 GetDetectForward()
    {
        if (detectFacing == DetectFacing.TransformForward) return transform.forward;
        else if (detectFacing == DetectFacing.CamLockTransformForward && Camera.main != null) return new Vector3(GetCam().transform.forward.x, 0, GetCam().transform.forward.z).normalized;
        else if (detectFacing == DetectFacing.CamForward && GetCam() != null) return GetCam().transform.forward;
        return transform.forward;
    }
    Vector3 GetDetectRight()
    {
        if (detectFacing == DetectFacing.TransformForward) return transform.right;
        else if (detectFacing == DetectFacing.CamLockTransformForward && GetCam() != null) return new Vector3(GetCam().transform.right.x, 0, GetCam().transform.right.z).normalized;
        else if (detectFacing == DetectFacing.CamForward && GetCam() != null) return GetCam().transform.right;
        return transform.right;
    }
    Transform GetDetectTr()
    {
        if (detectFacing == DetectFacing.TransformForward) return transform;
        else if (detectFacing == DetectFacing.CamLockTransformForward && GetCam() != null) return GetCam().transform;
        else if (detectFacing == DetectFacing.CamForward && GetCam() != null) return GetCam().transform;
        return transform;
    }
    //==========
    void DetectLogic()
    {

        Collider[] hitColliders;
        hitColliders = Physics.OverlapSphere(OffsetCenter(transform), detectDistance, detectLayer);
        float clostestDist = detectDistance+1;
        GameObject clostestActor = null;

        for (int i = 0; i < hitColliders.Length; i++)
        {

            if (hitColliders[i].gameObject != null) //requires an actor
            {
                float hitDistance = (hitColliders[i].transform.position - OffsetCenter(GetDetectTr())).magnitude;
                /* debug
                RaycastHit hit;
                Physics.Raycast(OffsetCenter(GetDetectTr()), (OffsetCenter(hitColliders[i].transform) - OffsetCenter(GetDetectTr())).normalized,out hit, hitDistance, ~obsticleMaskLayer);
                if (hit.collider != null) print(hit.collider.gameObject.name);
                */
                //debug angle
                //print(Vector3.Angle((hitColliders[i].transform.position - OffsetCenter(transform)).normalized, GetDetectForward()));

                bool obsticle = false;
                obsticle = Physics.Raycast(OffsetCenter(GetDetectTr()), (OffsetCenter(hitColliders[i].transform) - OffsetCenter(GetDetectTr())).normalized, hitDistance, obsticleMaskLayer & ~detectLayer);
                if (hitDistance <= clostestDist && Vector3.Angle((hitColliders[i].transform.position - OffsetCenter(transform)).normalized, GetDetectForward()) <= detectAngle && !obsticle)
                {
                    clostestDist = hitDistance;
                    clostestActor = hitColliders[i].gameObject;

                }
            }

        }
        inSightTarget = clostestActor;

        if (detectTarget == null)
        {
            detectTarget = inSightTarget;
        }
        else //if detectTarget has target
        {
            distTarget = (detectTarget.transform.position - OffsetCenter(transform)).magnitude;
            if (distTarget > lostDistance) //if target gets too far away, loose the target
            {
                detectTarget = null;
                distTarget = 0;
            }
            else
            {
               if (inSightTarget != null) detectTarget = inSightTarget;
            }
        }
    }

}
