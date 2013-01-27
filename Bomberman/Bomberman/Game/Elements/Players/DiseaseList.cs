using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;

namespace Bomberman.Game.Elements.Players
{
    public class DiseaseList
    {
        private bool[] flags;
        private int[] randomIndices;

        public DiseaseList()
        {
            flags = new bool[Diseases.Count];

            randomIndices = new int[Diseases.Count];
            for (int i = 0; i < randomIndices.Length; ++i)
            {
                randomIndices[i] = i;
            }
        }

        public void InfectRandom(int count)
        {
            if (count == 1)
            {
                int index = MathHelper.NextInt(Diseases.Count);
                TryInfect(index);
            }
            else if (count > 1)
            {
                Util.ShuffleArray(randomIndices);
                for (int i = 0; i < count; ++i)
                {
                    int index = randomIndices[i];
                    TryInfect(index);
                }
            }
        }

        public bool TryInfect(int index)
        {
            Diseases disease = Diseases.FromIndex(index);
            if (disease != null)
            {
                return TryInfect(disease);
            }

            return false;
        }

        public bool TryInfect(Diseases disease)
        {
            flags[disease.index] = true;

            if (Diseases.CRACK == disease)
            {
                Cure(Diseases.MOLASSES);
            }
            if (Diseases.MOLASSES == disease)
            {
                Cure(Diseases.CRACK);
            }
            if (Diseases.CONSTIPATION == disease)
            {
                Cure(Diseases.POOPS);
            }
            if (Diseases.POOPS == disease)
            {
                Cure(Diseases.CONSTIPATION);
            }
            if (Diseases.CRACKPOOPS == disease)
            {
                TryInfect(Diseases.CRACK);
                TryInfect(Diseases.POOPS);
            }

            return true;
        }

        public void Cure(Diseases disease)
        {
            flags[disease.index] = false;
        }

        public bool IsInfected(Diseases disease)
        {
            return flags[disease.index];
        }
    }
}
