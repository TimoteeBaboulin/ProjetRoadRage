using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UICutoutMask : Image {
    public override Material materialForRendering{
        get{
            Material material = new Material(base.materialForRendering);
            material.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            return material;
        }
    }
}

public class UICutoutManager : MonoBehaviour{
    public static UICutoutManager Instance;

    private Animator _animator;

    private void Awake(){
        if (Instance == null) Instance = this;
        else
            Destroy(gameObject);
        _animator = GetComponent<Animator>();
    }
}