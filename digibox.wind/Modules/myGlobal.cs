using digibox.wind.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.wind.Modules
{
    public class myGlobal
    {
        public static Guid UserID = new Guid("4D41A08C-21B9-44AF-AA46-DADA65092046");
        public static Guid BranchID = new Guid("9AC80D39-ED34-4208-9F2E-142FECA145DE");
        public static Guid SessionID;
        public static string UserName;
        public static string BranchName;

        public static string token { get; set; }

        public static UserModel currentUser
        {
            get;set;
        }

        public static string createToken()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            string token = Convert.ToBase64String(time.Concat(key).ToArray());
            return token;
        }

    }

    public sealed class userRole
    {
        public const string SUPERADMIN = "SUPERADMIN";
        public const string MANAGEMENT = "MANAGEMENT";
        public const string MANAGER = "MANAGER";
        public const string ADMIN = "ADMIN";
        public const string COLLECTOR = "COLLECTOR";
        public const string USER = "USER";
    }

    public abstract class Status
    {
        public static string DRAFT = "DRAFT";
        public static string APPROVED = "APPROVED";
        public static string REJECTED = "REJECTED";
        public static string RECEIVED = "RECEIVED";
    }

    public sealed class PriceStatus : Status
    {
        public static string POSTEDBYCOLLECTOR = "POSTED BY COLLECTOR";
        public static string POSTEDBYADMIN = "POSTED BY ADMIN";
    }

    public sealed class ReplenishStatus : Status
    {
        public static string POSTEDBYCOLLECTOR = "POSTED BY COLLECTOR";
    }
}
