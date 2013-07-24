using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual;
using Bomberman.Content;
using Assets;
using BomberEngine.Core.Assets.Types;
using Microsoft.Xna.Framework;

namespace Bomberman.Game.Screens
{
    public class TestScreen : Screen
    {
        private enum Animations
        {
            StillUp,
            StillDown,
            StillLeft,
            StillRight,

            WalkUp,
            WalkDown,
            WalkLeft,
            WalkRight,
        }

        private IDictionary<Animations, AnimationGroup> groupLookup;
        private AnimationInstance groupInstance;

        public TestScreen()
        {
            groupLookup = new Dictionary<Animations, AnimationGroup>();

            Animation animation = GetAnimation(A.anim_stand);
            groupLookup[Animations.StillUp] = animation.Group("stand north");
            groupLookup[Animations.StillDown] = animation.Group("stand south");
            groupLookup[Animations.StillLeft] = animation.Group("stand west");
            groupLookup[Animations.StillRight] = animation.Group("stand east");

            animation = GetAnimation(A.anim_walk);
            groupLookup[Animations.WalkUp] = animation.Group("walk north");
            groupLookup[Animations.WalkDown] = animation.Group("walk south");
            groupLookup[Animations.WalkLeft] = animation.Group("walk west");
            groupLookup[Animations.WalkRight] = animation.Group("walk east");

            groupInstance = new AnimationInstance();
            groupInstance.Init(groupLookup[Animations.WalkDown], animation.texture);
        }

        public override void Update(float delta)
        {
            groupInstance.Update(delta);
        }

        public override void Draw(Context context)
        {
            int frameIndex = groupInstance.FrameIndex;
            TextureImage texture = groupInstance.Texture;
            AnimationGroup group = groupInstance.Group;

            Rectangle src;
            int ox = group.frames[frameIndex].ox;
            int oy = group.frames[frameIndex].oy;

            src.X = group.frames[frameIndex].x;
            src.Y = group.frames[frameIndex].y;
            src.Width = group.frames[frameIndex].w;
            src.Height = group.frames[frameIndex].h;

            int x = 100 - ox;
            int y = 100 - oy;

            context.DrawImagePart(texture, src, x, y);
        }

        private Animation GetAnimation(int id)
        {
            return BmApplication.Assets().GetAnimation(id);
        }
    }
}
