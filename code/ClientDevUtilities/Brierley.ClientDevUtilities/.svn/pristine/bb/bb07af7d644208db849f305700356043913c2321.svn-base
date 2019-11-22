using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
    public class MGStoreDef
    {
        #region Properties

        /// <summary>
        /// Gets or sets the StoreId for the current StoreDef
        /// </summary>
        public virtual Int64 StoreId { get; set; }

        /// <summary>
        /// Gets or sets the StoreNumber for the current StoreDef
        /// </summary>
        public virtual string StoreNumber { get; set; }

        /// <summary>
        /// Gets or sets the StoreName for the current StoreDef
        /// </summary>
        public virtual String StoreName { get; set; }

        /// <summary>
        /// Gets or sets the BrandName for the current StoreDef
        /// </summary>
        public virtual String BrandName { get; set; }

        /// <summary>
        /// Gets or sets the BrandStoreNumber for the current StoreDef
        /// </summary>
        public virtual String BrandStoreNumber { get; set; }

        /// <summary>
        /// Gets or sets the PhoneNumber for the current StoreDef
        /// </summary>
        public virtual String PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the AddressLineOne for the current StoreDef
        /// </summary>
        public virtual string AddressLineOne { get; set; }

        /// <summary>
        /// Gets or sets the AddressLineTwo for the current StoreDef
        /// </summary>
        public virtual string AddressLineTwo { get; set; }

        /// <summary>
        /// Gets or sets the City for the current StoreDef
        /// </summary>
        public virtual string City { get; set; }

        /// <summary>
        /// Gets or sets the StateOrProvince for the current StoreDef
        /// </summary>
        public virtual string StateOrProvince { get; set; }

        /// <summary>
        /// Gets or sets the ZipOrPostalCode for the current StoreDef
        /// </summary>
        public virtual string ZipOrPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the County for the current StoreDef
        /// </summary>
        public virtual string County { get; set; }

        /// <summary>
        /// Gets or sets the Country for the current StoreDef
        /// </summary>
        public virtual string Country { get; set; }

        /// <summary>
        /// Gets or sets the Status for the current StoreDef
        /// </summary>
        public virtual StoreStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the OpenDate for the current StoreDef
        /// </summary>
        public virtual DateTime? OpenDate { get; set; }

        /// <summary>
        /// Gets or sets the CloseDate for the current StoreDef
        /// </summary>
        public virtual DateTime? CloseDate { get; set; }

        /// <summary>
        /// Gets or sets the CloseDate for the current StoreDef
        /// </summary>
        public virtual double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets the Latitude for the current StoreDef
        /// </summary>
        public virtual double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the Region for the current StoreDef
        /// </summary>
        public virtual String Region { get; set; }

        /// <summary>
        /// Gets or sets the District for the current StoreDef
        /// </summary>
        public virtual String District { get; set; }

        /// <summary>
        /// Gets or sets the Zone for the current StoreDef
        /// </summary>
        public virtual String Zone { get; set; }

        public virtual string StrUserField { get; set; }
        public virtual long? LongUserField { get; set; }

        public virtual double DistanceInKM { set; get; }

        #endregion

        #region Data Transfer Methods
        public static MGStoreDef Hydrate(Brierley.FrameWork.Data.DomainModel.StoreDef stdef)
        {
            MGStoreDef mgStore = new MGStoreDef()
            {
                StoreId = stdef.StoreId,
                StoreNumber = stdef.StoreNumber,
                StoreName = stdef.StoreName,
                BrandName = stdef.BrandName,
                BrandStoreNumber = stdef.BrandStoreNumber,
                PhoneNumber = stdef.PhoneNumber,
                AddressLineOne = stdef.AddressLineOne,
                AddressLineTwo = stdef.AddressLineTwo,
                City = stdef.City,
                StateOrProvince = stdef.StateOrProvince,
                ZipOrPostalCode = stdef.ZipOrPostalCode,
                County = stdef.County,
                Country = stdef.Country,
                Status = stdef.Status,
                OpenDate = stdef.OpenDate,
                CloseDate = stdef.CloseDate,
                Longitude = stdef.Longitude,
                Latitude = stdef.Latitude,
                Region = stdef.Region,
                District = stdef.District,
                Zone = stdef.Zone,
                StrUserField = stdef.StrUserField,
                LongUserField = stdef.LongUserField
            };
            return mgStore;
        }

        public static MGStoreDef[] ConvertFromJson(string contentAttsStr)
        {
            MGStoreDef[] storeList = JsonConvert.DeserializeObject<MGStoreDef[]>(contentAttsStr);
            return storeList;
        }
        #endregion
    }
}