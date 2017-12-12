using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ProjectileScripts.ProjectileData
{
    internal class ProjectileRepo : MonoBehaviour
    {
        public static ProjectileRepo Instance { get; private set; }
        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public List<ProjectileAttributes> ProjectileAttributesList;
        public Dictionary<string, int> NameToIndex = new Dictionary<string, int>();
        [HideInInspector] public List<string> IndexToName;
        protected void Start()
        {
            if (ProjectileAttributesList == null)
            {
                Debug.LogWarning("Projectile Attributes List was not set!!!");
                return;
            }
            IndexToName = new List<string>(ProjectileAttributesList.Count);
            for (var i = 0; i < ProjectileAttributesList.Count; i++) IndexToName.Add(null);
            foreach (var projectileAttribute in ProjectileAttributesList)
            {
                NameToIndex.Add(projectileAttribute.Name, projectileAttribute.Index);
                IndexToName[projectileAttribute.Index] = projectileAttribute.Name;
            }
        }
    }
}
