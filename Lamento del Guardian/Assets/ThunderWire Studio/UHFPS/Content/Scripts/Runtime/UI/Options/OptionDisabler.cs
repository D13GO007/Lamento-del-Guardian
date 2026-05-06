using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UHFPS.Runtime
{
    public class OptionDisabler : MonoBehaviour
    {
        public enum VisibilityAction
        {
            Show,
            Hide,
            Disable
        }

        public enum ConditionType
        {
            Equals,
            NotEquals,
            GreaterThan,
            LessThan,
            GreaterOrEqual,
            LessOrEqual
        }
        
        public enum ValueType
        {
            Int,
            Float,
            Bool
        }
        
        [Serializable]
        public sealed class OptionArgument
        {
            public ValueType ValueType;
            public int IntValue;
            public float FloatValue;
            public bool BoolValue;
        }
        
        [Serializable]
        public sealed class DisplayCondition
        {
            public OptionBehaviour SampleOption;
            public ConditionType Condition;
            public VisibilityAction Action;
            public uint Priority;
            public OptionArgument TriggerValue;
        }

        public GameObject TargetOption;
        public float DisabledAlpha = 0.5f;
        public List<DisplayCondition> DisplayConditions = new();

        public void OnSetVisibility(VisibilityAction action)
        {
            switch (action)
            {
                case VisibilityAction.Show:
                    TargetOption.SetActive(true);
                    break;
                case VisibilityAction.Hide:
                    TargetOption.SetActive(false);
                    break;
                case VisibilityAction.Disable:
                    TargetOption.SetActive(true);
                    break;
            }
            
            CanvasGroup canvasGroup = TargetOption.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                bool resultActive = action != VisibilityAction.Hide;
                bool resultDisabled = action != VisibilityAction.Disable;
                
                canvasGroup.alpha = resultDisabled ? 1f : DisabledAlpha;
                canvasGroup.blocksRaycasts = resultActive;
            }
        }
        
        public void EvaluateVisibility()
        {
            if (DisplayConditions == null || DisplayConditions.Count == 0)
            {
                OnSetVisibility(VisibilityAction.Show);
                return;
            }
            
            VisibilityAction resultAction = VisibilityAction.Show;
            
            // Sort conditions by higher priority first
            IEnumerable<DisplayCondition> displayConditions = DisplayConditions.OrderByDescending(c => c.Priority);
            
            foreach (DisplayCondition condition in displayConditions)
            {
                OptionBehaviour option = condition.SampleOption;
                if (option == null)
                    continue;

                object value = option.GetOptionValue();
                if (EvaluateCondition(value, condition))
                {
                    resultAction = condition.Action;
                    break;
                }
            }
            
            // Finally set the visibility
            OnSetVisibility(resultAction);
        }
        
        private bool EvaluateCondition(object currentValue, DisplayCondition condition)
        {
            switch (condition.TriggerValue.ValueType)
            {
                case ValueType.Int:
                    if (currentValue is int intValue)
                    {
                        return condition.Condition switch
                        {
                            ConditionType.Equals => intValue == condition.TriggerValue.IntValue,
                            ConditionType.NotEquals => intValue != condition.TriggerValue.IntValue,
                            ConditionType.GreaterThan => intValue > condition.TriggerValue.IntValue,
                            ConditionType.LessThan => intValue < condition.TriggerValue.IntValue,
                            ConditionType.GreaterOrEqual => intValue >= condition.TriggerValue.IntValue,
                            ConditionType.LessOrEqual => intValue <= condition.TriggerValue.IntValue,
                            _ => false
                        };
                    }
                    break;
                case ValueType.Float:
                    if (currentValue is float floatValue)
                    {
                        return condition.Condition switch
                        {
                            ConditionType.Equals => Mathf.Approximately(floatValue, condition.TriggerValue.FloatValue),
                            ConditionType.NotEquals => !Mathf.Approximately(floatValue, condition.TriggerValue.FloatValue),
                            ConditionType.GreaterThan => floatValue > condition.TriggerValue.FloatValue,
                            ConditionType.LessThan => floatValue < condition.TriggerValue.FloatValue,
                            ConditionType.GreaterOrEqual => floatValue >= condition.TriggerValue.FloatValue,
                            ConditionType.LessOrEqual => floatValue <= condition.TriggerValue.FloatValue,
                            _ => false
                        };
                    }
                    break;
                case ValueType.Bool:
                    if (currentValue is bool boolValue)
                    {
                        return condition.Condition switch
                        {
                            ConditionType.Equals => boolValue == condition.TriggerValue.BoolValue,
                            ConditionType.NotEquals => boolValue != condition.TriggerValue.BoolValue,
                            _ => false
                        };
                    }
                    break;
                default:
                    throw new NotImplementedException("[OptionModuleBase] EvaluateCondition does not support ValueType: " + condition.TriggerValue.ValueType);
            }
            
            return false;
        }
    }
}