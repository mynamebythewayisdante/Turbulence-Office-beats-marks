using LOR_DiceSystem;
using LOR_XML;
using Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompleteDisaster
{
    public class PassiveAbility_minospassive : PassiveAbilityBase
    {
        public override void OnRoundEnd()
        {
            base.OnRoundEnd();
            if (this.owner.bufListDetail.HasBuf<MINOSPRIMECOMBOHANDLER>())
            {
                this.owner.bufListDetail.AddBuf(new MINOSPRIMECOMBOHANDLER());
            }
        }
        public override void OnRoundEndTheLast()
        {
            base.OnRoundEndTheLast();
            if (this.owner.bufListDetail.HasBuf<MINOSPRIMECOMBOHANDLER>())
            {
                this.owner.bufListDetail.AddBuf(new MINOSPRIMECOMBOHANDLER());
            }
        }
        public override int SpeedDiceNumAdder()
        {
            if (this.owner.hp <= this.owner.MaxHp / 2)
            {
                return 6;
            }
            return 3;
        }
        bool fresh;
        public override void OnTakeDamageByAttack(BattleDiceBehavior atkDice, int dmg)
        {
            base.OnTakeDamageByAttack(atkDice, dmg);
            if (this.owner.hp <= this.owner.MaxHp / 2)
            {
                Singleton<StageController>.Instance.RoundEndForcely();
                fresh = true;
            }
        }
        public override int GetMinHp()
        {
            if (!fresh)
            {
                return this.owner.MaxHp / 2;
            }
            return base.GetMinHp();
        }
        public override int MaxPlayPointAdder()
        {
            if (this.owner.hp <= this.owner.MaxHp / 2)
            {
                return 5;
            }
            return base.MaxPlayPointAdder();
        }
        public override void OnDie()
        {
            base.OnDie();
            foreach (BattleUnitModel sussy in BattleObjectManager.instance.GetAliveList_opponent(this.owner.faction))
            {
                if (sussy != null)
                {
                    foreach (PassiveAbilityBase p in sussy.passiveDetail.PassiveList)
                    {
                        if (p != null)
                        {
                            p.disabled = false;
                        }
                        if (p is PassiveAbility_minosplayerpassiveleavemealoneaboutitnow)
                        {
                            sussy.passiveDetail.DestroyPassive(p);
                        }
                    }
                }
            }
        }
        public override int GetDamageReduction(BattleDiceBehavior behavior)
        {
            this.owner.TakeDamage((int)(behavior.DiceResultValue * 0.75));
            if (behavior.owner != null)
            {
                behavior.owner.RecoverHP(behavior.DiceResultValue / 2);
            }
            return behavior.DiceResultValue + 9999;
        }
        public override bool isStraighten => true;
        public override void OnLoseParrying(BattleDiceBehavior behavior)
        {
            base.OnLoseParrying(behavior);
            this.owner.cardSlotDetail.RecoverPlayPoint(1);
        }
        public override void OnRoundStart()
        {
            base.OnRoundStart();
            if (this.owner.bufListDetail.HasBuf<MINOSPRIMECOMBOHANDLER>())
            {
                this.owner.bufListDetail.AddBuf(new MINOSPRIMECOMBOHANDLER());
            }
            owner.allyCardDetail.DrawCards(6);
            if (this.owner.hp > this.owner.MaxHp / 2)
            {
                this.owner.cardSlotDetail.SpendCost(1);
                if (this.owner.cardSlotDetail.PlayPoint <= 0)
                {
                    this.owner.cardSlotDetail.RecoverPlayPoint(this.owner.cardSlotDetail.GetMaxPlayPoint());
                }
            }
            else
            {
                this.owner.cardSlotDetail.RecoverPlayPoint(this.owner.cardSlotDetail.GetMaxPlayPoint());
            }
        }
        public override void OnRoundStartAfter()
        {
            base.OnRoundStartAfter();
            if (this.owner.bufListDetail.HasBuf<MINOSPRIMECOMBOHANDLER>())
            {
                this.owner.bufListDetail.AddBuf(new MINOSPRIMECOMBOHANDLER());
            }
        }
        public override void OnWaveStart()
        {
            base.OnWaveStart();
            for (int i = 0; i < 5; i++)
            {
                this.owner.emotionDetail.LevelUp_Forcely(1);
                this.owner.emotionDetail.CheckLevelUp();
            }
            foreach (BattleUnitModel sussy in BattleObjectManager.instance.GetAliveList_opponent(this.owner.faction))
            {
                if (sussy != null)
                {
                    foreach (PassiveAbilityBase p in sussy.passiveDetail.PassiveList)
                    {
                        if (p != null)
                        {
                            p.disabled = true;
                        }
                    }
                    sussy.passiveDetail.AddPassive(new LorId("LuxCollection", 666));
                }
            }
        }

        public override void OnBattleEnd_alive()
        {
            base.OnBattleEnd_alive();
            owner.RecoverHP(owner.MaxHp);
        }

        public override bool IsImmune(KeywordBuf buf)
        {
            switch (buf)
            {
                case KeywordBuf.Stun:
                    return true;
                default:
                    return false;
            }
        }
    }
    public class PassiveAbility_minosplayerpassiveleavemealoneaboutitnow : PassiveAbilityBase
    {
        public override int SpeedDiceNumAdder()
        {
            return 2;
        }
        public override bool isStraighten => true;


        public override AtkResist GetResistHP(AtkResist origin, BehaviourDetail detail)
        {
            return AtkResist.Normal;
        }

        public override AtkResist GetResistBP(AtkResist origin, BehaviourDetail detail)
        {
            return AtkResist.Normal;
        }
    }
    public class MINOSPRIMECOMBOHANDLER : BattleUnitBuf
    {
        bool gamer;
        bool gamernt;
        public class addthings2 : BattleUnitBuf
        {
            public override void OnRoundEnd()
            {
                base.OnRoundEnd();
                Destroy();
            }
            public override void BeforeRollDice(BattleDiceBehavior behavior)
            {
                base.BeforeRollDice(behavior);
                behavior.AddAbility(new DiceCardAbility_lose30powerboowomp());
            }
        }
        public override void OnUseCard(BattlePlayingCardDataInUnitModel card)
        {
            base.OnUseCard(card);
            gamer = false;
            gamernt = false;
            if (card.cardAbility is DiceCardSelfAbility_DIEchaineffect || card.cardAbility is DiceCardSelfAbility_Uppercutchain || card.cardAbility is DiceCardSelfAbility_Dunkchain)
            {
                if (!_owner.bufListDetail.HasBuf<addthings>())
                {
                    _owner.bufListDetail.AddBuf(new addthings());
                }
            }
            if (card.cardAbility is DiceCardSelfAbility_serpentprojectileonclash)
            {
                if (!_owner.bufListDetail.HasBuf<addthings2>())
                {
                    _owner.bufListDetail.AddBuf(new addthings2());
                }
            }
        }
        public class addthings : BattleUnitBuf
        {
            public override void OnRoundEnd()
            {
                base.OnRoundEnd();
                Destroy();
            }
            public override void BeforeRollDice(BattleDiceBehavior behavior)
            {
                base.BeforeRollDice(behavior);
                if (behavior.abilityList.Find((DiceCardAbilityBase x) => x is DiceCardAbility_10powerdownvsevade) == null)
                {
                    behavior.AddAbility(new DiceCardAbility_10powerdownvsevade());
                }
            }
        }
        public override void OnSuccessAttack(BattleDiceBehavior behavior)
        {
            base.OnSuccessAttack(behavior);
            if (behavior.card.cardAbility is DiceCardSelfAbility_DIEchaineffect)
            {
                gamer = true;
            }
            if (behavior.card.cardAbility is DiceCardSelfAbility_Uppercutchain)
            {
                gamer = true;
            }
        }
        public override void OnLoseParrying(BattleDiceBehavior behavior)
        {
            base.OnLoseParrying(behavior);
            if (behavior.card.cardAbility is DiceCardSelfAbility_Uppercutchain || behavior.card.cardAbility is DiceCardSelfAbility_Dunkchain)
            {
                gamernt = true;
            }
        }
        public override void OnEndBattle(BattlePlayingCardDataInUnitModel curCard)
        {
            base.OnEndBattle(curCard);
            if (curCard.cardAbility is DiceCardSelfAbility_DIEchaineffect)
            {
                BattleUnitBuf thing = _owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is addthings);
                if (thing != null)
                {
                    _owner.bufListDetail.RemoveBuf(thing);
                }
                dudetrackerdudetracker dude = _owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is dudetrackerdudetracker) as dudetrackerdudetracker;
                if (gamer && dude != null)
                {
                    int gameing = 0;
                    foreach (BattlePlayingCardDataInUnitModel among in _owner.cardSlotDetail.cardAry)
                    {
                        if (among != curCard && among != null && !dude.list.Contains(among))
                        {
                            gameing += among.card.CurCost;
                        }
                    }
                    if (_owner.cardSlotDetail.PlayPoint - gameing > 0)
                    {
                        _owner.cardSlotDetail.SpendCost(1);
                        var target = curCard.target;
                        var b = new BattlePlayingCardDataInUnitModel
                        {
                            card = BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(new LorId("TurbulenceOffice", 1112))),
                            owner = _owner,
                            earlyTarget = target,
                            earlyTargetOrder = UnityEngine.Random.Range(0, target.speedDiceResult.Count),
                            slotOrder = 0,
                            speedDiceResultValue = _owner.GetSpeed(0),
                            cardAbility = new DiceCardSelfAbility_Uppercutchain()
                        };
                        b.owner = this._owner;
                        b.target = b.earlyTarget;
                        b.targetSlotOrder = b.earlyTargetOrder;
                        b.ResetCardQueue();
                        StageController.Instance.AddAllCardListInBattle(b, target);
                    }
                }
            }
            else if (curCard.cardAbility is DiceCardSelfAbility_Uppercutchain)
            {
                BattleUnitBuf thing = _owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is addthings);
                if (thing != null)
                {
                    _owner.bufListDetail.RemoveBuf(thing);
                }
                dudetrackerdudetracker dude = _owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is dudetrackerdudetracker) as dudetrackerdudetracker;
                if (gamer && dude != null)
                {
                    int gameing = 0;
                    foreach (BattlePlayingCardDataInUnitModel among in _owner.cardSlotDetail.cardAry)
                    {
                        if (among != curCard && among != null && !dude.list.Contains(among))
                        {
                            gameing += among.card.CurCost;
                        }
                    }
                    if (_owner.cardSlotDetail.PlayPoint - gameing > 0)
                    {
                        _owner.cardSlotDetail.SpendCost(1);
                        var target = curCard.target;
                        var b = new BattlePlayingCardDataInUnitModel
                        {
                            card = BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(new LorId("TurbulenceOffice", 1113))),
                            owner = _owner,
                            earlyTarget = target,
                            earlyTargetOrder = UnityEngine.Random.Range(0, target.speedDiceResult.Count),
                            slotOrder = 0,
                            speedDiceResultValue = _owner.GetSpeed(0),
                            cardAbility = new DiceCardSelfAbility_Dunkchain()
                        };
                        b.owner = this._owner;
                        b.target = b.earlyTarget;
                        b.targetSlotOrder = b.earlyTargetOrder;
                        b.ResetCardQueue();
                        StageController.Instance.AddAllCardListInBattle(b, target);
                    }
                }
                if (gamernt && dude != null)
                {
                    var enemies = BattleObjectManager.instance.GetAliveList_opponent(_owner.faction);
                    if (enemies.Count <= 0) return;
                    var target = RandomUtil.SelectOne(enemies);
                    var b = new BattlePlayingCardDataInUnitModel
                    {
                        card = BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(new LorId("TurbulenceOffice", 1111))),
                        owner = _owner,
                        earlyTarget = target,
                        earlyTargetOrder = UnityEngine.Random.Range(0, target.speedDiceResult.Count),
                        slotOrder = 0,
                        speedDiceResultValue = _owner.GetSpeed(0),
                        cardAbility = new DiceCardSelfAbility_serpentprojectileonclash()
                    };
                    b.owner = this._owner;
                    b.target = b.earlyTarget;
                    b.targetSlotOrder = b.earlyTargetOrder;
                    b.ResetCardQueue();
                    StageController.Instance.AddAllCardListInBattle(b, target);
                }
            }
            else if (curCard.cardAbility is DiceCardSelfAbility_Dunkchain)
            {
                BattleUnitBuf thing = _owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is addthings);
                if (thing != null)
                {
                    _owner.bufListDetail.RemoveBuf(thing);
                }
                dudetrackerdudetracker dude = _owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is dudetrackerdudetracker) as dudetrackerdudetracker;
                if (gamernt && dude != null)
                {
                    var enemies = BattleObjectManager.instance.GetAliveList_opponent(_owner.faction);
                    if (enemies.Count <= 0) return;
                    var target = RandomUtil.SelectOne(enemies);
                    var b = new BattlePlayingCardDataInUnitModel
                    {
                        card = BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(new LorId("TurbulenceOffice", 1111))),
                        owner = _owner,
                        earlyTarget = target,
                        earlyTargetOrder = UnityEngine.Random.Range(0, target.speedDiceResult.Count),
                        slotOrder = 0,
                        speedDiceResultValue = _owner.GetSpeed(0),
                        cardAbility = new DiceCardSelfAbility_serpentprojectileonclash()
                    };
                    b.owner = this._owner;
                    b.target = b.earlyTarget;
                    b.targetSlotOrder = b.earlyTargetOrder;
                    b.ResetCardQueue();
                    StageController.Instance.AddAllCardListInBattle(b, target);
                }
            }
            else
            {
                BattleUnitBuf thing = _owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is addthings2);
                if (thing != null)
                {
                    _owner.bufListDetail.RemoveBuf(thing);
                }
            }
        }
        protected override string keywordIconId => "stolenpage";
    }
    public class DiceCardSelfAbility_ohnoyouclashedwithme : DiceCardSelfAbilityBase
    {
        public override void OnStartParrying()
        {
            card?.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = -40
            });
        }
    }
    public class DiceCardAbility_take60damagedumbass : DiceCardAbilityBase
    {
        public override void OnLoseParrying()
        {
            owner?.TakeDamage(60);
            card?.target?.TakeDamage(10);
        }
    }

    public class DiceCardSelfAbility_JUDGEMENTchain : DiceCardSelfAbilityBase
    {
        bool gamer;
        public override void OnStartBattle()
        {
            base.OnStartBattle();
            if (!owner.bufListDetail.HasBuf<dudetrackerdudetracker>())
            {
                owner.bufListDetail.AddBuf(new dudetrackerdudetracker());
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
                foreach (BattlePlayingCardDataInUnitModel among in owner.cardSlotDetail.cardAry)
                {
                    if (among != card && among != null && !dude.list.Contains(among))
                    {
                        gameing += among.card.CurCost;
                    }
                }
                if (owner.cardSlotDetail.PlayPoint - gameing > 0)
                {
                    owner.cardSlotDetail.SpendCost(1);
                    var target = card.target;
                    var b = new BattlePlayingCardDataInUnitModel
                    {
                        card = BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(new LorId("TurbulenceOffice", 1105))),
                        owner = owner,
                        earlyTarget = target,
                        earlyTargetOrder = UnityEngine.Random.Range(0, target.speedDiceResult.Count),
                        slotOrder = 0,
                        speedDiceResultValue = owner.GetSpeed(0),
                        cardAbility = new DiceCardSelfAbility_DIEchaineffect()

                    };
                    b.owner = this.owner;
                    b.target = b.earlyTarget;
                    b.targetSlotOrder = b.earlyTargetOrder;
                    b.ResetCardQueue();
                    StageController.Instance.AddAllCardListInBattle(b, target);
                }
            }
        }
    }

    public class DiceCardSelfAbility_DIEnonchaineffect : DiceCardSelfAbilityBase
    {
        public override void OnStartParrying()
        {
            foreach (BattleDiceBehavior amongus in card.GetDiceBehaviorList())
            {
                if (amongus != null)
                {
                    amongus.AddAbility(new DiceCardAbility_5powerdownvsevade());
                }
            }
        }
    }

    public class DiceCardSelfAbility_DIEchaineffect : DiceCardSelfAbilityBase
    {
        bool gamer;
        public override void OnSucceedAttack()
        {
            base.OnSucceedAttack();
            gamer = true;
        }
    }

    public class DiceCardAbility_5powerdownvsevade : DiceCardAbilityBase
    {
        public override void BeforeRollDice()
        {
            if (behavior?.TargetDice?.Detail == BehaviourDetail.Evasion)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = -5
                });
            }
        }
    }

    public class DiceCardAbility_10powerdownvsevade : DiceCardAbilityBase
    {
        public override void BeforeRollDice()
        {
            if (behavior?.TargetDice?.Detail == BehaviourDetail.Evasion)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = -10
                });
            }
        }
    }

    public class DiceCardSelfAbility_serpentprojectile : DiceCardSelfAbilityBase
    {
        public override void OnStartBattle()
        {
            base.OnStartBattle();
            if (!owner.bufListDetail.HasBuf<dudetrackerdudetracker>())
            {
                owner.bufListDetail.AddBuf(new dudetrackerdudetracker());
            }
        }
        public override void OnEndBattle()
        {
            base.OnEndBattle();
            dudetrackerdudetracker dude = owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is dudetrackerdudetracker) as dudetrackerdudetracker;
            if (dude != null)
            {
                int gameing = 0;
                foreach (BattlePlayingCardDataInUnitModel among in owner.cardSlotDetail.cardAry)
                {
                    if (among != card && among != null && !dude.list.Contains(among))
                    {
                        gameing += among.card.CurCost;
                    }
                }
                var enemies = BattleObjectManager.instance.GetAliveList_opponent(owner.faction);
                if (enemies.Count <= 0) return;
                var target = RandomUtil.SelectOne(enemies);
                var b = new BattlePlayingCardDataInUnitModel
                {
                    card = BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(new LorId("TurbulenceOffice", 1111))),
                    owner = owner,
                    earlyTarget = target,
                    earlyTargetOrder = UnityEngine.Random.Range(0, target.speedDiceResult.Count),
                    slotOrder = 0,
                    speedDiceResultValue = owner.GetSpeed(0),
                    cardAbility = new DiceCardSelfAbility_serpentprojectileonclash()
                };
                b.owner = this.owner;
                b.target = b.earlyTarget;
                b.targetSlotOrder = b.earlyTargetOrder;
                b.ResetCardQueue();
                StageController.Instance.AddAllCardListInBattle(b, target);
            }
        }
    }

    public class DiceCardSelfAbility_thyendisnowparry : DiceCardSelfAbilityBase
    {
        public override void OnStartParrying()
        {
            foreach (BattleDiceBehavior amongus in card.GetDiceBehaviorList())
            {
                if (amongus != null)
                {
                    amongus.AddAbility(new DiceCardAbility_10powerdownvsevade());
                }
            }
        }
    }

    public class DiceCardAbility_thyendisnowlastdieparry : DiceCardAbilityBase
    {
        public override void BeforeRollDice()
        {
            if (behavior?.TargetDice?.Type == BehaviourType.Atk)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = -25
                });
            }
        }

        public override void OnLoseParrying()
        {
            owner?.TakeDamage(60);
            card?.target?.TakeDamage(10);
        }
    }

    public class DiceCardAbility_lose15powerdumbass : DiceCardAbilityBase
    {
        public override void BeforeRollDice()
        {
            if (behavior.TargetDice.Detail == BehaviourDetail.Evasion)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = -15
                });
            }
        }
    }

    public class DiceCardSelfAbility_CRUSHchain : DiceCardSelfAbilityBase
    {
        bool gamer;
        public override void OnStartBattle()
        {
            base.OnStartBattle();
            if (!owner.bufListDetail.HasBuf<dudetrackerdudetracker>())
            {
                owner.bufListDetail.AddBuf(new dudetrackerdudetracker());
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
                foreach (BattlePlayingCardDataInUnitModel among in owner.cardSlotDetail.cardAry)
                {
                    if (among != card && among != null && !dude.list.Contains(among))
                    {
                        gameing += among.card.CurCost;
                    }
                }
                if (owner.cardSlotDetail.PlayPoint - gameing > 0)
                {
                    owner.cardSlotDetail.SpendCost(2);
                    var target = card.target;
                    int amongus = RandomDeath();
                    switch (amongus)
                    {
                        case 1101:
                            var b = new BattlePlayingCardDataInUnitModel
                            {
                                card = BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(new LorId("TurbulenceOffice", amongus))),
                                owner = owner,
                                earlyTarget = target,
                                earlyTargetOrder = UnityEngine.Random.Range(0, target.speedDiceResult.Count),
                                slotOrder = 0,
                                speedDiceResultValue = owner.GetSpeed(0)
                            };
                            b.owner = this.owner;
                            b.target = b.earlyTarget;
                            b.targetSlotOrder = b.earlyTargetOrder;
                            b.ResetCardQueue();
                            StageController.Instance.AddAllCardListInBattle(b, target);
                            break;
                        case 1103:
                            b = new BattlePlayingCardDataInUnitModel
                            {
                                card = BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(new LorId("TurbulenceOffice", amongus))),
                                owner = owner,
                                earlyTarget = target,
                                earlyTargetOrder = UnityEngine.Random.Range(0, target.speedDiceResult.Count),
                                slotOrder = 0,
                                speedDiceResultValue = owner.GetSpeed(0)
                            };
                            b.owner = this.owner;
                            b.target = b.earlyTarget;
                            b.targetSlotOrder = b.earlyTargetOrder;
                            b.ResetCardQueue();
                            StageController.Instance.AddAllCardListInBattle(b, target);
                            break;
                        case 1107:
                            b = new BattlePlayingCardDataInUnitModel
                            {
                                card = BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(new LorId("TurbulenceOffice", amongus))),
                                owner = owner,
                                earlyTarget = target,
                                earlyTargetOrder = UnityEngine.Random.Range(0, target.speedDiceResult.Count),
                                slotOrder = 0,
                                speedDiceResultValue = owner.GetSpeed(0)
                            };
                            b.owner = this.owner;
                            b.target = b.earlyTarget;
                            b.targetSlotOrder = b.earlyTargetOrder;
                            b.ResetCardQueue();
                            StageController.Instance.AddAllCardListInBattle(b, target);
                            break;
                    }
                }
            }
        }
        public static int RandomDeath()
        {
            return RandomUtil.SelectOne<int>(new int[]
            {
                1107,
                1103,
                1101
            });
        }
    }

    public class DiceCardSelfAbility_serpentprojectileonclash : DiceCardSelfAbilityBase
    {
    }

    public class DiceCardAbility_lose30powerboowomp : DiceCardAbilityBase
    {
        public override void BeforeRollDice()
        {
            if (behavior?.TargetDice?.Type == BehaviourType.Atk || IsAttackDice(behavior.TargetDice.Detail))
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = -30
                });
            }
        }
    }
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
            Destroy();
        }
    }

    public class DiceCardAbility_fullhpyippee : DiceCardAbilityBase
    {
        public override void OnLoseParrying()
        {
            card.target?.RecoverHP(card.target.MaxHp);
        }
    }
    public class DiceCardSelfAbility_Uppercutchain : DiceCardSelfAbilityBase
    {
        bool gamer;
        bool gamernt;
        public override void OnSucceedAttack()
        {
            base.OnSucceedAttack();
            gamer = true;
        }

        public override void OnLoseParrying()
        {
            gamernt = true;
        }
    }

    public class DiceCardSelfAbility_Dunkchain : DiceCardSelfAbilityBase
    {
        bool gamernt;
        public override void OnSucceedAttack()
        {
            base.OnSucceedAttack();
            card.target?.bufListDetail.AddReadyBuf(new BattleUnitBuf_sealTemp());
        }

        public override void OnLoseParrying()
        {
            gamernt = true;
        }
    }

    /* public class DiceCardSelfAbility_gamer : DiceCardSelfAbilityBase
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
                 Destroy();
             }
         }
         public override void OnStartBattle()
         {
             base.OnStartBattle();
             if (!owner.bufListDetail.HasBuf<dudetrackerdudetracker>())
             {
                 owner.bufListDetail.AddBuf(new dudetrackerdudetracker());
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
                 foreach (BattlePlayingCardDataInUnitModel among in owner.cardSlotDetail.cardAry)
                 {
                     if (among != card && among != null && !dude.list.Contains(among))
                     {
                         gameing += among.card.CurCost;
                     }
                 }
                 if (owner.cardSlotDetail.PlayPoint - gameing > 0)
                 {
                     owner.cardSlotDetail.SpendCost(1);
                     var enemies = BattleObjectManager.instance.GetAliveList_opponent(owner.faction);
                     if (enemies.Count <= 0) return;
                     var target = RandomUtil.SelectOne(enemies);
                     var b = new BattlePlayingCardDataInUnitModel
                     {
                         card = BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(new LorId("LuxCollection", 9))),
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
     } */
}
