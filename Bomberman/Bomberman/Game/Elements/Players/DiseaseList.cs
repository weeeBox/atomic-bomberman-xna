using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;
using BomberEngine.Core;
using BomberEngine.Debugging;

namespace Bomberman.Game.Elements.Players
{
    public class DiseaseList : IUpdatable, IResettable
    {
        public static readonly Disease[] diseaseArray = 
        {
            new Disease("Molasses",     Diseases.MOLASSES,      15.0f),
            new Disease("Crack",        Diseases.CRACK,         15.0f),
            new Disease("Constipation", Diseases.CONSTIPATION,  15.0f),
            new Disease("Poops",        Diseases.POOPS,         15.0f),
            new Disease("Short Flame",  Diseases.SHORTFLAME,    15.0f),
            new Disease("Crack Poops",  Diseases.CRACKPOOPS,    15.0f),
            new Disease("Short Fuze",   Diseases.SHORTFUZE,     15.0f),
            new Disease("Swap",         Diseases.SWAP,          0.0f),
            new Disease("Reversed",     Diseases.REVERSED,      15.0f),
            new Disease("Leprosy",      Diseases.LEPROSY,       15.0f),
            new Disease("Invisible",    Diseases.INVISIBLE,     15.0f),
            new Disease("Duds",         Diseases.DUDS,          15.0f),
            new Disease("Hyperswap",    Diseases.HYPERSWAP,     0.0f)
        };

        private Player m_player;

        private bool[] flags;
        private bool[] oldFlags;

        private float[] remains;
        private int m_activeCount;

        private int[] randomIndices;

        public DiseaseList(Player player)
        {
            this.m_player = player;

            remains = new float[diseaseArray.Length];

            flags = new bool[diseaseArray.Length];
            oldFlags = new bool[diseaseArray.Length];

            randomIndices = new int[diseaseArray.Length];

            for (int i = 0; i < randomIndices.Length; ++i)
            {
                randomIndices[i] = i;
            }
        }

        public void Reset()
        {
            CureAll();

            for (int i = 0; i < randomIndices.Length; ++i)
            {
                randomIndices[i] = i;
            }
        }

        public void Update(float delta)
        {
            int curedCount = 0;
            for (int i = 0; i < diseaseArray.Length; ++i)
            {
                if (flags[i])
                {
                    remains[i] -= delta;
                    if (remains[i] <= 0)
                    {   
                        bool cured = TryCure(i);
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
                    int index = MathHelp.NextInt(diseaseArray.Length);
                    TryInfect(index);
                }
                else if (count > 1)
                {
                    ArrayUtils.Shuffle(randomIndices);
                    for (int i = 0; i < count; ++i)
                    {
                        int diseaseIndex = randomIndices[i];
                        TryInfect(diseaseIndex);
                    }
                }

                UpdateFlagsChanges();
            }
        }

        public bool TryInfect(Diseases disease)
        {
            int index = ToIndex(disease);
            return TryInfect(index);
        }

        public bool TryInfect(int diseaseIndex)
        {   
            bool infected = TryInfectHelper(diseaseIndex);
            UpdateFlagsChanges();

            return infected;
        }

        private bool TryInfectHelper(int index)
        {
            Disease disease = DiseaseForIndex(index);
            Debug.AssertNotNull(disease);

            bool wasInfected = flags[index];

            flags[index] = true;
            remains[index] = disease.duration;

            switch (disease.type)
            {
                case Diseases.CRACK:
                    TryCure(Diseases.MOLASSES);
                    break;

                case Diseases.MOLASSES:
                    TryCure(Diseases.CRACK);
                    break;

                case Diseases.CONSTIPATION:
                    TryCure(Diseases.POOPS);
                    break;

                case Diseases.POOPS:
                    TryCure(Diseases.CONSTIPATION);
                    break;

                case Diseases.CRACKPOOPS:
                    TryInfect(Diseases.CRACK);
                    TryInfect(Diseases.POOPS);
                    break;
            }

            if (!wasInfected && flags[index])
            {
                ++m_activeCount;
                return true;
            }

            return false;
        }

        public void CureAll()
        {
            ArrayUtils.Clear(flags);
            ArrayUtils.Clear(oldFlags);
            ArrayUtils.Clear(remains);
            m_activeCount = 0;
        }

        public bool TryCure(Diseases disease)
        {   
            return TryCure((int)disease);
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
            int index = ToIndex(disease);
            return IsInfected(index);
        }

        public bool IsInfected(int index)
        {
            Debug.AssertRange(index, flags);
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
                    Diseases disease = ToDisease(i);
                    m_player.OnInfected(disease);
                }
                else if (!flag && oldFlag)
                {
                    Diseases disease = ToDisease(i);
                    m_player.OnCured(disease);
                }

                oldFlags[i] = flag;
            }
        }

        public int activeCount
        {
            get { return m_activeCount; }
        }

        public static Diseases ToDisease(int index)
        {
            if (index >= 0 && index < (int)Diseases.Count)
            {
                return (Diseases)index;
            }

            return Diseases.Count;
        }

        public static int ToIndex(Diseases disease)
        {
            return (int)disease;
        }

        public static Disease DiseaseForType(Diseases type)
        {
            int index = ToIndex(type);
            return DiseaseForIndex(index);
        }

        public static Disease DiseaseForIndex(int index)
        {
            if (index >= 0 && index < diseaseArray.Length)
            {
                return diseaseArray[index];
            }

            return null;
        }
    }
}
