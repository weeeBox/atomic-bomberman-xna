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
using BomberEngine.Core.Events;
using BomberEngine.Core.Input;

namespace Bomberman.Game.Screens
{
    public class TestScreen : Screen
    {
        private class AnimationContainer
        {
            public Animation up;
            public Animation down;
            public Animation left;
            public Animation right;
        }

        public enum AnimationId
        {
            Stand,
            Walk,
            Kick,
        }

        private IDictionary<AnimationId, AnimationContainer> animationLookup;
        private AnimationInstance groupInstance;

        public TestScreen()
        {
            animationLookup = new Dictionary<AnimationId, AnimationContainer>();

            AnimationContainer container;

            container = new AnimationContainer();
            container.up = GetAnimation(A.anim_stand_north);
            container.down = GetAnimation(A.anim_stand_south);
            container.left = GetAnimation(A.anim_stand_west);
            container.right = GetAnimation(A.anim_stand_east);
            animationLookup[AnimationId.Stand] = container;

            container = new AnimationContainer();
            container.up = GetAnimation(A.anim_walk_north);
            container.down = GetAnimation(A.anim_walk_south);
            container.left = GetAnimation(A.anim_walk_west);
            container.right = GetAnimation(A.anim_walk_east);
            animationLookup[AnimationId.Walk] = container;

            container = new AnimationContainer();
            container.up = GetAnimation(A.anim_kick_north);
            container.down = GetAnimation(A.anim_kick_south);
            container.left = GetAnimation(A.anim_kick_west);
            container.right = GetAnimation(A.anim_kick_east);
            animationLookup[AnimationId.Kick] = container;
            
            groupInstance = new AnimationInstance();
            groupInstance.Init(GetAnimation(A.anim_die_green_1));
        }

        public override void Update(float delta)
        {
            groupInstance.Update(delta);
        }

        public override void Draw(Context context)
        {
            int x = 100;
            int y = 100;

            DrawAnim(context, groupInstance, x, y);
        }

        private void DrawAnim(Context context, AnimationInstance instance, int x, int y)
        {
            int frameIndex = instance.FrameIndex;
            TextureImage texture = instance.Texture;

            Animation group = instance.Animation;

            Rectangle src;
            int ox = group.frames[frameIndex].ox;
            int oy = group.frames[frameIndex].oy;

            src.X = group.frames[frameIndex].x;
            src.Y = group.frames[frameIndex].y;
            src.Width = group.frames[frameIndex].w;
            src.Height = group.frames[frameIndex].h;

            context.DrawImagePart(texture, src, x - ox, y - oy);
        }

        private Animation GetAnimation(int id)
        {
            return BmApplication.Assets().GetAnimation(id);
        }

        public override bool HandleEvent(Event evt)
        {
            if (evt.code == Event.KEY)
            {
                KeyEvent keyEvent = (KeyEvent)evt;
                if (keyEvent.IsKeyPressed(KeyCode.Up))
                {
                    
                    return true;
                }
            }

            return base.HandleEvent(evt);
        }
    }
}
