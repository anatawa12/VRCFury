using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VF.Utils {
    public static class EditorCurveBindingExtensions {
        /**
         * Used to make sure that two instances of EditorCurveBinding equal each other,
         * even if they have different discrete settings, etc
         */
        public static EditorCurveBinding Normalize(this EditorCurveBinding binding) {
            return EditorCurveBinding.FloatCurve(binding.path, binding.type, binding.propertyName);
        }
        
        public static bool IsMuscle(this EditorCurveBinding binding) {
            if (binding.path != "") return false;
            if (binding.type != typeof(Animator)) return false;

            var name = binding.propertyName;
            return GetHumanMuscleList().Contains(name)
                   || name.EndsWith(" Stretched")
                   || name.EndsWith(".Spread")
                   || name.EndsWith(".x")
                   || name.EndsWith(".y")
                   || name.EndsWith(".z")
                   || name.EndsWith(".w");
        }

        private static HashSet<string> _humanMuscleList;
        private static HashSet<string> GetHumanMuscleList() {
            if (_humanMuscleList != null) return _humanMuscleList;
            _humanMuscleList = new HashSet<string>();
            _humanMuscleList.UnionWith(HumanTrait.MuscleName);
            return _humanMuscleList;
        }

        public enum MuscleBindingType {
            None,
            Other,
            LeftHand,
            RightHand
        }

        public static MuscleBindingType GetMuscleBindingType(this EditorCurveBinding binding) {
            if (!binding.IsMuscle()) return MuscleBindingType.None;
            if (binding.propertyName.Contains("LeftHand")) return MuscleBindingType.LeftHand;
            if (binding.propertyName.Contains("RightHand")) return MuscleBindingType.RightHand;
            return MuscleBindingType.Other;
        }
    }
}
