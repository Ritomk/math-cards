using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
    
    
    [Category("Card Tasks")]
    public class CalculateChance : ActionTask
    {
        [BlackboardOnly] public BBParameter<object> inputValue;
        
        [ParadoxNotion.Design.Header("First range")]
        public BBParameter<float> rangeStart1 = 5f;
        public BBParameter<float> rangeEnd1 = 15f;
        public BBParameter<float> outputStart1 = 0f;
        public BBParameter<float> outputEnd1 = 25f;

        [ParadoxNotion.Design.Header("Second range")]
        public BBParameter<float> rangeStart2 = 15f;
        public BBParameter<float> rangeEnd2 = 20f;
        public BBParameter<float> outputStart2 = 25f;
        public BBParameter<float> outputEnd2 = 100f;
        
        [BlackboardOnly] public BBParameter<float> calculatedChance = new BBParameter<float>()
            { name = "Calculated Chance" };
        [BlackboardOnly] public BBParameter<float> invertedCalculatedChance = new BBParameter<float>()
            { name = "Inverted Calculated Chance" };
        
        protected override string OnInit() {
            return null;
        }


        protected override void OnExecute() {
            float normalizedInputValue = NormalizeInput(inputValue.value);

            calculatedChance.value = Calculate(normalizedInputValue);
            invertedCalculatedChance.value = 100 - calculatedChance.value;
            EndAction(true);
        }
        
        private float NormalizeInput(object input)
        {
            if (input is int intValue)
            {
                return (float)intValue;
            }
            else if (input is float floatValue)
            {
                return floatValue;
            }
            else
            {
                Debug.LogWarning($"Unsupported input type: {input.GetType()}. Defaulting to 0.");
                return 0f;
            }
        }
        
        private float Calculate(float inputValue)
        {
            if (inputValue < rangeStart1.value)
            {
                return outputStart1.value;
            }
            else if (inputValue >= rangeStart1.value && inputValue < rangeEnd1.value)
            {
                return CalculateRange(inputValue, rangeStart1.value, rangeEnd1.value, outputStart1.value, outputEnd1.value);
            }
            else if (inputValue >= rangeStart2.value && inputValue <= rangeEnd2.value)
            {
                return CalculateRange(inputValue, rangeStart2.value, rangeEnd2.value, outputStart2.value, outputEnd2.value);
            }
            else if (inputValue > rangeEnd2.value)
            {
                return outputEnd2.value;
            }

            throw new InvalidOperationException();
        }

        private float CalculateRange(float inputValue, float rangeStart, float rangeEnd, float outputStart,
            float outputEnd)
        {
            if (rangeStart > rangeEnd)
            {
                return Mathf.Lerp(outputEnd, outputStart, (inputValue - rangeEnd) / (rangeStart - rangeEnd));
            }
            else
            {
                return Mathf.Lerp(outputStart, outputEnd, (inputValue - rangeStart) / (rangeEnd - rangeStart));
            }
        }
    }
}