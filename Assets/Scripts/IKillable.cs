using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKillable 
{
    Field.DestroyResult Destroy(int x, int y);
}
