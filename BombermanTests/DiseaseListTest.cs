using System;
using System.Collections.Generic;
using Bomberman;
using Bomberman.Gameplay.Elements;
using Bomberman.Gameplay.Elements.Cells;
using Bomberman.Gameplay.Elements.Fields;
using Bomberman.Gameplay.Elements.Players;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BombermanTests.Mocks;

namespace BombermanTests.TestDiseases
{
    [TestClass]
    public class DiseaseListTest : TestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            GameMock mock = new GameMock(15, 11);
            new FieldMock(mock);
        }

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
            PlayerMock player = new PlayerMock(result);

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
            PlayerMock player = new PlayerMock();

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

        [TestMethod]
        public void TestMolasses()
        {
            List<String> result = new List<String>();
            PlayerMock player = new PlayerMock(result);

            DiseaseList list = new DiseaseListMock(player);

            Diseases disease = Diseases.MOLASSES;

            list.TryInfect(disease);
            Assert.AreEqual(CVars.cg_playerSpeedMolasses.floatValue, player.GetSpeed());

            list.TryCure(disease);
            Assert.AreEqual(CVars.cg_playerSpeed.floatValue, player.GetSpeed());
        }

        [TestMethod]
        public void TestCrack()
        {
            List<String> result = new List<String>();
            PlayerMock player = new PlayerMock(result);

            DiseaseList list = new DiseaseListMock(player);

            Diseases disease = Diseases.CRACK;

            list.TryInfect(disease);
            Assert.AreEqual(CVars.cg_playerSpeedCrack.floatValue, player.GetSpeed());

            list.TryCure(disease);
            Assert.AreEqual(CVars.cg_playerSpeed.floatValue, player.GetSpeed());
        }

        [TestMethod]
        public void TestConstipation()
        {
            List<String> result = new List<String>();
            PlayerMock player = new PlayerMock(result);

            DiseaseList list = new DiseaseListMock(player);

            Diseases disease = Diseases.CONSTIPATION;

            list.TryInfect(disease);
            Assert.IsFalse(player.TryAction());

            list.TryCure(disease);
            Assert.IsTrue(player.TryAction());
        }

        /*
        [TestMethod]
        public void TestPoops()
        {

        }
        */

        [TestMethod]
        public void TestShortFlame()
        {
            List<String> result = new List<String>();
            PlayerMock player = new PlayerMock(result);

            DiseaseList list = new DiseaseListMock(player);

            Diseases disease = Diseases.SHORTFLAME;

            list.TryInfect(disease);
            Bomb bomb = player.GetNextBomb();
            Assert.AreEqual(CVars.cg_bombShortFlame.intValue, bomb.GetRadius());

            bomb.Deactivate(); // hack

            list.TryCure(disease);
            bomb = player.GetNextBomb();
            Assert.AreEqual(CVars.cg_initFlame.intValue, bomb.GetRadius());
        }

        public void TestCrackPoops()
        {
            PlayerMock player = new PlayerMock();
            DiseaseListMock list = new DiseaseListMock(player);

            list.TryInfect(Diseases.CRACKPOOPS);
            list.AssertInfected(Diseases.CRACK);
            list.AssertInfected(Diseases.POOPS);
        }

        [TestMethod]
        public void TestShortFuze()
        {
            List<String> result = new List<String>();
            PlayerMock player = new PlayerMock(result);

            DiseaseList list = new DiseaseListMock(player);

            Diseases disease = Diseases.SHORTFUZE;

            list.TryInfect(disease);
            Bomb bomb = player.GetNextBomb();
            Assert.AreEqual(CVars.cg_fuzeTimeShort.floatValue, 1000 * bomb.timeRemains);

            bomb.Deactivate(); // hack

            list.TryCure(disease);
            bomb = player.GetNextBomb();
            Assert.AreEqual(CVars.cg_fuzeTimeNormal.floatValue, 1000 * bomb.timeRemains);
        }

        /*
        [TestMethod]
        public void TestSwap()
        {

        }
        */

        /*
        [TestMethod]
        public void TestReversed()
        {

        }
        */


        private static String Infected(Diseases disease)
        {
            return "i:" + disease;
        }

        private static String Cured(Diseases disease)
        {
            return "c:" + disease;
        }
    }

    class PlayerMock : Player
    {
        private List<String> m_list;

        public PlayerMock(List<String> list = null)
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

            base.OnInfected(disease);
        }

        public override void OnCured(Diseases disease)
        {
            if (m_list != null)
            {
                m_list.Add("c:" + disease);
            }

            base.OnCured(disease);
        }

        public void SetDiseaseList(DiseaseList list)
        {
            diseases = list;
        }
    }

    class DiseaseListMock : DiseaseList
    {
        public DiseaseListMock(PlayerMock player)
            : base(player)
        {
            player.SetDiseaseList(this);
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
