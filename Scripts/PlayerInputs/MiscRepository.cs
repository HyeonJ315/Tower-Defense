using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.PlayerInputs
{
    internal class MiscRepository : MonoBehaviour
    {
        public static MiscRepository Instance { get; private set; }

        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public GameObject CircleGameObjectPrefab;
        public Material   CircleMaterial;
        public Material   CircleMaterialFaded;
        public GameObject BuildIconPrefab;
        public GameObject SpawnIconPrefab;
    }
}