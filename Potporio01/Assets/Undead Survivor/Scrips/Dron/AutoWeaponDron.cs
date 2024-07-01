using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoWeaponDron : MonoBehaviour
{
    [SerializeField] Transform trsAuto1;
    //[SerializeField] Transform trsPlayer;
    //[SerializeField] Transform egoWeapon1;//����

    Transform trsPos;

    private void Awake()
    {
        trsPos = trsAuto1;
        transform.parent.position = trsPos.position;//��ġ �ʱ�ȭ

    }
    // Update is called once per frame
    void Update()
    {
        autocam();
    }
    /// <summary>
    /// �÷��̾ ����ٴϴ� ���
    /// </summary>
    private void autocam()
    {
        Vector3 fixpos = trsAuto1.position;
        fixpos.z = transform.position.z;
        transform.position = fixpos;
    }
}