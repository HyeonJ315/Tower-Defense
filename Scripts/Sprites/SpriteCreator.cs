#if (UNITY_EDITOR)

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Sprites
{
    public class SpriteCreator : MonoBehaviour
    {
        public List<string> SpriteSheetList = new List<string>();
        private List<Sprite> _spriteArray;

        private string _saveFolder;

        private const string Assets = "Assets";
        private const string AnimatorName = "Animator";

        private const string IdleFront = "IdleFront";
        private const string IdleUp    = "IdleUp";
        private const string IdleLeft  = "IdleLeft";
        private const string IdleRight = "IdleRight";

        private const string CorpseFront = "CorpseFront";
        private const string CorpseUp    = "CorpseUp";
        private const string CorpseLeft  = "CorpseLeft";
        private const string CorpseRight = "CorpseRight";

        private const string WalkFront = "WalkFront";
        private const string WalkUp    = "WalkUp";
        private const string WalkLeft  = "WalkLeft";
        private const string WalkRight = "WalkRight";

        private const string DieFront = "DieFront";
        private const string DieUp    = "DieUp";
        private const string DieLeft  = "DieLeft";
        private const string DieRight = "DieRight";

        public GameObject PrefabTemplate;
        public GameObject IconPrefabTemplate;

        // Use this for initialization
        protected void Start ()
        {
            foreach (var spriteSheet in SpriteSheetList)
            {
                _saveFolder = Assets + "\\" + spriteSheet;
                _spriteArray = new List<Sprite>(Resources.LoadAll<Sprite>(spriteSheet));

                AssetDatabase.CreateFolder(Assets, spriteSheet);
                for (var i = 0; i < _spriteArray.Count; i += 8)
                {
                    var index = i / 8;

                    var spritesFront = _spriteArray.GetRange(i + 0, 2);
                    var spritesUp = _spriteArray.GetRange(i + 2, 2);
                    var spritesLeft = _spriteArray.GetRange(i + 4, 2);
                    var spritesRight = _spriteArray.GetRange(i + 6, 2);

                    AssetDatabase.CreateFolder(_saveFolder, index.ToString());

                    var dieFront = CreateAnimationClip(_saveFolder + "\\" + index, DieFront, 1,
                        spritesFront.GetRange(0, 1));
                    var dieUp = CreateAnimationClip(_saveFolder + "\\" + index, DieUp, 1, spritesUp.GetRange(0, 1));
                    var dieLeft = CreateAnimationClip(_saveFolder + "\\" + index, DieLeft, 1,
                        spritesLeft.GetRange(0, 1));
                    var dieRight = CreateAnimationClip(_saveFolder + "\\" + index, DieRight, 1,
                        spritesRight.GetRange(0, 1));

                    var corpseFront = CreateAnimationClip(_saveFolder + "\\" + index, CorpseFront, 1,
                        spritesFront.GetRange(0, 1));
                    var corpseUp = CreateAnimationClip(_saveFolder + "\\" + index, CorpseUp, 1,
                        spritesUp.GetRange(0, 1));
                    var corpseLeft = CreateAnimationClip(_saveFolder + "\\" + index, CorpseLeft, 1,
                        spritesLeft.GetRange(0, 1));
                    var corpseRight = CreateAnimationClip(_saveFolder + "\\" + index, CorpseRight, 1,
                        spritesRight.GetRange(0, 1));

                    var idleFront = CreateAnimationClip(_saveFolder + "\\" + index, IdleFront, 1, spritesFront, true);
                    var idleUp = CreateAnimationClip(_saveFolder + "\\" + index, IdleUp, 1, spritesUp, true);
                    var idleLeft = CreateAnimationClip(_saveFolder + "\\" + index, IdleLeft, 1, spritesLeft, true);
                    var idleRight = CreateAnimationClip(_saveFolder + "\\" + index, IdleRight, 1, spritesRight, true);

                    var walkFront = CreateAnimationClip(_saveFolder + "\\" + index, WalkFront, 6, spritesFront, true);
                    var walkUp = CreateAnimationClip(_saveFolder + "\\" + index, WalkUp, 6, spritesUp, true);
                    var walkLeft = CreateAnimationClip(_saveFolder + "\\" + index, WalkLeft, 6, spritesLeft, true);
                    var walkRight = CreateAnimationClip(_saveFolder + "\\" + index, WalkRight, 6, spritesRight, true);

                    var animatorController = CreateAnimatorController(_saveFolder + "\\" + index, AnimatorName);

                    animatorController.AddMotion(dieFront);
                    animatorController.AddMotion(dieUp);
                    animatorController.AddMotion(dieLeft);
                    animatorController.AddMotion(dieRight);

                    animatorController.AddMotion(corpseFront);
                    animatorController.AddMotion(corpseUp);
                    animatorController.AddMotion(corpseLeft);
                    animatorController.AddMotion(corpseRight);

                    animatorController.AddMotion(walkFront);
                    animatorController.AddMotion(walkUp);
                    animatorController.AddMotion(walkLeft);
                    animatorController.AddMotion(walkRight);

                    animatorController.AddMotion(idleFront);
                    animatorController.AddMotion(idleUp);
                    animatorController.AddMotion(idleLeft);
                    animatorController.AddMotion(idleRight);

                    var prefab = Instantiate(PrefabTemplate);
                    var icon = Instantiate(IconPrefabTemplate);
                    PrefabUtility.DisconnectPrefabInstance(prefab);
                    PrefabUtility.DisconnectPrefabInstance(icon);

                    prefab.GetComponentInChildren<Animator>().runtimeAnimatorController = animatorController;
                    icon.GetComponent<Image>().sprite = spritesFront[0];
                    prefab.name = "_MobPrefab";
                    icon.name = "_MobIcon";

                    PrefabUtility.CreatePrefab(_saveFolder + "/" + index + "/" + prefab.name + ".prefab", prefab);
                    PrefabUtility.CreatePrefab(_saveFolder + "/" + index + "/" + icon.name + ".prefab", icon);

                    Destroy(prefab);
                    Destroy(icon);

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        public AnimationClip CreateAnimationClip( string savePath, string clipName, int framerate, List<Sprite> sprites, bool loop = false )
        {
            var animationClip = new AnimationClip()
            {
                frameRate = framerate,
                name = clipName,
            };
            if (loop)
            {
                var animClipSett = new AnimationClipSettings
                {
                    loopTime = true,
                };
                AnimationUtility.SetAnimationClipSettings(animationClip, animClipSett);
            }
            var spriteBinding = new EditorCurveBinding
            {
                type = typeof(SpriteRenderer),
                path = "",
                propertyName = "m_Sprite"
            };

            var spriteKeyFrames = new ObjectReferenceKeyframe[sprites.Count];
            for (var j = 0; j < sprites.Count; j++)
            {
                spriteKeyFrames[j] = new ObjectReferenceKeyframe
                {
                    time = (float) j / framerate,
                    value = sprites[j],
                };
            }

            AnimationUtility.SetObjectReferenceCurve( animationClip, spriteBinding, spriteKeyFrames );
            AssetDatabase.CreateAsset( animationClip, savePath + "\\" + clipName + ".anim" );
            return animationClip;
        }

        public AnimatorController CreateAnimatorController(string savePath, string controllerName)
        {
            var animatorController = AnimatorController.CreateAnimatorControllerAtPath( savePath + "\\" + controllerName + ".controller" );
            return animatorController;
        }
    }
}

#endif