﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GuessTheDate.Models
{
    public class QuizRepository : IQuizRepository
    {
        public Quiz GetRandomQuiz()
        {
            Task<Quiz> taskQuiz = CallWebApi();
            Quiz quiz = taskQuiz.Result;
            if (quiz != null)
            {
                return quiz;
            }
            else
            {
                return new Quiz() { Event = "No event Found!", EventYear = DateTime.Now.Year };
            }
        }

        private async Task<Quiz> CallWebApi()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));
            string beginDate = "19450000";
            string endDate = "19501231";
            string url = $@"https://www.vizgr.org/historical-events/search.php?begin_date={beginDate}&end_date={endDate}";
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();

                XmlSerializer serializer = new XmlSerializer(typeof(List<Event>), new XmlRootAttribute("result"));
                StringReader stringReader = new StringReader(responseString);
                List<Event> events = (List<Event>)serializer.Deserialize(stringReader);

                string[] date = events[0].Date.Split('/');
                Quiz quiz = new Quiz()
                {
                    Event = events[0].Description,
                    EventYear = Convert.ToInt32(date[0])
                };

                return quiz;
            }
            else
            {
                return null;
            }
        }
    }
}
