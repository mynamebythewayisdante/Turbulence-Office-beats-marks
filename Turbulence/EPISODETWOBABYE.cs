using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections;
using Workshop;
using HarmonyLib;
using LOR_DiceSystem;
using LOR_XML;
using Mod;
using UI;
using Battle.DiceAttackEffect;
using Battle.CreatureEffect;
using UnityEngine;
using UnityEngine.UI;
using NAudio.Wave;
using Sound;
using Spine.Unity;
using CustomMapUtility;

namespace CompleteDisaster
{
    public class EnemyTeamStageManager_TURBULENCEHELL2THESEQUEL : EnemyTeamStageManager
    {
        public override void OnStageClear()
        {
            base.OnStageClear();
            list.Clear();
            UIAlarmPopup.instance.SetAlarmText("thank you for playing my mod");
        }

        public bool isSus;
        public override bool IsStageFinishable()
        {
            return isSus;
        }
        public List<int> list;
        public enum Phase
        {
            act1act1, act1act2, act1act3, act2, act3
        }
        public Phase currentphase
        {
            get; private set;
        }
        public override void OnWaveStart()
        {
            base.OnWaveStart();
            List<int> list2 = new List<int>();
            Phase current = Phase.act1act1;
            if (Singleton<StageController>.Instance.GetStageModel().GetStageStorageData("summonedguys", out list2))
            {
                list = list2;
            }
            if (Singleton<StageController>.Instance.GetStageModel().GetStageStorageData("phase", out current))
            {
                currentphase = current;
            }
            if (list.Count < 1)
            {
                list.Add(1);
                list.Add(2);
                list.Add(3);
                list.Add(4);
                list.Add(5);
                list.Add(6);
                list.Add(7);
                list.Add(8);
                list.Add(9);
                list.Add(10);
                list.Add(11);
                list.Add(12);
                list.Add(13);
                list.Add(14);
                list.Add(15);
                list.Add(16);
                list.Add(17);
                list.Add(18);
                list.Add(19);
                list.Add(20);
                list.Add(21);
                list.Add(22);
                list.Add(23);
                list.Add(24);
                list.Add(666);
            }
            if (BattleObjectManager.instance.GetAliveList(Faction.Enemy).Count > 1 && current == Phase.act3)
            {
                //super ego moyai trigger untargetable/immobile
            }
            else if (current == Phase.act2)
            {
                //say useless
            }
        }
        public override void OnEndBattle()
        {
            base.OnEndBattle();
            Singleton<StageController>.Instance.GetStageModel().SetStageStorgeData("summonedguys", list);
            Singleton<StageController>.Instance.GetStageModel().SetStageStorgeData("currentphase", currentphase);
        }
        public override void OnRoundEndTheLast()
        {
            base.OnRoundEndTheLast();
            if (callforaid)
            {
                callforaid = false;
                if (list.Count >= 2)
                {
                    summondudes();
                }
            }
            if (BattleObjectManager.instance.GetAliveList(Faction.Enemy).Count <= 0)
            {
                switch (currentphase)
                {
                    case Phase.act1act1:
                        currentphase = Phase.act1act2;
                        summonguys();
                        break;
                    case Phase.act1act2:
                        currentphase = Phase.act1act3;
                        summonguys();
                        break;
                    case Phase.act1act3:
                        currentphase = Phase.act2;
                        minosprimeentrance();
                        break;
                    case Phase.act2:
                        currentphase = Phase.act3;
                        superegomoyairises();
                        break;
                }

            }
        }
        
        public bool callforaid;
        public void summonguys()
        {

        }
        public void summondudes()
        {
            list.Remove(666);
            int among = RandomUtil.SelectOne(list);
            list.Remove(among);
            int sussy = RandomUtil.SelectOne(list);
            list.Remove(sussy);
            int ginormous = RandomUtil.SelectOne(list);
            list.Remove(ginormous);
            int gamba = RandomUtil.SelectOne(list);
            list.Remove(gamba);
            list.Add(666);
            Singleton<StageController>.Instance.AddNewUnit(Faction.Enemy, new LorId("TurbulenceOffice", among), 1, 180);
            Singleton<StageController>.Instance.AddNewUnit(Faction.Enemy, new LorId("TurbulenceOffice", sussy), 2, 180);
            Singleton<StageController>.Instance.AddNewUnit(Faction.Enemy, new LorId("TurbulenceOffice", ginormous), 3, 180);
            Singleton<StageController>.Instance.AddNewUnit(Faction.Enemy, new LorId("TurbulenceOffice", gamba), 4, 180);
            int num = 0;
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetList())
            {
                SingletonBehavior<UICharacterRenderer>.Instance.SetCharacter(battleUnitModel.UnitData.unitData, num++, true, false);
            }
            BattleObjectManager.instance.InitUI();
        }
        public void minosprimeentrance()
        {

        }
        public void superegomoyairises()
        {

        }

    }
    public class PassiveAbility_tomagerification : PassiveAbilityBase
    {
        public override void OnMakeBreakState(BattleUnitModel target)
        {
            base.OnMakeBreakState(target);
            if (target != null)
            {
                int susp = target.allyCardDetail.GetHand().Count;
                target.allyCardDetail.ExhaustAllCards();
                foreach (BattleDiceCardModel among in this.owner.allyCardDetail.GetAllDeck())
                {
                    if (among != null)
                    {
                        if (target.allyCardDetail.GetHand().Count < susp)
                        {
                            target.allyCardDetail.AddNewCard(among.GetID());
                        }
                        else
                        {
                            target.allyCardDetail.AddNewCardToDeck(among.GetID());
                        }

                    }
                }
                target.view.ChangeSkin("Mook 2");
            }
        }
    }
    public class PassiveAbility_tomager_seizethemeansofproduction : PassiveAbilityBase
    {
        public override void OnSucceedAttack(BattleDiceBehavior behavior)
        {
            base.OnSucceedAttack(behavior);
            if (behavior.card.target != null)
            {
                int gamer = 0;
                foreach (BattleUnitModel am in BattleObjectManager.instance.GetAliveList_opponent(this.owner.faction))
                {
                    if (am != null)
                    {
                        if (gamer < am.emotionDetail.GetSelectedCardList().Count)
                        {
                            gamer = am.emotionDetail.GetSelectedCardList().Count;
                        }
                    }
                }
                if (behavior.card.target.emotionDetail.GetSelectedCardList().Count == gamer)
                {
                    int gamee = 99999;
                    List<EmotionCardXmlInfo> emotion = new List<EmotionCardXmlInfo>();
                    foreach (BattleEmotionCardModel em in behavior.card.target.emotionDetail.GetSelectedCardList())
                    {
                        emotion.Add(em.XmlInfo);
                    }
                    for (int i = 0; i < gamer; i++)
                    {
                        foreach (BattleUnitModel among in BattleObjectManager.instance.GetAliveList_opponent(this.owner.faction))
                        {
                            if (among != null && among.emotionDetail.GetSelectedCardList().Count < gamee && among != behavior.card.target)
                            {
                                gamee = among.emotionDetail.GetSelectedCardList().Count;
                            }
                        }
                        foreach (BattleUnitModel sussy in BattleObjectManager.instance.GetAliveList_opponent(this.owner.faction))
                        {
                            if (sussy != null && sussy.emotionDetail.GetSelectedCardList().Count == gamee && sussy != behavior.card.target)
                            {
                                if (emotion.Count > 0)
                                {
                                    EmotionCardXmlInfo sus = RandomUtil.SelectOne(emotion);
                                    sussy.emotionDetail.ApplyEmotionCard(sus);
                                    emotion.Remove(sus);
                                }
                            }
                        }
                    }
                    behavior.card.target.emotionDetail.RemoveAllEmotionCard();
                }
            }
        }
    }
    public class PassiveAbility_artairpassiveone : PassiveAbilityBase
    {
        private float impostro = 1f;
        public override bool BeforeTakeDamage(BattleUnitModel attacker, int dmg)
        {
            bool result = false;
            if ((float)((int)this.owner.hp) - (float)dmg < 1f && this.owner.hp != 1)
            {
                this.owner.SetHp(1);
                result = true;
            }
            else if (this.owner.hp == 1)
            {
                if (RandomUtil.valueForProb < impostro)
                {
                    BattleCardTotalResult battleCardResultLog = this.owner.battleCardResultLog;
                    if (battleCardResultLog != null)
                    {
                        battleCardResultLog.SetPassiveAbility(this);
                    }
                    result = true;
                    impostro -= 0.15f;
                }
            }
            return result;
        }
    }
    public class PassiveAbility_amongussuspassivetwoartairversion : PassiveAbilityBase
    {
        public override void BeforeGiveDamage(BattleDiceBehavior behavior)
        {
            base.BeforeGiveDamage(behavior);
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                dmgRate = 100 - (int)(this.owner.hp / (float)this.owner.MaxHp * 100f)
            });
        }
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            base.BeforeRollDice(behavior);
            if (RandomUtil.valueForProb < 0.3f)
            {
                BattleCardTotalResult battleCardResultLog = this.owner.battleCardResultLog;
                if (battleCardResultLog != null)
                {
                    battleCardResultLog.SetPassiveAbility(this);
                }
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    max = -9999
                });
            }
        }
    }
    public class PassiveAbility_howmanyyearsdoesittakeforamo : PassiveAbilityBase
    {
        public override void OnWaveStart()
        {
            base.OnWaveStart();
            slashhp = this.owner.GetResistHP(BehaviourDetail.Slash);
            slashst = this.owner.GetResistBP(BehaviourDetail.Slash);
            piercehp = this.owner.GetResistHP(BehaviourDetail.Penetrate);
            piercest = this.owner.GetResistBP(BehaviourDetail.Penetrate);
            blunthp = this.owner.GetResistHP(BehaviourDetail.Hit);
            bluntst = this.owner.GetResistBP(BehaviourDetail.Hit);
        }
        AtkResist slashhp;
        AtkResist piercehp;
        AtkResist blunthp;
        AtkResist slashst;
        AtkResist piercest;
        AtkResist bluntst;

        private bool isSus;
        public override bool isStraighten => true;
        public override void OnRoundEndTheLast()
        {
            base.OnRoundEndTheLast();
            Singleton<ModContentManager>.Instance.AddErrorLog("thrid passive :preload effect");
            if (isSus)
            {
                Singleton<ModContentManager>.Instance.AddErrorLog("third passive: loaded effect");
                isSus = false;
                this.owner.breakDetail.RecoverBreak(9001);
                this.owner.Book.SetResistHP(BehaviourDetail.Slash, slashhp);
                this.owner.Book.SetResistBP(BehaviourDetail.Slash, slashst);
                this.owner.Book.SetResistHP(BehaviourDetail.Penetrate, piercehp);
                this.owner.Book.SetResistBP(BehaviourDetail.Penetrate, piercest);
                this.owner.Book.SetResistHP(BehaviourDetail.Hit, blunthp);
                this.owner.Book.SetResistBP(BehaviourDetail.Hit, bluntst);
            }
        }
        public override void OnTakeDamageByAttack(BattleDiceBehavior atkDice, int dmg)
        {
            base.OnTakeDamageByAttack(atkDice, dmg);
            if (this.owner.breakDetail.breakGauge <= 1)
            {
                isSus = true;
                this.owner.Book.SetResistHP(BehaviourDetail.Slash, AtkResist.Vulnerable);
                this.owner.Book.SetResistBP(BehaviourDetail.Slash, AtkResist.Vulnerable);
                this.owner.Book.SetResistHP(BehaviourDetail.Penetrate, AtkResist.Vulnerable);
                this.owner.Book.SetResistBP(BehaviourDetail.Penetrate, AtkResist.Vulnerable);
                this.owner.Book.SetResistHP(BehaviourDetail.Hit, AtkResist.Vulnerable);
                this.owner.Book.SetResistBP(BehaviourDetail.Hit, AtkResist.Vulnerable);
            }
        }
        public override void AfterTakeDamage(BattleUnitModel attacker, int dmg)
        {
            base.AfterTakeDamage(attacker, dmg);
            if (this.owner.breakDetail.breakGauge <= 1)
            {
                isSus = true;
                this.owner.Book.SetResistHP(BehaviourDetail.Slash, AtkResist.Vulnerable);
                this.owner.Book.SetResistBP(BehaviourDetail.Slash, AtkResist.Vulnerable);
                this.owner.Book.SetResistHP(BehaviourDetail.Penetrate, AtkResist.Vulnerable);
                this.owner.Book.SetResistBP(BehaviourDetail.Penetrate, AtkResist.Vulnerable);
                this.owner.Book.SetResistHP(BehaviourDetail.Hit, AtkResist.Vulnerable);
                this.owner.Book.SetResistBP(BehaviourDetail.Hit, AtkResist.Vulnerable);
            }
        }
    }
    public class DiceCardSelfAbility_artairgoesfirst : DiceCardSelfAbilityBase
    {
        int speed;

        public static string Desc = "this page is melee (i have to specify this because it appears like a ranged move in battle) this page always goes first regardless of speed (this will definitely look super jank ingame)";
        public override void OnEnterCardPhase(BattleUnitModel unit, BattleDiceCardModel self)
        {
            base.OnEnterCardPhase(unit, self);
            self.GetSpec().Ranged = CardRange.FarAreaEach;

        }
        //public override void OnApplyCard()
        //{
        //    base.OnApplyCard();
        //    if (speed < 999)
        //    {
        //        speed = base.owner.GetSpeedDiceResult(base.owner.cardOrder).value;
        //    }
        //    base.owner.SetSpeedDiceValueAdder(base.owner.cardOrder, 999);
        //}

        //public override void OnReleaseCard()
        //{
        //   base.OnReleaseCard();
        //   base.owner.SetSpeedDiceValueAdder(base.owner.cardOrder, -(999-speed));
        //}
        public override void OnUseCard()
        {
            base.OnUseCard();
            this.card.card.GetSpec().Ranged = CardRange.Near;
        }
    }

    public class DiceCardSelfAbility_bloodinthemudmod : DiceCardSelfAbilityBase
    {
        public override void OnStartBattle()
        {
            base.OnStartBattle();
            BattleUnitBuf s = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf sauce) => sauce is slasherslusherslesher);
            if (s != null)
            {
                s.stack++;
            }
            if (s == null)
            {
                this.owner.bufListDetail.AddBuf(new slasherslusherslesher(1));
            }
        }
        public class slasherslusherslesher : BattleUnitBuf
        {
            public slasherslusherslesher(int sussy)
            {
                this.stack = sussy;
            }
            public override void OnRoundEnd()
            {
                base.OnRoundEnd();
                this.Destroy();
            }
            public override void OnSuccessAttack(BattleDiceBehavior behavior)
            {
                base.OnSuccessAttack(behavior);
                if (behavior.Detail == BehaviourDetail.Slash && behavior.card.target != null)
                {
                    behavior.card.target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Bleeding, this.stack * 2, this._owner);
                    behavior.card.target.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Bleeding, this.stack * 2, this._owner);
                }

            }
        }
    }
    public class DiceCardAbility_damageing7timeletsgo : DiceCardAbilityBase
    {
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            base.OnSucceedAttack(target);
            target.TakeDamage(7);
        }
    }
    public class DiceCardAbility_yoihavetomake10aswell : DiceCardAbilityBase
    {
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            base.OnSucceedAttack(target);
            target.TakeDamage(10);
        }
    }
    public class DiceCardAbility_inflict2bindandfeebleand : DiceCardAbilityBase
    {
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            base.OnSucceedAttack(target);
            target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Weak, 2, this.owner);
            target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Binding, 2, this.owner);
        }
    }
    public class DiceCardAbility_themostbalanceddiceeffectevercreated : DiceCardAbilityBase
    {
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            base.OnSucceedAttack(target);
            int why = target.MaxBreakLife / 4;
            if (why > 25)
            {
                why = 25;
            }
            target.breakDetail.TakeBreakDamage(why);
        }
    }
    public class DiceCardSelfAbility_theultimatewhenitcomestorecoveryandlight : DiceCardSelfAbilityBase
    {
        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            base.OnUseInstance(unit, self, targetUnit);
            unit.RecoverHP(15);
            unit.cardSlotDetail.RecoverPlayPoint(3);
            unit.allyCardDetail.DrawCards(2);
        }
        public override bool BeforeAddToHand(BattleUnitModel unit, BattleDiceCardModel self)
        {
            return unit.allyCardDetail.GetAllDeck().FindAll((BattleDiceCardModel x) => x.GetID() == self.GetID()).Count < 1;
        }
        public override void OnRoundStart_inHand(BattleUnitModel unit, BattleDiceCardModel self)
        {
            base.OnRoundStart_inHand(unit, self);
            int num = self.CurCost;
            if (unit.faction == Faction.Enemy && unit.cardSlotDetail.PlayPoint >= num)
            {
                if (unit.cardSlotDetail.PlayPoint >= unit.cardSlotDetail.GetMaxPlayPoint() && unit.hp >= unit.MaxHp)
                {
                    return;
                }
                unit.cardSlotDetail.LosePlayPoint(num);
                unit.RecoverHP(15);
                unit.cardSlotDetail.RecoverPlayPoint(3);
                unit.allyCardDetail.DrawCards(2);
                unit.allyCardDetail.AddNewCardToDiscarded(new LorId("TurbulenceOffice", 173));
                self.exhaust = true;
            }
        }
    }
    public class DiceCardAbility_superstrongonesidehitgo : DiceCardAbilityBase
    {
        public override void BeforeRollDice()
        {
            base.BeforeRollDice();
            if (!behavior.IsParrying())
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    dmgRate = 50
                });
            }
        }
    }




    public class PassiveAbility_Unclashable : PassiveAbilityBase
    {
        public override void OnWinParrying(BattleDiceBehavior behavior)
        {
            base.OnWinParrying(behavior);
            BattleUnitModel amongus = behavior.card.target;
            if (amongus != null)
            {
                BattlePlayingCardDataInUnitModel sussy = amongus.currentDiceAction;
                if (sussy != null)
                {
                    sussy.ApplyDiceStatBonus(DiceMatch.NextDice, new DiceStatBonus
                    {
                        max = -2
                    });
                }
            }
        }
        public override void OnLoseParrying(BattleDiceBehavior behavior)
        {
            base.OnLoseParrying(behavior);
            BattlePlayingCardDataInUnitModel among = behavior.card.owner.currentDiceAction;
            if (among != null)
            {
                among.ApplyDiceStatBonus(DiceMatch.NextDice, new DiceStatBonus
                {
                    max = -2
                });
            }
        }
    }

    public class PassiveAbility_ifitisnamedmirroritwillsurelyconflictwithsomething : PassiveAbilityBase
    {
        public override void OnRoundStart()
        {
            int amongus = 0;
            for (int i = 2; i <= this.owner.emotionDetail.EmotionLevel; i += 2 )
            {
                amongus++;
            }
            if (Singleton<StageController>.Instance.RoundTurn % 2 == 0)
            {
                base.owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Endurance, amongus, base.owner);
                base.owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Protection, amongus, base.owner);
                base.owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Vulnerable_break, amongus * 2, base.owner);
                return;
            }
            base.owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Strength, amongus, base.owner);
            base.owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.DmgUp, amongus, base.owner);
            base.owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Vulnerable, amongus * 2, base.owner);
        }
    }

    public class PassiveAbility_FairFairFairFairFairFairFair : PassiveAbilityBase
    {

        public override void OnRoundStart()
        {
            base.OnRoundStart();
            this.flag = true;
        }

        public override void OnStartParrying(BattlePlayingCardDataInUnitModel card)
        {
            base.OnStartParrying(card);
            bool flag = this.flag;
            if (flag)
            {
                BattleUnitModel target = card.target;
                bool flag2 = target == null;
                if (!flag2)
                {
                    card.ignorePower = true;
                    target.currentDiceAction.ignorePower = true;
                    this.flag = false;
                }
            }
        }

        public bool flag;
    }

    public class DiceCardAbility_Drawemostre : DiceCardAbilityBase
    {

        public override void OnSucceedAttack()
        {
            bool flag = base.owner.emotionDetail.EmotionLevel < 3;
            if (flag)
            {
                base.owner.allyCardDetail.DrawCards(1);
            }
            bool flag2 = base.owner.emotionDetail.EmotionLevel >= 3;
            if (flag2)
            {
                base.owner.allyCardDetail.DrawCards(1);
                base.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Strength, 1, base.owner);
            }
        }
    }

    public class DiceCardSelfAbility_Right : DiceCardSelfAbilityBase
    {
        public override void OnUseCard()
        {
            this._totalDamage = 0;
            this.count = 0;
        }

        public override void AfterGiveDamage(int damage, BattleUnitModel target)
        {
            this._totalDamage += damage;
        }

        public override void OnSucceedAttack()
        {
            BattleUnitModel target = this.card.target;
            bool flag = this._totalDamage >= this._activateLine && this.count < 1;
            if (flag)
            {
                this.count++;
                target.allyCardDetail.DiscardACardRandomlyByAbility(1);
                foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList_random(target.faction, 2))
                {
                    battleUnitModel.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Vulnerable, fragile, base.owner);
                }
            }
        }

        private int count;

        private int _totalDamage;

        private int _activateLine = 10;

        private int fragile = RandomUtil.Range(1, 2);
    }

    public class DiceCardSelfAbility_Ribs : DiceCardSelfAbilityBase
    {

        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                    "DrawCard_Keyword",
                    "Strength_Keyword"
                };
            }
        }

        public override void OnUseCard()
        {
            this._totalDamage = 0;
            this.count = 0;
        }

        public override void AfterGiveDamage(int damage, BattleUnitModel target)
        {
            this._totalDamage += damage;
        }

        public override void OnSucceedAttack()
        {
            bool flag = this._totalDamage >= this._activateLine && this.count < 1;
            if (flag)
            {
                base.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Strength, 1, base.owner);
                base.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.DmgUp, 1, base.owner);
                base.owner.allyCardDetail.DrawCards(1);
                this.count++;
            }
        }

        private int count;

        private int _totalDamage;

        private int _activateLine = 15;
    }


    public class DiceCardSelfAbility_boomMic : DiceCardSelfAbilityBase
    {

        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                    "Strength_Keyword"
                };
            }
        }

        public override void OnStartBattle()
        {
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList_random(base.owner.faction, 1))
            {
                battleUnitModel.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Strength, 2, base.owner);
                battleUnitModel.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.DmgUp, 2, base.owner);
            }
        }
    }

    public class DiceCardSelfAbility_Pre : DiceCardSelfAbilityBase
    {

        public override void OnStartParrying()
        {
            BattleUnitModel target = this.card.target;
            if (target == null)
            {
                return;
            }
            BattlePlayingCardDataInUnitModel currentDiceAction = target.currentDiceAction;
            if (currentDiceAction == null)
            {
                return;
            }
            currentDiceAction.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                max = -2
            });
        }
    }

    public class DiceCardAbility_Draw2OCW : DiceCardAbilityBase
    {
        public override void OnWinParrying()
        {
            base.owner.allyCardDetail.DrawCards(2);
        }
    }

    public class DiceCardAbility_Blow : DiceCardAbilityBase
    {

        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Vulnerable_Keyword"
                };
            }
        }

        public override void OnSucceedAttack()
        {
            BattleUnitModel target = base.card.target;
            if (target == null)
            {
                return;
            }
            target.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Vulnerable, 2, base.owner);
            target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Vulnerable, 2, base.owner);
        }
    }

    public class DiceCardAbility_Blowcounter : DiceCardAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                    "Energy_Keyword"
                };
            }
        }
        public override void OnSucceedAttack()
        {
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList_random(base.owner.faction, 1))
            {
                battleUnitModel.cardSlotDetail.RecoverPlayPointByCard(1);
                battleUnitModel.allyCardDetail.DrawCards(1);
            }
        }
    }

    public class DiceCardAbility_thesenamesaretooshortMight1 : DiceCardAbilityBase
    {
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            bool flag = target == null;
            if (!flag)
            {
                target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Weak, 2, base.owner);
            }
        }
        public override void OnWinParrying()
        {
            bool flag = this.behavior.DiceResultValue >= 11;
            if (flag)
            {
                {
                    bool flag2 = this.count == 0;
                    if (flag2)
                    {
                        this.count++;
                        base.ActivateBonusAttackDice();
                    }
                }

            }
        }
        private int count = 0;
    }


    public class DiceCardAbility_thesenamesaretooshortMight2 : DiceCardAbilityBase
    {
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            bool flag = target == null;
            if (!flag)
            {
                target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Disarm, 2, base.owner);
            }
        }
        public override void OnWinParrying()
        {
            bool flag = this.behavior.DiceResultValue >= 10;
            if (flag)
            {
                {
                    bool flag2 = this.count == 0;
                    if (flag2)
                    {
                        this.count++;
                        base.ActivateBonusAttackDice();
                    }
                }

            }
        }
        private int count = 0;
    }


    public class DiceCardAbility_WindDice : DiceCardAbilityBase
    {

        public override void OnSucceedAttack(BattleUnitModel target)
        {
            if (target != null && target.IsBreakLifeZero())
            {
                BattleUnitBuf_fleshweak.GainReadyBuf(target, 2);
            }
        }

        public override void AfterAction()
        {
            bool flag = !base.owner.IsBreakLifeZero() && this.reroll < base.owner.emotionDetail.EmotionLevel;
            if (flag)
            {
                this.reroll++;
                base.ActivateBonusAttackDice();
            }
        }
        private int reroll = 0;
    }


    public class BattleUnitBuf_fleshweak : BattleUnitBuf
    {

        public override AtkResist GetResistBP(AtkResist origin, BehaviourDetail detail)
        {
            return AtkResist.Vulnerable;
        }

        public override AtkResist GetResistHP(AtkResist origin, BehaviourDetail detail)
        {
            return AtkResist.Vulnerable;
        }

        public BattleUnitBuf_fleshweak(int value)
        {
            this.stack = value;
        }
        public BattleUnitBuf_fleshweak(BattleUnitModel model)
        {
            this._owner = model;
            this.stack = 0;
        }

        public void Add(int add)
        {
            this.stack += add;
            bool flag = this.stack < 1;
            if (flag)
            {
                this.Destroy();
            }
        }
        public override void OnAddBuf(int addedStack)
        {
            bool flag = this._owner.IsImmune(this.bufType);
            if (flag)
            {
                this.stack = 0;
            }
            bool flag2 = this.stack <= 0;
            if (flag2)
            {
                this.Destroy();
            }
        }


        public static void GainReadyBuf(BattleUnitModel model, int add)
        {
            BattleUnitBuf_fleshweak BattleUnitBuf_weak = model.bufListDetail.GetReadyBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_fleshweak && !x.IsDestroyed()) as BattleUnitBuf_fleshweak;
            bool flag = BattleUnitBuf_weak == null;
            if (flag)
            {
                BattleUnitBuf_weak = new BattleUnitBuf_fleshweak(model);
                model.bufListDetail.AddReadyBuf(new BattleUnitBuf_fleshweak(model));
                BattleUnitBuf_weak = (model.bufListDetail.GetReadyBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_fleshweak) as BattleUnitBuf_fleshweak);
                BattleUnitBuf_weak.Add(add);
            }
            else
            {
                BattleUnitBuf_weak.Add(add);
            }
        }
    }
}
