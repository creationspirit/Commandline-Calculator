using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrvaDomacaZadaca_Kalkulator
{
    class Class1
    {
        public static void Main(string[] args)
        {
            ICalculator calculator = Factory.CreateCalculator();
            calculator.Press('2');
            calculator.Press('M');
            calculator.Press('R');

            string displayState = calculator.GetCurrentDisplayState();
            Console.WriteLine(displayState);
            Console.ReadKey();
        }
    }
}
