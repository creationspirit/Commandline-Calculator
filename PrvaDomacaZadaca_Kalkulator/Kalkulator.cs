using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PrvaDomacaZadaca_Kalkulator
{
    public class Factory
    {
        public static ICalculator CreateCalculator()
        {
            // vratiti kalkulator
            return new Kalkulator();
        }
    }

    class Kalkulator : ICalculator
    {
        private const string unaryOperations = "MSKTQRI";
        private const string binaryOperations = "+-*/";

        private Display display;
        private Temporary temporary;
        private double memory;

        public Kalkulator()
        {
            display = new Display();
            temporary = null;
            memory = Double.NaN;
        }

        public string GetCurrentDisplayState()
        {
            return display.DisplayString;
        }

        public void Press(char inPressedDigit)
        {
            if (Char.IsDigit(inPressedDigit))
            {
                display.AddSignIfSpace(inPressedDigit);
            }
            if(Char.ToString(inPressedDigit) == ",")
            {
                display.AddCommaIfNotExist();
            }
            else if(unaryOperations.Contains(inPressedDigit)) 
            {
                double result = ExecuteUnary(display.GetAsDouble(), inPressedDigit);
                display.SetDisplay(result);
            }
            else if(binaryOperations.Contains(inPressedDigit))
            {   
                if(temporary == null)
                {
                    temporary = new Temporary(display.GetAsDouble(), inPressedDigit);
                }
                else if(display.LastWasBinaryOrEqual)
                {
                    temporary.Operation = inPressedDigit;
                }
                else
                {
                    temporary.Value = ExecuteBinary(temporary.Value, display.GetAsDouble(), temporary.Operation);
                    temporary.Operation = inPressedDigit;
                }
                display.LastWasBinaryOrEqual = true;
                display.TrimZeroes();
            }
            else if(Char.ToString(inPressedDigit) == "O")
            {
                display = new Display();
                temporary = null;
            }
            else if(Char.ToString(inPressedDigit) == "=")
            {
                if(display.LastWasBinaryOrEqual == true && temporary != null)
                {
                    double result = ExecuteBinary(temporary.Value, temporary.Value, temporary.Operation);
                    display.SetDisplay(result);
                    temporary = null;
                }
                else if (temporary == null)
                {
                    display.TrimZeroes();
                }
                else
                {
                    double result = ExecuteBinary(temporary.Value, display.GetAsDouble(), temporary.Operation);
                    display.SetDisplay(result);
                    temporary = null;
                }
                display.LastWasBinaryOrEqual = true;
            }
            else if(Char.ToString(inPressedDigit) == "P")
            {
                memory = display.GetAsDouble();
                display.TrimZeroes();
            }
            else if (Char.ToString(inPressedDigit) == "G")
            {
                display.SetDisplay(memory);
                display.LastWasBinaryOrEqual = false;
                display.TrimZeroes();
            }
            else if (Char.ToString(inPressedDigit) == "C")
            {
                display = new Display();
            }
        }

        public double ExecuteUnary(double number, char operation)
        {
            switch (Char.ToString(operation))
            {
                case "M":
                    if(display.DisplayString != "0") return number * (-1);
                    else return number * (-1);
                case "S":
                    return Math.Sin(number);
                case "K":
                    return Math.Cos(number);
                case "T":
                    return Math.Tan(number);
                case "Q":
                    return Math.Pow(number, 2);
                case "R":
                    return Math.Sqrt(number);
                case "I":
                    return 1.0 / number;
                default:
                    return Double.NaN;
            }
        }

        public double ExecuteBinary(double number1, double number2, char operation)
        {
            switch (Char.ToString(operation))
            {
                case "+":
                    return number1 + number2;
                case "-":
                    return number1 - number2;
                case "*":
                    return number1 * number2;
                case "/":
                    return number1 / number2;
                default:
                    return Double.NaN;
            }
        }

        private class Display
        {
            public string DisplayString { get; private set; }
            private int digitCount;
            private CultureInfo culture;
            public bool LastWasBinaryOrEqual { get; set; }

            public Display()
            {
                DisplayString = "0";
                digitCount = 1;
                culture = new CultureInfo("hr-HR");
                LastWasBinaryOrEqual = false;
            }

            private bool HasSpace()
            {
                if (digitCount < 10) return true;
                else return false;
            }

            public void AddSignIfSpace(char sign)
            {
                if (LastWasBinaryOrEqual)
                {
                    EraseDisplay();
                    LastWasBinaryOrEqual = false;
                }
                if(DisplayString == "0" && digitCount == 1)
                {
                    DisplayString = Char.ToString(sign);
                    return;
                }
                else if (HasSpace())
                {
                    DisplayString = DisplayString + sign;
                    digitCount++;
                }
            }

            public void AddCommaIfNotExist()
            {
                if(!DisplayString.Contains(",")) DisplayString = DisplayString + ",";
            }

            public void EraseDisplay()
            {
                DisplayString = "0";
                digitCount = 1;
            }

            public double GetAsDouble()
            {
                return Double.Parse(DisplayString);
            }

            public void SetDisplay(double value)
            {
                if (Double.IsNaN(value) || Double.IsInfinity(value))
                {
                    DisplayString = "-E-";
                    return;
                }
                int wholeDigitLength = WholeDigitLength(value);
                if(wholeDigitLength > 10)
                {
                    DisplayString = "-E-";
                    return;
                }
                value = Math.Round(value, 10 - wholeDigitLength);
                string number = value.ToString(culture);
                DisplayString = number;

                TrimZeroes();
            }

            private int WholeDigitLength(double number)
            {
                number = Math.Abs(number);
                int length = 1;
                while ((number /= 10) >= 1)
                    length++;
                return length;
            }

            public void TrimZeroes()
            {
                if(DisplayString.Contains(","))
                {
                    while (DisplayString.EndsWith("0"))
                    {
                        DisplayString = DisplayString.Remove(DisplayString.Length - 1);
                    }
                }
                if(DisplayString.EndsWith(","))
                {
                    DisplayString = DisplayString.Remove(DisplayString.Length - 1);
                }
            }

        }

        private class Temporary
        {
            public double Value { get; set; }
            public char Operation { get; set; }

            public Temporary(double value, char operation)
            {
                Value = value;
                Operation = operation;
            }
        }
    }
}
