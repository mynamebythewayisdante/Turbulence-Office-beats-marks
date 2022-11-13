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
    public class PassiveAbility_TurbulentWill : PassiveAbilityBase
    {
        public override void OnRoundStart()
        {
            this._recoveredAmount = 0;
            if (this.owner.faction == Faction.Enemy)
            {
                foreach (BattleUnitModel among in BattleObjectManager.instance.GetAliveList_opponent(this.owner.faction))
                {
                    BattleUnitBuf sus = among.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_ununtargetable);
                    if (sus == null)
                    {
                        among.bufListDetail.AddBuf(new BattleUnitBuf_ununtargetable());
                    }
                }
            }
        }
        public override void OnRecoverHp(int amount)
        {
            this._recoveredAmount += amount;
        }
        public override void OnSucceedAttack(BattleDiceBehavior behavior)
        {
            this.owner.RecoverHP(1);
            this.owner.breakDetail.RecoverBreak(3);
            BattleCardTotalResult battleCardResultLog = this.owner.battleCardResultLog;
            if (battleCardResultLog == null)
            {
                return;
            }
            battleCardResultLog.SetPassiveAbility(this);
        }
        public override void OnWaveStart()
        {
            base.OnWaveStart();
            if (this.owner.faction == Faction.Enemy)
            {
                foreach (BattleUnitModel among in BattleObjectManager.instance.GetAliveList_opponent(this.owner.faction))
                {
                    BattleUnitBuf sus = among.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_ununtargetable);
                    if (sus == null)
                    {
                        among.bufListDetail.AddBuf(new BattleUnitBuf_ununtargetable());
                    }
                }
            }
        }
        public override void OnRoundEndTheLast()
        {
            if (this._recoveredAmount >= 5)
            {
                this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 1, null);
                this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Endurance, 1, null);
            }
            this._recoveredAmount = 0;
            if (this.owner.faction == Faction.Enemy)
            {
                foreach (BattleUnitModel among in BattleObjectManager.instance.GetAliveList_opponent(this.owner.faction))
                {
                    BattleUnitBuf sus = among.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_ununtargetable);
                    if (sus == null)
                    {
                        among.bufListDetail.AddBuf(new BattleUnitBuf_ununtargetable());
                    }
                }
            }
        }
        public class BattleUnitBuf_ununtargetable : BattleUnitBuf
        {
            public override bool NullifyNotTargetable()
            {
                return true;
            }
            public override void OnRoundEnd()
            {
                base.OnRoundEnd();
                if (this._owner.faction == Faction.Enemy)
                {
                    this.Destroy();
                }
            }
        }
        private int _recoveredAmount;
        private const int _RECOVER_AMOUNT = 1;
    }
    public class PassiveAbility_Lightbringer : PassiveAbilityBase
    {
        public override void OnSucceedAttack(BattleDiceBehavior behavior)
        {
            base.OnSucceedAttack(behavior);
            int emergencymeeting = RandomUtil.SelectOne<int>(new int[]
                {
                1, 2, 3, 4, 5, 6, 7, 8, 9
                });
            switch (emergencymeeting)
            {
                case 1:
                    behavior?.card?.target?.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Burn, 1, null);
                    break;
                case 2:
                    behavior?.card?.target?.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Bleeding, 1, null);
                    break;
                case 3:
                    behavior?.card?.target?.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Paralysis, 1, null);
                    break;
                case 4:
                    behavior?.card?.target?.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Vulnerable, 1, null);
                    break;
                case 5:
                    behavior?.card?.target?.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Weak, 1, null);
                    break;
                case 6:
                    behavior?.card?.target?.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Disarm, 1, null);
                    break;
                case 7:
                    behavior?.card?.target?.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Binding, 1, null);
                    break;
                case 8:
                    BattleUnitBuf chill = behavior?.card?.target?.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_SnowQueen_Debuf);
                    if (chill == null)
                    {
                        behavior?.card?.target?.bufListDetail.AddBuf(new BattleUnitBuf_SnowQueen_Debuf(this.owner));
                    }
                    if (chill != null)
                    {
                        chill.stack++;
                    }
                    break;
                case 9:
                    behavior?.card?.target?.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.HeavySmoke, 1, null);
                    break;
            }
        }
        public override void OnRoundStartAfter()
        {
            if (this.owner.IsDead())
            {
                return;
            }
            foreach (BattleUnitModel crewmate in BattleObjectManager.instance.GetAliveList())
            {
                if (crewmate != null)
                {
                    crewmate.cardSlotDetail.RecoverPlayPoint(1);
                }
            }
            int playPoint = this.owner.PlayPoint;
            if (playPoint > 0)
            {
                foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(base.owner.faction))
                {
                    battleUnitModel.RecoverHP(playPoint);
                    battleUnitModel.breakDetail.RecoverBreak(playPoint);
                }
            }

        }
    }
    public class PassiveAbility_SteelDetermination : PassiveAbilityBase
    {
        private int timer;
        public override void OnWaveStart()
        {
            this.owner.allyCardDetail.SetMaxDrawHand(9);
            if (this.owner.faction == Faction.Player)
            {
                this.owner.personalEgoDetail.AddCard(new LorId("TurbulenceOffice", 34));
            }
            if (this.owner.faction == Faction.Enemy)
            {
                timer = 5;
            }
        }
        public override int GetDamageReductionAll()
        {
            return 2;
        }
        public override int GetBreakDamageReductionAll(int dmg, DamageType dmgType, BattleUnitModel attacker)
        {
            return 2;
        }
        public override int MaxPlayPointAdder()
        {
            return 1;
        }
        public override void OnRoundStart()
        {
            this.owner.allyCardDetail.AddTempCard(new LorId("TurbulenceOffice", RandomUtil.SelectOne<int>(new int[]
                         { 11, 12, 13, 14, 15, 16, 17, 18, 25, 22, 23, 24, 30, 31, 32, 33, 41, 42, 43, 44, 51, 52, 53, 54, 55, 56, 57, 58, 59, 61, 62, 63, 64, 65, 66, 67, 68, 71, 72, 73, 81, 82, 83, 84, 85, 87, 91, 92, 93, 94, 95, 96, 97, 101, 102, 103, 104, 105, 106, 107, 111, 112, 113, 114, 115, 116, 117, 118, 121, 122, 123, 124, 125, 131, 132, 133, 134, 135, 141, 142, 143, 144, 145, 146, 147, 148, 149, 151, 152, 153, 154, 155, 156, 157, 161, 162, 163, 164, 165, 166, 167 })));
            if (--timer == 0)
            {
                this.owner.cardSlotDetail.RecoverPlayPoint(69);
                this.owner.allyCardDetail.AddTempCard(new LorId("TurbulenceOffice", 36));
                timer = 5;
            }
        }
    }
    public class PassiveAbility_Unifier : PassiveAbilityBase
    {
        public override int OnAddKeywordBufByCard(BattleUnitBuf buf, int stack)
        {
            return base.OnAddKeywordBufByCard(buf, stack);
        }
        public override int OnGiveKeywordBufByCard(BattleUnitBuf buf, int stack, BattleUnitModel target)
        {
            switch (buf.bufType)
            {
                case KeywordBuf.Weak:
                    this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, stack, null);
                    break;
                case KeywordBuf.Disarm:
                    this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Endurance, stack, null);
                    break;
                case KeywordBuf.Vulnerable:
                    this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Protection, stack, null);
                    break;
                case KeywordBuf.Vulnerable_break:
                    this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.BreakProtection, stack, null);
                    break;
                case KeywordBuf.Binding:
                    this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Quickness, stack, null);
                    break;
            }
            return base.OnGiveKeywordBufByCard(buf, stack, target);
        }
        public override void OnAddKeywordBufByCardForEvent(KeywordBuf keywordBuf, int stack, BufReadyType readyType)
        {
            if (keywordBuf == KeywordBuf.Strength || keywordBuf == KeywordBuf.Endurance || keywordBuf == KeywordBuf.Protection || keywordBuf == KeywordBuf.BreakProtection || keywordBuf == KeywordBuf.Quickness)
            {
                List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(this.owner.faction);
                aliveList.Remove(this.owner);
                if (aliveList.Count > 0)
                {
                    if (readyType == BufReadyType.ThisRound)
                    {
                        foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(this.owner.faction))
                        {
                            if (battleUnitModel != this.owner && !battleUnitModel.IsDead())
                            {
                                battleUnitModel.bufListDetail.AddKeywordBufThisRoundByEtc(keywordBuf, stack, null);

                            }
                        }
                    }
                    else
                    {
                        if (readyType == BufReadyType.NextRound)
                        {
                            foreach (BattleUnitModel battleUnitModel2 in BattleObjectManager.instance.GetAliveList(this.owner.faction))
                            {
                                if (battleUnitModel2 != this.owner && !battleUnitModel2.IsDead())
                                {
                                    battleUnitModel2.bufListDetail.AddKeywordBufByEtc(keywordBuf, stack, null);

                                }
                            }
                        }
                    }
                }
            }
        }
    }
    public class DiceCardAbility_nicoleteenesmokehitter : DiceCardAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                    "Smoke_Keyword",
                };
            }
        }
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            base.OnSucceedAttack(target);
            target.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Smoke, 9, this.owner);
        }
        public static string Desc = "[On Hit] Inflict 9 Smoke";
    }
    public class DiceCardSelfAbility_nicoleteenereuse : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                    "Smoke_Keyword",
                };
            }
        }
        public override void OnStartBattle()
        {
            base.OnStartBattle();

            foreach (BattleUnitModel unit in BattleObjectManager.instance.GetAliveList_opponent(this.owner.faction))
            {
                unit.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Smoke, 1, this.owner);
            }
        }
        private int impostor;
        public override void OnEndBattle()
        {
            base.OnEndBattle();
            if (this.card.target != null && this.impostor < 2)
            {
                List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList((base.owner.faction == Faction.Player) ? Faction.Enemy : Faction.Player);
                if (aliveList.Count > 0)
                {
                    impostor++;
                    BattleUnitModel target = RandomUtil.SelectOne<BattleUnitModel>(aliveList);
                    Singleton<StageController>.Instance.AddAllCardListInBattle(this.card, target, -1);
                }
            }
        }
        public static string Desc = "[Combat Start] Inflict 1 Smoke to all enemies\nThis page is reused against 2 random targets";
    }
    public class DiceCardSelfAbility_ObserveBut3xBetter : DiceCardSelfAbilityBase
    {
        public override bool IsUniteCard => true;
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                    "Strength_Keyword",
                    "Endurance_Keyword",
                    "Quickness_Keyword"
                };
            }
        }
        public override void OnStartBattle()
        {
            base.OnStartBattle();

            foreach (BattleUnitModel unit in BattleObjectManager.instance.GetAliveList(this.owner.faction))
            {
                if (unit != base.owner && !unit.bufListDetail.HasBuf<BattleUnitBuf_lightningvoice>())
                {
                    unit.bufListDetail.AddBuf(new BattleUnitBuf_lightningvoice());
                }
            }
        }
        public class BattleUnitBuf_lightningvoice : BattleUnitBuf
        {
            public override void OnUseCard(BattlePlayingCardDataInUnitModel card)
            {
                if (card.cardAbility != null && card.cardAbility.IsUniteCard)
                {
                    new BattleDiceBehavior();
                    BattleDiceCardModel battleDiceCardModel = BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(new LorId("TurbulenceOffice", 35), false));
                    if (battleDiceCardModel != null)
                    {
                        foreach (BattleDiceBehavior diceBehavior in battleDiceCardModel.CreateDiceCardBehaviorList())
                        {
                            card.AddDice(diceBehavior);
                            diceBehavior.AddAbility(new DiceCardAbility_resistanceisfutileanmognus());
                        }
                    }
                }
            }

            public override void OnRoundEnd()
            {
                this.Destroy();
            }
        }
        public override void OnUseCard()
        {
            base.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Strength, 2, base.owner);
            base.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Endurance, 2, base.owner);
            base.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Quickness, 2, base.owner);
        }
        public static string Desc = "[Unity]\n[Combat Start] For this Scene, all other allies using a Unity page gain a Slash die (3-7, [On Hit] Gain 2 Resilience) on the page (Cannot be stacked)\n[On Use] Gain 2 Strength, Endurance, and Haste next Scene";
    }
    public class DiceCardAbility_resistanceisfutileanmognus : DiceCardAbilityBase
    {
        public override void OnSucceedAttack()
        {
            base.OnSucceedAttack();
            this.owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Resistance, 2, this.owner);
        }
    }
    public class PassiveAbility_Stain : PassiveAbilityBase
    {
        public static string Name = "Stained Path";
        public static string Desc = "At the start of a scene, gain +1 Strength and Endurance for every for every three pages in hand.";

        public override void OnRoundStart()
        {
            if (this.owner.allyCardDetail.GetHand().Count >= 3)
            {
                this.owner.ShowPassiveTypo(this);
                this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 1, this.owner);
                this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Endurance, 1, this.owner);
            }
            if (this.owner.allyCardDetail.GetHand().Count >= 6)
            {
                this.owner.ShowPassiveTypo(this);
                this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 1, this.owner);
                this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Endurance, 1, this.owner);
            }
            if (this.owner.allyCardDetail.GetHand().Count >= 9)
            {
                this.owner.ShowPassiveTypo(this);
                this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 1, this.owner);
                this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Endurance, 1, this.owner);
            }
        }
    }
    public class PassiveAbility_Preparation : PassiveAbilityBase
    {
        public static string Desc = "At the start of each Scene, a random enemy's resistance to Slash attacks changes to \"Weak\" for the Scene. (Chosen from resistances that are \"Endured\" or \"Normal\"";
        public override void OnRoundStart()
        {
            BattleUnitBuf_Slashresistweak weakslash = new BattleUnitBuf_Slashresistweak();
            // get the list of enemies via GetAliveList_opponent
            var targets = BattleObjectManager.instance.GetAliveList_opponent(owner.faction)
                // of those, find the ones that are resistant to slash in some way via FindAll
                .FindAll(x => x.GetResistHP(BehaviourDetail.Slash) == AtkResist.Normal ||
                x.GetResistHP(BehaviourDetail.Slash) == AtkResist.Endure);
            // check if there are any to make RandomUtil.SelectOne not cry
            if (targets.Count > 0)
                RandomUtil.SelectOne(targets).bufListDetail.AddBuf(weakslash);
            // use ThisRound version of AddKeywordBuf
        }
    }
    public class BattleUnitBuf_Slashresistweak : BattleUnitBuf
    {
        public override AtkResist GetResistHP(AtkResist origin, BehaviourDetail detail)
        {
            if (detail == BehaviourDetail.Slash)
            {
                int newresist = ((int)origin) >= 2 ? Mathf.Clamp((int)origin - stack, 2, 6) : (int)origin;
                return (AtkResist)newresist;
            }
            return base.GetResistHP(origin, detail);
        }
        public override void OnRoundEnd()
        {
            this.Destroy();
        }
    }

    public class DiceCardSelfAbility_DiscardAndUseLightDraw1 : DiceCardSelfAbilityBase
    {
        public static string Desc = "Restore 1 Light and draw a page upon discarding a page.\n[On Use]Restore 1 Light";
        public override void OnDiscard(BattleUnitModel unit, BattleDiceCardModel self)
        {
            unit.allyCardDetail.DrawCards(1);
            unit.cardSlotDetail.RecoverPlayPointByCard(1);
        }
        public override void OnUseCard()
        {
            base.owner.cardSlotDetail.RecoverPlayPointByCard(1);
        }
    }
    public class DiceCardSelfAbility_CombustibleDiscardDamage : DiceCardSelfAbilityBase
    {
        public static string Desc = "When this page is discarded, exhaust it and deal 6 damage to all enemies\nIf this page is in hand for 2 scenes, take 10 damage and exhaust this page.\n[On Use] Discard a page with the lowest Cost; Draw 1 page";
        public override void OnDiscard(BattleUnitModel unit, BattleDiceCardModel self)
        {
            self.exhaust = true;
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList_opponent(unit.faction))
            {
                if (battleUnitModel != null)
                {
                    battleUnitModel.TakeDamage(6, DamageType.Emotion, null, KeywordBuf.None);
                    SingletonBehavior<DiceEffectManager>.Instance.CreateCreatureEffect("1/MatchGirl_Footfall", 1f, battleUnitModel.view, null, 2f).AttachEffectLayer();
                    SoundEffectPlayer soundEffectPlayer = SingletonBehavior<SoundEffectManager>.Instance.PlayClip("Creature/MatchGirl_Explosion", false, 1f, null);
                    if (soundEffectPlayer != null)
                    {
                        soundEffectPlayer.SetGlobalPosition(battleUnitModel.view.WorldPosition);
                    }
                }
            }
        }
        public override void OnUseCard()
        {
            base.owner.allyCardDetail.DiscardACardLowest();
            base.owner.allyCardDetail.DrawCards(1);
        }
        public override void OnRoundStart_inHand(BattleUnitModel unit, BattleDiceCardModel self)
        {
            BattleDiceCardBuf combust = self.GetBufList().Find((BattleDiceCardBuf x) => x is BattleDiceCardBuf_TOCombustibleProgramExplode);
            if (combust == null)
            {
                self.AddBuf(new BattleDiceCardBuf_TOCombustibleProgramExplode());
            }
            if (combust != null)
            {

            }
        }
    }
    public class BattleDiceCardBuf_TOCombustibleProgramExplode : BattleDiceCardBuf
    {
        public BattleDiceCardBuf_TOCombustibleProgramExplode()
        {
            this.turn = 0;
            this._stack = 2;
        }
        protected override string keywordId
        {
            get
            {
                return "combustiblelemons";
            }
        }

        public void Add()
        {
            this._stack--;
        }

        public override void OnUseCard(BattleUnitModel owner)
        {
            base.OnUseCard(owner);
            this.used = true;
        }

        public override void OnRoundEnd()
        {
            base.OnRoundEnd();
            bool flag = this.used;
            if (this.used)
            {
                base.Destroy();
            }
            else
            {
                this.turn++;
                this.Add();
                if (this.turn >= 2)
                {
                    BattleDiceCardModel card = this._card;
                    BattleUnitModel battleUnitModel = (card != null) ? card.owner : null;
                    if (battleUnitModel != null)
                    {
                        battleUnitModel.TakeDamage(10, DamageType.Emotion, battleUnitModel, KeywordBuf.None);
                        SingletonBehavior<DiceEffectManager>.Instance.CreateCreatureEffect("1/MatchGirl_Footfall", 1f, this._card.owner.view, null, 2f).AttachEffectLayer();
                        SoundEffectPlayer soundEffectPlayer = SingletonBehavior<SoundEffectManager>.Instance.PlayClip("Creature/MatchGirl_Explosion", false, 1f, null);
                        if (soundEffectPlayer != null)
                        {
                            soundEffectPlayer.SetGlobalPosition(this._card.owner.view.WorldPosition);
                        }
                        this.turn = 0;
                    }
                    this._card.exhaust = true;
                }
            }
        }
        public override void OnDiscard(BattleUnitModel owner, BattleDiceCardModel card)
        {
            this.turn = 0;
        }

        private const int _turn = 2;

        private int turn;

        private bool used;
    }
    public class DiceCardSelfAbility_Strength1discardUsepower : DiceCardSelfAbilityBase
    {
        public static string Desc = "When discarded, gain 1 Strength\n[On Use] All dice on this page gain +1 Power";
        public override void OnDiscard(BattleUnitModel unit, BattleDiceCardModel self)
        {
            unit.bufListDetail.AddKeywordBufByCard(KeywordBuf.Strength, 1, unit);
        }
        public override void OnUseCard()
        {
            this.card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = 1
            });
        }
    }
    public class DiceCardSelfAbility_DrawRedraw : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] Discard a page of the lowest cost and draw all copies of that page from the deck";
        public override void OnUseCard()
        {
            BattleDiceCardModel battleDiceCardModel = base.owner.allyCardDetail.DiscardACardLowest();
            if (battleDiceCardModel != null)
            {
                base.owner.allyCardDetail.DrawCardsAllSpecific(battleDiceCardModel.GetID());
            }
        }
    }

    public class DiceCardAbility_CombustibleProgram : DiceCardAbilityBase
    {
        public static string Desc = "Add 'Combustible Program' to each other's hands";
        public override void AfterAction()
        {
            base.owner.allyCardDetail.AddNewCard(new LorId(packageId, 93));
            BattleUnitModel target = base.card.target;
            if (target != null)
            {
                target.allyCardDetail.AddNewCard(new LorId(packageId, 93));
            }
        }
        private string packageId = "TurbulenceOffice";
    }
    public class DiceCardAbility_CombustibleProgramHit : DiceCardAbilityBase
    {
        public static string Desc = "[On Hit] Add 'Combustible Program' to each other's hands";
        public override void OnSucceedAttack()
        {
            base.owner.allyCardDetail.AddNewCard(new LorId(packageId, 93));
            BattleUnitModel target = base.card.target;
            if (target != null)
            {
                target.allyCardDetail.AddNewCard(new LorId(packageId, 93));
            }
        }
        private string packageId = "TurbulenceOffice";
    }
    public class DiceCardAbility_CautiousProgram : DiceCardAbilityBase
    {
        public static string Desc = "Add 'Cautious Program' to each other's hands";
        public override void AfterAction()
        {
            base.owner.allyCardDetail.AddNewCard(new LorId(packageId, 94));
            BattleUnitModel target = base.card.target;
            if (target != null)
            {
                target.allyCardDetail.AddNewCard(new LorId(packageId, 94));
            }
        }
        private string packageId = "TurbulenceOffice";
    }
    public class DiceCardPriority_danceblunt : DiceCardPriorityBase
    {
        public override int GetPriorityBonus(BattleUnitModel owner)
        {
            BattleUnitBuf hit = owner.bufListDetail.GetActivatedBuf(KeywordBuf.HitPowerUp);
            if (hit != null)
            {
                return 20;
            }
            return 0;
        }
    }
    public class DiceCardPriority_danceblunt2 : DiceCardPriorityBase
    {
        public override int GetPriorityBonus(BattleUnitModel owner)
        {
            BattleUnitBuf hit = owner.bufListDetail.GetActivatedBuf(KeywordBuf.HitPowerUp);
            BattleUnitBuf strongness = owner.bufListDetail.GetActivatedBuf(KeywordBuf.Strength);
            if (hit != null || strongness != null)
            {
                return 40;
            }
            return 0;
        }
    }
    public class DiceCardSelfAbility_ShuffleDeckDraw : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] Discard all pages from hand and draw pages equal to the amount discarded";
        public override void OnUseCard()
        {
            int count = base.owner.allyCardDetail.GetHand().Count;
            if (count > 0)
            {
                base.owner.allyCardDetail.DiscardACardByAbility(base.owner.allyCardDetail.GetHand());
                base.owner.allyCardDetail.DrawCards(count);
            }

        }
    }
    public class DiceCardSelfAbility_FullPageDiscardPower : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] Gain power by +1 for each page in hand; Discard all pages from hand";
        public override void OnUseCard()
        {

            int count = base.owner.allyCardDetail.GetHand().Count;
            if (count > 0)
            {
                this.card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
                {
                    power = count
                });
                base.owner.allyCardDetail.DiscardACardByAbility(base.owner.allyCardDetail.GetHand());
            }
        }
    }
    public class DiceCardAbility_Fragile2atkTwoRound : DiceCardAbilityBase
    {
        public static string Desc = "[On Hit] Inflict 2 Fragile next Scene and the Scene after";
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            if (target != null)
            {
                target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Vulnerable, 2, base.owner);
            }
            if (target != null)
            {
                target.bufListDetail.AddKeywordBufNextNextByCard(KeywordBuf.Vulnerable, 2, base.owner);
            }
        }
    }
    public class PassiveAbility_DiscardFragile : PassiveAbilityBase
    {
        public static string Desc = "All pages in hand gain this effect: When discarded, inflict 1 Fragile to a random enemy.";
        public override void OnRoundStart()
        {
            List<BattleDiceCardModel> hand = this.owner.allyCardDetail.GetHand();
            int num = 0;
            int deck = hand.Count;
            while (num < deck && hand.Count > 0)
            {
                BattleDiceCardModel battleDiceCardModel = RandomUtil.SelectOne<BattleDiceCardModel>(hand);
                battleDiceCardModel.AddBuf(new BattleDiceCardBuf_DiscardFragile());
                hand.Remove(battleDiceCardModel);
                num++;
            }
        }
    }
    public class BattleDiceCardBuf_DiscardFragile : BattleDiceCardBuf
    {
        public override void OnDiscard(BattleUnitModel owner, BattleDiceCardModel card)
        {
            List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList((owner.faction == Faction.Player) ? Faction.Enemy : Faction.Player);
            BattleUnitModel RandomEnemy = RandomUtil.SelectOne(aliveList);
            RandomEnemy.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Vulnerable, 1, null);
        }
        public override void OnRoundEnd()
        {
            base.Destroy();
        }
    }
    public class DiceCardAbility_recoverlike10stagger : DiceCardAbilityBase
    {
        public override void OnSucceedAttack()
        {
            base.owner.breakDetail.RecoverBreak(10);
        }
        public static string Desc = "[On Hit] Recover 10 Stagger resist";
    }
    public class DiceCardAbility_3hastehitting : DiceCardAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Quickness_Keyword"
                };
            }
        }
        public override void OnSucceedAttack()
        {
            this.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Quickness, 2, base.owner);
        }
        public static string Desc = "[On Hit] Gain 2 Haste next Scene";
    }
    public class PassiveAbility_flexhard : PassiveAbilityBase
    {
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (behavior.Detail == BehaviourDetail.Hit)
            {
                BattleCardTotalResult battleCardResultLog = this.owner.battleCardResultLog;
                if (battleCardResultLog != null)
                {
                    battleCardResultLog.SetPassiveAbility(this);
                }
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = 2,
                    dmg = 6
                });
            }
            if (behavior.Detail != BehaviourDetail.Hit)
            {
                BattleCardTotalResult battleCardResultLog = this.owner.battleCardResultLog;
                if (battleCardResultLog != null)
                {
                    battleCardResultLog.SetPassiveAbility(this);
                }
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = -2
                });
            }
        }
        public override void OnWinParrying(BattleDiceBehavior behavior)
        {
            if (behavior.Detail != BehaviourDetail.Hit)
            {
                BattleCardTotalResult battleCardResultLog = this.owner.battleCardResultLog;
                if (battleCardResultLog != null)
                {
                    battleCardResultLog.SetPassiveAbility(this);
                }
                this.owner.RecoverHP(10);
                this.owner.breakDetail.RecoverBreak(10);
                this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 1, this.owner);
            }
        }
    }
    public class PassiveAbility_loveofmuscle : PassiveAbilityBase
    {
        public override int OnAddKeywordBufByCard(BattleUnitBuf buf, int stack)
        {
            if (buf.bufType == KeywordBuf.Strength)
            {
                BattleCardTotalResult battleCardResultLog = this.owner.battleCardResultLog;
                if (battleCardResultLog != null)
                {
                    battleCardResultLog.SetPassiveAbility(this);
                }
                return 1;
            }
            return 0;
        }
    }
    public class DiceCardSelfAbility_Barrage : DiceCardSelfAbilityBase
    {
        public static string Desc = "If this page hits an enemy, use it again on another random enemies up to 2 times. (Does not re-target already hit enemies)";
        private readonly List<BattleUnitModel> targets = new List<BattleUnitModel>();
        public bool hit;
        public override void OnSucceedAttack(BattleDiceBehavior behavior)
        {
            if (card.target is null || targets.Contains(card.target)) return;
            hit = true;
            targets.Add(card.target);
        }
        public override void OnEndBattle()
        {
            if (hit && targets.Count <= 2)
            {
                List<BattleUnitModel> retargets = BattleObjectManager.instance.GetAliveList_opponent(owner.faction).FindAll(x => !targets.Contains(x));
                if (retargets.Count > 0)
                    Singleton<StageController>.Instance.AddAllCardListInBattle(card, RandomUtil.SelectOne(retargets));
            }
            hit = false;
        }
    }
    public class DiceCardAbility_upreditcab : DiceCardAbilityBase
    {
        public override void OnSucceedAttack()
        {
            base.OnSucceedAttack();
            this.owner.cardSlotDetail.RecoverPlayPoint(1);
            this.owner.allyCardDetail.DrawCards(1);
        }
    }

    public class DiceCardSelfAbility_unpredictableskittering : DiceCardSelfAbilityBase
    {
        public override void OnStartOneSideAction()
        {
            base.OnStartOneSideAction();
            foreach (BattleDiceBehavior battleDiceBehavior in this.card.GetDiceBehaviorList())
            {
                battleDiceBehavior.behaviourInCard = battleDiceBehavior.behaviourInCard.Copy();
                battleDiceBehavior.behaviourInCard.Detail = BehaviourDetail.Slash;
                battleDiceBehavior.behaviourInCard.Type = BehaviourType.Atk;
                battleDiceBehavior.AddAbility(new DiceCardAbility_upreditcab());
            }
        }
        public static string Desc = "[In a one sided attack] All dice on this page become Slash dice and gain [On Hit] Draw 1 page and Restore 1 Light";
    }
    public class DiceCardSelfAbility_feastingrat : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Hit] Steal a random buff from target";
        private void effect(BattleUnitBuf sauce)
        {
            List<BattleUnitBuf> list = new List<BattleUnitBuf>();
            BattleUnitModel enemy = this.card.target;
            list = enemy.bufListDetail.GetActivatedBufList();
            if (list.Count > 0 && list != null)
            {
                if (this.isSus(sauce) && sauce != null)
                {
                    KeywordBuf buftype = sauce.bufType;
                    this.owner.bufListDetail.AddKeywordBufThisRoundByCard(buftype, sauce.stack, this.owner);
                    enemy.bufListDetail.RemoveBufAll(buftype);
                }
                if (sauce != null && sauce.GetBufIcon() != null && !this.isSus(sauce))
                {
                    this.owner.bufListDetail.AddBuf(sauce);
                    enemy.bufListDetail.RemoveBuf(sauce);
                }
            }
        }
        public override void OnSucceedAttack()
        {
            BattleUnitModel enemy = this.card.target;
            List<BattleUnitBuf> list = new List<BattleUnitBuf>();
            list = enemy.bufListDetail.GetActivatedBufList();
            if (list.Count > 0 && list != null)
            {
                BattleUnitBuf buff = RandomUtil.SelectOne(list);
                if (buff.GetBufIcon() != null && buff != null)
                {
                    this.effect(buff);
                }
            }
        }
        private bool isSus(BattleUnitBuf buf)
        {
            KeywordBuf bufType = buf.bufType;
            bool result = false;
            switch (bufType)
            {
                case KeywordBuf.Burn:
                case KeywordBuf.Paralysis:
                case KeywordBuf.Bleeding:
                case KeywordBuf.Vulnerable:
                case KeywordBuf.Vulnerable_break:
                case KeywordBuf.Weak:
                case KeywordBuf.Disarm:
                case KeywordBuf.Binding:
                case KeywordBuf.Regeneration:
                case KeywordBuf.Protection:
                case KeywordBuf.BreakProtection:
                case KeywordBuf.Strength:
                case KeywordBuf.Endurance:
                case KeywordBuf.Quickness:
                case KeywordBuf.Blurry:
                case KeywordBuf.DmgUp:
                case KeywordBuf.SlashPowerUp:
                case KeywordBuf.PenetratePowerUp:
                case KeywordBuf.HitPowerUp:
                case KeywordBuf.DefensePowerUp:
                case KeywordBuf.Stun:
                case KeywordBuf.WarpCharge:
                case KeywordBuf.Smoke:
                case KeywordBuf.NullifyPower:
                case KeywordBuf.HalfPower:
                case KeywordBuf.Shock:
                case KeywordBuf.RedShoes:
                case KeywordBuf.SnowQueenPower:
                case KeywordBuf.UniverseCardBuf:
                case KeywordBuf.UniverseEnlightenment:
                case KeywordBuf.FairyCare:
                case KeywordBuf.SpiderBudCocoon:
                case KeywordBuf.SingingMachineRecital:
                case KeywordBuf.QueenOfHatredSign:
                case KeywordBuf.QueenOfHatredHatred:
                case KeywordBuf.KnightOfDespairBlessing:
                case KeywordBuf.SweeperRevival:
                case KeywordBuf.SweeperDup:
                case KeywordBuf.TeddyLove:
                case KeywordBuf.CB_BigBadWolf_Stealth:
                case KeywordBuf.CB_CopiousBleeding:
                case KeywordBuf.CB_BlackSwanDeadBro:
                case KeywordBuf.CB_UniverseDecreaseMaxBp:
                case KeywordBuf.AllPowerUp:
                case KeywordBuf.TakeBpDmg:
                case KeywordBuf.DecreaseSpeedTo1:
                case KeywordBuf.BloodStackBlock:
                case KeywordBuf.Vibrate:
                case KeywordBuf.IndexRelease:
                case KeywordBuf.Decay:
                case KeywordBuf.BurnSpread:
                case KeywordBuf.BurnBreak:
                case KeywordBuf.Seal:
                case KeywordBuf.SealKeyword:
                case KeywordBuf.RedMist:
                case KeywordBuf.Fairy:
                case KeywordBuf.RedMistEgo:
                case KeywordBuf.NicolaiTarget:
                case KeywordBuf.MyoBerserk:
                case KeywordBuf.Maxim:
                case KeywordBuf.CB_RedHoodTarget:
                case KeywordBuf.CB_NothingSkin:
                case KeywordBuf.CB_NothingMimic:
                case KeywordBuf.RedHoodChange:
                case KeywordBuf.FreischutzChange:
                case KeywordBuf.WhiteNightChange:
                case KeywordBuf.PurpleSlash:
                case KeywordBuf.PurplePenetrate:
                case KeywordBuf.PurpleHit:
                case KeywordBuf.PurpleDefense:
                case KeywordBuf.PurpleCoolTime:
                case KeywordBuf.JaeheonPuppetThread:
                case KeywordBuf.JaeheonMark:
                case KeywordBuf.OswaldDaze:
                case KeywordBuf.Resistance:
                case KeywordBuf.HeavySmoke:
                case KeywordBuf.Roland2PhaseTakeDamaged:
                case KeywordBuf.Arrest:
                case KeywordBuf.ForbidRecovery:
                case KeywordBuf.UpSurge:
                case KeywordBuf.KeterFinal_Eager:
                case KeywordBuf.KeterFinal_FailLying:
                case KeywordBuf.KeterFinal_SuccessLying:
                case KeywordBuf.KeterFinal_ChangeCostAll:
                case KeywordBuf.KeterFinal_ChangeLibrarianHands:
                case KeywordBuf.KeterFinal_DoubleEmotion:
                case KeywordBuf.KeterFinal_Light:
                case KeywordBuf.KeterFinal_angela_ego:
                case KeywordBuf.Nail:
                case KeywordBuf.ClawCounter:
                case KeywordBuf.Emotion_Sin:
                case KeywordBuf.Alriune_Debuf:
                case KeywordBuf.CB_BigBadWolf_Scar:
                    result = true;
                    break;
            }
            return result;
        }
    }
    public class DiceCardAbility_feastfitforarat : DiceCardAbilityBase
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
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            base.OnSucceedAttack();
            target.cardSlotDetail.LoseWhenStartRound(1);
            this.owner.cardSlotDetail.RecoverPlayPointByCard(1);
        }
        public static string Desc = "[On Hit] Restore 1 Light; target loses 1 Light next Scene";
    }
    public class DiceCardAbility_feastfitforaking : DiceCardAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "DrawCard_Keyword"
                };
            }
        }
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            this.owner.allyCardDetail.DrawCards(1);
            target.allyCardDetail.DiscardACardRandomlyByAbility(1);
        }
        public static string Desc = "[On Hit] Draw 1 page; target discards 1 page";
    }
    public class DiceCardSelfAbility_stopthathievingrat : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "anotherspecialrangekeyword"
                };
            }
        }
        public static string Desc = "[Special Range]\nCannot be redirected. This page is used against all enemies. \nWhen clashing against Counter dice, set the value of the target's die to zero.";
    }

    public class DiceCardAbility_binding2atkTwoRound : DiceCardAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                    "Binding_Keyword"
                };
            }
        }
        public override void OnSucceedAreaAttack(BattleUnitModel target)
        {
            if (target != null)
            {
                target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Binding, 2, base.owner);
            }
            if (target != null)
            {
                target.bufListDetail.AddKeywordBufNextNextByCard(KeywordBuf.Binding, 2, base.owner);
            }
        }
        public static string Desc = "[On Hit] Inflict 2 Bind next Scene and the Scene after";
    }
    public class DiceCardAbility_weak2atkTwoRound : DiceCardAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                    "Weak_Keyword"
                };
            }
        }
        public override void OnSucceedAreaAttack(BattleUnitModel target)
        {
            if (target != null)
            {
                target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Weak, 2, base.owner);
            }
            if (target != null)
            {
                target.bufListDetail.AddKeywordBufNextNextByCard(KeywordBuf.Weak, 2, base.owner);
            }
        }
        public static string Desc = "[On Hit] Inflict 2 Feeble next Scene and the Scene after";
    }
    public class DiceCardAbility_vulnerable2atkTwoRound : DiceCardAbilityBase
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
        public override void OnSucceedAreaAttack(BattleUnitModel target)
        {
            if (target != null)
            {
                target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Vulnerable, 2, base.owner);
            };
            if (target != null)
            {
                target.bufListDetail.AddKeywordBufNextNextByCard(KeywordBuf.Vulnerable, 2, base.owner);
            }
        }
        public static string Desc = "[On Hit] Inflict 2 Fragile next Scene and the Scene after";
    }
    public class DiceCardSelfAbility_drawCard2cardPowerDown1target : DiceCardSelfAbilityBase
    {
        public static string Desc = "Draw 2 Pages, [Start of Clash] Reduce Power of all target's dice by 1";
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                    "DrawCard_Keyword"
                };
            }
        }
        public override void OnUseCard()
        {
            base.owner.allyCardDetail.DrawCards(2);
        }
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
                power = -1
            });
        }
    }
    public class PassiveAbility_barriermuscle : PassiveAbilityBase
    {
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            BattleUnitBuf activatedBuf = owner.bufListDetail.GetActivatedBuf(KeywordBuf.Strength);
            if (base.IsDefenseDice(behavior.Detail) && activatedBuf != null)
            {
                BattleCardTotalResult battleCardResultLog = this.owner.battleCardResultLog;
                if (battleCardResultLog != null)
                {
                    battleCardResultLog.SetPassiveAbility(this);
                }
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = activatedBuf.stack
                });
            }
        }
    }
    public class DiceCardAbility_ihavetomakehasteonclashwinagain : DiceCardAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Quickness_Keyword"
                };
            }
        }
        public override void OnWinParrying()
        {
            this.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Quickness, 2, base.owner);
        }
        public static string Desc = "[On Clash Win] Gain 2 Haste next Scene";
    }
    public class DiceCardSelfAbility_thricethehastethricethewaste : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Quickness_Keyword"
                };
            }
        }

        public override void OnUseCard()
        {
            base.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Quickness, 3, base.owner);
        }
        public static string Desc = "[On Use] Gain 3 Haste next Scene";
    }
    public class DiceCardSelfAbility_groupworkout : DiceCardSelfAbilityBase
    {
        public override void OnUseCard()
        {
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(this.card.owner.faction))
            {
                battleUnitModel.bufListDetail.AddKeywordBufByCard(KeywordBuf.HitPowerUp, 1, base.owner);
            }
        }
        public static string Desc = "[On Use] All allies' Blunt dice gain +1 Power next Scene";
    }
    public class DiceCardSelfAbility_groupworkout2 : DiceCardSelfAbilityBase
    {
        public override void OnUseCard()
        {
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(this.card.owner.faction))
            {
                battleUnitModel.bufListDetail.AddKeywordBufByCard(KeywordBuf.HitPowerUp, 2, base.owner);
            }
        }
        public static string Desc = "[On Use] All allies' Blunt dice gain +2 Power next Scene";
    }
    public class DiceCardSelfAbility_recoverlike10hp : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Recover_Keyword"
                };
            }
        }

        public override void OnUseCard()
        {
            base.owner.RecoverHP(10);
        }
        public static string Desc = "[On Use] Recover 10 HP";
    }
    public class PassiveAbility_GlassCannon00122 : PassiveAbilityBase
    {
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (behavior.Type == BehaviourType.Atk)
            {
                if (behavior.card?.card.GetSpec().Ranged == CardRange.Near)
                {
                    behavior.ApplyDiceStatBonus(new DiceStatBonus
                    {
                        dmgRate = 200,
                        breakRate = 200
                    });
                }
                else if (behavior.card?.card.GetSpec().Ranged == CardRange.Far)
                {
                    behavior.ApplyDiceStatBonus(new DiceStatBonus
                    {
                        dmgRate = 50,
                        breakRate = 50
                    });
                }
            }
        }
        public override void OnStartParrying(BattlePlayingCardDataInUnitModel card)
        {
            if (card?.target.currentDiceAction.card.GetSpec().Ranged == CardRange.Near)
            {
                card.target.currentDiceAction.ApplyDiceStatBonus(DiceMatch.AllAttackDice, new DiceStatBonus
                {
                    dmgRate = 200,
                    breakRate = 200
                });
            }
            else if (card?.target.currentDiceAction.card.GetSpec().Ranged == CardRange.Far)
            {
                card.target.currentDiceAction.ApplyDiceStatBonus(DiceMatch.AllAttackDice, new DiceStatBonus
                {
                    dmgRate = 50,
                    breakRate = 50
                });
            }
        }
        public override void OnStartTargetedOneSide(BattlePlayingCardDataInUnitModel attackerCard)
        {
            if (attackerCard?.card.GetSpec().Ranged == CardRange.Near)
            {
                attackerCard.ApplyDiceStatBonus(DiceMatch.AllAttackDice, new DiceStatBonus
                {
                    dmgRate = 200,
                    breakRate = 200
                });
            }
            else if (attackerCard?.card.GetSpec().Ranged == CardRange.Far)
            {
                attackerCard.ApplyDiceStatBonus(DiceMatch.AllAttackDice, new DiceStatBonus
                {
                    dmgRate = 50,
                    breakRate = 50
                });
            }
        }
    }
    public class PassiveAbility_Instanity00122 : PassiveAbilityBase
    {
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            int Min = RandomUtil.Range(-2, 2);
            int Max = RandomUtil.Range(-2, 2);
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                min = Min,
                max = Max
            });
        }
        public override void OnStartParrying(BattlePlayingCardDataInUnitModel card)
        {
            int Min = RandomUtil.Range(-2, 2);
            int Max = RandomUtil.Range(-2, 2);
            card.target.currentDiceAction.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                min = Min,
                max = Max
            });
        }
    }
    public class PassiveAbility_TimeStop00122 : PassiveAbilityBase
    {
        public class BattleUnitBuf_TimeStop_Immobilize : BattleUnitBuf
        {
            public override int SpeedDiceBreakedAdder()
            {
                return 10;
            }
            public override void OnRoundEnd()
            {
                this.Destroy();
            }
        }
        public override void OnDieOtherUnit(BattleUnitModel unit)
        {
            if (unit.faction == this.owner.faction)
            {
                List<BattleUnitModel> list = new List<BattleUnitModel>();
                foreach (BattleUnitModel Enemies in BattleObjectManager.instance.GetAliveList_opponent(this.owner.faction))
                {
                    list.Add(Enemies);
                }
                BattleUnitModel RandomEnemy = RandomUtil.SelectOne(list);
                RandomEnemy.bufListDetail.AddReadyBuf(new BattleUnitBuf_TimeStop_Immobilize());
            }
        }
    }
    public class DiceCardAbility_whywouldyoudothistomeprojectmoon : DiceCardAbilityBase
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
        public override void OnWinParrying()
        {
            this.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Strength, 1, base.owner);
            this.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Strength, 1, base.owner);
        }
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            base.OnSucceedAttack(target);
            if (!this.behavior.IsParrying())
            {
                this.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Strength, 1, base.owner);
                this.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Strength, 1, base.owner);
            }
        }
        public static string Desc = "[On Clash Win][On Hit] Gain 1 Strength next Scene 2 times";
    }
    public class DiceCardSelfAbility_beats_whywouldyoudothistomesmile00122ithoughtyouhadallthishandledicantbelievethis : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] Draw 1 page and gain 4 Charge";
        public override void OnUseCard()
        {
            base.OnUseCard();
            base.owner.allyCardDetail.DrawCards(1);
            base.owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.WarpCharge, 4, null);
        }
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "DrawCard_Keyword",
                "WarpCharge"
                };
            }
        }
    }
    public class DiceCardSelfAbility_beats_wtfthereweremorethisisunbelievable : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] Restore 2 Light and gain 5 Charge";
        public override void OnUseCard()
        {
            base.OnUseCard();
            base.owner.cardSlotDetail.RecoverPlayPointByCard(2);
            base.owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.WarpCharge, 5, null);
        }
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Energy_Keyword",
                "WarpCharge"
                };
            }
        }
    }
    public class PassiveAbility_gunman : PassiveAbilityBase
    {
        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.card.GetSpec().Ranged == CardRange.Far)
            {
                curCard.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
                {
                    power = 2
                });
            }
        }

        public override void OnStartBattle()
        {
            DiceCardXmlInfo cardItem = ItemXmlDataList.instance.GetCardItem(new LorId("TurbulenceOffice", 168), false);
            new DiceBehaviour();
            List<BattleDiceBehavior> list = new List<BattleDiceBehavior>();
            int num = 0;
            foreach (DiceBehaviour diceBehaviour in cardItem.DiceBehaviourList)
            {
                BattleDiceBehavior battleDiceBehavior = new BattleDiceBehavior();
                battleDiceBehavior.behaviourInCard = diceBehaviour.Copy();
                battleDiceBehavior.SetIndex(num++);
                list.Add(battleDiceBehavior);
            }
            this.owner.cardSlotDetail.keepCard.AddBehaviours(cardItem, list);
        }
    }
    public class PassiveAbility_danmaku : PassiveAbilityBase
    {
        public override void OnSucceedAttack(BattleDiceBehavior behavior)
        {
            if (behavior.card.target.passiveDetail.HasPassive<PassiveAbility_250113>() ||
                behavior.card.target.passiveDetail.HasPassive<PassiveAbility_250113>())
            {
                behavior.card.target.TakeDamage(behavior.DiceResultValue, DamageType.Card_Ability, this.owner, KeywordBuf.None);
            }
            if (behavior.DiceResultDamage == 0 && !behavior.card.target.passiveDetail.HasPassive<PassiveAbility_250113>())
            {
                behavior.card.target.TakeDamage(5, DamageType.Card_Ability, this.owner, KeywordBuf.None);
            }
        }
        public override void BeforeGiveDamage(BattleDiceBehavior behavior)
        {
            base.BeforeGiveDamage(behavior);
        }
    }
    public class PassiveAbility_sleepy : PassiveAbilityBase
    {
        public override void OnStartTargetedOneSide(BattlePlayingCardDataInUnitModel attackerCard)
        {
            this.Hit = true;
        }
        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            this.Used = true;
        }
        public override void OnRoundEndTheLast()
        {
            base.OnRoundEndTheLast();
            if (this.Hit && !this.Used)
            {
                this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.DmgUp, 2);
                this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 3);
                this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Protection, 2);
            }
            else if (!this.Hit && !this.Used)
            {
                int num = this.owner.MaxHp / 100;
                this.owner.cardSlotDetail.RecoverPlayPoint(2);
                this.owner.RecoverHP(this.owner.MaxHp / 10);
            }
            this.Hit = false;
            this.Used = false;
        }
        private bool Hit;
        private bool Used;
    }
    public class DiceCardAbility_beats_amongus : DiceCardAbilityBase
    {
        public static string Desc = "[On Clash Win] Gain 3 Charge";
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "WarpCharge"
                };
            }

        }
        public override void OnWinParrying()
        {
            base.OnWinParrying();
            base.owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.WarpCharge, 3, null);
        }
    }
    public class DiceCardSelfAbility_beats_whatiftherewasababywhocouldnolongerage : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] Spend 5 Charge to draw 2 pages";
        public override void OnUseCard()
        {
            base.OnUseCard();
            BattleUnitBuf_warpCharge battleUnitBuf_warpCharge = base.owner.bufListDetail.GetActivatedBuf(KeywordBuf.WarpCharge) as BattleUnitBuf_warpCharge;
            if (battleUnitBuf_warpCharge != null && battleUnitBuf_warpCharge.stack >= 5)
            {
                battleUnitBuf_warpCharge.UseStack(5, true);
                base.owner.allyCardDetail.DrawCards(2);
            }
        }
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "DrawCard_Keyword",
                "WarpCharge"
                };
            }
        }
    }
    public class DiceCardSelfAbility_beats_turelyimpeccable : DiceCardSelfAbilityBase
    {
        public static string Desc = "[Combat Start] Spend 6 Charge to give all allies 1 Strength this Scene and boost the power of all dice on this page by +1";
        public override void OnStartBattleAfterCreateBehaviour()
        {
            base.OnStartBattleAfterCreateBehaviour();
            BattleUnitBuf_warpCharge battleUnitBuf_warpCharge = base.owner.bufListDetail.GetActivatedBuf(KeywordBuf.WarpCharge) as BattleUnitBuf_warpCharge;
            if (battleUnitBuf_warpCharge != null && battleUnitBuf_warpCharge.stack >= 6)
            {
                battleUnitBuf_warpCharge.UseStack(6, true);
                this.card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
                {
                    power = 1
                });
                foreach (BattleUnitModel us in BattleObjectManager.instance.GetAliveList(this.owner.faction))
                {
                    us.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Strength, 1, this.owner);
                }
            }
        }
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Strength_Keyword",
                "WarpCharge"
                };
            }
        }
    }
    public class DiceCardAbility_beats_silencebetweentwostrikes : DiceCardAbilityBase
    {
        public static string Desc = "Spend all charge; increase damage and Stagger damage by the amount spent. If target has \"Marked for Death\" increase damage and Stagger damage by the natural roll";
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "WarpCharge"
                };
            }

        }
        private int chargeStack;
        public override void BeforeGiveDamage()
        {
            BattleUnitBuf sus = behavior.card.target.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_clownedmark);
            BattleUnitBuf_warpCharge battleUnitBuf_warpCharge = base.owner.bufListDetail.GetActivatedBuf(KeywordBuf.WarpCharge) as BattleUnitBuf_warpCharge;
            if (battleUnitBuf_warpCharge != null && battleUnitBuf_warpCharge.stack > 0)
            {
                this.chargeStack = battleUnitBuf_warpCharge.stack;
                battleUnitBuf_warpCharge.UseStack(battleUnitBuf_warpCharge.stack, true);
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    dmg = chargeStack,
                    breakDmg = chargeStack
                });
            }
            if (sus != null)
            {
                this.behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    dmg = this.behavior.DiceVanillaValue,
                    breakDmg = this.behavior.DiceVanillaValue
                });
            }
        }
    }
    public class PassiveAbility_Beats_StormBlade : PassiveAbilityBase
    {
        public class DiceCardAbility_Beats_InflictParalysis : DiceCardAbilityBase
        {
            public override void OnSucceedAttack()
            {
                this.card.target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Paralysis, 1, owner);
            }
        }
        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            curCard.ApplyDiceAbility(DiceMatch.NextAttackDice, new DiceCardAbility_Beats_InflictParalysis());
        }
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            int Paralysis = behavior.card.target.bufListDetail.GetKewordBufStack(KeywordBuf.Paralysis);
            if (Paralysis < 3)
            {
                Paralysis = 3;
            }
            int Power = 1 + (Paralysis - 3);
            if (Power > 5)
            {
                Power = 5;
            }
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                power = Power
            });
        }
    }
    public class DiceCardAbility_beats_amongus2 : DiceCardAbilityBase
    {
        public static string Desc = "[On Hit] Gain 2 Charge";
        public override void OnSucceedAttack()
        {
            base.OnSucceedAttack();
            this.owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.WarpCharge, 2);
        }
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "WarpCharge"
                };
            }

        }
    }
    public class PassiveAbility_Beats_LightningAspect : PassiveAbilityBase
    {
        public override void OnWaveStart()
        {
            if (this.owner.passiveDetail.HasPassive<PassiveAbility_250123>())
            {
                return;
            }
            this.owner.passiveDetail.AddPassive(new PassiveAbility_250123());
        }
        public class DiceCardAbility_Beats_FragileInflict : DiceCardAbilityBase
        {
            public override void OnSucceedAttack()
            {
                this.card.target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Vulnerable, 1, owner);
            }
        }
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            base.BeforeRollDice(behavior);
            if (this.owner.bufListDetail.GetKewordBufStack(KeywordBuf.WarpCharge) >= 20)
            {
                behavior.AddAbility(new DiceCardAbility_Beats_FragileInflict());
            }
        }
        public override void OnRoundStart()
        {
            if (this.owner.bufListDetail.GetKewordBufStack(KeywordBuf.WarpCharge) >= 4)
            {
                this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Protection, 1, owner);
                this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.BreakProtection, 1, owner);
                if (this.owner.bufListDetail.GetKewordBufStack(KeywordBuf.WarpCharge) >= 8)
                {
                    this.owner.RecoverHP(4);
                    this.owner.breakDetail.RecoverBreak(4);
                    this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.WarpCharge, 2, owner);
                    if (this.owner.bufListDetail.GetKewordBufStack(KeywordBuf.WarpCharge) >= 12)
                    {
                        this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 1, owner);
                        this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Endurance, 1, owner);
                        this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Quickness, 1, owner);
                        if (this.owner.bufListDetail.GetKewordBufStack(KeywordBuf.WarpCharge) >= 16)
                        {
                            this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Protection, 2, owner);
                            this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.BreakProtection, 2, owner);
                        }
                    }
                }
            }
        }
    }
    public class DiceCardAbility_DrawPageOnClashLose00122 : DiceCardAbilityBase
    {
        public static string Desc = "[On Clash Lose] Draw 1 Page.";
        public override void OnLoseParrying()
        {
            this.owner.allyCardDetail.DrawCards(1);
        }
    }
    public class DiceCardSelfAbility_Memories00122 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Play/Discard] Draw 1 page, take 3 Stagger Damage and exhaust this Page.";
        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            unit.breakDetail.TakeBreakDamage(3, DamageType.Card_Ability, unit);
            unit.allyCardDetail.DrawCards(1);
            self.exhaust = true;
        }
        public override void OnDiscard(BattleUnitModel unit, BattleDiceCardModel self)
        {
            unit.breakDetail.TakeBreakDamage(3, DamageType.Card_Ability, unit);
            unit.allyCardDetail.DrawCards(1);
            self.exhaust = true;
        }
        public override void OnEnterCardPhase(BattleUnitModel unit, BattleDiceCardModel self)
        {
            base.OnEnterCardPhase(unit, self);
            unit.allyCardDetail.DrawCards(1);
        }
    }
    public class DiceCardSelfAbility_Reminisce00122 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] Add 1 Memories into this character's deck; draw 1 Page.";
        public override void OnUseCard()
        {
            this.owner.allyCardDetail.DrawCards(1);
            this.owner.allyCardDetail.AddNewCardToDeck(new LorId("TurbulenceOffice", 51)); //Note: I have no idea what this mod's id is so change it, also change the page id to the one of Memories please//
        }
    }
    public class DiceCardAbility_ClashLoseGainMemories00122 : DiceCardAbilityBase
    {
        public static string Desc = "[On Clash Lose] Add 1 'Memories' into this character's deck."; //^//
        public override void OnLoseParrying()
        {
            this.owner.allyCardDetail.AddNewCardToDeck(new LorId("TurbulenceOffice", 51));
        }
    }
    public class DiceCardSelfAbility_Echoes00122 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] Discard all ‘Memories’ in hand add an Offensive die [Pierce, 2-6] to the dice queue for each one discarded. All Offensive dice on this page gain Power for each 'Memories' discarded.";
        public override void OnUseCard()
        {
            int count = 0;
            List<BattleDiceCardModel> list = new List<BattleDiceCardModel>();
            foreach (BattleDiceCardModel card in this.owner.allyCardDetail.GetHand())
            {
                if (card.GetID() == new LorId("TurbulenceOffice", 51)) //^//
                {
                    list.Add(card);
                }
            }
            DiceCardXmlInfo cardItem = ItemXmlDataList.instance.GetCardItem(new LorId("TurbulenceOffice", 52), false); //Here the Card Id needs to be changed to the Pierce 2-6 Page's Id.//
            BattleDiceBehavior battleDiceBehavior = new BattleDiceBehavior();
            foreach (BattleDiceCardModel card2 in list)
            {
                count++;
                battleDiceBehavior.behaviourInCard = cardItem.DiceBehaviourList[0].Copy();
                battleDiceBehavior.SetIndex(3);
                this.card.AddDice(battleDiceBehavior);
            }
            this.card.ApplyDiceStatBonus(DiceMatch.NextAttackDice, new DiceStatBonus
            {
                power = count
            });
            this.owner.allyCardDetail.DiscardACardByAbility(list);
        }
    }
    public class DiceCardSelfAbility_Fading00122 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] Restore 1 Light; if you have no ‘Memories’ in hand, restore 1 additional Light and recover 9 Stagger Resist.";
        public override void OnUseCard()
        {
            Memories = 0;
            this.owner.cardSlotDetail.RecoverPlayPointByCard(1);
            foreach (BattleDiceCardModel card in this.owner.allyCardDetail.GetHand())
            {
                if (card.GetID() == new LorId("TurbulenceOffice", 51)) //^//
                {
                    Memories++;
                }
            }
            if (Memories == 0)
            {
                this.owner.cardSlotDetail.RecoverPlayPointByCard(1);
                this.owner.breakDetail.RecoverBreak(9);
            }
            Memories = 0;
        }
        private int Memories;
    }
    public class DiceCardSelfAbility_Regrets00122 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] Add 2 'Memories' to this characters deck; discard a Page, draw a Page if it was a 'Memories'.";
        public override void OnUseCard()
        {
            this.owner.allyCardDetail.AddNewCardToDeck(new LorId("TurbulenceOffice", 51)); //^//
            this.owner.allyCardDetail.AddNewCardToDeck(new LorId("TurbulenceOffice", 51)); //^//
            List<BattleDiceCardModel> list = new List<BattleDiceCardModel>();
            foreach (BattleDiceCardModel card in this.owner.allyCardDetail.GetHand())
            {
                list.Add(card);
            }
            BattleDiceCardModel cardtodiscard = RandomUtil.SelectOne(list);
            this.owner.allyCardDetail.DiscardACardByAbility(cardtodiscard);
            if (cardtodiscard.GetID() == new LorId("TurbulenceOffice", 51)) //^//
            {
                this.owner.allyCardDetail.DrawCards(1);
            }
        }
    }
    public class DiceCardSelfAbility_SufferInSilence00122 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] A Speed Die becomes unavailable next Scene; discard all ‘Memories’ in hand, restore 1 Light and all dice on this page gain 1 Power for each one discarded; draw 1 Page. [On Discard] Add a ‘Memories’ into this character’s deck.";
        public class BattleUnitBuf_SufferInSilence00122 : BattleUnitBuf
        {
            public BattleUnitBuf_SufferInSilence00122(int stack)
            {
                this.stack = stack;
            }
            public override int SpeedDiceBreakedAdder()
            {
                return stack;
            }
            public override void OnRoundEnd()
            {
                this.Destroy();
            }
        }
        public override void OnUseCard()
        {
            this.owner.bufListDetail.AddReadyBuf(new BattleUnitBuf_SufferInSilence00122(1));
            List<BattleDiceCardModel> list = new List<BattleDiceCardModel>();
            foreach (BattleDiceCardModel card in this.owner.allyCardDetail.GetHand())
            {
                if (card.GetID() == new LorId("TurbulenceOffice", 51)) //^//
                {
                    list.Add(card);
                }
            }
            foreach (BattleDiceCardModel card2 in list)
            {
                this.owner.cardSlotDetail.RecoverPlayPoint(1);
            }
            this.card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = list.Count
            });
            this.owner.allyCardDetail.DiscardACardByAbility(list);
            this.owner.allyCardDetail.DrawCards(1);
        }
        public override void OnDiscard(BattleUnitModel unit, BattleDiceCardModel self)
        {
            unit.allyCardDetail.DrawCards(1);
        }
    }
    public class DiceCardSelfAbility_LetGo00122 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] Discard 2 Pages and restore 1 Light; for every non-‘Memories’ discarded, recover 3 Stagger Resist. [On Discard] Restore 1 Light.";
        public override void OnUseCard()
        {
            List<BattleDiceCardModel> list = new List<BattleDiceCardModel>();
            foreach (BattleDiceCardModel card in this.owner.allyCardDetail.GetHand())
            {
                list.Add(card);
            }
            int Stagger = 0;
            List<BattleDiceCardModel> discardcards = new List<BattleDiceCardModel>();
            for (int i = 0; i < 2; i++)
            {
                discardcards.Add(RandomUtil.SelectOne(list));
            }
            foreach (BattleDiceCardModel pagesforpower in discardcards)
            {
                if (pagesforpower.GetID() != new LorId("TurbulenceOffice", 51)) //^//
                {
                    Stagger++;
                }
            }
            this.owner.allyCardDetail.DiscardACardByAbility(discardcards);
            this.owner.cardSlotDetail.RecoverPlayPoint(1);
            this.owner.breakDetail.RecoverBreak(Stagger * 3);
            Stagger = 0;
        }
        public override void OnDiscard(BattleUnitModel unit, BattleDiceCardModel self)
        {
            unit.cardSlotDetail.RecoverPlayPoint(1);
        }
    }
    public class PassiveAbility_TollofTime : PassiveAbilityBase
    {

        int scenecount;

        public override void OnRoundStart()
        {
            if (scenecount < 12)
            {
                scenecount++;
            }

        }

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                power = scenecount / 3
            });
            base.BeforeRollDice(behavior);
        }
    }
    public class PassiveAbility_PassageofTime : PassiveAbilityBase
    {
        bool oddscene = true;
        public override void OnRoundStart()
        {
            if (oddscene == true)
            {
                oddscene = false;
            }
            else if (oddscene == false)
            {
                oddscene = true;
            }

            if (oddscene == true)
            {
                owner.allyCardDetail.DrawCards(1);
            }
            else if (oddscene == false)
            {
                owner.cardSlotDetail.RecoverPlayPoint(2);
            }
            base.OnRoundStart();

        }
    }
    public class PassiveAbility_TimehealsAll : PassiveAbilityBase
    {
        float check = 999999f;
        BattleUnitModel result;

        List<BattleUnitModel> list = BattleObjectManager.instance.GetAliveList();

        public override void OnRoundEnd()
        {
            List<BattleUnitModel> list = BattleObjectManager.instance.GetAliveList(owner.faction);
            foreach (var unit in list)
            {
                if (unit != null && !unit.IsDead())
                {
                    if (unit.hp < check)
                    {
                        result = unit;
                        check = unit.hp;
                    }
                }
            }
            this.result.RecoverHP(8);
            this.result.breakDetail.RecoverBreak(8);
            base.OnRoundEnd();
        }
    }
    public class DiceCardAbility_sap : DiceCardAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "WarpCharge"
                };
            }
        }
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            BattleUnitBuf_warpCharge sabotage = target.bufListDetail.GetActivatedBuf(KeywordBuf.WarpCharge) as BattleUnitBuf_warpCharge;
            if (sabotage != null && sabotage.stack > 0)
            {
                this.owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.WarpCharge, sabotage.stack, this.owner);
                sabotage.UseStack(sabotage.stack, true);
            }
            base.OnSucceedAttack(target);
        }

        public static string Desc = "[On Hit] Steal all of target's Charge";
    }
    // Accelerated Recovery on use
    public class DiceCardSelfAbility_accrecover : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Energy_Keyword",
                "Recover_Keyword",
                "WarpCharge"
                };
            }
        }
        public override void OnUseCard()
        {
            if (owner.bufListDetail.GetActivatedBuf(KeywordBuf.WarpCharge) is BattleUnitBuf_warpCharge charge && charge.UseStack(4, true))
            {
                owner.cardSlotDetail.RecoverPlayPointByCard(3);
                owner.RecoverHP(8);
            }
            base.OnUseCard();
        }

        public static string Desc = "[On Use] Spend 4 Charge to restore 3 Light and recover 8 HP";
    }
    // 3 Charge on clash win
    public class DiceCardAbility_3chargepw : DiceCardAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "WarpCharge"
                };
            }
        }
        public override void OnWinParrying()
        {
            base.owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.WarpCharge, 3, null);
            base.OnWinParrying();
        }

        public static string Desc = "[On Clash Win] Gain 3 Charge";
    }
    // Time Protocol on use
    public class DiceCardSelfAbility_5charge2quick : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Quickness_Keyword",
                "WarpCharge"
                };
            }
        }
        public override void OnUseCard()
        {
            owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.WarpCharge, 5, base.owner);
            owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Quickness, 2, base.owner);
            base.OnUseCard();
        }

        public static string Desc = "[On Use] Gain 5 Charge and 2 Haste next Scene";
    }
    // Time Protocol first die
    public class DiceCardAbility_1quickatk : DiceCardAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Quickness_Keyword"
                };
            }
        }
        public override void OnSucceedAttack()
        {
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(base.owner.faction))
            {
                battleUnitModel.bufListDetail.AddKeywordBufByCard(KeywordBuf.Quickness, 1, base.owner);
            }
            base.OnSucceedAttack();
        }

        public static string Desc = "[On Hit] Give 1 Haste to all allies next Scene";
    }
    // Deceleration Protocol on use
    public class DiceCardSelfAbility_decel : DiceCardSelfAbilityBase
    {

        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Binding_Keyword"
                };
            }
        }
        public override void OnUseCard()
        {
            base.OnUseCard();
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList_opponent(base.owner.faction))
            {
                battleUnitModel.bufListDetail.AddKeywordBufByCard(KeywordBuf.Binding, 3, base.owner);
            }
        }

        public static string Desc = "[On Use] Inflict 3 Bind to all enemies next Scene";
    }
    // on hit gain 4 charge
    public class DiceCardAbility_4chargeatk : DiceCardAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "WarpCharge"
                };
            }
        }
        public override void OnSucceedAttack()
        {
            base.owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.WarpCharge, 4, null);
            base.OnSucceedAttack();
        }

        public static string Desc = "[On Hit] Gain 4 Charge";
    }
    // accelerated lunge on use
    public class DiceCardSelfAbility_acclunge : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "WarpCharge"
                };
            }
        }
        public override void OnUseCard()
        {
            for (int i = 0; i < 3; i++)
            {
                if (owner.bufListDetail.GetActivatedBuf(KeywordBuf.WarpCharge) is BattleUnitBuf_warpCharge charge && charge.UseStack(2, true))
                {
                    base.card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
                    {
                        max = 2
                    });
                }
            }
            base.OnUseCard();
        }

        public static string Desc = "[On Use] Spend 2 Charge to increase max value of dice on this page by 2, up to three times";
    }
    // Pause on use
    public class DiceCardSelfAbility_pause : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "WarpCharge"
                };
            }
        }
        public override void OnStartBattle()
        {
            if (owner.bufListDetail.GetActivatedBuf(KeywordBuf.WarpCharge) is BattleUnitBuf_warpCharge charge && charge.UseStack(12, true))
            {
                foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList_random((base.owner.faction == Faction.Player) ? Faction.Enemy : Faction.Player, 2))
                {
                    battleUnitModel.bufListDetail.AddKeywordBufByCard(KeywordBuf.Stun, 1, base.owner);
                }
            }
            base.OnStartBattle();
        }

        public static string Desc = "[On Combat Start] Spend 12 Charge to Immobilize two random enemies next scene";
    }
    // Momentum on use
    public class DiceCardSelfAbility_momentum : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "WarpCharge"
                };
            }
        }
        public override void OnUseCard()
        {
            base.OnUseCard();
            if (owner.bufListDetail.GetActivatedBuf(KeywordBuf.WarpCharge) is BattleUnitBuf_warpCharge charge && charge.UseStack(8, true))
            {
                this.card.ApplyDiceAbility(DiceMatch.AllDice, new DiceCardAbility_smoemtnusdja());
            }
        }


        public static string Desc = "[On Use] Spend 8 Charge to give all dice on this page [On Hit] Restore 2 Light; Draw 2 Pages; Gain 5 Charge";
    }
    public class DiceCardAbility_smoemtnusdja : DiceCardAbilityBase
    {
        public override void OnSucceedAttack()
        {
            base.OnSucceedAttack();
            owner.cardSlotDetail.RecoverPlayPointByCard(2);
            owner.allyCardDetail.DrawCards(2);
            owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.WarpCharge, 5, this.owner);
        }
    }
    // Accelerated Mending on use
    public class DiceCardSelfAbility_accmending : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Recover_Keyword",
                "WarpCharge"
                };
            }
        }
        public override void OnStartBattle()
        {
            if (owner.bufListDetail.GetActivatedBuf(KeywordBuf.WarpCharge) is BattleUnitBuf_warpCharge charge && charge.UseStack(14, true))
            {
                List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(base.owner.faction);
                aliveList.Remove(base.owner);
                if (aliveList.Count > 0)
                {
                    BattleUnitModel battleUnitModel = RandomUtil.SelectOne<BattleUnitModel>(aliveList);
                    battleUnitModel.RecoverHP(28);
                    battleUnitModel.breakDetail.RecoverBreak(28);
                }
                owner.RecoverHP(28);
                owner.breakDetail.RecoverBreak(28);
            }
            base.OnStartBattle();
        }

        public static string Desc = "[On Combat Start] Spend 14 Charge to recover 28 HP and Stagger Resist of self and another random ally";
    }
    public class DiceCardSelfAbility_MandatoryUnity : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                    "Energy_Keyword",
                    "DrawCard_Keyword"
                };
            }
        }
        public override bool IsUniteCard
        {
            get
            {
                return true;
            }
        }
        public override void OnUseCard()
        {
            base.owner.cardSlotDetail.RecoverPlayPointByCard(2);
            base.owner.allyCardDetail.DrawCards(1);
        }
        public override void OnStartBattle()
        {
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(base.owner.faction))
            {
                if (battleUnitModel != base.owner && !battleUnitModel.bufListDetail.HasBuf<DiceCardSelfAbility_MandatoryUnity.BattleUnitBuf_MandatoryUnity>())
                {
                    battleUnitModel.bufListDetail.AddBuf(new DiceCardSelfAbility_MandatoryUnity.BattleUnitBuf_MandatoryUnity());
                }
            }
        }

        public class BattleUnitBuf_MandatoryUnity : BattleUnitBuf
        {
            public override void OnUseCard(BattlePlayingCardDataInUnitModel card)
            {
                if (card.cardAbility != null && card.cardAbility.IsUniteCard)
                {
                    card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
                    {
                        power = 2
                    });
                }
            }
        }
        public static string Desc = "[Unity]\n[Combat Start] For this Scene, all other allies using a Unity page gain +2 Power for all dice (Cannot be stacked); [On Use] Restore 2 Light and Draw 1 Page.";
    }
    public class DiceCardPriority_chaoticswarm : DiceCardPriorityBase
    {
        private int count;
        public override int GetPriorityBonus(BattleUnitModel owner)
        {
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(owner.faction))
            {
                BattleAllyCardDetail allyCardDetail = battleUnitModel.allyCardDetail;
                List<BattleDiceCardModel> list = ((allyCardDetail != null) ? allyCardDetail.GetHand() : null);
                List<BattleDiceCardModel> list2 = ((allyCardDetail != null) ? allyCardDetail.GetUse() : null);
                if ((list != null && list.Exists((BattleDiceCardModel x) => x.GetID() == new LorId("TurbulenceOffice", 31))) || (list2 != null && list2.Exists((BattleDiceCardModel x) => x.GetID() == new LorId("TurbulenceOffice", 31))))
                {
                    count++;
                }
            }
            if (count >= 3)
            {
                return 30;
            }
            return 0;
        }
    }
    public class PassiveAbility_clownmarker : PassiveAbilityBase
    {
        public override void OnRoundStart()
        {
            if (this.owner.faction == Faction.Enemy)
            {
                List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(base.owner.faction);
                aliveList.Remove(base.owner);
                if (aliveList.Count > 0)
                {
                    BattleUnitModel battleUnitModel = RandomUtil.SelectOne<BattleUnitModel>(aliveList);
                    battleUnitModel.bufListDetail.AddBuf(new BattleUnitBuf_clownbuff());
                }
            }
        }
        public override void OnWaveStart()
        {
            if (this.owner.faction == Faction.Player)
            {
                this.owner.personalEgoDetail.AddCard(new LorId(this.packageId, 69));
                this.owner.personalEgoDetail.AddCard(new LorId(this.packageId, 610));
            }
            if (this.owner.faction == Faction.Enemy)
            {
                foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList_random((base.owner.faction == Faction.Enemy) ? Faction.Player : Faction.Enemy, 1))
                {
                    battleUnitModel.bufListDetail.AddBuf(new BattleUnitBuf_clownedmark());
                }
            }
        }
        //simply change the card ids to whatever they will be in the finished project so free
        private string packageId = "TurbulenceOffice";
    }
    public class DiceCardSelfAbility_beats_ultraunique : DiceCardSelfAbilityBase
    {
        public static string Desc = "Usable on and after the third Scene. Can only be used at Emotion Level 3 and above. If target's HP is at 50% or lower, deal twice as much damage.";
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (this.card.target != null && this.card.target.hp <= (float)this.card.target.MaxHp * 0.5f)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    dmgRate = 100
                });
            }
        }
        public override bool OnChooseCard(BattleUnitModel owner)
        {
            return Singleton<StageController>.Instance.RoundTurn >= 3 && owner.emotionDetail.EmotionLevel >= 3 && base.OnChooseCard(owner);
        }
    }
    public class DiceCardSelfAbility_clownmark : DiceCardSelfAbilityBase
    {
        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            if (targetUnit != null)
            {
                foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(targetUnit.faction))
                {
                    BattleUnitBuf_clownedmark activatedbuf = battleUnitModel.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_clownedmark) as BattleUnitBuf_clownedmark;
                    if (activatedbuf != null && battleUnitModel != targetUnit)
                    {
                        activatedbuf.Destroy();
                    }
                }
                BattleUnitBuf_clownedmark activatedbuf2 = targetUnit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_clownedmark) as BattleUnitBuf_clownedmark;
                if (activatedbuf2 == null)
                {
                    targetUnit.bufListDetail.AddBuf(new BattleUnitBuf_clownedmark());
                }
            }
        }
        public static string Desc = "[On Play; Target Enemy] Inflict Marked for Death (Only one target can be marked at a time)";
    }
    public class DiceCardSelfAbility_clownbuffer : DiceCardSelfAbilityBase
    {
        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            if (targetUnit != null)
            {
                foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(targetUnit.faction))
                {
                    BattleUnitBuf_clownbuff activatedbuf = battleUnitModel.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_clownbuff) as BattleUnitBuf_clownbuff;
                    if (activatedbuf != null && battleUnitModel != targetUnit)
                    {
                        activatedbuf.Destroy();
                    }
                }
                BattleUnitBuf_clownbuff activatedbuf2 = targetUnit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_clownbuff) as BattleUnitBuf_clownbuff;
                if (activatedbuf2 == null)
                {
                    targetUnit.bufListDetail.AddBuf(new BattleUnitBuf_clownbuff());
                }
            }
        }
        public override bool IsValidTarget(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            return targetUnit != null && targetUnit.faction == unit.faction;
        }
        public override bool IsOnlyAllyUnit()
        {
            return true;
        }
        public static string Desc = "[On Play; Target Ally] Give Offensive Command (Only one ally can have this status at a time)";
    }
    public class BattleUnitBuf_clownedmark : BattleUnitBuf
    {
        protected override string keywordIconId
        {
            get
            {
                return "clownnikolai";
            }
        }
        protected override string keywordId
        {
            get
            {
                return "clownnikolai";
            }
        }
        public override int GetDamageIncreaseRate()
        {
            return 50;
        }

        public override int GetBreakDamageIncreaseRate()
        {
            return 50;
        }
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            base.BeforeRollDice(behavior);
            BattleUnitBuf activatedBuf = this._owner.bufListDetail.GetActivatedBuf(KeywordBuf.Paralysis);
            if (activatedBuf != null && activatedBuf.stack >= 3)
            {
                int stack = activatedBuf.stack - 3;
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    min = -stack
                });
            }
        }
    }
    public class BattleUnitBuf_clownbuff : BattleUnitBuf
    {
        protected override string keywordIconId
        {
            get
            {
                return "clownmaxim";
            }
        }
        protected override string keywordId
        {
            get
            {
                return "clownmaxim";
            }
        }
        public override void BeforeGiveDamage(BattleDiceBehavior behavior)
        {
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                dmgRate = 50,
                breakRate = 50
            });
        }
        public override void OnRoundEnd()
        {
            if (this._owner.faction == Faction.Enemy)
            {
                this.Destroy();
            }
        }
        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            base.OnUseCard(curCard);
            if (curCard == null)
            {
                return;
            }
            curCard.ApplyDiceAbility(DiceMatch.NextAttackDice, new DiceCardAbility_paralysis1atk());
        }
    }
    public class SkinChanger
    {

    }
    public class BattleDiceCardBuf_Stolen : BattleDiceCardBuf
    {
        protected override string keywordId
        {
            get
            {
                return "stolenpage";
            }
        }
        public static void AddBuf(BattleDiceCardModel card, BattleUnitModel thief, BattleUnitModel victim)
        {
            BattleDiceCardBuf_Stolen buff = card.GetBufList().Find((BattleDiceCardBuf x) => x is BattleDiceCardBuf_Stolen) as BattleDiceCardBuf_Stolen;
            bool present = buff != null;

            bool HasDistribute = thief.passiveDetail.PassiveList.Exists((PassiveAbilityBase x) => x is PassiveAbility_DistributePage);
            bool WillExhaust = thief.passiveDetail.PassiveList.Exists((PassiveAbilityBase x) => x is PassiveAbility_ExhaustPage);

            if (!present)
            {
                buff = new BattleDiceCardBuf_Stolen
                {
                    Distribute = HasDistribute,
                    Exhausting = WillExhaust,
                    rightfulowner = victim,
                    Scenesinhand = 0
                };
                card.AddBuf(buff);
                card.AddCost(-2);
            }
        }
        public BattleUnitModel rightfulowner;
        public bool Distribute;
        public bool Exhausting;
        public int Scenesinhand;

        public override void OnUseCard(BattleUnitModel owner, BattlePlayingCardDataInUnitModel playingCard)
        {

            playingCard.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus { power = -2 });
            if (Distribute)
            {
                if (playingCard.target == rightfulowner)
                {
                    playingCard.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus { power = 2 });
                }
                LorId id = playingCard.card.GetID();
                playingCard.target.allyCardDetail.AddNewCard(id);
                playingCard.card.ReserveExhaust();
            }
            if (Scenesinhand == 2 && Exhausting)
            {
                playingCard.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus { power = 2 });
            }
        }
        public override void OnRoundStart()
        {
            Scenesinhand++;
            this._stack++;
            if (Scenesinhand > 2 && Exhausting)
            {
                this._card.ReserveExhaust();
            }
        }
    }
    public class BattleUnitBuf_ClothesStolen : BattleUnitBuf
    {
        private bool transformed;
        public static void AddBuf(BattleUnitModel target)
        {
            BattleUnitBuf_ClothesStolen buff = target.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_ClothesStolen) as BattleUnitBuf_ClothesStolen;
            bool present = buff != null;
            if (present)
            {

            }
            else
            {
                buff = new BattleUnitBuf_ClothesStolen
                {
                    transformed = false
                };
                target.bufListDetail.AddBuf(buff);
            }
        }
        public override void OnRoundStartAfter()
        {
            if (transformed == false)
            {
                transformed = true;

                SephirahType floor = Singleton<StageController>.Instance.CurrentFloor;
                bool Sephirah = _owner.UnitData.unitData.isSephirah;

                if (!Sephirah)
                {
                    switch (floor)
                    {
                        case SephirahType.Malkuth:
                            _owner.view.ChangeSkin("MalkuthLibrarian");
                            break;
                        case SephirahType.Yesod:
                            _owner.view.ChangeSkin("YesodLibrarian");
                            break;
                        case SephirahType.Hod:
                            _owner.view.ChangeSkin("HodLibrarian");
                            break;
                        case SephirahType.Netzach:
                            _owner.view.ChangeSkin("NetzachLibrarian");
                            break;
                        case SephirahType.Tiphereth:
                            _owner.view.ChangeSkin("TipherethLibrarian");
                            break;
                        case SephirahType.Gebura:
                            _owner.view.ChangeSkin("GeburaLibrarian");
                            break;
                        case SephirahType.Chesed:
                            _owner.view.ChangeSkin("ChesedLibrarian");
                            break;
                        case SephirahType.Binah:
                            _owner.view.ChangeSkin("BinahLibrarian");
                            break;
                        case SephirahType.Hokma:
                            _owner.view.ChangeSkin("HokmaLibrarian");
                            break;
                        case SephirahType.Keter:
                            _owner.view.ChangeSkin("KetherLibrarian");
                            break;
                        default:
                            _owner.view.ChangeSkin("KetherLibrarian");
                            break;

                    }
                }
                else
                {
                    switch (floor)
                    {
                        case SephirahType.Malkuth:
                            _owner.view.ChangeSkin("Malkuth");
                            break;
                        case SephirahType.Yesod:
                            _owner.view.ChangeSkin("Yesod");
                            break;
                        case SephirahType.Hod:
                            _owner.view.ChangeSkin("Hod");
                            break;
                        case SephirahType.Netzach:
                            _owner.view.ChangeSkin("Netzach");
                            break;
                        case SephirahType.Tiphereth:
                            _owner.view.ChangeSkin("Tiphereth");
                            break;
                        case SephirahType.Gebura:
                            _owner.view.ChangeSkin("Gebura");
                            break;
                        case SephirahType.Chesed:
                            _owner.view.ChangeSkin("Chesed");
                            break;
                        case SephirahType.Binah:
                            _owner.view.ChangeSkin("Binah");
                            break;
                        case SephirahType.Hokma:
                            _owner.view.ChangeSkin("Hokma");
                            break;
                        case SephirahType.Keter:
                            _owner.view.ChangeSkin("Roland");
                            break;
                        default:
                            _owner.view.ChangeSkin("KetherLibrarian");
                            break;
                    }
                }
                SingletonBehavior<BattleManagerUI>.Instance.ui_unitListInfoSummary.UpdateCharacterProfile(_owner, _owner.faction, _owner.hp, _owner.breakDetail.breakGauge, _owner.bufListDetail.GetBufUIDataList());
                int num2 = 0;
                foreach (BattleUnitModel battleUnitModel2 in BattleObjectManager.instance.GetList())
                {
                    SingletonBehavior<UICharacterRenderer>.Instance.SetCharacter(battleUnitModel2.UnitData.unitData, num2++, true, false);
                }
                BattleObjectManager.instance.InitUI();
                ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("FartReverb.wav"), _owner.view.transform, 2f);
            }
        }
        public override AtkResist GetResistBP(AtkResist origin, BehaviourDetail detail)
        {
            switch (detail)
            {
                case BehaviourDetail.Hit:
                    return AtkResist.Weak;
                case BehaviourDetail.Slash:
                    return AtkResist.Normal;
                case BehaviourDetail.Penetrate:
                    return AtkResist.Vulnerable;
                default:
                    return AtkResist.None;
            }
        }
        public override AtkResist GetResistHP(AtkResist origin, BehaviourDetail detail)
        {
            switch (detail)
            {
                case BehaviourDetail.Hit:
                    return AtkResist.Weak;
                case BehaviourDetail.Slash:
                    return AtkResist.Normal;
                case BehaviourDetail.Penetrate:
                    return AtkResist.Vulnerable;
                default:
                    return AtkResist.None;
            }
        }

    }
    public class PassiveAbility_StealPage : PassiveAbilityBase
    {
        public override void OnWaveStart()
        {
            this.listPAGE = new List<LorId>();
            if (owner.faction == Faction.Enemy)
            {
                int rnd = RandomUtil.Range(1, 2);
                if (rnd == 1)
                {
                    ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("LinetteEnter1.wav"), owner.view.transform, 2f);
                    owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "SPECIAL_EVENT_0");
                }
                else
                {
                    ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("LinetteEnter2.wav"), owner.view.transform, 2f);
                    owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "SPECIAL_EVENT_1");
                }
            }
        }

        public override void OnSucceedAttack(BattleDiceBehavior behavior)
        {
            if (this.listPAGE.Count < 3)
            {
                BattleUnitModel target = behavior.card.target;
                BattleDiceCardModel loot = target.allyCardDetail.GetRandomCardInHand();
                LorId Id = loot.GetID();
                //Remove Page from target
                target.allyCardDetail.ExhaustACard(loot);
                //Add Page to hand
                BattleDiceCardModel newcard = this.owner.allyCardDetail.AddNewCard(Id);
                BattleDiceCardBuf_Stolen.AddBuf(newcard, owner, target);
                this.listPAGE.Add(Id);
            }


        }
        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            LorId cardId = curCard.card.GetID();
            if (this.listPAGE.Contains(cardId))
            {
                this.listPAGE.Remove(cardId);
            }
        }
        private List<LorId> listPAGE;

        //Appearance Change
        public override void OnDie()
        {
            if (owner.faction == Faction.Enemy)
            {

                ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("LinetteDeath.wav"), owner.view.transform, 2f);
                owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "SPECIAL_EVENT_3");
            }
        }
        public override void OnKill(BattleUnitModel target)
        {
            if (owner.faction == Faction.Enemy)
            {
                int random = RandomUtil.Range(0, 2);
                switch (random)
                {
                    case 0:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("LinetteKill1.wav"), owner.view.transform, 2f);
                        owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "SPECIAL_EVENT_6");
                        break;
                    case 1:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("LinetteKill2.wav"), owner.view.transform, 2f);
                        owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "SPECIAL_EVENT_7");
                        break;
                    default:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("LinetteKill3.wav"), owner.view.transform, 2f);
                        owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "SPECIAL_EVENT_8");
                        break;
                }
            }
        }
        public override void OnDieOtherUnit(BattleUnitModel unit)
        {
            if (unit.faction == Faction.Enemy)
            {
                int random = RandomUtil.Range(0, 1);
                if (random == 0)
                {
                    ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("LinetteAllyDeath1.wav"), owner.view.transform, 2f);
                    owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "SPECIAL_EVENT_4");
                }
                else
                {
                    ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("LinetteAllyDeath2.wav"), owner.view.transform, 2f);
                    owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "SPECIAL_EVENT_5");
                }
            }
        }
    }

    public class PassiveAbility_DistributePage : PassiveAbilityBase
    {

    }
    public class PassiveAbility_ExhaustPage : PassiveAbilityBase
    {

    }
    // Plagiarism Single Use, On Use, Copy a random passive of a target that isn't speed.
    public class DiceCardSelfAbility_Plagiarism : DiceCardSelfAbilityBase
    {
        private int count;
        public static string Desc = "[Combat Start] Copy all of the target's passives and permanently steal the target's most expensive page";
        public override void OnStartBattle()
        {
            this.passiveId = new List<LorId>();
            this.innerType = new List<int>();
            this.innerType.Add(1);

            foreach (PassiveAbilityBase passiveAbilityBase in this.owner.passiveDetail.PassiveList)
            {
                count++;
                this.passiveId.Add(passiveAbilityBase.id);
                if (passiveAbilityBase.InnerTypeId != -1)
                {
                    this.innerType.Add(passiveAbilityBase.InnerTypeId);
                }
            }
            using (List<PassiveAbilityBase>.Enumerator enumerator = this.card.target.passiveDetail.PassiveList.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    PassiveAbilityBase passive = enumerator.Current;
                    PassiveXmlInfo passiveXmlInfo = new PassiveXmlInfo();
                    passiveXmlInfo = Singleton<PassiveXmlList>.Instance.GetData(passive.id);
                    if (passiveXmlInfo != null && passiveXmlInfo.CanGivePassive && !this.passiveId.Exists((LorId x) => x == passive.id) && !this.innerType.Exists((int x) => x == passive.InnerTypeId))
                    {
                        var plagiarisedpassive = owner.passiveDetail.AddPassive(passive.id);
                        count++;
                        this.passiveId.Add(passive.id);
                        string name = PassiveDescXmlList.Instance.GetName(passive.id);
                        if (!string.IsNullOrEmpty(name))
                            plagiarisedpassive.name = name;
                        string desc = PassiveDescXmlList.Instance.GetDesc(passive.id);
                        if (!string.IsNullOrEmpty(desc))
                            plagiarisedpassive.desc = desc;
                        if (passive.InnerTypeId != -1)
                        {
                            this.innerType.Add(passive.InnerTypeId);
                        }
                        if (count >= 12)
                        {
                            plagiarisedpassive.Hide();
                        }
                    }
                }
            }
            ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("LinetteCopy.wav"), owner.view.transform, 2f);
            owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "SPECIAL_EVENT_9");

            BattleUnitModel target = base.card.target;
            List<BattleDiceCardModel> hand = target.allyCardDetail.GetAllDeck();
            if (hand.Count == 0)
            {
                return;
            }
            hand.Sort((BattleDiceCardModel x, BattleDiceCardModel y) => y.GetOriginCost() - x.GetOriginCost());
            int targetCost = hand[0].GetOriginCost();
            List<BattleDiceCardModel> list = hand.FindAll((BattleDiceCardModel x) => x.GetOriginCost() == targetCost);
            if (list.Count <= 0)
            {
                return;
            }
            BattleDiceCardModel battleDiceCardModel = RandomUtil.SelectOne<BattleDiceCardModel>(list);
            target.allyCardDetail.ExhaustACardAnywhere(battleDiceCardModel);
            BattleDiceCardModel battleDiceCardModel2 = base.owner.allyCardDetail.AddNewCard(battleDiceCardModel.GetID(), false);
            battleDiceCardModel2.AddCost(-2);
            this.card.card.ReserveExhaust();
        }

        private List<LorId> passiveId;
        private List<int> innerType;
    }
    public class DiceCardSelfAbility_Plagiarism2 : DiceCardSelfAbilityBase
    {
        private int count;
        public static string Desc = "[Combat Start] Copy all of the target's passives and permanently steal the target's most expensive page\n[Start of Clash] Reduce Power of all target's dice by 5";
        public override void OnStartBattle()
        {
            this.passiveId = new List<LorId>();
            this.innerType = new List<int>();
            this.innerType.Add(1);

            foreach (PassiveAbilityBase passiveAbilityBase in this.owner.passiveDetail.PassiveList)
            {
                count++;
                this.passiveId.Add(passiveAbilityBase.id);
                if (passiveAbilityBase.InnerTypeId != -1)
                {
                    this.innerType.Add(passiveAbilityBase.InnerTypeId);
                }
            }
            using (List<PassiveAbilityBase>.Enumerator enumerator = this.card.target.passiveDetail.PassiveList.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    PassiveAbilityBase passive = enumerator.Current;
                    PassiveXmlInfo passiveXmlInfo = new PassiveXmlInfo();
                    passiveXmlInfo = Singleton<PassiveXmlList>.Instance.GetData(passive.id);
                    if (passiveXmlInfo != null && passiveXmlInfo.CanGivePassive && !this.passiveId.Exists((LorId x) => x == passive.id) && !this.innerType.Exists((int x) => x == passive.InnerTypeId))
                    {
                        var plagiarisedpassive = owner.passiveDetail.AddPassive(passive.id);
                        count++;
                        this.passiveId.Add(passive.id);
                        string name = PassiveDescXmlList.Instance.GetName(passive.id);
                        if (!string.IsNullOrEmpty(name))
                            plagiarisedpassive.name = name;
                        string desc = PassiveDescXmlList.Instance.GetDesc(passive.id);
                        if (!string.IsNullOrEmpty(desc))
                            plagiarisedpassive.desc = desc;
                        if (passive.InnerTypeId != -1)
                        {
                            this.innerType.Add(passive.InnerTypeId);
                        }
                        if (count >= 12)
                        {
                            plagiarisedpassive.Hide();
                        }
                    }
                }
            }
            BattleUnitModel target = base.card.target;
            List<BattleDiceCardModel> hand = target.allyCardDetail.GetAllDeck();
            if (hand.Count == 0)
            {
                return;
            }
            hand.Sort((BattleDiceCardModel x, BattleDiceCardModel y) => y.GetOriginCost() - x.GetOriginCost());
            int targetCost = hand[0].GetOriginCost();
            List<BattleDiceCardModel> list = hand.FindAll((BattleDiceCardModel x) => x.GetOriginCost() == targetCost);
            if (list.Count <= 0)
            {
                return;
            }
            BattleDiceCardModel battleDiceCardModel = RandomUtil.SelectOne<BattleDiceCardModel>(list);
            target.allyCardDetail.ExhaustACardAnywhere(battleDiceCardModel);
            BattleDiceCardModel battleDiceCardModel2 = base.owner.allyCardDetail.AddNewCard(battleDiceCardModel.GetID(), false);
            battleDiceCardModel2.AddCost(-2);
            this.card.card.ReserveExhaust();
        }
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
                power = -5
            });
        }

        private List<LorId> passiveId;
        private List<int> innerType;
    }
    public class DiceCardAbility_sigharthealer : DiceCardAbilityBase
    {
        public override void OnWinParrying()
        {
            base.OnWinParrying();
            foreach (BattleUnitModel among in BattleObjectManager.instance.GetAliveList(this.owner.faction))
            {
                among.RecoverHP(behavior.DiceResultValue * 3);
            }
        }
        public override void OnLoseParrying()
        {
            base.OnLoseParrying();
            foreach (BattleUnitModel among in BattleObjectManager.instance.GetAliveList_opponent(this.owner.faction))
            {
                among.RecoverHP(behavior.DiceResultValue * 3);
            }
        }
        public static string Desc = "[On Clash Win] All allies recover HP equal to 3x the roll's value\n[On Clash Lose] All enemies recover HP equal to 3x the roll's value";
    }

    // I am You! - On Use, Copy the most expensive combat page of the enemy. The copied page's cost is reduced by 2 and gains the ability
    public class DiceCardSelfAbility_YOURENOTME : DiceCardSelfAbilityBase
    {
        public static string Desc = "[Combat Start] Copy target's appearance; Target's resistances and appearance becomes of a regular librarian";
        public override void OnStartBattle()
        {
            BattleUnitBuf_ClothesStolen.AddBuf(base.card.target);
            ChangeSkin(base.card.target.UnitData.unitData.CustomBookItem, owner);
            UnitCustomizingData data2 = base.card.target.UnitData.unitData.customizeData;
            if (!(data2.browID == -1 && data2.eyeID == -1 && data2.frontHairID == -1 && ((data2.hairColor == Color.black && data2.eyeColor == Color.black && data2.skinColor == Color.black) || (data2.hairColor == Color.white && data2.eyeColor == Color.white && data2.skinColor == Color.white))))
            {
                owner.UnitData.unitData.customizeData.Copy(data2);
                owner.view.ChangeHeight(data2.height);
                owner.UnitData.unitData.customizeData.headID = data2.headID;
                this.owner.UnitData.unitData.customizeData.SetCustomData(true);

            }
            int num2 = 0;
            foreach (BattleUnitModel battleUnitModel2 in BattleObjectManager.instance.GetList())
            {
                SingletonBehavior<UICharacterRenderer>.Instance.SetCharacter(battleUnitModel2.UnitData.unitData, num2++, true, false);
            }
            SingletonBehavior<BattleManagerUI>.Instance.ui_unitListInfoSummary.UpdateCharacterProfileAll();
            BattleObjectManager.instance.InitUI();
        }
        public void ChangeSkin(BookModel Bookmodel, BattleUnitModel owner)
        {
            string charName = Bookmodel.GetCharacterName();
            BookModel original = owner.UnitData.unitData.CustomBookItem;
            bool ismodded = Bookmodel.IsWorkshop;
            if (!ismodded)
            {
                owner.view.ChangeSkin(charName);
            }
            else
            {
                string packagename = Bookmodel.BookId.packageId;
                BattleUnitView.SkinInfo skinInfo = typeof(BattleUnitView).GetField("_skinInfo", AccessTools.all).GetValue(owner.view) as BattleUnitView.SkinInfo;
                skinInfo.state = BattleUnitView.SkinState.Changed;
                skinInfo.skinName = charName;
                WorkshopSkinData workshopBookSkinData = Singleton<CustomizingBookSkinLoader>.Instance.GetWorkshopBookSkinData(packagename, charName);
                ActionDetail currentMotionDetail = owner.view.charAppearance.GetCurrentMotionDetail();
                string resourceName;

                if (workshopBookSkinData != null)
                {
                    GameObject gameObject = Singleton<AssetBundleManagerRemake>.Instance.LoadCharacterPrefab(charName, "", out resourceName);
                    if (gameObject != null)
                    {
                        owner.view.DestroySkin();
                        UnitCustomizingData customizeData = owner.UnitData.unitData.customizeData;
                        GiftInventory giftInventory = owner.UnitData.unitData.giftInventory;
                        GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject, owner.view.characterRotationCenter);
                        gameObject2.GetComponent<WorkshopSkinDataSetter>().SetData(workshopBookSkinData);
                        owner.view.charAppearance = gameObject2.GetComponent<CharacterAppearance>();
                        owner.view.charAppearance.Initialize(resourceName);
                        owner.view.charAppearance.InitCustomData(customizeData, owner.UnitData.unitData.defaultBook.GetBookClassInfoId());
                        owner.view.charAppearance.InitGiftDataAll(giftInventory.GetEquippedList());
                        owner.view.charAppearance.ChangeMotion(currentMotionDetail);
                        owner.view.charAppearance.ChangeLayer("Character");
                        owner.view.charAppearance.SetLibrarianOnlySprites(owner.faction);
                    }
                }
                else
                {
                    BookXmlInfo info = Bookmodel.ClassInfo;
                    charName = info.GetCharacterSkin();
                    owner.view.ChangeSkin(charName);
                }
            }



        }
    }

    public class DiceCardSelfAbility_Shuffleitup : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Hit] Shuffle combat pages with other hit targets";
        public override void OnStartBattle()
        {
            victims = new List<BattleUnitModel>();
            cards = new List<LorId>();
        }
        //Get cards
        public override void OnSucceedAreaAttack(BattleUnitModel target)
        {
            victims.Add(target);
            foreach (BattleDiceCardModel page in target.allyCardDetail.GetAllDeck())
            {
                cards.Add(page.GetID());
            }
            target.allyCardDetail.ExhaustAllCards();

        }
        //Distribute cards
        public override void OnEndAreaAttack()
        {
            using (List<BattleUnitModel>.Enumerator enumerator = victims.GetEnumerator())
            {
                while (enumerator.MoveNext() && cards.Count > 0)
                {
                    BattleUnitModel victim = enumerator.Current;
                    AddCardFromPool(victim);
                    victim.allyCardDetail.DrawCards(5);
                }
            }
            ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("LinetteUlt.wav"), owner.view.transform, 2f);
            owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "SPECIAL_EVENT_10");

        }
        private void AddCardFromPool(BattleUnitModel target)
        {
            int count = 9;
            while (count > 0 && cards.Count > 0)
            {
                LorId cardid = RandomUtil.SelectOne<LorId>(cards);
                target.allyCardDetail.AddNewCardToDeck(cardid);
                cards.Remove(cardid);
                count--;
            }
        }
        private List<BattleUnitModel> victims;
        private List<LorId> cards;
    }
    public class DiceCardAbility_NoDamage : DiceCardAbilityBase
    {
        public static string Desc = "This die deals no damage or stagger damage";
        public override void BeforeRollDice()
        {
            base.behavior.ApplyDiceStatBonus(new DiceStatBonus { dmg = -100, breakDmg = -100 });
        }
    }
    public class DiceCardPriority_coughingfit : DiceCardPriorityBase
    {
        public override int GetPriorityBonus(BattleUnitModel owner)
        {
            if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.Smoke) >= 9)
            {
                return -1;
            }
            if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.Smoke) <= 1)
            {
                return 25;
            }
            return 0;
        }
    }
    public class DiceCardPriority_recoverypriority : DiceCardPriorityBase
    {
        public override int GetPriorityBonus(BattleUnitModel owner)
        {
            if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.WarpCharge) >= 4)
            {
                return 2;
            }
            if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.WarpCharge) <= 3)
            {
                return -2;
            }
            return 0;
        }
    }
    public class DiceCardPriority_decelpriority : DiceCardPriorityBase
    {
        public override int GetPriorityBonus(BattleUnitModel owner)
        {
            if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.WarpCharge) >= 6)
            {
                return 4;
            }
            return 0;
        }
    }
    public class DiceCardPriority_accelpriority : DiceCardPriorityBase
    {
        public override int GetPriorityBonus(BattleUnitModel owner)
        {
            if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.WarpCharge) >= 2)
            {
                return 1;
            }
            if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.WarpCharge) >= 4)
            {
                return 2;
            }
            if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.WarpCharge) >= 6)
            {
                return 3;
            }
            return 0;
        }
    }
    public class DiceCardPriority_pausepriority : DiceCardPriorityBase
    {
        public override int GetPriorityBonus(BattleUnitModel owner)
        {
            List<BattleDiceCardModel> list2 = ((owner.allyCardDetail != null) ? owner.allyCardDetail.GetUse() : null);
            if ((list2 != null && list2.Exists((BattleDiceCardModel x) => x.GetID() == new LorId("TurbulenceOffice", 118))))
            {
                return -100;
            }
            if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.WarpCharge) >= 12)
            {
                return 12;
            }
            if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.WarpCharge) <= 11)
            {
                return -100;
            }
            return 0;
        }
    }
    public class DiceCardPriority_momentumpriority : DiceCardPriorityBase
    {
        public override int GetPriorityBonus(BattleUnitModel owner)
        {
            if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.WarpCharge) >= 8)
            {
                return 11;
            }
            if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.WarpCharge) <= 7)
            {
                return -2;
            }
            return 0;
        }
    }
    public class DiceCardPriority_mendingpriority : DiceCardPriorityBase
    {
        public override int GetPriorityBonus(BattleUnitModel owner)
        {
            if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.WarpCharge) >= 14)
            {
                return 30;
            }
            if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.WarpCharge) <= 13)
            {
                return -150;
            }
            return 0;
        }
    }
    //Lightning Hands
    public class PassiveAbility_SC_LightningHands : PassiveAbilityBase
    {
        public override void OnRoundStart()
        {
            isFirstUseCard = false;
            trigger = false;
        }
        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.card.GetSpec().Ranged == CardRange.Far
                && curCard.card.GetSpec().affection == CardAffection.One
                && !isFirstUseCard)
            {
                trigger = true;
                isFirstUseCard = true;
            }
        }
        public override void OnEndBattle(BattlePlayingCardDataInUnitModel curCard)
        {
            if (trigger)
            {
                List<BattleUnitModel> aliveList_opponent = BattleObjectManager.instance.GetAliveList_opponent(owner.faction);
                if (aliveList_opponent.Count > 0)
                {
                    BattleUnitModel battleUnitModel = RandomUtil.SelectOne(aliveList_opponent);
                    Singleton<StageController>.Instance.AddAllCardListInBattle(curCard, battleUnitModel, -1);
                    trigger = false;
                }
            }
        }
        private bool trigger;
        private bool isFirstUseCard;
    }
    //Anti-Blue Ammunition
    public class PassiveAbility_SC_AntiBlueAmmo : PassiveAbilityBase
    {
        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.card.GetSpec().Ranged == CardRange.Far)
            {
                curCard.cardBehaviorQueue.ToList().ForEach(delegate (BattleDiceBehavior x)
                {
                    x.AddAbility(new DiceCardAbility_blueAmmoDice());
                });
            }
        }
        public class DiceCardAbility_blueAmmoDice : DiceCardAbilityBase
        {
            public override void BeforeGiveDamage()
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    dmg = -9999,
                    breakDmg = -9999
                });
            }
            public override void OnSucceedAttack()
            {
                BattleUnitModel target = behavior.card.target;
                target.LoseHp(behavior.DiceResultValue);
                target.breakDetail.LoseBreakGauge(behavior.DiceResultValue / 2);
                if (behavior.DiceResultValue > target.hp)
                {
                    target.SetHp(0);
                    target.TakeDamage(0, DamageType.Attack, null, KeywordBuf.None);
                    return;
                }
                if (behavior.DiceResultValue / 2 > target.breakDetail.breakGauge)
                {
                    target.breakDetail.breakGauge = 0;
                    target.breakDetail.TakeBreakDamage(0, DamageType.Attack, null, AtkResist.Normal, KeywordBuf.None);
                    if (!target.IsStraighten())
                    {
                        target.breakDetail.breakGauge = 0;
                        if (!target.OnBreakGageZero())
                        {
                            target.breakDetail.LoseBreakLife();
                            BattleCardTotalResult battleCardResultLog = target.battleCardResultLog;
                            if (battleCardResultLog != null)
                            {
                                battleCardResultLog.SetBreakState(true);
                            }
                            base.owner.OnMakeBreakState(target);
                            return;
                        }
                    }
                    else
                    {
                        target.breakDetail.breakGauge = 1;
                    }
                }
            }
        }
    }
    //Ever Vigilant
    public class PassiveAbility_SC_EverVigilant : PassiveAbilityBase
    {
        public override void OnStartBattle()
        {
            base.OnStartBattle();
            DiceCardXmlInfo cardItem = ItemXmlDataList.instance.GetCardItem(new LorId("TurbulenceOffice", 45));
            List<BattleDiceBehavior> list = new List<BattleDiceBehavior>();
            int num = 0;
            foreach (DiceBehaviour diceBehaviour in cardItem.DiceBehaviourList)
            {
                BattleDiceBehavior battleDiceBehavior = new BattleDiceBehavior();
                battleDiceBehavior.behaviourInCard = diceBehaviour.Copy();
                battleDiceBehavior.SetIndex(num++);
                list.Add(battleDiceBehavior);
            }
            this.owner.cardSlotDetail.keepCard.AddBehaviours(cardItem, list);
        }
    }
    /*
     * Cards
     */
    //Cover Fire
    public class DiceCardSelfAbility_SC_coverFire : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "AreaCard_Keyword",
                "Protection_Keyword",
                "BreakProtection_Keyword"
                };
            }
        }
        public override void OnStartBattle()
        {
            DiceCardXmlInfo cardItem = ItemXmlDataList.instance.GetCardItem(new LorId("TurbulenceOffice", 46));
            List<BattleDiceBehavior> list = new List<BattleDiceBehavior>();
            int num = 0;
            foreach (DiceBehaviour diceBehaviour in cardItem.DiceBehaviourList)
            {
                BattleDiceBehavior battleDiceBehavior = new BattleDiceBehavior();
                battleDiceBehavior.behaviourInCard = diceBehaviour.Copy();
                battleDiceBehavior.SetIndex(num++);
                list.Add(battleDiceBehavior);
            }
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(card.owner.faction))
            {
                if (battleUnitModel != null)
                {
                    battleUnitModel.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Protection, 2, owner);
                    battleUnitModel.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.BreakProtection, 2, owner);
                    battleUnitModel.cardSlotDetail.keepCard.AddBehaviours(cardItem, list);
                }
            }
        }
        public static string Desc = "[Combat Start] All allies gain 2 Protection and Stagger Protection and a Ranged Counter die (Pierce, 4-8) this Scene";
    }
    //Bulletstorm
    public class DiceCardSelfAbility_SC_uniteBulletstorm : DiceCardSelfAbilityBase
    {
        public override bool IsUniteCard
        {
            get
            {
                return true;
            }
        }
        public override void OnStartBattle()
        {
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(base.owner.faction))
            {
                if (battleUnitModel != base.owner && !battleUnitModel.bufListDetail.HasBuf<BattleUnitBuf_SC_unitePowerDown>())
                {
                    battleUnitModel.bufListDetail.AddBuf(new BattleUnitBuf_SC_unitePowerDown());
                }
            }
        }

        public class BattleUnitBuf_SC_unitePowerDown : BattleUnitBuf
        {
            public override void OnStartParrying(BattlePlayingCardDataInUnitModel card)
            {
                if (card.cardAbility != null && card.cardAbility.IsUniteCard)
                {
                    BattleUnitModel target = card.target;
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
                        power = -1
                    });
                }
            }
            public override void OnRoundEnd()
            {
                this.Destroy();
            }
        }
        public static string Desc = "[Unity]\n[Combat Start] For this Scene, all other allies’ Unity pages gain ‘[Start of Clash] Reduce Power of all target's dice by 1’ (Cannot be stacked)";
    }
    //Rupture
    public class DiceCardSelfAbility_SC_badStatusPower : DiceCardSelfAbilityBase
    {
        public override void OnUseCard()
        {
            BattleUnitModel target = card.target;
            if (target == null)
            {
                return;
            }
            if (target.bufListDetail.GetActivatedBufList().Exists((BattleUnitBuf x) => x.positiveType == BufPositiveType.Negative))
            {
                card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
                {
                    power = 1
                });
            }
        }
        public static string Desc = "[On Use] If target has a status ailment, all dice on this page gain +1 Power";
    }
    public class PassiveAbility_Stocks : PassiveAbilityBase //Stock Images
    {
        public override void OnRoundEnd()
        {
            BattleUnitBuf battleUnitBuf = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Stocks);
            if (battleUnitBuf.stack >= 1)
            {
                this.owner.cardSlotDetail.RecoverPlayPoint(battleUnitBuf.stack / 10);
            }
        }
        public override void OnSucceedAttack(BattleDiceBehavior behavior)
        {
            BattleCardTotalResult battleCardResultLog = this.owner.battleCardResultLog;
            bool flag3 = battleCardResultLog != null;
            bool flag4 = flag3;
            if (flag4)
            {
                battleCardResultLog.SetPassiveAbility(this);
            }
            BattleUnitModel target = behavior?.card?.target;
            BattleUnitBuf_Copyright BattleUnitBuf_Copyright = target?.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Copyright) as BattleUnitBuf_Copyright;
            bool flagx = BattleUnitBuf_Copyright == null;
            if (flagx)
            {
                target?.bufListDetail.AddBuf(new BattleUnitBuf_Copyright(1));
            }
            int stack = RandomUtil.Range(1, 3);
            bool flag2x = BattleUnitBuf_Copyright.stack >= 1 && BattleUnitBuf_Copyright != null;
            if (flag2x)
            {
                BattleUnitBuf_Copyright.stack += stack;
            }

        }

        public override void OnTakeDamageByAttack(BattleDiceBehavior atkDice, int dmg)
        {
            BattleCardTotalResult battleCardResultLog = this.owner.battleCardResultLog;
            if (battleCardResultLog != null)
            {
                battleCardResultLog.SetPassiveAbility(this);
            }
            BattleUnitBuf_Copyright BattleUnitBuf_Copyright = atkDice?.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Copyright) as BattleUnitBuf_Copyright;
            if (BattleUnitBuf_Copyright == null)
            {
                atkDice?.owner?.bufListDetail.AddBuf(new BattleUnitBuf_Copyright(1));
            }
            int stack = RandomUtil.Range(1, 3);
            if (BattleUnitBuf_Copyright.stack >= 1 && BattleUnitBuf_Copyright != null)
            {
                BattleUnitBuf_Copyright.stack += stack;
            }

        }
    }
    public class BattleUnitBuf_Stocks : BattleUnitBuf
    {
        protected override string keywordId
        {
            get
            {
                return "Stocks";
            }
        }
        protected override string keywordIconId
        {
            get
            {
                return "Stocks";
            }
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
        public override void OnRoundEndTheLast()
        {
            if (this.stack <= 0)
            {
                this.Destroy();
            }
            int stack = this.stack;
            if (this.stack > 30)
            {
                this.stack = 30;
            }
        }
        public override void OnAddBuf(int addedStack)
        {
            if (this.stack > 30)
            {
                this.stack = 30;
            }
        }
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (this._owner.IsImmune(this.bufType))
            {
                return;
            }
            if (this.stack >= 1)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    max = this.stack / 5,
                    min = -this.stack / 10,
                });
            }
        }
        public override void OnRoundStart()
        {
            if (this.stack <= 0)
            {
                this.Destroy();
            }
            if (this._owner.IsImmune(this.bufType))
            {
                return;
            }
        }
    }
    public class BattleUnitBuf_Copyright : BattleUnitBuf
    {
        public override BufPositiveType positiveType => BufPositiveType.Negative;
        public void Add(int add)
        {
            this.stack += add;
            bool flag = this.stack < 1;
            if (flag)
            {
                this.Destroy();
            }
        }
        public BattleUnitBuf_Copyright(int value)
        {
            this.stack = value;
        }
        public override void OnRoundEnd()
        {
            if (this.stack >= 30)
            {
                this._owner.cardSlotDetail.LoseWhenStartRound(3);
                this._owner.allyCardDetail.DiscardACardLowest();
                this.Destroy();
            }
        }
        protected override string keywordId
        {
            get
            {
                return "Copyright";
            }
        }
        protected override string keywordIconId
        {
            get
            {
                return "Copyright";
            }
        }
        public override void OnRoundEndTheLast()
        {
            if (this.stack <= 0)
            {
                this.Destroy();
            }
            if (!this._owner.IsImmune(this.bufType))
            {
                int num = this.stack;
                this._owner.TakeBreakDamage(this.stack, DamageType.Buf, null, AtkResist.Normal, this.bufType);
            }
            this.stack = this.stack * 2 / 3;
            if (this.stack <= 0)
            {
                this.Destroy();
            }
        }
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (this._owner.IsImmune(this.bufType))
            {
                return;
            }
            if (this.stack >= 1)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    max = -this.stack / 4
                });
            }
        }
        public override void OnRoundStart()
        {
            if (this.stack <= 0)
            {
                this.Destroy();
            }
            if (this._owner.IsImmune(this.bufType))
            {
                return;
            }
        }

    }
    public class PassiveAbility_JackOfAllTrades : PassiveAbilityBase
    {
        public override void OnRoundStart()
        {
            base.OnRoundEnd();
            this.owner.bufListDetail.AddBuf(new PassiveAbility_JackOfAllTrades.BattleUnitBuf_StockGacha(RandomUtil.SelectOne<BehaviourDetail>(this._dmgTypes)));
        }

        private readonly BehaviourDetail[] _dmgTypes = new BehaviourDetail[]
        {
            BehaviourDetail.Slash,
            BehaviourDetail.Penetrate,
            BehaviourDetail.Hit
        };


        public class BattleUnitBuf_StockGacha : BattleUnitBuf
        {
            protected override string keywordId
            {
                get
                {
                    string result;
                    switch (this._dmgType)
                    {
                        case BehaviourDetail.Slash:
                            result = "StockSlash";
                            break;
                        case BehaviourDetail.Penetrate:
                            result = "StockPenetrate";
                            break;
                        case BehaviourDetail.Hit:
                            result = "StockHit";
                            break;
                        default:
                            result = base.keywordIconId;
                            break;
                    }
                    return result;
                }
            }

            protected override string keywordIconId
            {
                get
                {
                    string result;
                    switch (this._dmgType)
                    {
                        case BehaviourDetail.Slash:
                            result = "SlashPowerUp";
                            break;
                        case BehaviourDetail.Penetrate:
                            result = "PenetratePowerUp";
                            break;
                        case BehaviourDetail.Hit:
                            result = "HitPowerUp";
                            break;
                        default:
                            result = base.keywordIconId;
                            break;
                    }
                    return result;
                }
            }

            public override int paramInBufDesc
            {
                get
                {
                    return 2;
                }
            }

            public BattleUnitBuf_StockGacha(BehaviourDetail dmgType)
            {
                this._dmgType = dmgType;
                this.stack = 0;
            }

            public override void BeforeRollDice(BattleDiceBehavior behavior)
            {
                base.BeforeRollDice(behavior);
                bool flag = behavior.Detail == this._dmgType;
                if (flag)
                {
                    behavior.ApplyDiceStatBonus(new DiceStatBonus
                    {
                        max = 2,
                    });
                }
            }

            public override AtkResist GetResistHP(AtkResist origin, BehaviourDetail detail)
            {
                bool flag = detail == this._dmgType;
                AtkResist result;
                if (flag)
                {
                    result = AtkResist.Endure;
                }
                else
                {
                    result = base.GetResistHP(origin, detail);
                }
                return result;
            }

            public override AtkResist GetResistBP(AtkResist origin, BehaviourDetail detail)
            {
                bool flag = detail == this._dmgType;
                AtkResist result;
                if (flag)
                {
                    result = AtkResist.Endure;
                }
                else
                {
                    result = base.GetResistBP(origin, detail);
                }
                return result;
            }
            public override void OnRoundEnd()
            {
                base.OnRoundEnd();
                this.Destroy();
            }

            private readonly BehaviourDetail _dmgType;

        }
    }
    public class PassiveAbility_Everything : PassiveAbilityBase
    {
        private bool isSus;
        public override void OnRoundStart()
        {
            base.OnRoundStart();
            isSus = false;
        }
        public override void ChangeDiceResult(BattleDiceBehavior behavior, ref int diceResult)
        {
            if (!isSus)
            {
                int diceMin = behavior.GetDiceMin();
                int diceMax = behavior.GetDiceMax();
                int num = (int)Math.Floor((double)(diceMin + diceMax) / 2.0);
                int num2 = diceResult;
                if (diceResult <= num)
                {
                    diceResult = DiceStatCalculator.MakeDiceResult(num2, diceMax, 0);
                    BattleCardTotalResult battleCardResultLog = behavior.owner.battleCardResultLog;
                    if (battleCardResultLog != null)
                    {
                        battleCardResultLog.SetPassiveAbility(this);
                        battleCardResultLog.SetVanillaDiceValue(num2);
                        isSus = true;
                    }
                }
            }
        }
    }

    public class DiceCardPriority_StockManEnergyYes : DiceCardPriorityBase //light AI
    {
        public override int GetPriorityBonus(BattleUnitModel owner)
        {
            if (owner.cardSlotDetail.PlayPoint >= 6)
            {
                return -1;
            }
            return 0;
        }
    }
    public class DiceCardPriority_StockManDrawYes : DiceCardPriorityBase //draw AI
    {
        public override int GetPriorityBonus(BattleUnitModel owner)
        {
            if (owner.allyCardDetail.GetHand().Count >= 6)
            {
                return -1;
            }
            return 0;
        }
    }
    public class DiceCardSelfAbility_Stocks1 : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                    "Stocks_Keyword",
                    "DrawCard_Keyword"
                };
            }
        }
        public static string Desc = "[On Use] Draw 1 Page and gain 3 Stock Images";
        public override void OnUseCard()
        {
            this.owner.allyCardDetail.DrawCards(1);
            for (int i = 0; i < 3; i++)
            {
                BattleUnitBuf battleUnitBuf = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Stocks);
                if (battleUnitBuf == null)
                {
                    battleUnitBuf = new BattleUnitBuf_Stocks();
                    this.owner.bufListDetail.AddBuf(new BattleUnitBuf_Stocks());
                }
                battleUnitBuf.stack++;
            }
        }
    }
    public class DiceCardSelfAbility_Stocks2 : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                    "Stocks_Keyword",
                    "Energy_Keyword"
                };
            }
        }
        public static string Desc = "[On Use] Restore 1 Light and spend 4 Stock Images to restore 2 Light";
        public override void OnUseCard()
        {
            this.owner.allyCardDetail.DrawCards(1);
            this.owner.cardSlotDetail.RecoverPlayPoint(1);
            for (int i = 0; i < 3; i++)
            {
                BattleUnitBuf battleUnitBuf = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Stocks);
                if (battleUnitBuf == null)
                {
                    battleUnitBuf = new BattleUnitBuf_Stocks();
                    this.owner.bufListDetail.AddBuf(new BattleUnitBuf_Stocks());
                }
                battleUnitBuf.stack++;
            }
        }
    }
    public class DiceCardSelfAbility_Stocks3 : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                    "Stocks_Keyword",
                    "DrawCard_Keyword"
                };
            }
        }
        public static string Desc = "[On Use] Draw 1 Page; spend 6 Stock Images to draw 2 pages.";
        public override void OnUseCard()
        {
            this.owner.allyCardDetail.DrawCards(1);
            BattleUnitBuf battleUnitBuf = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Stocks);
            if (battleUnitBuf.stack >= 6)
            {
                battleUnitBuf.stack -= 6;
                this.owner.allyCardDetail.DrawCards(2);
            }
        }
    }
    public class DiceCardSelfAbility_Stocks4 : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                    "Stocks_Keyword",
                };
            }
        }
        public static string Desc = "[On Use] Gain 6 Stock Images";
        public override void OnUseCard()
        {
            for (int i = 0; i < 6; i++)
            {
                BattleUnitBuf battleUnitBuf = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Stocks);
                if (battleUnitBuf == null)
                {
                    battleUnitBuf = new BattleUnitBuf_Stocks();
                    this.owner.bufListDetail.AddBuf(new BattleUnitBuf_Stocks());
                }
                battleUnitBuf.stack++;
            }
        }
    }
    public class DiceCardSelfAbility_Stocks5 : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                    "Stocks_Keyword",
                    "DrawCard_Keyword"
                };
            }
        }
        public static string Desc = "[On Use] Gain 8 Stock Images and draw 1 Page";
        public override void OnUseCard()
        {
            this.owner.allyCardDetail.DrawCards(1);
            for (int i = 0; i < 6; i++)
            {
                BattleUnitBuf battleUnitBuf = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Stocks);
                if (battleUnitBuf == null)
                {
                    battleUnitBuf = new BattleUnitBuf_Stocks();
                    this.owner.bufListDetail.AddBuf(new BattleUnitBuf_Stocks());
                }
                battleUnitBuf.stack++;
            }
        }
    }
    public class DiceCardSelfAbility_Stocks6 : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                    "Stocks_Keyword",
                };
            }
        }
        public static string Desc = "[On Use] Inflict Power Nullification on self and target; Spend 10 Stock Images to boost the minimum and maximum roll value of all dice on this page by +6.";
        public override void OnUseCard()
        {
            BattleUnitModel target = base.card.target;
            bool ass = target == null;
            if (!ass)
            {
                target.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.NullifyPower, 1, null);
                base.owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.NullifyPower, 1, null);
            }
            BattleUnitBuf battleUnitBuf = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Stocks);
            if (battleUnitBuf.stack >= 10)
            {
                battleUnitBuf.stack -= 10;
                this.card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
                {
                    min = 6,
                    max = 6,
                });
            }
        }
    }
    public class DiceCardAbility_1CopyrightHit : DiceCardAbilityBase
    {
        public static string Desc = "[On Hit] Inflict 1 Copyright this Scene";
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            BattleUnitBuf_Copyright BattleUnitBuf_Copyright = target.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Copyright) as BattleUnitBuf_Copyright;
            bool flag = BattleUnitBuf_Copyright == null;
            if (flag)
            {
                target.bufListDetail.AddBuf(new BattleUnitBuf_Copyright(1));
            }
            bool flag2 = BattleUnitBuf_Copyright.stack >= 1;
            if (flag2)
            {
                BattleUnitBuf_Copyright.stack += 1;
            }
        }
    }
    public class DiceCardAbility_2CopyrightHit : DiceCardAbilityBase
    {
        public static string Desc = "[On Hit] Inflict 2 Copyright this Scene";
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            BattleUnitBuf_Copyright BattleUnitBuf_Copyright = target.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Copyright) as BattleUnitBuf_Copyright;
            bool flag = BattleUnitBuf_Copyright == null;
            if (flag)
            {
                target.bufListDetail.AddBuf(new BattleUnitBuf_Copyright(2));
            }
            bool flag2 = BattleUnitBuf_Copyright.stack >= 1;
            if (flag2)
            {
                BattleUnitBuf_Copyright.stack += 2;
            }
        }
    }
    public class DiceCardAbility_3CopyrightHit : DiceCardAbilityBase
    {
        public static string Desc = "[On Hit] Inflict 3 Copyright this Scene";
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            BattleUnitBuf_Copyright BattleUnitBuf_Copyright = target.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Copyright) as BattleUnitBuf_Copyright;
            bool flag = BattleUnitBuf_Copyright == null;
            if (flag)
            {
                target.bufListDetail.AddBuf(new BattleUnitBuf_Copyright(3));
            }
            bool flag2 = BattleUnitBuf_Copyright.stack >= 1;
            if (flag2)
            {
                BattleUnitBuf_Copyright.stack += 3;
            }
        }
    }
    public class DiceCardAbility_1or2CopyrightHitAll : DiceCardAbilityBase
    {
        public static string Desc = "[On Hit] Inflict 1-2 Copyright to all enemies this Scene";
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            int stack = RandomUtil.Range(1, 2);
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList((base.owner.faction == Faction.Player) ? Faction.Enemy : Faction.Player))
            {
                if (battleUnitModel != null)
                {
                    BattleUnitBuf_Copyright BattleUnitBuf_Copyright = battleUnitModel.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Copyright) as BattleUnitBuf_Copyright;
                    bool flag = BattleUnitBuf_Copyright == null;
                    if (flag)
                    {
                        battleUnitModel.bufListDetail.AddBuf(new BattleUnitBuf_Copyright(stack));
                    }
                    bool flag2 = BattleUnitBuf_Copyright != null && BattleUnitBuf_Copyright.stack >= 1;
                    if (flag2)
                    {
                        BattleUnitBuf_Copyright.stack += stack;
                    }
                }
            }

        }
    }
    public class DiceCardSelfAbility_ReverbkuthVibrateBurn : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] If Speed is equal to target's Vibration, all dice on this page gain [On Hit]Inflict 4 Burn";
        public override void OnUseCard()
        {
            BattleUnitBuf activatedBuf = this.card.target.bufListDetail.GetActivatedBuf(KeywordBuf.Vibrate);
            if (activatedBuf != null && this.card.speedDiceResultValue == activatedBuf.stack)
            {
                this.card.ApplyDiceAbility(DiceMatch.AllAttackDice, new DiceCardAbility_burn4atk());
            }
        }
    }
    public class PassiveAbility_nuovobingaze : PassiveAbilityBase
    {

        // Token: 0x0600015D RID: 349 RVA: 0x00002935 File Offset: 0x00000B35
        public override int GetDamageReduction(BattleDiceBehavior behavior)
        {
            return RandomUtil.Range(1, 2);
        }

        // Token: 0x0600015E RID: 350 RVA: 0x00002935 File Offset: 0x00000B35
        public override int GetBreakDamageReduction(BattleDiceBehavior behavior)
        {
            return RandomUtil.Range(1, 2);
        }
    }
    public class PassiveAbility_Reverbkuth1 : PassiveAbilityBase
    {
        // Token: 0x06004EBB RID: 20155 RVA: 0x001A78C0 File Offset: 0x001A5AC0
        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.card.GetSpec().Ranged == CardRange.Near)
            {
                BattleUnitBuf activatedBuf = curCard.target.bufListDetail.GetActivatedBuf(KeywordBuf.Vibrate);
                if (activatedBuf != null)
                {
                    int speedDiceResultValue = curCard.speedDiceResultValue;
                    int stack = activatedBuf.stack;
                    if (this.owner.bufListDetail.HasBuf<BattleUnitBuf_argaliaResonanceControl>())
                    {
                        if (stack >= 4)
                        {
                            curCard.ApplyDiceAbility(DiceMatch.AllAttackDice, new DiceCardAbility_ResonanceBurn());
                            curCard.OnActivateResonance();
                            return;
                        }
                    }
                    else if (Mathf.Abs(speedDiceResultValue - stack) <= 1)
                    {
                        curCard.ApplyDiceAbility(DiceMatch.AllAttackDice, new DiceCardAbility_ResonanceBurn());

                        curCard.OnActivateResonance();
                    }
                }
            }
        }

        // Token: 0x06004EBC RID: 20156 RVA: 0x0018BB20 File Offset: 0x00189D20
        public override void OnWaveStart()
        {
            this.owner.allyCardDetail.DrawCards(2);

        }
    }
    public class DiceCardAbility_ResonanceBurn : DiceCardAbilityBase
    {
        // Token: 0x06000016 RID: 22 RVA: 0x0000249C File Offset: 0x0000069C
        public override void OnSucceedAttack()
        {
            BattleUnitModel target = base.card.target;
            bool flag = target != null;
            if (flag)
            {
                BattleUnitBuf activatedBuf = target.bufListDetail.GetActivatedBuf(KeywordBuf.Vibrate);
                bool flag2 = activatedBuf != null && activatedBuf.stack >= 1;
                if (flag2)
                {
                    target.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Burn, activatedBuf.stack, null);
                }
            }
        }
    }
    public class DiceCardSelfAbility_reverbkuth_trailsofblue : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] If target has 30 or more Burn, target's dice Power cannot be influenced by any effects next Scene";
        public override void OnUseCard()
        {
            base.OnUseCard();
            BattleUnitModel target = this.card.target;
            if (target.bufListDetail.GetKewordBufStack(KeywordBuf.Burn) >= 30)
            {
                target.bufListDetail.AddKeywordBufByCard(KeywordBuf.NullifyPower, 1, this.owner);
            }
        }
    }
    public class PassiveAbility_Reverbkuth2 : PassiveAbilityBase
    {
        public override bool IsImmuneDmg(DamageType type, KeywordBuf keyword = KeywordBuf.None)
        {
            return keyword == KeywordBuf.Burn || base.IsImmuneDmg(type, keyword);
        }
        // Token: 0x0600015D RID: 349 RVA: 0x00002935 File Offset: 0x00000B35
        public override int GetDamageReduction(BattleDiceBehavior behavior)
            => behavior?.card != null && IsBurnCard(behavior.card) ? 9999 : base.GetDamageReduction(behavior);

        // Token: 0x0600015E RID: 350 RVA: 0x00002935 File Offset: 0x00000B35
        public override int GetBreakDamageReduction(BattleDiceBehavior behavior)
            => behavior?.card != null && IsBurnCard(behavior.card) ? 9999 : base.GetDamageReduction(behavior);

        // Token: 0x060000DD RID: 221 RVA: 0x00004308 File Offset: 0x00002508
        private bool IsBurnCard(BattlePlayingCardDataInUnitModel card)
            => card?.card?.XmlData is DiceCardXmlInfo xmlData
            && (xmlData.Keywords.Contains("Burn_Keyword")
            || BattleCardAbilityDescXmlList.Instance.GetAbilityKeywords(xmlData).Contains("Burn_Keyword")
            || xmlData.DiceBehaviourList.Exists(x => BattleCardAbilityDescXmlList.Instance.GetAbilityKeywords_byScript(x.Script).Contains("Burn_Keyword")));


    }

    public class PassiveAbility_RatTactics : PassiveAbilityBase
    {
        public override void OnStartParrying(BattlePlayingCardDataInUnitModel card)
        {
            int DiceOnOppPage = (card.card.CreateDiceCardBehaviorList().Count + card.target.currentDiceAction.card.CreateDiceCardBehaviorList().Count) / 2;
            card.target.currentDiceAction.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                min = -DiceOnOppPage,
                max = -DiceOnOppPage
            });
        }
    }
    public class DiceCardPriority_PogoHasteLightP : DiceCardPriorityBase
    {
        public override int GetPriorityBonus(BattleUnitModel owner)
        {
            if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.Quickness) >= 4)
            {
                return 1;
            }
            return 0;
        }
    }
    public class DiceCardPriority_PogoHasteDrawP : DiceCardPriorityBase
    {
        public override int GetPriorityBonus(BattleUnitModel owner)
        {
            if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.Quickness) >= 2)
            {
                return 1;
            }
            return 0;
        }
    }
    public class DiceCardAbility_Endurancethisscene : DiceCardAbilityBase
    {
        public override void OnWinParrying()
        {
            base.owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Endurance, 1, base.owner);
        }
        public static string Desc = "[On Clash Win] Gain 1 Endurance this Scene";
    }
    public class DiceCardSelfAbility_Masterofsurvival : DiceCardSelfAbilityBase
    {
        public void ExhaustAndReturn()
        {
            this.card.card.exhaust = true;
            base.owner.bufListDetail.AddBuf(new BattleUnitBuf_howmanyyears(this.card.card.GetID(), 2));
        }

        public override void OnUseCard()
        {
            this.owner.bufListDetail.AddReadyBuf(new battleUnitBuf_Stealthyratbuff());
            this.ExhaustAndReturn();
        }
        public static string Desc = "This page is exhausted on use and returns to hand after 2 Scenes\n[On Use] Become Untargetable next Scene";
    }
    public class BattleUnitBuf_howmanyyears : BattleUnitBuf
    {
        public BattleUnitBuf_howmanyyears(LorId cardId, int turnCount)
        {
            this._cardId = cardId;
            this._count = turnCount;
        }

        public override void OnRoundStart()
        {
            this._count--;
            if (this._count <= 0)
            {
                this._owner.allyCardDetail.AddNewCard(this._cardId, false);
                this.Destroy();
            }
        }

        private int _count;

        private LorId _cardId = LorId.None;
    }
    public class battleUnitBuf_Stealthyratbuff : BattleUnitBuf
    {
        public override bool IsTargetable()
        {
            return false;
        }
        public override void OnRoundEnd()
        {
            base.OnRoundEnd();
            this.Destroy();
        }
    }
    public class DiceCardSelfAbility_Onesidedeconomy : DiceCardSelfAbilityBase
    {
        public override void OnStartOneSideAction()
        {
            List<BattleUnitModel> alivelist = BattleObjectManager.instance.GetAliveList(this.card.owner.faction);
            foreach (BattleUnitModel battleUnitModel in alivelist)
            {
                battleUnitModel.cardSlotDetail.RecoverPlayPoint(1);
                battleUnitModel.allyCardDetail.DrawCards(1);
            }
        }
        public static string Desc = "[On a One-Sided attack] all Allies restore 1 Light and draw 1 Page";
    }
    public class DiceCardAbility_Onesidedbuff : DiceCardAbilityBase
    {
        public override void OnWinParrying()
        {
            base.OnWinParrying();
            this.Ability();
        }
        public override void OnDrawParrying()
        {
            base.OnDrawParrying();
            this.Ability();
        }

        public override void OnLoseParrying()
        {
            base.OnLoseParrying();
            BattleUnitModel target = card.target;
            BattleDiceBehavior among = target.currentDiceAction.currentBehavior;
            if (among != null && !base.IsAttackDice(among.Detail))
            {
                this.Ability();
            }

        }
        private void Ability()
        {
            base.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Strength, 1, base.owner);
            base.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Endurance, 1, base.owner);
            base.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Protection, 1, base.owner);
            base.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.BreakProtection, 1, base.owner);
        }

        public override void AfterAction()
        {
            BattleUnitBuf buf = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is battleUnitBuf_Underhandedbuff);
            if (buf != null)
            {
                this.owner.bufListDetail.RemoveBuf(buf);
            }
        }
        public static string Desc = "[Unattacked] Gain 1 Strength, Endurance, Protection and Stagger protection next Scene";
    }
    public class battleUnitBuf_Underhandedbuff : BattleUnitBuf
    {
        public override void OnRoundEnd()
        {
            base.OnRoundEnd();
            this.Destroy();
        }
    }
    public class DiceCardSelfAbility_Underhandedattack : DiceCardSelfAbilityBase
    {
        public override void OnStartOneSideAction()
        {
            base.owner.cardSlotDetail.RecoverPlayPointByCard(3);
            base.owner.allyCardDetail.DrawCards(1);
            base.OnStartOneSideAction();
            this.owner.bufListDetail.AddBuf(new battleUnitBuf_Underhandedbuff());
        }
        public override void OnEndBattle()
        {
            base.OnEndBattle();
            BattleUnitBuf buf = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is battleUnitBuf_Underhandedbuff);
            if (buf != null)
            {
                base.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Strength, 1, base.owner);
                base.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Endurance, 1, base.owner);
                base.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Protection, 1, base.owner);
                base.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.BreakProtection, 1, base.owner);
                this.owner.bufListDetail.RemoveBuf(buf);
            }
        }
        public static string Desc = "[On a One-Sided Attack] Restore 3 Light and draw 1 Page";

    }
    public class DiceCardAbility_feebledisarmfragilebreakfragileattack : DiceCardAbilityBase
    {
        public override void OnSucceedAttack()
        {
            {
                BattleUnitModel target = base.card.target;
                if (target == null)
                {
                    target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Weak, 1, base.owner);
                    target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Disarm, 1, base.owner);
                    target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Vulnerable, 1, base.owner);
                    target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Vulnerable_break, 1, base.owner);
                }
            }
        }
        public static string Desc = "[On Hit] Inflict 1 Feeble, Disarm, Fragile and Stagger Fragile next Scene";
    }
    public class DiceCardSelfAbility_PogoHasteDraw : DiceCardSelfAbilityBase

    {
        public override void OnUseCard()
        {
            BattleUnitBuf activatedBuf = this.owner.bufListDetail.GetActivatedBuf(KeywordBuf.Quickness);
            if (activatedBuf != null)
            {
                int stack = activatedBuf.stack;
                if (stack >= 2)
                {
                    this.owner.cardSlotDetail.RecoverPlayPoint(3);
                }
            }

        }

        public static string Desc = "[On Use] If this character has at least 2 Haste, Restore 3 Light";
    }
    public class DiceCardSelfAbility_PogoHasteLight : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] If this character has at least 4 Haste, Draw 4 Pages";
        public override void OnUseCard()
        {
            BattleUnitBuf activatedBuf = this.owner.bufListDetail.GetActivatedBuf(KeywordBuf.Quickness);
            if (activatedBuf != null)
            {
                int stack = activatedBuf.stack;
                if (stack >= 4)
                {
                    this.owner.allyCardDetail.DrawCards(4);
                }
            }

        }
    }
    public class DiceCardSelfAbility_PogoHastePwr : DiceCardSelfAbilityBase
    {
        public override void OnUseCard()
        {
            int power = base.owner.bufListDetail.GetKewordBufStack(KeywordBuf.Quickness) / 2;
            this.card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = power
            });
        }
        public static string Desc = "[On Use] All dice on this page gain +1 Power for each Haste/2 on self";
    }
    public class DiceCardSelfAbility_PogoHastePwr2 : DiceCardSelfAbilityBase
    {
        public override void OnUseCard()
        {
            int power = base.owner.bufListDetail.GetKewordBufStack(KeywordBuf.Quickness) / 2;
            this.card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = power
            });
        }
        public static string Desc = "Cannot be redirected. This page is used against all enemies.\n[On Use] All dice on this page gain +1 Power for each Haste/2 on self";
    }
    public class PassiveAbility_HammerTime_BoingBitch : PassiveAbilityBase
    {
        public static string Name = "Pogo Bitch";
        public static string Desc = "When this Character is above Emotion Level 3, Haste Gain is doubled and Dice Gain 1 Power if this character has 2 or more Haste";
        public override int GetMultiplierOnGiveKeywordBufByCard(BattleUnitBuf buf, BattleUnitModel target)
        {
            if (this.owner.emotionDetail.EmotionLevel >= 3)
            {
                if (target == this.owner && buf.bufType == KeywordBuf.Quickness)
                {
                    return 2;
                }
            }
            return 1;
        }
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            BattleCardTotalResult battleCardResultLog = this.owner.battleCardResultLog;
            if (battleCardResultLog != null)
            {
                battleCardResultLog.SetPassiveAbility(this);
            }
            BattleUnitBuf activatedBuf = this.owner.bufListDetail.GetActivatedBuf(KeywordBuf.Quickness);
            if (activatedBuf != null && activatedBuf.stack >= 2)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = 1
                });
            }
        }
    }
    public class PassiveAbility_HammerTime_PogoHammer : PassiveAbilityBase
    {
        public static string Name = "Pogo Stick";
        public static string Desc = "On winning a clash using Evade dice, gain 1 Haste next Scene. (Up to 3 times per Scene)";
        public override void OnRoundStart()
        {
            base.OnRoundStart();
            among = 0;
        }
        public override void OnWinParrying(BattleDiceBehavior behavior)
        {
            base.OnWinParrying(behavior);
            if (behavior.Detail == BehaviourDetail.Evasion && among < 3)
            {
                among++;
                this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Quickness, 1, base.owner);
            }


        }
        private int among;


    }
    public class PassiveAbility_HammerTime_PogoneSmash : PassiveAbilityBase
    {
        public static string Name = "Pogo Smash";
        public static string Desc = "On winning a clash using Block Dice, inflict up to 3 Bind(Per Scene)";
        private bool doingitdoingitdoingitwelldoingitdoingitdoingitwell;
        public override void OnRoundStart()
        {
            base.OnRoundStart();
            doingitdoingitdoingitwelldoingitdoingitdoingitwell = false;
        }
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            base.BeforeRollDice(behavior);
            if (!doingitdoingitdoingitwelldoingitdoingitdoingitwell)
            {
                int amongus = this.owner.bufListDetail.GetKewordBufStack(KeywordBuf.Quickness) / 2;
                doingitdoingitdoingitwelldoingitdoingitdoingitwell = true;
                if (amongus > 5)
                {
                    amongus = 5;
                }
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = amongus
                });
            }
        }
    }
    public class DiceCardAbility_heal2cwteam : DiceCardAbilityBase
    {
        public static string Desc = "[On Clash Win] All allies recover 2 HP and Stagger Resist";
        public override void OnWinParrying()
        {
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(base.owner.faction))
            {
                battleUnitModel.RecoverHP(2);
                battleUnitModel.breakDetail.RecoverBreak(2);
            }
        }
    }
    public class DiceCardAbility_inversedmg : DiceCardAbilityBase
    {
        public static string Desc = "Deals more damage based on target's missing HP";
        public override void BeforeGiveDamage(BattleUnitModel target)
        {
            this.behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                dmgRate = 100 - (int)(target.hp / (float)target.MaxHp * 100f)
            });
        }
    }
    public class DiceCardAbility_staggerdmgcw : DiceCardAbilityBase
    {
        public static string Desc = "[On Clash Win] Deal 3 Stagger damage to target";
        public override void OnWinParrying()
        {
            base.card.target.TakeBreakDamage(3, DamageType.Attack, null, AtkResist.Normal, KeywordBuf.None);
        }
    }
    public class DiceCardSelfAbility_recover1draw1 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] Restore 2 Light and draw 1 page";
        // Token: 0x0600000A RID: 10 RVA: 0x00002386 File Offset: 0x00000586
        public override void OnUseCard()
        {
            base.owner.allyCardDetail.DrawCards(1);
            base.owner.cardSlotDetail.RecoverPlayPointByCard(2);
        }
    }
    public class DiceCardSelfAbility_returnlater : DiceCardSelfAbilityBase
    {
        public static string Desc = "This page is exhausted on use and returns to hand after 4 Scenes";
        public override void OnUseCard()
        {
            this.card.card.exhaust = true;
            base.owner.bufListDetail.AddBuf(new DiceCardSelfAbility_returnlater.BattleUnitBuf_returnlater(this.card.card.GetID(), 4));
        }
        public class BattleUnitBuf_returnlater : BattleUnitBuf
        {
            public BattleUnitBuf_returnlater(LorId card, int turnCount)
            {
                this._card = card;
                this._count = turnCount;
            }
            public override void OnRoundStart()
            {
                this._count--;
                bool flag = this._count <= 0;
                if (flag)
                {
                    this._owner.allyCardDetail.AddNewCard(this._card, false);
                    this.Destroy();
                }
            }
            public LorId _card = LorId.None;
            private int _count;
        }
    }
    public class PassiveAbility_13001 : PassiveAbilityBase
    {
        public override bool BeforeTakeDamage(BattleUnitModel attacker, int dmg)
        {
            bool result = false;
            bool flag = this.owner.UnitData.floorBattleData.param1 != 2 && (float)((int)this.owner.hp) - (float)dmg < 1f;
            if (flag)
            {
                BattleCardTotalResult battleCardResultLog = this.owner.battleCardResultLog;
                if (battleCardResultLog != null)
                {
                    battleCardResultLog.SetPassiveAbility(this);
                }
                this.owner.RecoverHP(this.owner.MaxHp);
                this.activations++;
                this.owner.UnitData.floorBattleData.param1 = this.activations;
                bool flag2 = this.owner.breakDetail.IsBreakLifeZero();
                if (flag2)
                {
                    this.owner.RecoverBreakLife(this.owner.MaxBreakLife, false);
                    this.owner.breakDetail.nextTurnBreak = false;
                }
                this.owner.breakDetail.RecoverBreak(this.owner.breakDetail.GetDefaultBreakGauge());
                result = true;
            }
            return result;
        }
        private int activations;
    }
    public class PassiveAbility_13002 : PassiveAbilityBase
    {
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            bool flag = this.owner.emotionDetail.EmotionLevel >= 3;
            if (flag)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = 1
                });
            }
            bool flag2 = BattleObjectManager.instance.GetAliveList(this.owner.faction).Count == 1;
            if (flag2)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    min = 3,
                    max = 3
                });
            }
            bool flag3 = this.owner.emotionDetail.EmotionLevel >= 3 || BattleObjectManager.instance.GetAliveList(this.owner.faction).Count == 1;
            if (flag3)
            {
                BattleCardTotalResult battleCardResultLog = this.owner.battleCardResultLog;
                if (battleCardResultLog != null)
                {
                    battleCardResultLog.SetPassiveAbility(this);
                }
                this.active = true;
            }
        }
        public void DestroyAura()
        {
            bool flag = this.aura != null && this.aura.gameObject != null;
            if (flag)
            {
                UnityEngine.Object.Destroy(this.aura.gameObject);
            }
            this.aura = null;
        }
        public override void OnDie()
        {
            base.OnDie();
            this.DestroyAura();
        }
        public override void OnRoundStart()
        {
            bool flag = this.aura == null && this.active;
            if (flag)
            {
                try
                {
                    this.aura = SingletonBehavior<DiceEffectManager>.Instance.CreateCreatureEffect(this.path, 1f, this.owner.view, null, -1f);
                }
                catch
                {
                    this.aura = null;
                }
            }
        }
        private CreatureEffect aura;
        private string path = "6/RedHood_Emotion_Aura";
        private bool active;
    }
    public class PassiveAbility_13003 : PassiveAbilityBase
    {
        public override void BeforeGiveDamage(BattleDiceBehavior behavior)
        {
            bool flag = behavior.card.target.breakDetail.IsBreakLifeZero();
            if (!flag)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    breakRate = 100
                });
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    dmg = -(behavior.DiceResultValue / 2)
                });
            }
        }
    }
    public class PassiveAbility_hollowpoint : PassiveAbilityBase
    {
        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.card.GetSpec().Ranged == CardRange.Far)
            {
                curCard.ApplyDiceAbility(DiceMatch.NextDice, new DiceCardAbility_forbidReuseDice());
            }
        }
    }
    public class PassiveAbility_lookmatwohands : PassiveAbilityBase
    {
        public override void OnRoundStart()
        {
            this.owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Smoke, 1, null);
            int count = this.owner.allyCardDetail.GetHand().Count;
            int num = 2 - count;
            if (num > 0)
            {
                this.owner.allyCardDetail.DrawCards(num);
            }
        }
    }
    public class PassiveAbility_intimeyouwilllearnwhatlogicis : PassiveAbilityBase
    {
        public override void OnWinParrying(BattleDiceBehavior behavior)
        {
            base.OnWinParrying(behavior);
            this.owner.RecoverHP(2);
        }
    }
    public class DiceCardAbility_thievingrat : DiceCardAbilityBase
    {
        private void effect()
        {
            List<BattleUnitBuf> list = new List<BattleUnitBuf>();
            BattleUnitModel enemy = this.card.target;
            list = enemy.bufListDetail.GetActivatedBufList();
            if (list.Count > 0 && list != null)
            {
                foreach (BattleUnitBuf buff in enemy.bufListDetail.GetActivatedBufList())
                {
                    if (this.isSus(buff) && buff != null)
                    {
                        KeywordBuf buftype = buff.bufType;
                        this.owner.bufListDetail.AddKeywordBufThisRoundByCard(buftype, buff.stack, this.owner);
                        enemy.bufListDetail.RemoveBufAll(buftype);
                        this.effect();
                    }
                    if (buff != null && buff.GetBufIcon() != null && !this.isSus(buff))
                    {
                        this.owner.bufListDetail.AddBuf(buff);
                        enemy.bufListDetail.RemoveBuf(buff);
                        this.effect();
                    }
                }
            }
        }
        public override void OnSucceedAttack(BattleUnitModel enemy)
        {
            foreach (BattleUnitBuf buff in enemy?.bufListDetail.GetActivatedBufList())
            {
                if (buff.GetBufIcon() != null && buff != null)
                {
                    this.effect();
                }
            }
        }
        public override void BeforeRollDice()
        {
            base.BeforeRollDice();
            if (behavior.TargetDice != null && behavior.TargetDice.Type == BehaviourType.Standby)
            {
                behavior.TargetDice.AddAbility(new DiceCardAbility_ratsroll0());
            }
        }
        public static string Desc = "[On Hit] steal all their buffs :)";
        private bool isSus(BattleUnitBuf buf)
        {
            KeywordBuf bufType = buf.bufType;
            bool result = false;
            switch (bufType)
            {
                case KeywordBuf.Burn:
                case KeywordBuf.Paralysis:
                case KeywordBuf.Bleeding:
                case KeywordBuf.Vulnerable:
                case KeywordBuf.Vulnerable_break:
                case KeywordBuf.Weak:
                case KeywordBuf.Disarm:
                case KeywordBuf.Binding:
                case KeywordBuf.Regeneration:
                case KeywordBuf.Protection:
                case KeywordBuf.BreakProtection:
                case KeywordBuf.Strength:
                case KeywordBuf.Endurance:
                case KeywordBuf.Quickness:
                case KeywordBuf.Blurry:
                case KeywordBuf.DmgUp:
                case KeywordBuf.SlashPowerUp:
                case KeywordBuf.PenetratePowerUp:
                case KeywordBuf.HitPowerUp:
                case KeywordBuf.DefensePowerUp:
                case KeywordBuf.Stun:
                case KeywordBuf.WarpCharge:
                case KeywordBuf.Smoke:
                case KeywordBuf.NullifyPower:
                case KeywordBuf.HalfPower:
                case KeywordBuf.Shock:
                case KeywordBuf.RedShoes:
                case KeywordBuf.SnowQueenPower:
                case KeywordBuf.UniverseCardBuf:
                case KeywordBuf.UniverseEnlightenment:
                case KeywordBuf.FairyCare:
                case KeywordBuf.SpiderBudCocoon:
                case KeywordBuf.SingingMachineRecital:
                case KeywordBuf.QueenOfHatredSign:
                case KeywordBuf.QueenOfHatredHatred:
                case KeywordBuf.KnightOfDespairBlessing:
                case KeywordBuf.SweeperRevival:
                case KeywordBuf.SweeperDup:
                case KeywordBuf.TeddyLove:
                case KeywordBuf.CB_BigBadWolf_Stealth:
                case KeywordBuf.CB_CopiousBleeding:
                case KeywordBuf.CB_BlackSwanDeadBro:
                case KeywordBuf.CB_UniverseDecreaseMaxBp:
                case KeywordBuf.AllPowerUp:
                case KeywordBuf.TakeBpDmg:
                case KeywordBuf.DecreaseSpeedTo1:
                case KeywordBuf.BloodStackBlock:
                case KeywordBuf.Vibrate:
                case KeywordBuf.IndexRelease:
                case KeywordBuf.Decay:
                case KeywordBuf.BurnSpread:
                case KeywordBuf.BurnBreak:
                case KeywordBuf.Seal:
                case KeywordBuf.SealKeyword:
                case KeywordBuf.RedMist:
                case KeywordBuf.Fairy:
                case KeywordBuf.RedMistEgo:
                case KeywordBuf.NicolaiTarget:
                case KeywordBuf.MyoBerserk:
                case KeywordBuf.Maxim:
                case KeywordBuf.CB_RedHoodTarget:
                case KeywordBuf.CB_NothingSkin:
                case KeywordBuf.CB_NothingMimic:
                case KeywordBuf.RedHoodChange:
                case KeywordBuf.FreischutzChange:
                case KeywordBuf.WhiteNightChange:
                case KeywordBuf.PurpleSlash:
                case KeywordBuf.PurplePenetrate:
                case KeywordBuf.PurpleHit:
                case KeywordBuf.PurpleDefense:
                case KeywordBuf.PurpleCoolTime:
                case KeywordBuf.JaeheonPuppetThread:
                case KeywordBuf.JaeheonMark:
                case KeywordBuf.OswaldDaze:
                case KeywordBuf.Resistance:
                case KeywordBuf.HeavySmoke:
                case KeywordBuf.Roland2PhaseTakeDamaged:
                case KeywordBuf.Arrest:
                case KeywordBuf.ForbidRecovery:
                case KeywordBuf.UpSurge:
                case KeywordBuf.KeterFinal_Eager:
                case KeywordBuf.KeterFinal_FailLying:
                case KeywordBuf.KeterFinal_SuccessLying:
                case KeywordBuf.KeterFinal_ChangeCostAll:
                case KeywordBuf.KeterFinal_ChangeLibrarianHands:
                case KeywordBuf.KeterFinal_DoubleEmotion:
                case KeywordBuf.KeterFinal_Light:
                case KeywordBuf.KeterFinal_angela_ego:
                case KeywordBuf.Nail:
                case KeywordBuf.ClawCounter:
                case KeywordBuf.Emotion_Sin:
                case KeywordBuf.Alriune_Debuf:
                case KeywordBuf.CB_BigBadWolf_Scar:
                    result = true;
                    break;
            }
            return result;
        }
    }
    public class DiceCardAbility_ratsroll0 : DiceCardAbilityBase
    {
        public override bool Invalidity => true;
    }
    public class PassiveAbility_ratking : PassiveAbilityBase
    {
        public override void OnWaveStart()
        {
            base.OnWaveStart();
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList())
            {
                if (!battleUnitModel.passiveDetail.HasPassive<PassiveAbility_bowtotheratking>())
                {
                    battleUnitModel.passiveDetail.AddPassive(new PassiveAbility_bowtotheratking());
                }
            }
        }
        public override void OnRoundStart()
        {
            base.OnRoundStart();
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList())
            {
                if (!battleUnitModel.passiveDetail.HasPassive<PassiveAbility_bowtotheratking>())
                {
                    battleUnitModel.passiveDetail.AddPassive(new PassiveAbility_bowtotheratking());
                }
            }
        }
        public override void OnRoundEndTheLast()
        {
            base.OnRoundEndTheLast();
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList())
            {
                if (!battleUnitModel.passiveDetail.HasPassive<PassiveAbility_bowtotheratking>())
                {
                    battleUnitModel.passiveDetail.AddPassive(new PassiveAbility_bowtotheratking());
                }
            }
        }
    }
    public class BattleDiceCardBuf_ratkinglaugh : BattleDiceCardBuf
    {
        public override void OnUseCard(BattleUnitModel owner)
        {
            base.OnUseCard(owner);
            SingletonBehavior<SoundEffectManager>.Instance.PlayClip("Battle/Oswald_Attract", false, 1f, null);
        }
        public override void OnRoundEnd() => Destroy();
    }
    public class DiceCardSelfAbility_beats_overload : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] Restore 1 Light and gain 4 Charge";
        public override void OnUseCard()
        {
            base.OnUseCard();
            base.owner.cardSlotDetail.RecoverPlayPointByCard(1);
            base.owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.WarpCharge, 4, null);
        }
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Energy_Keyword",
                "WarpCharge"
                };
            }
        }
    }
    public class PassiveAbility_bowtotheratking : PassiveAbilityBase
    {
        public override void Init(BattleUnitModel self)
        {
            base.Init(self);
            Hide();
        }
        public override void OnStartBattle()
        {
            foreach (BattlePlayingCardDataInUnitModel battlePlayingCardDataInUnitModel in base.owner.cardSlotDetail.cardAry)
            {
                BattleUnitModel target = battlePlayingCardDataInUnitModel?.target;
                if (target != null && RandomUtil.valueForProb < 0.5f && target.passiveDetail.PassiveList.Exists((PassiveAbilityBase y) => y is PassiveAbility_ratking) && battlePlayingCardDataInUnitModel != null && (battlePlayingCardDataInUnitModel.card.XmlData.Spec.Ranged == CardRange.Far || battlePlayingCardDataInUnitModel.card.XmlData.Spec.Ranged == CardRange.Near || battlePlayingCardDataInUnitModel.card.XmlData.Spec.Ranged == CardRange.Special))
                {
                    List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(false);
                    List<BattleUnitModel> list = new List<BattleUnitModel>();
                    foreach (BattleUnitModel battleUnitModel in aliveList)
                    {
                        if (battleUnitModel != null && battleUnitModel != base.owner && battleUnitModel.speedDiceResult.Count > 0 && battleUnitModel.IsTargetable(base.owner))
                        {
                            list?.Add(battleUnitModel);
                            list?.Remove(target);
                        }
                    }
                    if (list.Count > 0 && list != null)
                    {
                        battlePlayingCardDataInUnitModel.card.AddBuf(new BattleDiceCardBuf_ratkinglaugh());
                        BattleUnitModel battleUnitModel2 = list[RandomUtil.Range(0, list.Count)];
                        battlePlayingCardDataInUnitModel.target = battleUnitModel2;
                        battlePlayingCardDataInUnitModel.targetSlotOrder = RandomUtil.Range(0, battleUnitModel2.speedDiceResult.Count);
                        this.owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Weak, 1, base.owner);
                        this.owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Disarm, 1, base.owner);
                        this.owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.DmgUp, 5, base.owner);
                    }
                }
            }
        }
    }
    public class ratsforallcounterdiegiver : PassiveAbilityBase
    {
        public override void Init(BattleUnitModel self)
        {
            base.Init(self);
            Hide();
        }
        public override void OnStartBattle()
        {
            DiceCardXmlInfo cardItem = ItemXmlDataList.instance.GetCardItem(new LorId("TurbulenceOffice", 86), false);
            new DiceBehaviour();
            List<BattleDiceBehavior> list = new List<BattleDiceBehavior>();
            int num = 0;
            foreach (DiceBehaviour diceBehaviour in cardItem.DiceBehaviourList)
            {
                BattleDiceBehavior battleDiceBehavior = new BattleDiceBehavior();
                battleDiceBehavior.behaviourInCard = diceBehaviour.Copy();
                battleDiceBehavior.SetIndex(num++);
                list.Add(battleDiceBehavior);
            }
            this.owner.cardSlotDetail.keepCard.AddBehaviours(cardItem, list);
        }
    }
    public class PassiveAbility_Rats_for_All : PassiveAbilityBase
    {
        public override void OnDie()
        {
            base.OnDie();
            foreach (BattleUnitModel unit in BattleObjectManager.instance.GetAliveList(this.owner.faction))
            {
                if (unit.passiveDetail.HasPassive<ratsforallcounterdiegiver>())
                {
                    unit.passiveDetail.DestroyPassive(new ratsforallcounterdiegiver());
                }
            }
        }
        public override void OnWaveStart()
        {
            base.OnWaveStart();
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(this.owner.faction))
            {
                if (!battleUnitModel.passiveDetail.HasPassive<ratsforallcounterdiegiver>())
                {
                    battleUnitModel.passiveDetail.AddPassive(new ratsforallcounterdiegiver());
                }
            }
        }
        public override void OnRoundStart()
        {
            base.OnRoundStart();
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(this.owner.faction))
            {
                if (!battleUnitModel.passiveDetail.HasPassive<ratsforallcounterdiegiver>())
                {
                    battleUnitModel.passiveDetail.AddPassive(new ratsforallcounterdiegiver());
                }
            }
        }
        public override void OnRoundEndTheLast()
        {
            base.OnRoundEndTheLast();
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(this.owner.faction))
            {
                if (!battleUnitModel.passiveDetail.HasPassive<ratsforallcounterdiegiver>())
                {
                    battleUnitModel.passiveDetail.AddPassive(new ratsforallcounterdiegiver());
                }
            }
        }
        public override void OnAddKeywordBufByCardForEvent(KeywordBuf keywordBuf, int stack, BufReadyType readyType)
        {
            List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList();
            aliveList.Remove(this.owner);
            if (aliveList.Count > 0)
            {
                if (readyType == BufReadyType.ThisRound)
                {
                    foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance?.GetAliveList())
                    {
                        if (battleUnitModel != this.owner && !battleUnitModel.IsDead())
                        {
                            battleUnitModel?.bufListDetail.AddKeywordBufThisRoundByEtc(keywordBuf, stack, null);
                        }
                    }
                }
                if (readyType == BufReadyType.NextRound)
                {
                    foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance?.GetAliveList())
                    {
                        if (battleUnitModel != this.owner && !battleUnitModel.IsDead())
                        {
                            battleUnitModel?.bufListDetail.AddKeywordBufByEtc(keywordBuf, stack, null);
                        }
                    }
                }
            }
        }
    }
    public class DiceCardSelfAbility_malkuthcrescendo : DiceCardSelfAbilityBase
    {
        public override bool OnChooseCard(BattleUnitModel owner)
        {
            return Singleton<StageController>.Instance.RoundTurn >= 3 && base.OnChooseCard(owner);
        }
        public override void OnUseCard()
        {
            base.OnUseCard();
            this.card.card.exhaust = true;
            this.owner.allyCardDetail.AddNewCard(new LorId("TurbulenceOffice", 18));
            BattleUnitBuf buf = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_malkuthcrescentscuffer);
            if (buf == null)
            {
                this.owner.bufListDetail.AddBuf(new BattleUnitBuf_malkuthcrescentscuffer());
            }
        }
        public override void OnEndBattle()
        {
            base.OnEndBattle();
            BattleUnitBuf_malkuthcrescentscuffer buf = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_malkuthcrescentscuffer) as BattleUnitBuf_malkuthcrescentscuffer;
            if (buf != null)
            {
                foreach (BattleUnitModel impostor in BattleObjectManager.instance.GetAliveList_opponent(this.owner.faction))
                {
                    BattleUnitBuf among = impostor.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_malkuthcrescentscuffer2);
                    if (among != null && impostor != null)
                    {
                        for (int i = 0; i < buf.sussyimpostor; i++)
                        {
                            impostor.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Burn, 1, this.owner);
                        }
                        impostor.bufListDetail.RemoveBuf(new BattleUnitBuf_malkuthcrescentscuffer2());
                    }
                }
                buf.sussyimpostor = 0;
                this.owner.bufListDetail.RemoveBuf(new BattleUnitBuf_malkuthcrescentscuffer());
            }
        }
        public static string Desc = "Usable on and after the third Scene\n[On Use] Add a copy of 'Grand Malkuth' to hand";
    }
    public class BattleUnitBuf_malkuthcrescentscuffer : BattleUnitBuf
    {
        public int sussyimpostor;
        public override void OnRoundEnd()
        {
            base.OnRoundEnd();
            this.Destroy();
        }
    }
    public class BattleUnitBuf_malkuthcrescentscuffer2 : BattleUnitBuf
    {
        public override void OnRoundEnd()
        {
            base.OnRoundEnd();
            this.Destroy();
        }
    }
    public class DiceCardAbility_malkscendo : DiceCardAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Burn_Keyword"
                };
            }
        }
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            base.OnSucceedAttack(target);
            target?.bufListDetail.AddBuf(new BattleUnitBuf_malkuthcrescentscuffer2());
            BattleUnitBuf_malkuthcrescentscuffer buf = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_malkuthcrescentscuffer) as BattleUnitBuf_malkuthcrescentscuffer;
            if (buf != null)
            {
                buf.sussyimpostor++;
            }
        }
        public static string Desc = "[On Hit] Inflict 1 Burn for every target hit by this die this Scene";
    }
    public class DiceCardAbility_malkfinale : DiceCardAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Burn_Keyword"
                };
            }
        }
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            base.OnSucceedAttack(target);
            BattleUnitBuf burn = target.bufListDetail.GetActivatedBuf(KeywordBuf.Burn);
            if (burn != null && burn.stack >= 1 && target != null)
            {
                burn.stack = target.bufListDetail.GetKewordBufStack(KeywordBuf.Burn) * 8;
            }
        }
        public static string Desc = "[On Hit] Octuple target's Burn";
    }
    public class DiceCardSelfAbility_futuregraspersieghart : DiceCardSelfAbilityBase
    {
        public override bool IsUniteCard
        {
            get
            {
                return true;
            }
        }
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Energy_Keyword",
                "Protection_Keyword"
                };
            }
        }
        public override void OnStartBattle()
        {
            base.OnStartBattle();
            this.owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Protection, 1, this.owner);
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(base.owner.faction))
            {
                if (battleUnitModel != base.owner && !battleUnitModel.bufListDetail.HasBuf<BattleUnitBuf_graspedfuture>())
                {
                    battleUnitModel.bufListDetail.AddBuf(new BattleUnitBuf_graspedfuture());
                }
            }
        }
        public class BattleUnitBuf_graspedfuture : BattleUnitBuf
        {
            public override void OnUseCard(BattlePlayingCardDataInUnitModel card)
            {
                if (card.cardAbility != null && card.cardAbility.IsUniteCard)
                {
                    this._owner.allyCardDetail.DrawCards(1);
                }
            }

            public override void OnRoundEnd()
            {
                this.Destroy();
            }
        }
        public override void OnUseCard()
        {
            base.OnUseCard();
            this.owner.cardSlotDetail.RecoverPlayPointByCard(1);
        }
        public static string Desc = "[Unity]\n[Combat Start] Gain 1 Protection this Scene. For this Scene, all other allies' Unity pages gain '[On Use] Draw 1 page' (Cannot be stacked)\n[On Use] Restore 1 Light";
    }

    public class DiceCardAbility_ridethelightning2 : DiceCardAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Paralysis_Keyword"
                };
            }
        }
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            base.OnSucceedAttack(target);
            target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Paralysis, 1, this.owner);
            target.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Paralysis, 1, this.owner);
        }
        public static string Desc = "[On Hit] Inflict 1 Paralysis this Scene and next Scene";
    }
    public class DiceCardAbility_ridethelightning : DiceCardAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Paralysis_Keyword"
                };
            }
        }
        public override void BeforeRollDice()
        {
            base.BeforeRollDice();
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                power = behavior.card.target.bufListDetail.GetKewordBufStack(KeywordBuf.Paralysis) / 2
            });

        }
        public static string Desc = "Add Power equal to half the Paralysis on target";
    }
    public class DiceCardAbility_incredibleimixedupthenamestrulyamazing : DiceCardAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Energy_Keyword",
                "Smoke_Keyword"
                };
            }
        }
        public static string Desc = "[On Hit] Gain 2 Smoke and restore 1 Light";
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            base.OnSucceedAttack(target);
            if (target != null)
            {
                this.owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Smoke, 2, base.owner);
                base.owner.cardSlotDetail.RecoverPlayPointByCard(2);
            }
        }
    }
    public class DiceCardSelfAbility_cannonblow : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "DrawCard_Keyword",
                "Smoke_Keyword"
                };
            }
        }
        public override void OnUseCard()
        {
            base.OnUseCard();
            this.owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Smoke, 3, this.owner);
            base.owner.allyCardDetail.DrawCards(1);
        }
        public static string Desc = "[On Use] Gain 3 Smoke and draw 1 page";
    }
    public class DiceCardSelfAbility_rangedescapethedepths : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Smoke_Keyword"
                };
            }
        }
        public override void OnUseCard()
        {
            base.OnUseCard();
            BattleUnitBuf among = this.owner.bufListDetail.GetActivatedBuf(KeywordBuf.Smoke);
            this.card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = among.stack / 2
            });
        }
        public static string Desc = "[On Use] Dice on this page gain Power equal to half the amount of Smoke on self (Rounded down)";
    }
    public class DiceCardSelfAbility_smokebreakdraw : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "DrawCard_Keyword",
                "Smoke_Keyword"
                };
            }
        }

        public override void OnUseCard()
        {
            base.OnUseCard();
            BattleUnitModel target = this.card.target;
            if (target != null)
            {
                BattleUnitBuf activatedbuf = target.bufListDetail.GetActivatedBuf(KeywordBuf.Smoke);
                if (activatedbuf != null && activatedbuf.stack >= 4)
                {
                    base.owner.allyCardDetail.DrawCards(1);
                }
            }
        }
        public static string Desc = "[On Use] If target has 4 or more Smoke, draw 1 page";
    }

    public class NoReduceBleed : BattleUnitBuf
    {
        public override void OnEndBattle(BattlePlayingCardDataInUnitModel curCard)
        {
            this.Destroy();
            base.OnEndBattle(curCard);
        }
    }
    public class MortalBleed : BattleUnitBuf
    {
        public override BufPositiveType positiveType => BufPositiveType.Negative;
        protected override string keywordId
        {
            get
            {
                return "MortalBleedID";
            }
        }

        public override void AfterDiceAction(BattleDiceBehavior behavior)
        {

            if (behavior.Type == BehaviourType.Atk)
            {
                if (!this._owner.bufListDetail.HasBuf<NoReduceBleed>())
                {
                    if (this._owner.passiveDetail.HasPassive<PassiveAbility_HemoRecov>())
                    {
                        this._owner.TakeDamage(this.stack / 2, DamageType.Buf, null);
                    }
                    else
                    {
                        this._owner.TakeDamage(this.stack, DamageType.Buf, null);
                    }

                }

                BattleCardTotalResult battleCardResultLog = this._owner.battleCardResultLog;
                if (battleCardResultLog != null)
                {
                    battleCardResultLog.SetAfterActionEvent(new BattleCardBehaviourResult.BehaviourEvent(this.PrintEffect));
                }


                float temp = stack * 0.33f;
                stack = Mathf.FloorToInt(stack - temp);


                if (stack == 0) { this.Destroy(); }
                base.AfterDiceAction(behavior);
            }

        }

        public MortalBleed(int stack_)
        {
            stack = stack_;
        }


        private void PrintEffect()
        {
            GameObject gameObject = Util.LoadPrefab("Battle/DiceAttackEffects/New/FX/DamageDebuff/FX_DamageDebuff_Blooding");
            if (gameObject != null)
            {
                BattleUnitModel owner = this._owner;
                if (((owner != null) ? owner.view : null) != null)
                {
                    gameObject.transform.parent = this._owner.view.camRotationFollower;
                    gameObject.transform.localPosition = Vector3.zero;
                    gameObject.transform.localScale = Vector3.one;
                    gameObject.transform.localRotation = Quaternion.identity;
                }
            }
            //Add Bleed Sound
            SoundEffectPlayer.PlaySound("Buf/Effect_Bleeding");
        }
    }
    public static class HandleHemokinesis
    {
        public static void AddHemo(int stack_, BattleUnitModel targ)
        {
            if (!targ.bufListDetail.HasBuf<HemoBlock>())
            {
                var buf = targ.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is Hemokinesis) as Hemokinesis;
                if (buf == null)
                {
                    targ.bufListDetail.AddBuf(new Hemokinesis(stack_));
                }
                else
                {
                    buf.stack += stack_;
                }
                var test = targ.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is Hemokinesis) as Hemokinesis;
                if (test != null)
                {
                    if (test.stack >= 10)
                    {
                        test.stack = 10;
                        test.ActivateThingy();
                    }
                }
            }

        }

        public static void AddMortalBleed(int stack_, BattleUnitModel targ)
        {
            var buf = targ.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is MortalBleed) as MortalBleed;
            if (buf == null)
            {
                targ.bufListDetail.AddBuf(new MortalBleed(stack_));
            }
            else
            {
                buf.stack += stack_;
            }


        }
    }
    public class Hemokinesis : BattleUnitBuf
    {
        public Hemokinesis(int xo)
        {
            stack = xo;
        }


        protected override string keywordId
        {
            get
            {
                return "HemokinesisID";
            }
        }
        public override BufPositiveType positiveType => BufPositiveType.Negative;
        public void ActivateThingy()
        {
            if (!_owner.bufListDetail.HasBuf<HemoBlock>())
            {
                _owner.bufListDetail.AddBuf(new HemoBlock());
                int fifteen = (int)(_owner.MaxHp * 0.30f);
                _owner.TakeDamage(fifteen);
                BattleCardTotalResult battleCardResultLog = this._owner.battleCardResultLog;
                if (battleCardResultLog != null)
                {
                    battleCardResultLog.SetAfterActionEvent(new BattleCardBehaviourResult.BehaviourEvent(this.PrintEffect));
                }
                RestoreHpTrigger();
                this.Destroy();
            }

        }
        public void RestoreHpTrigger()
        {
            List<BattleUnitModel> allfuckers = BattleObjectManager.instance.GetAliveList().FindAll((BattleUnitModel x) => x.passiveDetail.HasPassive<PassiveAbility_HemoRecov>());
            foreach (BattleUnitModel unit in allfuckers)
            {
                var passiveo = unit.passiveDetail.PassiveList.Find((PassiveAbilityBase b) => b is PassiveAbility_HemoRecov) as PassiveAbility_HemoRecov;
                passiveo.ActivateRecov();
            }
        }
        public override void OnRoundStart()
        {
            if (!this._owner.bufListDetail.HasBuf<BattleUnitBuf_hemostasis>())
            {
                stack--;
                if (stack <= 0) { this.Destroy(); }
            }
            base.OnRoundStart();
        }
        private void PrintEffect()
        {
            GameObject gameObject = Util.LoadPrefab("Battle/DiceAttackEffects/New/FX/DamageDebuff/FX_DamageDebuff_Blooding");
            if (gameObject != null)
            {
                BattleUnitModel owner = this._owner;
                if (((owner != null) ? owner.view : null) != null)
                {
                    gameObject.transform.parent = this._owner.view.camRotationFollower;
                    gameObject.transform.localPosition = Vector3.zero;
                    gameObject.transform.localScale = Vector3.one;
                    gameObject.transform.localRotation = Quaternion.identity;
                }
            }
            //Add BIG bleed
            SoundEffectPlayer.PlaySound("Buf/Effect_Bleeding");
        }
    }
    public class HemoBlock : BattleUnitBuf
    {
        int cooldown = 2;
        public override void OnRoundStart()
        {

            cooldown--;
            if (cooldown <= 0) { this.Destroy(); }
            var boofo = _owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is Hemokinesis);
            if (boofo != null) { boofo.Destroy(); }
            base.OnRoundStart();
        }

        public override bool IsImmune(BattleUnitBuf buf)
        {
            if (buf is Hemokinesis)
            {
                return true;
            }
            return false;
        }
    }
    //Cards

    //Ready Blade
    public class DiceCardSelfAbility_MoveDice : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use:] Triple Mortal Bleed on self. Restore 1 Light and Draw 1 Page";
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "MortalBleed_keyword",
                "Energy_Keyword",
                "DrawCard_Keyword"
                };
            }

        }
        public override void OnUseCard()
        {
            owner.cardSlotDetail.RecoverPlayPoint(1);
            owner.allyCardDetail.DrawCards(1);
            var bleedin = owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is MortalBleed);
            if (bleedin != null)
            {
                bleedin.stack = bleedin.stack * 3;
            }
        }

    }
    public class DiceCardAbility_3Bleed : DiceCardAbilityBase
    {
        public static string Desc = "[On Hit:] Inflict 3 MortalBleed";
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "MortalBleed_keyword"
                };
            }

        }
        public override void OnSucceedAttack()
        {
            HandleHemokinesis.AddMortalBleed(3, card.target);

            base.OnSucceedAttack();
        }
    }
    //Rend My Flesh
    public class DiceCardSelfAbility_Fleshy : DiceCardSelfAbilityBase
    {
        public static string Desc = "[The cost of this page is reduced by stack of Mortal Bleed on self.]\n[On Use:] Draw 2 Pages. If the cost of this page is 0. Draw 2 pages and restore 3 light instead.";
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "MortalBleed_keyword",
                "Energy_Keyword",
                "DrawCard_Keyword"
                };
            }

        }
        public override int GetCostAdder(BattleUnitModel unit, BattleDiceCardModel self)
        {
            var bleedin = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is MortalBleed);
            if (bleedin != null)
            {
                return -bleedin.stack;
            }
            return base.GetCostAdder(unit, self);
        }

        public override void OnUseCard()
        {
            if (card.card.CurCost <= 0)
            {
                owner.allyCardDetail.DrawCards(2);
                owner.cardSlotDetail.RecoverPlayPoint(3);
            }
            else
            {
                owner.allyCardDetail.DrawCards(2);
            }
            base.OnUseCard();
        }
    }
    public class DiceCardAbility_Repeato : DiceCardAbilityBase
    {
        public static string Desc = "This die recycles 2 times, Deal slash damage to self equal to double the vanilla roll of this die.\n[On Hit:] Inflict 1 Hemokinesis";
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Hemokinesis_keyword"
                };
            }

        }
        int count = 2;
        public override void OnRollDice()
        {

            float resisto = BookModel.GetResistRate(owner.GetResistHP(LOR_DiceSystem.BehaviourDetail.Slash));
            int fuck = (int)((behavior.DiceVanillaValue * 2) * resisto);
            owner.TakeDamage(fuck);
            base.OnRollDice();

        }
        public override void OnSucceedAttack()
        {
            HandleHemokinesis.AddHemo(1, card.target);
            count--;
            if (count > 0)
            {
                ActivateBonusAttackDice();
            }
            base.OnSucceedAttack();
        }
        public override void OnDrawParrying()
        {
            count--;
            if (count > 0)
            {
                ActivateBonusAttackDice();
            }
            base.OnDrawParrying();
        }
        public override void OnLoseParrying()
        {
            count--;
            if (count > 0)
            {
                ActivateBonusAttackDice();
            }
            base.OnLoseParrying();
        }
    }
    //Hemoulus Wall
    public class DiceCardAbility_BlockIncrease : DiceCardAbilityBase
    {
        public static string Desc = "Increase Min and Max roll by amount of MortalBleed on self (Max 7)";
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "MortalBleed_keyword"
                };
            }

        }
        public override void BeforeRollDice()
        {
            var bleedin = owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is MortalBleed);
            if (bleedin != null)
            {
                int fug = bleedin.stack;
                if (fug > 7) { fug = 7; }
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    min = fug,
                    max = fug,
                });
            }
            base.BeforeRollDice();
        }
    }
    public class DiceCardAbility_Apply1Hemo : DiceCardAbilityBase
    {
        public static string Desc = "[On Hit:] Inflict 1 Hemokinesis";
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Hemokinesis_keyword"
                };
            }

        }
        public override void OnSucceedAttack()
        {
            HandleHemokinesis.AddHemo(1, card.target);
            base.OnSucceedAttack();
        }
    }
    public class DiceCardAbility_Apply2Hemo : DiceCardAbilityBase
    {
        public static string Desc = "[On Hit:] Inflict 2 Hemokinesis";
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Hemokinesis_keyword"
                };
            }

        }
        public override void OnSucceedAttack()
        {
            HandleHemokinesis.AddHemo(2, card.target);
            base.OnSucceedAttack();
        }
    }
    public class DiceCardAbility_Apply2HemoArea : DiceCardAbilityBase
    {
        public static string Desc = "[On Hit:] Inflict 2 Hemokinesis";
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Hemokinesis_keyword"
                };
            }

        }
        public override void OnSucceedAreaAttack(BattleUnitModel target)
        {
            HandleHemokinesis.AddHemo(2, target);

            base.OnSucceedAreaAttack(target);
        }
    }
    public class DiceCardAbility_Apply10Hemo : DiceCardAbilityBase
    {
        public static string Desc = "[On Hit:] Inflict 10 Hemokinesis";
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Hemokinesis_keyword"
                };
            }

        }
        public override void OnSucceedAttack()
        {
            HandleHemokinesis.AddHemo(10, this.card.target);
            base.OnSucceedAttack();
        }
    }
    //Utilize Blood
    public class DiceCardSelfAbility_DrawinBlood : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use:] Draw 1 Page, Remove 3 MortalBleed and take 6 Slash damage to restore 2 light. If target has Hemokinesis, add 2 to Its stack.";
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "MortalBleed_keyword",
                "Hemokinesis_keyword",
                "Energy_Keyword",
                "DrawCard_Keyword"
                };
            }

        }
        public override void OnUseCard()
        {
            owner.allyCardDetail.DrawCards(1);
            var bleedin = owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is MortalBleed);
            if (bleedin != null)
            {
                if (bleedin.stack >= 3)
                {
                    bleedin.stack -= 3;
                    float resisto = BookModel.GetResistRate(owner.GetResistHP(LOR_DiceSystem.BehaviourDetail.Slash));
                    owner.TakeDamage((int)(6 * resisto));
                    owner.cardSlotDetail.RecoverPlayPoint(2);
                }
            }
            var boofo = card.target.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is Hemokinesis) as Hemokinesis;
            if (boofo != null)
            {
                HandleHemokinesis.AddHemo(2, card.target);
            }
            base.OnUseCard();
        }
    }
    //Hemomancy Blitz
    public class DiceCardSelfAbility_BigLight : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use:] If target has Hemokinesis, Restore 1 light. Target's Hemokinesis does not reduce next Scene.  Dice on this page lose 2 power.";
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "Hemokinesis_keyword",
                "Energy_Keyword"
                };
            }

        }
        public override void OnUseCard()
        {
            if (card.target.bufListDetail.HasBuf<Hemokinesis>())
            {
                owner.cardSlotDetail.RecoverPlayPoint(1);
                card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
                {
                    power = -2
                });
            }
            BattleUnitBuf_hemostasis stasis = card.target.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_hemostasis) as BattleUnitBuf_hemostasis;
            if (stasis == null)
            {
                card.target.bufListDetail.AddBuf(new BattleUnitBuf_hemostasis());
            }
            if (stasis != null)
            {
                stasis.amongus = false;
            }
            base.OnUseCard();
        }
    }
    public class BattleUnitBuf_hemostasis : BattleUnitBuf
    {
        public bool amongus;
        public override void OnRoundStart()
        {
            base.OnRoundStart();
            amongus = true;
        }
        public override void OnRoundEnd()
        {
            base.OnRoundEnd();
            if (amongus)
            {
                this.Destroy();
            }
        }
    }
    public class DiceCardSelfAbility_skyfalling : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "DrawCard_Keyword"
                };
            }

        }
        public override void OnUseCard()
        {
            this.owner.allyCardDetail.DrawCards(1);
            this._isBreakedStart = false;
            if (this.card.target != null && this.card.target.IsBreakLifeZero())
            {
                this._isBreakedStart = true;
            }
        }

        public override void OnEndBattle()
        {
            if (this.card.target != null && (this.card.target.IsDead() || (!this._isBreakedStart && this.card.target.IsBreakLifeZero())))
            {
                List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList((base.owner.faction == Faction.Player) ? Faction.Enemy : Faction.Player);
                if (aliveList.Count > 0)
                {
                    BattleUnitModel target = RandomUtil.SelectOne<BattleUnitModel>(aliveList);
                    Singleton<StageController>.Instance.AddAllCardListInBattle(this.card, target, -1);
                }
            }
        }
        public static string Desc = "If target is defeated or Staggered, use this page again on a random enemy.\n[On Use] Draw 1 Page";
        private bool _isBreakedStart;
    }
    //Blood Soaked Blade
    public class DiceCardSelfAbility_BoostMinMax : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use:] All Slash Dice gain +2 to Min and Max this scene. Inflict 10 MortalBleed on Self.";
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "MortalBleed_keyword"
                };
            }

        }

        public override void OnUseCard()
        {
            HandleHemokinesis.AddMortalBleed(10, owner);
            owner.bufListDetail.AddBuf(new SlashUp());
            base.OnUseCard();
        }


        public class SlashUp : BattleUnitBuf
        {
            public override void OnRoundEnd()
            {
                this.Destroy();
            }
            public override void BeforeRollDice(BattleDiceBehavior behavior)
            {
                if (behavior != null)
                {
                    if (behavior.Detail == LOR_DiceSystem.BehaviourDetail.Slash)
                    {
                        behavior.ApplyDiceStatBonus(new DiceStatBonus
                        {
                            min = 2,
                            max = 2,
                        });
                    }
                }
                base.BeforeRollDice(behavior);
            }
        }
    }
    public class DiceCardAbility_EvadeDmgUp : DiceCardAbilityBase
    {
        public static string Desc = "[On Lose Clash:] Next Die gains 5 Power.";
        public override void OnLoseParrying()
        {
            card.ApplyDiceStatBonus(DiceMatch.NextDice, new DiceStatBonus
            {
                power = 5
            });
            base.OnLoseParrying();
        }
    }
    //Blood Splatter
    public class DiceCardSelfAbility_All5Bleed : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] All characters are inflicted with 10 MortalBleed";
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "MortalBleed_keyword"
                };
            }

        }
        public override void OnUseCard()
        {
            List<BattleUnitModel> allfuckers = BattleObjectManager.instance.GetAliveList();
            foreach (BattleUnitModel model in allfuckers)
            {
                HandleHemokinesis.AddMortalBleed(10, model);
            }
            base.OnUseCard();
        }
    }
    //Yield my Desires
    public class DiceCardSelfAbility_RemoveBleed : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use:] Remove all MortalBleed on self. Taking damage equal to its stack. If removed stacks is 20 or more, Restore all Light and draw 1 page.";
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                "MortalBleed_keyword",
                "Energy_Keyword",
                "DrawCard_Keyword"
                };
            }

        }
        public override void OnUseCard()
        {
            var bleedin = owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is MortalBleed);
            if (bleedin != null)
            {
                int wow = bleedin.stack;
                bleedin.Destroy();
                if (wow >= 20) { owner.cardSlotDetail.RecoverPlayPoint(owner.cardSlotDetail.GetMaxPlayPoint()); owner.allyCardDetail.DrawCards(1); }
                owner.TakeDamage(wow);
            }
            base.OnUseCard();
        }
    }
    public class DiceCardAbility_LoseTime : DiceCardAbilityBase
    {
        public static string Desc = "[On Clash Lose:] Add “To Claim Their Blood and Desires” to users hand.";
        public override void OnLoseParrying()
        {
            owner.allyCardDetail.DisCardACardRandom();
            owner.allyCardDetail.AddNewCard(new LorId("TurbulenceOffice", 149));
            base.OnLoseParrying();
        }
    }
    //Action Scripts
    //Claim Their Blood
    public class BehaviourAction_ClaimBlood : BehaviourActionBase
    {
        List<RencounterManager.MovingAction> list = new List<RencounterManager.MovingAction>();
        public override List<RencounterManager.MovingAction> GetMovingAction(ref RencounterManager.ActionAfterBehaviour self, ref RencounterManager.ActionAfterBehaviour opponent)
        {
            if (self.result != Result.Win)
            {
                return base.GetMovingAction(ref self, ref opponent);
            }
            List<RencounterManager.MovingAction> infoList = opponent.infoList;
            if (infoList != null && infoList.Count > 0)
            {
                opponent.infoList.Clear();
            }
            bool flag = false;
            flag = opponent.behaviourResultData.IsFarAtk();
            if (true)
            {
                RencounterManager.MovingAction movingAction1 = new RencounterManager.MovingAction(ActionDetail.Guard, CharMoveState.Stop, 1f, true, 0.5f, 1f);
                movingAction1.SetEffectTiming(EffectTiming.PRE, EffectTiming.NONE, EffectTiming.NONE);
                movingAction1.customEffectRes = "Kimsatgat_S2";
                RencounterManager.MovingAction movingAction2 = new RencounterManager.MovingAction(ActionDetail.S2, CharMoveState.Stop, 1f, true, 0.5f, 1f);
                movingAction2.SetEffectTiming(EffectTiming.PRE, EffectTiming.NONE, EffectTiming.NONE);
                RencounterManager.MovingAction movingAction3 = new RencounterManager.MovingAction(ActionDetail.S1, CharMoveState.Stop, 1f, true, .5f, 1f);
                movingAction3.SetEffectTiming(EffectTiming.PRE, EffectTiming.NONE, EffectTiming.NONE);
                movingAction3.customEffectRes = "Kimsatgat_S1";

                RencounterManager.MovingAction movingAction4 = new RencounterManager.MovingAction(ActionDetail.Slash, CharMoveState.TeleportBack, 1f, true, .7f, 1f);
                movingAction4.SetEffectTiming(EffectTiming.PRE, EffectTiming.NONE, EffectTiming.NONE);
                movingAction4.dstDistance = 5f;
                movingAction4.customEffectRes = "Kimsatgat_S3";

                RencounterManager.MovingAction movingAction5 = new RencounterManager.MovingAction(ActionDetail.Slash, CharMoveState.TeleportBack, 1f, true, .7f, 1f);
                movingAction5.SetEffectTiming(EffectTiming.PRE, EffectTiming.NONE, EffectTiming.NONE);
                movingAction5.dstDistance = 5f;
                movingAction5.customEffectRes = "Kimsatgat_S3";

                RencounterManager.MovingAction movingAction6 = new RencounterManager.MovingAction(ActionDetail.S1, CharMoveState.TeleportBack, 1f, false, .5f, 1f);
                movingAction6.SetEffectTiming(EffectTiming.PRE, EffectTiming.NONE, EffectTiming.NONE);
                movingAction6.dstDistance = 6f;
                RencounterManager.MovingAction movingAction7 = new RencounterManager.MovingAction(ActionDetail.S2, CharMoveState.Stop, 1f, false, 0.3f, 1f);
                movingAction7.SetEffectTiming(EffectTiming.PRE, EffectTiming.NONE, EffectTiming.NONE);
                RencounterManager.MovingAction movingAction8 = new RencounterManager.MovingAction(ActionDetail.S2, CharMoveState.Stop, 1f, true, 0.8f, 1f);
                movingAction8.SetEffectTiming(EffectTiming.PRE, EffectTiming.PRE, EffectTiming.PRE);
                movingAction8.customEffectRes = "FX_PC_RolRang_ShadowSlsah";

                list.Add(movingAction1);
                list.Add(movingAction2);
                list.Add(movingAction3);
                list.Add(movingAction4);
                list.Add(movingAction5);
                list.Add(movingAction6);
                list.Add(movingAction7);
                list.Add(movingAction8);

                // RencounterManager.MovingAction movingAction2 = new RencounterManager.MovingAction(ActionDetail.Penetrate, CharMoveState.MoveForward, 20f, false, .1f, 1.5f);
                //movingAction2.SetEffectTiming(EffectTiming.PRE, EffectTiming.PRE, EffectTiming.PRE);
                //movingAction2.customEffectRes = "BlackSilence_Z";
                //list.Add(movingAction2);
                ActionDetail actionDetail2 = ActionDetail.Hit;
                if (opponent.behaviourResultData != null && opponent.behaviourResultData.behaviourRawData != null)
                {
                    actionDetail2 = MotionConverter.MotionToAction(opponent.behaviourResultData.behaviourRawData.MotionDetail);
                }
                RencounterManager.MovingAction movingAction69 = new RencounterManager.MovingAction(ActionDetail.Move, CharMoveState.Stop, 1f, true, .1f, 1f);
                movingAction69.customEffectRes = "None";
                movingAction69.SetEffectTiming(EffectTiming.NOT_PRINT, EffectTiming.NOT_PRINT, EffectTiming.NOT_PRINT);
                RencounterManager.MovingAction movingAction70 = new RencounterManager.MovingAction(actionDetail2, CharMoveState.Stop, 1f, true, 1f, 1f);
                movingAction70.customEffectRes = "None";
                movingAction70.SetEffectTiming(EffectTiming.NOT_PRINT, EffectTiming.NOT_PRINT, EffectTiming.NOT_PRINT);
                RencounterManager.MovingAction movingAction71 = new RencounterManager.MovingAction(ActionDetail.Damaged, CharMoveState.Stop, 1f, true, 0.5f, 1f);
                movingAction71.customEffectRes = "None";
                movingAction71.SetEffectTiming(EffectTiming.NOT_PRINT, EffectTiming.NOT_PRINT, EffectTiming.NOT_PRINT);
                opponent.infoList.Add(movingAction69);
                opponent.infoList.Add(movingAction70);
                opponent.infoList.Add(movingAction71);
            }
            else
            {
                return base.GetMovingAction(ref self, ref opponent);


            }
            return list;
        }
    }
    //Evade Moment
    public class BehaviourAction_EvadeMoment : BehaviourActionBase
    {
        BattleUnitModel impostor;
        List<RencounterManager.MovingAction> list = new List<RencounterManager.MovingAction>();
        public override List<RencounterManager.MovingAction> GetMovingAction(ref RencounterManager.ActionAfterBehaviour self, ref RencounterManager.ActionAfterBehaviour opponent)
        {
            if (self.result != Result.Win)
            {
                return base.GetMovingAction(ref self, ref opponent);
            }
            List<RencounterManager.MovingAction> infoList = opponent.infoList;
            if (infoList != null && infoList.Count > 0)
            {
                opponent.infoList.Clear();
            }
            if (!opponent.behaviourResultData.IsFarAtk() && self.result == Result.Win)
            {
                this.impostor = self.view.model;
                BattleUnitBuf among = impostor.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is MortalBleed);
                if (among != null)
                {
                    int stacko = impostor.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is MortalBleed).stack;
                    if (stacko >= 5)
                    {
                        ActionDetail actionDetail = ActionDetail.Evade;
                        if (self.view.charAppearance.GetCharacterMotion(ActionDetail.S3) != null)
                        {
                            actionDetail = ActionDetail.S3;
                        }
                        RencounterManager.MovingAction movingAction1 = new RencounterManager.MovingAction(actionDetail, CharMoveState.MoveForward, 15f, true, 0.5f, 1f);
                        movingAction1.SetEffectTiming(EffectTiming.NONE, EffectTiming.NONE, EffectTiming.NONE);
                        list.Add(movingAction1);
                    }
                }
                else
                {
                    RencounterManager.MovingAction movingAction1 = new RencounterManager.MovingAction(ActionDetail.Evade, CharMoveState.MoveBack, 14f, false, 0.5f, 1f);
                    movingAction1.SetEffectTiming(EffectTiming.NONE, EffectTiming.NONE, EffectTiming.NONE);
                    list.Add(movingAction1);
                }
            }
            return list;
        }
    }
    //Yield
    public class BehaviourAction_YieldDesire : BehaviourActionBase
    {
        List<RencounterManager.MovingAction> list = new List<RencounterManager.MovingAction>();

        public override List<RencounterManager.MovingAction> GetMovingAction(ref RencounterManager.ActionAfterBehaviour self, ref RencounterManager.ActionAfterBehaviour opponent)
        {
            if (self.result != Result.Win)
            {
                RencounterManager.MovingAction movingAction1 = new RencounterManager.MovingAction(ActionDetail.S2, CharMoveState.Stop, 1f, true, 0.5f, 1f);
                movingAction1.SetEffectTiming(EffectTiming.NONE, EffectTiming.NONE, EffectTiming.NONE);
                list.Add(movingAction1);
            }
            else
            {
                RencounterManager.MovingAction movingAction1 = new RencounterManager.MovingAction(ActionDetail.Hit, CharMoveState.Stop, 1f, true, 0.3f, 1f);
                movingAction1.SetEffectTiming(EffectTiming.PRE, EffectTiming.NONE, EffectTiming.NONE);
                RencounterManager.MovingAction movingAction2 = new RencounterManager.MovingAction(ActionDetail.Slash, CharMoveState.Stop, 1f, true, 0.4f, 1f);
                movingAction2.SetEffectTiming(EffectTiming.PRE, EffectTiming.PRE, EffectTiming.PRE);
                list.Add(movingAction1);
                list.Add(movingAction2);

                RencounterManager.MovingAction movingAction71 = new RencounterManager.MovingAction(ActionDetail.Damaged, CharMoveState.Stop, 1f, true, .3f, 1f);
                movingAction71.customEffectRes = "None";
                movingAction71.SetEffectTiming(EffectTiming.NOT_PRINT, EffectTiming.NOT_PRINT, EffectTiming.NOT_PRINT);
                opponent.infoList.Add(movingAction71);
            }






            return list;
        }
    }
    public class BehaviourAction_BloodTime : BehaviourActionBase
    {
        public override FarAreaEffect SetFarAreaAtkEffect(BattleUnitModel self)
        {
            this._self = self;
            FarAreaEffect_BloodSplatter pleasework = new GameObject().AddComponent<FarAreaEffect_BloodSplatter>();
            pleasework.Init(self, Array.Empty<object>());
            return pleasework;


        }
    }
    public class FarAreaEffect_BloodSplatter : FarAreaEffect
    {
        public override void Init(BattleUnitModel self, params object[] args)
        {
            base.Init(self, args);
            this.state = EffectState.Start;
            this.Substate = SubState.standstill;
            Singleton<BattleFarAreaPlayManager>.Instance.SetActionDelay(0f, 0.5f);
            this._beforeMotion = ActionDetail.Default;

        }
        protected override void Update()
        {
            if (this.state == EffectState.Start)
            {
                if (this.Substate == SubState.standstill)
                {
                    _elapsed += Time.deltaTime;
                    _self.view.charAppearance.ChangeMotion(ActionDetail.S2);
                    if (ActiveOnce == false)
                    {
                        ActiveOnce = true;
                        SingletonBehavior<DiceEffectManager>.Instance.CreateNewFXCreatureEffect("0_K/FX_IllusionCard_0_K_Heart", 1f, _self.view, _self.view, 3f);
                    }
                    if (_elapsed >= 1.0f)
                    {
                        Substate = SubState.charge;
                        ActiveOnce = false;
                        _elapsed = 0.0f;
                    }
                }
                else if (this.Substate == SubState.charge)
                {
                    _self.view.charAppearance.ChangeMotion(ActionDetail.S1);
                    _elapsed += Time.deltaTime;
                    if (_elapsed >= 0.5f)
                    {
                        Substate = SubState.ready;
                        ActiveOnce = false;
                        _elapsed = 0.0f;
                    }
                }
                else if (this.Substate == SubState.ready)
                {
                    _self.view.charAppearance.ChangeMotion(ActionDetail.Move);
                    if (ActiveOnce == false)
                    {
                        ActiveOnce = true;
                        _self.moveDetail.Move(Vector3.zero, 200f, true, false);
                    }
                    if (ActiveOnce == true)
                    {
                        if (_self.moveDetail.isArrived)
                        {
                            this.state = EffectState.GiveDamage;
                            ActiveOnce = false;
                            _elapsed = 0.0f;
                        }
                    }
                }
            }
            else if (this.state == EffectState.GiveDamage)
            {
                _elapsed += Time.deltaTime;
                if (_elapsed >= 0.1f)
                {
                    this._self.view.charAppearance.ChangeMotion(ActionDetail.Slash);

                    if (ActiveOnce == false)
                    {
                        ActiveOnce = true;

                        Util.LoadPrefab("Battle/SpecialEffect/Gloria_BloodFilterParticle");
                    }


                    StartCoroutine(BleedSound());

                    _elapsed = 0.0f;
                    this.isRunning = false;
                    this.state = EffectState.End;

                }
            }
            else if (this.state == EffectState.End)
            {
                _elapsed += Time.deltaTime;
                if (_elapsed >= 2.0f)
                {
                    this._self.view.charAppearance.ChangeMotion(this._beforeMotion);
                    this._self.moveDetail.ReturnToFormationByBlink(false);
                    this.state = FarAreaEffect.EffectState.None;
                    this._elapsed = 0.0f;
                }

            }
            else if (this.state == EffectState.None && this._self.view.FormationReturned)
            {
                this._isDoneEffect = true;
                UnityEngine.Object.Destroy(base.gameObject);
            }
        }
        private float _elapsed;
        private ActionDetail _beforeMotion;
        private SubState Substate;
        private bool ActiveOnce = false;
        private bool activatedMove;
        private enum SubState
        {
            standstill,
            charge,
            ready

        };

        private IEnumerator BleedSound()
        {
            float thing = 0.0f;
            int repeats = 0;
            while (repeats < 20)
            {
                repeats++;
                SoundEffectPlayer.PlaySound("Buf/Effect_Bleeding");
                yield return new WaitForSeconds(.1f);
            }
        }
    }
    public class DiceAttackEffect_Clown_BloodySlash : DiceAttackEffect
    {
        public BattleUnitModel target1;
        public override void Initialize(BattleUnitView self, BattleUnitView target, float destroyTime)
        {
            this._self = self.model;
            this._selfTransform = self.atkEffectRoot;
            this._targetTransform = target.atkEffectRoot;
            base.transform.parent = this._selfTransform;
            base.transform.localScale = Vector3.one;
            base.transform.localPosition = Vector3.zero;
            base.transform.localRotation = Quaternion.identity;
            this._destroyTime = destroyTime;
            Texture2D texture = ClownModInitializer.BloodySlash;
            this.spr.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            base.Initialize(self, target, destroyTime);
            this.time = 0f;
            this.atkdir = self.model.direction;

        }

        protected override void Update()
        {
            try
            {
                base.Update();
                this.time -= Time.deltaTime;
                float num = (this.time * -1f) / this._destroyTime;
                this.spr.color = new Color(1f, 1f, 1f, 1f - num);
                spr.transform.localPosition = target1.view.WorldPosition;
            }
            catch (Exception ex)
            {

            }
        }

        public float time;

        public Direction atkdir;
    }
    public class PassiveAbility_TESTO : PassiveAbilityBase
    {
        public override void OnRoundStart()
        {
            this._recoveredAmount = 0;
        }
        public override void OnRecoverHp(int amount)
        {
            this._recoveredAmount += amount;
        }
        public override void OnSucceedAttack(BattleDiceBehavior behavior)
        {
            this.owner.RecoverHP(1);
            this.owner.breakDetail.RecoverBreak(3);
            BattleCardTotalResult battleCardResultLog = this.owner.battleCardResultLog;
            if (battleCardResultLog == null)
            {
                return;
            }
            battleCardResultLog.SetPassiveAbility(this);
        }
        public override void OnRoundEndTheLast()
        {
            if (this._recoveredAmount >= 5)
            {
                this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 1, null);
                this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Endurance, 1, null);
            }
            this._recoveredAmount = 0;
        }
        private int _recoveredAmount;
        private const int _RECOVER_AMOUNT = 1;
    }
    public class PassiveAbility_HemoRecov : PassiveAbilityBase
    {
        public void ActivateRecov()
        {
            int hpGain = (int)(owner.MaxHp * 0.15f);
            if (hpGain > 20) { hpGain = 20; }
            owner.RecoverHP(hpGain);
        }
    }
    public class PassiveAbility_HemoBoost : PassiveAbilityBase
    {
        public override void OnRoundStart()
        {
            var buf = owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is MortalBleed);
            if (buf != null)
            {
                int boost = buf.stack / 5;
                if (boost > 3) { boost = 3; }
                owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Strength, boost, null);
            }
            base.OnRoundStart();
        }
    }
    public class PassiveAbility_Reckless : PassiveAbilityBase
    {
        public override void OnRoundStart()
        {
            owner.bufListDetail.AddBuf(new NoReduceBleed());
            base.OnRoundStart();
        }
        public override void OnSucceedAttack(BattleDiceBehavior behavior)
        {
            if (behavior.card.target != null)
            {
                HandleHemokinesis.AddMortalBleed(2, behavior.card.target);
                HandleHemokinesis.AddMortalBleed(UnityEngine.Random.Range(2, 5), owner);
            }
            BattleCardTotalResult battleCardResultLog = this.owner.battleCardResultLog;
            if (battleCardResultLog == null)
            {
                return;
            }
            battleCardResultLog.SetPassiveAbility(this);
            base.OnSucceedAttack(behavior);
        }
    }
    /// Enemy Only Passives
    public class PassiveAbility_funnytime : PassiveAbilityBase
    {
        public override void OnFixedUpdateInWaitPhase(float delta)
        {
            base.OnFixedUpdateInWaitPhase(delta);
            {
                if (this.rolled && !this._bTimeLimitOvered)
                {
                    Silence_Emotion_Clock clock = this._clock;
                    if (clock != null)
                    {
                        clock.Run(this._elapsed);
                    }
                    this._elapsed += delta;
                    if (this._elapsed >= 30f)
                    {
                        if (SingletonBehavior<BattleCamManager>.Instance.IsCamIsReturning)
                        {
                            return;
                        }
                        Singleton<StageController>.Instance.CompleteApplyingLibrarianCardPhase(true);
                        this._bTimeLimitOvered = true;
                        this._elapsed = 0f;
                    }
                }
            }
        }

        public override void OnAfterRollSpeedDice()
        {
            if (Singleton<StageController>.Instance.RoundTurn % 3 == 0)
            {
                base.OnAfterRollSpeedDice();
                this.Init();
                this.rolled = true;
                if (_clock == null)
                {
                    return;
                }
                _clock.OnStartRollSpeedDice();
            }
        }
        public override void OnDie()
        {
            base.OnDie();
            if (this._clock != null)
            {
                this._clock.gameObject.SetActive(false);
            }
        }

        public override void OnRoundStart()
        {
            base.OnRoundStartAfter();
            if (Singleton<StageController>.Instance.RoundTurn % 3 == 0)
            {
                this._clock = SingletonBehavior<BattleManagerUI>.Instance.EffectLayer.GetComponentInChildren<Silence_Emotion_Clock>();
                if (this._clock == null)
                {
                    Silence_Emotion_Clock silence_Emotion_Clock = Resources.Load<Silence_Emotion_Clock>("Prefabs/Battle/CreatureEffect/8/Silence_Emotion_Clock");
                    if (silence_Emotion_Clock != null)
                    {
                        this._clock = UnityEngine.Object.Instantiate<Silence_Emotion_Clock>(silence_Emotion_Clock);
                        this._clock.gameObject.transform.SetParent(SingletonBehavior<BattleManagerUI>.Instance.EffectLayer);
                        this._clock.gameObject.transform.localPosition = new Vector3(0f, 800f, 0f);
                        this._clock.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                        this._clock.gameObject.SetActive(true);
                    }
                }
            }
        }

        public override void OnRoundEndTheLast()
        {
            base.OnRoundEndTheLast();
            if (this._clock != null)
            {
                this._clock.gameObject.SetActive(false);
            }

        }

        public override void OnRoundEnd()
        {
            base.OnRoundEnd();
            this.rolled = false;
        }

        private void Init()
        {
            this._elapsed = 0f;
            this._bTimeLimitOvered = false;
            this.rolled = false;
        }

        private const float _START_BATTLE_TIME = 30f;
        private bool rolled;
        private float _elapsed;
        private bool _bTimeLimitOvered;
        private Silence_Emotion_Clock _clock;
    }
    public class EnemyUnitTargetSetter_turbulence : EnemyUnitTargetSetter
    {
        public override BattleUnitModel SelectTargetUnit(List<BattleUnitModel> candidates)
        {
            bool flag = false;
            List<BattleUnitModel> list = new List<BattleUnitModel>();
            foreach (BattleUnitModel battleUnitModel in candidates)
            {
                BattleUnitBuf activatedBuf = battleUnitModel.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_clownedmark) as BattleUnitBuf_clownedmark;
                if (activatedBuf != null)
                {
                    flag = true;
                    list.Add(battleUnitModel);
                }
            }
            BattleUnitModel result;
            if (flag == false)
            {
                result = RandomUtil.SelectOne<BattleUnitModel>(candidates);
            }
            else
            {
                result = RandomUtil.SelectOne<BattleUnitModel>(list);
            }
            return result;
        }
    }
    public class DiceCardSelfAbility_gamer : DiceCardSelfAbilityBase
    {
        bool gamer;
        public class dudetrackerdudetracker : BattleUnitBuf
        {
            public List<BattlePlayingCardDataInUnitModel> list = new List<BattlePlayingCardDataInUnitModel>();
            public override void OnUseCard(BattlePlayingCardDataInUnitModel card)
            {
                base.OnUseCard(card);
                list.Add(card);
            }
            public override void OnRoundEnd()
            {
                base.OnRoundEnd();
                this.Destroy();
            }
        }
        public override void OnStartBattle()
        {
            base.OnStartBattle();
            if (!this.owner.bufListDetail.HasBuf<dudetrackerdudetracker>())
            {
                this.owner.bufListDetail.AddBuf(new dudetrackerdudetracker());
            }
        }
        public override void OnUseCard()
        {
            base.OnUseCard();
            gamer = false;
        }
        public override void OnSucceedAttack()
        {
            base.OnSucceedAttack();
            gamer = true;
        }
        public override void OnEndBattle()
        {
            base.OnEndBattle();
            dudetrackerdudetracker dude = owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is dudetrackerdudetracker) as dudetrackerdudetracker;
            if (gamer && dude != null)
            {
                int gameing = 0;
                try
                {
                    foreach (BattlePlayingCardDataInUnitModel among in this.owner.cardSlotDetail.cardAry)
                    {
                        if (among != this.card && among != null && !dude.list.Contains(among))
                        {
                            gameing += among.card.CurCost;
                        }
                    }
                }
                catch
                {

                }
                if (owner.cardSlotDetail.PlayPoint - gameing > 0)
                {
                    owner.cardSlotDetail.SpendCost(1);
                    var enemies = BattleObjectManager.instance.GetAliveList_opponent(owner.faction);
                    if (enemies.Count <= 0) return;
                    var target = RandomUtil.SelectOne(enemies);
                    var b = new BattlePlayingCardDataInUnitModel
                    {
                        card = BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(new LorId("TurbulenceOffice", 192))),
                        owner = owner,
                        earlyTarget = target,
                        earlyTargetOrder = UnityEngine.Random.Range(0, target.speedDiceResult.Count),
                        slotOrder = 0,
                        speedDiceResultValue = owner.GetSpeed(0)
                    };
                    b.target = b.earlyTarget;
                    b.targetSlotOrder = b.earlyTargetOrder;
                    b.cardAbility = b.card.CreateDiceCardSelfAbilityScript();
                    b.ResetCardQueue();
                    StageController.Instance.AddAllCardListInBattle(b, target);
                }
            }
        }
    }
    public class PassiveAbility_massofchaos_player : PassiveAbilityBase
    {
        public override void OnTakeDamageByAttack(BattleDiceBehavior atkDice, int dmg)
        {
            base.OnTakeDamageByAttack(atkDice, dmg);
            if (RandomUtil.valueForProb < 0.07f)
            {
                ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("ahhhhhmuffled.ogg"), owner.view.transform, 2f);
            }
        }
        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            base.OnUseCard(curCard);
            if (RandomUtil.valueForProb < 0.15f)
            {
                int among = RandomUtil.SelectOne<int>(new int[]
                {
                    1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14
                });
                switch (among)
                {
                    case 1:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("crushmuffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 2:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("die2muffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 3:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("diemuffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 4:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("judgement1muffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 5:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("judgement2muffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 6:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("prepare1muffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 7:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("prepare2muffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 8:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("thecrimesthykindmuffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 9:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("thyend1muffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 10:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("thyend2muffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 11:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("thygoreshallglistenmuffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 12:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("thypunishmentmuffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 13:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("uselessmuffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 14:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("weakmuffled.ogg"), owner.view.transform, 2f);
                        break;
                }
            }
            if (RandomUtil.valueForProb < 0.35f)
            {
                int among = RandomUtil.SelectOne<int>(new int[]
                {
                    1, 2, 3
                });
                switch (among)
                {
                    case 1:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("bringthatshit.ogg"), owner.view.transform, 2f);
                        break;
                    case 2:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("10years.ogg"), owner.view.transform, 2f);
                        break;
                    case 3:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("cmonfucker.ogg"), owner.view.transform, 2f);
                        break;
                }
            }
        }
        public override void OnRoundStart()
        {
            base.OnRoundStart();
            this.owner.allyCardDetail.ExhaustAllCards(); 
            int one = RandomUtil.SelectOne<int>(new int[]
             {
                1, 2, 3, 4, 5, 6
             });
            int two = RandomUtil.SelectOne<int>(new int[]
            {
                1, 2, 3, 4, 5, 6
            });
            int drei = RandomUtil.SelectOne<int>(new int[]
            {
                1, 2, 3, 4, 5, 6
            });
            this.owner.Book.SetResistHP(BehaviourDetail.Slash, AtkResist.Normal);
            this.owner.Book.SetResistBP(BehaviourDetail.Slash, AtkResist.Normal);
            this.owner.Book.SetResistHP(BehaviourDetail.Penetrate, AtkResist.Normal);
            this.owner.Book.SetResistBP(BehaviourDetail.Penetrate, AtkResist.Normal);
            this.owner.Book.SetResistHP(BehaviourDetail.Hit, AtkResist.Normal);
            this.owner.Book.SetResistBP(BehaviourDetail.Hit, AtkResist.Normal);
            switch (one)
            {
                case 1:
                    this.owner.Book.SetResistHP(BehaviourDetail.Slash, AtkResist.Endure);
                    break;
                case 2:
                    this.owner.Book.SetResistBP(BehaviourDetail.Slash, AtkResist.Endure);
                    break;
                case 3:
                    this.owner.Book.SetResistHP(BehaviourDetail.Penetrate, AtkResist.Endure);
                    break;
                case 4:
                    this.owner.Book.SetResistBP(BehaviourDetail.Penetrate, AtkResist.Endure);
                    break;
                case 5:
                    this.owner.Book.SetResistHP(BehaviourDetail.Hit, AtkResist.Endure);
                    break;
                case 6:
                    this.owner.Book.SetResistBP(BehaviourDetail.Hit, AtkResist.Endure);
                    break;
            }
            switch (two)
            {
                case 1:
                    this.owner.Book.SetResistHP(BehaviourDetail.Slash, AtkResist.Endure);
                    break;
                case 2:
                    this.owner.Book.SetResistBP(BehaviourDetail.Slash, AtkResist.Endure);
                    break;
                case 3:
                    this.owner.Book.SetResistHP(BehaviourDetail.Penetrate, AtkResist.Endure);
                    break;
                case 4:
                    this.owner.Book.SetResistBP(BehaviourDetail.Penetrate, AtkResist.Endure);
                    break;
                case 5:
                    this.owner.Book.SetResistHP(BehaviourDetail.Hit, AtkResist.Endure);
                    break;
                case 6:
                    this.owner.Book.SetResistBP(BehaviourDetail.Hit, AtkResist.Endure);
                    break;
            }
            switch (drei)
            {
                case 1:
                    this.owner.Book.SetResistHP(BehaviourDetail.Slash, AtkResist.Endure);
                    break;
                case 2:
                    this.owner.Book.SetResistBP(BehaviourDetail.Slash, AtkResist.Endure);
                    break;
                case 3:
                    this.owner.Book.SetResistHP(BehaviourDetail.Penetrate, AtkResist.Endure);
                    break;
                case 4:
                    this.owner.Book.SetResistBP(BehaviourDetail.Penetrate, AtkResist.Endure);
                    break;
                case 5:
                    this.owner.Book.SetResistHP(BehaviourDetail.Hit, AtkResist.Endure);
                    break;
                case 6:
                    this.owner.Book.SetResistBP(BehaviourDetail.Hit, AtkResist.Endure);
                    break;
            }
            this.owner.allyCardDetail.AddNewCard(new LorId("TurbulenceOffice", RandomUtil.SelectOne<int>(new int[]
                         { 13, 14, 17, 18, 21, 22, 23, 24, 30, 31, 32, 33, 41, 42, 44, 55, 58, 61, 62, 63, 64, 65, 66, 67, 68, 71, 72, 73, 81, 83, 84, 85, 87, 94, 96, 97, 101, 102, 103, 105, 106, 107, 111, 113, 114, 115, 117, 131, 132, 134, 135, 141, 143, 144, 145, 149, 151, 156, 157, 161, 162, 163, 164, 165, 166, 167 })), false);
            this.owner.allyCardDetail.AddNewCard(new LorId("TurbulenceOffice", RandomUtil.SelectOne<int>(new int[]
                         { 13, 14, 17, 18, 21, 22, 23, 24, 30, 31, 32, 33, 41, 42, 44, 55, 58, 61, 62, 63, 64, 65, 66, 67, 68, 71, 72, 73, 81, 83, 84, 85, 87, 94, 96, 97, 101, 102, 103, 105, 106, 107, 111, 113, 114, 115, 117, 131, 132, 134, 135, 141, 143, 144, 145, 149, 151, 156, 157, 161, 162, 163, 164, 165, 166, 167 })), false);
            this.owner.allyCardDetail.AddNewCard(new LorId("TurbulenceOffice", RandomUtil.SelectOne<int>(new int[]
                         { 13, 14, 17, 18, 21, 22, 23, 24, 30, 31, 32, 33, 41, 42, 44, 55, 58, 61, 62, 63, 64, 65, 66, 67, 68, 71, 72, 73, 81, 83, 84, 85, 87, 94, 96, 97, 101, 102, 103, 105, 106, 107, 111, 113, 114, 115, 117, 131, 132, 134, 135, 141, 143, 144, 145, 149, 151, 156, 157, 161, 162, 163, 164, 165, 166, 167 })), false);
            this.owner.allyCardDetail.AddNewCard(new LorId("TurbulenceOffice", RandomUtil.SelectOne<int>(new int[]
                         { 13, 14, 17, 18, 21, 22, 23, 24, 30, 31, 32, 33, 41, 42, 44, 55, 58, 61, 62, 63, 64, 65, 66, 67, 68, 71, 72, 73, 81, 83, 84, 85, 87, 94, 96, 97, 101, 102, 103, 105, 106, 107, 111, 113, 114, 115, 117, 131, 132, 134, 135, 141, 143, 144, 145, 149, 151, 156, 157, 161, 162, 163, 164, 165, 166, 167 })), false);

            if (this.owner.emotionDetail.EmotionLevel >= 3)
            {
                this.owner.allyCardDetail.AddNewCard(new LorId("TurbulenceOffice", RandomUtil.SelectOne<int>(new int[]
                         { 13, 14, 17, 18, 21, 22, 23, 24, 30, 31, 32, 33, 41, 42, 44, 55, 58, 61, 62, 63, 64, 65, 66, 67, 68, 71, 72, 73, 81, 83, 84, 85, 87, 94, 96, 97, 101, 102, 103, 105, 106, 107, 111, 113, 114, 115, 117, 131, 132, 134, 135, 141, 143, 144, 145, 149, 151, 156, 157, 161, 162, 163, 164, 165, 166, 167 })), false);

            }
            if (this.owner.emotionDetail.EmotionLevel >= 5)
            {
                this.owner.allyCardDetail.AddNewCard(new LorId("TurbulenceOffice", RandomUtil.SelectOne<int>(new int[]
                         { 13, 14, 17, 18, 21, 22, 23, 24, 30, 31, 32, 33, 41, 42, 44, 55, 58, 61, 62, 63, 64, 65, 66, 67, 68, 71, 72, 73, 81, 83, 84, 85, 87, 94, 96, 97, 101, 102, 103, 105, 106, 107, 111, 113, 114, 115, 117, 131, 132, 134, 135, 141, 143, 144, 145, 149, 151, 156, 157, 161, 162, 163, 164, 165, 166, 167 })), false);

            }
        }
    }
    public class PassiveAbility_massofchaos : PassiveAbilityBase
    {

        public override void OnTakeDamageByAttack(BattleDiceBehavior atkDice, int dmg)
        {
            base.OnTakeDamageByAttack(atkDice, dmg);
            if (RandomUtil.valueForProb < 0.07f)
            {
                ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("ahhhhhmuffled.ogg"), owner.view.transform, 2f);
            }
        }
        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            base.OnUseCard(curCard);
            if (RandomUtil.valueForProb < 0.15f)
            {
                int among = RandomUtil.SelectOne<int>(new int[]
                {
                    1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14
                });
                switch (among)
                {
                    case 1:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("crushmuffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 2:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("die2muffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 3:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("diemuffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 4:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("judgement1muffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 5:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("judgement2muffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 6:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("prepare1muffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 7:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("prepare2muffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 8:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("thecrimesthykindmuffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 9:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("thyend1muffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 10:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("thyend2muffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 11:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("thygoreshallglistenmuffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 12:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("thypunishmentmuffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 13:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("uselessmuffled.ogg"), owner.view.transform, 2f);
                        break;
                    case 14:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("weakmuffled.ogg"), owner.view.transform, 2f);
                        break;
                }
            }
            if (RandomUtil.valueForProb < 0.35f && Singleton<StageController>.Instance.CurrentWave == 3)
            {
                int among = RandomUtil.SelectOne<int>(new int[]
                {
                    1, 2, 3
                });
                switch (among)
                {
                    case 1:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("bringthatshit.ogg"), owner.view.transform, 4f);
                        break;
                    case 2:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("10years.ogg"), owner.view.transform, 4f);
                        break;
                    case 3:
                        ClownModInitializer.PlaySound(CustomMapHandler.GetAudioClip("cmonfucker.ogg"), owner.view.transform, 4f);
                        break;
                }
            }
        }
        public override void OnRoundStart()
        {
            base.OnRoundStart();
            this.owner.allyCardDetail.ExhaustAllCards();
            int one = RandomUtil.SelectOne<int>(new int[]
            {
                1, 2, 3, 4, 5, 6
            });
            int two = RandomUtil.SelectOne<int>(new int[]
            {
                1, 2, 3, 4, 5, 6
            });
            int drei = RandomUtil.SelectOne<int>(new int[]
            {
                1, 2, 3, 4, 5, 6
            });
            this.owner.Book.SetResistHP(BehaviourDetail.Slash, AtkResist.Normal);
            this.owner.Book.SetResistBP(BehaviourDetail.Slash, AtkResist.Normal);
            this.owner.Book.SetResistHP(BehaviourDetail.Penetrate, AtkResist.Normal);
            this.owner.Book.SetResistBP(BehaviourDetail.Penetrate, AtkResist.Normal);
            this.owner.Book.SetResistHP(BehaviourDetail.Hit, AtkResist.Normal);
            this.owner.Book.SetResistBP(BehaviourDetail.Hit, AtkResist.Normal);
            switch (one)
            {
                case 1:
                    this.owner.Book.SetResistHP(BehaviourDetail.Slash, AtkResist.Endure);
                    break;
                case 2:
                    this.owner.Book.SetResistBP(BehaviourDetail.Slash, AtkResist.Endure);
                    break;
                case 3:
                    this.owner.Book.SetResistHP(BehaviourDetail.Penetrate, AtkResist.Endure);
                    break;
                case 4:
                    this.owner.Book.SetResistBP(BehaviourDetail.Penetrate, AtkResist.Endure);
                    break;
                case 5:
                    this.owner.Book.SetResistHP(BehaviourDetail.Hit, AtkResist.Endure);
                    break;
                case 6:
                    this.owner.Book.SetResistBP(BehaviourDetail.Hit, AtkResist.Endure);
                    break;
            }
            switch (two)
            {
                case 1:
                    this.owner.Book.SetResistHP(BehaviourDetail.Slash, AtkResist.Endure);
                    break;
                case 2:
                    this.owner.Book.SetResistBP(BehaviourDetail.Slash, AtkResist.Endure);
                    break;
                case 3:
                    this.owner.Book.SetResistHP(BehaviourDetail.Penetrate, AtkResist.Endure);
                    break;
                case 4:
                    this.owner.Book.SetResistBP(BehaviourDetail.Penetrate, AtkResist.Endure);
                    break;
                case 5:
                    this.owner.Book.SetResistHP(BehaviourDetail.Hit, AtkResist.Endure);
                    break;
                case 6:
                    this.owner.Book.SetResistBP(BehaviourDetail.Hit, AtkResist.Endure);
                    break;
            }
            switch (drei)
            {
                case 1:
                    this.owner.Book.SetResistHP(BehaviourDetail.Slash, AtkResist.Endure);
                    break;
                case 2:
                    this.owner.Book.SetResistBP(BehaviourDetail.Slash, AtkResist.Endure);
                    break;
                case 3:
                    this.owner.Book.SetResistHP(BehaviourDetail.Penetrate, AtkResist.Endure);
                    break;
                case 4:
                    this.owner.Book.SetResistBP(BehaviourDetail.Penetrate, AtkResist.Endure);
                    break;
                case 5:
                    this.owner.Book.SetResistHP(BehaviourDetail.Hit, AtkResist.Endure);
                    break;
                case 6:
                    this.owner.Book.SetResistBP(BehaviourDetail.Hit, AtkResist.Endure);
                    break;
            }
            if (this.owner.faction == Faction.Enemy)
            {
                this.owner.allyCardDetail.AddNewCard(new LorId("TurbulenceOffice", RandomUtil.SelectOne<int>(new int[]
                 { 11, 12, 13, 14, 15, 16, 25, 22, 23, 24, 31, 32, 33, 42, 43, 44, 51, 52, 53, 54, 55, 56, 57, 58, 59, 61, 62, 63, 64, 65, 66, 71, 81, 82, 83, 84, 87, 91, 92, 93, 94, 95, 96, 97, 101, 102, 103, 104, 105, 106, 111, 112, 113, 114, 115, 116, 117, 118, 121, 122, 123, 124, 125, 131, 132, 133, 134, 135, 141, 142, 143, 144, 145, 146, 148, 149, 151, 152, 153, 154, 155, 157, 161, 162, 163, 164, 165 })), false);
                this.owner.allyCardDetail.AddNewCard(new LorId("TurbulenceOffice", RandomUtil.SelectOne<int>(new int[]
                             { 11, 12, 13, 14, 15, 16, 25, 22, 23, 24, 31, 32, 33, 42, 43, 44, 51, 52, 53, 54, 55, 56, 57, 58, 59, 61, 62, 63, 64, 65, 66, 71, 81, 82, 83, 84, 87, 91, 92, 93, 94, 95, 96, 97, 101, 102, 103, 104, 105, 106, 111, 112, 113, 114, 115, 116, 117, 118, 121, 122, 123, 124, 125, 131, 132, 133, 134, 135, 141, 142, 143, 144, 145, 146, 148, 149, 151, 152, 153, 154, 155, 157, 161, 162, 163, 164, 165 })), false);
                this.owner.allyCardDetail.AddNewCard(new LorId("TurbulenceOffice", RandomUtil.SelectOne<int>(new int[]
                             { 11, 12, 13, 14, 15, 16, 25, 22, 23, 24, 31, 32, 33, 42, 43, 44, 51, 52, 53, 54, 55, 56, 57, 58, 59, 61, 62, 63, 64, 65, 66, 71, 81, 82, 83, 84, 87, 91, 92, 93, 94, 95, 96, 97, 101, 102, 103, 104, 105, 106, 111, 112, 113, 114, 115, 116, 117, 118, 121, 122, 123, 124, 125, 131, 132, 133, 134, 135, 141, 142, 143, 144, 145, 146, 148, 149, 151, 152, 153, 154, 155, 157, 161, 162, 163, 164, 165 })), false);
                this.owner.allyCardDetail.AddNewCard(new LorId("TurbulenceOffice", RandomUtil.SelectOne<int>(new int[]
                             { 11, 12, 13, 14, 15, 16, 25, 22, 23, 24, 31, 32, 33, 42, 43, 44, 51, 52, 53, 54, 55, 56, 57, 58, 59, 61, 62, 63, 64, 65, 66, 71, 81, 82, 83, 84, 87, 91, 92, 93, 94, 95, 96, 97, 101, 102, 103, 104, 105, 106, 111, 112, 113, 114, 115, 116, 117, 118, 121, 122, 123, 124, 125, 131, 132, 133, 134, 135, 141, 142, 143, 144, 145, 146, 148, 149, 151, 152, 153, 154, 155, 157, 161, 162, 163, 164, 165})), false);

                if (this.owner.emotionDetail.EmotionLevel >= 3)
                {
                    this.owner.allyCardDetail.AddNewCard(new LorId("TurbulenceOffice", RandomUtil.SelectOne<int>(new int[]
                 { 11, 12, 13, 14, 15, 16, 25, 22, 23, 24, 31, 32, 33, 42, 43, 44, 51, 52, 53, 54, 55, 56, 57, 58, 59, 61, 62, 63, 64, 65, 66, 71, 81, 82, 83, 84, 87, 91, 92, 93, 94, 95, 96, 97, 101, 102, 103, 104, 105, 106, 111, 112, 113, 114, 115, 116, 117, 118, 121, 122, 123, 124, 125, 131, 132, 133, 134, 135, 141, 142, 143, 144, 145, 146, 148, 149, 151, 152, 153, 154, 155, 157, 161, 162, 163, 164, 165 })), false);

                }
                if (this.owner.emotionDetail.EmotionLevel >= 5)
                {
                    this.owner.allyCardDetail.AddNewCard(new LorId("TurbulenceOffice", RandomUtil.SelectOne<int>(new int[]
                 { 11, 12, 13, 14, 15, 16, 25, 22, 23, 24, 31, 32, 33, 42, 43, 44, 51, 52, 53, 54, 55, 56, 57, 58, 59, 61, 62, 63, 64, 65, 66, 71, 81, 82, 83, 84, 87, 91, 92, 93, 94, 95, 96, 97, 101, 102, 103, 104, 105, 106, 111, 112, 113, 114, 115, 116, 117, 118, 121, 122, 123, 124, 125, 131, 132, 133, 134, 135, 141, 142, 143, 144, 145, 146, 148, 149, 151, 152, 153, 154, 155, 157, 161, 162, 163, 164, 165})), false);

                }
            }
            if (this.owner.faction == Faction.Player)
            {
                this.owner.allyCardDetail.AddNewCard(new LorId("TurbulenceOffice", RandomUtil.SelectOne<int>(new int[]
                 { 11, 12, 13, 14, 15, 16, 21, 22, 23, 24, 31, 32, 33, 42, 43, 44, 51, 52, 53, 54, 55, 56, 57, 58, 59, 61, 62, 63, 64, 65, 66, 71, 81, 82, 83, 84, 87, 91, 92, 93, 94, 95, 96, 97, 101, 102, 103, 104, 105, 106, 111, 112, 113, 114, 115, 116, 117, 118, 121, 122, 123, 124, 125, 131, 132, 133, 134, 135, 141, 142, 143, 144, 145, 146, 148, 149, 151, 152, 153, 154, 155, 157, 161, 162, 163, 164, 165 })), false);
                this.owner.allyCardDetail.AddNewCard(new LorId("TurbulenceOffice", RandomUtil.SelectOne<int>(new int[]
                             { 11, 12, 13, 14, 15, 16, 21, 22, 23, 24, 31, 32, 33, 42, 43, 44, 51, 52, 53, 54, 55, 56, 57, 58, 59, 61, 62, 63, 64, 65, 66, 71, 81, 82, 83, 84, 87, 91, 92, 93, 94, 95, 96, 97, 101, 102, 103, 104, 105, 106, 111, 112, 113, 114, 115, 116, 117, 118, 121, 122, 123, 124, 125, 131, 132, 133, 134, 135, 141, 142, 143, 144, 145, 146, 148, 149, 151, 152, 153, 154, 155, 157, 161, 162, 163, 164, 165 })), false);
                this.owner.allyCardDetail.AddNewCard(new LorId("TurbulenceOffice", RandomUtil.SelectOne<int>(new int[]
                             { 11, 12, 13, 14, 15, 16, 21, 22, 23, 24, 31, 32, 33, 42, 43, 44, 51, 52, 53, 54, 55, 56, 57, 58, 59, 61, 62, 63, 64, 65, 66, 71, 81, 82, 83, 84, 87, 91, 92, 93, 94, 95, 96, 97, 101, 102, 103, 104, 105, 106, 111, 112, 113, 114, 115, 116, 117, 118, 121, 122, 123, 124, 125, 131, 132, 133, 134, 135, 141, 142, 143, 144, 145, 146, 148, 149, 151, 152, 153, 154, 155, 157, 161, 162, 163, 164, 165 })), false);
                this.owner.allyCardDetail.AddNewCard(new LorId("TurbulenceOffice", RandomUtil.SelectOne<int>(new int[]
                             { 11, 12, 13, 14, 15, 16, 21, 22, 23, 24, 31, 32, 33, 42, 43, 44, 51, 52, 53, 54, 55, 56, 57, 58, 59, 61, 62, 63, 64, 65, 66, 71, 81, 82, 83, 84, 87, 91, 92, 93, 94, 95, 96, 97, 101, 102, 103, 104, 105, 106, 111, 112, 113, 114, 115, 116, 117, 118, 121, 122, 123, 124, 125, 131, 132, 133, 134, 135, 141, 142, 143, 144, 145, 146, 148, 149, 151, 152, 153, 154, 155, 157, 161, 162, 163, 164, 165 })), false);

                if (this.owner.emotionDetail.EmotionLevel >= 3)
                {
                    this.owner.allyCardDetail.AddNewCard(new LorId("TurbulenceOffice", RandomUtil.SelectOne<int>(new int[]
                 { 11, 12, 13, 14, 15, 16, 21, 22, 23, 24, 31, 32, 33, 42, 43, 44, 51, 52, 53, 54, 55, 56, 57, 58, 59, 61, 62, 63, 64, 65, 66, 71, 81, 82, 83, 84, 87, 91, 92, 93, 94, 95, 96, 97, 101, 102, 103, 104, 105, 106, 111, 112, 113, 114, 115, 116, 117, 118, 121, 122, 123, 124, 125, 131, 132, 133, 134, 135, 141, 142, 143, 144, 145, 146, 148, 149, 151, 152, 153, 154, 155, 157, 161, 162, 163, 164, 165 })), false);

                }
                if (this.owner.emotionDetail.EmotionLevel >= 5)
                {
                    this.owner.allyCardDetail.AddNewCard(new LorId("TurbulenceOffice", RandomUtil.SelectOne<int>(new int[]
                 { 11, 12, 13, 14, 15, 16, 21, 22, 23, 24, 31, 32, 33, 42, 43, 44, 51, 52, 53, 54, 55, 56, 57, 58, 59, 61, 62, 63, 64, 65, 66, 71, 81, 82, 83, 84, 87, 91, 92, 93, 94, 95, 96, 97, 101, 102, 103, 104, 105, 106, 111, 112, 113, 114, 115, 116, 117, 118, 121, 122, 123, 124, 125, 131, 132, 133, 134, 135, 141, 142, 143, 144, 145, 146, 148, 149, 151, 152, 153, 154, 155, 157, 161, 162, 163, 164, 165 })), false);

                }
            }
        }
    }
    public class ClownModInitializer : ModInitializer
    {
        public override void OnInitializeMod()
        {
            base.OnInitializeMod();
            ClownModInitializer.path = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
            Harmony harmony = new Harmony("TurbulenceOffice");
            harmony.PatchAll();
            MethodInfo method = typeof(ClownModInitializer).GetMethod("DiceEffectManager_CreateAttackEffect");
            harmony.Patch(typeof(DiceEffectManager).GetMethod("CreateAttackEffect", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
            method = typeof(ClownModInitializer).GetMethod("DiceEffectManager_CreateBehaviourEffect");
            harmony.Patch(typeof(DiceEffectManager).GetMethod("CreateBehaviourEffect", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
            ClownModInitializer.BloodySlash = new Texture2D(2, 2);
            ClownModInitializer.BloodySlash.LoadImage(File.ReadAllBytes(ClownModInitializer.ModPath + "/Resource/bloodslash.png"));
            method = typeof(ClownModInitializer).GetMethod("BookModel_SetXmlInfo_Post");
            harmony.Patch(typeof(BookModel).GetMethod("SetXmlInfo", AccessTools.all), null, new HarmonyMethod(method), null, null, null);
            harmony.Patch(typeof(UISpriteDataManager).GetMethod("SetStoryIconDictionary", AccessTools.all), new HarmonyMethod(typeof(ClownModInitializer).GetMethod("AddIcon")));
            Harmony.CreateAndPatchAll(typeof(PatchList), base.GetType().Assembly.GetName().Name);
            ClownModInitializer.language = GlobalGameManager.Instance.CurrentOption.language;
            ClownModInitializer.RemoveError();
            ClownModInitializer.PreLoadBufIcons();
            ClownModInitializer.Init = true;
        }
        private class PatchList
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(BattleUnitView), "ChangeSkin")]
            public static void BattleUnitView_ChangeSkin(BattleUnitView __instance, string charName)
            {
                WorkshopSkinData workshopBookSkinData = Singleton<CustomizingBookSkinLoader>.Instance.GetWorkshopBookSkinData(packageId, charName);
                if (workshopBookSkinData != null)
                {
                    Dictionary<ActionDetail, ClothCustomizeData> dic = __instance.charAppearance.gameObject.GetComponent<WorkshopSkinDataSetter>().dic;
                    if (dic == null || dic.Count == 0)
                    {
                        __instance.charAppearance.gameObject.GetComponent<WorkshopSkinDataSetter>().SetData(workshopBookSkinData);
                    }
                }
            }
        }
        public static bool DiceEffectManager_CreateAttackEffect(DiceEffectManager __instance, BattleUnitModel unit, ref DiceAttackEffect __result, float destroyTime = 1f)
        {
            bool flag = unit.currentDiceAction.currentBehavior.behaviourInCard.EffectRes.StartsWith("Clown_");
            bool result;
            if (flag)
            {
                string effectRes = unit.currentDiceAction.currentBehavior.behaviourInCard.EffectRes;
                string a = effectRes;
                Type componentType;
                if (!(a == "Clown_BloodySlash"))
                {
                    if (!(a == "fffffff"))
                    {
                        componentType = null;
                    }
                    else
                    {
                        componentType = null;
                    }
                }
                else
                {
                    componentType = typeof(DiceAttackEffect_Clown_BloodySlash);
                }
                GameObject gameObject = new GameObject(unit.currentDiceAction.currentBehavior.behaviourInCard.EffectRes);
                DiceAttackEffect diceAttackEffect = gameObject.AddComponent(componentType) as DiceAttackEffect;
                int dice = unit.currentDiceAction.currentBehavior.behaviourInCard.Dice;
                float num = (float)unit.currentDiceAction.currentBehavior.DiceResultValue / (float)dice;
                num = Mathf.Clamp(num, 0f, 1f);
                Vector3 localScale = diceAttackEffect.transform.localScale * (1f + num);
                diceAttackEffect.transform.localScale = localScale;
                __result = diceAttackEffect;
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }
        public static bool DiceEffectManager_CreateBehaviourEffect(DiceEffectManager __instance, string resource, float scaleFactor, BattleUnitView self, BattleUnitView target, ref DiceAttackEffect __result, float time = 1f)
        {
            bool flag = resource.StartsWith("Clown_");
            bool result;
            if (flag)
            {
                Type componentType;
                if (!(resource == "Clown_BloodySlash"))
                {
                    if (!(resource == "454grf"))
                    {
                        componentType = null;
                    }
                    else
                    {
                        componentType = null;
                    }
                }
                else
                {
                    componentType = typeof(DiceAttackEffect_Clown_BloodySlash);
                }
                GameObject gameObject = new GameObject(resource);
                DiceAttackEffect diceAttackEffect = gameObject.AddComponent(componentType) as DiceAttackEffect;
                diceAttackEffect.Initialize(self, target, time);
                diceAttackEffect.SetScale(scaleFactor);
                __result = diceAttackEffect;
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }
        public static void PlaySound(AudioClip audio, Transform transform, float VolumnControl = 1.5f)
        {
            BattleEffectSound battleEffectSound = UnityEngine.Object.Instantiate<BattleEffectSound>(SingletonBehavior<BattleSoundManager>.Instance.effectSoundPrefab, transform);
            float volume = 1f;
            bool flag = SingletonBehavior<BattleSoundManager>.Instance != null;
            if (flag)
            {
                volume = SingletonBehavior<BattleSoundManager>.Instance.VolumeFX * VolumnControl;
            }
            battleEffectSound.Init(audio, volume, false);
        }

        public static void RemoveError()
        {
            List<string> list = new List<string>();
            List<string> list2 = new List<string>();
            list.Add("0Harmony");
            list.Add("Mono.Cecil");
            list.Add("MonoMod.Common");
            list.Add("MonoMod.RuntimeDetour");
            list.Add("MonoMod.Utils");
            list.Add("NAudio");
            using (List<string>.Enumerator enumerator = Singleton<ModContentManager>.Instance.GetErrorLogs().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    string errorLog = enumerator.Current;
                    if (list.Exists((string x) => errorLog.Contains(x)))
                    {
                        list2.Add(errorLog);
                    }
                }
            }
            foreach (string item in list2)
            {
                Singleton<ModContentManager>.Instance.GetErrorLogs().Remove(item);
            }
        }
        public static void AddIcon(UISpriteDataManager __instance)
        {
            try
            {
                Texture2D texture = new Texture2D(2, 2);
                Texture2D textureGlow = new Texture2D(2, 2);
                var bookIconDir = new DirectoryInfo(ClownModInitializer.path + "/ArtWork");
                texture.LoadImage(File.ReadAllBytes(bookIconDir + "/icon.png"));
                textureGlow.LoadImage(File.ReadAllBytes(bookIconDir + "/icon.png"));
                UIIconManager.IconSet StoryIcon = new UIIconManager.IconSet
                {

                    type = "clowns",
                    icon = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100),
                    color = Color.white,
                    iconGlow = Sprite.Create(textureGlow, new Rect(0f, 0f, textureGlow.width, textureGlow.height), new Vector2(0.5f, 0.5f), 100),
                    colorGlow = Color.white,
                };
                __instance._storyicons.Add(StoryIcon);
            }
            catch
            {
                Singleton<ModContentManager>.Instance.AddErrorLog("Failed to load icon");
            }
        }
        public static void PreLoadBufIcons()
        {
            foreach (var baseGameIcon in Resources.LoadAll<Sprite>("Sprites/BufIconSheet/").Where(x => !BattleUnitBuf._bufIconDictionary.ContainsKey(x.name)))
                BattleUnitBuf._bufIconDictionary.Add(baseGameIcon.name, baseGameIcon);
            string bufIconDirectory = (ClownModInitializer.path + "/ArtWork/BufIcons");
            if (Directory.Exists(bufIconDirectory))
            {
                var path = new DirectoryInfo(bufIconDirectory);
                if (path != null)
                {
                    LoadSpritesIntoDict(path, BufIcons);
                    BufIcons.Where(x => !BattleUnitBuf._bufIconDictionary.ContainsKey(x.Key)).Do(x => BattleUnitBuf._bufIconDictionary.Add(x.Key, x.Value));
                }
            }
        }
        private static void LoadSpritesIntoDict(DirectoryInfo path, Dictionary<string, Sprite> dict)
        {
            if (path != null && Directory.Exists(path.FullName))
                foreach (var file in path.GetFiles().Where(x => x.Extension == ".png"))
                {
                    if (!dict.ContainsKey(Path.GetFileNameWithoutExtension(file.FullName)))
                    {
                        Texture2D texture2D = new Texture2D(2, 2);
                        texture2D.LoadImage(File.ReadAllBytes(file.FullName));
                        Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0f, 0f));
                        dict.Add(Path.GetFileNameWithoutExtension(file.FullName), sprite);
                    }
                }
        }
        public static void BookModel_SetXmlInfo_Post(BookModel __instance, BookXmlInfo ____classInfo, ref List<DiceCardXmlInfo> ____onlyCards)
        {
            if (__instance.BookId.packageId == ClownModInitializer.packageId)
            {
                foreach (int id in ____classInfo.EquipEffect.OnlyCard)
                {
                    DiceCardXmlInfo cardItem = ItemXmlDataList.instance.GetCardItem(new LorId(ClownModInitializer.packageId, id), false);
                    ____onlyCards.Add(cardItem);
                }
            }
        }
        public static string ModPath
        {
            get
            {
                return Singleton<ModContentManager>.Instance.GetModPath("TurbulenceOffice");
            }
        }

        public static string path;
        public static string language;
        public static string packageId = "TurbulenceOffice";
        public static bool Init;
        public static Texture2D BloodySlash;
        public static Dictionary<string, Sprite> ArtWorks = new Dictionary<string, Sprite>();
        internal static Dictionary<string, Sprite> BufIcons = new Dictionary<string, Sprite>();
    }
    public class DiceCardAbility_hahafuckyou : DiceCardAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                    "Vulnerable_Keyword",
                    "Weak_Keyword",
                    "Binding_Keyword"
                };
            }
        }

        public override void OnWinParrying()
        {
            BattleUnitModel target = base.card.target;
            if (target != null)
            {
                target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Vulnerable, 2, base.owner);
                target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Weak, 2, base.owner);
                target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Binding, 2, base.owner);
            }
            if (target != null)
            {
                target.bufListDetail.AddKeywordBufNextNextByCard(KeywordBuf.Vulnerable, 2, base.owner);
                target.bufListDetail.AddKeywordBufNextNextByCard(KeywordBuf.Weak, 2, base.owner);
                target.bufListDetail.AddKeywordBufNextNextByCard(KeywordBuf.Binding, 2, base.owner);
            }
        }

        public static string Desc = "[On Clash Win] Inflict 2 Bind, Fragile, and Feeble next Scene and the Scene after";
    }
    public class DiceCardSelfAbility_AwokenWrath : DiceCardSelfAbilityBase
    {
        public override string[] Keywords
        {
            get
            {
                return new string[]
                {
                    "awoken_Keyword"
                };
            }
        }

        public override void OnStartBattle()
        {
            if (base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is DiceCardSelfAbility_AwokenWrath.BattleUnitBuf_AwokenWrath) == null)
            {
                base.owner.bufListDetail.AddBuf(new DiceCardSelfAbility_AwokenWrath.BattleUnitBuf_AwokenWrath());
            }
        }

        public static string Desc = "If there are no counter dice remaining on this page, gain 2 Strength";

        public class BattleUnitBuf_AwokenWrath : BattleUnitBuf
        {
            public override void OnRoundEnd()
            {
                int num = 0;
                if (this._owner.breakDetail.IsBreakLifeZero())
                {
                    this.Destroy();
                }
                foreach (DiceBehaviour diceBehaviour in this._owner.cardSlotDetail.keepCard.GetDiceBehaviourXmlList())
                {
                    if (diceBehaviour.Script == "hahafuckyou" || diceBehaviour.Script == "hahafuckyou")
                    {
                        num++;
                    }
                }
                if (num <= 0)
                {
                    this._owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Strength, 2, this._owner);
                }
                this.Destroy();
            }
        }
    }
    public class PassiveAbility_turbulence_startatemotion3 : PassiveAbilityBase
    {
        private void trolled()
        {

        }
        private const string Desc = "check out the enemyteamstagemanager instead";
    }
    public class DiceCardAbility_lightvoicereroll : DiceCardAbilityBase
    {
        public static string Desc = "This die is rolled 4 times; [On Hit] Inflict 3 Bleed this Scene and next Scene";
        public override void AfterAction()
        {
            if (!base.owner.IsBreakLifeZero() && this._repeatCount < 3)
            {
                this._repeatCount++;
                base.ActivateBonusAttackDice();
            }
        }
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            base.OnSucceedAttack(target);
            target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Bleeding, 3, this.owner);
            target.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Bleeding, 3, this.owner);
        }
        private int _repeatCount;
    }
    public class PassiveAbility_turbulence_startatemotion4 : PassiveAbilityBase
    {
        private void pranked()
        {

        }
        private const string Desc = "check out the enemyteamstagemanager instead";
    }
    public class PassiveAbility_turbulence_startatemotion5 : PassiveAbilityBase
    {
        private void swindled()
        {

        }
        private const string Desc = "check out the enemyteamstagemanager instead";
    }
    public class PassiveAbility_whenthemookissus : PassiveAbilityBase
    {
        public override void OnDie()
        {
            base.OnDie();
            foreach (BattleUnitModel crewmate in BattleObjectManager.instance.GetAliveList_opponent(this.owner.faction))
            {
                int sus = (int)((float)crewmate.MaxHp*0.2);
                int sabotage = (int)((float)crewmate.breakDetail.GetDefaultBreakGauge()*0.2);
                if (sus > 20)
                {
                    sus = 20;
                }
                if (sabotage > 20)
                {
                    sabotage = 20;
                }
                crewmate.RecoverHP(sus);
                crewmate.breakDetail.RecoverBreak(sabotage);
                crewmate.breakDetail.nextTurnBreak = false;
                crewmate.turnState = BattleUnitTurnState.WAIT_CARD;
                this.DestroyNegativeBuf(crewmate.bufListDetail.GetActivatedBufList());
                this.DestroyNegativeBuf(crewmate.bufListDetail.GetReadyBufList());
                this.DestroyNegativeBuf(crewmate.bufListDetail.GetReadyReadyBufList());
            }
        }
        private void DestroyNegativeBuf(List<BattleUnitBuf> bufList)
        {
            foreach (BattleUnitBuf battleUnitBuf in bufList)
            {
                if (battleUnitBuf.positiveType == BufPositiveType.Negative)
                {
                    battleUnitBuf.Destroy();
                }
            }
        }
    }
    public class EnemyTeamStageManager_turbulenceoffice : EnemyTeamStageManager
    {
        public override void OnWaveStart()
        {
            base.OnWaveStart();
            foreach (BattleUnitModel among in BattleObjectManager.instance.GetAliveList(Faction.Player))
            {
                among.allyCardDetail.DrawCards(3);
            }
            switch (Singleton<StageController>.Instance.CurrentWave)
            {
                case 1:
                    CustomMapHandler.InitCustomMap<TurbulenceMapManager>("turbulence");
                    break;
                case 2:
                    CustomMapHandler.InitCustomMap<turbulence2MapManager>("turbulence2");
                    foreach (BattleUnitModel motherfuckerofchoice in BattleObjectManager.instance.GetAliveList())
                    {
                        if (motherfuckerofchoice.emotionDetail.EmotionLevel == 0)
                        {
                            for (int i = 0; i < 3; i++)
                            {

                                motherfuckerofchoice.emotionDetail.LevelUp_Forcely(1);
                                motherfuckerofchoice.emotionDetail.CheckLevelUp();
                            }
                        }
                    }
                    break;
                case 3:
                    CustomMapHandler.InitCustomMap<Turbulence3MapManager>("turbulence3");
                    foreach (BattleUnitModel motherfuckerofchoice in BattleObjectManager.instance.GetAliveList())
                    {
                        if (motherfuckerofchoice.emotionDetail.EmotionLevel == 0)
                        {
                            for (int i = 0; i < 4; i++)
                            {

                                motherfuckerofchoice.emotionDetail.LevelUp_Forcely(1);
                                motherfuckerofchoice.emotionDetail.CheckLevelUp();
                            }
                        }
                    }
                    break;
                case 4:
                    CustomMapHandler.InitCustomMap<Turbulence4MapManager>("turbulence4");
                    foreach (BattleUnitModel motherfuckerofchoice in BattleObjectManager.instance.GetAliveList())
                    {
                        if (motherfuckerofchoice.emotionDetail.EmotionLevel == 0)
                        {
                            for (int i = 0; i < 5; i++)
                            {

                                motherfuckerofchoice.emotionDetail.LevelUp_Forcely(1);
                                motherfuckerofchoice.emotionDetail.CheckLevelUp();
                            }
                        }
                    }
                    break;

            }
            CustomMapHandler.EnforceMap();
            int emotionTotalCoinNumber = Singleton<StageController>.Instance.GetCurrentStageFloorModel().team.emotionTotalCoinNumber;
            Singleton<StageController>.Instance.GetCurrentWaveModel().team.emotionTotalBonus = emotionTotalCoinNumber + 1;
            if (BattleObjectManager.instance.GetAliveList().Exists(x => x.Book.equipeffect.PassiveList.Exists(y => y.packageId == "Project Arenuvestin")))
            {
                Singleton<StageController>.Instance.GetCurrentStageFloorModel().Defeat();
                Singleton<StageController>.Instance.EndBattle();
                UIAlarmPopup.instance.SetAlarmText("As project lead of Turbulence Office, I'm sorely disappointed that we found Xela's Mod within your list of mods. Please uninstall the mod to enjoy the full experience. -Katsuda\n" +
"i invented the xela ban: ) - Memeglow\n" +
"Hi Xela -Wes\n" +
"Remember balance is important! - Grey\n" +
"Guys, you’re covering the screen!-Vollster\n" +
"i am in the message as well\n" +
"Truly a bruh moment -012\n" +
"Bingaze - BongBong\n" +
"No anime protagonists allowed, sorry bro. - Kirisada\n" +
"XELA IS NOT REAL UWOOOOOOOOOOOOOOOOOOOOOOOOOOO - Battler\n");
            }
        }
        public class PassiveAbilityd_1300001_replace : PassiveAbilityBase
        {
            public override void OnCreated()
            {
                this.name = Singleton<PassiveDescXmlList>.Instance.GetName(1300001);
                this.desc = Singleton<PassiveDescXmlList>.Instance.GetDesc(1300001);
            }

            public override int GetDamageReduction(BattleDiceBehavior behavior)
            {
                return RandomUtil.Range(1, 2);
            }

            public override int GetBreakDamageReduction(BattleDiceBehavior behavior)
            {
                return RandomUtil.Range(1, 2);
            }
        }
        public class PassiveAbdility_230124_replace : PassiveAbilityBase
        {
            public override void OnCreated()
            {
                this.name = Singleton<PassiveDescXmlList>.Instance.GetName(230124);
                this.desc = Singleton<PassiveDescXmlList>.Instance.GetDesc(230124);
            }

            public override void BeforeRollDice(BattleDiceBehavior behavior)
            {
                bool flag = behavior.Detail == BehaviourDetail.Penetrate;
                if (flag)
                {
                    this.owner.ShowPassiveTypo(this);
                    behavior.ApplyDiceStatBonus(new DiceStatBonus
                    {
                        power = 1
                    });
                }
            }
        }

        public override void OnRoundStart()
        {
            CustomMapHandler.EnforceMap();
            int emotionTotalCoinNumber = Singleton<StageController>.Instance.GetCurrentStageFloorModel().team.emotionTotalCoinNumber;
            Singleton<StageController>.Instance.GetCurrentWaveModel().team.emotionTotalBonus = emotionTotalCoinNumber + 1;
            foreach (BattleUnitModel among in BattleObjectManager.instance.GetAliveList(Faction.Enemy))
            {
                int count = 0;
                foreach (PassiveAbilityBase passiveAbilityBase in among.passiveDetail.PassiveList)
                {
                    count++;
                }
                PassiveAbility_1300001 passiveAbility_ = among.passiveDetail.PassiveList.Find((PassiveAbilityBase y) => y is PassiveAbility_1300001) as PassiveAbility_1300001;
                if (passiveAbility_ != null)
                {
                    among.passiveDetail.DestroyPassive(passiveAbility_);
                    among.passiveDetail.RemovePassive();
                    EnemyTeamStageManager_turbulenceoffice.PassiveAbilityd_1300001_replace passiveAbility_1300001_replace = new EnemyTeamStageManager_turbulenceoffice.PassiveAbilityd_1300001_replace();
                    among.passiveDetail.PassiveList.Add(passiveAbility_1300001_replace);
                    passiveAbility_1300001_replace.Init(among);
                    passiveAbility_1300001_replace.rare = passiveAbility_.rare;
                    passiveAbility_1300001_replace.OnCreated();
                    if (count >= 12)
                    {
                        passiveAbility_1300001_replace.Hide();
                    }
                }
                PassiveAbility_230124 passiveAbility_2 = among.passiveDetail.PassiveList.Find((PassiveAbilityBase y) => y is PassiveAbility_230124) as PassiveAbility_230124;
                if (passiveAbility_2 != null)
                {
                    among.passiveDetail.DestroyPassive(passiveAbility_2);
                    among.passiveDetail.RemovePassive();
                    EnemyTeamStageManager_turbulenceoffice.PassiveAbdility_230124_replace passiveAbility_230124_replace = new EnemyTeamStageManager_turbulenceoffice.PassiveAbdility_230124_replace();
                    among.passiveDetail.PassiveList.Add(passiveAbility_230124_replace);
                    passiveAbility_230124_replace.Init(among);
                    passiveAbility_230124_replace.rare = passiveAbility_2.rare;
                    passiveAbility_230124_replace.OnCreated();
                    if (count >= 12)
                    {
                        passiveAbility_230124_replace.Hide();
                    }
                }
            }
        }
    }
    public class TurbulenceMapManager : CustomMapManager
    {
        protected internal override string[] CustomBGMs
        {
            get
            {
                return new string[]
                {
                    "stellaglow xeno.ogg"
                };
            }
        }
    }
    public class turbulence2MapManager : CustomMapManager
    {
        protected internal override string[] CustomBGMs
        {
            get
            {
                return new string[]
                {
                    "ultrakill cybergrind.ogg"
                };
            }
        }
    }
    public class Turbulence3MapManager : CustomMapManager
    {
        protected internal override string[] CustomBGMs
        {
            get
            {
                return new string[]
                {
                    "triple q japanese goblin.ogg"
                };
            }
        }
    }
    public class Turbulence4MapManager : CustomMapManager
    {
        protected internal override string[] CustomBGMs
        {
            get
            {
                return new string[]
                {
                    "xenoblade mechanical rhythm.ogg"
                };
            }
        }
    }
}
