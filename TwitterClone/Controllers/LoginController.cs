using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TwitterClone.Models;
using System.Web.Security;
using System.Text;
using TwitterClone.Common;
using System.Globalization;
using TwitterRepo;

namespace TwitterClone.Controllers
{
    public class LoginController : Controller
    {
        private LoginRepository loginRepository;
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignIn(Tweeps tweeple)
        {
           
            loginRepository = new LoginRepository();
            TwitterRepo.Models.Person personModel = new TwitterRepo.Models.Person();
            personModel.UserId = tweeple.UserId;
            personModel.Password = Encryptor.SHA256Hash(tweeple.Password);
            bool isValid = loginRepository.Validate(personModel);
            if (isValid)
            {
                FormsAuthentication.SetAuthCookie(tweeple.UserId, true);
                return RedirectToAction("Dashboard", new { userid = tweeple.UserId });
            }
            ViewBag.Error = "Invalid Username or Password";
            return View("Login", tweeple);
        }


        //[HttpPost]
        public ActionResult SignOut(string userid)
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            ViewBag.Message = "You have successfully logged out!";
            //return View();
            Tweeps tweeple = new Tweeps();
            return View("Login", tweeple);
        }

        public ActionResult SignUp()
        {
            Tweeps tweep = new Tweeps();
            return View(tweep);
        }

        [HttpPost]
        public ActionResult SignUp(Tweeps tweeple)
        {
            if (ModelState.IsValid)
            {
                loginRepository = new LoginRepository();
                TwitterRepo.Models.Person tweepsModel = new TwitterRepo.Models.Person();
                tweepsModel.UserId = tweeple.UserId;
                tweepsModel.Password = Encryptor.SHA256Hash(tweeple.Password);
                tweepsModel.FullName = tweeple.FullName;
                tweepsModel.Email = tweeple.Email;
                if (loginRepository.SignUp(tweepsModel))
                {
                    ViewBag.Message = "You have successfully created a Twitter Account!";
                }
                else 
                {
                    ViewBag.Error = "There is already an existing user with the same Username/Email ID.";
                }
            }
            return View(tweeple);
        }

        public ActionResult Dashboard(string userid)
        {
            loginRepository = new LoginRepository();
            DashboardModel dashModel = new DashboardModel();
            Dashboard dash = new Dashboard();
            dash = loginRepository.GetDashboardDetails(userid);
            dash.TweetList = new List<TwitterRepo.Models.Tweet>();
            dash.TweetList = loginRepository.GetAllTweets(userid);
            
            dashModel.Following = dash.Following;
            dashModel.Followers = dash.Followers;
            dashModel.TweetCount = dash.TweetCount;
            dashModel.Tweeps = new Tweeps
            {
                FullName = dash.Person.FullName,
                Id = dash.Person.Id
            };
            dashModel.LoggedInUserId = dash.Person.UserId;
            Tweet tweet = new Tweet();
            dashModel.TweetList = new List<Tweet>();
            dashModel.TweetList = MapToTweetsModel(dash.TweetList);
            return View(dashModel);
        }

        [HttpPost]
        public ActionResult PostTweet(DashboardModel dashModel)
        {
            TwitterRepo.Models.Tweet tweetModel = new TwitterRepo.Models.Tweet();
            tweetModel.UserId = dashModel.LoggedInUserId;
            tweetModel.Message = dashModel.Message;
            loginRepository = new LoginRepository();
            loginRepository.PostTweets(tweetModel);
            dashModel.TweetList = new List<Tweet>();
            dashModel.TweetList = MapToTweetsModel(loginRepository.GetAllTweets(dashModel.LoggedInUserId));
            return View("_TweetList", dashModel);
        }

        public ActionResult Error()
        {
            return View();
        }

        private List<Tweet> MapToTweetsModel(List<TwitterRepo.Models.Tweet> twList)
        {
            List<Tweet> list = new List<Tweet>();
            Tweet tweet = new Tweet();
            foreach (var item in twList)
            {
                tweet.TweetUser = item.UserId.ToUpper();
                tweet.TweetMessage = item.Message;
                tweet.TweetTime = item.Created.ToString("MM/dd/yyyy HH:mm:ss");
                list.Add(tweet);
                tweet = new Tweet();
            }
            return list;
        }

    }
}