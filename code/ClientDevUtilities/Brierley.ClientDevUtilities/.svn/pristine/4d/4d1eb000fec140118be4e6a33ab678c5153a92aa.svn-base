using System;
using System.Xml.Linq;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("ID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_AWDevice")]
    public class AppleWalletDevice : LWCoreObjectBase
    {
        private enum NotificationFlag
        {
            LoyaltyCards = 1,
            Coupons = 2,
            Rewards = 3,
            Offers = 4,
            Promotions = 5
        }

        private XElement _flagsXml;

        [PetaPoco.Column(IsNullable = false)]
        public long ID { get; set; }

        [PetaPoco.Column(IsNullable = false, Length = 512)]
        public string DeviceID { get; set; }

        [PetaPoco.Column(IsNullable = false, Length = 512)]
        public string PushToken { get; set; }

        [PetaPoco.Column(IsNullable = true, Length = 2000)]
        public string FlagsXml { get; set; }

        public bool NotifyForLoyaltyCards
        {
            get
            {
                bool result = GetFlagValue(NotificationFlag.LoyaltyCards);
                return result;
            }
            set
            {
                SetFlagValue(NotificationFlag.LoyaltyCards, value);
            }
        }

        public bool NotifyForCoupons
        {
            get
            {
                bool result = GetFlagValue(NotificationFlag.Coupons);
                return result;
            }
            set
            {
                SetFlagValue(NotificationFlag.Coupons, value);
            }
        }

        public bool NotifyForRewards
        {
            get
            {
                bool result = GetFlagValue(NotificationFlag.Rewards);
                return result;
            }
            set
            {
                SetFlagValue(NotificationFlag.Rewards, value);
            }
        }

        public bool NotifyForOffers
        {
            get
            {
                bool result = GetFlagValue(NotificationFlag.Offers);
                return result;
            }
            set
            {
                SetFlagValue(NotificationFlag.Offers, value);
            }
        }

        public bool NotifyForPromotions
        {
            get
            {
                bool result = GetFlagValue(NotificationFlag.Promotions);
                return result;
            }
            set
            {
                SetFlagValue(NotificationFlag.Promotions, value);
            }
        }

        public AppleWalletDevice() { _flagsXml = null; }

        private bool GetFlagValue(NotificationFlag notificationFlag)
        {
            InitializeFlags();
            bool result = false;
            result = StringUtils.FriendlyBool(_flagsXml.Element("notification").Attribute(notificationFlag.ToString()).Value);
            return result;
        }

        private void SetFlagValue(NotificationFlag notificationFlag, bool value)
        {
            InitializeFlags();
            _flagsXml.Element("notification").Attribute(notificationFlag.ToString()).Value = value.ToString();
            FlagsXml = _flagsXml.ToString();
        }

        private void InitializeFlags()
        {
            if (_flagsXml == null)
            {
                XElement notification = null;
                if (!string.IsNullOrWhiteSpace(FlagsXml) && FlagsXml.ToLower().StartsWith("<flags>"))
                {
                    _flagsXml = XElement.Parse(FlagsXml);
                    notification = _flagsXml.Element("notification");
                }
                else
                {
                    notification = new XElement("notification");
                    _flagsXml = new XElement("flags", notification);
                }
                foreach (var flag in Enum.GetNames(typeof(NotificationFlag)))
                {
                    XAttribute flagAttr = notification.Attribute(flag);
                    if (flagAttr == null)
                    {
                        notification.Add(new XAttribute(flag, "false"));
                    }
                }
            }
        }
    }
}
