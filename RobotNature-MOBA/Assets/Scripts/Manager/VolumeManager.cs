using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VolumeManager : MonoBehaviour
{
   public static VolumeManager Instance;

   private Volume volume;
   public ColorAdjustments colorAdjustments;

   private void Awake()
   {
      if(Instance != null) return;
      Instance = this;
   }


   private void Start()
   {
      volume = GetComponent<Volume>();
      volume.profile.TryGet(out colorAdjustments);
   }
}
