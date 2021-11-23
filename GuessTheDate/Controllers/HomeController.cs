using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuessTheDate.Models;
using GuessTheDate.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GuessTheDate.Controllers
{
    public class HomeController : Controller
    {
        private readonly IQuizRepository quizRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDataProtector protector;

        public HomeController(IQuizRepository quizRepository, IDataProtectionProvider dataProtectionProvider, 
            DataProtectionPorposeString dataProtectionPorposeString, UserManager<ApplicationUser> userManager)
        {
            this.quizRepository = quizRepository;
            this.userManager = userManager;
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
                ViewBag.ErrorMessage = "No event found for the time interval selected, try to select a different interval of time.\n" +
                    "If youre playing with random events just start a new quiz.";
                return View("NotFound");
            }
        }

        [HttpPost]
        public async Task<ViewResult> Question(Quiz model)
        {
            if (ModelState.IsValid)
            {
                int eventYearDecrypted = Convert.ToInt32(protector.Unprotect(model.EventYear));
                int yearMissed = model.Answer - eventYearDecrypted;
                if (yearMissed < 0)
                {
                    yearMissed = yearMissed * (-1);
                }

                int points = CalculatePoints(yearMissed);              

                AnswerViewModel answer = new AnswerViewModel()
                {
                    Points = points,
                    YearMissed = yearMissed,
                    Event = model.Event,
                    EventYear = eventYearDecrypted.ToString(),
                    Answer = model.Answer
                };

                if (User.Identity.IsAuthenticated)
                {
                    var user = await userManager.FindByNameAsync(User.Identity.Name);
                    long currentPoints = user.ExPoints + points;

                    if (currentPoints >= user.PointsNextLevel)
                    {
                        user.ExPoints = currentPoints - user.PointsNextLevel;
                        user.PointsNextLevel *= 2;
                        user.Level++;
                    }
                    else
                    {
                        user.ExPoints = currentPoints;
                    }

                    await userManager.UpdateAsync(user);
                }

                return View("Answer", answer);
            }

            return View(model);
        }

        public int CalculatePoints(int yearMissed)
        {
            if (yearMissed == 0)
            {
                return 10;
            }
            else if (yearMissed == 1)
            {
                return 5;
            }
            else if (yearMissed < 5)
            {
                return 4;
            }
            else if (yearMissed < 10)
            {
                return 3;
            }
            else if (yearMissed < 20)
            {
                return 2;
            }
            else if (yearMissed < 1000)
            {
                return 5;
            }
            else
            {
                return 0;
            }
        }

        public ViewResult Rules()
        {
            return View();
        }

        public ViewResult About()
        {
            return View();
        }

        [Authorize]
        public ViewResult Ranking()
        {
            var users = userManager.Users;
            users = users.OrderByDescending(user => user.Level).ThenByDescending(user => user.ExPoints);
            return View(users);
        }
    }
}