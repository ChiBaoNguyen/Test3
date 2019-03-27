using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Root.Helper;

namespace Website.Utilities
{
	public interface ILicenseValidation
	{
		bool CheckLicense(string licensePath, int currTruckTotal, string hashStrKey);
		bool CheckCustomerLicense(string licensePath, string token, string hashStrKey);
		int GetTruckLimitation(string licensePath);
		LicenseInfo GetLicenseInfo(string licensePath);
	}

	public class LicenseValidation : ILicenseValidation
	{
		protected static readonly ILog log = LogManager.GetLogger(typeof(LicenseValidation));
		public bool CheckLicense(string licensePath, int currTruckTotal, string hashStrKey)
		{
			var customerCode = "";
			var truckQuantity = "";
			var serialNo = "";
			if (!IsFileValid(licensePath))
			{
				return false;
			}
			using (var file = new StreamReader(licensePath))
			{
				string line;
				while ((line = file.ReadLine()) != null)
				{
					if (line.Contains("Customer Code"))
					{
						customerCode = line.Split(':')[1].Trim();
					}
					if (line.Contains("Truck Quantity"))
					{
						truckQuantity = line.Split(':')[1].Trim();

					}
					if (line.Contains("Serial No"))
					{
						serialNo = line.Split(':')[1].Trim();
					}
				}
				
				var companySerial = Encrypt(customerCode + truckQuantity, hashStrKey);
				log.Info("companySerial: " + companySerial);
				log.Info("serialNo: " + serialNo);
				//license truck quantity must be equal or greater than currTruckQuantity
				if (Convert.ToInt32(truckQuantity) >= currTruckTotal && companySerial.Equals(serialNo))
				{
					log.Info("Check License for request: true");
					return true;
				}
				log.Info("Check License for request: false");
				return false;
			}
		}

		public bool CheckCustomerLicense(string licensePath, string token, string hashStrKey)
		{
			var serverLicense = "";
			if (!IsFileValid(licensePath))
			{
				return false;
			}
			using (var file = new StreamReader(licensePath))
			{
				string line;
				while ((line = file.ReadLine()) != null)
				{
					if (line.Contains("Serial No"))
					{
						serverLicense = line.Split(':')[1].Trim();
					}
				}

				var serial = Decrypt(token, hashStrKey);
				var custLicense = serial.Split('|')[0];
				var licenseExpiredD = serial.Split('|')[1];

				var a = DateTime.Parse(licenseExpiredD).Date;

				if (custLicense.Equals(serverLicense))
				{
					if (DateTime.Now.Date > a)
					{
						return false;
					}
					return true;
				}
				return false;
			}
		}

		public int GetTruckLimitation(string licensePath)
		{
			var licenseTruckQuantity = 0;
			using (var file = new StreamReader(licensePath))
			{
				string line;
				while ((line = file.ReadLine()) != null)
				{
					if (line.Contains("Truck Quantity"))
					{
						licenseTruckQuantity = Convert.ToInt32(line.Split(':')[1].Trim());
					}
				}
			}
			return licenseTruckQuantity;
		}

		public LicenseInfo GetLicenseInfo(string licensePath)
		{
			var licenseTruckQuantity = 0;
			var licenseCustomerName = "";
			using (var file = new StreamReader(licensePath))
			{
				string line;
				while ((line = file.ReadLine()) != null)
				{
					if (line.Contains("Truck Quantity"))
					{
						licenseTruckQuantity = Convert.ToInt32(line.Split(':')[1].Trim());
					}
					if (line.Contains("Customer Name"))
					{
						licenseCustomerName = line.Split(':')[1].Trim();
					}
				}
			}
			return new LicenseInfo()
			{
				LicenseCustomerName = licenseCustomerName,
				LicenseTruckLimitation = licenseTruckQuantity
			};
		}

		private bool IsFileValid(string filePath)
		{
			bool isValid = true;

			if (!File.Exists(filePath))
			{
				isValid = false;
			}
			else
			{
				var extension = Path.GetExtension(filePath);
				if (extension != null && extension.ToLower() != ".lic")
				{
					isValid = false;
				}
			}
			return isValid;
		}

		static string GetMacAddress()
		{
			string macAddresses = "";
			try
			{
				foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
				{
					// Only consider Ethernet network interfaces, thereby ignoring any
					// loopback devices etc.
					if (nic.NetworkInterfaceType != NetworkInterfaceType.Ethernet) continue;
					if (nic.OperationalStatus == OperationalStatus.Up)
					{
						macAddresses += nic.GetPhysicalAddress().ToString();
						break;
					}
				}
				return macAddresses;
			}
			catch (Exception ex)
			{
				log.Info("MacAddress exception: " + ex);
				return "error";
			}
		}

		static string GetHddId()
		{
			var serial = "";
			try
			{
				var mos = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_DiskDrive");
				var moc = mos.Get();

				foreach (ManagementObject mo in moc)
				{
					serial = mo["SerialNumber"].ToString();
					break;
				}
				return serial;
			}
			catch (Exception ex)
			{
				log.Info("HDD exception: " + ex);
				return "error";
			}
		}

		static string Encrypt(string strToEncrypt, string hashStrKey)
		{
			try
			{
				var strKey = DecryptCustomerKey(hashStrKey);
				var macAddressId = GetMacAddress();
				var hddId = GetHddId();
				log.Info("macAddressId: " + macAddressId);
				log.Info("hddId: " + hddId);
				if (macAddressId.Equals("error"))
				{
					return "Can not get MacAddress Id";
				}

				if (hddId.Equals("error"))
				{
					return "Can not get HDD Id";
				}

				strToEncrypt += macAddressId + hddId;
				var objDesCrypto = new TripleDESCryptoServiceProvider();
				var objHashMd5 = new MD5CryptoServiceProvider();

				string strTempKey = strKey;

				byte[] byteHash = objHashMd5.ComputeHash(Encoding.ASCII.GetBytes(strTempKey));
				objHashMd5 = null;
				objDesCrypto.Key = byteHash;
				objDesCrypto.Mode = CipherMode.ECB; //CBC, CFB

				byte[] byteBuff = Encoding.ASCII.GetBytes(strToEncrypt);
				
				return Convert.ToBase64String(objDesCrypto.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));

			}
			catch (Exception ex)
			{
				return "Wrong Input. " + ex.Message;
			}
		}

		public string Decrypt(string strEncrypted, string hashStrKey)
		{
			try
			{
				var strKey = DecryptCustomerKey(hashStrKey);
				var objDesCrypto = new TripleDESCryptoServiceProvider();
				var objHashMd5 = new MD5CryptoServiceProvider();

				var strTempKey = strKey;
				var byteHash = objHashMd5.ComputeHash(Encoding.ASCII.GetBytes(strTempKey));
				objHashMd5 = null;
				objDesCrypto.Key = byteHash;
				objDesCrypto.Mode = CipherMode.ECB; //CBC, CFB

				var byteBuff = Convert.FromBase64String(strEncrypted);
				var strDecrypted = Encoding.ASCII.GetString(objDesCrypto.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
				objDesCrypto = null;

				return strDecrypted;
			}
			catch (Exception ex)
			{
				return "error";
			}
		}

		static string DecryptCustomerKey(string cypherText)
		{
			
			byte[] b = Convert.FromBase64String(cypherText);
			TripleDES des = CreateDES();
			ICryptoTransform ct = des.CreateDecryptor();
			byte[] output = ct.TransformFinalBlock(b, 0, b.Length);
			return Encoding.Unicode.GetString(output);
		}

		static TripleDES CreateDES()
		{
			var key = "BGVN@321654$";
			MD5 md5 = new MD5CryptoServiceProvider();
			TripleDES des = new TripleDESCryptoServiceProvider();
			des.Key = md5.ComputeHash(Encoding.Unicode.GetBytes(key));
			des.IV = new byte[des.BlockSize / 8];
			return des;
		}
	}

	public class LicenseInfo
	{
		public string LicenseCustomerName { get; set; }
		public int LicenseTruckLimitation { get; set; }
	}
}
