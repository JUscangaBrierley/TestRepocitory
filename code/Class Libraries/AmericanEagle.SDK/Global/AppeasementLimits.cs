using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Logging;
using Brierley.Clients.AmericanEagle.DataModel;

namespace AmericanEagle.SDK.Global
{
    public static class AppeasementLimits
    {
        private static LWLogger logger = LWLoggerManager.GetLogger("AppeasementLimits");
        static TwoKeyDictionary<string, string, int> limitsForAppeasements = new TwoKeyDictionary<string, string, int>();


        static AppeasementLimits()
        {
            try
            {
                // AEO-1881 begin

                //synchrony
                limitsForAppeasements.Add("synchrony", "$10 - $ reward appeasement", 0);
                limitsForAppeasements.Add("synchrony", "$15 - $ reward appeasement", 0);
                limitsForAppeasements.Add("synchrony", "$20 - $ reward appeasement", 0);
                limitsForAppeasements.Add("synchrony", "$30 - $ reward appeasement", 0);
                limitsForAppeasements.Add("synchrony", "$40 - $ reward appeasement", 0);
                limitsForAppeasements.Add("synchrony", "$45 - $ reward appeasement", 0);
                limitsForAppeasements.Add("synchrony", "$50 - $ reward appeasement", 0);
                limitsForAppeasements.Add("synchrony", "$60 - $ reward appeasement", 0);
                limitsForAppeasements.Add("synchrony", "15% - appeasement", 0);
                limitsForAppeasements.Add("synchrony", "15% birthday appeasement", 0);
                limitsForAppeasements.Add("synchrony", "20% - appeasement", 0);
                limitsForAppeasements.Add("synchrony", "20% birthday appeasement", 0);
                limitsForAppeasements.Add("synchrony", "30% - appeasement", 0);
                limitsForAppeasements.Add("synchrony", "40% - appeasement", 0);
                limitsForAppeasements.Add("synchrony", "b5g1 bra appeasement", 0);
                limitsForAppeasements.Add("synchrony", "b5g1 jean appeasement", 0);

                limitsForAppeasements.Add("synchrony", "missing receipt", 0);
                limitsForAppeasements.Add("synchrony", "customer service points adjustment", 0);
                limitsForAppeasements.Add("synchrony", "period adjustment", 0);
                limitsForAppeasements.Add("synchrony", "points correction", 0);
                limitsForAppeasements.Add("synchrony", "points transfer", 0);
                limitsForAppeasements.Add("synchrony", "promo adjustment", 0);
                limitsForAppeasements.Add("synchrony", "bra credit appeasement", 0);
                limitsForAppeasements.Add("synchrony", "jean credit appeasement", 0);

                //synchrony admin
                limitsForAppeasements.Add("synchrony admin", "$10 - $ reward appeasement", 2);
                limitsForAppeasements.Add("synchrony admin", "$15 - $ reward appeasement", 2);
                limitsForAppeasements.Add("synchrony admin", "$20 - $ reward appeasement", 2);
                limitsForAppeasements.Add("synchrony admin", "$30 - $ reward appeasement", 2);
                limitsForAppeasements.Add("synchrony admin", "$40 - $ reward appeasement", 2);
                limitsForAppeasements.Add("synchrony admin", "$45 - $ reward appeasement", 2);
                limitsForAppeasements.Add("synchrony admin", "$50 - $ reward appeasement", 2);
                limitsForAppeasements.Add("synchrony admin", "$60 - $ reward appeasement", 2);
                limitsForAppeasements.Add("synchrony admin", "15% - appeasement", 2);
                limitsForAppeasements.Add("synchrony admin", "15% birthday appeasement", 2);
                limitsForAppeasements.Add("synchrony admin", "20% - appeasement", 2);
                limitsForAppeasements.Add("synchrony admin", "20% birthday appeasement", 2);
                limitsForAppeasements.Add("synchrony admin", "30% - appeasement", 2);
                limitsForAppeasements.Add("synchrony admin", "40% - appeasement", 2);
                limitsForAppeasements.Add("synchrony admin", "b5g1 bra appeasement", 2);
                limitsForAppeasements.Add("synchrony admin", "b5g1 jean appeasement", 2);

                limitsForAppeasements.Add("synchrony admin", "missing receipt", 5000);
                limitsForAppeasements.Add("synchrony admin", "customer service points adjustment", 5000);
                limitsForAppeasements.Add("synchrony admin", "period adjustment", 5000);
                limitsForAppeasements.Add("synchrony admin", "points correction", 5000);
                limitsForAppeasements.Add("synchrony admin", "points transfer", 5000);
                limitsForAppeasements.Add("synchrony admin", "promo adjustment", 5000);
                limitsForAppeasements.Add("synchrony admin", "bra credit appeasement", 2);
                limitsForAppeasements.Add("synchrony admin", "jean credit appeasement", 2);

                //synchrony csr
                limitsForAppeasements.Add("synchrony csr", "$10 - $ reward appeasement", 2);
                limitsForAppeasements.Add("synchrony csr", "$15 - $ reward appeasement", 2);
                limitsForAppeasements.Add("synchrony csr", "$20 - $ reward appeasement", 2);
                limitsForAppeasements.Add("synchrony csr", "$30 - $ reward appeasement", 2);
                limitsForAppeasements.Add("synchrony csr", "$40 - $ reward appeasement", 2);
                limitsForAppeasements.Add("synchrony csr", "$45 - $ reward appeasement", 2);
                limitsForAppeasements.Add("synchrony csr", "$50 - $ reward appeasement", 2);
                limitsForAppeasements.Add("synchrony csr", "$60 - $ reward appeasement", 2);
                limitsForAppeasements.Add("synchrony csr", "15% - appeasement", 2);
                limitsForAppeasements.Add("synchrony csr", "15% birthday appeasement", 2);
                limitsForAppeasements.Add("synchrony csr", "20% - appeasement", 2);
                limitsForAppeasements.Add("synchrony csr", "20% birthday appeasement", 2);
                limitsForAppeasements.Add("synchrony csr", "30% - appeasement", 2);
                limitsForAppeasements.Add("synchrony csr", "40% - appeasement", 2);
                limitsForAppeasements.Add("synchrony csr", "b5g1 bra appeasement", 2);
                limitsForAppeasements.Add("synchrony csr", "b5g1 jean appeasement", 2);

                limitsForAppeasements.Add("synchrony csr", "missing receipt", 5000);
                limitsForAppeasements.Add("synchrony csr", "customer service points adjustment", 5000);
                limitsForAppeasements.Add("synchrony csr", "period adjustment", 5000);
                limitsForAppeasements.Add("synchrony csr", "points correction", 5000);
                limitsForAppeasements.Add("synchrony csr", "points transfer", 5000);
                limitsForAppeasements.Add("synchrony csr", "promo adjustment", 5000);
                limitsForAppeasements.Add("synchrony csr", "bra credit appeasement", 2);
                limitsForAppeasements.Add("synchrony csr", "jean credit appeasement", 2);
                // AEO-1881 end

                //csr
                limitsForAppeasements.Add("csr", "$10 - $ reward appeasement", 2);
                limitsForAppeasements.Add("csr", "$15 - $ reward appeasement", 2);
                limitsForAppeasements.Add("csr", "$20 - $ reward appeasement", 2);
                limitsForAppeasements.Add("csr", "$30 - $ reward appeasement", 2);
                limitsForAppeasements.Add("csr", "$40 - $ reward appeasement", 2);
                limitsForAppeasements.Add("csr", "$45 - $ reward appeasement", 2);
                limitsForAppeasements.Add("csr", "$50 - $ reward appeasement", 2);
                limitsForAppeasements.Add("csr", "$60 - $ reward appeasement", 2);
                limitsForAppeasements.Add("csr", "15% - appeasement", 2);
                limitsForAppeasements.Add("csr", "15% birthday appeasement", 2);
                limitsForAppeasements.Add("csr", "20% - appeasement", 2);
                limitsForAppeasements.Add("csr", "20% birthday appeasement", 2);
                limitsForAppeasements.Add("csr", "30% - appeasement", 2);
                limitsForAppeasements.Add("csr", "40% - appeasement", 2);
                limitsForAppeasements.Add("csr", "b5g1 bra appeasement", 2);
                limitsForAppeasements.Add("csr", "b5g1 jean appeasement", 2);

                limitsForAppeasements.Add("csr", "missing receipt", 5000);
                limitsForAppeasements.Add("csr", "customer service points adjustment", 5000);
                limitsForAppeasements.Add("csr", "period adjustment", 5000);
                limitsForAppeasements.Add("csr", "points correction", 5000);
                limitsForAppeasements.Add("csr", "points transfer", 5000);
                limitsForAppeasements.Add("csr", "promo adjustment", 5000);
                limitsForAppeasements.Add("csr", "bra credit appeasement", 4);
                limitsForAppeasements.Add("csr", "jean credit appeasement", 4);

                //supervisor
                limitsForAppeasements.Add("supervisor", "$10 - $ reward appeasement", 2);
                limitsForAppeasements.Add("supervisor", "$15 - $ reward appeasement", 2);
                limitsForAppeasements.Add("supervisor", "$20 - $ reward appeasement", 2);
                limitsForAppeasements.Add("supervisor", "$30 - $ reward appeasement", 2);
                limitsForAppeasements.Add("supervisor", "$40 - $ reward appeasement", 2);
                limitsForAppeasements.Add("supervisor", "$45 - $ reward appeasement", 2);
                limitsForAppeasements.Add("supervisor", "$50 - $ reward appeasement", 2);
                limitsForAppeasements.Add("supervisor", "$60 - $ reward appeasement", 2);
                limitsForAppeasements.Add("supervisor", "15% - appeasement", 2);
                limitsForAppeasements.Add("supervisor", "15% birthday appeasement", 2);
                limitsForAppeasements.Add("supervisor", "20% - appeasement", 2);
                limitsForAppeasements.Add("supervisor", "20% birthday appeasement", 2);
                limitsForAppeasements.Add("supervisor", "30% - appeasement", 2);
                limitsForAppeasements.Add("supervisor", "40% - appeasement", 2);
                limitsForAppeasements.Add("supervisor", "b5g1 bra appeasement", 2);
                limitsForAppeasements.Add("supervisor", "b5g1 jean appeasement", 2);

                limitsForAppeasements.Add("supervisor", "missing receipt", 5000);
                limitsForAppeasements.Add("supervisor", "customer service points adjustment", 5000);
                limitsForAppeasements.Add("supervisor", "period adjustment", 5000);
                limitsForAppeasements.Add("supervisor", "points correction", 5000);
                limitsForAppeasements.Add("supervisor", "points transfer", 5000);
                limitsForAppeasements.Add("supervisor", "promo adjustment", 5000);
                limitsForAppeasements.Add("supervisor", "bra credit appeasement", 4);
                limitsForAppeasements.Add("supervisor", "jean credit appeasement", 4);

                //super admin
                limitsForAppeasements.Add("super admin", "$10 - $ reward appeasement", -1);
                limitsForAppeasements.Add("super admin", "$15 - $ reward appeasement", -1);
                limitsForAppeasements.Add("super admin", "$20 - $ reward appeasement", -1);
                limitsForAppeasements.Add("super admin", "$30 - $ reward appeasement", -1);
                limitsForAppeasements.Add("super admin", "$40 - $ reward appeasement", -1);
                limitsForAppeasements.Add("super admin", "$45 - $ reward appeasement", -1);
                limitsForAppeasements.Add("super admin", "$50 - $ reward appeasement", -1);
                limitsForAppeasements.Add("super admin", "$60 - $ reward appeasement", -1);
                limitsForAppeasements.Add("super admin", "15% - appeasement", -1);
                limitsForAppeasements.Add("super admin", "15% birthday appeasement", -1);
                limitsForAppeasements.Add("super admin", "20% - appeasement", -1);
                limitsForAppeasements.Add("super admin", "20% birthday appeasement", -1);
                limitsForAppeasements.Add("super admin", "30% - appeasement", -1);
                limitsForAppeasements.Add("super admin", "40% - appeasement", -1);
                limitsForAppeasements.Add("super admin", "b5g1 bra appeasement", -1);
                limitsForAppeasements.Add("super admin", "b5g1 jean appeasement", -1);

                limitsForAppeasements.Add("super admin", "missing receipt", -1);
                limitsForAppeasements.Add("super admin", "customer service points adjustment", -1);
                limitsForAppeasements.Add("super admin", "period adjustment", -1);
                limitsForAppeasements.Add("super admin", "points correction", -1);
                limitsForAppeasements.Add("super admin", "points transfer", -1);
                limitsForAppeasements.Add("super admin", "promo adjustment", -1);
                limitsForAppeasements.Add("super admin", "bra credit appeasement", -1);
                limitsForAppeasements.Add("super admin", "jean credit appeasement", -1);

            }
            catch(Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Appeasement Limits could not be defined: " + ex.Message);
            }
        }

        public static int GetLimitFor(string Role, string LimitName)
        {
            string role = Role.ToLower();
            string limitName = LimitName.ToLower();
            int appeaseLimit = 0;
            if(limitsForAppeasements.ContainsKey(role, limitName))
            {
                appeaseLimit = limitsForAppeasements[role, limitName];
            }

            if(appeaseLimit == -1)
            {
                appeaseLimit = int.MaxValue;
            }

            return appeaseLimit;
        }
    }
    #region Helper Classes

    /// <summary>
    /// Base class that implements a dictionary based on a two values key
    /// </summary>
    public class TwoKeyDictionary<TKey1, TKey2, TValue> : Dictionary<TwoKey<TKey1, TKey2>, TValue>
    {
        public TwoKey<TKey1, TKey2> Key(TKey1 key1, TKey2 key2)
        {
            return new TwoKey<TKey1, TKey2>(key1, key2);
        }

        public TValue this[TKey1 key1, TKey2 key2]
        {
            get { return this[Key(key1, key2)]; }
            set { this[Key(key1, key2)] = value; }
        }

        public void Add(TKey1 key1, TKey2 key2, TValue value)
        {
            Add(Key(key1, key2), value);
        }

        public bool ContainsKey(TKey1 key1, TKey2 key2)
        {
            return ContainsKey(Key(key1, key2));
        }
    }

    /// <summary>
    /// Base tuple class for definig a two value key.
    /// </summary>
    public class TwoKey<TKey1, TKey2> : Tuple<TKey1, TKey2>
    {
        public TwoKey(TKey1 item1, TKey2 item2) : base(item1, item2) { }

        public override string ToString()
        {
            return string.Format("({0},{1})", Item1, Item2);
        }
    }
    
    #endregion
}
