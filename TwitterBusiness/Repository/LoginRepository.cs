using System;
using System.Collections.Generic;
using System.Linq;
using TwitterRepo.Models;

namespace TwitterRepo
{
    public class LoginRepository
    {
        private TWITTEREntities context;
        public LoginRepository()
        {
            context = new TWITTEREntities();
        }
        public bool Validate(Person model)
        {
            return context.People.FirstOrDefault(p => p.UserId == model.UserId
                && p.Password== model.Password) != null ? context.People.FirstOrDefault(p => p.UserId == model.UserId
                && p.Password == model.Password).Active : false;
        }

        public List<Tweet> GetAllTweets(string userId)
        {
            Dashboard dash = new Dashboard();            
            dash.TweetList = new List<Tweet>();
            dash.TweetList = context.Tweets.Select(t => t).Where(t=> t.UserId == userId).OrderByDescending(m => m.Created).ToList();
            return dash.TweetList;
        }

        public bool SignUp(Person tweeple)
        {
            Person tweep = new Person();
            tweep = context.People.Where(p => p.UserId == tweeple.UserId || p.Email == tweeple.Email).FirstOrDefault();
            if (tweep == null)
            {
                tweeple.Active = true;
                tweeple.Joined = DateTime.Now;
                context.People.Add(tweeple);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool PostTweets(Tweet tweet)
        {
            tweet.Created = DateTime.Now;
            context.Tweets.Add(tweet);
            context.SaveChanges();
            return true;
        }

        public Dashboard GetDashboardDetails(string userId)
        {
            Dashboard dash = new Dashboard();
            dash.TweetList = new List<Tweet>();
            dash.Person = new Person();
            dash.TweetCount = context.Tweets.Select(t => t.UserId == userId).Count();
            dash.Followers = context.People.FirstOrDefault(p => p.UserId == userId).People.Count();
            dash.Following = context.People.FirstOrDefault(p => p.UserId == userId).Person1.Count();           
            dash.Person = context.People.FirstOrDefault(p => p.UserId == userId);           
            dash.TweetList = GetAllTweets(userId);
            return dash;
        }
    }

    public class Dashboard
    {
        public virtual Person Person { get; set; }
        public virtual List<Tweet> TweetList { get; set; }
        public int TweetCount { get; set; }
        public int Following { get; set; }
        public int Followers { get; set; }
        public string Message { get; set; }

    }

}
