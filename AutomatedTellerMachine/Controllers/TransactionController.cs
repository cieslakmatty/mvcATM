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
            if (db.CheckingAccounts.Find(transaction.CheckingAccountId).Balance < Math.Abs(transaction.Amount)) {
                //do they have enough money?
                ModelState.AddModelError("Amount", "You've insufficient funds!");
            }
            if (ModelState.IsValid) {
                if (transaction.Amount >= 0) {
                    //make sure its always negative
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

        public ActionResult QuickCash(int checkingAccountId) {
            return View();
        }

        [HttpPost]
        public ActionResult QuickCash(Transaction transaction) {
            transaction.Amount = -100.00m;
            if (db.CheckingAccounts.Find(transaction.CheckingAccountId).Balance < Math.Abs(transaction.Amount)) {
                //do they have enough money?
                ModelState.AddModelError("", "You've insufficient funds!");
            }
            if (ModelState.IsValid) {
                db.Transactions.Add(transaction);
                db.SaveChanges();
                var service = new CheckingAccountService(db);
                service.UpdateBalance(transaction.CheckingAccountId);
                return RedirectToAction("Index", "Home");

            }
            return View();
        }

        public ActionResult Transfer(int checkingAccountId) {
            return View();
        }


        [HttpPost]
        public ActionResult Transfer(Transaction tempTransaction) {
            //tempTransaction stores TO checkingAccountId

            //get fromCheckingAccount
            var UserId = User.Identity.GetUserId();
            var fromCheckingAccount = db.CheckingAccounts.Where(c => c.ApplicationUserId == UserId).First();

            if (db.CheckingAccounts.Find(fromCheckingAccount.Id).Balance < Math.Abs(tempTransaction.Amount)) {
                //do they have enough money?
                ModelState.AddModelError("Amount", "You've insufficient funds!");

            }

            if (db.CheckingAccounts.Find(tempTransaction.CheckingAccountId) == null) {
                //does other account exist?
                ModelState.AddModelError("CheckingaccountId", "That account does not exist!");
            }

            if (fromCheckingAccount.Id.Equals(tempTransaction.CheckingAccountId)) {
                //cant send money to yourself
                ModelState.AddModelError("CheckingaccountId", "You cannot send money to yourself!");
            }
            if (ModelState.IsValid) {
                //get toCheckingAccount
                var toCheckingAccount = db.CheckingAccounts.Find(tempTransaction.CheckingAccountId);

                //create fromTransacation and toTransaction
                var toTransaction = new Transaction {
                    Amount = tempTransaction.Amount,
                    CheckingAccountId = toCheckingAccount.Id,
                    CheckingAccount = toCheckingAccount
                };
                var fromTransaction = new Transaction {
                    Amount = tempTransaction.Amount,
                    CheckingAccountId = fromCheckingAccount.Id,
                    CheckingAccount = fromCheckingAccount
                };

                if (toTransaction.Amount <= 0) {
                    //make sure TO is always positive
                    toTransaction.Amount *= -1;
                }
                if (fromTransaction.Amount >= 0) {
                    //make sure FROM is always negative
                    fromTransaction.Amount *= -1;
                }
                //add to the other acc
                db.Transactions.Add(toTransaction);
                //add to curr acc
                db.Transactions.Add(fromTransaction);
                db.SaveChanges();
                var service = new CheckingAccountService(db);
                service.UpdateBalance(toTransaction.CheckingAccountId);
                service.UpdateBalance(fromTransaction.CheckingAccountId);
                return RedirectToAction("Index", "Home");

            }
            return View();
        }
    }
}