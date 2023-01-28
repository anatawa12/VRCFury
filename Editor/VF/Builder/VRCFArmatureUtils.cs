using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using VF.Builder.Exceptions;

namespace VF.Builder {
    public class VRCFArmatureUtils {
        private static FieldInfo parentNameField = 
            typeof(SkeletonBone).GetField("parentName", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        
        /**
         * This basically does what Animator.GetBoneTransform SHOULD do, except GetBoneTransform randomly sometimes
         * returns bones on clothing armatures instead of the avatar, and also sometimes returns null for no reason.
         */
        public static GameObject FindBoneOnArmature(GameObject avatarObject, HumanBodyBones findBone) {
            var animator = avatarObject.GetComponent<Animator>();
            if (!animator || !animator.avatar) {
                return null;
            }

            var humanDescription = animator.avatar.humanDescription;
            var humanBoneName = HumanTrait.BoneName[(int)findBone];
            var avatarBoneName = humanDescription.human
                .FirstOrDefault(humanBone => humanBone.humanName == humanBoneName)
                .boneName;

            var skeleton = humanDescription.skeleton;
            bool DoesBoneMatch(GameObject obj, SkeletonBone bone) {
                if (bone.name != obj.name) return false;
                if (obj.transform.parent.gameObject != avatarObject) {
                    var boneParentName = (string)parentNameField.GetValue(bone);
                    if (boneParentName != obj.transform.parent.name) return false;
                }
                return true;
            }
            bool IsProbablyInSkeleton(GameObject obj) {
                if (obj == null) return false;
                if (obj == avatarObject) return true;
                if (!skeleton.Any(b => DoesBoneMatch(obj, b))) return false;
                return IsProbablyInSkeleton(obj.transform.parent.gameObject);
            }
            var eligibleAvatarBones = avatarObject.GetComponentsInChildren<Transform>(true)
                .Where(t => t.name == avatarBoneName)
                .Select(t => t.gameObject)
                .Where(IsProbablyInSkeleton)
                .ToList();
            if (eligibleAvatarBones.Count == 0) {
                return null;
            }
            if (eligibleAvatarBones.Count > 1) {
                throw new VRCFBuilderException(
                    "Found multiple possible matching " + findBone + " bones on avatar.");
            }
            return eligibleAvatarBones[0];
        }
    }
}