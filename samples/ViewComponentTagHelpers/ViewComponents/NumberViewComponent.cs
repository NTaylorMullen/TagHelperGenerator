using System;
using Microsoft.AspNet.Mvc;

namespace ViewComponentTagHelpers.ViewComponents
{
    public class NumberViewComponent : ViewComponent
    {
        private readonly Random _numberGenerator;

        public NumberViewComponent()
        {
            _numberGenerator = new Random();
        }

        public string Invoke(int min, int max)
        {
            return _numberGenerator.Next(min, max).ToString();
        }
    }
}
