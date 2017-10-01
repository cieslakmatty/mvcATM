using AutomatedTellerMachine.Models;
using AutomatedTellerMachine.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace AutomatedTellerMachine.Controllers {

    [Authorize]
    public class TransactionController : Controller {

        private IApplicationDbContext db;

        public TransactionController() {
            db = new ApplicationDbContext();
        }

        public TransactionController(IApplicationDbContext dbContext) {
            db = dbContext;
        }

        public ActionResult Deposit(int checkingAccountId) {
            return View();
        }

        [HttpPost]
        public ActionResult Deposit(Transaction transaction) {
            if (ModelState.IsValid) {
                if (transaction.Amount <= 0) {
                    //make sure its always positive
                    transaction.Amount *= -1;
                }
                db.Transactions.Add(transaction);
                db.SaveChanges();
                var service = new CheckingAccountService(db);
                service.UpdateBalance(transaction.CheckingAccountId);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult Withdraw(int checkingAccountId) {
            return View();
        }


        [HttpPost]
        public ActionResult Withdraw(Transaction transaction) {
            if (ModelState.IsValid) {
                if (transaction.Amount >= 0) {
                    //make sure its always negative
                    transaction.Amount *= -1;
                }
                if (db.CheckingAccounts.Find(transaction.CheckingAccountId).Balance < Math.Abs(transaction.Amount)) {
                    //do they have enough money?
                    ViewBag.Error = "Funds too low";
                    return View();
                } else {
                    db.Transactions.Add(transaction);
                    db.SaveChanges();
                    var service = new CheckingAccountService(db);
                    service.UpdateBalance(transaction.CheckingAccountId);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        public ActionResult QuickCash(int checkingAccountId) {
            return View();
        }

        [HttpPost]
        public ActionResult QuickCash(Transaction transaction) {
            transaction.Amount = -100.00m;
            if (ModelState.IsValid) {
                var test0 = db.CheckingAccounts.Find(transaction.CheckingAccountId).Balance;
                var test1 = Math.Abs(transaction.Amount);
                if (test0 < test1) {
                    //do they have enough money?
                    ViewBag.Error = "Funds too low";
                    return View();
                } else {
                    db.Transactions.Add(transaction);
                    db.SaveChanges();
                    var service = new CheckingAccountService(db);
                    service.UpdateBalance(transaction.CheckingAccountId);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }
    }
}