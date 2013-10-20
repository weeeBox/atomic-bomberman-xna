using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements;

namespace BombermanTests
{
    [TestClass]
    public class DiseaseListTest : TestBase
    {
        [TestMethod]
        public void TestIndices()
        {
            int count = (int)Diseases.Count;

            Disease[] diseases = DiseaseList.diseaseArray;
            Assert.AreEqual(count, diseases.Length);
            
            for (int i = 0; i < count; ++i)
            {
                Assert.AreEqual(diseases[i].type, (Diseases)i);
            }
        }

        [TestMethod]
        public void TestInfect()
        {
            List<String> result = new List<String>();
            DiseaseMockPlayer player = new DiseaseMockPlayer(result);

            DiseaseList list = new DiseaseList(player);

            bool infected = list.TryInfect(Diseases.MOLASSES);
            Assert.IsTrue(infected);

            infected = list.TryInfect(Diseases.MOLASSES);
            Assert.IsFalse(infected);

            AssertResult(result, "i:" + Diseases.MOLASSES);

            Disease disease = DiseaseList.DiseaseForType(Diseases.MOLASSES);
            list.Update(disease.duration);

            AssertResult(result, "i:" + Diseases.MOLASSES, "c:" + Diseases.MOLASSES);
        }
    }

    class DiseaseMockPlayer : Player
    {
        private List<String> m_list;

        public DiseaseMockPlayer(List<String> list)
            : base(0)
        {
            m_list = list;
        }

        public override void OnInfected(Diseases disease)
        {
            m_list.Add("i:" + disease);
        }

        public override void OnCured(Diseases disease)
        {
            m_list.Add("c:" + disease);
        }
    }
}
