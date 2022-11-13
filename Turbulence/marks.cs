using LOR_DiceSystem;
using LOR_XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thiswasamistake
{
    public class markbase : BattleUnitBuf
    {
        public override void OnRoundEnd()
        {
            base.OnRoundEnd();
            this.Destroy();
        }
    }
    public class BattleUnitBuf_beatsmarkexsanguination : markbase
    {
        protected override string keywordId => "Mark: Exsanguination";
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            base.BeforeRollDice(behavior);
            this._owner.TakeDamage(3, DamageType.Buf);
        }
    }
    public class BattleUnitBuf_beatsmarkdebilitate : markbase
    {
        protected override string keywordId => "Mark: Debilitate";
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            base.BeforeRollDice(behavior);
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                power = -1
            });
        }
    }
    public class BattleUnitBuf_beatsmarkdripless : markbase
    {
        BehaviourDetail gamer = BehaviourDetail.None;
        public BattleUnitBuf_beatsmarkdripless()
        {
            gamer = RandomUtil.SelectOne(new List<BehaviourDetail>() { BehaviourDetail.Slash, BehaviourDetail.Penetrate, BehaviourDetail.Hit });
        }
        protected override string keywordId => "Mark: Dripless";
        public override AtkResist GetResistHP(AtkResist origin, BehaviourDetail detail)
        {
            if (detail == gamer && origin != AtkResist.Weak)
            {
                return origin - 1;
            }
            return base.GetResistHP(origin, detail);
        }
        public override AtkResist GetResistBP(AtkResist origin, BehaviourDetail detail)
        {
            if (detail == gamer && origin != AtkResist.Weak)
            {
                return origin - 1;
            }
            return base.GetResistBP(origin, detail);
        }
    }
    public class BattleUnitBuf_beatsmarkheadphones : markbase
    {

        protected override string keywordId => "Mark: Headphones";
        public override void OnTakeDamageByAttack(BattleDiceBehavior atkDice, int dmg)
        {
            base.OnTakeDamageByAttack(atkDice, dmg);
            //play a sound insert code later when the thingy is filled up
        }
    }
    public class BattleUnitBuf_beatsmarksus : markbase
    {
        protected override string keywordId => "Mark: Sus";
        public override void OnRoundStart()
        {
            base.OnRoundStart();
            //change skin
        }
        public override void OnStartParrying(BattlePlayingCardDataInUnitModel card)
        {
            base.OnStartParrying(card);
            card.target.currentDiceAction.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = 1
            });
        }
        public override void Destroy()
        {
            base.Destroy();
            //change it back
        }
    }
    public class BattleUnitBuf_beatsmarkschizophrenia : markbase
    {
        protected override string keywordId => "Mark: Schizophrenia";
        public override bool IsControllable
        {
            get
            {
                if (this._owner.faction == Faction.Player)
                {
                    return false;
                }
                return true;
            }
        }
    }
    public class BattleUnitBuf_beatsmarkbelow0 : markbase
    {
        protected override string keywordId => "Mark: Below 0";
        public override int GetDamageIncreaseRate()
        {
            return -30;
        }
        public override int GetBreakDamageIncreaseRate()
        {
            return -30;
        }
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            base.BeforeRollDice(behavior);
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                dmgRate = -30,
                breakRate = -30
            });
        }
    }
    public class BattleUnitBuf_beatsmarkfordeath : markbase
    {
        protected override string keywordId => "Mark: For Death"; 
        public override int GetDamageIncreaseRate()
        {
            return 49;
        }

        public override int GetBreakDamageIncreaseRate()
        {
            return 49;
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
    public class DiceCardSelfAbility_beatsmarkymarkermarkymarkmarkmanmarkmaryk : DiceCardSelfAbilityBase
    {
        public static string Desc = "All offensive dice on this page gain \"On Hit: Inflict a random \"Mark\" status effect next scene\"";
        public override void OnUseCard()
        {
            base.OnUseCard();
            this.card.ApplyDiceAbility(DiceMatch.AllAttackDice, new DiceCardAbility_beatsfeiawjaoewj());
        }
        public class DiceCardAbility_beatsfeiawjaoewj : DiceCardAbilityBase
        {
            public override void OnSucceedAttack(BattleUnitModel target)
            {
                base.OnSucceedAttack(target);
                BattleUnitBuf gamer = RandomUtil.SelectOne(new List<BattleUnitBuf>
                {
                    new BattleUnitBuf_beatsmarkbelow0(), new BattleUnitBuf_beatsmarkdebilitate(), new BattleUnitBuf_beatsmarkdripless(), new BattleUnitBuf_beatsmarkexsanguination(), new BattleUnitBuf_beatsmarkfordeath(), new BattleUnitBuf_beatsmarkheadphones(), new BattleUnitBuf_beatsmarkschizophrenia(), new BattleUnitBuf_beatsmarksus()
                });
                if (target != null && gamer != null)
                {
                    target.bufListDetail.AddReadyBuf(gamer);
                }
            }
        }
    }
    public class PassiveAbility_beatsinsertmarkpassiveshere : PassiveAbilityBase
    {
        public override void OnRoundStart()
        {
            base.OnRoundStart();
            List<BattleUnitBuf> fish = new List<BattleUnitBuf>
                {
                    new BattleUnitBuf_beatsmarkbelow0(), new BattleUnitBuf_beatsmarkdebilitate(), new BattleUnitBuf_beatsmarkdripless(), new BattleUnitBuf_beatsmarkexsanguination(), new BattleUnitBuf_beatsmarkfordeath(), new BattleUnitBuf_beatsmarkheadphones(), new BattleUnitBuf_beatsmarkschizophrenia(), new BattleUnitBuf_beatsmarksus()
                };
            List<BattleUnitModel> bananas = new List<BattleUnitModel>();
            foreach (BattleUnitModel gamer in BattleObjectManager.instance.GetAliveList_opponent(this.owner.faction))
            {
                if (gamer != null)
                {
                    bananas.Add(gamer);
                }
            }
            for (int i = 0; i < 5; i++)
            {
                BattleUnitModel fishy = RandomUtil.SelectOne(bananas);
                BattleUnitBuf fame = RandomUtil.SelectOne(fish);
                fish.Remove(fame);
                bananas.Remove(fishy);
                fishy.bufListDetail.AddBuf(fame);

            }
            if (Singleton<StageController>.Instance.RoundTurn == 3)
            {
                //add the page to hand (i don't know what the id is yet)
            }
        }
    }
}
