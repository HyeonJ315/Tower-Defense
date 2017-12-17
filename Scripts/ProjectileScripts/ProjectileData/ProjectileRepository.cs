using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ProjectileScripts.ProjectileData
{
    internal class ProjectileRepository : MonoBehaviour
    {
        public static ProjectileRepository Instance { get; private set; }
        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public List<ProjectileAttributes> ProjectileAttributesList = null;
        public Dictionary<string, int> NameToIndex = new Dictionary<string, int>();
        [HideInInspector] public List<string> IndexToName;
        [HideInInspector] public int ProjectileCount { private set; get; }
        protected void OnEnable()
        {
            if (ProjectileAttributesList == null)
            {
                Debug.LogWarning("Projectile Attributes List was not set!!!");
                return;
            }
            IndexToName = new List<string>(ProjectileAttributesList.Count);
            for (var i = 0; i < ProjectileAttributesList.Count; i++) IndexToName.Add(null);
            for (var i = 0; i < ProjectileAttributesList.Count; i++)
            {
                var projectileAttribute = ProjectileAttributesList[i];
                NameToIndex.Add(projectileAttribute.Name, projectileAttribute.Index);
                IndexToName[ i ] = projectileAttribute.Name;
                projectileAttribute.Index = i;
            }
            ProjectileCount = ProjectileAttributesList.Count;
        }
    }
}
