using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class DisableInspector : PropertyAttribute
{
}