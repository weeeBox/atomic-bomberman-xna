using System.Collections.Generic;
using Assets;
using BomberEngine;
using Bomberman.Content;

namespace Bomberman.Gameplay.Elements.Players
{
    public class PlayerAnimations
    {
        public enum Id
        {
            Undefined,
            Stand,
            Walk,
            KickBomb,
            PunchBomb,
            PickupBomb,
            StandBomb,
            WalkBomb,
            Cornerhead,
            Die,
        }

        private IDictionary<Id, DirectionalAnimationGroup> m_Directionals;
        private IDictionary<Id, Animation[]> m_Singles;

        public PlayerAnimations()
        {
            InitAnimations();
        }

        public Animation Find(Id type, Direction dir)
        {
            DirectionalAnimationGroup directional;
            if (m_Directionals.TryGetValue(type, out directional))
            {
                return directional.Get(dir);
            }

            Animation[] array;
            if (m_Singles.TryGetValue(type, out array))
            {
                int index = MathHelp.NextInt(array.Length);
                return array[index];
            }

            return null;
        }

        private void InitAnimations()
        {
            // directional animations
            m_Directionals = new Dictionary<Id, DirectionalAnimationGroup>();

            DirectionalAnimationGroup group = new DirectionalAnimationGroup();
            group.Set(Direction.UP, GetAnimation(A.anim_stand_north));
            group.Set(Direction.DOWN, GetAnimation(A.anim_stand_south));
            group.Set(Direction.LEFT, GetAnimation(A.anim_stand_west));
            group.Set(Direction.RIGHT, GetAnimation(A.anim_stand_east));
            m_Directionals[Id.Stand] = group;

            group = new DirectionalAnimationGroup();
            group.Set(Direction.UP, GetAnimation(A.anim_walk_north));
            group.Set(Direction.DOWN, GetAnimation(A.anim_walk_south));
            group.Set(Direction.LEFT, GetAnimation(A.anim_walk_west));
            group.Set(Direction.RIGHT, GetAnimation(A.anim_walk_east));
            m_Directionals[Id.Walk] = group;

            group = new DirectionalAnimationGroup();
            group.Set(Direction.UP, GetAnimation(A.anim_kick_north));
            group.Set(Direction.DOWN, GetAnimation(A.anim_kick_south));
            group.Set(Direction.LEFT, GetAnimation(A.anim_kick_west));
            group.Set(Direction.RIGHT, GetAnimation(A.anim_kick_east));
            m_Directionals[Id.KickBomb] = group;

            group = new DirectionalAnimationGroup();
            group.Set(Direction.UP, GetAnimation(A.anim_punch_north));
            group.Set(Direction.DOWN, GetAnimation(A.anim_punch_south));
            group.Set(Direction.LEFT, GetAnimation(A.anim_punch_west));
            group.Set(Direction.RIGHT, GetAnimation(A.anim_punch_east));
            m_Directionals[Id.PunchBomb] = group;

            group = new DirectionalAnimationGroup();
            group.Set(Direction.UP, GetAnimation(A.anim_pickup_north));
            group.Set(Direction.DOWN, GetAnimation(A.anim_pickup_south));
            group.Set(Direction.LEFT, GetAnimation(A.anim_pickup_west));
            group.Set(Direction.RIGHT, GetAnimation(A.anim_pickup_east));
            m_Directionals[Id.PickupBomb] = group;

            group = new DirectionalAnimationGroup();
            group.Set(Direction.UP, GetAnimation(A.anim_standbomb_north));
            group.Set(Direction.DOWN, GetAnimation(A.anim_standbomb_south));
            group.Set(Direction.LEFT, GetAnimation(A.anim_standbomb_west));
            group.Set(Direction.RIGHT, GetAnimation(A.anim_standbomb_east));
            m_Directionals[Id.StandBomb] = group;

            group = new DirectionalAnimationGroup();
            group.Set(Direction.UP, GetAnimation(A.anim_walkbomb_north));
            group.Set(Direction.DOWN, GetAnimation(A.anim_walkbomb_south));
            group.Set(Direction.LEFT, GetAnimation(A.anim_walkbomb_west));
            group.Set(Direction.RIGHT, GetAnimation(A.anim_walkbomb_east));
            m_Directionals[Id.WalkBomb] = group;

            m_Singles = new Dictionary<Id, Animation[]>();

            // cornerhead
            Animation[] array = new Animation[]
            {
                GetAnimation(A.anim_cornerhead_0),
                GetAnimation(A.anim_cornerhead_1),
                GetAnimation(A.anim_cornerhead_2),
                GetAnimation(A.anim_cornerhead_3),
                GetAnimation(A.anim_cornerhead_4),
                GetAnimation(A.anim_cornerhead_5),
                GetAnimation(A.anim_cornerhead_6),
                GetAnimation(A.anim_cornerhead_7),
                GetAnimation(A.anim_cornerhead_8),
                GetAnimation(A.anim_cornerhead_9),
                GetAnimation(A.anim_cornerhead_10),
                GetAnimation(A.anim_cornerhead_11),
                GetAnimation(A.anim_cornerhead_12),
            };
            m_Singles[Id.Cornerhead] = array;

            // die
            array = new Animation[]
            {
                GetAnimation(A.anim_die_green_1),
                GetAnimation(A.anim_die_green_2),
                GetAnimation(A.anim_die_green_3),
                GetAnimation(A.anim_die_green_4),
                GetAnimation(A.anim_die_green_5),
                GetAnimation(A.anim_die_green_6),
                GetAnimation(A.anim_die_green_7),
                GetAnimation(A.anim_die_green_8),
                GetAnimation(A.anim_die_green_9),
                GetAnimation(A.anim_die_green_10),
                GetAnimation(A.anim_die_green_11),
                GetAnimation(A.anim_die_green_12),
                GetAnimation(A.anim_die_green_13),
                GetAnimation(A.anim_die_green_14),
                GetAnimation(A.anim_die_green_15),
                GetAnimation(A.anim_die_green_16),
                GetAnimation(A.anim_die_green_17),
                GetAnimation(A.anim_die_green_18),
                GetAnimation(A.anim_die_green_19),
                GetAnimation(A.anim_die_green_20),
                GetAnimation(A.anim_die_green_21),
                GetAnimation(A.anim_die_green_22),
                GetAnimation(A.anim_die_green_23),
                GetAnimation(A.anim_die_green_24),
            };
            m_Singles[Id.Die] = array;
        }

        private Animation GetAnimation(int id)
        {
            Animation anim = BmApplication.Assets().GetAnimation(id);
            Assert.IsTrue(anim != null);
            return anim;
        }
    }

    class DirectionalAnimationGroup
    {
        private Animation[] m_Animations;

        public DirectionalAnimationGroup()
        {
            m_Animations = new Animation[(int)Direction.Count];
        }

        public void Set(Direction dir, Animation animation)
        {
            m_Animations[(int)dir] = animation;
        }

        public Animation Get(Direction dir)
        {
            return m_Animations[(int)dir];
        }
    }
}
