using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuessTheDate.Models
{
    public interface IQuizRepository
    {
        Quiz GetRandomQuiz();
    }
}
