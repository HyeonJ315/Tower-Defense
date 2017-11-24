﻿using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MobScripts.MobData
{
    public class MobHealthBar : MonoBehaviour
    {
        private Image _imageBackground;
        private Image _imageHealth;
        private MobAttributes _current;
        private MobAttributes _max;
        private MobRotation   _mobRotation;
        private Transform     _backgroundTransform;

        private float _defaultAlphaBackground;
        private float _defaultAlphaHealth;

        // Use this for initialization
        protected void Start ()
        {
            var mob = transform.parent.parent.parent.GetComponent<Mob>();

            _imageHealth                  = GetComponent<Image>();
            _imageBackground              = transform.parent.GetComponent<Image>();
            _backgroundTransform          = transform.parent.transform;
            _mobRotation                  = mob.GetComponentInChildren<MobRotation>();
            _current                      = mob.MobAttributesCurrent;
            _max                          = mob.MobAttributesMax;
            var backgroundPos             = _backgroundTransform.position;
            backgroundPos.y               = mob.MobAttributesMax.HealthCanvasY;
            _backgroundTransform.position = backgroundPos;
            _defaultAlphaBackground       = _imageBackground.color.a;
            _defaultAlphaHealth           = _imageHealth.color.a;
        }
	
        // Update is called once per frame
        protected void FixedUpdate ()
        {
            var ratio = _current.Health / _max.Health ;
            _imageHealth.fillAmount = ratio;

            var healthColor = _imageHealth.color;
            var backgroundColor = _imageBackground.color;
            healthColor.a     = Mathf.Lerp( _defaultAlphaHealth    , 0, _mobRotation.FadeValue );
            backgroundColor.a = Mathf.Lerp( _defaultAlphaBackground, 0, _mobRotation.FadeValue );
            _imageHealth.color     = healthColor;
            _imageBackground.color = backgroundColor;
        }
    }
}