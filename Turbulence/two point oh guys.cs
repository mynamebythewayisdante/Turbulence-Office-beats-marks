using LOR_DiceSystem;
using LOR_XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompleteDisaster
{
    public class PassiveAbility_meekminimum_hatemuscle : PassiveAbilityBase
    {
        public override int OnAddKeywordBufByCard(BattleUnitBuf buf, int stack)
        {
            if (buf.bufType == KeywordBuf.Weak)
            {
                BattleCardTotalResult battleCardResultLog = this.owner.battleCardResultLog;
                if (battleCardResultLog != null)
                {
                    battleCardResultLog.SetPassiveAbility(this);
                }
                return 1;
            }
            return base.OnAddKeywordBufByCard(buf, stack);
        }
    }
    public class PassiveAbility_meekminimum_flexsoft : PassiveAbilityBase
    {
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            base.BeforeRollDice(behavior);
            BattleCardTotalResult battleCardResultLog = this.owner.battleCardResultLog;
            if (battleCardResultLog != null)
            {
                battleCardResultLog.SetPassiveAbility(this);
            }
            if (behavior.Detail == BehaviourDetail.Hit)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = -2,
                    dmg = -6
                });
            }
            else
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = 2
                });
            }
        }
        public override void OnLoseParrying(BattleDiceBehavior behavior)
        {
            base.OnLoseParrying(behavior);
            if (behavior.Detail == BehaviourDetail.Hit)
            {
                BattleCardTotalResult battleCardResultLog = this.owner.battleCardResultLog;
                if (battleCardResultLog != null)
                {
                    battleCardResultLog.SetPassiveAbility(this);
                }
                this.owner.TakeDamage(10);
                this.owner.breakDetail.TakeBreakDamage(10);
                this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Weak, 1, this.owner);
            }
        }
    }
    public class PassiveAbility_meekminimum_unbarrierofmuscle : PassiveAbilityBase
    {
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            BattleUnitBuf activatedBuf = owner.bufListDetail.GetActivatedBuf(KeywordBuf.Weak);
            if (base.IsDefenseDice(behavior.Detail) && activatedBuf != null)
            {
                BattleCardTotalResult battleCardResultLog = this.owner.battleCardResultLog;
                if (battleCardResultLog != null)
                {
                    battleCardResultLog.SetPassiveAbility(this);
                }
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = -activatedBuf.stack
                });
            }
        }
    }
    public class DiceCardAbility_meekmin_softdrop : DiceCardAbilityBase
    {
        private bool isSus;
        public static string Desc = "[Not Hit] Inflict 1 Feeble to self next Scene 2 times";
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
        public override void OnSucceedAttack()
        {
            base.OnSucceedAttack();
            isSus = true;
        }
        public override void BeforeRollDice()
        {
            base.BeforeRollDice();
            isSus = false;
        }
        public override void AfterAction()
        {
            base.AfterAction();
            if (!isSus)
            {
                this.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Weak, 1, this.owner);
                this.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Weak, 1, this.owner);
            }
        }
    }
    public class DiceCardSelfAbility_meekmin_death : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] Take 10 damage";
        public override void OnUseCard()
        {
            base.OnUseCard();
            this.owner.TakeDamage(10, DamageType.Card_Ability);
        }
    }
    public class DiceCardAbility_meekmin_death2 : DiceCardAbilityBase
    {
        public static string Desc = "[On Hit] Take 10 Stagger damage";
        public override void OnSucceedAttack()
        {
            base.OnSucceedAttack();
            this.owner.breakDetail.TakeBreakDamage(10, DamageType.Card_Ability);
        }
    }
    public class DiceCardPriority_meekmin_imagineiftherewasnopriorityscript : DiceCardPriorityBase
    {
        public override int GetPriorityBonus(BattleUnitModel owner)
        {
            if (owner.bufListDetail.GetKewordBufStack(KeywordBuf.Weak) <= 1)
            {
                return -10;
            }
            return 10;
        }
    }
    public class DiceCardSelfAbility_meekmin_groupamongussession : DiceCardSelfAbilityBase
    {
        public static string Desc = "[Combat Start] Inflict (amount of user's Feeble) Feeble to all enemies next Scene";
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
        public override void OnStartBattle()
        {
            base.OnStartBattle();
            foreach (BattleUnitModel sussy in BattleObjectManager.instance.GetAliveList_opponent(this.owner.faction))
            {
                if (sussy != null)
                {
                    sussy.bufListDetail.AddKeywordBufByCard(KeywordBuf.Weak, this.owner.bufListDetail.GetKewordBufStack(KeywordBuf.Weak), this.owner);
                }
            }
        }
    }
    public class DiceCardSelfAbility_meekmin_theoppositeoflegday : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] Inflict 3 Bind to self next Scene";
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
            this.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Binding, 3, this.owner);
        }
    }
    public class DiceCardAbility_meekmin_theoppositeoflegday2 : DiceCardAbilityBase
    {
        public static string Desc = "[On Hit] Inflict 2 Bind to self next Scene";
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
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            base.OnSucceedAttack(target);
            this.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Binding, 2, this.owner);
        }
    }
    public class DiceCardAbility_meekmin_theoppositeoflegday3 : DiceCardAbilityBase
    {
        public static string Desc = "[On Clash Win] Inflict 2 Bind to self next Scene";
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
        public override void OnWinParrying()
        {
            base.OnWinParrying();
            this.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Binding, 2, this.owner);
        }
    }
}
