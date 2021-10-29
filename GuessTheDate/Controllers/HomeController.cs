using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuessTheDate.Models;
using GuessTheDate.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace GuessTheDate.Controllers
{
    public class HomeController : Controller
    {
        private readonly IQuizRepository quizRepository;
        private readonly IDataProtector protector;

        public HomeController(IQuizRepository quizRepository, IDataProtectionProvider dataProtectionProvider, DataProtectionPorposeString dataProtectionPorposeString)
        {
            this.quizRepository = quizRepository;
            this.protector = dataProtectionProvider.CreateProtector(dataProtectionPorposeString.answerValue);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Question()
        {
            Quiz quiz = quizRepository.GetRandomQuiz();
            if (quiz.Event != "No event Found!")
            {
                quiz.EventYear = protector.Protect(quiz.EventYear);
                return View(quiz);
            }
            else
            {
                return View("Error"); //Pagina di errore ancora da implementare: Nessun quiz trovato per i parametri inseriti.
            }
        }

        [HttpPost]
        public ViewResult Question(Quiz model)
        {
            if (ModelState.IsValid)
            {
                int eventYearDecrypted = Convert.ToInt32(protector.Unprotect(model.EventYear));
                int yearMissed = model.Answer - eventYearDecrypted;
                if (yearMissed < 0)
                {
                    yearMissed = yearMissed * (-1);
                }

                int points = 0;
                if (yearMissed == 0)
                {
                    points = 10;
                }
                else if (yearMissed == 1)
                {
                    points = 5;
                }
                else if (yearMissed < 5)
                {
                    points = 4;
                }
                else if (yearMissed < 10)
                {
                    points = 3;
                }
                else if (yearMissed < 20)
                {
                    points = 1;
                }

                AnswerViewModel answer = new AnswerViewModel()
                {
                    Points = points,
                    YearMissed = yearMissed,
                    Event = model.Event,
                    EventYear = eventYearDecrypted.ToString(),
                    Answer = model.Answer
                };

                return View("Answer", answer);
            }

            return View(model);
        }
    }
}