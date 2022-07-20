using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FootIK : MonoBehaviour
{
    private Animator animator;
    //public Transform rightFoot;
   // public Transform lefFoot;

    [Range(0f, 1f)]
    public float rightFootWeight = 1f;
    [Range(0f, 1f)]
    public float leftFootWeight = 1f;

    void Awake(){
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        Vector3 rightFootPos = animator.GetIKPosition(AvatarIKGoal.RightFoot);
        Vector3 leftFootPos = animator.GetIKPosition(AvatarIKGoal.LeftFoot);

        RaycastHit hit;
        if (Physics.Raycast(rightFootPos + Vector3.up, Vector3.down, out hit)) {
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight);
            animator.SetIKPosition(AvatarIKGoal.RightFoot, hit.point);
        }
        else{
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0f);
        }

        if (Physics.Raycast(leftFootPos + Vector3.up, Vector3.down, out hit))
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, hit.point);
        }
        else
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0f);
        }
    }
}
