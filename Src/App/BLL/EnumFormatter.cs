using System;


namespace Vulcan.Stypendia.Bll
{
    public class EnumFormatter : IFormatProvider, ICustomFormatter
    {
        private static EnumFormatter formatter;
        public static readonly IFormatProvider Default = GetDefault();

        private static IFormatProvider GetDefault()
        {
            if (formatter == null)
                formatter = new EnumFormatter();

            return formatter;
        }

        #region IFormatProvider Members

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }
            else
            {
                return null;
            }
        }

        #endregion

 //       #region ICustomFormatter Members

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
//            if (arg.GetType() == typeof(UUser.UserStatus))
//                return UserStatusAsString((UUser.UserStatus)arg, format);
//
//            if (arg.GetType() == typeof(SJob.JobState))
//                return JobStateAsString((SJob.JobState)arg, format);
//
//            if (arg.GetType() == typeof(STask.TaskState))
//                return TaskStateAsString((STask.TaskState)arg, format);
//
//            if (arg.GetType() == typeof(SDoer.DoerPriviledge))
//                return DoerPriviledgeAsString((SDoer.DoerPriviledge)arg, format);

            return arg.ToString();
        }
        /*
                static string UserStatusAsString(UUser.UserStatus status, string format)
                {
                    switch (status)
                    {
                        case UUser.UserStatus.Pending: return "Oczekuj¹cy";
                        case UUser.UserStatus.Locked: return "Zablokowany";
                        case UUser.UserStatus.Active:
                        default:
                            return "Aktywny";
                    }
                }

                static string JobStateAsString(SJob.JobState state, string format)
                {
                    switch (state)
                    {
                        case SJob.JobState.Completed: return "Zakoñczone";
                        case SJob.JobState.Pending: return "Trwaj¹ce";
                        case SJob.JobState.Init:
                        default:
                            return "Zainicjowane";
                    }
                }

                static string TaskStateAsString(STask.TaskState state, string format)
                {
                    switch (state)
                    {
                        case STask.TaskState.Completed: return "Zakoñczone";
                        case STask.TaskState.Pending: return "Trwaj¹ce";
                        case STask.TaskState.Init:
                        default:
                            return "Zainicjowane";
                    }
                }

                static string DoerPriviledgeAsString(SDoer.DoerPriviledge p, string format)
                {
                    switch (p)
                    {
                        case SDoer.DoerPriviledge.AdminRW: return "Administrator RW";
                        case SDoer.DoerPriviledge.AdminRO: return "Administrator RO";
                        case SDoer.DoerPriviledge.TaskAdmin: return "Administrator sprawy";
                        case SDoer.DoerPriviledge.User:
                        default:
                            return "U¿ytkownik";
                    }
                }
        */
    //    #endregion
    }
}
