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

            AssertResult(result, Infected(Diseases.MOLASSES));

            Disease disease = DiseaseList.DiseaseForType(Diseases.MOLASSES);
            list.Update(disease.duration);

            AssertResult(result, Infected(Diseases.MOLASSES), Cured(Diseases.MOLASSES));
        }

        [TestMethod]
        public void TestExclusiveInfect()
        {
            DiseaseMockPlayer player = new DiseaseMockPlayer();

            DiseaseListMock list = new DiseaseListMock(player);

            list.TryInfect(Diseases.MOLASSES);
            list.AssertInfected(Diseases.MOLASSES);

            list.TryInfect(Diseases.CRACK);
            list.AssertInfected(Diseases.CRACK);
            list.AssertNotInfected(Diseases.MOLASSES);

            list.TryInfect(Diseases.MOLASSES);
            list.AssertInfected(Diseases.MOLASSES);
            list.AssertNotInfected(Diseases.CRACK);

            list.TryInfect(Diseases.CONSTIPATION);
            list.AssertInfected(Diseases.CONSTIPATION);

            list.TryInfect(Diseases.POOPS);
            list.AssertInfected(Diseases.POOPS);
            list.AssertNotInfected(Diseases.CONSTIPATION);

            list.TryInfect(Diseases.CONSTIPATION);
            list.AssertInfected(Diseases.CONSTIPATION);
            list.AssertNotInfected(Diseases.POOPS);
        }

        private static String Infected(Diseases disease)
        {
            return "i:" + disease;
        }

        private static String Cured(Diseases disease)
        {
            return "c:" + disease;
        }
    }

    class DiseaseMockPlayer : Player
    {
        private List<String> m_list;

        public DiseaseMockPlayer(List<String> list = null)
            : base(0)
        {
            m_list = list;
        }

        public override void OnInfected(Diseases disease)
        {
            if (m_list != null)
            {
                m_list.Add("i:" + disease);
            }
        }

        public override void OnCured(Diseases disease)
        {
            if (m_list != null)
            {
                m_list.Add("c:" + disease);
            }
        }
    }

    class DiseaseListMock : DiseaseList
    {
        public DiseaseListMock(Player player)
            : base(player)
        {   
        }

        public void AssertInfected(Diseases disease)
        {
            Assert.IsTrue(IsInfected(disease));
        }

        public void AssertNotInfected(Diseases disease)
        {
            Assert.IsFalse(IsInfected(disease));
        }
    }
}
