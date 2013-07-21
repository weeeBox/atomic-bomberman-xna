using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;
using BomberEngine.Core;

namespace Bomberman.Game.Elements.Players
{
    public class DiseaseList : IUpdatable, IResettable
    {
        private Player m_player;

        private bool[] flags;
        private bool[] oldFlags;

        private float[] remains;
        private int m_activeCount;

        private int[] randomIndices;

        public DiseaseList(Player player)
        {
            this.m_player = player;

            remains = new float[Diseases.Count];

            flags = new bool[Diseases.Count];
            oldFlags = new bool[Diseases.Count];

            randomIndices = new int[Diseases.Count];

            for (int i = 0; i < randomIndices.Length; ++i)
            {
                randomIndices[i] = i;
            }
        }

        public void Reset()
        {
            ArrayUtils.Clear(flags);
            ArrayUtils.Clear(oldFlags);
            ArrayUtils.Clear(remains);
            m_activeCount = 0;

            for (int i = 0; i < randomIndices.Length; ++i)
            {
                randomIndices[i] = i;
            }
        }

        public void Update(float delta)
        {
            int curedCount = 0;
            for (int i = 0; i < Diseases.Count; ++i)
            {
                if (flags[i])
                {
                    remains[i] -= delta;
                    if (remains[i] <= 0)
                    {
                        Diseases disease = Diseases.FromIndex(i);
                        bool cured = TryCure(disease);
                        if (cured)
                        {
                            ++curedCount;
                        }
                    }
                }
            }

            if (curedCount > 0)
            {
                UpdateFlagsChanges();
            }
        }

        public void InfectRandom(int count)
        {
            if (count > 0)
            {
                if (count == 1)
                {
                    int index = MathHelp.NextInt(Diseases.Count);
                    Diseases disease = Diseases.FromIndex(index);
                    TryInfect(disease);
                }
                else if (count > 1)
                {
                    Util.ShuffleArray(randomIndices);
                    for (int i = 0; i < count; ++i)
                    {
                        int index = randomIndices[i];
                        Diseases disease = Diseases.FromIndex(index);
                        TryInfect(disease);
                    }
                }

                UpdateFlagsChanges();
            }
        }

        private bool TryInfect(Diseases disease)
        {
            int index = disease.index;

            bool wasInfected = flags[index];

            flags[index] = true;
            remains[index] = disease.duration;

            if (Diseases.CRACK == disease)
            {
                TryCure(Diseases.MOLASSES);
            }
            if (Diseases.MOLASSES == disease)
            {
                TryCure(Diseases.CRACK);
            }
            if (Diseases.CONSTIPATION == disease)
            {
                TryCure(Diseases.POOPS);
            }
            if (Diseases.POOPS == disease)
            {
                TryCure(Diseases.CONSTIPATION);
            }
            if (Diseases.CRACKPOOPS == disease)
            {
                TryInfect(Diseases.CRACK);
                TryInfect(Diseases.POOPS);
            }

            if (!wasInfected && flags[index])
            {
                ++m_activeCount;
                return true;
            }

            return false;
        }

        public bool TryInfect(int diseaseIndex)
        {
            Diseases disease = Diseases.FromIndex(diseaseIndex);
            bool infected = TryInfect(disease);
            UpdateFlagsChanges();

            return infected;
        }

        public bool TryCure(Diseases disease)
        {   
            return TryCure(disease.index);
        }

        public bool TryCure(int index)
        {
            bool wasInfected = flags[index];
            flags[index] = false;
            if (wasInfected)
            {
                --m_activeCount;
                return true;
            }

            return false;
        }

        public bool IsInfected(Diseases disease)
        {
            return IsInfected(disease.index);
        }

        public bool IsInfected(int index)
        {
            return flags[index];
        }

        public float GetInfectedRemains(int index)
        {
            return IsInfected(index) ? remains[index] : 0.0f;
        }

        public void SetInfectedRemains(int index, float time)
        {
            remains[index] = time < 0.0f ? 0.0f : time;
        }

        private void UpdateFlagsChanges()
        {
            for (int i = 0; i < flags.Length; ++i)
            {
                bool flag = flags[i];
                bool oldFlag = oldFlags[i];

                if (flag && !oldFlag)
                {
                    m_player.OnInfected(Diseases.FromIndex(i));
                }
                else if (!flag && oldFlag)
                {
                    m_player.OnCured(Diseases.FromIndex(i));
                }

                oldFlags[i] = flag;
            }
        }

        public int activeCount
        {
            get { return m_activeCount; }
        }
    }
}
