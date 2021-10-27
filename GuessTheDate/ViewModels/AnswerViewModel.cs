using GuessTheDate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuessTheDate.ViewModels
{
    public class AnswerViewModel : Quiz
    {
        public int Points { get; set; }
        public int YearMissed { get; set; }
    }
}
