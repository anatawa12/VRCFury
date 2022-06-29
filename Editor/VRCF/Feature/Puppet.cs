using UnityEditor.Animations;
using UnityEngine;

namespace VRCF.Feature {

public class Puppet : BaseFeature {
    public void Generate(Model.Feature.Puppet config) {
        var layerName = config.name;
        var layer = manager.NewLayer(layerName);
        var tree = manager.NewBlendTree(config.name);
        tree.blendType = BlendTreeType.FreeformDirectional2D;
        tree.AddChild(noopClip, new Vector2(0,0));
        var i = 0;
        var usesX = false;
        var usesY = false;
        foreach (var stop in config.stops) {
            if (stop.x != 0) usesX = true;
            if (stop.y != 0) usesY = true;
            tree.AddChild(loadClip(config.name + "_" + i++, stop.state, featureBaseObject), new Vector2(stop.x,stop.y));
        }
        var on = layer.NewState("Blend").WithAnimation(tree);

        var x = manager.NewFloat(config.name + "_x", synced: usesX);
        tree.blendParameter = x.Name();
        var y = manager.NewFloat(config.name + "_y", synced: usesY);
        tree.blendParameterY = y.Name();
        if (config.slider) {
            if (usesX) manager.NewMenuSlider(config.name, x);
        } else {
            manager.NewMenuPuppet(config.name, usesX ? x : null, usesY ? y : null);
        }
    }
}

}