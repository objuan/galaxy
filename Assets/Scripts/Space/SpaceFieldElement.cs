using JetBrains.Annotations;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.SpeedTree.Importer;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;


public class SpaceFieldElement : MonoBehaviour
{
    public Vector2 fieldPosition;

    public SpaceField spaceField;

    private void OnEnable()
    {
        spaceField = GO.Instance<SpaceField>(); 

    }
    private void Start()
    {
         

    }
}