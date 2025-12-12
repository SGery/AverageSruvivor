using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeController : WeaponController
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Attack()
    {
        base.Attack();

        GameObject spawnedAxeR = Instantiate(weaponData.Prefab);
        spawnedAxeR.transform.position = transform.position;
        spawnedAxeR.GetComponent<AxeBehaviour>().DirectionChecker(Vector3.right);

        GameObject spawnedAxeL = Instantiate(weaponData.Prefab);
        spawnedAxeL.transform.position = transform.position;
        spawnedAxeL.GetComponent<AxeBehaviour>().DirectionChecker(Vector3.left);
    }
}
