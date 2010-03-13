using System;
using System.Web.Security;
using Bll;
using Wilson.ORMapper;

namespace Roberta.Security
{


    public class MyMembershipProvider : MembershipProvider
    {
        public override string ApplicationName
        {
            get { return "Roberta"; }
            set { throw new Exception("The method or operation is not implemented."); }
        }


        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (ValidateUser(username, oldPassword))
            {
                return true;
            }
            else
            {
                return false;

            }

        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password,
                                                             string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override MembershipUser CreateUser(string username, string password, string email,
                                                  string passwordQuestion, string passwordAnswer, bool isApproved,
                                                  object providerUserKey, out MembershipCreateStatus status)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool EnablePasswordReset
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize,
                                                                  out int totalRecords)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize,
                                                                 out int totalRecords)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string GetPassword(string username, string answer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            return
                new MembershipUser("RepozytoriumMembershipProvider", username, null, null, null, null, true, true,
                                   DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 0; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 6; }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool UnlockUser(string userName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Autentykacja u¿ytkownika.
        /// 
        /// Procedura autentykacji:
        /// 1. sprawdzenie istnienia rekordu o podanym username w bazie danych
        ///    
        ///    a) w wypadku u¿ytkownika AD - sprawdzenie to¿samoœci w AD
        ///    b) w wypadku u¿ytkownika bazy danych - porównanie skrótu has³a z zapamietanym w bazie danych
        /// </summary>
        /// <param name="username">Nazwa u¿ytkownika (w przypadku u¿ytkowników AD w formacie [domain\user]</param>
        /// <param name="password">Has³o</param>
        /// <returns>Zwraca informacjê o poprawnoœci autentykacji u¿ytkownika.</returns>
        public override bool ValidateUser(string username, string password)
        {
            return ValidateUser(username, password, true);
        }

        public bool ValidateUser(string username, string password, bool storeAttempt)
        {
            User user = Dm.GetObject<User>("Login==? && Password==?", new object[] { username, password });
            return user == null ? false : true;

            
        }
    }


    
}


