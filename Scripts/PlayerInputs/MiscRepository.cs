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

        public GameObject CircleGameObjectPrefab = null;
        public Material   CircleMaterial         = null;
        public Material   CircleMaterialFaded    = null;
        public GameObject BuildIconPrefab        = null;
        public GameObject SpawnIconPrefab        = null;
    }
}