using System;

using Brierley.FrameWork.Common;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_X509Cert")]
    public class X509Cert : LWCoreObjectBase
	{
        [PetaPoco.Column(IsNullable = false)]
		public long ID { get; set; }

        [PetaPoco.Column(Length = 255, IsNullable = false)]
		public string CertName { get; set; }

        [PetaPoco.Column(Length = 512)]
        public string PassTypeID { get; set; }

        [PetaPoco.Column(Length = 20, IsNullable = false, PersistEnumAsString = true)]
		public X509CertType CertType { get; set; }

        [PetaPoco.Column(Length = 255, IsNullable = true)]
		public string CertPassword { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public string Value { get; set; }

        [PetaPoco.Column]
		public bool? IsProduction { get; set; }

		public X509Cert() 
		{ 
			IsProduction = false; 
		}
        
        public X509Certificate2 X509Certificate2
        {
            get
            {
                string certstr = Value;
                byte[] certbytes;
                if (certstr.StartsWith("-----BEGIN CERTIFICATE-----"))
                {
                    certbytes = new UTF8Encoding().GetBytes(certstr);
                }
                else
                {
                    certbytes = Convert.FromBase64String(certstr);
                }

                if (!string.IsNullOrEmpty(CertPassword))
                {
                    string password = CryptoUtil.DecodeUTF8(CertPassword);
                    return new X509Certificate2(certbytes, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
                }
                else
                {
                    return new X509Certificate2(certbytes);
                }
            }
        }

        #region data migration
        public X509Cert Clone()
		{
			return Clone(new X509Cert());
		}

		public X509Cert Clone(X509Cert other)
		{
			other.CertName = CertName;
			other.PassTypeID = PassTypeID;
			other.CertType = CertType;
			other.CertPassword = CertPassword;
			other.Value = Value;
			other.IsProduction = IsProduction;
			return (X509Cert)base.Clone(other);
		}
		#endregion
	}
}