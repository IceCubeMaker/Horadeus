using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : PoolObject {

    public override Type GetPoolObjectType() {
        return typeof(Entity);
    }

}
